using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Song : TriggerAbstract
{
    [SerializeField] bool DesactiveOnPlay = true;
    [SerializeField] bool IsAmbiance = false;
    [SerializeField] bool CutMusic = false;
    [EventRef]
    public String Event = "";

    private void Start()
    {
        if (CutMusic)
            ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).StopCurrentMusic();

        if (IsAmbiance)
        {
            DesactivateTrigger();

            if (string.IsNullOrEmpty(Event))
                Debug.LogWarning("Event is not set in a TriggerSong");
            else
                ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong(Event, -1);
        }
    }

    protected override void TriggerEnter()
    {
        if (DesactiveOnPlay)
            DesactivateTrigger();

        if (string.IsNullOrEmpty(Event))
            Debug.LogWarning("Event is not set in a TriggerSong");
        else
            ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong(Event, 3, transform.position);
    }
}
