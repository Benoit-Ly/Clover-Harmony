using System;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : Service
{
    #region Services
    PlayerManager m_PlayerManager;
    #endregion

    #region Events
    public event Action OnFusionMenuInstantiation;
    #endregion

    GameObject m_MainMenuPrefab;
    GameObject m_PauseMenuPrefab;
    GameObject m_FusionMenuPrefab;
    GameObject m_ShopMenuPrefab;
    GameObject m_HudPrefab;

    GameObject m_MainMenu;
    GameObject m_PauseMenu;
    GameObject m_FusionMenu;        public GameObject FusionMenu { get { return m_FusionMenu; } }
    GameObject m_ShopMenu;
    GameObject m_HUD;

    public ShopMenu Shop;
    public HUD HUD;

    bool m_CreditsRequested = false;    public bool CreditsRequested { get { return m_CreditsRequested; } set { m_CreditsRequested = value; } }

    private void Awake()
    {
        m_Type = ManagerType.GUI_MANAGER;
    }

    private void OnEnable()
    {
        m_MainMenuPrefab = Resources.Load("Prefabs/GUI/MainMenu") as GameObject;
        m_PauseMenuPrefab = Resources.Load("Prefabs/GUI/PauseMenu") as GameObject;
        m_FusionMenuPrefab = Resources.Load("Prefabs/GUI/FusionMenu") as GameObject;
        m_ShopMenuPrefab = Resources.Load("Prefabs/GUI/ShopMenu") as GameObject;
        m_HudPrefab = Resources.Load("Prefabs/GUI/HUD") as GameObject;
    }

    public void InstantiateMainMenu()
    {
        if (m_MainMenuPrefab == null)
        {
            Debug.LogWarning("MainMenu prefab loading failed!");
            return;
        }

        m_MainMenu = GameObject.Instantiate(m_MainMenuPrefab);
    }

    public void InstantiateLevelMenu()
    {
        if (m_PauseMenuPrefab == null)
        {
            Debug.LogWarning("PauseMenu prefab loading failed!");
            return;
        }

        if (m_FusionMenuPrefab == null)
        {
            Debug.LogWarning("FusionMenu prefab loading failed!");
            return;
        }

        if (m_ShopMenuPrefab == null)
        {
            Debug.LogWarning("ShopMenu prefab loading failed!");
            return;
        }

        if (m_HudPrefab == null)
        {
            Debug.LogWarning("HUD prefab loading failed!");
            return;
        }

        m_PauseMenu = GameObject.Instantiate(m_PauseMenuPrefab);

        m_ShopMenu = GameObject.Instantiate(m_ShopMenuPrefab);
        Shop = m_ShopMenu.GetComponentInChildren<ShopMenu>();

        m_HUD = GameObject.Instantiate(m_HudPrefab);
        HUD = m_HUD.GetComponent<HUD>();

        m_FusionMenu = GameObject.Instantiate(m_FusionMenuPrefab);
        m_PlayerManager = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER);
        m_PlayerManager.OnFusionComplete += ShowFusionMenu;
        m_PlayerManager.OnDefuseComplete += HideFusionMenu;

        m_PauseMenu.SetActive(false);
        m_ShopMenu.SetActive(false);
        m_FusionMenu.SetActive(false);
        m_HUD.SetActive(true);

        if (OnFusionMenuInstantiation != null)
            OnFusionMenuInstantiation();
    }

    public void ShowShopMenu()
    {
        ServiceLocator.Instance.GetService<ControllerManager>(ManagerType.CONTROLLER_MANAGER).EnableControllers(false);
        m_ShopMenu.SetActive(true);
    }

    public void HideShopMenu()
    {
        m_ShopMenu.SetActive(false);
        ServiceLocator.Instance.GetService<ControllerManager>(ManagerType.CONTROLLER_MANAGER).EnableControllers(true);
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Back", 3);
    }

    public void ShowPauseMenu()
    {
        ServiceLocator.Instance.GetService<ControllerManager>(ManagerType.CONTROLLER_MANAGER).EnableControllers(false);
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Pause", 3);
        m_PauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        ServiceLocator.Instance.GetService<ControllerManager>(ManagerType.CONTROLLER_MANAGER).EnableControllers(true);
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Back", 3);
        m_PauseMenu.SetActive(false);
    }

    public void ShowHUD()
    {
        m_HUD.SetActive(true);
    }

    public void HideHUD()
    {
        m_HUD.SetActive(false);
    }

    public void ShowFusionMenu()
    {
        m_FusionMenu.SetActive(true);
    }

    public void HideFusionMenu()
    {
        m_FusionMenu.SetActive(false);
    }

    public void PlayButtonPressed()
    {
        Destroy(m_MainMenu);
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Click", 3);
        LevelManager levelManager = ServiceLocator.Instance.GetService(ManagerType.LEVEL_MANAGER) as LevelManager;
        levelManager.LoadNextLevel();
    }

    public void ResumeButtonPressed()
    {
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Click", 3);
        HidePauseMenu();
    }

    public void MainMenuButtonPressed()
    {
        Destroy(m_PauseMenu);
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Click", 3);
        LevelManager levelManager = ServiceLocator.Instance.GetService(ManagerType.LEVEL_MANAGER) as LevelManager;
        levelManager.LoadMainMenu();
    }

    private void OptionsButtonPressed()
    {
    }

    public void QuitButtonPressed()
    {
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Click", 3);
        Application.Quit();
    }

    public void RestartButtonPressed()
    {
        Destroy(m_PauseMenu);
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Click", 3);
        ServiceLocator.Instance.GetService<LevelManager>(ManagerType.LEVEL_MANAGER).ReloadLevel();
    }

    public void RequestCredits()
    {
        m_CreditsRequested = true;
    }

    private void OnDestroy()
    {
        m_PlayerManager.OnFusionInit -= ShowFusionMenu;
        m_PlayerManager.OnDefuseComplete -= HideFusionMenu;
    }
}
