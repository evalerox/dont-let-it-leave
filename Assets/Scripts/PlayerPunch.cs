using UnityEngine;

public class PlayerPunch : MonoBehaviour
{
    public PlayerCombat combat;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector3 collisionPoint = other.ClosestPoint(transform.position);
            Vector3 fromDirection = transform.forward;

            if (!combat.readyToBasicAttack)
            {
                other.GetComponent<Enemy>().RecieveHit(collisionPoint, fromDirection, combat.basicAttackDamage);
            }
            else if (!combat.readyToHeavyAttack)
            {
                other.GetComponent<Enemy>().RecieveHit(collisionPoint, fromDirection, combat.heavyAttackDamage);
            }

        }
    }
}
