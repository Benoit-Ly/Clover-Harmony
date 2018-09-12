using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyTypesTab : EditorTab {

    EnemyTypeListData m_Data;
    List<bool> m_FastFoldout;
    List<bool> m_HeavyFoldout;

    public EnemyTypesTab()
    {
        m_Data = AssetDatabase.LoadAssetAtPath<EnemyTypeListData>("Assets/Data/Enemy/EnemyTypeListData.asset");
        m_FastFoldout = InitFoldout(m_Data.FastEnemyData);
        m_HeavyFoldout = InitFoldout(m_Data.HeavyEnemyData);
    }

    List<bool> InitFoldout(EnemyTypeData data)
    {
        List<bool> foldout = new List<bool>();

        int nbAttacks = data.AttackList.Count;
        for (int i = 0; i < nbAttacks; i++)
        {
            foldout.Add(false);
        }

        return foldout;
    }

    public override void OnGUI()
    {
        base.OnGUI();

        EditorGUILayout.LabelField("Fast Enemy");
        m_Data.FastEnemyData = DisplayForm(m_Data.FastEnemyData, ref m_FastFoldout);

        GUILayout.Space(10f);

        EditorGUILayout.LabelField("Heavy Enemy");
        m_Data.HeavyEnemyData = DisplayForm(m_Data.HeavyEnemyData, ref m_HeavyFoldout);

        if (GUI.changed)
            EditorUtility.SetDirty(m_Data);
            
    }

    EnemyTypeData DisplayForm(EnemyTypeData data, ref List<bool> foldout)
    {
        EnemyTypeData newData = new EnemyTypeData();

        newData.Speed = EditorGUILayout.FloatField("Speed", data.Speed);
        newData.DashDistance = EditorGUILayout.FloatField("Dash Distance", data.DashDistance);
        newData.VitaeDropQuantity = EditorGUILayout.IntField("Vitae Drop Quantity", data.VitaeDropQuantity);
        newData.Agressivity = EditorGUILayout.IntSlider("Agressivity", data.Agressivity, 0, 100);

        bool addButton = GUILayout.Button("Add new attack");

        newData.AttackList = DisplayAttackForm(data, ref foldout);

        if (addButton)
        {
            newData.AttackList.Add(new AttackStats());
            foldout.Add(false);
        }
            

        return newData;
    }

    List<AttackStats> DisplayAttackForm(EnemyTypeData data, ref List<bool> foldout)
    {
        List<AttackStats> newList = new List<AttackStats>();

        int nbAttacks = data.AttackList.Count;
        for (int i = 0; i < nbAttacks; i++)
        {
            AttackStats stats = data.AttackList[i];

            string label;

            if (!string.IsNullOrEmpty(stats.name))
                label = stats.name;
            else
                label = "New Attack";

            foldout[i] = EditorGUILayout.Foldout(foldout[i], label);

            if (foldout[i])
            {
                stats.name = EditorGUILayout.TextField("Attack Name", stats.name);
                stats.damage = EditorGUILayout.IntField("Damage", stats.damage);
                stats.stunDuration = EditorGUILayout.IntField("Stun Duration", stats.stunDuration);

                GUILayout.Space(5f);
            }

            newList.Add(stats);
        }

        return newList;
    }
}
