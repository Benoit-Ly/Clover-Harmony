using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePhysics : ScriptableObject {

    [System.Serializable]
    public struct Knock
    {
        public float Angle;
        public float Force;
        public float Speed;
    };

    // Knockback
    [SerializeField]
    Knock m_Knockback;              public Knock Knockback { get { return m_Knockback; } set { m_Knockback = value; } }

    // Knockup
    [SerializeField]
    Knock m_Knockup;                public Knock Knockup { get { return m_Knockup; } set { m_Knockup = value; } }

    // Knockdown
    [SerializeField]
    Knock m_Knockdown;              public Knock Knockdown { get { return m_Knockdown; } set { m_Knockdown = value; } }
}
