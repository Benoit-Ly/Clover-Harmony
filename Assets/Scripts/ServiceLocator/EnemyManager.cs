using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EnemyManager : Service
{
    #region Services
    BattleManager m_BattleManager;
    #endregion

    #region Events
    public event Action<Entity, int> OnEnemyDeath;
    #endregion

    #region Vars
    List<Enemy> m_EnemyList;

    public int m_CurrentEnemyCount = 0;
    #endregion

    private void Awake()
    {
        m_Type = ManagerType.ENEMY_MANAGER;

        m_EnemyList = new List<Enemy>();

        m_BattleManager = ServiceLocator.Instance.GetService<BattleManager>(ManagerType.BATTLE_MANAGER);
        m_BattleManager.OnDamage += OnEnemyDamage;
        m_BattleManager.OnEnterWeakAoE += OnEnterWeakAoE;
        m_BattleManager.OnExitWeakAoE += OnExitWeakAoE;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void RegisterEnemy(Enemy newEnemy)
    {
        m_EnemyList.Add(newEnemy);
    }

    public void ReportEnemySpawn()
    {
        m_CurrentEnemyCount++;
    }

    void OnEnemyDamage(Entity owner, Entity target, AttackStats stats)
    {
        if (target.gameObject.layer == LayerMask.NameToLayer("EnemyHitBox"))
        {
            target.TakeDamage(stats);
        }
    }

    public void ReportEnemyDeath(Enemy enemy, Entity killer, int vitae)
    {
        m_CurrentEnemyCount--;

        m_EnemyList.Remove(enemy);

        if (OnEnemyDeath != null)
            OnEnemyDeath(killer, vitae);
    }

    void OnEnterWeakAoE(GameObject target)
    {
        if (target.tag == "Enemy")
        {
            Enemy enemy = target.GetComponent<Enemy>();

            enemy.DamageCoeff = 2;
        }
    }

    void OnExitWeakAoE(GameObject target)
    {
        if (target.tag == "Enemy")
        {
            Enemy enemy = target.GetComponent<Enemy>();

            enemy.DamageCoeff = 1;
        }
    }

    private void OnDestroy()
    {
        m_BattleManager.OnDamage -= OnEnemyDamage;
        m_BattleManager.OnEnterWeakAoE -= OnEnterWeakAoE;
        m_BattleManager.OnExitWeakAoE -= OnExitWeakAoE;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SpawnEnemy(GameObject prefab, Vector3 position, TriggerAbstract trigger)
    {
        if (prefab)
        {
            Enemy enemy = Instantiate(prefab, position, Quaternion.identity, null).GetComponent<Enemy>();
            enemy.SetTrigger(trigger);
            m_CurrentEnemyCount++;
        }
        else
        {
            Debug.LogWarning("try to spawn an ArenaEnemy with no prefab");
        }
    }

    public void RemoveAllEnemies()
    {
        int nbEnemies = m_EnemyList.Count;
        for (int i = 0; i < nbEnemies; i++)
        {
            m_EnemyList[i].ReactiveTrigger();
            DestroyImmediate(m_EnemyList[i].gameObject);
        }

        m_EnemyList.Clear();

        m_CurrentEnemyCount = 0;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        m_EnemyList.Clear();
        m_CurrentEnemyCount = 0;
    }
}
