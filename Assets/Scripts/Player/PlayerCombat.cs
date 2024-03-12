using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float basicAttackDamage = 0.5f;
    public float heavyAttackDamage = 1.5f;
    private readonly float basicAttackCooldown = 0.75f;
    private readonly float heavyAttackCooldown = 1.5f;

    public Animator animator;

    public AudioSource attackEnemySound;
    public AudioSource attackAirSound;

    private bool meleeSide;
    [HideInInspector]
    public bool readyToBasicAttack;
    [HideInInspector]
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

            attackAirSound.pitch = Random.Range(0.7f, 0.9f);
            attackAirSound.Play();

            Invoke(nameof(ResetBasicAttack), basicAttackCooldown);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && readyToHeavyAttack)
        {
            readyToHeavyAttack = false;
            meleeSide = !meleeSide;

            animator.SetTrigger("HeavyAttack");
            animator.SetInteger("HeavyMeleeSide", meleeSide ? 0 : 1);

            attackAirSound.pitch = Random.Range(0.9f, 1.2f);
            attackAirSound.Play();

            Invoke(nameof(ResetHeavyAttack), heavyAttackCooldown);
        }
    }

    private void ResetBasicAttack() { readyToBasicAttack = true; }
    private void ResetHeavyAttack() { readyToHeavyAttack = true; }
}
