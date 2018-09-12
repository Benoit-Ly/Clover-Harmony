using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thorns : MonoBehaviour
{
    AttackStats m_AttackStats;
    [SerializeField] int Damages = 5;
    [SerializeField] float Rate = 1f;

    IEnumerator m_CloverCoroutine;
    IEnumerator m_AmaranthCoroutine;

    private void Start()
    {
        m_AttackStats = new AttackStats
        {
            damage = Damages,
            name = "Thorns"
        };
    }

    IEnumerator DealDamages(Player player, int id)
    {
        player.TakeDamage(m_AttackStats);

        yield return new WaitForSeconds(Rate);

        if (id == 1)
        {
            m_CloverCoroutine = DealDamages(player, 1);
            StartCoroutine(m_CloverCoroutine);
        }
        else
        {
            m_AmaranthCoroutine = DealDamages(player, 2);
            StartCoroutine(m_AmaranthCoroutine);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();

        if (player)
        {
            if (player.ID == 1)
            {
                m_CloverCoroutine = DealDamages(player, 1);
                StartCoroutine(m_CloverCoroutine);
            }
            else
            {
                m_AmaranthCoroutine = DealDamages(player, 2);
                StartCoroutine(m_AmaranthCoroutine);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();

        if (player)
        {
            if (player.ID == 1)
                StopCoroutine(m_CloverCoroutine);
            else
                StopCoroutine(m_AmaranthCoroutine);
        }
    }
}
