using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : Service {

    #region Services
    GUIManager m_GUIManager;
    #endregion

    // TODO: Clean that
    #region HUD
    FusionMenu m_FusionMenu;
    #endregion

    #region Events
    public event Action<Entity, Entity, AttackStats> OnDamage;
    public event Action<Entity, Vector3> OnBrickDamage;
    public event Action<GameObject> OnEnterWeakAoE;
    public event Action<GameObject> OnExitWeakAoE;
    #endregion

    #region Vars
    GameObject m_WeakAoEPrefab;
    GameObject m_WeakAoE;
    #endregion

    private void Awake()
    {
        m_Type = ManagerType.BATTLE_MANAGER;

        m_WeakAoEPrefab = Resources.Load<GameObject>("Prefabs/WeakArea");

        
    }

    public void SummonAoE(Vector3 position)
    {
        if (m_WeakAoE == null)
            m_WeakAoE = Instantiate<GameObject>(m_WeakAoEPrefab);

        m_WeakAoE.transform.position = position;
        m_WeakAoE.SetActive(true);

        ServiceLocator.Instance.StartServiceCoroutine(AoECountdown(10f));
    }

    void EndAoE()
    {
        m_WeakAoE.SetActive(false);
    }

    public void ReportAttack(Entity owner, Entity target, AttackStats attackStats)
    {
        if (OnDamage != null && owner != null && target != null)
            OnDamage(owner, target, attackStats);
    }

    public void ReportBrickAttack(Entity owner, Vector3 brickPosition)
    {
        if (OnBrickDamage != null && owner != null)
            OnBrickDamage(owner, brickPosition);
    }

    public void ReportEnterWeakAoE(GameObject target)
    {
        if (OnEnterWeakAoE != null)
            OnEnterWeakAoE(target);
    }

    public void ReportExitWeakAoE(GameObject target)
    {
        if (OnExitWeakAoE != null)
            OnExitWeakAoE(target);
    }

    IEnumerator AoECountdown(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (m_WeakAoE != null)
            m_WeakAoE.SetActive(false);
    }
}
