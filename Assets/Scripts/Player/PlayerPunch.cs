using UnityEngine;

public class PlayerPunch : MonoBehaviour
{
    public PlayerCombat combat;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector3 collisionPoint = other.ClosestPoint(transform.position);
            GuardAI enemy = other.GetComponent<GuardAI>();

            if (!combat.readyToBasicAttack)
            {
                combat.attackEnemySound.pitch = Random.Range(0.7f, 0.9f);
                combat.attackEnemySound.Play();
                enemy.RecieveHit(collisionPoint, combat.basicAttackDamage);
            }
            else if (!combat.readyToHeavyAttack)
            {
                combat.attackEnemySound.pitch = Random.Range(0.9f, 1.2f);
                combat.attackEnemySound.Play();
                enemy.RecieveHit(collisionPoint, combat.heavyAttackDamage);
            }
        }
    }
}
