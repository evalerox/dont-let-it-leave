using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float maxHealth = 10f;
    public float health;
    public float healCooldown = 3f;
    public bool isHealing = false;
    private bool isDead = false;

    public Volume myVolume;
    public Animator animator;
    public GameObject deadCanvas;
    private Vignette vg;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        isHealing = false;
        isDead = false;

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

            if (health <= 0 && !isDead)
            {
                StartCoroutine(Dead());
            }
        }
    }

    private void ResetAutoHealing() { isHealing = true; }

    IEnumerator Dead()
    {
        isDead = true;
        animator.SetTrigger("Dead");
        Destroy(GetComponent<PlayerMovement>());
        Destroy(GetComponent<PlayerCombat>());
        yield return new WaitForSeconds(1f);
        deadCanvas.SetActive(true);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}
