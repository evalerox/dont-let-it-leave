using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardAI : MonoBehaviour
{
    public float maxHealth = 10f;
    private float health;

    public Transform player;
    public ParticleSystem hitBloodParticle;

    public bool doPatrol = false;
    public List<GameObject> patrolCheckpoints = new();

    public Transform bulletEmitter;
    public GameObject bulletPrefab;
    public AudioSource shootSource;
    public AudioSource hitSound;
    public bool readyToShoot;
    public float shootCooldown = 0.75f;

    NavMeshAgent agent;
    Animator anim;
    [HideInInspector]
    public GuardState currentState;
    Rigidbody rb;
    [HideInInspector]
    public bool isDead;

    void Start()
    {
        isDead = false;
        health = maxHealth;
        readyToShoot = true;

        agent = GetComponent<NavMeshAgent>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        currentState = new Idle(gameObject, agent, anim, player);
    }

    void Update()
    {
        currentState = currentState.Process();

        if (currentState.name == GuardState.STATE.ATTACK)
        {
            FireBullet();
        }
    }

    void FireBullet()
    {
        if (!readyToShoot) return;

        readyToShoot = false;

        shootSource.pitch = Random.Range(0.8f, 1f);
        shootSource.Play();

        // Fire bullet
        Instantiate(bulletPrefab, bulletEmitter.position, transform.rotation);

        Invoke(nameof(ResetShoot), shootCooldown);
    }

    private void ResetShoot() { readyToShoot = true; }

    public void RecieveHit(Vector3 collisionPoint, float damage)
    {
        if (currentState.name == GuardState.STATE.DEAD) { return; }

        currentState.Hit();

        hitSound.pitch = Random.Range(0.7f, 1.1f);
        hitSound.Play();

        // Effects
        Instantiate(hitBloodParticle, collisionPoint, Quaternion.identity);
        rb.AddForce(damage * 500f * -transform.forward, ForceMode.Impulse);

        // Variables changes
        health -= damage;
        if (health <= 0)
        {
            isDead = true;
            currentState.Die();

            Destroy(GetComponent<CapsuleCollider>());
            Destroy(rb);
        }
    }
}
