using UnityEngine;
using UnityEngine.UI;

public class Shop : TriggerAbstract
{
    bool m_InShopZone = false;
    [SerializeField] Text m_ShopMsg;

    protected override void TriggerEnter()
    {
        m_InShopZone = true;
        m_ShopMsg.gameObject.SetActive(true);
    }

    protected override void TriggerExit()
    {
        m_InShopZone = false;
        m_ShopMsg.gameObject.SetActive(false);

        base.TriggerExit();
    }

    private void Update()
    {
        if (m_InShopZone)
        {
            if (Input.GetButtonDown("P1_Fire3") || Input.GetButtonDown("P2_Fire3"))
            {
                ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER).ShowShopMenu();
            }
        }
    }
}
