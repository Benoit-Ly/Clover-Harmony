using UnityEngine;

public class ArenaObject : TriggerAbstract
{
    [SerializeField] ArenaData ArenaData;
    [SerializeField] Vector3 CameraPosition;
    [SerializeField] float FieldOfView;

    int m_CurrentWaveIndex;
    bool m_IsLaunch = false;
    EnemyManager m_EnemyManager;
    GameManager m_GameManager;
    PlayerManager m_PlayerManager;

    private void Start()
    {
        m_CurrentWaveIndex = -1;
        m_EnemyManager = ServiceLocator.Instance.GetService<EnemyManager>(ManagerType.ENEMY_MANAGER);
        m_GameManager = ServiceLocator.Instance.GetService<GameManager>(ManagerType.GAME_MANAGER);
        m_PlayerManager = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER);
    }

    protected override void TriggerEnter()
    {
        if (ArenaData)
        {
            if (m_PlayerManager.HarmonyRate >= 0.7f)
                ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).SwitchMusicParameter("DT_FightBase", 1.0f);
            else
                ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).SwitchMusicParameter("DT_FightSad", 1.0f);

            m_GameManager.SwitchToArenaMode(CameraPosition, FieldOfView);
            m_IsLaunch = true;
            m_GameManager.RegisterCurrentArena(this);
            DesactivateTrigger();
        }
        else
            Debug.LogWarning(string.Format("ArenaData of {0} is not set", name));
    }

    public void Cancel()
    {
        if (m_PlayerManager.HarmonyRate >= 0.7f)
            ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).SwitchMusicParameter("DT_ExploBase", 1.0f);
        else
            ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).SwitchMusicParameter("DT_ExploSad", 1.0f);

        m_IsLaunch = false;
        m_CurrentWaveIndex = -1;
        //Camera.main.GetComponent<MainCamera>().EndArena();
        m_GameManager.mainCamera.EndArena();
    }

    private void Update()
    {
        if (ArenaData)
        {
            if (m_IsLaunch)
            {
                if(m_EnemyManager.m_CurrentEnemyCount == 0)
                {
                    SpawnNextWave();
                }
            }
        }
    }

    private void SpawnNextWave()
    {
        ++m_CurrentWaveIndex;

        if (m_CurrentWaveIndex < ArenaData.Waves.Count)
        {
            ArenaData.SpawnWave(m_CurrentWaveIndex, transform.position, this);
        }
        else
        {
            if (m_PlayerManager.HarmonyRate >= 0.7f)
                ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).SwitchMusicParameter("DT_ExploBase", 1.0f);
            else
                ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).SwitchMusicParameter("DT_ExploSad", 1.0f);

            m_IsLaunch = false;
            m_CurrentWaveIndex = -1;
            m_GameManager.mainCamera.EndArena();
            m_GameManager.EndCurrentArena();
        }
    }
}
