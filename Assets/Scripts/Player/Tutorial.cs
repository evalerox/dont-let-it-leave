using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public TextMeshProUGUI jumpText;
    public TextMeshProUGUI dashText;
    public TextMeshProUGUI combatText;

    public AudioSource tutorialSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tutorial_jump") && jumpText)
        {
            tutorialSound.Play();
            jumpText.enabled = true;
        }

        if (other.CompareTag("Tutorial_dash") && dashText)
        {
            tutorialSound.Play();
            dashText.enabled = true;
        }

        if (other.CompareTag("Tutorial_combat") && combatText)
        {
            tutorialSound.Play();
            combatText.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Destroy texts on exit
        if (other.CompareTag("Tutorial_jump"))
        {
            Destroy(jumpText);
        }

        if (other.CompareTag("Tutorial_dash"))
        {
            Destroy(dashText);
        }

        if (other.CompareTag("Tutorial_combat"))
        {
            Destroy(combatText);
        }
    }
}
