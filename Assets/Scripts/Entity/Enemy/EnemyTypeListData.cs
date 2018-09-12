using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTypeListData", menuName = "Enemy Data", order = 1)]
public class EnemyTypeListData : ScriptableObject
{
    [SerializeField]
    EnemyTypeData m_FastEnemyData;          public EnemyTypeData FastEnemyData { get { return m_FastEnemyData; } set { m_FastEnemyData = value; } }

    [SerializeField]
    EnemyTypeData m_HeavyEnemyData;         public EnemyTypeData HeavyEnemyData { get { return m_HeavyEnemyData; } set { m_HeavyEnemyData = value; } }

    public EnemyTypeData GetDataFromType(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.FAST:
                return m_FastEnemyData;

            case EnemyType.HEAVY:
                return m_HeavyEnemyData;
        }

        return null;
    }
}

[System.Serializable]
public class EnemyTypeData {

    [SerializeField]
    float m_Speed = 10f;                public float Speed { get { return m_Speed; } set { m_Speed = value; } }

    [SerializeField]
    int m_VitaeDropQuantity = 1;        public int VitaeDropQuantity { get { return m_VitaeDropQuantity; } set { m_VitaeDropQuantity = value; } }

    [SerializeField]
    float m_DashDistance = 10f;         public float DashDistance { get { return m_DashDistance; } set { m_DashDistance = value; } }

    [SerializeField]
    int m_Agressivity;                  public int Agressivity { get { return m_Agressivity; } set { m_Agressivity = value; } }

    [SerializeField]
    List<AttackStats> m_AttackList;     public List<AttackStats> AttackList { get { return m_AttackList; } set { m_AttackList = value; } }

    public AttackStats GetAttack(string name)
    {
        int nbAttacks = m_AttackList.Count;
        for (int i = 0; i < nbAttacks; i++)
        {
            if (m_AttackList[i].name == name)
                return m_AttackList[i];
        }

        return null;
    }
}
