using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : ScriptableObject {

    [SerializeField]
    int m_HP;                           public int HP { get { return m_HP; } set { m_HP = value; } }

    [SerializeField]
    int m_Energy;                       public int Energy { get { return m_Energy; } set { m_Energy = value; } }

    [SerializeField]
    float m_Speed;                      public float Speed { get { return m_Speed; } set { m_Speed = value; } }

    [SerializeField]
    float m_DebuffSpeed;                public float DebuffSpeed { get { return m_DebuffSpeed; } set { m_DebuffSpeed = value; } }

    [SerializeField]
    float m_VerticalSpeed;              public float VerticalSpeed { get { return m_VerticalSpeed; } set { m_VerticalSpeed = value; } }

    [SerializeField]
    float m_DebuffVerticalSpeed;        public float DebuffVerticalSpeed { get { return m_DebuffVerticalSpeed; } set { m_DebuffVerticalSpeed = value; } }

    [SerializeField]
    float m_JumpHeight;                 public float JumpHeight { get { return m_JumpHeight; } set { m_JumpHeight = value; } }

    [SerializeField]
    float m_JumpSpeed;                  public float JumpSpeed { get { return m_JumpSpeed; } set { m_JumpSpeed = value; } }

    [SerializeField]
    List<AttackStats> m_AttackList;         public List<AttackStats> AttackList { get { return m_AttackList; } set { m_AttackList = value; } }

    public AttackStats GetAttack(string name)
    {
        int nbAttacks = m_AttackList.Count;
        for (int i = 0; i < nbAttacks; i++)
        {
            // We return the copy to avoid permanent data modification by power ups, etc...
            if (m_AttackList[i].name == name)
                return new AttackStats(m_AttackList[i]);
        }

        Debug.Log("Could not find attack : " + name);

        return null;
    }
}
