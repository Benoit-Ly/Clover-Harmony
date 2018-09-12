using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemy : MonoBehaviour {

    [SerializeField]
    int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("collision");
            GameManager gamemNager = ServiceLocator.Instance.GetService<GameManager>(ManagerType.GAME_MANAGER);
            gamemNager.DealDamage(other.gameObject, 10);
        }
    }
}
