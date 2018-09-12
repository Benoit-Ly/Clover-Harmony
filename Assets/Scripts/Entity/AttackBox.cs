using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBox : MonoBehaviour {

    #region Services
    BattleManager m_BattleManager;
    #endregion

    #region Components
    BoxCollider m_BoxCollider;
    #endregion

    #region Vars
    [SerializeField]
    Entity m_Owner;

    // Variables for Mecanim
    [SerializeField]
    AttackType m_Type;
    [SerializeField]
    LaunchDirection m_LaunchDirection;
    [SerializeField]
    int m_Damage;
    [SerializeField]
    int m_StunDuration;
    #endregion

    // Use this for initialization
    void Start () {
        m_BattleManager = ServiceLocator.Instance.GetService<BattleManager>(ManagerType.BATTLE_MANAGER);

        m_BoxCollider = GetComponent<BoxCollider>();
        m_BoxCollider.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate(bool active)
    {
        m_BoxCollider.enabled = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        AttackStats stats = m_Owner.GetCurrentAttack();

        if (stats == null)
            return;

        m_Owner.QuickFreeze();

        Vector3 direction = Vector3.zero;
        direction.x = other.transform.position.x - transform.position.x;
        direction = direction.normalized;
        stats.horizontalDir = direction.x;

        stats.damageProvider = m_Owner;

        if (stats.launchDirection == LaunchDirection.MOVE)
        {
            switch(stats.axis)
            {
                case AttackStats.Axis.X:
                    {
                        Vector3 extents = m_BoxCollider.bounds.extents;
                        stats.targetPosition = transform.TransformPoint(m_BoxCollider.center).x + extents.x * direction.x;
                        break;
                    }

                case AttackStats.Axis.Z:
                    {
                        stats.targetPosition = transform.position.z;
                        break;
                    }

                default: break;
            }
        }

        if (other.gameObject.layer != LayerMask.NameToLayer("BrickHitbox"))
            m_BattleManager.ReportAttack(m_Owner, other.GetComponent<Entity>(), stats);
        else
        {
            IDamageable temp = other.gameObject.GetComponent<IDamageable>();
            if (temp != null)
                temp.TakeDamage(stats);

            m_BattleManager.ReportBrickAttack(m_Owner, other.transform.position);
        }
            //m_BattleManager.ReportBrickDestruction(other.GetComponent<IDamageable>(), stats);
    }
}
