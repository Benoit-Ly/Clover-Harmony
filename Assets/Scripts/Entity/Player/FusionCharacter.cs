using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionCharacter : MonoBehaviour {

    #region Services
    PlayerManager m_PlayerManager;
    #endregion

    [SerializeField]
    GameObject m_FXPower1;
    [SerializeField]
    GameObject m_FXPower2;
    [SerializeField]
    GameObject m_FXPower3;
    [SerializeField]
    GameObject m_FXPower4;


    // Use this for initialization
    void Start () {
        m_PlayerManager = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER);
	}

    private void OnEnable()
    {
        // Temp
        //StartCoroutine(Countdown());
    }

    // Update is called once per frame
    void Update () {
		
	}

    // Temp
    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(2f);
        m_PlayerManager.ReportFusionActionComplete();
    }
}
