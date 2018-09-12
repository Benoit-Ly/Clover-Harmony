using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PhysicsTab : EditorTab {

    GamePhysics m_Physics;

    public PhysicsTab()
    {
        m_Physics = AssetDatabase.LoadAssetAtPath<GamePhysics>("Assets/Data/Config/GamePhysics.asset");
    }

    public override void OnGUI()
    {
        base.OnGUI();

        m_Physics.Knockback = DisplayKnockFields("Knockback Motion", m_Physics.Knockback);

        EditorGUILayout.Space();

        m_Physics.Knockup = DisplayKnockFields("Knockup Motion", m_Physics.Knockup);

        EditorGUILayout.Space();

        m_Physics.Knockdown = DisplayKnockFields("Knockdown Motion", m_Physics.Knockdown);

        if (GUI.changed)
            EditorUtility.SetDirty(m_Physics);
    }

    public GamePhysics.Knock DisplayKnockFields(string label, GamePhysics.Knock knock)
    {
        GamePhysics.Knock newKnock = knock;

        EditorGUILayout.LabelField(label);
        newKnock.Angle = EditorGUILayout.FloatField("Angle", newKnock.Angle);
        newKnock.Force = EditorGUILayout.FloatField("Force", newKnock.Force);
        newKnock.Speed = EditorGUILayout.FloatField("Speed", newKnock.Speed);

        return newKnock;
    }
}
