using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IDamageable
{
    bool m_IsOpen = false;
    int m_Life = 5;

    public void TakeDamage(AttackStats attackStats)
    {
        if (!m_IsOpen)
        {
            if (m_Life-- == 0)
            {
                m_IsOpen = true;
                Destroy(gameObject); // temp
            }
        }
    }
}
