using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Narration : EditorTab
{
    public NarrationData narrationData; // to get for draw
    List<bool> m_Foldout;

    public Narration()
    {
        narrationData = AssetDatabase.LoadAssetAtPath<NarrationData>("Assets/Data/Narration/NarrationData.asset");

        if (narrationData != null)
            InitFoldout();
    }

    void InitFoldout()
    {
        m_Foldout = new List<bool>();

        int nbNarrative = narrationData.NarrativeScreen.Count;
        for (int i = 0; i < nbNarrative; i++)
        {
            m_Foldout.Add(false);
        }
    }

    public override void OnGUI()
    {
        base.OnGUI();

        if (narrationData == null)
        {
            Debug.LogError("Narration Data not found");
            return;
        }

        GUILayout.Space(10f);

        if (GUILayout.Button("Add new NarrativeScreen"))
            CreateNewNarrativeScreen();

        DisplayNarrative();

        if (GUI.changed)
            EditorUtility.SetDirty(narrationData);
    }

    void CreateNewNarrativeScreen()
    {
        narrationData.NarrativeScreen.Add(new NarrativeScreen());
        m_Foldout.Add(false);
    }

    void DisplayNarrative()
    {
        int nbNarrative = narrationData.NarrativeScreen.Count;

        for (int i = 0; i < nbNarrative; i++)
        {
            NarrativeScreen stats = narrationData.NarrativeScreen[i];

            string label;

            if (!string.IsNullOrEmpty(stats.name))
                label = stats.name;
            else
                label = "Narrative " + i;

            m_Foldout[i] = EditorGUILayout.Foldout(m_Foldout[i], label);

            if (m_Foldout[i])
            {
                stats.name = EditorGUILayout.TextField("Narrative Name", stats.name);
                stats.descriptionOneCharaOne = EditorGUILayout.TextField("DescriptionOneCharaOne", stats.descriptionOneCharaOne);
                stats.descriptionTwoCharaOne = EditorGUILayout.TextField("DescriptionTwoCharaOne", stats.descriptionTwoCharaOne);
                stats.descriptionThreeCharaOne = EditorGUILayout.TextField("DescriptionThreeCharaOne", stats.descriptionThreeCharaOne);
                stats.descriptionOneCharaTwo = EditorGUILayout.TextField("DescriptionOneCharaTwo", stats.descriptionOneCharaTwo);
                stats.descriptionTwoCharaTwo = EditorGUILayout.TextField("DescriptionTwoCharaTwo", stats.descriptionTwoCharaTwo);
                stats.descriptionThreeCharaTwo = EditorGUILayout.TextField("DescriptionThreeCharaTwo", stats.descriptionThreeCharaTwo);
                stats.image = EditorGUILayout.ObjectField("Image", stats.image, typeof(Image), true) as Image;
                stats.number = EditorGUILayout.IntField("DescriptionNumber", stats.number);

                narrationData.NarrativeScreen[i] = stats;

                GUILayout.Space(5f);
            }
        }
    }
}
