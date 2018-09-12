using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Service {

    #region Events
    public event Action<GameObject, int> OnCollectVitae;
    #endregion

    private void Awake()
    {
        m_Type = ManagerType.ITEM_MANAGER;
    }

    public void ReportCollect(GameObject actor, int amount)
    {
        if (OnCollectVitae != null)
            OnCollectVitae(actor, amount);
    }
}
