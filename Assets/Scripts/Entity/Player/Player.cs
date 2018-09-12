using System;
using System.Collections;
using System.Collections.Generic;
using UnityChan;
using UnityEngine;
using XInputDotNetPure;

public abstract class Player : Entity, IDamageable, ICanBeSlowed {

    #region Services
    protected PlayerManager m_PlayerManager;
    #endregion

    #region Components
    protected CharacterController m_CharacterController;

    protected PlayerController m_PlayerController;
    
    [SerializeField]
    protected AttackBox m_AttackBox;
    #endregion

    #region Events
    public event Action<int> OnEnergyChange;
    public event Action OnTakeDamage;
    public event Action<int> OnMaxEnergyChange;
    public event Action<int> OnMaxHPChange;
    //public event Action<int> OnVitaeChange;
    #endregion

    #region Coroutines
    Coroutine m_DeathCoroutine = null;
    #endregion

    #region Vars
    int m_ID = 0;       public int ID { get { return m_ID; } set { m_ID = value; } }
    [SerializeField]
    protected PlayerData m_Data;
    protected PowerUpsLevels m_PowerUpsLevels;                  public PowerUpsLevels PowerLevels { get { return m_PowerUpsLevels; } set { m_PowerUpsLevels = value; } }
    [SerializeField]
    protected PowerUpsList m_PowerUpsData;

    protected int m_Energy = 0;

    int m_MaxHP;                                                public int MaxHP { get { return m_MaxHP; } }
    int m_MaxEnergy;                                            public int MaxEnergy { get { return m_MaxEnergy; } }

    [SerializeField]
    protected string m_HeavyAttackName;

    protected Vector3 m_FusionPosition = Vector3.zero;
    protected Vector3 m_DefusePosition = Vector3.zero;
    protected float m_JumpSpeed = 0f;
    float m_CurrentJumpHeight = 0f;
    float m_CurrentFloatTime = 0f;
    bool m_IsJumping = true;

    protected bool m_IsFusing = false;
    protected bool m_IsDefusing = false;
    protected bool m_ChainReady = true;

    [SerializeField] float VibrationDuration = 0.1f;

    float m_FusionMoveTime = 0f;

    int m_NbInvincibilityFrames = 60;
    bool m_Invincible = false;

    int m_KillCount = 0;

    [SerializeField]
    bool m_InvertSprite = false;

    [SerializeField]
    GameObject m_shiftingFX;
    [SerializeField] GameObject ReadyForFusionFX;
    GameObject m_readyForFusionFX;

    [SerializeField] GameObject InvincibilityFX;
    GameObject m_InvincibilityFX;

    [SerializeField]
    ParticleSystem m_FXFusionHeal; public ParticleSystem FXFusionHeal { get { return m_FXFusionHeal; } }
    [SerializeField]
    ParticleSystem m_FXFusionEnergy; public ParticleSystem FXFusionEnergy { get { return m_FXFusionEnergy; } }
    [SerializeField]
    ParticleSystem m_FXFusionAOE; public ParticleSystem FXFusionAOE { get { return m_FXFusionAOE; } }
    [SerializeField]
    protected ParticleSystem m_FXAttack1;
    [SerializeField]
    protected ParticleSystem m_FXAttack2;
    [SerializeField]
    protected ParticleSystem m_FXAttack3;

    ParticleSystem attack1; public ParticleSystem Attack1 { get { return attack1; } }
    ParticleSystem attack2; public ParticleSystem Attack2 { get { return attack2; } }
    ParticleSystem attack3; public ParticleSystem Attack3 { get { return attack3; } }

    VisualFeedback visualFeedback;

    //int m_Vitae = 0;        public int Vitae { get { return m_Vitae; } }
    #endregion

