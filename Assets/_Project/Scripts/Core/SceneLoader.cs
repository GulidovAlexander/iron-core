using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private TextMeshProUGUI _progressText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(_loadingScreen);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if(_loadingScreen)
            _loadingScreen.SetActive(true);
        
        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        UpdateProgress(0f);
        
        while (operation.progress < 0.9f)
        {
            UpdateProgress(operation.progress);
            yield return null;
        }
        
        UpdateProgress(1f);
        
        yield return new  WaitForSeconds(0.5f);
        
        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }
        
        if(_loadingScreen)
            _loadingScreen.SetActive(false);
    }

    private void UpdateProgress(float progress)
    {
        if(_progressBar)
            _progressBar.value = progress;
        
        if(_progressText)
            _progressText.text = $"Loading ... {Mathf.RoundToInt(progress * 100)}%";
    }
}
