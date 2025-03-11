
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject whole;
    public GameObject sliced;

    private Rigidbody bombRigidbody;
    private Collider bombCollider;
    private ParticleSystem sliceEffect;

    public int points = 1;

    private void Awake()
    {
        bombRigidbody = GetComponent<Rigidbody>();
        bombCollider = GetComponent<Collider>();
        sliceEffect = GetComponentInChildren<ParticleSystem>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            //GameManager.Instance.Explode();
            
            Blade blade = other.GetComponent<Blade>();
            Slice(blade.direction, blade.transform.position, blade.sliceForce);
        }
    }
    private void Slice(Vector3 direction, Vector3 position, float force)
    {
        GameController.Instance.DecreaseScore();
        GameController.Instance.SpawnFloatingText($"- {points}", position, Color.red);

        // Disable the whole fruit
        bombCollider.enabled = false;
        whole.SetActive(false);

        // Enable the sliced fruit
        sliced.SetActive(true);
        sliceEffect.Play();

        // Rotate based on the slice angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        // Add a force to each slice based on the blade direction
        foreach (Rigidbody slice in slices)
        {
            slice.velocity = bombRigidbody.velocity;
            slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
        }
    }
}
