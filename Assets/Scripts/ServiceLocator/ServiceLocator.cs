using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    static ServiceLocator p_Instance = null;
    static public ServiceLocator Instance { get { return p_Instance; } }
    List<Service> m_Services;

    [SerializeField]
    GameSettings m_GameSettings;            public GameSettings Settings { get { return m_GameSettings; } }

    private void Awake()
    {
        if (p_Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        p_Instance = this;

        DontDestroyOnLoad(gameObject);

        Cursor.visible = false;

        m_Services = new List<Service>();

        CreateService(ManagerType.GUI_MANAGER);
        CreateService(ManagerType.LEVEL_MANAGER);
        CreateService(ManagerType.SOUND_MANAGER);

        Debug.Log("Service Locator");
    }

    public Service GetService(ManagerType type)
    {
        foreach (Service service in m_Services)
        {
            if (service.m_Type == type)
            {
                return service;
            }
        }

        if (type == ManagerType.NONE)
        {
            Debug.LogWarning("Trying to create a manager that hasn't a type!");
            return null;
        }

        return CreateService(type);
    }

    public T GetService<T>(ManagerType type) where T : Service
    {
        return (T)GetService(type);
    }

    private Service CreateService(ManagerType type)
    {
        Service newService = null;

        switch (type)
        {
            case ManagerType.GAME_MANAGER:
                newService = ScriptableObject.CreateInstance<GameManager>();
                break;

            case ManagerType.LEVEL_MANAGER:
                newService = ScriptableObject.CreateInstance<LevelManager>();
                break;

            case ManagerType.PLAYER_MANAGER:
                newService = ScriptableObject.CreateInstance<PlayerManager>();
                break;

            case ManagerType.ENEMY_MANAGER:
                newService = ScriptableObject.CreateInstance<EnemyManager>();
                break;

            case ManagerType.LANDSCAPE_MANAGER:
                newService = ScriptableObject.CreateInstance<LandscapeManager>();
                break;

            case ManagerType.GUI_MANAGER:
                newService = ScriptableObject.CreateInstance<GUIManager>();
                break;

            case ManagerType.SOUND_MANAGER:
                newService = ScriptableObject.CreateInstance<SoundManager>();
                break;

            case ManagerType.BATTLE_MANAGER:
                newService = ScriptableObject.CreateInstance<BattleManager>();
                break;

            case ManagerType.ITEM_MANAGER:
                newService = ScriptableObject.CreateInstance<ItemManager>();
                break;

            case ManagerType.AI_TACTICIAN:
                newService = ScriptableObject.CreateInstance<AITactician>();
                break;

            case ManagerType.CONTROLLER_MANAGER:
                newService = ScriptableObject.CreateInstance<ControllerManager>();
                break;

            case ManagerType.FX_MANAGER:
                newService = ScriptableObject.CreateInstance<FxManager>();
                break;

            default:
                Debug.LogError("Unknown Service Locator type : " + type);
                return null;
        }
        m_Services.Add(newService);

        return newService;
    }

    public Coroutine StartServiceCoroutine(IEnumerator coroutine)
    {
        if (coroutine == null)
            return null;

        return StartCoroutine(coroutine);
    }

    public void StopServiceCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }

    public void DestroyService(ManagerType type)
    {
        foreach (Service service in m_Services)
        {
            if (service.m_Type == type)
            {
                Destroy(service);
                return;
            }
        }
    }
}