    // Use this for initialization
    public override void Start () {
        base.Start();

        m_PlayerManager = ServiceLocator.Instance.GetService<PlayerManager>(ManagerType.PLAYER_MANAGER);
        m_CharacterController = GetComponent<CharacterController>();
        m_PlayerController = GetComponent<PlayerController>();
        //m_Sprite = transform.Find("Sprite");

        visualFeedback = GetComponent<VisualFeedback>();

        // Init stats
        m_HP = m_Data.HP;
        m_Animator.SetInteger("HP", m_HP);

        UpdateMaxHP();
        UpdateMaxEnergy();

        m_Energy = m_MaxEnergy;

        m_JumpSpeed = Physics.gravity.y;
        m_CurrentJumpHeight = m_Data.JumpHeight / 10f;

        m_CurrentSpeed = m_Data.Speed;
        m_CurrentVerticalSpeed = m_Data.VerticalSpeed;

        m_PlayerManager.OnFusionComplete += ReportFusionComplete;

        m_readyForFusionFX = Instantiate(ReadyForFusionFX, transform);
        m_readyForFusionFX.SetActive(false);

        m_InvincibilityFX = Instantiate(InvincibilityFX, transform);
        m_InvincibilityFX.SetActive(false);

        if (m_InvertSprite)
            m_SpriteRotation.y = 180f;

        if (attack1 == null)
            attack1 = Instantiate(m_FXAttack1);
        if (attack2 == null)
            attack2 = Instantiate(m_FXAttack2);
        if (attack3 == null)
            attack3 = Instantiate(m_FXAttack3);
    }

    public void SetPowers(PowerUpsLevels newPowers)
    {
        PowerUpsLevels initialLevels = m_PowerUpsLevels;
        m_PowerUpsLevels = newPowers;

        ApplyPowers(initialLevels);
    }

