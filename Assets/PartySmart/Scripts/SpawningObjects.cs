using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningObjects : MonoBehaviour
{
    public GameObject whole;
    public GameObject sliced;

    private Rigidbody2D objectRigidbody;
    private Collider2D objectCollider;
    private ParticleSystem sliceEffect;

    public int points = 1;

    // Enum to define object type
    public enum ObjectType
    {
        Negative,
        Postive
    }

    public ObjectType objectType;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody2D>();
        objectCollider = GetComponent<Collider2D>();
        sliceEffect = GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D Postive)
    {
        if (Postive.CompareTag("Player"))
        {
            GetComponent<Collider2D>().enabled = false;
            PartySmartBlade blade = Postive.GetComponent<PartySmartBlade>();
            HandleSlice(blade.direction, blade.transform.position, blade.sliceForce);
        }
    }

    private void HandleSlice(Vector3 direction, Vector3 position, float force)
    {
        // Adjust score based on object type
        if (objectType == ObjectType.Negative)
        {
            //PartySmartManager.Instance.IncreaseScore(position, points);
            SentinelMainManager.Instance.DecreaseScore(position, points);
        }
        else
        {
            //PartySmartManager.Instance.DecreaseScore(position, points);
            SentinelMainManager.Instance.IncreaseScore(position, points);
        }

        Slice(direction, position, force);
    }

    private void Slice(Vector3 direction, Vector3 position, float force)
    {
        // Disable the whole object
        objectCollider.enabled = false;
        whole.SetActive(false);

        // Enable the sliced object
        sliced.SetActive(true);
        sliceEffect.Play();

        // Rotate based on the slice angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Rigidbody2D[] slices = sliced.GetComponentsInChildren<Rigidbody2D>();

        // Add a force to each slice based on the blade direction
        foreach (Rigidbody2D slice in slices)
        {
            slice.velocity = objectRigidbody.velocity;
            slice.AddForceAtPosition(direction * force, position, ForceMode2D.Impulse);
        }
    }
}
