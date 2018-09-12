using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PowerLevel
{
    public string Name;
    public int id;

    public string CloverDescription;
    public float CloverEffectValue;

    public string AmaranthDescription;
    public float AmaranthEffectValue;

    public float CostMultiplier;
    public int VitaePrice;
}

[System.Serializable]
public class PowerUp {

    [SerializeField]
    List<PowerLevel> m_Levels;                  public List<PowerLevel> Levels { get { return m_Levels; } set { m_Levels = value; } }

    public float GetMeleeValue(int level)
    {
        if (level == 0)
            return 0f;

        return m_Levels[level - 1].CloverEffectValue;
    }

    public float GetDistanceValue(int level)
    {
        if (level == 0)
            return 0f;

        return m_Levels[level - 1].AmaranthEffectValue;
    }

    public int GetLevelByID(int id)
    {
        int nbLevels = m_Levels.Count;
        for (int i = 0; i < nbLevels; i++)
        {
            if (m_Levels[i].id == id)
                return i + 1;
        }

        return 0;
    }
}