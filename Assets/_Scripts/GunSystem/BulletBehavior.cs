using UnityEngine;


public class BulletBehavior : MonoBehaviour
{
    public float moveSpeed = 0.1f;
    private float maxLifeTime = 20.0f;
    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (startTime + maxLifeTime < Time.time)
            Destroy(gameObject);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Target")
            Destroy(other.gameObject);
        Destroy(gameObject);
    }
}