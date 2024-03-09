using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardState
{
    public enum STATE
    {
        IDLE,
        PATROL,
        PURSUE,
        ATTACK,
        HIT,
        DEAD
        //RUNAWAY
    };

    public enum EVENT
    {
        ENTER,
        UPDATE,
        EXIT
    };

    public STATE name;
    protected EVENT stage;
    protected GameObject npc;
    protected Animator anim;
    protected Transform player;
    protected GuardState nextState;
    protected NavMeshAgent agent;
    protected bool doPatrol;
    protected List<GameObject> patrolCheckpoints;

    protected bool isHit = false;
    protected bool isDead = false;

    protected Vector3 lastPlayerPositon;

    readonly float visDist = 14.0f;
    readonly float hearDist = 5.0f;
    readonly float visAngle = 90.0f;
    readonly float shootDist = 7.0f;

    public GuardState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, bool _doPatrol, List<GameObject> _patrolCheckpoints)
    {
        npc = _npc;
        agent = _agent;
        anim = _anim;
        player = _player;
        doPatrol = _doPatrol;
        patrolCheckpoints = _patrolCheckpoints;

        stage = EVENT.ENTER;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }

    public GuardState Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }

    public bool CanSeePlayer()
    {
        Vector3 direction = player.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);

        if (direction.magnitude < visDist && angle < visAngle)
        {
            return true;
        }
        return false;
    }

    public bool CanHearPlayer()
    {
        Vector3 direction = player.position - npc.transform.position;

        if (direction.magnitude < hearDist)
        {
            return true;
        }
        return false;
    }

    public bool CanAttackPlayer()
    {
        Vector3 direction = player.position - npc.transform.position;

        if (direction.magnitude < shootDist)
        {
            return true;
        }
        return false;
    }

    public void Hit() { isHit = true; }
    public void Die() { isDead = true; }
}

public class Idle : GuardState
{
    public Idle(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, bool _doPatrol, List<GameObject> _patrolCheckpoints)
        : base(_npc, _agent, _anim, _player, _doPatrol, _patrolCheckpoints)
    {
        name = STATE.IDLE;
    }

    public override void Enter()
    {
        anim.SetTrigger("isIdle");
        base.Enter();
    }

    public override void Update()
    {
        if (CanSeePlayer() || CanHearPlayer())
        {
            nextState = new Pursue(npc, agent, anim, player, doPatrol, patrolCheckpoints);
            stage = EVENT.EXIT;
        }
        else if (doPatrol && Random.Range(0, 100) < 10)
        {
            nextState = new Patrol(npc, agent, anim, player, doPatrol, patrolCheckpoints);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isIdle");
        base.Exit();
    }
}

public class Patrol : GuardState
{
    int currentIndex = -1;

    public Patrol(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, bool _doPatrol, List<GameObject> _patrolCheckpoints)
        : base(_npc, _agent, _anim, _player, _doPatrol, _patrolCheckpoints)
    {
        name = STATE.PATROL;
        agent.speed = 2.0f;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        float lastDistance = Mathf.Infinity;

        for (int i = 0; i < patrolCheckpoints.Count; ++i)
        {
            GameObject thisWP = patrolCheckpoints[i];
            float distance = Vector3.Distance(npc.transform.position, thisWP.transform.position);
            if (distance < lastDistance)
            {
                currentIndex = i - 1;
                lastDistance = distance;
            }
        }

        anim.SetTrigger("isWalking");
        base.Enter();
    }

    public override void Update()
    {
        if (agent.remainingDistance < 1)
        {
            if (currentIndex >= patrolCheckpoints.Count - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }

            agent.SetDestination(patrolCheckpoints[currentIndex].transform.position);
        }

        if (CanSeePlayer() || CanHearPlayer())
        {
            nextState = new Pursue(npc, agent, anim, player, doPatrol, patrolCheckpoints);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isWalking");
        base.Exit();
    }
}

public class Pursue : GuardState
{
    public Pursue(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, bool _doPatrol, List<GameObject> _patrolCheckpoints)
        : base(_npc, _agent, _anim, _player, _doPatrol, _patrolCheckpoints)
    {
        name = STATE.PURSUE;
        agent.speed = 5.0f;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        lastPlayerPositon = player.position;
        anim.SetTrigger("isRunning");
        base.Enter();
    }

    public override void Update()
    {
        if (CanSeePlayer())
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(lastPlayerPositon);
        }

