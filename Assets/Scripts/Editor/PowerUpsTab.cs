using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PowerUpsTab : EditorTab {

    #region Structs
    public struct HierarchicFoldout
    {
        public bool[] SubFoldout;
        public bool Foldout;
    }
    #endregion

    #region Vars
    PowerUpsList m_Data;
    List<HierarchicFoldout> m_Foldout;

    Vector2 m_ScrollPos;
    #endregion

    public PowerUpsTab()
    {
        m_Data = AssetDatabase.LoadAssetAtPath<PowerUpsList>("Assets/Data/PowerUps/PowerUpsData.asset");

        InitFoldout();
    }

    void InitFoldout()
    {
        m_Foldout = new List<HierarchicFoldout>();
        for (int i = 0; i < 8; i++)
        {
            HierarchicFoldout newFoldout;
            newFoldout.SubFoldout = new bool[3];
            newFoldout.Foldout = false;

            m_Foldout.Add(newFoldout);
        }
    }

    public override void OnGUI()
    {
        base.OnGUI();

        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

        m_Data.PowerAttack = DisplayPowerUpForm("Increased Powered Attack", m_Data.PowerAttack, 0);
        m_Data.PowerRetention = DisplayPowerUpForm("Power Retention", m_Data.PowerRetention, 1);
        m_Data.AirAttack = DisplayPowerUpForm("Increased Air Attack", m_Data.AirAttack, 2);
        m_Data.CrowdControl = DisplayPowerUpForm("Crowd Control", m_Data.CrowdControl, 3);
        m_Data.PowerCharges = DisplayPowerUpForm("Power Attack Charges", m_Data.PowerCharges, 4);
        m_Data.PowerAbsorption = DisplayPowerUpForm("Power Absorption", m_Data.PowerAbsorption, 5);
        m_Data.Resilience = DisplayPowerUpForm("Resilience", m_Data.Resilience, 6);
        m_Data.LightAttack = DisplayPowerUpForm("Increased Attack", m_Data.LightAttack, 7);

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
            EditorUtility.SetDirty(m_Data);
    }

    PowerUp DisplayPowerUpForm(string title, PowerUp data, int foldoutIndex)
    {
        PowerUp newData = data;

        HierarchicFoldout foldout = m_Foldout[foldoutIndex];

        foldout.Foldout = EditorGUILayout.Foldout(m_Foldout[foldoutIndex].Foldout, title);
        if (foldout.Foldout)
        {
            int nbLevels = data.Levels.Count;
            for (int i = 0; i < nbLevels; i++)
            {
                string level = (i + 1).ToString();

                newData.Levels[i] = DisplayLevelForm("Level " + level, newData.Levels[i], ref foldout.SubFoldout[i]);

                GUILayout.Space(10);
            }

            GUILayout.Space(20);
        }

        m_Foldout[foldoutIndex] = foldout;

        return newData;
    }

    PowerLevel DisplayLevelForm(string label, PowerLevel data, ref bool foldout)
    {
        PowerLevel newData = data;

        foldout = EditorGUILayout.Foldout(foldout, label + " : " + newData.Name);

        if (foldout)
        {
            newData.Name = EditorGUILayout.TextField("Name", newData.Name);
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            DisplayIndividualForm("Clover", ref newData.CloverDescription, ref newData.CloverEffectValue);
            DisplayIndividualForm("Amaranth", ref newData.AmaranthDescription, ref newData.AmaranthEffectValue);
            EditorGUILayout.EndHorizontal();

            newData.CostMultiplier = EditorGUILayout.FloatField("Cost Multiplier", newData.CostMultiplier);
            newData.VitaePrice = EditorGUILayout.IntField("Price (Vitae)", newData.VitaePrice);
        }

        return newData;
    }

    void DisplayIndividualForm(string label, ref string description, ref float effectValue)
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField(label);
        description = EditorGUILayout.TextArea(description, GUILayout.Height(150f));
        effectValue = EditorGUILayout.FloatField("Effect value", effectValue);

        EditorGUILayout.EndVertical();
    }
}
