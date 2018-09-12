using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anima2D;

public class VisualFeedback : MonoBehaviour {

    [SerializeField]
    Color m_color;
    [SerializeField]
    float m_timeChangeHasApply;
    [SerializeField]
    List<GameObject> m_spriteList;

    List<SpriteMeshInstance> m_spriteMeshInstance;
    List<Color> m_baseColor;

    void Start ()
    {
        m_spriteMeshInstance = new List<SpriteMeshInstance>();
        m_baseColor = new List<Color>();

        for (int i = 0; i < m_spriteList.Count; ++i)
        {
            m_spriteMeshInstance.Add(m_spriteList[i].GetComponent<SpriteMeshInstance>());
            m_baseColor.Add(m_spriteList[i].GetComponent<SpriteMeshInstance>().color);
        }

        //StartCoroutine("ApplyChange");
    }

    public IEnumerator ApplyChange()
    {
        for (int i = 0; i < m_spriteMeshInstance.Count; ++i)
            m_spriteMeshInstance[i].color = m_color;

        yield return new WaitForSeconds(m_timeChangeHasApply);

        ReturnBaseColor();
    }

    void ReturnBaseColor()
    {
        for (int i = 0; i < m_spriteMeshInstance.Count; ++i)
            m_spriteMeshInstance[i].color = m_baseColor[i];
    }

}
