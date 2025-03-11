using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] private Blade blade;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timerText; 
    [SerializeField] private Image fadeImage;

    public int score { get; private set; } = 0;
    public float gameDuration = 60f;  // Game duration in seconds
    private float timeLeft;

    public GameObject startScreen;
    public GameObject mainScreen;
    public GameObject endScreen;

    [SerializeField] private TMP_Text endScreenScoreDisplay;

    string PlayerName;
    public void sendPlayername(string name)
    {
        PlayerName = name;
        Debug.Log("name coming to to maine script" + name);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        //NewGame();
        startScreen.SetActive(true);
        mainScreen.SetActive(false);
        endScreen.SetActive(false);
    }

    public void NewGame()
    {
        startScreen.SetActive(false);
        mainScreen.SetActive(true);

        spawner.gameObject.SetActive(true);

        timeLeft = gameDuration; // Reset timer to 60 seconds
        Time.timeScale = 1f;

        ClearScene();

        blade.enabled = true;
        spawner.enabled = true;

        score = 0;
        scoreText.text = score.ToString();
        timerText.text = timeLeft.ToString("F0");  // Display the timer

        StartCoroutine(Timer());
    }

    private void ClearScene()
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();

        foreach (Fruit fruit in fruits)
        {
            Destroy(fruit.gameObject);
        }

        Bomb[] bombs = FindObjectsOfType<Bomb>();

        foreach (Bomb bomb in bombs)
        {
            Destroy(bomb.gameObject);
        }
    }
    public GameObject floatingTextPrefab; // Assign the prefab in the inspector
    public Transform canvasTransform; // Assign the Canvas' RectTransform in the inspector

    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();

        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }
    }
    bool gameOver;
    private GameObject previousFloatingText;  // To store the previously spawned floating text

    public void SpawnFloatingText(string text, Vector3 worldPosition, Color color)
    {
        if (gameOver) return;  // Don't spawn text if the game is over

        // Destroy the previous floating text if it exists
        if (previousFloatingText != null)
        {
            Destroy(previousFloatingText);
        }

        // Instantiate the floating text prefab
        GameObject floatingText = Instantiate(floatingTextPrefab, canvasTransform);

        // Convert world position to canvas position
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        floatingText.transform.position = screenPosition;

        // Set the color of the text
        floatingText.GetComponent<TMP_Text>().color = color;

        // Animate the floating text
        FloatingText floatingTextScript = floatingText.GetComponent<FloatingText>();
        floatingTextScript.Animate(text);

        // Store the new floating text as the previous one
        previousFloatingText = floatingText;
    }

    public void DecreaseScore()
    {
        score -= 1;
        scoreText.text = score.ToString();
    }

    public void Explode()
    {
        blade.enabled = false;
        spawner.enabled = false;

        StartCoroutine(ExplodeSequence());
    }

    private IEnumerator ExplodeSequence()
    {
        float elapsed = 0f;
        float duration = 0.5f;
        gameOver = true;
        // Fade to white
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.black, t);

            Time.timeScale = 1f - t;
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

        mainScreen.SetActive(false);
        endScreen.SetActive(true);
        if(score >= 10)
        {
            endScreenScoreDisplay.text = "Great Score<br><size=50%>Party Time..</size>";
        }
        else
        {
            endScreenScoreDisplay.text = "Better Luck Next Time<br><size=50%>Party Over</size>";
        }

        StartCoroutine(WaitAndReload());
    }

    private IEnumerator Timer()
    {
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = timeLeft.ToString("F0");  // Update the timer display

            yield return null;
        }

        // Time's up! End the game.
        EndGame();
    }

    private void EndGame()
    {
        blade.enabled = false;
        spawner.enabled = false;

        Explode();
    }
    IEnumerator WaitAndReload()
    {
        yield return new WaitForSeconds(3f);
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
