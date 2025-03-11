using UnityEngine;
using UnityEngine.EventSystems;

public class InputManagerComponent : MonoBehaviour
{
    public enum Controllers
    {
        GameController,
        SentinelMainManager,
        PartySmartManager,
        IndeedManager
    }

    public Controllers selectedController;
    private MonoBehaviour controller;

    public void FindAndAssignSelectedController()
    {
        switch (selectedController)
        {
            case Controllers.GameController:
                controller = FindAnyObjectByType<GameController>();
                break;

            case Controllers.PartySmartManager:
                controller = FindAnyObjectByType<PartySmartManager>();
                break;

            case Controllers.SentinelMainManager:
                controller = FindAnyObjectByType<SentinelMainManager>();
                break;
                
            case Controllers.IndeedManager:
                controller = FindAnyObjectByType<IndeedManager>();
                break;
        }

        if (controller == null)
        {
            Debug.LogError($"Controller of type {selectedController} not found in the scene!");
        }
    }
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if (InputManager.Instance != null && InputManager.Instance.inGameKeyboard.activeSelf)
    //    {
    //        InputManager.Instance.HideKeyboard();
    //    }
    //}
    public void SendPlayerName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("Name is required!");
            return;
        }

        if (controller == null)
        {
            Debug.LogError("No controller assigned! Make sure FindAndAssignSelectedController() is called first.");
            return;
        }

        // Call the correct methods dynamically
        if (controller is GameController gameController)
        {
            gameController.sendPlayername(name);
            gameController.NewGame();
        }
        else if (controller is PartySmartManager partySmartManager)
        {
            partySmartManager.sendPlayername(name);
            partySmartManager.NewGame();
        }
        else if (controller is SentinelMainManager sentinelMainManager)
        {
            sentinelMainManager.sendPlayername(name);
            sentinelMainManager.NewGame();
        }
        else if (controller is IndeedManager indeedManager)
        {
            indeedManager.sendPlayername(name);
            indeedManager.NewGame();
        }
    }
}
