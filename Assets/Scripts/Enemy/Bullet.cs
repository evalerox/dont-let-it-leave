using UnityEngine;

public class Bullet : MonoBehaviour
{
    readonly float bulletSpeed = 22f;
    readonly float timeToDestroy = 5f;

    private void Start()
    {
        Invoke(nameof(DestroyBullet), timeToDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += bulletSpeed * Time.deltaTime * transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        DestroyBullet();
    }

    void DestroyBullet()
    {
        Destroy(this.gameObject);
    }
}
