using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(CustomImage))]
public class CustomImageEditor : ImageEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();//Draw inspector UI of ImageEditor

        CustomImage image = (CustomImage)target;
        image.selectID = EditorGUILayout.IntField("selectID", image.selectID);
        image.price = EditorGUILayout.IntField("price", image.price);

        EditorGUILayout.Space();

        int newCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("restriction size", image.restrictionImages.Count));
        while (newCount < image.restrictionImages.Count)
            image.restrictionImages.RemoveAt(image.restrictionImages.Count - 1);
        while (newCount > image.restrictionImages.Count)
            image.restrictionImages.Add(null);

        for (int i = 0; i < image.restrictionImages.Count; i++)
        {
            image.restrictionImages[i] = EditorGUILayout.ObjectField("restriction skill", image.restrictionImages[i], typeof(CustomImage), true) as CustomImage;
        }

        EditorGUILayout.Space();

        image.skillName = EditorGUILayout.TextField("skill name", image.skillName);
        image.skillDescription = EditorGUILayout.TextField("skill description", image.skillDescription);
        image.defaultSprite = EditorGUILayout.ObjectField("default", image.defaultSprite, typeof(Sprite), true) as Sprite;
        image.lockedSprite = EditorGUILayout.ObjectField("locked", image.lockedSprite, typeof(Sprite), true) as Sprite;
        image.unlockedSprite = EditorGUILayout.ObjectField("unlocked", image.unlockedSprite, typeof(Sprite), true) as Sprite;
    }
}
