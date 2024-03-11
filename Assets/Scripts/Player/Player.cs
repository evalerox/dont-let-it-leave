using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    public float maxHealth = 10f;
    public float health;
    public float healCooldown = 3f;
    public bool isHealing;

    public Volume myVolume;
    private Vignette vg;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        isHealing = false;

        myVolume.profile.TryGet(out Vignette vg);
        this.vg = vg;
    }

    // Update is called once per frame
    void Update()
    {
        vg.intensity.value = Mathf.Lerp(1f, 0f, health / maxHealth);

        if (isHealing && health < maxHealth)
        {
            health += 1f * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            health--;

            isHealing = false;
            CancelInvoke();
            Invoke(nameof(ResetAutoHealing), healCooldown);
            //StartCoroutine(ResetAutoHealing());

            if (health <= 0)
            {
                Debug.Log("Dead");
                Dead();
            }
        }
    }

    void Dead()
    {
        Destroy(this.gameObject);
    }

    private void ResetAutoHealing() { isHealing = true; }

    /*IEnumerator ResetAutoHealing()
    {
        isHealing = false;
        yield return new WaitForSeconds(healCooldown);
        isHealing = true;
    }*/
}
