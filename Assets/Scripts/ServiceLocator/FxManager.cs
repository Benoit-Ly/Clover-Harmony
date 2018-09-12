using System;
using System.Collections;
using System.Collections.Generic;
using UnityChan;
using UnityEngine;
using XInputDotNetPure;

public class FxManager : Service
{
    private void Awake()
    {
        m_Type = ManagerType.FX_MANAGER;
    }

    public void PlayFX(ParticleSystem FX)
    {
        FX.Play(true);
    }
}
