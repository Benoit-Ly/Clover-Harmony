using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarController : MonoBehaviour
{
    Player m_Player;

    GameObject EnergySlotPrefab;

    List<EnergySlotController> m_EnergySlots;

    HorizontalLayoutGroup m_LayoutGroup;

    int m_SlotNumber;
    int m_CurrentActiveSlot;

    public void Init(int playerNumber)
    {
        m_EnergySlots = new List<EnergySlotController>();

        m_LayoutGroup = GetComponent<HorizontalLayoutGroup>();

        string prefabPath = "";
        if (playerNumber == 1)
        {
            prefabPath = "Prefabs/GUI/CloverEnergySlot";
            GetComponent<RectTransform>().rotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
            GetComponent<RectTransform>().anchoredPosition = new Vector3(-475.0f, -250.0f, 0.0f);
        }
        else
        {
            prefabPath = "Prefabs/GUI/AmaranthEnergySlot";
            GetComponent<RectTransform>().anchoredPosition = new Vector3(475.0f, -250.0f, 0.0f);
        }

        EnergySlotPrefab = Resources.Load(prefabPath) as GameObject;

        PlayerManager playerManager = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER);
        m_Player = playerManager.GetPlayer(playerNumber);
        int slotNomber = m_Player.MaxEnergy;

        for (int i = 0; i < slotNomber; ++i)
        {
            AddNewEnergySlot();
        }

        m_Player.OnEnergyChange += OnEnergyChanged;
        m_Player.OnMaxEnergyChange += OnEnergySlotNumberChanged;
    }

    void OnEnergySlotNumberChanged(int slotNumber)
    {
        while (slotNumber > m_SlotNumber)
            AddNewEnergySlot();
    }

    void OnEnergyChanged(int number)
    {
        if (number < 0 || number > m_EnergySlots.Count)
            return;

        while (number < m_CurrentActiveSlot)
            UseSlot();

        while (number > m_CurrentActiveSlot)
            RechargeSlot();
    }

    private void UseSlot()
    {
        UseSlotRecursive(m_EnergySlots.Count - 1);
    }

    private void UseSlotRecursive(int idx)
    {
        if (idx < 0)
            return;

        if (m_EnergySlots[idx].IsActive())
        {
            m_EnergySlots[idx].UseSlot();
            m_CurrentActiveSlot--;
            return;
        }
        else
        {
            idx--;
            UseSlotRecursive(idx);
        }
    }

    private void RechargeSlot()
    {
        RechargeSlotRecursive(0);
    }

    private void RechargeSlotRecursive(int idx)
    {
        if (idx > m_EnergySlots.Count - 1)
            return;

        if (m_EnergySlots[idx].IsActive())
        {
            idx++;
            RechargeSlotRecursive(idx);
        }
        else
        {
            m_EnergySlots[idx].RechargeSlot();
            m_CurrentActiveSlot++;
            return;
        }
    }

    void AddNewEnergySlot()
    {
        if (EnergySlotPrefab != null)
        {
            EnergySlotController slot = Instantiate(EnergySlotPrefab, transform).GetComponent<EnergySlotController>();
            m_EnergySlots.Add(slot);
            m_SlotNumber++;
            m_CurrentActiveSlot++;
        }
        else
            Debug.LogWarning("EnergySlotPrefab Loading failed");
    }

    private void OnDestroy()
    {
        m_Player.OnEnergyChange -= OnEnergyChanged;
        m_Player.OnMaxEnergyChange -= OnEnergySlotNumberChanged;
    }
}
