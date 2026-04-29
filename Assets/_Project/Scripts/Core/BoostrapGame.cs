using System;
using Game.Scripts.Settings;
using UnityEngine;

public class BoostrapGame : MonoBehaviour
{
   private void Awake()
   {
      GameSettings.Initialize();
   }

   private void Start()
   {
      SceneLoader.Instance.LoadScene("MainMenu");
   }
}
