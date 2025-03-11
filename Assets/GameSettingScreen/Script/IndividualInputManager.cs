
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IndividualInputManager : MonoBehaviour
{
    //public TMP_InputField Timer;
    //public TMP_InputField MinForce;
    //public TMP_InputField MaxForce;
    //public TMP_InputField MinSwanDily;
    //public TMP_InputField MaxSwanDily;
    //public Slider valueSlider;

    //public TMP_Text sliderValueText;
    //public TMP_Text errorMessage;
    //public Button submitButton;

    //private const string SliderKey = "SliderValue";

    //void Start()
    //{
    //    // Load and display saved data on startup
    //    LoadData();

    //    // Update slider value display
    //    sliderValueText.text = $"Value: {valueSlider.value:F2}";

    //    // Add listener to the slider to update the text
    //    valueSlider.onValueChanged.AddListener((value) =>
    //    {
    //        sliderValueText.text = $"Value: {value:F2}";
    //    });

    //    // Add listener to the submit button
    //    submitButton.onClick.AddListener(SaveData);
    //}

    //void LoadData()
    //{
    //    // Load each input field value
    //    Timer.text = PlayerPrefs.GetString("Timer", "");
    //    MinForce.text = PlayerPrefs.GetString("MinForce", "");
    //    MaxForce.text = PlayerPrefs.GetString("MaxForce", "");
    //    MinSwanDily.text = PlayerPrefs.GetString("MinSwanDily", "");
    //    MaxSwanDily.text = PlayerPrefs.GetString("MaxSwanDily", "");

    //    // Load slider value
    //    float sliderValue = PlayerPrefs.GetFloat(SliderKey, 0.5f); // Default to 0.5
    //    valueSlider.value = sliderValue;
    //    sliderValueText.text = $"Value: {sliderValue:F2}";
    //}

    //void SaveData()
    //{
    //    errorMessage.text = ""; // Clear any previous error message

    //    // Validate and save each input field individually
    //    if (!ValidateAndSave(Timer, "Input1")) return;
    //    if (!ValidateAndSave(MinForce, "Input2")) return;
    //    if (!ValidateAndSave(MaxForce, "Input3")) return;
    //    if (!ValidateAndSave(MinSwanDily, "Input4")) return;
    //    if (!ValidateAndSave(MaxSwanDily, "Input5")) return;

    //    // Save slider value
    //    float sliderValue = valueSlider.value;
    //    PlayerPrefs.SetFloat(SliderKey, sliderValue);
    //    PlayerPrefs.Save();
    //    Debug.Log("All data saved successfully!");

    //    errorMessage.text = "Data saved successfully!";
    //}


    //bool ValidateAndSave(TMP_InputField field, string key, bool allowDecimal = false)
    //{
    //    string input = field.text;

    //    // Check for alphabets first
    //    if (Regex.IsMatch(input, @"[a-zA-Z]"))
    //    {
    //        errorMessage.text = $"Invalid input in {field.name}. Alphabets are not allowed.";
    //        Debug.LogError($"Invalid input in {field.name} (contains alphabets): {input}");
    //        return false;
    //    }

    //    // Check for valid number (integer or decimal if allowed)
    //    string pattern = allowDecimal ? @"^\d+(\.\d+)?$" : @"^\d+$";
    //    if (!Regex.IsMatch(input, pattern))
    //    {
    //        errorMessage.text = $"Invalid input in {field.name}. Please enter a valid {(allowDecimal ? "number or decimal" : "number")}.";
    //        Debug.LogError($"Invalid input in {field.name} (invalid format): {input}");
    //        return false;
    //    }

    //    // Save valid input to PlayerPrefs
    //    PlayerPrefs.SetString(key, input);
    //    Debug.Log($"Saved {field.name}: {input}");
    //    return true;
    //}
    public TMP_InputField Timer;
    public TMP_InputField MinForce;
    public TMP_InputField MaxForce;
    public TMP_InputField MinSwanDily;
    public TMP_InputField MaxSwanDily;
    public Slider valueSlider;

    public TMP_Text sliderValueText;
    public TMP_Text errorMessage;
    public Button submitButton;

    private const string SliderKey = "SliderValue";

    void Start()
    {
        // Load and display saved data on startup
        LoadData();

        // Update slider value display
        sliderValueText.text = $"Value: {valueSlider.value:F2}";

        // Add listener to the slider to update the text
        valueSlider.onValueChanged.AddListener((value) =>
        {
            sliderValueText.text = $"Value: {value:F2}";
        });

        // Add listener to the submit button
        submitButton.onClick.AddListener(SaveData);
    }

    void LoadData()
    {
        // Load each input field value and display it
        //Timer.text = PlayerPrefs.GetString("gameDuration", "30");
        //MinForce.text = PlayerPrefs.GetString("minForce", "10");
        //MaxForce.text = PlayerPrefs.GetString("maxForce", "20");
        //MinSwanDily.text = PlayerPrefs.GetString("minSpawnDelay", "0.5");
        //MaxSwanDily.text = PlayerPrefs.GetString("maxSpawnDelay", "1");


        Timer.text = PlayerPrefs.HasKey("gameDuration") ? PlayerPrefs.GetFloat("gameDuration").ToString() : "30";
        MinForce.text = PlayerPrefs.HasKey("minForce") ? PlayerPrefs.GetFloat("minForce").ToString() : "30";
        MaxForce.text = PlayerPrefs.HasKey("maxForce") ? PlayerPrefs.GetFloat("maxForce").ToString() : "40";
        MinSwanDily.text = PlayerPrefs.HasKey("minSpawnDelay") ? PlayerPrefs.GetFloat("minSpawnDelay").ToString() : "0.5";
        MaxSwanDily.text = PlayerPrefs.HasKey("maxSpawnDelay") ? PlayerPrefs.GetFloat("maxSpawnDelay").ToString() : "1";
        // Load slider value
        float sliderValue = PlayerPrefs.GetFloat(SliderKey, 0.5f); // Default to 0.5
        valueSlider.value = sliderValue;
        sliderValueText.text = $"Value: {sliderValue:F2}";

        Debug.Log("Loaded previous data successfully!");
    }

    void SaveData()
    {

        errorMessage.text = ""; // Clear any previous error message

        // Validate and save each input field individually as float
        if (!ValidateAndSave(Timer, "gameDuration", true)) return;
        if (!ValidateAndSave(MinForce, "minForce", true)) return;
        if (!ValidateAndSave(MaxForce, "maxForce", true)) return;
        if (!ValidateAndSave(MinSwanDily, "minSpawnDelay", true)) return;
        if (!ValidateAndSave(MaxSwanDily, "maxSpawnDelay", true)) return;
        // Save slider value as float
        float sliderValue = valueSlider.value;
        PlayerPrefs.SetFloat("bombChance", sliderValue);
        PlayerPrefs.Save();
        Debug.Log("All data saved successfully!");

        // Display success message in green
        errorMessage.color = Color.green;
        errorMessage.text = "Data saved successfully!";
        LoadNewScene();

    }
    public string SentinalMain = "SentinalMain";//Assets/Sentinel/SentinalMain.unity
    public void LoadNewScene()
    {
        // Check if the scene name is not empty
        if (!string.IsNullOrEmpty(SentinalMain))
        {
            // Load the specified scene
            SceneManager.LoadScene(SentinalMain); //Assets / setting_screen / Setting_scene.unity
        }
        else
        {
            Debug.LogError("Scene name is not set!");
        }
    }

    bool ValidateAndSave(TMP_InputField field, string key, bool allowDecimal = false)
    {
        string input = field.text;

        // If the input is empty, load the previous value from PlayerPrefs
        if (string.IsNullOrEmpty(input))
        {
            float savedValue = PlayerPrefs.GetFloat(key, 0f);
            field.text = savedValue.ToString();
            Debug.LogWarning($"Empty input for {field.name}. Loaded saved value: {savedValue}");
            return true;
        }

        // Check for alphabets first
        if (Regex.IsMatch(input, @"[a-zA-Z]"))
        {
            // Display error message in red
            errorMessage.color = Color.red;
            errorMessage.text = $"Invalid input in {field.name}. Alphabets are not allowed.";
            Debug.Log($"Invalid input in {field.name} (contains alphabets): {input}");
            return false;
        }

        // Check for valid number (integer or decimal if allowed)
        string pattern = allowDecimal ? @"^\d+(\.\d+)?$" : @"^\d+$";
        if (!Regex.IsMatch(input, pattern))
        {
            // Display error message in red
            errorMessage.color = Color.red;
            errorMessage.text = $"Invalid input in {field.name}. Please enter a valid {(allowDecimal ? "number or decimal" : "number")}.";
            Debug.Log($"Invalid input in {field.name} (invalid format): {input}");
            return false;
        }

        // Try to parse the input as a float
        if (float.TryParse(input, out float floatValue))
        {
            // Save valid input to PlayerPrefs as a float
            PlayerPrefs.SetFloat(key, floatValue);
            Debug.Log($"Saved {field.name}: {floatValue}");
            return true;
        }
        else
        {
            // Display error message in red
            errorMessage.color = Color.red;
            errorMessage.text = $"Invalid input in {field.name}. Please enter a valid number.";
            Debug.Log($"Invalid input in {field.name} (not a float): {input}");
            return false;
        }
    }

}
