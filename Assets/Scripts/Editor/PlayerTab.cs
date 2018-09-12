using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerTab : EditorTab {

    #region Params
    PlayerData m_playerData;
    List<bool> m_Foldout;
    #endregion

    public PlayerTab(int playerNum)
    {
        m_playerData = AssetDatabase.LoadAssetAtPath<PlayerData>("Assets/Data/Player/P" + playerNum + "Data.asset");

        if (m_playerData != null)
            InitFoldout();
    }

    void InitFoldout()
    {
        m_Foldout = new List<bool>();

        int nbAttacks = m_playerData.AttackList.Count;
        for (int i = 0; i < nbAttacks; i++)
        {
            m_Foldout.Add(false);
        }
    }

    public override void OnGUI()
    {
        base.OnGUI();

        if (m_playerData == null)
        {
            Debug.LogError("Player Data not found");
            return;
        }
            

        //EditorGUILayout.LabelField("This is Player " + m_PlayerNum + " area");
        m_playerData.HP = EditorGUILayout.IntField("HP", m_playerData.HP);
        m_playerData.Energy = EditorGUILayout.IntField("Energy", m_playerData.Energy);
        m_playerData.Speed = EditorGUILayout.FloatField("Horizontal Speed", m_playerData.Speed);
        m_playerData.DebuffSpeed = EditorGUILayout.FloatField("Slowed down Horizontal Speed", m_playerData.DebuffSpeed);
        m_playerData.VerticalSpeed = EditorGUILayout.FloatField("Vertical Speed", m_playerData.VerticalSpeed);
        m_playerData.DebuffVerticalSpeed = EditorGUILayout.FloatField("Slowed down Vertical Speed", m_playerData.DebuffVerticalSpeed);
        m_playerData.JumpHeight = EditorGUILayout.FloatField("Jump Height", m_playerData.JumpHeight);
        m_playerData.JumpSpeed = EditorGUILayout.FloatField("Jump Speed", m_playerData.JumpSpeed);

        GUILayout.Space(10f);

        if (GUILayout.Button("Add new Attack"))
            CreateNewAttack();

        DisplayAttackForm();

        if (GUI.changed)
            EditorUtility.SetDirty(m_playerData);
            
    }

    void CreateNewAttack()
    {
        m_playerData.AttackList.Add(new AttackStats());
        m_Foldout.Add(false);
    }

    void DisplayAttackForm()
    {
        int nbAttacks = m_playerData.AttackList.Count;
        for (int i = 0; i < nbAttacks; i++)
        {
            AttackStats stats = m_playerData.AttackList[i];

            string label;

            if (!string.IsNullOrEmpty(stats.name))
                label = stats.name;
            else
                label = "New Attack";

            m_Foldout[i] = EditorGUILayout.Foldout(m_Foldout[i], label);

            if (m_Foldout[i])
            {
                stats.name = EditorGUILayout.TextField("Attack Name", stats.name);
                stats.type = (AttackType)EditorGUILayout.EnumPopup("Type", stats.type);
                stats.launchDirection = (LaunchDirection)EditorGUILayout.EnumPopup("Launch Direction", stats.launchDirection);
                if (stats.launchDirection == LaunchDirection.MOVE)
                    stats.axis = (AttackStats.Axis)EditorGUILayout.EnumPopup("Axis", stats.axis);
                stats.damage = EditorGUILayout.IntField("Damage", stats.damage);
                stats.stunDuration = EditorGUILayout.IntField("Stun Duration", stats.stunDuration);

                m_playerData.AttackList[i] = stats;

                GUILayout.Space(5f);
            }
        }
    }
}
