using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FusionBarController : MonoBehaviour
{
    #region Services
    PlayerManager m_PlayerManager;
    #endregion

    [SerializeField] Slider m_CloverSlider;
    [SerializeField] Slider m_AmaranthSlider;
    Animator m_Animator;

    public void Init()
    {
        m_Animator = GetComponentInChildren<Animator>();

        m_PlayerManager = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER);
        m_PlayerManager.OnFusionScoreChange += FusionBarController_OnFusionScoreChange;
        m_PlayerManager.OnFusionAvailable += PlayerManager_OnFusionAvailable;
    }

    private void PlayerManager_OnFusionAvailable(bool isAvailable)
    {
        if (isAvailable)
        {
            m_Animator.SetTrigger("Active");
            ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_FusionReady", 3);
        }
        else
            m_Animator.SetTrigger("Desactive");
    }

    private void FusionBarController_OnFusionScoreChange(int current, int max)
    {
        m_CloverSlider.value = (float)current / (float)max;
        m_AmaranthSlider.value = (float)current / (float)max;
    }

    private void OnDestroy()
    {
        m_PlayerManager.OnFusionScoreChange -= FusionBarController_OnFusionScoreChange;
        m_PlayerManager.OnFusionAvailable -= PlayerManager_OnFusionAvailable;
    }
}
