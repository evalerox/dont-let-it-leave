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
    //private bool isDead = false;

    NavMeshAgent agent;
    Animator anim;
    GuardState currentState;
    Rigidbody rb;

    void Start()
    {
        //isDead = false;
        agent = GetComponent<NavMeshAgent>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        currentState = new Idle(
            gameObject,
            agent,
            anim,
            player,
            doPatrol,
            patrolCheckpoints
        );
    }

    void Update()
    {
        //if (isDead) { return; }
        currentState = currentState.Process();
    }

    public void RecieveHit(Vector3 collisionPoint, float damage)
    {
        //if (isDead) { return; }

        currentState.Hit();

        // Effects
        Instantiate(hitBloodParticle, collisionPoint, Quaternion.identity);
        rb.AddForce(damage * 500f * -transform.forward, ForceMode.Impulse);

        // Variables changes
        health -= damage;
        if (health <= 0)
        {
            currentState.Die();

            Destroy(GetComponent<CapsuleCollider>());
            Destroy(rb);
        }
    }
}
