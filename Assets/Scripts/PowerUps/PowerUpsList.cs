using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsList : ScriptableObject {

    // 11, 12, 13
    [SerializeField]
    PowerUp m_PowerAttack;          public PowerUp PowerAttack { get { return m_PowerAttack; } set { m_PowerAttack = value; } }

    // 22, 23
    [SerializeField]
    PowerUp m_PowerRetention;       public PowerUp PowerRetention { get { return m_PowerRetention; } set { m_PowerRetention = value; } }

    // 9, 8, 7
    [SerializeField]
    PowerUp m_AirAttack;            public PowerUp AirAttack { get { return m_AirAttack; } set { m_AirAttack = value; } }

    // 21
    [SerializeField]
    PowerUp m_CrowdControl;         public PowerUp CrowdControl { get { return m_CrowdControl; } set { m_CrowdControl = value; } }

    // -1, -2, -3
    [SerializeField]
    PowerUp m_PowerCharges;         public PowerUp PowerCharges { get { return m_PowerCharges; } set { m_PowerCharges = value; } }

    // 19
    [SerializeField]
    PowerUp m_PowerAbsorption;      public PowerUp PowerAbsorption { get { return m_PowerAbsorption; } set { m_PowerAbsorption = value; } }

    // 1, 2, 3
    [SerializeField]
    PowerUp m_Resilience;           public PowerUp Resilience { get { return m_Resilience; } set { m_Resilience = value; } }

    // 0, 10, 20
    [SerializeField]
    PowerUp m_LightAttack;          public PowerUp LightAttack { get { return m_LightAttack; } set { m_LightAttack = value; } }

    public void UpgradePowers(ref PowerUpsLevels levels, int id)
    {
        UpdateLevel(m_PowerAttack, ref levels.PowerAttack, id);
        UpdateLevel(m_PowerRetention, ref levels.PowerRetention, id);
        UpdateLevel(m_AirAttack, ref levels.AirAttack, id);
        UpdateLevel(m_CrowdControl, ref levels.CrowdControl, id);
        UpdateLevel(m_PowerCharges, ref levels.PowerCharges, id);
        UpdateLevel(m_PowerAbsorption, ref levels.PowerAbsorption, id);
        UpdateLevel(m_Resilience, ref levels.Resilience, id);
        UpdateLevel(m_LightAttack, ref levels.LightAttack, id);
    }

    void UpdateLevel(PowerUp power, ref int currentLevel, int id)
    {
        int newLevel = power.GetLevelByID(id);

        if (newLevel > currentLevel)
            currentLevel = newLevel;
    }
}
