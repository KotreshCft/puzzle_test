using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySmartObjectSpawner : MonoBehaviour
{
    private RectTransform spawnArea;

    public GameObject[] fruitPrefabs; // negative points
    public GameObject bombPrefab; // positive point
    [Range(0f, 1f)] public float bombChance = 0.05f;

    public enum SpawnPosition
    {
        Top,
        Bottom
    }

    public SpawnPosition spawnFrom = SpawnPosition.Top;

    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    public float minAngle = -15f;
    public float maxAngle = 15f;

    public float minForce = 18f;
    public float maxForce = 22f;

    public float maxLifetime = 5f;

    private void Awake()
    {
        // Clean up the fruitPrefabs array by removing nulls
        List<GameObject> validPrefabs = new List<GameObject>();
        foreach (var prefab in fruitPrefabs)
        {
            if (prefab != null)
            {
                validPrefabs.Add(prefab);
            }
            else
            {
                Debug.LogWarning("Null prefab found in fruitPrefabs. It will be ignored.");
            }
        }

        fruitPrefabs = validPrefabs.ToArray();

        if (fruitPrefabs.Length == 0)
        {
            Debug.LogError("All prefabs in fruitPrefabs are null or missing. Please assign valid prefabs in the Inspector.");
        }
        spawnArea = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        StartCoroutine(Spawn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);
        // Validate fruitPrefabs
        if (fruitPrefabs == null || fruitPrefabs.Length == 0)
        {
            Debug.LogError("fruitPrefabs array is empty or null. Please assign prefabs in the Inspector.");
            yield break;
        }

        foreach (var prefab in fruitPrefabs)
        {
            if (prefab == null)
            {
                Debug.LogError("One or more elements in the fruitPrefabs array is null. Please check the array.");
                yield break;
            }
        }

        while (enabled)
        {
            GameObject prefab = Random.value < bombChance ? bombPrefab : fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

            if (prefab == null)
            {
                Debug.LogError($"Selected prefab({prefab.name}) is null. Skipping spawn.");
                continue;
            }

            Vector2 spawnPosition;
            Vector2 forceDirection;

            if (spawnFrom == SpawnPosition.Top)
            {
                // Spawn from the top
                spawnPosition = new Vector2(
                    Random.Range(spawnArea.rect.xMin, spawnArea.rect.xMax),
                    spawnArea.rect.yMax
                );
                forceDirection = Vector2.down; // Fall down
            }
            else
            {
                // Spawn from the bottom
                spawnPosition = new Vector2(
                    Random.Range(spawnArea.rect.xMin, spawnArea.rect.xMax),
                    spawnArea.rect.yMin
                );
                forceDirection = Vector2.up; // Push upward
            }

            // Convert spawn position to world coordinates
            Vector3 worldPosition = spawnArea.TransformPoint(spawnPosition);

            // Instantiate UI object
            GameObject spawnedObject = Instantiate(prefab, spawnArea);
            RectTransform objectTransform = spawnedObject.GetComponent<RectTransform>();
            objectTransform.position = worldPosition;
            objectTransform.rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

            Rigidbody2D rb = spawnedObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float forceMagnitude = Random.Range(minForce, maxForce);
                rb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
            }

            Destroy(spawnedObject, maxLifetime);

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }
}
