using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum AIState
{
    STANDBY = 0,
    POSITION,
    PULLBACK,
    AGRESSION
}

public class AITactician : Service {

    #region Services
    PlayerManager m_PlayerManager;
    #endregion

    #region Vars
    AITacticalPosition[] m_TacticalPositions;

    //List<AIController> m_AIList;
    List<AIController> m_Attackers;
    //int m_CurrentTarget = 1;
    int m_CurrentAttackersNum = 0;
    int m_MaxAttackers = 4;
    #endregion

    private void Awake()
    {
        m_Type = ManagerType.AI_TACTICIAN;

        m_Attackers = new List<AIController>();

        m_PlayerManager = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER);
        m_PlayerManager.OnRegisterPlayer += RegisterTarget;

        InitTacticalPosition();

        SceneManager.sceneLoaded += ResetAttackers;
    }

    void InitTacticalPosition()
    {
        m_TacticalPositions = new AITacticalPosition[2];

        int nbTargets = m_TacticalPositions.Length;
        for (int i = 0; i < nbTargets; i++)
        {
            Player target = m_PlayerManager.GetPlayer(i + 1);

            if (target)
                m_TacticalPositions[i] = new AITacticalPosition(target);
        }
    }

    public void RegisterTarget(Player target)
    {
        m_TacticalPositions[target.ID - 1] = new AITacticalPosition(target);
    }

    public void RegisterAI(AIController controller)
    {
        //m_AIList.Add(controller);
        //controller.Target = m_PlayerManager.GetPlayer(m_CurrentTarget);
        //controller.OrderPosition();

        //if (m_CurrentTarget == 1)
        //    m_CurrentTarget = 2;
        //else
        //    m_CurrentTarget = 1;

        controller.Target = RequestTarget();
        controller.OrderPosition();
    }

    public Player RequestTarget()
    {
        //Player target = m_PlayerManager.GetPlayer(m_CurrentTarget);

        //if (m_CurrentTarget == 1)
        //    m_CurrentTarget = 2;
        //else
        //    m_CurrentTarget = 1;

        //return target;

        int cloverAggro = m_TacticalPositions[0].GetAggroNumber();
        int amaranthAggro = m_TacticalPositions[1].GetAggroNumber();
        int playerID = 1;

        if (cloverAggro > amaranthAggro)
        {
            playerID = 2;
        }
        else if (cloverAggro == amaranthAggro)
        {
            playerID = Random.Range(1, 3);
        }

        return m_PlayerManager.GetPlayer(playerID);
    }

    public Vector3 RequestTacticalPosition(AIController controller)
    {
        Player target = controller.Target;

        if (target == null)
            target = RequestTarget();

        Vector3 currentPosition = controller.transform.position;

        AITacticalPosition tacticalPosition = m_TacticalPositions[target.ID - 1];
        AITacticalPosition.Space freeSpace = tacticalPosition.GetFreeSpace();

        Vector3 targetPosition = Vector3.zero;

        switch (freeSpace)
        {
            case AITacticalPosition.Space.LEFT:
                {
                    targetPosition = -Vector3.right;
                    break;
                }

            case AITacticalPosition.Space.RIGHT:
                {
                    targetPosition = Vector3.right;
                    break;
                }

            case AITacticalPosition.Space.BOTH:
                {
                    if (currentPosition.x >= target.transform.position.x)
                    {
                        targetPosition = Vector3.right;
                        freeSpace = AITacticalPosition.Space.RIGHT;
                    }
                    else
                    {
                        targetPosition = -Vector3.right;
                        freeSpace = AITacticalPosition.Space.LEFT;
                    }

                    break;
                }
        }

        float random = Random.Range(-0.3f, 0.3f);
        targetPosition.z = random;
        targetPosition.Normalize();

        tacticalPosition.AddAI(controller, freeSpace);

        return targetPosition;
    }

    public void ReportWaitingForOrder(AIController controller)
    {
        //if (controller.IsFarFromTarget() || m_CurrentAttackersNum >= m_MaxAttackers)
        //{
        //    controller.OrderPosition();
        //}
        //else
        //{
        //    controller.OrderAttack();
        //    m_CurrentAttackersNum++;
        //}

        //Debug.Log("Wait for order");

        int layerMask = 1 << LayerMask.NameToLayer("PlayerHitBox");
        Collider[] playersInSight = Physics.OverlapSphere(controller.transform.position, 8f, layerMask);
        int nbPlayers = playersInSight.Length;

        if (nbPlayers > 0)
        {
            if (nbPlayers > 1)
            {
                int cloverAggro = m_TacticalPositions[0].GetAggroNumber();
                int amaranthAggro = m_TacticalPositions[1].GetAggroNumber();

                int playerID = 1;

                if (cloverAggro > amaranthAggro)
                {
                    playerID = 2;
                }
                else if (cloverAggro == amaranthAggro)
                {
                    playerID = Random.Range(1, 3);
                }

                controller.Target = GetPlayerByID(playerID, playersInSight);
            }
            else
                controller.Target = playersInSight[0].GetComponent<Player>();

            m_CurrentAttackersNum = m_Attackers.Count;

            if (m_CurrentAttackersNum >= m_MaxAttackers)
                controller.OrderPullBack();
            else
            {
                Vector3 tacticalPosition = RequestTacticalPosition(controller);
                controller.OrderAttack(tacticalPosition);
                m_Attackers.Add(controller);
            }
        }
        else
        {
            controller.OrderPosition();
        }
    }

    Player GetPlayerByID(int id, Collider[] colliders)
    {
        int nbPlayers = colliders.Length;

        for (int i = 0; i < nbPlayers; i++)
        {
            Player player = colliders[i].GetComponent<Player>();

            if (player.ID == id)
                return player;
        }

        return null;
    }

    public void ReportAttackFinished(AIController controller)
    {
        if (controller.Target)
        {
            int targetID = controller.Target.ID;
            m_TacticalPositions[targetID - 1].RemoveAI(controller);
        }

        m_Attackers.Remove(controller);
        //m_CurrentAttackersNum--;
    }

    void ResetAttackers(Scene scene, LoadSceneMode mode)
    {
        m_Attackers.Clear();
        //m_CurrentAttackersNum = 0;
    }

    public void ReportDeath(AIController controller)
    {
        if (controller.Target)
        {
            int targetID = controller.Target.ID;
            m_TacticalPositions[targetID - 1].RemoveAI(controller);
        }

        if (controller.CurrentState == AIState.AGRESSION)
            m_Attackers.Remove(controller);
            //m_CurrentAttackersNum--;
    }

    private void OnDestroy()
    {
        m_PlayerManager.OnRegisterPlayer -= RegisterTarget;
        SceneManager.sceneLoaded -= ResetAttackers;
    }
}
