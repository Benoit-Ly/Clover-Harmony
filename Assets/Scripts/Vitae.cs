using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vitae : MonoBehaviour {

    #region Services
    ItemManager m_ItemManager;
    #endregion

    #region Vars
    [SerializeField]
    int m_Amount = 1;
    #endregion

    // Use this for initialization
    void Start () {
        m_ItemManager = ServiceLocator.Instance.GetService<ItemManager>(ManagerType.ITEM_MANAGER);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        m_ItemManager.ReportCollect(other.gameObject, m_Amount);

        Destroy(gameObject);
    }
}
