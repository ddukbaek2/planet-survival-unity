using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleController : MonoBehaviour {
    [SerializeField] private TMP_Text versionText;
    [SerializeField] private string nextSceneName = "CharacterSelect";
    [SerializeField] private Button stageModeButton;
    [SerializeField] private Button bossTrainingButton;

    private void Start() {
        var versionAsset = Resources.Load<TextAsset>("version");
        if (versionText != null) {
            versionText.text = versionAsset != null ? versionAsset.text : "dev";
        }
        if (stageModeButton != null) {
            stageModeButton.onClick.AddListener(OnStageModeClicked);
        }
        if (bossTrainingButton != null) {
            bossTrainingButton.onClick.AddListener(OnBossTrainingClicked);
        }
    }

    private void OnStageModeClicked() {
        GameSelection.BossTraining = false;
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnBossTrainingClicked() {
        GameSelection.BossTraining = true;
        SceneManager.LoadScene(nextSceneName);
    }
}
