using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Scripts.UI.Menus
{
    public class MainMenuUI : MonoBehaviour
    {
        private VisualElement rootUiDocument;

        private Button newGameButton;

        private void Start()
        {
            rootUiDocument = GetComponent<UIDocument>().rootVisualElement;

            newGameButton = rootUiDocument.Q<Button>("NewGame");

            newGameButton.RegisterCallback<ClickEvent>(NewGameButtonClicked);
        }

        private void OnDestroy()
        {
            newGameButton.UnregisterCallback<ClickEvent>(NewGameButtonClicked);
        }

        private void NewGameButtonClicked(ClickEvent evt)
        {
            Debug.Log("New Game");
        }
    }
}