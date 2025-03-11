using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySmartBlade : MonoBehaviour
{
    public float sliceForce = 5f;
    public float minSliceVelocity = 0.01f;

    private RectTransform rectTransform;
    private Camera uiCamera; // Assign the camera used for the Canvas (usually Screen Space - Camera)
    private Collider2D sliceCollider;
    private TrailRenderer sliceTrail;

    public Vector2 direction { get; private set; }
    public bool slicing { get; private set; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        uiCamera = Camera.main; // Use the main camera or assign the UI camera
        sliceCollider = GetComponent<Collider2D>();
        sliceTrail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnEnable()
    {
        StopSlice();
    }

    private void OnDisable()
    {
        StopSlice();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartSlice();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopSlice();
        }
        else if (slicing)
        {
            ContinueSlice();
        }
    }

    private void StartSlice()
    {
        Vector3 position = uiCamera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0f; // Ensure 2D
        rectTransform.position = position;

        slicing = true;
        sliceCollider.enabled = true;
        sliceTrail.enabled = true;
        sliceTrail.Clear();
    }

    private void StopSlice()
    {
        slicing = false;
        sliceCollider.enabled = false;
        sliceTrail.enabled = false;
    }

    private void ContinueSlice()
    {
        Vector3 newPosition = uiCamera.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0f; // Ensure 2D
        direction = new Vector2(newPosition.x - rectTransform.position.x, newPosition.y - rectTransform.position.y);

        float velocity = direction.magnitude / Time.deltaTime;
        sliceCollider.enabled = velocity > minSliceVelocity;

        rectTransform.position = newPosition;
    }
}
