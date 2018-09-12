using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionController : MonoBehaviour
{
    [SerializeField] TreeController TreeController;

    [SerializeField] Text SkillName;
    [SerializeField] Text SkillDescription;
    [SerializeField] Text VitaePrices;
    [SerializeField] Text Status;

    private void Start()
    {
        TreeController.RefreshDescription += RefreshDescription;
    }

    void RefreshDescription(CustomImage image)
    {
        SkillName.text = image.skillName;
        SkillDescription.text = image.skillDescription;
        if (image.isUnlock)
            VitaePrices.text = "-----";
        else
            VitaePrices.text = image.price.ToString();

        if (image.isUnlock)
            Status.text = "UNLOCKED";
        else if (!image.isUnlockable)
            Status.text = "LOCKED";
        else
            Status.text = "UNLOCKABLE";
    }

    private void OnDestroy()
    {
        TreeController.RefreshDescription -= RefreshDescription;
    }
}