    void ApplyPowers(PowerUpsLevels initialLevels)
    {
        if (m_PowerUpsLevels.Resilience > initialLevels.Resilience)
            UpdateMaxHP();

        if (m_PowerUpsLevels.PowerCharges > initialLevels.PowerCharges)
            UpdateMaxEnergy();
    }
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
	}

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!m_IsStun && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Stun"))
        {
            UpdateFusion();
        }
        else
            m_MoveDirection = Vector3.zero;

        if (!m_Animator.GetBool("IsAttacking"))
            UpdateMovement();

        UpdateAnimation();

        // Fix speed to normal after debuff
        m_CurrentSpeed = m_Data.Speed;
        m_CurrentVerticalSpeed = m_Data.VerticalSpeed;
    }

    void UpdateMaxEnergy()
    {
        m_MaxEnergy = m_Data.Energy;

        if (m_PowerUpsLevels.PowerCharges > 0)
            m_MaxEnergy += (int)m_PowerUpsData.PowerCharges.GetMeleeValue(m_PowerUpsLevels.PowerCharges);

        if (OnMaxEnergyChange != null)
            OnMaxEnergyChange(m_MaxEnergy);

        RecoverAllEnergy();
    }

    void UpdateMaxHP()
    {
        m_MaxHP = m_Data.HP;

        if (m_PowerUpsLevels.Resilience > 0)
        {
            float rate = m_PowerUpsData.Resilience.GetMeleeValue(m_PowerUpsLevels.Resilience) / 100f;

            m_MaxHP += (int)(1f + rate);
        }

        if (OnMaxHPChange != null)
            OnMaxHPChange(m_MaxHP);
    }

    public void RecoverEnergy(int amount = 1)
    {
        if (m_Energy < m_MaxEnergy)
        {
            m_Energy++;

            if (OnEnergyChange != null)
                OnEnergyChange(m_Energy);
        }
    }

    public void RecoverAllEnergy()
    {
        m_Energy = m_MaxEnergy;

        if (OnEnergyChange != null)
            OnEnergyChange(m_Energy);
    }

    void UpdateMovement()
    {
        if (m_Sprite)
            m_Sprite.rotation = Quaternion.Euler(m_SpriteRotation);
        m_AttackBox.transform.rotation = Quaternion.Euler(m_Rotation);

        if (m_CurrentJumpHeight < m_Data.JumpHeight / 10f)
        {
            m_CurrentFloatTime = 0f;
            m_CurrentJumpHeight += m_JumpSpeed * Time.fixedDeltaTime;
        }
        else if (!m_CharacterController.isGrounded && m_JumpSpeed > -m_Data.JumpSpeed)
        {
            m_JumpSpeed = Mathf.Lerp(m_Data.JumpSpeed, -m_Data.JumpSpeed, m_CurrentFloatTime);
            m_CurrentFloatTime += 2f * m_Data.JumpSpeed / 10f * Time.fixedDeltaTime;

            if (m_JumpSpeed + m_Data.JumpSpeed < 0.1f)
                m_JumpSpeed = -m_Data.JumpSpeed;
        }

        m_MoveDirection.y = m_JumpSpeed * Time.fixedDeltaTime;

        m_CharacterController.Move(m_MoveDirection);
    }

    void UpdateFusion()
    {
        if (m_IsFusing)
        {
            m_FusionMoveTime += Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(transform.position, m_FusionPosition, 30f * Time.fixedDeltaTime);

            if (Vector3.Distance(transform.position, m_FusionPosition) < 0.1f || m_FusionMoveTime >= 0.5f)
            {
                transform.position = m_FusionPosition;
                m_IsFusing = false;
                m_FusionMoveTime = 0f;

                m_FusionPosition = Vector3.zero;

                m_PlayerManager.ReportFusionComplete();
            }
        }
        else if (m_IsDefusing)
        {
            m_FusionMoveTime += Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(transform.position, m_DefusePosition, 30f * Time.fixedDeltaTime);

            if (Vector3.Distance(transform.position, m_DefusePosition) < 0.1f || m_FusionMoveTime >= 0.5f)
            {
                transform.position = m_DefusePosition;
                m_IsDefusing = false;

                m_DefusePosition = Vector3.zero;

                m_PlayerManager.ReportDefuseComplete();
            }
        }
    }

    protected virtual void UpdateAnimation()
    {
        // Jump/Fall
        m_Animator.SetBool("IsGrounded", m_CharacterController.isGrounded);
        m_Animator.SetFloat("yVelocity", m_MoveDirection.y);

        // Stun
        m_Animator.SetBool("IsStun", m_IsStun);
    }

    public void GetInvincible()
    {
        m_InvincibilityFX.SetActive(true);
    }

    public void StopInvincible()
    {
        m_InvincibilityFX.SetActive(false);
    }

    public void CancelReadyForFusion(int playerNumber)
    {
        m_readyForFusionFX.SetActive(false);
        m_PlayerManager.CancelReadyForFusion(playerNumber);
    }

    public void ReportReadyForFusion(int playernumber)
    {
        if (m_PlayerManager.ReportReadyForFusion(playernumber))
            m_readyForFusionFX.SetActive(true);
    }

    public void ReportFusionComplete()
    {
        m_readyForFusionFX.SetActive(false);
    }

    protected void BreakJump()
    {
        // Act like you already reached your jump peak
        m_CurrentJumpHeight = m_Data.JumpHeight / 10f;
        m_CurrentFloatTime = 1f;

        if (!m_CharacterController.isGrounded)
            m_JumpSpeed = 0f;
        else
            m_JumpSpeed = -m_Data.JumpSpeed;
    }

    public override void SetAttack(string name)
    {
        base.SetAttack(name);

        m_AttackBox.Activate(true);
    }

    public override AttackStats GetCurrentAttack()
    {
        if (string.IsNullOrEmpty(m_CurrentAttack))
            return null;

        return m_Data.GetAttack(m_CurrentAttack);
    }

    public override AttackStats GetAttackStats(string attackName)
    {
        if (m_Data != null)
            return m_Data.GetAttack(attackName);

        return null;
    }

    public override void Move(float x, float y)
    {
        base.Move(x, y);

        if (!m_CharacterController.isGrounded)
        {
            m_Animator.ResetTrigger("JumpTrigger");
            m_MoveDirection.z = 0f;
            return;
        }

        bool isMoving = (x <= -0.1f || x >= 0.1f || y <= -0.1f || y >= 0.1f);
        m_Animator.SetBool("IsMoving", isMoving);

        if (x < 0f)
        {
            x = -1f;
            FlipSprite(true);
            m_Rotation.y = 180f;
        }
        else if (x > 0f)
        {
            x = 1f;
            FlipSprite(false);
            m_Rotation.y = 0f;
        }

        if (y < 0f)
            y = -1f;
        else if (y > 0f)
            y = 1f;

        Vector2 norm = new Vector2(x, y);
        norm.Normalize();

        m_MoveDirection.x = norm.x * m_CurrentSpeed * Time.fixedDeltaTime;
        m_MoveDirection.z = norm.y * m_CurrentVerticalSpeed * Time.fixedDeltaTime;
    }

    public override void Attack()
    {
        base.Attack();

        if (m_ChainReady)
        {
            BreakJump();
            m_Animator.SetTrigger("LightAttack");
            m_ChainReady = false;
        }
    }

    public virtual void HeavyAttack()
    {
        if (m_Energy > 0)
        {
            m_Animator.SetTrigger("HeavyAttack");
        }
    }

    public void RecoverHP(int value)
    {
        m_HP += value;

        if (m_HP > m_Data.HP)
            m_HP = m_Data.HP;

        if (m_DeathCoroutine != null)
        {
            StopCoroutine(m_DeathCoroutine);
            m_DeathCoroutine = null;
        }

        if (!m_CharacterController.enabled)
            m_CharacterController.enabled = true;

        m_Animator.SetInteger("HP", m_HP);

        RaiseHPChangeEvent();
    }

    public void RecoverAllHP()
    {
        RecoverHP(m_Data.HP);
    }

    public virtual void OnLightAttackHit(Entity target)
    {
        StartCoroutine(Vibrate());
    }

    public virtual void PlayImpact(Vector3 brickPosition)
    {

    }

    public void ShortVibration()
    {
        StartCoroutine(Vibrate());
    }

    private IEnumerator Vibrate()
    {
        PlayerIndex testPlayerIndex = (PlayerIndex)m_PlayerController.PlayerNumber - 1;
        //GamePad.SetVibration(testPlayerIndex, 2, 1);

        yield return new WaitForSeconds(VibrationDuration);

        //GamePad.SetVibration(testPlayerIndex, 0, 0);
    }

    public virtual void OnHeavyAttackHit(Entity target)
    {
        //m_Energy--;

        //if (OnEnergyChange != null)
        //    OnEnergyChange(m_Energy);
    }

    public void EnableChain()
    {
        m_AttackBox.Activate(false);
        m_ChainReady = true;
    }

    public void Jump()
    {
        if (m_ChainReady && m_CharacterController.isGrounded)
        {
            m_JumpSpeed = m_Data.JumpSpeed;
            m_CurrentJumpHeight = 0f;
            m_Animator.SetTrigger("JumpTrigger");
        }
    }

    public void GetReadyForFusion()
    {

    }

    public void ExecuteFusion(Transform target)
    {
        m_IsFusing = true;
        m_FusionPosition = target.position;
    }

    public void Defuse(Vector3 direction, bool success)
    {
        if (success && m_HP == 0)
            RecoverHP(500);

        int layerMask = 1 << (LayerMask.NameToLayer("Default") | LayerMask.NameToLayer("BrickHitbox"));

        if (!Physics.Raycast(transform.position, direction, 1f, layerMask))
            m_DefusePosition = transform.position + direction;
        else
            m_DefusePosition = transform.position;

        m_IsDefusing = true;
    }

    public void TryToSlow()
    {
        m_CurrentSpeed = m_Data.DebuffSpeed;
        m_CurrentVerticalSpeed = m_Data.DebuffVerticalSpeed;
    }

    //public void CollectVitae(int amount)
    //{
    //    m_Vitae += amount;

    //    if (OnVitaeChange != null)
    //        OnVitaeChange(m_Vitae);
    //}

    public override void TakeDamage(AttackStats attackStats)
    {
        if (!m_Invincible)
        {
            base.TakeDamage(attackStats);

            if (OnTakeDamage != null)
                OnTakeDamage();

            m_Animator.SetInteger("HP", m_HP);
            m_Animator.SetBool("IsStun", true);

            StartCoroutine(visualFeedback.ApplyChange());

            if (m_HP > 0)
            {
                m_Invincible = true;
                StartCoroutine(InvincibilityFrames());
            }
            else if (m_DeathCoroutine == null)
            {
                m_PlayerManager.ReportPlayerDeath();
                m_DeathCoroutine = StartCoroutine(DeathCoroutine());
            }
        }
    }

    protected void UseEnergy(int amount = 1)
    {
        if (m_Energy >= amount)
            m_Energy -= amount;

        if (OnEnergyChange != null)
            OnEnergyChange(m_Energy);
    }

    void FlipSprite(bool flip)
    {
        if (!flip)
        {
            IsLookingRight = true;
            m_SpriteRotation.y = 0f;
        }
        else
        {
            IsLookingRight = false;
            m_SpriteRotation.y = 180f;
        }

        if (m_InvertSprite)
            m_SpriteRotation.y -= 180f;
    }

    public void Hide()
    {
        m_Sprite.gameObject.SetActive(false);
    }

    public void Show()
    {
        m_Sprite.gameObject.SetActive(true);
    }

    IEnumerator InvincibilityFrames()
    {
        int currentFrame = 0;

        while (currentFrame < m_NbInvincibilityFrames)
        {
            yield return new WaitForEndOfFrame();
            currentFrame++;
        }

        m_Invincible = false;
    }

    public void PlayShiftingFX()
    {
        m_shiftingFX.GetComponent<ParticleSystem>().Play();
    }

    public void ActivePhysiqueParam()
    {
        GetComponent<SpringManager>().enabled = true;
    }

    public void DesactivePhysiqueParam()
    {
        GetComponent<SpringManager>().enabled = false;
    }

    public void AddKill()
    {
        m_KillCount++;

        if ((m_KillCount % 5 == 0) && m_PowerUpsLevels.PowerAbsorption > 0)
            RecoverEnergy();
    }

    public void UpgradePowerUp(int id)
    {
        PowerUpsLevels initialLevels = m_PowerUpsLevels;

        m_PowerUpsData.UpgradePowers(ref m_PowerUpsLevels, id);
        ApplyPowers(initialLevels);
    }

    public void DrainHP()
    {
        StartCoroutine(HPDrainCoroutine());
    }

    IEnumerator HPDrainCoroutine()
    {
        while (m_HP > 0)
        {
            m_HP -= 10;

            if (m_HP < 0)
                m_HP = 0;

            m_Animator.SetInteger("HP", m_HP);
            RaiseHPChangeEvent();
            yield return new WaitForEndOfFrame();
        }

        if (m_DeathCoroutine == null)
        {
            m_PlayerManager.ReportPlayerDeath();
            m_DeathCoroutine = StartCoroutine(DeathCoroutine());
        }

        yield return null;
    }

    IEnumerator DeathCoroutine()
    {
        while (!m_CharacterController.isGrounded)
            yield return null;

        m_CharacterController.enabled = false;
        m_DeathCoroutine = null;
    }

    public virtual void OnAttackStateEnter(AnimatorStateInfo stateInfo)
    {
       
    }

    private void OnDestroy()
    {
        m_PlayerManager.OnFusionComplete -= ReportFusionComplete;
    }
}
