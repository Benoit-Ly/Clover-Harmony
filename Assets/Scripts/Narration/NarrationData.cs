using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NarrationData : ScriptableObject
{
    [SerializeField]
    List<NarrativeScreen> m_NarrativeScreen; public List<NarrativeScreen> NarrativeScreen { get { return m_NarrativeScreen; } set { m_NarrativeScreen = value; } }

    public NarrativeScreen GetNarrative(string name)
    {
        int nbAttacks = m_NarrativeScreen.Count;
        for (int i = 0; i < nbAttacks; i++)
        {
            if (m_NarrativeScreen[i].name == name)
                return m_NarrativeScreen[i];
        }

        return null;
    }
}
