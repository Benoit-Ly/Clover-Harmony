using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour {

    #region Services
    protected ControllerManager m_ControllerManager;
    #endregion

    // Use this for initialization
    protected virtual void Start () {
        m_ControllerManager = ServiceLocator.Instance.GetService<ControllerManager>(ManagerType.CONTROLLER_MANAGER);

        m_ControllerManager.RegisterController(this);
	}

    protected virtual void Update()
    {
        
    }

    private void OnDestroy()
    {
        m_ControllerManager.ReportDestroyed(this);
    }
}
