using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneDialogue", menuName = "Narration/SceneDialogue", order = 0)]
public class SceneDialogue : ScriptableObject
{
    public Sprite Background;
    public List<TextsOfThoughts> TextsOfThoughtsList = new List<TextsOfThoughts>();
}
