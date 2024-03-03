using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float basicPunchDamage = 1f;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.SetTrigger("Punch");
        }
    }
}
