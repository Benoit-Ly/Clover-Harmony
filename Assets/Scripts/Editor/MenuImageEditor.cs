using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(MenuImage))]
public class MenuImageEditor : ImageEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();//Draw inspector UI of ImageEditor

        MenuImage image = (MenuImage)target;
        image.selectID = EditorGUILayout.IntField("selectID", image.selectID);
        image.playerOne = EditorGUILayout.ObjectField("player one", image.playerOne, typeof(GameObject), true) as GameObject;
        image.playerTwo = EditorGUILayout.ObjectField("player two", image.playerTwo, typeof(GameObject), true) as GameObject;
    }
}
