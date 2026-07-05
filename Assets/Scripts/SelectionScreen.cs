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

    void Start() {
        for (int index = 0; index < optionButtons.Length; index++) {
            int capturedIndex = index;
            optionButtons[index].onClick.AddListener(delegate {
                OnOptionClicked(capturedIndex);
            });
        }
    }

    void OnOptionClicked(int index) {
        if (selectionKind == SelectionKind.Character) {
            GameSelection.CharacterIndex = index;
        }
        else {
            GameSelection.MapIndex = index;
        }
        SceneManager.LoadScene(nextSceneName);
    }
}
