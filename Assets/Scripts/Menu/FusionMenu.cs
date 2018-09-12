using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FusionMenu : MonoBehaviour {

    [SerializeField] MenuImage m_InvincibleImage;
    [SerializeField] MenuImage m_ConfusionImage;
    [SerializeField] MenuImage m_HealImage;
    [SerializeField] MenuImage m_EnergyImage;

    public event Action InvincibleSpellSelected;
    public event Action ConfusionSpellSelected;
    public event Action HealSpellSelected;
    public event Action EnergySpellSelected;
    public event Action OnCancel;

    MenuImage m_CurrentButtonPlayerOne;
    MenuImage m_CurrentButtonPlayerTwo;

    bool m_P1FusionHold = true;
    bool m_P2FusionHold = true;

    void OnEnable ()
    {
        m_CurrentButtonPlayerOne = null;
        m_CurrentButtonPlayerTwo = null;

        m_P1FusionHold = true;
        m_P2FusionHold = true;

        m_InvincibleImage.playerOne.SetActive(false);
        m_InvincibleImage.playerTwo.SetActive(false);

        m_ConfusionImage.playerOne.SetActive(false);
        m_ConfusionImage.playerTwo.SetActive(false);

        m_HealImage.playerOne.SetActive(false);
        m_HealImage.playerTwo.SetActive(false);

        m_EnergyImage.playerOne.SetActive(false);
        m_EnergyImage.playerTwo.SetActive(false);
    }

    void Update()
    {
        float P1FusionTrigger = Input.GetAxis("P1_Fusion");
        float P2FusionTrigger = Input.GetAxis("P2_Fusion");

        if (P1FusionTrigger > -1f)
            m_P1FusionHold = false;
        else if (!m_P1FusionHold && OnCancel != null)
        {
            m_P1FusionHold = true;
            OnCancel();
        }
            

        if (P2FusionTrigger > -1f)
            m_P2FusionHold = false;
        else if (!m_P2FusionHold && OnCancel != null)
        {
            m_P2FusionHold = true;
            OnCancel();
        }

        if (Input.GetButtonDown("P1_Fire3"))
        {
            PlayerOneSelect(m_InvincibleImage);
        }
        else if (Input.GetButtonDown("P1_Fire2"))
        {
            PlayerOneSelect(m_ConfusionImage);
        }
        else if (Input.GetButtonDown("P1_Fire1"))
        {
            PlayerOneSelect(m_EnergyImage);
        }
        else if (Input.GetButtonDown("P1_Jump"))
        {
            PlayerOneSelect(m_HealImage);
        }

        if (Input.GetButtonDown("P2_Fire3"))
        {
            PlayerTwoSelect(m_InvincibleImage);
        }
        else if (Input.GetButtonDown("P2_Fire2"))
        {
            PlayerTwoSelect(m_ConfusionImage);
        }
        else if (Input.GetButtonDown("P2_Fire1"))
        {
            PlayerTwoSelect(m_EnergyImage);
        }
        else if (Input.GetButtonDown("P2_Jump"))
        {
            PlayerTwoSelect(m_HealImage);
        }
    }

    void PlayerOneSelect(MenuImage spell)
    {
            if (m_CurrentButtonPlayerTwo == spell)
                PlayerIsAgree(spell);
            else if (m_CurrentButtonPlayerOne)
                m_CurrentButtonPlayerOne.playerOne.SetActive(false);

            m_CurrentButtonPlayerOne = spell;
            m_CurrentButtonPlayerOne.playerOne.SetActive(true);
    }

    void PlayerTwoSelect(MenuImage spell)
    {
        if (m_CurrentButtonPlayerOne == spell)
            PlayerIsAgree(spell);
        else if (m_CurrentButtonPlayerTwo)
            m_CurrentButtonPlayerTwo.playerTwo.SetActive(false);

        m_CurrentButtonPlayerTwo = spell;
        m_CurrentButtonPlayerTwo.playerTwo.SetActive(true);
    }

    void PlayerIsAgree(MenuImage spell)
    {
        if (spell == m_InvincibleImage)
        {
            if (InvincibleSpellSelected != null)
                InvincibleSpellSelected();
        }
        else if (spell == m_ConfusionImage)
        {
            if (ConfusionSpellSelected != null)
                ConfusionSpellSelected();
        }
        else if (spell == m_HealImage)
        {
            if (HealSpellSelected != null)
                HealSpellSelected();
        }
        else if (spell == m_EnergyImage)
        {
            if (EnergySpellSelected != null)
                EnergySpellSelected();
        }

        ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER).ReportFusionActionComplete();
    }
}
