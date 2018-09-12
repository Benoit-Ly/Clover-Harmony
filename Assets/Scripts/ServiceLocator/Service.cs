using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ManagerType
{
    NONE = 0,
    GAME_MANAGER,
    LEVEL_MANAGER,
    PLAYER_MANAGER,
    ENEMY_MANAGER,
    LANDSCAPE_MANAGER,
    GUI_MANAGER,
    HUD_MANAGER,
    SOUND_MANAGER,
    FX_MANAGER,
    BATTLE_MANAGER,
    ITEM_MANAGER,
    AI_TACTICIAN,
    CONTROLLER_MANAGER
}

public class Service : ScriptableObject
{
    public ManagerType m_Type;
}
