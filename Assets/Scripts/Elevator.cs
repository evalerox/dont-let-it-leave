using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public bool startOpen;
    public List<GuardAI> enemiesList;

    Animator animator;
    bool levelComplete;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        levelComplete = false;

        if (startOpen)
        {
            animator.SetTrigger("Open");
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool areAllEnemiesDead = enemiesList.All(g => g.isDead || g.currentState.name == GuardState.STATE.DEAD);
        if (areAllEnemiesDead && !levelComplete)
        {
            levelComplete = true;
            animator.SetTrigger("Open");
        }
    }
}
