using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPlaying { get; private set; } = true;
    float survivalTime = 0f;

    [Header("UI")]
    public TextMeshProUGUI timeText;
    public GameObject gameOverPanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartGame();
        }

        if (!IsPlaying) return;

        survivalTime += Time.deltaTime;
        if (timeText != null)
            timeText.text = $"Time: {survivalTime:F1}s";
    }

    public void GameOver()
    {
        if (!IsPlaying) return;
        IsPlaying = false;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}