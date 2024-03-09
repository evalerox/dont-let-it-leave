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
                enemy.RecieveHit(collisionPoint, combat.basicAttackDamage);
            }
            else if (!combat.readyToHeavyAttack)
            {
                enemy.RecieveHit(collisionPoint, combat.heavyAttackDamage);
            }
        }
    }
}
