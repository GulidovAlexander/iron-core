#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

namespace MarieIT.MaskMapGenerator
{
    public class HDRPMaskMapGenerator : EditorWindow
    {
        private enum Tab { MaskMapGenerator, TextureManipulator}
        
        [Header("Textures")]
        private Texture2D sourceTexture;
        private Texture2D generatedPreview;

        [Header("Vectors")]
        private Vector2 scrollPosition;

        [Header("Saving Options")]
        private bool saveMetallic = true;
        private bool saveAO = true;
        private bool saveDetail = true;
        private bool saveSmoothness = true;
        // Normal Map 
        private bool saveNormalMap = true;
        private float normalStrength = 1.0f;

        [Header("Destination")]
        private string destinationFolderRelative = ""; // Main Folder Path
        private string subfolderName = "";             // Subfolder Path
        private bool createSubFolder = false;

        // Creating the Editor Menu
        [MenuItem("Tools/MarieIT/Mask Map Generator")]
        public static void ShowWindow() { GetWindow<HDRPMaskMapGenerator>("HDRP Mask Map Generator"); }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(destinationFolderRelative)) 
                destinationFolderRelative = "Assets/MarieIT/HDRP Mask Map Generator/Demo/Textures/MaskMaps";

            if (normalStrength <= 0) normalStrength = 1.0f;

