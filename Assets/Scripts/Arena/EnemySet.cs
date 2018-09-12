using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySet", menuName = "Arena/EnemySet", order = 1)]
public class EnemySet : ScriptableObject
{
    public GameObject Enemy;
    public int Number;
    public bool RightSide;
}
