using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 10f;
    private float health;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RecieveHit(float damage)
    {
        animator.SetTrigger("Hit");
        health -= damage;

        if (health <= 0)
        {
            // TODO: Make it a ragdoll
            Destroy(this.gameObject);
        }
    }
}
