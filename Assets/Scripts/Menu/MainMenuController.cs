using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] float AxisValue = 0.5f;
    [SerializeField] float SwitchDelay = 0.1f;
    [SerializeField] List<MenuImage> m_MenuImage = new List<MenuImage>();

    int m_PlayerOneSelectedID = 0;
    int m_PlayerTwoSelectedID = 0;

    MenuImage m_PlayerOneSelectedImage;
    MenuImage m_PlayerTwoSelectedImage;

    bool m_PlayerOneSelection = false;
    bool m_PlayerTwoSelection = false;

    bool m_IsInTitleMenu = true;
    bool m_IsInCredits = false;
    Animator m_Animator;

    GUIManager m_GUIManager;
    SoundManager m_SoundManager;
    GameObject m_PageCredits;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();

        m_SoundManager = ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER);
        m_GUIManager = ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER);

        m_PageCredits = transform.Find("Pages/Credits").gameObject;

        if (m_GUIManager.CreditsRequested)
            InitCredits();

        m_PlayerOneSelectedImage = m_MenuImage[0];
        m_PlayerTwoSelectedImage = m_MenuImage[0];

        StartCoroutine(PlayerOneIndexChanged(0));
        StartCoroutine(PlayerTwoIndexChanged(0));
    }

    private void Update()
    {
        if (m_IsInCredits)
        {
            if (Input.GetButtonDown("P1_Fire2") || Input.GetButtonDown("P2_Fire2"))
            {
                m_PageCredits.SetActive(false);
                m_IsInCredits = false;
            }
        }
        else if (m_IsInTitleMenu)
        {
            if (Input.GetButtonDown("P1_Jump"))
            {
                m_SoundManager.PlaySong("event:/SFX/UI/UI_Click", 3);
                m_Animator.SetBool("SwitchToMainMenu", true);
            }

            if (Input.GetButtonDown("P2_Jump"))
            {
                m_SoundManager.PlaySong("event:/SFX/UI/UI_Click", 3);
                m_Animator.SetBool("SwitchToMainMenu", true);
            }
        }
        else
        {
            if (Input.GetAxis("P1_Vertical") > AxisValue && !m_PlayerOneSelection)
                StartCoroutine(PlayerOneIndexChanged(m_PlayerOneSelectedID - 1));

            if (Input.GetAxis("P1_Vertical") < -AxisValue && !m_PlayerOneSelection)
                StartCoroutine(PlayerOneIndexChanged(m_PlayerOneSelectedID + 1));

            if (Input.GetButtonDown("P1_Jump"))
                PlayerOneValidationButtonPressed();

            if (Input.GetAxis("P2_Vertical") > AxisValue && !m_PlayerTwoSelection)
                StartCoroutine(PlayerTwoIndexChanged(m_PlayerTwoSelectedID - 1));

            if (Input.GetAxis("P2_Vertical") < -AxisValue && !m_PlayerTwoSelection)
                StartCoroutine(PlayerTwoIndexChanged(m_PlayerTwoSelectedID + 1));

            if (Input.GetButtonDown("P2_Jump"))
                PlayerTwoValidationButtonPressed();
        }
    }

    void InitCredits()
    {
        m_IsInCredits = true;
        m_GUIManager.CreditsRequested = false;

        m_PageCredits.SetActive(true);
    }

    public void TitleToMainMenuAnimationEnd(string s)
    {
        m_IsInTitleMenu = false;
    }

    private IEnumerator PlayerOneIndexChanged(int index)
    {
        m_PlayerOneSelection = true;

        foreach (MenuImage image in m_MenuImage)
        {
            if (index == image.selectID)
            {
                PlayerOneSelectChange(image);
                yield return new WaitForSeconds(SwitchDelay);
                m_PlayerOneSelection = false;
                yield return null;
            }
        }

        yield return new WaitForSeconds(SwitchDelay);
        m_PlayerOneSelection = false;
        yield return null;
    }

    private IEnumerator PlayerTwoIndexChanged(int index)
    {
        m_PlayerTwoSelection = true;

        foreach (MenuImage image in m_MenuImage)
        {
            if (index == image.selectID)
            {
                PlayerTwoSelectChange(image);
                yield return new WaitForSeconds(SwitchDelay);
                m_PlayerTwoSelection = false;
                yield return null;
            }
        }

        yield return new WaitForSeconds(SwitchDelay);
        m_PlayerTwoSelection = false;
        yield return null;
    }

    private void PlayerOneSelectChange(MenuImage image)
    {
        m_SoundManager.PlaySong("event:/SFX/UI/UI_Choose", 3);
        m_PlayerOneSelectedImage.playerOne.SetActive(false);

        m_PlayerOneSelectedID = image.selectID;
        m_PlayerOneSelectedImage = image;

        image.playerOne.SetActive(true);
    }

    private void PlayerTwoSelectChange(MenuImage image)
    {
        m_SoundManager.PlaySong("event:/SFX/UI/UI_Choose", 3);
        m_PlayerTwoSelectedImage.playerTwo.SetActive(false);

        m_PlayerTwoSelectedID = image.selectID;
        m_PlayerTwoSelectedImage = image;

        image.playerTwo.SetActive(true);
    }

    private void  PlayerOneValidationButtonPressed()
    {
        switch (m_PlayerOneSelectedImage.selectID)
        {
            case 0:     // Play
                m_SoundManager.PlaySong("event:/SFX/UI/UI_Click", 3);
                ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).PlayButtonPressed();
                break;

            case -1:    // Quit
                m_SoundManager.PlaySong("event:/SFX/UI/UI_Click", 3);
                ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).QuitButtonPressed();
                break;

            case -2:    // Credit
                m_SoundManager.PlaySong("event:/SFX/UI/UI_Click", 3);
                m_PageCredits.SetActive(true);
                m_IsInCredits = true;
                break;

            default:
                break;
        }
    }

    private void PlayerTwoValidationButtonPressed()
    {        
        switch (m_PlayerTwoSelectedImage.selectID)
        {
            case 0:     // Play
                m_SoundManager.PlaySong("event:/SFX/UI/UI_Click", 3);
                ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).PlayButtonPressed();
                break;

            case -1:    // Quit
                m_SoundManager.PlaySong("event:/SFX/UI/UI_Click", 3);
                ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).QuitButtonPressed();
                break;

            case -2:    // Credit
                m_SoundManager.PlaySong("event:/SFX/UI/UI_Click", 3);
                m_PageCredits.SetActive(true);
                m_IsInCredits = true;
                break;

            default:
                break;
        }
    }
}
