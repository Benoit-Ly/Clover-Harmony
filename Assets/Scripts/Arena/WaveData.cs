using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Arena/Wave", order = 1)]
public class WaveData : ScriptableObject
{
    public List<EnemySet> EnemySet = new List<EnemySet>();

    EnemyManager m_EnemyManager;
    float m_Offset = 15f;

    List<float> m_Slots;
    List<float> m_UsedSlots = new List<float>();

    private void OnEnable()
    {
        InitSlot();
    }

    private void InitSlot()
    {
        m_Slots = new List<float>();

        for (float slot = 2f; slot > -3f; slot -= 0.5f)
        {
            m_Slots.Add(slot);
        }
    }

    private float GetRandomSlot()
    {
        float result = -10f;
        if (m_Slots.Count > 0)
        {
            int rand = Random.Range(0, m_Slots.Count);
            result = ConsumeSlot(rand, true);
        }
        else
        {
            int rand = Random.Range(0, m_UsedSlots.Count);
            result = ConsumeSlot(rand, false);
        }

        return result;
    }

    private float ConsumeSlot(int idx, bool first)
    {
        float slotResult = 0f;
        if (first)
        {
            slotResult = m_Slots[idx];
            m_UsedSlots.Add(slotResult);
            m_Slots.RemoveAt(idx);
        }
        else
        {
            slotResult = m_UsedSlots[idx];
            m_Slots.Add(slotResult);
            m_UsedSlots.RemoveAt(idx);
        }

        return slotResult;
    }

    public void Spawn(Vector3 center, TriggerAbstract trigger)
    {
        m_EnemyManager = ServiceLocator.Instance.GetService<EnemyManager>(ManagerType.ENEMY_MANAGER);
        
        foreach (EnemySet set in EnemySet)
        {
            for (int i = 0; i < set.Number; ++i)
            {
                m_EnemyManager.SpawnEnemy(set.Enemy, GetEnemyPosition(center, set.RightSide), trigger);
            }
        }
    }

    private Vector3 GetEnemyPosition(Vector3 center, bool rightSide)
    {
        float zPos = GetRandomSlot();
        float xPos = rightSide ? center.x + m_Offset : center.x - m_Offset;
        float yPos = 0.2f;

        return new Vector3(xPos, yPos, zPos);
    }
}
