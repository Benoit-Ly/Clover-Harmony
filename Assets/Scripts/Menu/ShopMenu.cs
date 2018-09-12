using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    [SerializeField]
    Text VitaeField;
    int Vitae = 10000;

    public event Action<int, int, int> SkillBought;
    public event Action ShopClosed;

    int InitVitae;


    private void Start()
    {
        Vitae = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER).Vitae;
        InitVitae = Vitae;
        VitaeField.text = Vitae.ToString();
    }

    private void OnEnable()
    {
        Vitae = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER).Vitae;
        InitVitae = Vitae;
        VitaeField.text = Vitae.ToString();
    }

    public void LeaveTheShop()
    {
        if (ShopClosed != null)
            ShopClosed();

        ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER).SpendVitae(InitVitae - Vitae);
        ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).HideShopMenu();

        if (ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER).HarmonyRate >= 0.7f)
            ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).SwitchMusicParameter("DT_ExploBase", 1.0f);
        else
            ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).SwitchMusicParameter("DT_ExploSad", 1.0f);
    }

    public bool TryToBuySkills(CustomImage skill, int playerNumber)
    {
        if (Vitae >= skill.price)
        {
            Vitae -= skill.price;
            VitaeField.text = Vitae.ToString();

            if (SkillBought != null)
                SkillBought(skill.selectID, skill.price, playerNumber);

            ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Buy", 3);

            return true;
        }

        return false;
    }
}
