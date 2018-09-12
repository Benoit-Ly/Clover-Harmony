using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
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

    private void Start()
    {
        m_PlayerOneSelectedImage = m_MenuImage[0];
        m_PlayerTwoSelectedImage = m_MenuImage[0];

        StartCoroutine(PlayerOneIndexChanged(0));
        StartCoroutine(PlayerTwoIndexChanged(0));
    }

    private void Update()
    {
        if (Input.GetAxis("P1_Vertical") > AxisValue && !m_PlayerOneSelection)
            StartCoroutine(PlayerOneIndexChanged(m_PlayerOneSelectedID - 1));

        if (Input.GetAxis("P1_Vertical") < -AxisValue && !m_PlayerOneSelection)
            StartCoroutine(PlayerOneIndexChanged(m_PlayerOneSelectedID + 1));

        if (Input.GetButtonDown("P1_Jump"))
            PlayerOneValidationButtonPressed();

        if (Input.GetButtonDown("P1_Fire2"))
            ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).ResumeButtonPressed();

        if (Input.GetAxis("P2_Vertical") > AxisValue && !m_PlayerTwoSelection)
            StartCoroutine(PlayerTwoIndexChanged(m_PlayerTwoSelectedID - 1));

        if (Input.GetAxis("P2_Vertical") < -AxisValue && !m_PlayerTwoSelection)
            StartCoroutine(PlayerTwoIndexChanged(m_PlayerTwoSelectedID + 1));

        if (Input.GetButtonDown("P2_Jump"))
            PlayerTwoValidationButtonPressed();

        if (Input.GetButtonDown("P2_Fire2"))
            ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).ResumeButtonPressed();
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
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Choose", 3);
        m_PlayerOneSelectedImage.playerOne.SetActive(false);

        m_PlayerOneSelectedID = image.selectID;
        m_PlayerOneSelectedImage = image;

        image.playerOne.SetActive(true);
    }

    private void PlayerTwoSelectChange(MenuImage image)
    {
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Choose", 3);
        m_PlayerTwoSelectedImage.playerTwo.SetActive(false);

        m_PlayerTwoSelectedID = image.selectID;
        m_PlayerTwoSelectedImage = image;

        image.playerTwo.SetActive(true);
    }

    private void PlayerOneValidationButtonPressed()
    {
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Click", 3);
        switch (m_PlayerOneSelectedImage.selectID)
        {
            case 0:     // Return
                ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).ResumeButtonPressed();
                break;

            //case -1:    // Restart
            //    ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).RestartButtonPressed();
            //    break;

            case -1:    // MainMenu
                ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).MainMenuButtonPressed();
                break;

            case -2:    // Quit
                ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).QuitButtonPressed();
                break;

            default:
                break;
        }
    }

    private void PlayerTwoValidationButtonPressed()
    {
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/UI/UI_Click", 3);
        switch (m_PlayerTwoSelectedImage.selectID)
        {
            case 0:     // Return
                ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).ResumeButtonPressed();
                break;

            //case -1:    // Restart
            //    ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).RestartButtonPressed();
            //    break;

            case -1:    // MainMenu
                ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).MainMenuButtonPressed();
                break;

            case -2:    // Quit
                ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).QuitButtonPressed();
                break;

            default:
                break;
        }
    }
}
