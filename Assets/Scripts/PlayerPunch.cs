using UnityEngine;

public class PlayerPunch : MonoBehaviour
{
    public PlayerCombat combat;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !combat.readyToAttack)
        {
            Vector3 collisionPoint = other.ClosestPoint(transform.position);
            Vector3 fromDirection = transform.forward;
            other.GetComponent<Enemy>().RecieveHit(collisionPoint, fromDirection, combat.basicPunchDamage);
        }
    }
}
