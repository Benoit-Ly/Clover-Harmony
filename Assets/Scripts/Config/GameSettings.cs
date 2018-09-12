using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : ScriptableObject {

    //[SerializeField]
    //int m_MaxLives = 3;             public int MaxLives { get { return m_MaxLives; } set { m_MaxLives = value; } }

    //[SerializeField]
    //float m_FusionSpeed = 30f;      public float FusionSpeed { get { return m_FusionSpeed; } set { m_FusionSpeed = value; } }

    [SerializeField]
    int m_FusionKillRatio = 40;         public int FusionKillRatio { get { return m_FusionKillRatio; } set { m_FusionKillRatio = value; } }

    [SerializeField]
    int m_FusionTimeRatio = 130;        public int FusionTimeRatio { get { return m_FusionTimeRatio; } set { m_FusionTimeRatio = value; } }

    [SerializeField]
    int m_FusionHitRatio = 200;         public int FusionHitRatio { get { return m_FusionHitRatio; } set { m_FusionHitRatio = value; } }

    [SerializeField]
    int m_MaxFusionRatio = 6;           public int MaxFusionRatio { get { return m_MaxFusionRatio; } set { m_MaxFusionRatio = value; } }

    [SerializeField]
    int m_MinFusionCap = 200;           public int MinFusionCap { get { return m_MinFusionCap; } set { m_MinFusionCap = value; } }

    [SerializeField]
    int m_MaxFusionCap = 1200;          public int MaxFusionCap { get { return m_MaxFusionCap; } set { m_MaxFusionCap = value; } }

    [SerializeField]
    int m_FusionConsumptionAmount = 200; public int FusionConsumptionAmount { get { return m_FusionConsumptionAmount; } set { m_FusionConsumptionAmount = value; } }
}
