using UnityEngine.SceneManagement;

public class LevelManager : Service
{
    int m_levelIndex = 0;

    private void Awake()
    {
        m_Type = ManagerType.LEVEL_MANAGER;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += LevelWasLoaded;
    }

    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void LoadMainMenu()
    {
        m_levelIndex = 0;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadNextLevel()
    {
        ++m_levelIndex;
        if (m_levelIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(m_levelIndex);
        else
            LoadMainMenu();
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(m_levelIndex);
    }

    private void LevelWasLoaded(Scene scene, LoadSceneMode mode)
    {
        GUIManager gui = ServiceLocator.Instance.GetService(ManagerType.GUI_MANAGER) as GUIManager;

        if (scene.name == "MainMenu")
        {
            gui.InstantiateMainMenu();
        }
        else if (!scene.name.Contains("Narration"))
        {
            gui.InstantiateLevelMenu();
        }

        SoundManager soundMgr = ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER);
        if (scene.buildIndex == 0)
        {
            soundMgr.PlaySong("event:/MUSIC/Music_MenuHome_loop", 0);
        }
        if (scene.buildIndex == 2)
        {
            soundMgr.PlaySong("event:/MUSIC/Music_LVL1", 0);
            soundMgr.SwitchMusicParameter("DT_ExploBase", 1.0f);
        }
        else if (scene.buildIndex == 4)
        {
            soundMgr.PlaySong("event:/MUSIC/Music_LVL2", 0);
            soundMgr.SwitchMusicParameter("DT_ExploBase", 1.0f);
        }
        else if (scene.buildIndex == 6)
        {
            //soundMgr.PlaySong("event:/MUSIC/Music_LVL3", 0);
            //soundMgr.SwitchMusicParameter("DT_ExploBase", 1.0f);
        }
    }
}