            if (string.IsNullOrEmpty(subfolderName))
                subfolderName = "MaskMaps_" + DateTime.Now.ToString("yyyyMMdd");
       }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawHeader();
            DrawTextureGenerator();

            EditorGUILayout.EndScrollView();
        }

        #region Main GUI

        private void DrawHeader()
        {
            EditorGUILayout.LabelField("HDRP Mask Map Generator v1.1.0", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "This Tool creates a Mask Map from a single texture and optionally generates/saves supporting textures (Metallic, AO, Detail, Smoothness).\n\n" +
                "Key improvements: progress + cancel for large textures, choose which textures to save, pick destination folder inside your project, and organized subfolder per source texture.", MessageType.Info);
            EditorGUILayout.Space(8);
        }

        private void DrawTextureGenerator()
        {
            EditorGUILayout.LabelField("Source Texture", EditorStyles.boldLabel);
            sourceTexture = (Texture2D)EditorGUILayout.ObjectField("Source Texture", sourceTexture, typeof(Texture2D), false);

            EditorGUILayout.Space(6);

            EditorGUILayout.LabelField("Which generated textures should be saved to disk?", EditorStyles.boldLabel);
            saveMetallic = EditorGUILayout.ToggleLeft("Save Metallic (R)", saveMetallic);
            saveAO = EditorGUILayout.ToggleLeft("Save AO (G)", saveAO);
            saveDetail = EditorGUILayout.ToggleLeft("Save Detail (B)", saveDetail);
            saveSmoothness = EditorGUILayout.ToggleLeft("Save Smoothness (A)", saveSmoothness);
            
            EditorGUILayout.Space(5);
            saveNormalMap = EditorGUILayout.ToggleLeft("Save Normal Map", saveNormalMap);
            if (saveNormalMap)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox(
                    "Generates a normal map based on the texture’s brightness values (height map). " +
                    "Higher strength = more pronounced surface detail. Recommend: 1.0-2.0", 
                    MessageType.Info);
                normalStrength = EditorGUILayout.Slider("Normal Map Strength", normalStrength, 0.1f, 5.0f);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(6);

            DrawFolderSelector("Destination Folder", ref destinationFolderRelative);

            EditorGUILayout.Space(6);
            if (GUILayout.Button($"Create Subfolder - {(createSubFolder ? "Activated" : "Not activated")}", GUILayout.Width(250))) createSubFolder = createSubFolder ? false : true;

            if (createSubFolder) 
            {
                EditorGUILayout.LabelField("Subfolder (optional)", EditorStyles.boldLabel);
                subfolderName = EditorGUILayout.TextField(subfolderName);
            }

            EditorGUILayout.Space(10);

            GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
            if (GUILayout.Button("Generate Texture/s", GUILayout.Height(40)))
            {
                if (ValidateInput())
                {
                    try { ProcessTextures(); }
                    catch (OperationCanceledException)
                    {
                        EditorUtility.DisplayDialog("Cancelled", "Texture generation was cancelled by the user.", "OK");
                    }
                    catch (Exception ex)
                    {
                        EditorUtility.DisplayDialog("Error", "An unexpected error occurred: " + ex.Message, "OK");
                    }
                }
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(10);

            DrawPreview("Mask Map Preview", generatedPreview);
        }

        #endregion

        #region Helpers

        #region Validaters

        private bool ValidateInput()
        {
            if (sourceTexture == null)
            {
                EditorUtility.DisplayDialog("Missing Texture", "Please assign a Source Texture before starting.", "OK");
                return false;
            }

            if (string.IsNullOrEmpty(destinationFolderRelative) || !destinationFolderRelative.StartsWith("Assets"))
            {
                EditorUtility.DisplayDialog("Invalid Destination", "Destination folder must be inside the project's Assets folder. Set it before continuing.", "OK");
                return false;
            }

            if (sourceTexture.width * sourceTexture.height > 4096 * 4096)
            {
                return EditorUtility.DisplayDialog("Large Texture Warning",
                    "This texture is very large and may take a while to process. Continue?",
                    "Yes", "Cancel");
            }

            return true;
        }

        #endregion

        #region Generation

        private void ProcessTextures()
        {
            if (!MakeTextureReadable(sourceTexture)) return;

            string sourceName = Path.GetFileNameWithoutExtension(sourceTexture.name);
            string destBase = destinationFolderRelative.Replace("\\", "/");

            if (!AssetDatabase.IsValidFolder(destBase))
                CreateFolderRecursively(destBase);
            
            string finalDest = destBase;
            if (!string.IsNullOrEmpty(subfolderName) && createSubFolder)
            {
                finalDest = Path.Combine(destBase, subfolderName).Replace("\\", "/");
                if (!AssetDatabase.IsValidFolder(finalDest))
                    CreateFolderRecursively(finalDest);
            }

            string perSourceFolder = Path.Combine(finalDest, sourceName).Replace("\\", "/");
            if (!AssetDatabase.IsValidFolder(perSourceFolder))
                CreateFolderRecursively(perSourceFolder);

            EditorUtility.DisplayCancelableProgressBar("Generating Textures", "Processing...", 0f);

            int width = sourceTexture.width;
            int height = sourceTexture.height;
            Color32[] originalPixels = sourceTexture.GetPixels32();

            if (originalPixels == null || originalPixels.Length != width * height)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Error", "Failed to read pixels from the source texture.", "OK");
                return;
            }

            Color32[] maskPixels = new Color32[originalPixels.Length];
            Color32[] metallicPixels = saveMetallic ? new Color32[originalPixels.Length] : null;
            Color32[] aoPixels = saveAO ? new Color32[originalPixels.Length] : null;
            Color32[] detailPixels = saveDetail ? new Color32[originalPixels.Length] : null;
            Color32[] smoothPixels = saveSmoothness ? new Color32[originalPixels.Length] : null;

            for (int y = 0; y < height; y++)
            {
                int rowStart = y * width;
                for (int x = 0; x < width; x++)
                {
                    int i = rowStart + x;
                    Color32 p = originalPixels[i];

                    int grey = (int)(0.299f * p.r + 0.587f * p.g + 0.114f * p.b);
                    byte greyByte = (byte)Mathf.Clamp(grey, 0, 255);

                    byte metallicByte = 0;
                    byte aoByte = (byte)(255 - greyByte);
                    byte detailByte = greyByte;
                    byte smoothByte = greyByte;

                    maskPixels[i] = new Color32(metallicByte, aoByte, detailByte, smoothByte);

                    if (saveMetallic) metallicPixels[i] = new Color32(metallicByte, metallicByte, metallicByte, 255);
                    if (saveAO) aoPixels[i] = new Color32(aoByte, aoByte, aoByte, 255);
                    if (saveDetail) detailPixels[i] = new Color32(detailByte, detailByte, detailByte, 255);
                    if (saveSmoothness) smoothPixels[i] = new Color32(smoothByte, smoothByte, smoothByte, 255);
                }

                float progress = (float)y / height * 0.8f;
                if (EditorUtility.DisplayCancelableProgressBar("Generating Textures", $"Row {y+1}/{height}", progress))
                {
                    EditorUtility.ClearProgressBar();
                    throw new OperationCanceledException();
                }
            }

            Texture2D maskMap = new Texture2D(width, height, TextureFormat.RGBA32, false);
            maskMap.SetPixels32(maskPixels);
            maskMap.Apply(false, false);
            generatedPreview = maskMap;
            Repaint();

            string maskPath = Path.Combine(perSourceFolder, sourceName + "_MaskMap.png").Replace("\\", "/");
            WriteTextureToPngAndImport(maskMap, maskPath, false, false);
            DestroyImmediate(maskMap);

            if (saveMetallic) SaveTexture(metallicPixels, width, height, perSourceFolder, sourceName + "_Metallic.png");
            if (saveAO) SaveTexture(aoPixels, width, height, perSourceFolder, sourceName + "_AO.png");
            if (saveDetail) SaveTexture(detailPixels, width, height, perSourceFolder, sourceName + "_Detail.png");
            if (saveSmoothness) SaveTexture(smoothPixels, width, height, perSourceFolder, sourceName + "_Smoothness.png");

            if (saveNormalMap)
            {
                Texture2D normalMap = GenerateNormalMap(sourceTexture, normalStrength);
                string normalPath = Path.Combine(perSourceFolder, sourceName + "_Normal.png").Replace("\\", "/");
                SaveNormalMap(normalMap, normalPath);
                DestroyImmediate(normalMap);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success!", $"Mask Map Saved to:\n{perSourceFolder}", "OK");

            maskPixels = null;
            metallicPixels = null;
            aoPixels = null;
            detailPixels = null;
            smoothPixels = null;
            originalPixels = null;

            GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        private Texture2D GenerateNormalMap(Texture2D source, float strength)
        {
            int width = source.width;
            int height = source.height;
            Color[] sourcePixels = source.GetPixels();
            Color[] normalPixels = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                if (y % 10 == 0)
                {
                    float progress = 0.8f + (0.2f * y / height);
                    if (EditorUtility.DisplayCancelableProgressBar("Generating Textures",
                        $"Generating Normal Map... {y}/{height}", progress))
                    {
                        EditorUtility.ClearProgressBar();
                        throw new OperationCanceledException();
                    }
                }

                for (int x = 0; x < width; x++)
                {
                    float left = GetHeightValue(sourcePixels, x - 1, y, width, height);
                    float right = GetHeightValue(sourcePixels, x + 1, y, width, height);
                    float up = GetHeightValue(sourcePixels, x, y + 1, width, height);
                    float down = GetHeightValue(sourcePixels, x, y - 1, width, height);

                    // Gradient calculation
                    float dx = (right - left) * strength;
                    float dy = (up - down) * strength;
                    
                    // Normal Vector
                    float length = Mathf.Sqrt(dx * dx + dy * dy + 1.0f);
                    float nx = -dx / length;
                    float ny = -dy / length;
                    float nz = 1.0f / length;

                    // Convert to color (0-1 range to 0-255 in texture)
                    normalPixels[y * width + x] = new Color(
                        nx * 0.5f + 0.5f,
                        ny * 0.5f + 0.5f,
                        nz * 0.5f + 0.5f,
                        1.0f
                    );
                }
            }

            Texture2D normalMap = new Texture2D(width, height, TextureFormat.RGBA32, true);
            normalMap.SetPixels(normalPixels);
            normalMap.Apply();

            return normalMap;
        }

        private void WriteTextureToPngAndImport(Texture2D tex, string assetRelativePath, bool isSRGB, bool makeReadable)
        {
            string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
            string absPath = Path.Combine(projectPath, assetRelativePath).Replace("\\", "/");
            string dir = Path.GetDirectoryName(absPath).Replace("\\", "/");
            
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllBytes(absPath, tex.EncodeToPNG());
            
            string rel = assetRelativePath.StartsWith("Assets") ? assetRelativePath : ("Assets" + absPath.Substring(Application.dataPath.Length));
            rel = rel.Replace("\\", "/");

            AssetDatabase.ImportAsset(rel);

            TextureImporter importer = AssetImporter.GetAtPath(rel) as TextureImporter;
            if (importer != null)
            {
                importer.sRGBTexture = isSRGB;
                importer.isReadable = makeReadable;
                importer.textureType = TextureImporterType.Default;
                importer.SaveAndReimport();
            }
        }

        #endregion

        #region Saving

        private void SaveTexture(Color32[] pixels, int w, int h, string folder, string name)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.SetPixels32(pixels);
            tex.Apply(false, false);
            string path = Path.Combine(folder, name).Replace("\\", "/");
            WriteTextureToPngAndImport(tex, path, true, false);
            DestroyImmediate(tex);
        }

        private void SaveNormalMap(Texture2D normalMap, string assetRelativePath)
        {
            string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
            string absPath = Path.Combine(projectPath, assetRelativePath).Replace("\\", "/");
            string dir = Path.GetDirectoryName(absPath).Replace("\\", "/");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllBytes(absPath, normalMap.EncodeToPNG());

            string rel = assetRelativePath.StartsWith("Assets") ? assetRelativePath : ("Assets" + absPath.Substring(Application.dataPath.Length));
            rel = rel.Replace("\\", "/");

            AssetDatabase.ImportAsset(rel);

            TextureImporter importer = AssetImporter.GetAtPath(rel) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.NormalMap;
                importer.isReadable = false;
                importer.textureCompression = TextureImporterCompression.Compressed;
                importer.SaveAndReimport();
            }
        }

        #endregion

        #region Getters

        private float GetHeightValue(Color[] pixels, int x, int y, int width, int height)
        {
            x = Mathf.Clamp(x, 0, width - 1);
            y = Mathf.Clamp(y, 0, height - 1);
            Color pixel = pixels[y * width + x];
            return pixel.grayscale;
        }

        #endregion

        #region Texture Setting

        private bool MakeTextureReadable(Texture2D texture)
        {
            if (texture.isReadable) return true;

            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.isReadable = true;
                importer.textureCompression = TextureImporterCompression.Uncompressed; // avoid compression artifacts for processing
                importer.SaveAndReimport();
                AssetDatabase.Refresh();
                return true;
            }

            EditorUtility.DisplayDialog("Error", $"Couldn't make the Texture '{texture.name}' readable. Please activate it manually in the Import Settings.", "OK");
            return false;
        }

        #endregion

        #region Creator

        private void CreateFolderRecursively(string relativePath)
        {
            relativePath = relativePath.Replace("\\", "/");
            if (AssetDatabase.IsValidFolder(relativePath)) return;

            string[] parts = relativePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            string current = parts[0]; // should be "Assets"
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }
                current = next;
            }
        }

        #endregion

        #endregion

        #region Utility

        private void DrawFolderSelector(string label, ref string folderPath)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            folderPath = EditorGUILayout.TextField(folderPath);
            if (GUILayout.Button("Choose...", GUILayout.Width(80)))
            {
                string abs = EditorUtility.OpenFolderPanel("Select folder (must be inside Assets)", Application.dataPath, "");
                if (!string.IsNullOrEmpty(abs))
                {
                    string dataPath = Application.dataPath.Replace("\\", "/");
                    abs = abs.Replace("\\", "/");
                    if (abs.StartsWith(dataPath, StringComparison.OrdinalIgnoreCase))
                        folderPath = "Assets" + abs.Substring(dataPath.Length);
                    else 
                    {
                        EditorUtility.DisplayDialog("Invalid Folder", "Please choose a folder inside Assets", "OK");
                        folderPath = "Assets/Generated";
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPreview(string label, Texture2D preview)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            if (preview != null)
            {
                float maxWidth = Mathf.Max(64f, position.width - 40f);
                float aspect = (float)preview.width / Mathf.Max(1, preview.height);
                float height = Mathf.Min(512f, maxWidth / aspect);
                Rect rect = EditorGUILayout.GetControlRect(false, height);
                EditorGUI.DrawPreviewTexture(rect, preview, null, ScaleMode.ScaleToFit);
            }
            else
            {
                Rect rect = EditorGUILayout.GetControlRect(false, 64f);
                EditorGUI.LabelField(rect, "Preview will appear here after processing.");
            }
        }

        #endregion

        private void OnDestroy()
        {
            if (generatedPreview != null) { DestroyImmediate(generatedPreview); }
        }
    }
}
#endif
