using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float basicPunchDamage = 1f;
    public float attackCooldown = 1f;

    public Animator animator;

    private bool meleeCombo;
    [HideInInspector]
    public bool readyToAttack;

    // Start is called before the first frame update
    void Start()
    {
        meleeCombo = false;
        readyToAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && readyToAttack)
        {
            readyToAttack = false;
            animator.SetTrigger("Attack");
            Invoke(nameof(ResetReadyToAttack), attackCooldown);
            // TODO: Search for a better attack animation
            // meleeCombo = !meleeCombo;
            //animator.SetInteger("MeleeCombo", meleeCombo ? 0 : 1);
        }
    }

    private void ResetReadyToAttack()
    {
        readyToAttack = true;
    }
}
