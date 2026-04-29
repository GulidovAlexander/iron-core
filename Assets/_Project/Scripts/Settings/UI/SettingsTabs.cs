using UnityEngine;

namespace Game.Scripts.Settings.UI
{
    public class SettingsTabs: MonoBehaviour
    {
        [SerializeField] private GameObject _displayTab;
        [SerializeField] private GameObject _qualityTab;
        [SerializeField] private GameObject _audioTab;
        [SerializeField] private GameObject _gameplayTab;
        [SerializeField] private GameObject _controlsTab;
        
        private GameObject[] _allTabs;
        
        private void Awake()
        {
            _allTabs = new[] { _displayTab, _qualityTab ,_audioTab, _gameplayTab, _controlsTab };
            ShowDisplay();
        }
        
        public void ShowDisplay() => Show(0);
        public void ShowQuality() => Show(1);
        public void ShowAudio() => Show(2);
        public void ShowGameplay() => Show(3);
        public void ShowControls() => Show(4);
        
        private void Show(int index)
        {
            for (int i = 0; i < _allTabs.Length; i++)
            {
                if (_allTabs[i] != null)
                    _allTabs[i].SetActive(i == index);
            }
        }
    }
}