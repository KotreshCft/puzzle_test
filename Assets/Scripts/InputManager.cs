using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject inGameKeyboard;
    [SerializeField] private InputManagerComponent inputManagerComponent;

    private TMP_InputField activeInputField;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        inGameKeyboard.SetActive(false);
        nameInput.onSelect.AddListener((_) => ShowKeyboard(nameInput));
        startButton.onClick.AddListener(ValidateAndStartGame);
    }

    private void ShowKeyboard(TMP_InputField inputField)
    {
        activeInputField = inputField;
        inGameKeyboard.SetActive(true);
    }

    public void HideKeyboard()
    {
        inGameKeyboard.SetActive(false);
        activeInputField = null;
    }

    private void Update()
    {
        // Check for mouse clicks outside the keyboard and input field
        if (inGameKeyboard.activeSelf && Input.GetMouseButtonDown(0))
        {
            // Check if the click is not on the keyboard or input field
            if (!IsPointerOverUIElement(inGameKeyboard) && !IsPointerOverUIElement(activeInputField.gameObject))
            {
                HideKeyboard();
            }
        }
    }
    private bool IsPointerOverUIElement(GameObject element)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // Create a list to store results
        List<RaycastResult> results = new List<RaycastResult>();

        // Raycast using the EventSystem
        EventSystem.current.RaycastAll(eventData, results);

        // Check if any of the results are the element or its children
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == element || result.gameObject.transform.IsChildOf(element.transform))
            {
                return true;
            }
        }

        return false;
    }
    public void AddLetter(string letter)
    {
        if (activeInputField != null)
        {
            activeInputField.text += letter;
        }
    }
 

    public void DeleteLetter()
    {
        if (activeInputField != null && activeInputField.text.Length > 0)
        {
            activeInputField.text = activeInputField.text.Remove(activeInputField.text.Length - 1);
        }
    }

    public void SubmitWord()
    {
        Debug.Log("Submitted: " + (activeInputField != null ? activeInputField.text : "No active input"));
    }

    private void ValidateAndStartGame()
    {
        string name = nameInput.text;
        if (string.IsNullOrEmpty(name))
        {
            ShowError("Name is required!");
            return;
        }

        Debug.Log($"Name: {name}");

        if (inputManagerComponent != null)
        {
            inputManagerComponent.FindAndAssignSelectedController();
            inputManagerComponent.SendPlayerName(name);
        }
        else
        {
            Debug.LogError("InputManagerComponent is not assigned!");
        }
    }

    private void ShowError(string errorMessage)
    {
        Debug.LogError(errorMessage);
    }
}
