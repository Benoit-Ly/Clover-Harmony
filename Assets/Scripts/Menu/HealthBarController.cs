using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    Player m_Player;

    Slider m_Slider;
    Animator m_Animator;
    int m_MaxHP;

    public void Init(int playerNumber)
    {
        m_Player = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER).GetPlayer(playerNumber);
        m_Animator = GetComponent<Animator>();
        m_Slider = GetComponent<Slider>();

        m_Player.OnHPChange += Player_OnHPChange;
        m_Player.OnTakeDamage += Player_TakeDamage;
        m_MaxHP = m_Player.MaxHP;
    }

    private void Player_TakeDamage()
    {
        m_Animator.SetTrigger("TakeDamage");
    }

    private void Player_OnHPChange(int currentHp)
    {
        m_Slider.value = (float)currentHp / m_MaxHP;
    }

    private void OnDestroy()
    {
        m_Player.OnHPChange -= Player_OnHPChange;
        m_Player.OnTakeDamage -= Player_TakeDamage;
    }
}
