using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class TitleController : MonoBehaviour {
    [SerializeField] private TMP_Text versionText;
    [SerializeField] private string gameSceneName = "Game";

    void Start() {
        TextAsset versionAsset = Resources.Load<TextAsset>("version");
        if (versionText != null) {
            versionText.text = versionAsset != null ? versionAsset.text : "dev";
        }
    }

    void Update() {
        Pointer pointer = Pointer.current;
        if (pointer == null) {
            return;
        }
        if (pointer.press.wasPressedThisFrame) {
            SceneManager.LoadScene(gameSceneName);
        }
    }
}
