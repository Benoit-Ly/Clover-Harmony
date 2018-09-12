using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideHUD : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).HideHUD();
    }
	
	// Update is called once per frame
	void Update () {
        if ( (Input.GetButtonDown("P1_Fire2")) || (Input.GetButtonDown("P2_Fire2")) )
            ServiceLocator.Instance.GetService<LevelManager>(ManagerType.LEVEL_MANAGER).LoadMainMenu();
    }
}
