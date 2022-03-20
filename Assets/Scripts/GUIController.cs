using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    #region Variables
    public Text scoreText;
    public Text highscoreText;
    public Text realtimeScoreText;
    public GameObject gameOverPanel;
    public GameObject holdStartText;
    #endregion

    private void Awake()
    {
        // Just In Case, If Panels Are In Incorrect Status, Those Lines Are Sets Them Correctly
        ShowHoldStartText(true);
        ShowGameOverPanel(false);
    }

    public void HandleRestartButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShowGameOverPanel(bool status)
    {
        gameOverPanel.SetActive(status);
    }

    public void ShowHoldStartText(bool status)
    {
        holdStartText.SetActive(status);
    }
}