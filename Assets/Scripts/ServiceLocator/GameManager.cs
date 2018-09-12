using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Service
{
    PlayerManager m_PlayerManager;
    EnemyManager m_EnemyManager;
    LandscapeManager m_LandscapeManager;
    GUIManager m_GUIManager;

    List<ShopTreeSave> m_CloverTreeSave = new List<ShopTreeSave>();
    List<ShopTreeSave> m_AmaranthTreeSave = new List<ShopTreeSave>();

    public List<ShopTreeSave> CloverTreeSave { get { return m_CloverTreeSave; } }
    public List<ShopTreeSave> AmaranthTreeSave { get { return m_AmaranthTreeSave; } }

    Checkpoint m_CurrentCheckpoint;
    ArenaObject m_CurrentArena;
    MainCamera m_MainCamera;        public MainCamera mainCamera { get { return m_MainCamera; } set { m_MainCamera = value; } }

    private void Awake()
    {
        m_Type = ManagerType.GAME_MANAGER;
        m_PlayerManager = (PlayerManager)ServiceLocator.Instance.GetService(ManagerType.PLAYER_MANAGER);
        m_EnemyManager = (EnemyManager)ServiceLocator.Instance.GetService(ManagerType.ENEMY_MANAGER);
        m_LandscapeManager = (LandscapeManager)ServiceLocator.Instance.GetService(ManagerType.LANDSCAPE_MANAGER);
        m_GUIManager = ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER);

        m_PlayerManager.OnAllDead += Loose;
    }

    public void SaveShopTree(bool isClover, List<CustomImage> images)
    {
        if (isClover)
        {
            m_CloverTreeSave.Clear();

            foreach (CustomImage image in images)
            {
                ShopTreeSave newOne = new ShopTreeSave()
                {
                    Save_isUnlock = image.isUnlock,
                    Save_isUnlockable = image.isUnlockable,
                    Save_selectID = image.selectID
                };

                m_CloverTreeSave.Add(newOne);
            }
        }
        else
        {
            m_AmaranthTreeSave.Clear();

            foreach (CustomImage image in images)
            {
                ShopTreeSave newOne = new ShopTreeSave()
                {
                    Save_isUnlock = image.isUnlock,
                    Save_isUnlockable = image.isUnlockable,
                    Save_selectID = image.selectID
                };

                m_AmaranthTreeSave.Add(newOne);
            }
        }
    }

    public void ResetShop()
    {
        m_CloverTreeSave.Clear();
        m_AmaranthTreeSave.Clear();
    }

    public void Win()
    {
        PlayerManager playerManager = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER);
        playerManager.SaveStats();

        LevelManager levelManager = (LevelManager)ServiceLocator.Instance.GetService(ManagerType.LEVEL_MANAGER);
        levelManager.LoadNextLevel();
    }

    public void Loose()
    {
        ServiceLocator.Instance.StartServiceCoroutine(Respawn());
    }

    public void EndCurrentArena()
    {
        m_CurrentArena = null;
    }

    public void RegisterCurrentArena(ArenaObject arena)
    {
        m_CurrentArena = arena;
    }

    public void RegisterCurrentCheckpoint(Checkpoint checkpoint)
    {
        m_CurrentCheckpoint = checkpoint;
    }

    IEnumerator Respawn()
    {
        if (!m_MainCamera)
            yield break;

        m_MainCamera.FadeIn();
        //m_GUIManager.HideHUD();

        yield return new WaitForSeconds(m_MainCamera.fadeDuration);

        if (m_CurrentArena != null)
        {
            m_CurrentArena.Cancel();
            m_CurrentArena = null;
        }

        m_PlayerManager.RespawnPlayer(m_CurrentCheckpoint);

        yield return new WaitForSeconds(1f);

        m_EnemyManager.RemoveAllEnemies();

        yield return new WaitForSeconds(1f);

        m_MainCamera.SetPos(m_CurrentCheckpoint.playerOne.position.x);

        yield return new WaitForSeconds(m_MainCamera.fadeBlackScreenTime);

        m_MainCamera.FadeOut();
        //m_GUIManager.ShowHUD();
    }

    public void SwitchToArenaMode(Vector3 posCamera, float fieldOfView)
    {
        //Camera.main.GetComponent<MainCamera>().LaunchArena(posCamera, fieldOfView);
        m_MainCamera.LaunchArena(posCamera, fieldOfView);
    }

    public void PlaySong(string pathToGoodMusic, string pathToBadMusic, float goodHarmony)
    {
        if (m_PlayerManager.HarmonyRate >= goodHarmony)
            ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong(pathToGoodMusic, 0, new Vector3(0.0f, 0.0f));

        else if (m_PlayerManager.HarmonyRate < goodHarmony)
            ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong(pathToBadMusic, 0, new Vector3(0.0f, 0.0f));
    }

    public void SwitchToExploringMode()
    {
        //Camera.main.GetComponent<MainCamera>().EndArena();
        m_MainCamera.EndArena();
    }

    public void DealDamage(GameObject player, int damage)
    {
        player.GetComponent<Player>().TakeDamage(damage);
    }

    public void ShakeCamera()
    {
        if (m_MainCamera)
        {
            m_MainCamera.Shake();
        }
    }

    public void OnDestroy()
    {
        m_PlayerManager.OnAllDead -= Loose;
    }
}
