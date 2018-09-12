using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] GameObject m_EnergyBarPrefab;
    [SerializeField] GameObject m_CloverHealthBarPrefab;
    [SerializeField] GameObject m_AmaranthHealthBarPrefab;
    [SerializeField] GameObject m_FusionBarPrefab;

    [SerializeField] Text VitaeHUD;

    PlayerManager playerManager;
    Animator m_Animator;

    private void Start()
    {
        if (m_EnergyBarPrefab != null)
        {
            Instantiate(m_EnergyBarPrefab, transform).GetComponent<EnergyBarController>().Init(1);
            Instantiate(m_EnergyBarPrefab, transform).GetComponent<EnergyBarController>().Init(2);
        }
        else
            Debug.LogWarning("EnergyBarPrefab is null");

        if (m_CloverHealthBarPrefab != null)
            Instantiate(m_CloverHealthBarPrefab, transform).GetComponent<HealthBarController>().Init(1);
        else
            Debug.LogWarning("CloverHealthBarPrefab is null");

        if (m_AmaranthHealthBarPrefab != null)
            Instantiate(m_AmaranthHealthBarPrefab, transform).GetComponent<HealthBarController>().Init(2);
        else
            Debug.LogWarning("AmaranthHealthBarPrefab is null");

        if (m_FusionBarPrefab != null)
            Instantiate(m_FusionBarPrefab, transform).GetComponent<FusionBarController>().Init();
        else
            Debug.LogWarning("FusionBarPrefab is null");

        playerManager = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER);
        playerManager.OnVitaeChange += PlayerManager_OnVitaeChange;
        VitaeHUD.text = playerManager.Vitae.ToString();
        m_Animator = GetComponent<Animator>();
    }

    private void PlayerManager_OnVitaeChange(int Vitae)
    {
        VitaeHUD.text = Vitae.ToString();
        m_Animator.SetTrigger("GainVitae");
    }

    private void OnDestroy()
    {
        if (!playerManager)
            Debug.Log("PlayerManager is NULL");

        playerManager.OnVitaeChange -= PlayerManager_OnVitaeChange;
    }
}
