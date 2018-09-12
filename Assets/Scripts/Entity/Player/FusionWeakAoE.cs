using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionWeakAoE : MonoBehaviour {

    #region Services
    BattleManager m_BattleManager;
    #endregion

    // Use this for initialization
    void Start () {
        m_BattleManager = ServiceLocator.Instance.GetService<BattleManager>(ManagerType.BATTLE_MANAGER);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        m_BattleManager.ReportEnterWeakAoE(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        m_BattleManager.ReportExitWeakAoE(other.gameObject);
    }
}
