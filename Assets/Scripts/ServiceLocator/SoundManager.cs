using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SoundManager : Service
{
    StudioEventEmitter m_PlayerOneStudioEventEmitter;
    StudioEventEmitter m_PlayerTwoStudioEventEmitter;
    StudioEventEmitter m_TriggerStudioEventEmitter;

    EventInstance m_MusicInstance;
    EventInstance m_AmbianceInstance;

    string m_CurrentNameParameter;
    float m_CurrentValueParameter;

    private void Awake()
    {
        m_Type = ManagerType.SOUND_MANAGER;

        StudioEventEmitter[] studioEventEmitter = ServiceLocator.Instance.GetComponentsInChildren<StudioEventEmitter>();
        RegisterStudioEventEmitter(studioEventEmitter);
    }

    private void RegisterStudioEventEmitter(StudioEventEmitter[] studioEventEmitter)
    {
        m_PlayerOneStudioEventEmitter = studioEventEmitter[0];
        m_PlayerOneStudioEventEmitter.OverrideAttenuation = true;
        m_PlayerOneStudioEventEmitter.OverrideMaxDistance = 100.0f;

        m_PlayerTwoStudioEventEmitter = studioEventEmitter[1];
        m_PlayerTwoStudioEventEmitter.OverrideAttenuation = true;
        m_PlayerTwoStudioEventEmitter.OverrideMaxDistance = 100.0f;

        m_TriggerStudioEventEmitter = studioEventEmitter[2];
        m_TriggerStudioEventEmitter.OverrideAttenuation = true;
        m_TriggerStudioEventEmitter.OverrideMaxDistance = 100.0f;
    }

    public void StopCurrentMusic()
    {
        if (m_MusicInstance.isValid())
        {
            m_MusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
            m_MusicInstance.release();
        }
    }

    public void SwitchMusicParameter(string parameterName, float value)
    {
        if (string.IsNullOrEmpty(m_CurrentNameParameter))
        {
            m_MusicInstance.setParameterValue(parameterName, value);
            m_CurrentNameParameter = parameterName;
            m_CurrentValueParameter = value;
        }
        else if(m_CurrentNameParameter.Equals(parameterName))
        { 
            if (m_CurrentValueParameter != value)
            {
                m_MusicInstance.setParameterValue(parameterName, value);
                m_CurrentValueParameter = value;
            }
        }
        else
        {
            ServiceLocator.Instance.StartServiceCoroutine(MusicTransition(parameterName, value));
        }
    }

    IEnumerator MusicTransition(string parameterName, float value)
    {
        m_MusicInstance.setParameterValue(parameterName, value);
        yield return new WaitForSeconds(2f);
        m_MusicInstance.setParameterValue(m_CurrentNameParameter, 0.0f);
        m_CurrentNameParameter = parameterName;
        m_CurrentValueParameter = value;
    }

    public void PlaySong(string pathEvent, int playerIdx)
    {
        Vector3 position = Camera.main.transform.position;
        PlaySong(pathEvent, playerIdx, position);
    }

    public void PlaySong(string pathEvent, int playerIdx, Vector3 position)
    {
        if (playerIdx == -1)
        {
            if (m_AmbianceInstance.isValid())
            {
                m_AmbianceInstance.stop(STOP_MODE.ALLOWFADEOUT);
                m_AmbianceInstance.release();
            }

            m_AmbianceInstance = RuntimeManager.CreateInstance(pathEvent);
            m_AmbianceInstance.start();
        }
        else if (playerIdx == 0)
        {
            if (m_MusicInstance.isValid())
            {
                m_MusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
                m_MusicInstance.release();
            }

            m_MusicInstance = RuntimeManager.CreateInstance(pathEvent);
            m_MusicInstance.start();
        }
        else
        {
            GameObject temp = new GameObject();
            temp.transform.position = position;
            StudioEventEmitter emitter = temp.AddComponent<StudioEventEmitter>();
            emitter.Event = pathEvent;
            emitter.OverrideAttenuation = true;
            emitter.OverrideMaxDistance = 100.0f;
            emitter.PlayEvent = EmitterGameEvent.ObjectDestroy;
            GameObject.Destroy(temp);
        }
    }
}
