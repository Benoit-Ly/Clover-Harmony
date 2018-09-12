using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : Service {

    #region Services
    PlayerManager m_PlayerManager;
    #endregion

    #region Vars
    PlayerController[] m_PlayerControllers;
    List<Controller> m_Controllers;

    int m_NbPlayers = 2;
    #endregion

    private void Awake()
    {
        m_Type = ManagerType.CONTROLLER_MANAGER;

        m_Controllers = new List<Controller>();
        m_PlayerControllers = new PlayerController[m_NbPlayers];

        m_PlayerManager = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER);
        m_PlayerManager.OnFusionComplete += OnFusionComplete;
        m_PlayerManager.OnDefuseComplete += OnDefuseComplete;
    }

    public void RegisterController(Controller controller)
    {
        m_Controllers.Add(controller);
    }

    public void RegisterPlayerController(PlayerController controller, int playerID)
    {
        m_PlayerControllers[playerID - 1] = controller;
    }

    void OnFusionComplete()
    {
        EnablePlayerControllers(false);
    }

    void OnDefuseComplete()
    {
        EnablePlayerControllers(true);
    }

    public void EnableControllers(bool enable)
    {
        int nbControllers = m_Controllers.Count;
        for (int i = 0; i < nbControllers; i++)
        {
            if (m_Controllers[i] != null)
                m_Controllers[i].enabled = enable;
        }
    }

    void EnablePlayerControllers(bool enable)
    {
        for (int i = 0; i < m_NbPlayers; i++)
        {
            if (m_PlayerControllers[i] != null)
                m_PlayerControllers[i].enabled = enable;
        }
    }

    public void ReportDestroyed(Controller controller)
    {
        m_Controllers.Remove(controller);
    }

    private void OnDestroy()
    {
        m_PlayerManager.OnFusionComplete -= OnFusionComplete;
        m_PlayerManager.OnDefuseComplete -= OnDefuseComplete;
    }
}