        if (agent.hasPath)
        {
            if (CanAttackPlayer())
            {
                nextState = new Attack(npc, agent, anim, player, doPatrol, patrolCheckpoints);
                stage = EVENT.EXIT;
            }
            else if (doPatrol && !CanSeePlayer())
            {
                nextState = new Patrol(npc, agent, anim, player, doPatrol, patrolCheckpoints);
                stage = EVENT.EXIT;
            }
        }
        else
        {
            nextState = new Idle(npc, agent, anim, player, doPatrol, patrolCheckpoints);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        lastPlayerPositon = player.position;
        anim.ResetTrigger("isRunning");
        base.Exit();
    }
}

public class Attack : GuardState
{
    readonly float rotationSpeed = 2.0f;
    readonly AudioSource shoot;

    public Attack(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, bool _doPatrol, List<GameObject> _patrolCheckpoints)
        : base(_npc, _agent, _anim, _player, _doPatrol, _patrolCheckpoints)
    {
        name = STATE.ATTACK;
        shoot = _npc.GetComponent<AudioSource>();
    }

    public override void Enter()
    {
        anim.SetTrigger("isShooting");
        agent.isStopped = true;
        shoot.Play();
        base.Enter();
    }

    public override void Update()
    {
        Vector3 direction = player.position - npc.transform.position;
        direction.y = 0.0f;

        npc.transform.rotation = Quaternion.Slerp(
            npc.transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * rotationSpeed
        );

        if (isDead)
        {
            nextState = new Die(npc, agent, anim, player, doPatrol, patrolCheckpoints);
            stage = EVENT.EXIT;
        }
        else if (isHit)
        {
            nextState = new Hit(npc, agent, anim, player, doPatrol, patrolCheckpoints);
            shoot.Stop();
            stage = EVENT.EXIT;
        }
        else if (!CanAttackPlayer())
        {
            nextState = new Pursue(npc, agent, anim, player, doPatrol, patrolCheckpoints);
            shoot.Stop();
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isShooting");
        shoot.Stop();
        base.Exit();
    }
}

public class Hit : GuardState
{
    float clipDuration = 1f;

    public Hit(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, bool _doPatrol, List<GameObject> _patrolCheckpoints)
        : base(_npc, _agent, _anim, _player, _doPatrol, _patrolCheckpoints)
    {
        name = STATE.HIT;
    }

    public override void Enter()
    {
        anim.SetTrigger("isHit");
        agent.isStopped = true;
        agent.speed = 0;
        clipDuration = 1f;
        base.Enter();
    }

    public override void Update()
    {
        if (clipDuration >= 0)
        {
            clipDuration -= 1f * Time.deltaTime;
            return;
        }

        if (isDead)
        {
            nextState = new Die(npc, agent, anim, player, doPatrol, patrolCheckpoints);
            stage = EVENT.EXIT;
        }
        else if (CanSeePlayer() || CanHearPlayer())
        {
            nextState = new Pursue(npc, agent, anim, player, doPatrol, patrolCheckpoints);
            stage = EVENT.EXIT;
        }
        else if (doPatrol && Random.Range(0, 100) < 10)
        {
            nextState = new Patrol(npc, agent, anim, player, doPatrol, patrolCheckpoints);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        isHit = false;
        anim.ResetTrigger("isHit");
        base.Exit();
    }
}

public class Die : GuardState
{
    public Die(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, bool _doPatrol, List<GameObject> _patrolCheckpoints)
        : base(_npc, _agent, _anim, _player, _doPatrol, _patrolCheckpoints)
    {
        name = STATE.DEAD;
    }

    public override void Enter()
    {
        anim.SetTrigger("isDead");
        agent.isStopped = true;
        agent.speed = 0;
        base.Enter();
    }
}

/*public class RunAway : GuardState
{
    GameObject safeLocation;

    public RunAway(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, bool _doPatrol, List<GameObject> _patrolCheckpoints)
        : base(_npc, _agent, _anim, _player, _doPatrol, _patrolCheckpoints)
    {
        name = STATE.RUNAWAY;
        safeLocation = GameObject.FindGameObjectWithTag("Safe");
    }

    public override void Enter()
    {
        anim.SetTrigger("isRunning");
        agent.isStopped = false;
        agent.speed = 6;
        agent.SetDestination(safeLocation.transform.position);
        base.Enter();
    }

    public override void Update()
    {
        if (agent.remainingDistance < 1.0f)
        {
            nextState = new Idle(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isRunning");
        base.Exit();
    }
}*/