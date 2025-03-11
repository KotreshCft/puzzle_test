using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;

public class PartySmartManager : MonoBehaviour
{
    public static PartySmartManager Instance { get; private set; }

    [SerializeField] private PartySmartBlade blade;
    [SerializeField] private PartySmartObjectSpawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timerText;
    [SerializeField] private Image fadeImage;

    public int score { get; private set; } = 0;
    public float gameDuration = 60f;  // Game duration in seconds
    private float timeLeft;

    public GameObject startScreen;
    public GameObject mainScreen;
    public GameObject endScreen;
    public GameObject leaderScreen;

    public Button startButton;


    //leaderboard
    public leader_board leaderboardManager;
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
        canvasCamera = Camera.main; // Assuming the main camera is used for the canvas

        blade.enabled = false;
        spawner.enabled = false;
        startButton.onClick.AddListener(NewGame);
        //NewGame();
        startScreen.SetActive(true);
        mainScreen.SetActive(false);
        endScreen.SetActive(false);
        leaderScreen.SetActive(false);
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
    public GameObject floatingImagePrefab; // Assign the prefab in the inspector
    public Transform canvasTransform; // Assign the Canvas' RectTransform in the inspector

    public void IncreaseScore(Vector3 worldPosition, int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
        
        SpawnFloatingImage(true, worldPosition); // true for increase
        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }
    }
    bool gameOver;
    public Sprite increaseSprite; // Assign the sprite for score increase in the Inspector
    public Sprite decreaseSprite; // Assign the sprite for score decrease in the Inspector
    private Camera canvasCamera;

    private void SpawnFloatingImage(bool isIncrease, Vector3 worldPosition)
    {
        if (floatingImagePrefab == null || canvasTransform == null) return;

        // Convert the world position of the object to screen space
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Ensure the position is within screen bounds (optional)
        if (screenPosition.x < 0 || screenPosition.x > Screen.width || screenPosition.y < 0 || screenPosition.y > Screen.height)
        {
            return; // Don't spawn the floating image outside of the screen
        }

        // Instantiate the prefab at the calculated screen position
        GameObject floatingImage = Instantiate(floatingImagePrefab, canvasTransform);
        RectTransform rectTransform = floatingImage.GetComponent<RectTransform>();

        // Convert screen position to canvas space for Screen Space Camera
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform as RectTransform, screenPosition, canvasCamera, out canvasPosition);
        rectTransform.localPosition = canvasPosition;

        // Set the appropriate image for increase or decrease
        UnityEngine.UI.Image imageComponent = floatingImage.GetComponent<UnityEngine.UI.Image>();
        if (imageComponent != null)
        {
            imageComponent.sprite = isIncrease ? PartySmartManager.Instance.increaseSprite : PartySmartManager.Instance.decreaseSprite;
            StartCoroutine(FloatingImageAnimation(rectTransform, floatingImage));
        }
    }
    private IEnumerator FloatingImageAnimation(RectTransform rectTransform, GameObject floatingImage)
    {
        // Store the initial position
        Vector3 initialPosition = rectTransform.position;

        // Define the target position (move upwards)
        Vector3 targetPosition = initialPosition + new Vector3(0, 5f, 0); // Move 100 units up (adjust as needed)

        // Set up the fade effect using CanvasGroup
        CanvasGroup canvasGroup = floatingImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = floatingImage.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1f; // Ensure it's visible initially

        // Animation speed
        float duration = 1f; // Adjust the time to move up and fade out
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            // Move the image upwards
            rectTransform.position = Vector3.Lerp(initialPosition, targetPosition, timeElapsed / duration);

            // Fade the image out
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timeElapsed / duration);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final state
        rectTransform.position = targetPosition;
        canvasGroup.alpha = 0f;

        // Destroy the floating image after the animation
        Destroy(floatingImage);
    }
    public void DecreaseScore(Vector3 worldPosition, int amount)
    {
        SpawnFloatingImage(false, worldPosition); // false for decrease

        score = Mathf.Max(0, score - amount); ;
        scoreText.text = score.ToString();
    }

    public void Explode()
    {
        blade.enabled = false;
        spawner.enabled = false;

        StartCoroutine(ExplodeSequence());
    }
    public int scoreToWin = 10;
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
        
        // Get the first and second children of the EndScreen object
        Transform winTransform = endScreen.transform.GetChild(0);  // First child (Win)
      
       
            winTransform.gameObject.SetActive(true);   // Show the "Win" object
           // lostTransform.gameObject.SetActive(false); // Hide the "Lost" object
      
        StartCoroutine(DelayedUpdateLeaderboard(PlayerName, score));


    }
    // updating and shoing leader board

    string PlayerName ;
    public void sendPlayername(string name)
    {
        PlayerName = name;
        Debug.Log("name coming to to maine script" + name);
    }
    private IEnumerator DelayedUpdateLeaderboard(string playerName, float score)
    {
        yield return new WaitForSeconds(5f); // Wait for 5 seconds
        UpdateLeaderboard(playerName, score);
    }

    public void UpdateLeaderboard(string teamName, float activityTime)
    {
       

        Debug.Log(activityTime + ";" + teamName);
        LeaderboardEntry newEntry = new LeaderboardEntry(teamName, activityTime);

        // Load existing leaderboard data
        string jsonString = PlayerPrefs.GetString("Leaderboard", "{}");
        Leaderboard leaderboard = JsonUtility.FromJson<Leaderboard>(jsonString);

        if (leaderboard == null)
            leaderboard = new Leaderboard();

        // Add the new entry
        leaderboard.entries.Add(newEntry);

        // Save updated leaderboard data
        string updatedJsonString = JsonUtility.ToJson(leaderboard);
        PlayerPrefs.SetString("Leaderboard", updatedJsonString);
        PlayerPrefs.Save();

        startScreen.SetActive(false);
        mainScreen.SetActive(false);
        endScreen.SetActive(false);
        leaderScreen.SetActive(true);
        leaderboardManager.ShowLeaderboard();

       
    }
    public IEnumerator ShowLeaderboardAfterDelay()
    {
        yield return new WaitForSeconds(5f);

        leaderScreen.SetActive(true);
        leaderboardManager.ShowLeaderboard();
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
        yield return new WaitForSecondsRealtime(3f);
        ReloadGame();
    }
    void ReloadGame()
    {
        Time.timeScale = 1f;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReloadGame();
        }
    }

}
