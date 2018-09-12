using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GeneralTab : EditorTab
{
    GameSettings m_Settings;

    public GeneralTab()
    {
        m_Settings = AssetDatabase.LoadAssetAtPath<GameSettings>("Assets/Data/Config/Settings.asset");
    }

    int NonZeroIntField(string label, int sourceValue)
    {
        int newValue = EditorGUILayout.IntField(label, sourceValue);

        if (newValue == 0)
            newValue = 1;

        return newValue;
    }

    public override void OnGUI()
    {
        base.OnGUI();

        //m_Settings.MaxLives = EditorGUILayout.IntField("Max life number", m_Settings.MaxLives);
        //m_Settings.FusionSpeed = EditorGUILayout.FloatField("Fusion Speed", m_Settings.FusionSpeed);

        m_Settings.FusionHitRatio = NonZeroIntField("Fusion Ratio (Hit)", m_Settings.FusionHitRatio);
        m_Settings.FusionKillRatio = NonZeroIntField("Fusion Ratio (Kill)", m_Settings.FusionKillRatio);
        m_Settings.FusionTimeRatio = NonZeroIntField("Fusion Ratio (Over Time)", m_Settings.FusionTimeRatio);
        m_Settings.MaxFusionRatio = NonZeroIntField("Max Fusion Ratio", m_Settings.MaxFusionRatio);

        EditorGUILayout.Space();

        m_Settings.MinFusionCap = EditorGUILayout.IntField("Fusion Cap (Min)", m_Settings.MinFusionCap);
        m_Settings.MaxFusionCap = EditorGUILayout.IntField("Fusion Cap (Max)", m_Settings.MaxFusionCap);
        m_Settings.FusionConsumptionAmount = EditorGUILayout.IntField("Fusion Consumption Amount", m_Settings.FusionConsumptionAmount);

        if (GUI.changed)
            EditorUtility.SetDirty(m_Settings);
    }
}
