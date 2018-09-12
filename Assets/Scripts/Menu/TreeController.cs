using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TreeController : MonoBehaviour
{
    public event Action<CustomImage> RefreshDescription;
    [Range(1, 2)] public int PlayerNumber = 1;

    [SerializeField] ShopMenu Shop;
    [SerializeField] float AxisValue = 0.5f;
    [SerializeField] float SwitchDelay = 0.1f;
    [SerializeField] Vector2 DefaultSize = new Vector2(15f, 15f);
    [SerializeField] Vector2 ZoomSize = new Vector2(30f, 30f);
    [SerializeField] float ZoomTime = 1f;
    [SerializeField] List<CustomImage> m_SkillImages = new List<CustomImage>();

    int m_CurrentSelectedID = 0;
    CustomImage m_CurrentImageSelected;
    string m_PlayerPrefix;

    GameManager m_GameManager;
    bool m_selected = false;

    Coroutine m_SwitchCoroutine;
    Coroutine m_ValidationCoroutine;

    private void Start()
    {
        m_PlayerPrefix = PlayerNumber == 1 ? "P1_" : "P2_";
        Shop.ShopClosed += OnShopClosed;
        m_GameManager = ServiceLocator.Instance.GetService<GameManager>(ManagerType.GAME_MANAGER);

        bool isClover = PlayerNumber == 1 ? true : false;
        if (isClover)
        {
            LoadShopTree(m_GameManager.CloverTreeSave);
        }
        else
        {
            LoadShopTree(m_GameManager.AmaranthTreeSave);
        }

        RefreshTree();
        m_CurrentImageSelected = m_SkillImages[0];
        m_SwitchCoroutine = StartCoroutine(CurrentIndexChanged(0));
    }

    private void OnEnable()
    {
        RefreshTree();
        m_CurrentImageSelected = m_SkillImages[0];
        m_SwitchCoroutine = StartCoroutine(CurrentIndexChanged(0)); // reset at first image
    }

    void OnShopClosed()
    {
        bool isClover = PlayerNumber == 1 ? true : false;

        m_GameManager.SaveShopTree(isClover, m_SkillImages);
    }

    private void LoadShopTree(List<ShopTreeSave> saves)
    {
        foreach (CustomImage image in m_SkillImages)
        {
            foreach (ShopTreeSave save in saves)
            {
                if (image.selectID == save.Save_selectID)
                {
                    image.isUnlock = save.Save_isUnlock;
                    image.isUnlockable = save.Save_isUnlockable;
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown(m_PlayerPrefix + "Fire2"))
        {
            Shop.LeaveTheShop();
        }

        if (Input.GetAxis(m_PlayerPrefix + "Horizontal") > AxisValue && !m_selected)
        {
            m_SwitchCoroutine = StartCoroutine(CurrentIndexChanged(m_CurrentSelectedID + 1));
        }

        if (Input.GetAxis(m_PlayerPrefix + "Horizontal") < -AxisValue && !m_selected)
        {
            StartCoroutine(CurrentIndexChanged(m_CurrentSelectedID - 1));
        }

        if (Input.GetAxis(m_PlayerPrefix + "Vertical") > AxisValue && !m_selected)
        {
            StartCoroutine(CurrentIndexChanged(m_CurrentSelectedID - 10));
        }

        if (Input.GetAxis(m_PlayerPrefix + "Vertical") < -AxisValue && !m_selected)
        {
            m_SwitchCoroutine = StartCoroutine(CurrentIndexChanged(m_CurrentSelectedID + 10));
        }

        if (Input.GetButtonDown(m_PlayerPrefix + "Jump"))
        {
            m_ValidationCoroutine = StartCoroutine(ValidationButtonPressed());
        }
    }

    void RefreshTree()
    {
        foreach (CustomImage image in m_SkillImages)
        {
            if (image.isUnlock)
                image.sprite = image.unlockedSprite;

            else if (image.restrictionImages.Count > 0)
            {
                bool unlocked = false;
                foreach (CustomImage restrictionImage in image.restrictionImages)
                {
                    if (restrictionImage.isUnlock)
                    {
                        unlocked = true;
                    }
                }

                if (unlocked)
                {
                    image.isUnlockable = true;
                    image.sprite = image.defaultSprite;
                }
                else
                    image.sprite = image.lockedSprite;

            }
            else
            {
                image.isUnlockable = true;
                image.sprite = image.defaultSprite;
            }
        }
    }

    IEnumerator ValidationButtonPressed()
    {
        if (m_CurrentImageSelected.isUnlockable && !m_CurrentImageSelected.isUnlock)
        {
            if (Shop.TryToBuySkills(m_CurrentImageSelected, PlayerNumber))
            {
                m_CurrentImageSelected.isUnlock = true;
                RefreshTree();

                StartCoroutine(ZoomIn(m_CurrentImageSelected));

                if (RefreshDescription != null)
                    RefreshDescription(m_CurrentImageSelected);
            }
            else
                ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Click", 3);
        }
        else
            ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Click", 3);

        yield return new WaitForSeconds(0.2f);

        StopCoroutine(m_ValidationCoroutine);
    }

    IEnumerator ZoomIn(CustomImage image)
    {
        for (float t = 0; t < 1; t += Time.deltaTime / ZoomTime)
        {
            image.rectTransform.sizeDelta = Vector2.Lerp(DefaultSize, ZoomSize, t);
            yield return null;
        }
    }

    IEnumerator ZoomOut(CustomImage image)
    {
        for (float t = 0; t < 1; t += Time.deltaTime / ZoomTime)
        {
            image.rectTransform.sizeDelta = Vector2.Lerp(ZoomSize, DefaultSize, t);
            yield return null;
        }
    }

    private void SetNewImage(CustomImage image)
    {
        if (m_CurrentImageSelected.isUnlock)
            m_CurrentImageSelected.sprite = m_CurrentImageSelected.unlockedSprite;
        else if (m_CurrentImageSelected.isUnlockable)
            m_CurrentImageSelected.sprite = m_CurrentImageSelected.defaultSprite;
        else
            m_CurrentImageSelected.sprite = m_CurrentImageSelected.lockedSprite;

        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Choose", 3);

        StartCoroutine(ZoomOut(m_CurrentImageSelected));

        m_CurrentImageSelected = image;
        m_CurrentSelectedID = image.selectID;

        StartCoroutine(ZoomIn(m_CurrentImageSelected));

        if (RefreshDescription != null)
            RefreshDescription(m_CurrentImageSelected);
    }

    IEnumerator CurrentIndexChanged(int newID)
    {
        m_selected = true;
        foreach (CustomImage image in m_SkillImages)
        {
            if (newID == image.selectID)
            {
                SetNewImage(image);
                yield return new WaitForSeconds(SwitchDelay);
                m_selected = false;
                StopCoroutine(m_SwitchCoroutine);
            }
        }

        yield return new WaitForSeconds(SwitchDelay);

        m_selected = false;

        StopCoroutine(m_SwitchCoroutine);
    }

    private void OnDestroy()
    {
        Shop.ShopClosed -= OnShopClosed;
    }
}
