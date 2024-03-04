using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 10f;
    private float health;

    public Transform player;
    public ParticleSystem hitBloodParticle;

    Animator animator;
    Rigidbody rb;

    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        animator = transform.GetChild(0).GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            transform.LookAt(new Vector3(player.position.x, 0, player.position.z));
        }
    }

    public void RecieveHit(Vector3 collisionPoint, Vector3 fromDirection, float damage)
    {
        if (isDead) { return; }

        // Effects
        animator.SetTrigger("Hit");
        Instantiate(hitBloodParticle, collisionPoint, Quaternion.identity);
        rb.AddForce(damage * 500f * fromDirection, ForceMode.Impulse);

        // Variables changes
        health -= damage;
        if (health <= 0)
        {
            isDead = true;
            // TODO: Make it a ragdoll
            animator.SetBool("Dead", true);
            Destroy(GetComponent<CapsuleCollider>());
            Destroy(rb);
        }
    }
}
