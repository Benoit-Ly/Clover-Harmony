using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    [SerializeField]
    Player player;
    [SerializeField]
    int nbPlayer;

    Slider healthBar;

    // Use this for initialization
    void Start()
    {
        healthBar = GetComponent<Slider>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        healthBar.value = (float)ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER).GetPlayer(nbPlayer).HP / ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER).GetPlayer(nbPlayer).MaxHP;
	}
}
