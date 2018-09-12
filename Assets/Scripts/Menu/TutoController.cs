using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoController : TriggerAbstract
{
    [SerializeField] GameObject Tuto;
    bool m_IsActive = false;

    private void Start()
    {
        Tuto.SetActive(false);
    }

    protected override void TriggerEnter()
    {
        m_IsActive = true;
        Tuto.SetActive(true);
        DesactivateTrigger();
    }

    private void Update()
    {
        if (m_IsActive)
        {
            if (Input.GetButtonDown("P1_Fire3") || Input.GetButtonDown("P2_Fire3"))
            {
                Tuto.SetActive(false);
                m_IsActive = false;
            }
        }
    }
}
