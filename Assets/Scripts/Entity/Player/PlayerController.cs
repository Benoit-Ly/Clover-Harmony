using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    IDLE = 0,
    READY_FOR_FUSION,
    IS_FUSING,
    DEAD
}

public class PlayerController : Controller {

    #region Services
    PlayerManager m_PlayerManager;
    GUIManager m_GUIManager;
    SoundManager m_SoundManager;
    #endregion

    #region Enums
    enum EnumPlayerNum
    {
        NONE = 0,
        PLAYER_1,
        PLAYER_2
    }
    #endregion

    #region Params
    Player m_Player;

    [SerializeField]
    EnumPlayerNum m_EnumPlayerNumber;

    PlayerState m_CurrentState;             public PlayerState State { get { return m_CurrentState; } set { m_CurrentState = value; } }

    PlayerState[] m_CommandDisablingStates = { PlayerState.IS_FUSING, PlayerState.DEAD };

    string m_InputPrefix;

    int m_PlayerNumber = 0;                 public int PlayerNumber { get { return m_PlayerNumber; } set { m_PlayerNumber = value; } }

    bool m_FusionHold = false;

    [SerializeField]
    ParticleSystem m_readyForFusion;
    ParticleSystem readyForFusion;

    #endregion
    UnityEngine.Vector3 posSong;
    // Use this for initialization
    protected override void Start () {
        base.Start();

        m_PlayerManager = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER);
        m_GUIManager = ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER);
        m_SoundManager = ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER);

        m_Player = GetComponent<Player>();

        m_PlayerNumber = (int)m_EnumPlayerNumber;
        m_InputPrefix = "P" + m_PlayerNumber + "_";

        m_PlayerManager.RegisterPlayer(m_Player, m_PlayerNumber);
        m_ControllerManager.RegisterPlayerController(this, m_PlayerNumber);

        m_PlayerManager.OnFusionInit += OnFusionInit;
        m_PlayerManager.OnFusionCancel += OnFusionCancel;
        m_PlayerManager.OnDefuseComplete += OnDefuse;
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();

        if (m_Player.HP == 0 && m_CurrentState != PlayerState.DEAD)
            m_CurrentState = PlayerState.DEAD;
        else if (m_Player.HP > 0 && m_CurrentState == PlayerState.DEAD)
            m_CurrentState = PlayerState.IDLE;

        if (Input.GetAxis(m_InputPrefix + "Fusion") == -1f && !m_FusionHold)
        {
            m_CurrentState = PlayerState.READY_FOR_FUSION;
            m_Player.ReportReadyForFusion(m_PlayerNumber);
            //m_PlayerManager.ReportReadyForFusion(m_PlayerNumber);
            m_FusionHold = true;
        }
        else if (Input.GetAxis(m_InputPrefix + "Fusion") > -1f && m_CurrentState == PlayerState.READY_FOR_FUSION)
        {
            m_Player.CancelReadyForFusion(m_PlayerNumber);
            //m_PlayerManager.CancelReadyForFusion(m_PlayerNumber);
            m_CurrentState = PlayerState.IDLE;
        }

        if (Input.GetAxis(m_InputPrefix + "Fusion") > -1f)
        {
            m_FusionHold = false;
        }

        if (IsDisabled())
            return;

        // Update Movement
        float x = Input.GetAxis(m_InputPrefix + "Horizontal");
        float y = -Input.GetAxis(m_InputPrefix + "Vertical");
        bool isTilted = (x <= -0.1f || x >= 0.1f || y <= -0.1f || y >= 0.1f);

        if (isTilted)
            m_Player.Move(x, y);
        else
            m_Player.Move(0f, 0f);

        if (Input.GetButtonDown(m_InputPrefix + "Jump"))
            m_Player.Jump();

        if (Input.GetButtonDown("Start"))
            m_GUIManager.ShowPauseMenu();

        if (Input.GetButtonDown(m_InputPrefix + "Fire1"))
        {
            m_Player.Attack();
        }

        if (Input.GetButtonDown(m_InputPrefix + "Fire2"))
        {
            m_Player.HeavyAttack();
        }
    }

    void OnFusionInit()
    {
        m_CurrentState = PlayerState.IS_FUSING;
    }

    void OnDefuse()
    {
        m_CurrentState = PlayerState.IDLE;
    }

    void OnFusionCancel()
    {
        if (Input.GetAxis(m_InputPrefix + "Fusion") == -1f)
            m_PlayerManager.ReportReadyForFusion(m_PlayerNumber);
    }

    bool IsDisabled()
    {
        return (Array.IndexOf(m_CommandDisablingStates, m_CurrentState) != -1);
    }

    private void OnDisable()
    {
        m_Player.Move(0f, 0f);
    }

    private void OnDestroy()
    {
        m_PlayerManager.OnFusionInit -= OnFusionInit;
        m_PlayerManager.OnFusionCancel -= OnFusionCancel;
        m_PlayerManager.OnDefuseComplete -= OnDefuse;
    }
    public void PlaySong_Light1()
    {
        if (PlayerNumber == 1)
            m_SoundManager.PlaySong("event:/SFX/Attack/Attack_C1", PlayerNumber, transform.position);
        else
            m_SoundManager.PlaySong("event:/SFX/Attack/Attack_A1", PlayerNumber, transform.position);
    }

    public void PlaySong_Light2()
    {
        if (PlayerNumber == 1)
            m_SoundManager.PlaySong("event:/SFX/Attack/Attack_C2", PlayerNumber, transform.position);
        else
            m_SoundManager.PlaySong("event:/SFX/Attack/Attack_A2_2", PlayerNumber, transform.position);
    }

    public void PlaySong_Light3()
    {
        if (PlayerNumber == 1)
            m_SoundManager.PlaySong("event:/SFX/Attack/Attack_C3", PlayerNumber, transform.position);
        else
            m_SoundManager.PlaySong("event:/SFX/Attack/Attack_A2_3", PlayerNumber, transform.position);
    }

    public void PlaySong_AirAttack()
    {
        if (PlayerNumber == 1)
            m_SoundManager.PlaySong("event:/SFX/Attack/Attack_C4", PlayerNumber, transform.position);
        else
            m_SoundManager.PlaySong("event:/SFX/Attack/Attack_A3", PlayerNumber, transform.position);
    }

    public void PlaySong_Heavy()
    {
        if (PlayerNumber == 1)
            m_SoundManager.PlaySong("event:/SFX/Attack/Attack_C5", PlayerNumber, transform.position);
        else
            m_SoundManager.PlaySong("event:/SFX/Attack/Attack_A4", PlayerNumber, transform.position);
    }

    public void PlaySong_Death()
    {
        if (PlayerNumber == 1)
            m_SoundManager.PlaySong("event:/SFX/HitDeath_And_Fusion/Death_C", PlayerNumber, transform.position);
        else
            m_SoundManager.PlaySong("event:/SFX/HitDeath_And_Fusion/Death_A", PlayerNumber, transform.position);
    }

    public void PlaySong_TakeDamage()
    {
        if (PlayerNumber == 1)
            m_SoundManager.PlaySong("event:/SFX/HitDeath_And_Fusion/Take_Attack_C", PlayerNumber, transform.position);
        else
            m_SoundManager.PlaySong("event:/SFX/HitDeath_And_Fusion/Take_Attack_A", PlayerNumber, transform.position);
    }

    public void PlaySong_FootSteps()
    {
        if (PlayerNumber == 1)
            m_SoundManager.PlaySong("event:/SFX/HitDeath_And_Fusion/Footsteps_C", PlayerNumber, transform.position);
        else
            m_SoundManager.PlaySong("event:/SFX/HitDeath_And_Fusion/Footsteps_A", PlayerNumber, transform.position);
    }
}
