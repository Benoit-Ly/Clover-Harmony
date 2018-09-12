using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : Service
{
    #region Structs
    public struct DataCache
    {
        public int Vitae;
        public int TotalVitae;
        public float HarmonyRate;
    }
    #endregion

    #region Services
    BattleManager m_BattleManager;
    EnemyManager m_EnemyManager;
    GUIManager m_GUIManager;
    ItemManager m_ItemManager;
    #endregion

    // TODO: Clean that
    #region HUD
    FusionMenu m_FusionMenu;
    #endregion

    #region Events
    public event Action<Player> OnRegisterPlayer;

    public event Action<int, int> OnFusionScoreChange;
    public event Action<bool> OnFusionAvailable;
    public event Action OnFusionInit;
    public event Action OnFusionCancel;
    public event Action OnFusionComplete;
    public event Action OnDefuseComplete;
    public event Action OnAllDead;

    public event Action<int> OnVitaeChange;
    public event Action<bool> OnPayment;
    #endregion

    #region Vars
    GameSettings m_GameSettings;

    Player[] m_Players;
    GameObject m_FusionCharacterPrefab;
    GameObject m_FusionCharacter = null;

    PowerUpsLevels[] m_PowerUpsLevels;
    DataCache m_DataCache;

    int m_NbPlayers;

    int m_FusionRequestPlayerNum = -1;

    bool m_Invincible = false;

    int m_FusionScore = 100;          public int FusionScore { get { return m_FusionScore; } set { m_FusionScore = value; } }
    int m_MaxFusionScore = 1000;    public int MaxFusionScore { get { return m_FusionScore; } }

    int m_Vitae = 1;                public int Vitae { get { return m_Vitae; } }
    int m_TotalVitae = 1000;           public int TotalVitae { get { return m_TotalVitae; } }

    float m_HarmonyRate = 1f;       public float HarmonyRate { get { return m_HarmonyRate; } }

    bool m_IsFusionAvailable = false;
    bool m_IsOnFusionState = false;
    bool m_IsFusing = false;
    bool m_IsDefusing = false;

    ParticleSystem readyForFusion;

    FxManager fxManager;

    ParticleSystem FxFusionEnergy;
    ParticleSystem FxFusionHeal;
    ParticleSystem FxFusionAOE;
    ParticleSystem FxFusionInvincibilite;
    ParticleSystem FxFusionEnergyP2;
    ParticleSystem FxFusionHealP2;
    ParticleSystem FxFusionAOEP2;
    ParticleSystem FxFusionInvincibiliteP2;
    #endregion

    private void Awake()
    {
        m_Type = ManagerType.PLAYER_MANAGER;
        m_GameSettings = ServiceLocator.Instance.Settings;
        m_Players = new Player[2];
        m_NbPlayers = m_Players.Length;

        m_PowerUpsLevels = new PowerUpsLevels[2];

        InitDataCache();

        m_FusionCharacterPrefab = Resources.Load<GameObject>("Prefabs/FusionCharacter");

        UpdateMaxFusionGauge();

        ServiceLocator.Instance.StartServiceCoroutine(FillFusionGauge());

        m_BattleManager = ServiceLocator.Instance.GetService<BattleManager>(ManagerType.BATTLE_MANAGER);
        m_BattleManager.OnDamage += OnPlayerDamage;
        m_BattleManager.OnBrickDamage += OnBrickDamage;

        m_EnemyManager = ServiceLocator.Instance.GetService<EnemyManager>(ManagerType.ENEMY_MANAGER);
        m_EnemyManager.OnEnemyDeath += OnEnemyDeath;

        m_GUIManager = ServiceLocator.Instance.GetService<GUIManager>(ManagerType.GUI_MANAGER);
        m_GUIManager.OnFusionMenuInstantiation += OnGUIInstantiation;

        m_ItemManager = ServiceLocator.Instance.GetService<ItemManager>(ManagerType.ITEM_MANAGER);
        m_ItemManager.OnCollectVitae += OnCollectVitae;

        fxManager = ServiceLocator.Instance.GetService<FxManager>(ManagerType.FX_MANAGER);

        SceneManager.sceneLoaded += ResetStats;
    }

    void InitDataCache()
    {
        m_DataCache.Vitae = 1;
        m_DataCache.TotalVitae = 100;
        m_DataCache.HarmonyRate = 1f;
    }

    void OnGUIInstantiation()
    {
        m_GUIManager.Shop.SkillBought += OnNewPowerUp;
        m_GUIManager.Shop.ShopClosed += OnShopClosed;

        m_FusionMenu = m_GUIManager.FusionMenu.GetComponentInChildren<FusionMenu>();
        m_FusionMenu.HealSpellSelected += OnFusionHeal;
        m_FusionMenu.InvincibleSpellSelected += OnFusionInvincible;
        m_FusionMenu.ConfusionSpellSelected += SummonAoE;
        m_FusionMenu.EnergySpellSelected += OnFusionEnergy;
        m_FusionMenu.OnCancel += CancelFusion;
    }

    void ResetStats(Scene scene, LoadSceneMode mode)
    {
        m_FusionScore = 100;
        m_IsFusionAvailable = false;

        if (scene.buildIndex == 0)
        {
            m_Vitae = 1;
            m_TotalVitae = 100;

            m_HarmonyRate = 0.01f;

            ServiceLocator.Instance.GetService<GameManager>(ManagerType.GAME_MANAGER).ResetShop();
            ResetSkills();
            SaveStats();
        }
        else
        {
            LoadStats();
        }

        if (OnFusionScoreChange != null)
            OnFusionScoreChange(m_FusionScore, m_MaxFusionScore);
    }

    void ResetSkills()
    {
        for (int i = 0; i < m_NbPlayers; i++)
        {
            m_PowerUpsLevels[i].AirAttack = 0;
            m_PowerUpsLevels[i].CrowdControl = 0;
            m_PowerUpsLevels[i].LightAttack = 0;
            m_PowerUpsLevels[i].PowerAbsorption = 0;
            m_PowerUpsLevels[i].PowerAttack = 0;
            m_PowerUpsLevels[i].PowerCharges = 0;
            m_PowerUpsLevels[i].PowerRetention = 0;
            m_PowerUpsLevels[i].Resilience = 0;
        }
    }

    public void SaveStats()
    {
        m_DataCache.Vitae = m_Vitae;
        m_DataCache.TotalVitae = m_TotalVitae;
        m_DataCache.HarmonyRate = m_HarmonyRate;
    }

    void LoadStats()
    {
        m_Vitae = m_DataCache.Vitae;
        m_TotalVitae = m_DataCache.TotalVitae;
        m_HarmonyRate = m_DataCache.HarmonyRate;

        if (OnVitaeChange != null)
            OnVitaeChange(m_Vitae);
    }

    public void RegisterPlayer(Player player, int playerNum)
    {
        if (playerNum <= 0 || playerNum > m_NbPlayers)
        {
            Debug.LogError("This game cannot have a player " + playerNum);
            return;
        }

        m_Players[playerNum - 1] = player;
        player.ID = playerNum;

        player.SetPowers(m_PowerUpsLevels[playerNum - 1]);

        if (OnRegisterPlayer != null)
            OnRegisterPlayer(player);
    }

    public Player GetPlayer(int num)
    {
        if (num <= 0 || num > m_NbPlayers)
            return null;

        return m_Players[num - 1];
    }

    public void RespawnPlayer(Checkpoint checkpoint)
    {
        m_Players[0].RecoverAllHP();
        m_Players[1].RecoverAllHP();

        m_Players[0].RecoverAllEnergy();
        m_Players[1].RecoverAllEnergy();

        m_Players[0].transform.position = checkpoint.playerOne.position;
        m_Players[1].transform.position = checkpoint.playerTwo.position;
    }

    void UpdateMaxFusionGauge()
    {
        m_MaxFusionScore = m_Vitae / m_GameSettings.MaxFusionRatio;

        if (m_MaxFusionScore > m_GameSettings.MaxFusionCap)
            m_MaxFusionScore = m_GameSettings.MaxFusionCap;

        if (m_MaxFusionScore < m_GameSettings.MinFusionCap)
            m_MaxFusionScore = m_GameSettings.MinFusionCap;

        if (OnFusionScoreChange != null)
            OnFusionScoreChange(m_FusionScore, m_MaxFusionScore);
    }

    public void AddFusionScore(int value = 1)
    {
        if (value == 0 && m_Vitae > 0)
            value = 1;

        m_FusionScore += value;

        if (m_FusionScore > m_MaxFusionScore)
            m_FusionScore = m_MaxFusionScore;

        if (OnFusionScoreChange != null)
            OnFusionScoreChange(m_FusionScore, m_MaxFusionScore);

        if (m_FusionScore >= m_GameSettings.FusionConsumptionAmount && !m_IsFusionAvailable && OnFusionAvailable != null)
        {
            m_IsFusionAvailable = true;
            OnFusionAvailable(true);
        }
            
    }

    void UseFusionScore(int amount)
    {
        m_FusionScore -= amount;

        if (m_FusionScore < 0)
            m_FusionScore = 0;

        if (OnFusionScoreChange != null)
            OnFusionScoreChange(m_FusionScore, m_MaxFusionScore);

        if (m_FusionScore < m_GameSettings.FusionConsumptionAmount && m_IsFusionAvailable && OnFusionAvailable != null)
        {
            m_IsFusionAvailable = false;
            OnFusionAvailable(false);
        }
    }

    public void ResetFusionScore()
    {
        m_FusionScore = 0;

        if (OnFusionScoreChange != null)
            OnFusionScoreChange(m_FusionScore, m_MaxFusionScore);

        if (m_IsFusionAvailable && OnFusionAvailable != null)
        {
            m_IsFusionAvailable = false;
            OnFusionAvailable(false);
        }
    }

    public bool ReportReadyForFusion(int playerNum)
    {
        // Uncomment this when fusion debugging is finished
        if (m_FusionScore >= m_GameSettings.FusionConsumptionAmount && !m_IsFusing)
        {
            int index = playerNum - 1;

            //if (readyForFusion == null)
            //    readyForFusion = Instantiate(m_Players[index].ReadyForFusionFX.GetComponent<ParticleSystem>(), m_Players[index].transform);

            ////readyForFusion.transform.position = m_Players[index].transform.position;
            //readyForFusion.Play(true);

            if (m_FusionRequestPlayerNum == -1)
            {
                m_FusionRequestPlayerNum = index;
            }
            else if (m_FusionRequestPlayerNum != index)
            {
                m_IsOnFusionState = true;
                if (m_Players[index].HP > 0)
                {
                    Player target = m_Players[m_FusionRequestPlayerNum];
                    m_Players[index].ExecuteFusion(target.transform);
                }
                else
                {
                    Player target = m_Players[index];
                    m_Players[m_FusionRequestPlayerNum].ExecuteFusion(target.transform);
                }

                m_IsFusing = true;

                if (OnFusionInit != null)
                    OnFusionInit();
            }
            return true;
        }
        else
            return false;
    }

    public void StopFeedbackFusion(bool stopFusion)
    {
        if (stopFusion)
            if (readyForFusion != null)
                readyForFusion.Stop(true);
    }

    public void CancelReadyForFusion(int playerNum)
    {
        if (playerNum - 1 == m_FusionRequestPlayerNum)
        {
            m_FusionRequestPlayerNum = -1;

            if (OnFusionCancel != null)
                OnFusionCancel();
        }
    }

    public void ReportFusionComplete()
    {
        if (m_FusionCharacter == null)
            m_FusionCharacter = Instantiate(m_FusionCharacterPrefab, m_Players[0].transform.parent);

        if (FxFusionEnergy == null)
        {
            FxFusionEnergy = Instantiate(m_Players[0].FXFusionEnergy, m_Players[0].transform);
            FxFusionHeal = Instantiate(m_Players[0].FXFusionHeal, m_Players[0].transform);
            FxFusionAOE = Instantiate(m_Players[0].FXFusionAOE, m_Players[0].transform);

            FxFusionEnergyP2 = Instantiate(m_Players[1].FXFusionEnergy, m_Players[1].transform);
            FxFusionHealP2 = Instantiate(m_Players[1].FXFusionHeal, m_Players[1].transform);
            FxFusionAOEP2 = Instantiate(m_Players[1].FXFusionAOE, m_Players[1].transform);
        }

        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/HitDeath_And_Fusion/Fusion_Sound", 3);

        m_FusionCharacter.transform.position = m_Players[0].transform.position;
        m_FusionCharacter.gameObject.SetActive(true);

        for (int i = 0; i < m_NbPlayers; i++)
        {
            //m_Players[i].gameObject.SetActive(false);
            m_Players[i].Hide();
        }

        if (OnFusionComplete != null)
            OnFusionComplete();

        StopFeedbackFusion(true);
    }

    void CancelFusion()
    {
        ReportFusionActionComplete(false);
    }

    public void ReportFusionActionComplete(bool success = true)
    {
        if (m_IsDefusing)
            return;

        m_IsDefusing = true;

        m_FusionRequestPlayerNum = -1;

        if (success)
            UseFusionScore(m_GameSettings.FusionConsumptionAmount);

        Vector3 direction = Vector3.right;
        for (int i = 0; i < m_NbPlayers; i++)
        {
            //m_Players[i].gameObject.SetActive(true);
            m_Players[i].Show();
            m_Players[i].Defuse(direction, success);
            direction *= -1f;
        }
        if (m_FusionCharacter)
            m_FusionCharacter.gameObject.SetActive(false);

        m_IsOnFusionState = false;
    }

    public void ReportDefuseComplete()
    {
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/HitDeath_And_Fusion/Separate_Sound", 3);

        m_IsFusing = false;
        m_IsDefusing = false;

        if (OnDefuseComplete != null)
            OnDefuseComplete();
    }

    public void OnPlayerDamage(Entity owner, Entity target, AttackStats attackStats)
    {
        int targetLayer = target.gameObject.layer;
        if (targetLayer == LayerMask.NameToLayer("PlayerHitBox") && !m_Invincible)
        {
            target.TakeDamage(attackStats);

            //CheckDeath();
                
        }
        else if (targetLayer == LayerMask.NameToLayer("EnemyHitBox") || targetLayer == LayerMask.NameToLayer("BrickHitbox"))
        {
            Player player = (Player)owner;

            if (targetLayer == LayerMask.NameToLayer("EnemyHitBox"))
                AddFusionScore(m_Vitae / m_GameSettings.FusionHitRatio);

            if (attackStats.type == AttackType.LIGHT)
                player.OnLightAttackHit(target);
            else
                player.OnHeavyAttackHit(target);
        }
        
    }

    public void OnBrickDamage(Entity owner, Vector3 brickPosition)
    {
        if (owner.tag == "Player")
        {
            Player player = (Player)owner;

            player.PlayImpact(brickPosition);
            player.ShortVibration();
        }
    }

    public void ReportPlayerDeath()
    {
        if (m_IsOnFusionState)
            ReportFusionActionComplete(false);

        CheckDeath();
    }

    void OnEnemyDeath(Entity killer, int vitae)
    {
        if (killer.tag == "Player")
        {
            //Player player = (Player)killer;
            OnCollectVitae(killer.gameObject, vitae);
            AddFusionScore(m_Vitae / m_GameSettings.FusionKillRatio);
        }
    }

    void CheckDeath()
    {
        bool dead = true;

        for (int i = 0; i < m_Players.Length; i++)
        {
            if (m_Players[i].HP != 0)
                dead = false;
        }

        if (OnAllDead != null && dead)
            OnAllDead();
    }

    void OnFusionHeal()
    {
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/Attack/Attack_F3", 3);

        for (int i = 0; i < m_NbPlayers; i++)
        {
            m_Players[i].RecoverAllHP();

            if (i == 0)
            {
                GameObject main = FxFusionHeal.gameObject;
                main.transform.position = m_Players[i].transform.position;
                fxManager.PlayFX(main.GetComponent<ParticleSystem>());
            }

            else if (i == 1)
            {
                GameObject main = FxFusionHealP2.gameObject;
                main.transform.position = m_Players[i].transform.position;
                fxManager.PlayFX(main.GetComponent<ParticleSystem>());
            }
        }
    }

    void OnFusionInvincible()
    {
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/Attack/Attack_F2", 3);
        m_Invincible = true;
        m_Players[0].GetInvincible();
        m_Players[1].GetInvincible();
        ServiceLocator.Instance.StartServiceCoroutine(InvincibilityCoutdown(10.0f));
    }

    void OnFusionInvincibleEnd()
    {
        m_Invincible = false;
        m_Players[0].StopInvincible();
        m_Players[1].StopInvincible();
    }

    IEnumerator InvincibilityCoutdown(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        OnFusionInvincibleEnd();
    }

    void OnFusionEnergy()
    {
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/Attack/Attack_F4", 3);
        for (int i = 0; i < m_NbPlayers; i++)
        {
            m_Players[i].RecoverAllEnergy();

            if (i == 0)
            {
                GameObject main = FxFusionEnergy.gameObject;
                main.transform.position = m_Players[i].transform.position;
                fxManager.PlayFX(main.GetComponent<ParticleSystem>());
            }

            else if (i == 1)
            {
                GameObject main = FxFusionEnergyP2.gameObject;
                main.transform.position = m_Players[i].transform.position;
                fxManager.PlayFX(main.GetComponent<ParticleSystem>());
            }
        }

    }

    void SummonAoE()
    {
        ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER).PlaySong("event:/SFX/Attack/Attack_F1", 3);
        m_BattleManager.SummonAoE(m_FusionCharacter.transform.position);
    }

    void OnCollectVitae(GameObject actor, int amount)
    {
        if (actor.tag == "Player")
        {
            //Player player = actor.GetComponent<Player>();
            //player.CollectVitae(amount);

            m_Vitae += amount;
            m_TotalVitae += amount;

            UpdateMaxFusionGauge();

            if (OnVitaeChange != null)
                OnVitaeChange(m_Vitae);
        }
    }

    public void SpendVitae(int amount, bool forcePay = false)
    {
        bool success = false;

        if (m_Vitae >= amount || forcePay)
        {
            m_Vitae -= amount;

            if (m_Vitae < 0)
                m_Vitae = 0;

            success = true;

            UpdateMaxFusionGauge();

            if (OnVitaeChange != null)
                OnVitaeChange(m_Vitae);
        }

        if (OnPayment != null)
            OnPayment(success);
    }

    IEnumerator FillFusionGauge()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            AddFusionScore(m_Vitae / m_GameSettings.FusionTimeRatio);
        }
    }

    public void UpdateHarmonyRate()
    {
        if (m_TotalVitae == 0)
        {
            m_HarmonyRate = 1f;
            return;
        }

        m_HarmonyRate = (float)m_Vitae / (float)m_TotalVitae;
    }

    void OnNewPowerUp(int id, int price, int numPlayer)
    {

        SpendVitae(price);
        m_Players[numPlayer - 1].UpgradePowerUp(id);
    }

    void OnShopClosed()
    {
        UpdateHarmonyRate();

        for (int i = 0; i < m_NbPlayers; i++)
            m_PowerUpsLevels[i] = m_Players[i].PowerLevels;
    }

    public void SentencePlayers()
    {
        for (int i = 0; i < m_NbPlayers; i++)
        {
            m_Players[i].DrainHP();
        }
    }

    private void OnDestroy()
    {
        Debug.Log("PlayerManager destroyed");
        m_BattleManager.OnDamage -= OnPlayerDamage;
        m_BattleManager.OnBrickDamage -= OnBrickDamage;
        m_EnemyManager.OnEnemyDeath -= OnEnemyDeath;

        m_GUIManager.OnFusionMenuInstantiation -= OnGUIInstantiation;

        m_ItemManager.OnCollectVitae -= OnCollectVitae;

        SceneManager.sceneLoaded -= ResetStats;

        m_GUIManager.Shop.SkillBought -= OnNewPowerUp;
        m_GUIManager.Shop.ShopClosed -= OnShopClosed;

        if (m_FusionMenu)
        {
            m_FusionMenu.HealSpellSelected -= OnFusionHeal;
            m_FusionMenu.InvincibleSpellSelected -= OnFusionInvincible;
            m_FusionMenu.ConfusionSpellSelected -= SummonAoE;
            m_FusionMenu.EnergySpellSelected -= OnFusionEnergy;
            m_FusionMenu.OnCancel -= CancelFusion;
        }
    }
}
