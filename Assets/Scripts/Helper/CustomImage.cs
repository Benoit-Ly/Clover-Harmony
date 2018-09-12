using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomImage : Image
{
    public int selectID;
    public int price;
    public bool isUnlock;
    public bool isUnlockable;
    public List<CustomImage> restrictionImages = new List<CustomImage>();
    public string skillName;
    public string skillDescription;
    public Sprite defaultSprite;
    public Sprite lockedSprite;
    public Sprite unlockedSprite;
}
