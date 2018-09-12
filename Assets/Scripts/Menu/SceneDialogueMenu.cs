using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SceneDialogueMenu : MonoBehaviour
{
    [SerializeField] SceneDialogue m_SceneDialogue;
    [SerializeField] GameObject m_Background;
    [SerializeField] Text m_CloverText;
    [SerializeField] Text m_AmaranthText;

    int m_CurrentTextsOfThoughts = 0;

    private void Start()
    {
        m_Background.GetComponent<Image>().sprite = m_SceneDialogue.Background;
        SetTextsOfThoughts(m_SceneDialogue.TextsOfThoughtsList[m_CurrentTextsOfThoughts]);
    }

    private void SetTextsOfThoughts(TextsOfThoughts textsOfThoughts)
    {
        float harmonie = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER).HarmonyRate;

        if (harmonie <= 0.3f)
        {
            m_CloverText.text = textsOfThoughts.CloverTextLow;
            m_AmaranthText.text = textsOfThoughts.AmaranthTextLow;
        }
        else if (harmonie > 0.3f && harmonie <= 0.7f)
        {
            m_CloverText.text = textsOfThoughts.CloverTextMedium;
            m_AmaranthText.text = textsOfThoughts.AmaranthTextMedium;
        }
        else if (harmonie > 0.7f)
        {
            m_CloverText.text = textsOfThoughts.CloverTextHight;
            m_AmaranthText.text = textsOfThoughts.AmaranthTextHight;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("P1_Jump") || Input.GetButtonDown("P2_Jump"))
        {
            ++m_CurrentTextsOfThoughts;

            if (m_CurrentTextsOfThoughts < m_SceneDialogue.TextsOfThoughtsList.Count)
                SetTextsOfThoughts(m_SceneDialogue.TextsOfThoughtsList[m_CurrentTextsOfThoughts]);
            else
                ServiceLocator.Instance.GetService<LevelManager>(ManagerType.LEVEL_MANAGER).LoadNextLevel();
        }
    }
}
