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
    public GameObject completeLevelCanvas;
    private Vignette vg;
    public AudioSource hitSound;

    bool elevator;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        isHealing = false;
        isDead = false;

        elevator = false;

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

            if (!isDead)
            {
                hitSound.pitch = Random.Range(0.9f, 1.1f);
                hitSound.Play();
            }

            isHealing = false;
            CancelInvoke();
            Invoke(nameof(ResetAutoHealing), healCooldown);

            if (health <= 0 && !isDead)
            {
                StartCoroutine(Dead());
            }
        }

        if (other.CompareTag("Elevator") && !elevator)
        {
            elevator = true;
            Animator elevatorAnimator = other.GetComponent<Animator>();
            AudioSource elevatorOpenSound = other.GetComponent<Elevator>().elevatorSound;
            StartCoroutine(CompleteLevel(elevatorAnimator, elevatorOpenSound));
        }

        if (other.CompareTag("GreenLiquid") && !isDead)
        {
            StartCoroutine(Dead());
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

    IEnumerator CompleteLevel(Animator elevatorAnimator, AudioSource elevatorOpenSound)
    {
        elevatorAnimator.SetTrigger("Close");
        elevatorOpenSound.Play();
        yield return new WaitForSeconds(1f);
        completeLevelCanvas.SetActive(true);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }
}
