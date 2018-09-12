using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BTAEditor : EditorWindow {

    #region Params
    List<EditorTab> m_TabList;
    List<string> m_TabNames;

    int m_currentTab;
    int m_previousTab = -1;
    #endregion

    [MenuItem("Window/Beat'em All Settings")]
    static void CreateWindow()
    {
        EditorWindow.GetWindow<BTAEditor>();
    }

    private void OnEnable()
    {
        m_TabList = new List<EditorTab>();
        m_TabNames = new List<string>();

        AddTab("General", new GeneralTab());
        AddTab("Physics", new PhysicsTab());
        AddTab("Player 1", new PlayerTab(1));
        AddTab("Player 2", new PlayerTab(2));
        AddTab("Enemy Types", new EnemyTypesTab());
        AddTab("Narration", new Narration());
        AddTab("Power Ups", new PowerUpsTab());
    }

    private void OnGUI()
    {
        m_currentTab = GUILayout.Toolbar(m_currentTab, m_TabNames.ToArray());

        if (m_currentTab != m_previousTab)
        {
            m_previousTab = m_currentTab;
            GUI.FocusControl(null);
        }

        m_TabList[m_currentTab].OnGUI();
    }

    void AddTab(string name, EditorTab newTab)
    {
        m_TabNames.Add(name);
        m_TabList.Add(newTab);
    }
}
