using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigRoots : MonoBehaviour, IDamageable
{
    #region Services
    GameManager m_GameManager;
    #endregion

    bool m_IsAlive = true;

    [SerializeField] int Life = 3;

    VisualFeedbackBrique visualFeedback;

    void Start()
    {
        m_GameManager = ServiceLocator.Instance.GetService<GameManager>(ManagerType.GAME_MANAGER);
        visualFeedback = GetComponent<VisualFeedbackBrique>();
    }

    public void TakeDamage(AttackStats attackStats)
    {
        if (!m_IsAlive)
            return;

        Life -= 1;
        m_GameManager.ShakeCamera();
        StartCoroutine(visualFeedback.ApplyChange());

        if (Life == 0)
        {
            Death();
        }
    }

    private void Death()
    {
        m_IsAlive = false;
        Destroy(gameObject);
    }
}
