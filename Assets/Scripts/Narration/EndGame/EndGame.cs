using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour {

	GameObject gfx;
	PlayerManager m_PlayerManager;
	int endVitaeAmount;
	Text vitaeText;
	[SerializeField] Image vitaeSlider;
	bool complete;
	Animator animator;
    GUIManager m_GUIManager;
    GameManager m_GameManager;

    float fillAmount = 0f;
    int maxAmount = 0;

    [SerializeField]
    float minimumHarmonyRate = 0.7f;

    [SerializeField]
    int vitaePerFrame = 10;

    int currentVitae = 0;

	void Start () {
		m_PlayerManager = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER);
        m_GUIManager = ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER);
        m_GameManager = ServiceLocator.Instance.GetService<GameManager>(ManagerType.GAME_MANAGER);


        animator = GetComponent<Animator>();
		vitaeText = GameObject.Find("HUD(Clone)").GetComponentInChildren<Text>();
        endVitaeAmount = m_PlayerManager.Vitae;
		vitaeText.text = endVitaeAmount.ToString();

        maxAmount = (int)(m_PlayerManager.TotalVitae * minimumHarmonyRate);
        Debug.Log(maxAmount);

        m_PlayerManager.OnAllDead += TriggerBadEnding;
	}
	
	void Update () {
		if(Input.GetButton("P1_Fire3") || Input.GetButton("P2_Fire3"))
		{
			if(!complete)
				ReduceVitae();	
		}
	}

    private void LateUpdate()
    {
        vitaeSlider.fillAmount = fillAmount;
    }

    void ReduceVitae()
	{
        m_PlayerManager.SpendVitae(vitaePerFrame, true);
        currentVitae += vitaePerFrame;

        if (currentVitae >= maxAmount)
        {
            complete = true;
            animator.SetTrigger("win");
            ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).HideHUD();
        }
        else if (m_PlayerManager.Vitae <= 0)
        {
            complete = true;
            m_PlayerManager.SentencePlayers();
        }

        fillAmount = (float)currentVitae / maxAmount;
    }

    void TriggerBadEnding()
    {
        animator.SetTrigger("lose");
    }

    public void FinishGame()
    {
        StartCoroutine(CountdownBeforeRestart(3f));
    }

    IEnumerator CountdownBeforeRestart(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        //m_GUIManager.RequestCredits();
        ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).HideHUD();
        m_GameManager.Win();
        ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).HideHUD();
    }

    private void OnDestroy()
    {
        m_PlayerManager.OnAllDead -= TriggerBadEnding;
    }
}
