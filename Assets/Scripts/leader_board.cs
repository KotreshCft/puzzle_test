
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.Tilemaps.TilemapRenderer;
using System.Collections;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LeaderboardEntry
{
    public string teamName;
    public float activityTime;
    public string distance;
    public long timestamp;
    public bool isDistanceEntry;

    public LeaderboardEntry(string teamName, float activityTime)
    {
        this.teamName = teamName;
        this.activityTime = activityTime;
        this.distance = "";
        this.isDistanceEntry = false;
        this.timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public LeaderboardEntry(string teamName, float activityTime, string distance)
    {
        this.teamName = teamName;
        this.activityTime = activityTime;
        this.distance = distance;
        this.isDistanceEntry = !string.IsNullOrEmpty(distance);
        this.timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}

[System.Serializable]
public class Leaderboard
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public enum TimeFormat
{
    Seconds,
    MinuteSecond,
    HourMinuteSecond
}

public class leader_board : MonoBehaviour
{
    public GameObject leaderboardScreen;
    public Transform entryContainer;
    public Transform entryTemplate;

    public Sprite firstRankImage;
    public Sprite secondRankImage;
    public Sprite thirdRankImage;
    public Sprite defaultRankImage;

    public Sprite firstRankMedal;
    public Sprite secondRankMedal;
    public Sprite thirdRankMedal;
    public Image noEntriesImage;

    [Tooltip("0: Ascending, 1: Descending.")]
    public float sortOrder = 0;

    [Tooltip("Padding between leaderboard entries in pixels")]
    public float entryPadding = 10f; // New variable for padding

    private List<Transform> highscoreEntryTransformList;
    [SerializeField] private int MAX_DISPLAYED_ENTRIES = 10;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearLeaderboard();
        }
    }

    public void ClearLeaderboard()
    {
        Leaderboard emptyLeaderboard = new Leaderboard();
        string emptyJson = JsonUtility.ToJson(emptyLeaderboard);
        PlayerPrefs.SetString("Leaderboard", emptyJson);
        PlayerPrefs.Save();
        Debug.Log("Leaderboard cleared.");
        ShowLeaderboard();
    }

    public void UpdateLeaderboard(string teamName, float timeInSeconds)
    {
        string jsonString = PlayerPrefs.GetString("Leaderboard", "{}");
        Leaderboard leaderboard = JsonUtility.FromJson<Leaderboard>(jsonString);

        if (leaderboard == null)
            leaderboard = new Leaderboard();

        LeaderboardEntry newEntry = new LeaderboardEntry(teamName, timeInSeconds);
        leaderboard.entries.Add(newEntry);

        string updatedJsonString = JsonUtility.ToJson(leaderboard);
        PlayerPrefs.SetString("Leaderboard", updatedJsonString);
        PlayerPrefs.Save();

        ShowLeaderboard();
    }

    public void ShowLeaderboard()
    {
      
        string jsonString = PlayerPrefs.GetString("Leaderboard", "{}");
        Leaderboard leaderboard = JsonUtility.FromJson<Leaderboard>(jsonString);

        // Check for valid leaderboard data before modifying UI
        bool hasEntries = leaderboard != null && leaderboard.entries.Count > 0;

        // Show or hide the "No Entries" image accordingly
        if (noEntriesImage != null)
        {
            noEntriesImage.gameObject.SetActive(!hasEntries);
        }

        // If there are no entries, stop execution here
        if (!hasEntries)
        {
            Debug.Log("No leaderboard data found.");
            return;
        }

        foreach (Transform child in entryContainer)
        {
            if (child != entryTemplate)
                Destroy(child.gameObject);
        }

        LeaderboardEntry latestEntry = leaderboard.entries
            .OrderByDescending(x => x.timestamp)
            .First();

        if (sortOrder == 0f) // Ascending order
        {
            leaderboard.entries.Sort((x, y) => x.activityTime.CompareTo(y.activityTime));
        }
        else // Descending order
        {
            leaderboard.entries.Sort((x, y) => y.activityTime.CompareTo(x.activityTime));
        }

        highscoreEntryTransformList = new List<Transform>();

        int latestEntryRank = leaderboard.entries.FindIndex(x =>
            x.teamName == latestEntry.teamName &&
            x.activityTime == latestEntry.activityTime &&
            x.timestamp == latestEntry.timestamp) + 1;

        int entriesToDisplay = Mathf.Min(leaderboard.entries.Count, MAX_DISPLAYED_ENTRIES);

        for (int i = 0; i < entriesToDisplay; i++)
        {
            LeaderboardEntry entry = leaderboard.entries[i];
            bool isLatestEntry = entry.timestamp == latestEntry.timestamp;
            CreateHighscoreEntryTransform(entry, entryContainer, highscoreEntryTransformList, i + 1, isLatestEntry);
        }

        if (!leaderboard.entries.Take(entriesToDisplay).Any(x => x.timestamp == latestEntry.timestamp)
            && latestEntryRank > entriesToDisplay)
        {
            CreateSeparatorLine();
            CreateHighscoreEntryTransform(latestEntry, entryContainer, highscoreEntryTransformList, latestEntryRank, true);
        }
    }

    private void CreateSeparatorLine()
    {
        GameObject separator = new GameObject("Separator");
        separator.transform.SetParent(entryContainer);
        RectTransform sepRect = separator.AddComponent<RectTransform>();
        Image sepImage = separator.AddComponent<Image>();

        sepRect.anchoredPosition = new Vector2(0, -110f * highscoreEntryTransformList.Count - 10f);
        sepRect.sizeDelta = new Vector2(entryContainer.GetComponent<RectTransform>().rect.width - 40f, 2f);
        sepImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }

    private void CreateHighscoreEntryTransform(LeaderboardEntry entry, Transform container,
    List<Transform> transformList, int rank, bool isLatestEntry)
    {
       
        //transformList.Add(entryTransform);
        float templateHeight = 10F + entryPadding; // Added padding to template height
        Transform entryTransform = Instantiate(entryTemplate, container);

        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        // **Find and assign rank medal Image**
        Image rankImage = entryTransform.Find("rankImage")?.GetComponent<Image>();
        if (rankImage != null)
        {
            rankImage.gameObject.SetActive(true);

            switch (rank)
            {
                case 1:
                    rankImage.sprite = firstRankMedal;
                    break;
                case 2:
                    rankImage.sprite = secondRankMedal;
                    break;
                case 3:
                    rankImage.sprite = thirdRankMedal;
                    break;
                default:
                    rankImage.enabled = false; // Hide for ranks > 3
                    break;
            }
        }

        // **Remove text-based ranking**
        TMP_Text rankText = entryTransform.Find("rankText")?.GetComponent<TMP_Text>();
        if (rankText != null)
        {
            rankText.gameObject.SetActive(rank > 3); // Show text only for ranks > 3
            if (rank > 3)
            {
                rankText.text = $"{rank}{GetOrdinalSuffix(rank)}";
            }
        }

        // **Assign team name**
        TMP_Text teamText = entryTransform.Find("teamText")?.GetComponent<TMP_Text>();
        if (teamText != null)
        {
            teamText.text = entry.teamName;
            teamText.color = isLatestEntry ? Color.green : Color.white;
        }

        // **Assign activity time**
        TMP_Text activityTimeText = entryTransform.Find("activityTimeText")?.GetComponent<TMP_Text>();
        if (activityTimeText != null)
        {
            if (entry.isDistanceEntry)
            {
                activityTimeText.text = $"{entry.distance}m";
            }
            else
            {
                int minutes = Mathf.FloorToInt(entry.activityTime / 60);
                int seconds = Mathf.FloorToInt(entry.activityTime % 60);
                activityTimeText.text = $"{entry.activityTime}";
            }
        }

        // **Assign background panel for styling**
        Image backgroundPanel = entryTransform.Find("Panel")?.GetComponent<Image>();
        if (backgroundPanel != null)
        {
            backgroundPanel.sprite = rank switch
            {
                1 => firstRankImage,
                2 => secondRankImage,
                3 => thirdRankImage,
                _ => defaultRankImage
            };

            Color baseColor = (rank % 2 == 1) ? Color.white : new Color(0.9f, 0.9f, 0.9f);
            if (isLatestEntry)
            {
                baseColor = new Color(baseColor.r, baseColor.g * 0.95f, baseColor.b * 0.8f, baseColor.a);
            }
            backgroundPanel.color = baseColor;
        }

        transformList.Add(entryTransform);
    }

    private string GetOrdinalSuffix(int number)
    {
        if (number <= 0) return "";

        return (number % 100) switch
        {
            11 or 12 or 13 => "th",
            _ => (number % 10) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            }
        };
    }

    public void ReloadSceneAfterDelay()
    {
        Debug.Log("Reloading scene now.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private string FormatTime(float timeInSeconds, TimeFormat format)
    {
        int totalSeconds = Mathf.FloorToInt(timeInSeconds);
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        return format switch
        {
            TimeFormat.Seconds => seconds.ToString(),
            TimeFormat.MinuteSecond => string.Format("{0:D2}:{1:D2}", minutes, seconds),
            TimeFormat.HourMinuteSecond => string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds),
            _ => seconds.ToString()
        };
    }
}