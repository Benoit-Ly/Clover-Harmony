using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySlotController : MonoBehaviour
{
    [SerializeField] GameObject Foreground;
    Animator m_Animator;
    bool m_IsFill = true;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public bool IsActive()
    {
        return m_IsFill;
    }

    public void UseSlot()
    {
        m_IsFill = false;
        m_Animator.SetTrigger("Empty");
    }

    public void RechargeSlot()
    {
        m_IsFill = true;
        m_Animator.SetTrigger("Fill");
    }
	
}
