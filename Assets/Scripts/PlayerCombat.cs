using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float basicAttackDamage = 1f;
    public float heavyAttackDamage = 2f;
    private float basicAttackCooldown = 0.75f;
    private float heavyAttackCooldown = 1.5f;

    public Animator animator;

    private bool meleeSide;
    [HideInInspector]
    public bool readyToBasicAttack;
    public bool readyToHeavyAttack;

    // Start is called before the first frame update
    void Start()
    {
        meleeSide = false;
        readyToBasicAttack = true;
        readyToHeavyAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && readyToBasicAttack)
        {
            readyToBasicAttack = false;
            meleeSide = !meleeSide;
            animator.SetTrigger("BasicAttack");
            animator.SetInteger("BasicMeleeSide", meleeSide ? 0 : 1);
            Invoke(nameof(ResetBasicAttack), basicAttackCooldown);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && readyToHeavyAttack)
        {
            readyToHeavyAttack = false;
            meleeSide = !meleeSide;
            animator.SetTrigger("HeavyAttack");
            animator.SetInteger("HeavyMeleeSide", meleeSide ? 0 : 1);
            Invoke(nameof(ResetHeavyAttack), heavyAttackCooldown);
        }
    }

    private void ResetBasicAttack() { readyToBasicAttack = true; }
    private void ResetHeavyAttack() { readyToHeavyAttack = true; }
}
