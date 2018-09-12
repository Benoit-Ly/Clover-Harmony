using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : Controller {

    #region Vars
    AITactician m_Tactician;

    Player m_Target;                                    public Player Target { get { return m_Target; } set { m_Target = value; } }
    Enemy m_Enemy;
    AIState m_CurrentState = AIState.STANDBY;           public AIState CurrentState { get { return m_CurrentState; } set { m_CurrentState = value; } }

    Vector3 m_Direction = Vector3.zero;
    Vector2 m_StandbyDirection = Vector3.zero;

    Vector3 m_TacticalPosition = Vector3.zero;

    float m_WaitingDistanceToPlayer = 5f;
    float m_AttackDistanceToPlayer = 2f;

    [SerializeField]
    float m_AttackDelay = 0.5f;
    float m_CurrentAttackSeconds = 0f;

    float m_WaitDelay = 1f;
    float m_CurrentWaitSeconds = 0f;

    int m_MaxAttackNumber = 2;
    int m_CurrentAttackNumber = 0;

    bool m_IsAlive = true;

    [Range(0.6f, 1f)]
    float m_MinTacticalDistance = 0.6f;
    #endregion

    // Use this for initialization
    protected override void Start () {
        m_Enemy = GetComponent<Enemy>();

        m_StandbyDirection = Random.insideUnitCircle.normalized;

        m_Tactician = ServiceLocator.Instance.GetService<AITactician>(ManagerType.AI_TACTICIAN);
        m_Tactician.RegisterAI(this);

        m_Enemy.OnKnockback += InterruptAttack;
        m_Enemy.OnDeath += OnDeath;

        base.Start();
	}

    // Update is called once per frame
    void FixedUpdate () {
        if (!m_IsAlive)
            return;

        m_Direction = Vector3.zero;

		switch (m_CurrentState)
        {
            case AIState.POSITION:
                {
                    UpdatePosition();
                    break;
                }

            case AIState.AGRESSION:
                {
                    UpdateAttack();
                    break;
                }

            case AIState.PULLBACK:
                {
                    UpdatePullBack();
                    break;
                }

            case AIState.STANDBY:
                {
                    UpdateRandomPosition();
                    break;
                }

            default: break;
        }

        m_Enemy.Move(m_Direction.x, m_Direction.z);
    }

    void UpdateRandomPosition()
    {
        m_Tactician.ReportWaitingForOrder(this);
        //m_CurrentWaitSeconds += Time.fixedDeltaTime;

        //m_Direction.x = m_StandbyDirection.x;
        //m_Direction.z = m_StandbyDirection.y;

        //if (m_CurrentWaitSeconds >= m_WaitDelay)
        //{
        //    m_CurrentWaitSeconds = 0f;
        //    m_Tactician.ReportWaitingForOrder(this);
        //}
    }

    void UpdatePosition()
    {
        if (!m_Target)
            m_Target = m_Tactician.RequestTarget();

        if (!IsFarFromTarget())
            ReturnToStandby();
        else
        {
            m_Direction = (m_Target.transform.position - transform.position).normalized;
        }
            
    }

    void UpdatePullBack()
    {
        if (!m_Target)
            m_Target = m_Tactician.RequestTarget();

        if (IsFarFromTarget())
            ReturnToStandby();
        else
            m_Direction = -(m_Target.transform.position - transform.position).normalized;
    }

    void UpdateAttack()
    {
        if (!m_Target)
            m_Target = m_Tactician.RequestTarget();

        // If player got killed
        if (m_Target.HP == 0)
        {
            m_Tactician.ReportAttackFinished(this);
            ReturnToStandby();
            return;
        }

        int layerMask = 1 << LayerMask.NameToLayer("Default");
        Vector3 direction = (m_TacticalPosition - transform.position).normalized;
        bool isBlocked = Physics.Raycast(transform.position, direction, Mathf.Infinity, layerMask);

        if (isBlocked)
            m_TacticalPosition.z *= -1f;

        if (!IsFarFronTacticalPosition())
        {
            if (m_CurrentAttackSeconds == 0f)
            {
                FaceTarget();
                m_Enemy.Attack();
                m_CurrentAttackNumber++;
            }

            m_CurrentAttackSeconds += Time.fixedDeltaTime;

            if (m_CurrentAttackSeconds >= m_AttackDelay)
            {
                m_CurrentAttackSeconds = 0f;

                if (m_CurrentAttackNumber == m_MaxAttackNumber)
                {
                    m_CurrentAttackSeconds = -0.25f;
                    m_CurrentAttackNumber = 0;
                }
            }
        }
        else if (m_CurrentAttackSeconds == 0f)
        {
            Vector3 destination = GetClosestDestination();
            m_Direction = (destination - transform.position).normalized;
        }
        else
        {
            m_CurrentAttackSeconds += Time.fixedDeltaTime;

            if (m_CurrentAttackSeconds >= m_AttackDelay)
                m_CurrentAttackSeconds = 0f;
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (m_Target.transform.position - transform.position).normalized;

        m_Enemy.FaceDirection(direction);
    }

    public bool IsFarFromTarget()
    {
        float distance = GetDistance();

        Vector3 flatPosition = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 targetFlatPosition = new Vector3(m_Target.transform.position.x, 0f, m_Target.transform.position.z);

        float currentDistance = Vector3.Distance(flatPosition, targetFlatPosition);

        return (distance <= currentDistance);
    }

    public void OrderPosition()
    {
        m_CurrentState = AIState.POSITION;
    }

    public void OrderPullBack()
    {
        m_CurrentState = AIState.PULLBACK;
    }

    public void OrderAttack(Vector3 tacticalPosition)
    {
        m_TacticalPosition = tacticalPosition;
        m_CurrentState = AIState.AGRESSION;
    }

    void InterruptAttack()
    {
        m_Tactician.ReportAttackFinished(this);
        ReturnToStandby();
    }

    void ReturnToStandby()
    {
        m_CurrentState = AIState.STANDBY;
        m_StandbyDirection = Random.insideUnitCircle.normalized;
    }

    float GetDistance()
    {
        if (m_CurrentState == AIState.AGRESSION)
            return m_AttackDistanceToPlayer;
        else
            return m_WaitingDistanceToPlayer;
    }

    Vector3 GetClosestDestination()
    {
        return m_Target.transform.position + m_TacticalPosition * m_AttackDistanceToPlayer;
    }

    bool IsFarFronTacticalPosition()
    {
        Vector3 flatDestination = GetClosestDestination();
        flatDestination.y = 0f;

        Vector3 flatPosition = transform.position;
        flatPosition.y = 0f;

        float distance = Vector3.Distance(flatPosition, flatDestination);

        return (distance > m_MinTacticalDistance);
    }

    void OnDeath()
    {
        m_IsAlive = false;
        m_Tactician.ReportDeath(this);
    }

    private void OnDisable()
    {
        m_Enemy.Move(0f, 0f);
    }

    private void OnDestroy()
    {
        OnDeath();
        m_Enemy.OnKnockback -= InterruptAttack;
        m_Enemy.OnDeath -= OnDeath;
    }
}
