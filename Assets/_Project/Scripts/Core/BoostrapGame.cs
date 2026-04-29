using UnityEngine;

public class BoostrapGame : MonoBehaviour
{
   private void Start()
   {
      SceneLoader.Instance.LoadScene("MainMenu");
   }
}
