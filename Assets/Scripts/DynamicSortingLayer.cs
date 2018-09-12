using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DynamicSortingLayer : MonoBehaviour {

    #region Vars
    SortingGroup m_SortingGroup;
    #endregion

    // Use this for initialization
    void Start () {
        m_SortingGroup = GetComponent<SortingGroup>();

        if (!m_SortingGroup)
            Debug.LogWarning("You forgot to add the \"Sorting Group\" component to the gameObject : " + gameObject.name);
	}
	
	// Update is called once per frame
	void Update () {
        if (m_SortingGroup)
        {
            m_SortingGroup.sortingLayerName = "LevelObject";
            m_SortingGroup.sortingOrder = (int)(-transform.position.z * 10f);
        }
	}
}
