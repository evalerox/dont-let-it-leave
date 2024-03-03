using UnityEngine;

public class PlayerPunch : MonoBehaviour
{
    public PlayerCombat combat;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().RecieveHit(combat.basicPunchDamage);
        }
    }
}
