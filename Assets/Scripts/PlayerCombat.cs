using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float basicPunchDamage = 1f;

    public Animator animator;

    private bool meleeCombo;

    // Start is called before the first frame update
    void Start()
    {
        meleeCombo = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            meleeCombo = !meleeCombo;
            animator.SetTrigger("Attack");
            animator.SetInteger("MeleeCombo", meleeCombo ? 0 : 1);
        }
    }
}
