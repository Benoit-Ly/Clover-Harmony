using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anima2D;

public class VisualFeedbackBrique : MonoBehaviour
{

    [SerializeField]
    Color m_color;
    [SerializeField]
    float m_timeChangeHasApply;

    SpriteRenderer m_spriteMeshInstance;
    Color m_baseColor;

    void Start()
    {
        m_spriteMeshInstance = GetComponent<SpriteRenderer>();
        m_baseColor = GetComponent<SpriteRenderer>().color;
    }

    public IEnumerator ApplyChange()
    {
        m_spriteMeshInstance.color = m_color;

        yield return new WaitForSeconds(m_timeChangeHasApply);

        ReturnBaseColor();
    }

    void ReturnBaseColor()
    {
        m_spriteMeshInstance.color = m_baseColor;
    }
}