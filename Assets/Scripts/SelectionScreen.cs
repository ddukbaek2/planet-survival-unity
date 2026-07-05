using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectionScreen : MonoBehaviour {
    public enum SelectionKind {
        Character,
        Map
    }

    [SerializeField] private SelectionKind selectionKind = SelectionKind.Character;
    [SerializeField] private string nextSceneName = "MapSelect";
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private Button confirmButton;

    private int selectedIndex = -1;

    void Start() {
        for (int index = 0; index < optionButtons.Length; index++) {
            int capturedIndex = index;
            optionButtons[index].onClick.AddListener(delegate {
                OnOptionClicked(capturedIndex);
            });
        }
        if (confirmButton != null) {
            confirmButton.onClick.AddListener(OnConfirmClicked);
            confirmButton.interactable = false;
        }
        UpdateHighlight();
    }

    void OnOptionClicked(int index) {
        selectedIndex = index;
        UpdateHighlight();
        if (confirmButton != null) {
            confirmButton.interactable = true;
        }
    }

    void UpdateHighlight() {
        for (int index = 0; index < optionButtons.Length; index++) {
            float scale = index == selectedIndex ? 1.12f : 1f;
            optionButtons[index].transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    void OnConfirmClicked() {
        if (selectedIndex < 0) {
            return;
        }
        if (selectionKind == SelectionKind.Character) {
            GameSelection.CharacterIndex = selectedIndex;
        }
        else {
            GameSelection.MapIndex = selectedIndex;
        }
        SceneManager.LoadScene(nextSceneName);
    }
}
