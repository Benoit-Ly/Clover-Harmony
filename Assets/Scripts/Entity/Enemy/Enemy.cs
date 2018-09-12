using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity, IDamageable, ICanBeSlowed
{
    #region Services
    EnemyManager m_EnemyManager;
    #endregion

    #region Events
    public event Action OnKnockback;
    public event Action OnDeath;
    #endregion

    [SerializeField]
    GamePhysics m_Physics;
    [SerializeField]
    EnemyTypeListData m_DataList = null;
    EnemyTypeData m_Data;
    [SerializeField]
    EnemyType m_Type;                               public EnemyType Type { get { return m_Type; } }
    [SerializeField]
    int m_HPModifier = 100;
    [SerializeField]
    string m_DamageModifier = "";
    [SerializeField]
    float m_DelayToDestroy = 1.5f;
    [SerializeField]
    ParticleSystem m_FXAttack1;
    ParticleSystem attack1;
    [SerializeField]
    ParticleSystem m_FXDeath;
    ParticleSystem FXDeath;

    CharacterController m_CharacterController;
    Renderer m_Renderer;
    public float AttackRate = 1f;
    public int AttackDammages = 10;
    public float PushBackForce = 2f;
    public float PushUpForce = 10f;

    LaunchDirection m_LaunchDirection;
    float m_HorizontalDir = 1f;

    float m_CurrentKnockTime = 0f;
    Vector3 m_KnockStartPosition;
    Vector3 m_PushTargetPosition;

    TriggerAbstract m_Spawner;

    bool m_IsMarked;                public bool IsMarked { get { return m_IsMarked; } }
    [SerializeField] GameObject m_MarkGO;
    AttackBox m_AttackBox;

    GameObject m_Target;            public GameObject Target { get { return m_Target; } set { m_Target = value; } }

    bool m_IsAlive = true;

    VisualFeedback visualFeedback;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Renderer = GetComponent<Renderer>();
        m_EnemyManager = ServiceLocator.Instance.GetService<EnemyManager>(ManagerType.ENEMY_MANAGER);

        m_HP = m_HPModifier;
    }

    public override void Start()
    {
        //StartCoroutine(EnemyAttack());

        base.Start();

        if (m_DataList)
            m_Data = m_DataList.GetDataFromType(m_Type);

        m_AttackBox = GetComponentInChildren<AttackBox>(true);
        m_CurrentSpeed = m_Data.Speed;

        m_EnemyManager.RegisterEnemy(this);

        if (attack1 == null)
            attack1 = Instantiate(m_FXAttack1);

        visualFeedback = GetComponent<VisualFeedback>();
    }

    public override void Update()
    {
        base.Update();
        m_Animator.SetInteger("HP", m_HP);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        UpdateKnockback();

        if (!m_IsStun)
            UpdateMovement();

        UpdateAnimation();
    }

    public void SetTrigger(TriggerAbstract trigger)
    {
        m_Spawner = trigger;
    }

    public void ReactiveTrigger()
    {
        m_Spawner.ActivateTrigger();
    }

    void UpdateMovement()
    {
        m_MoveDirection.y = Physics.gravity.y * Time.fixedDeltaTime;

        if (m_MoveDirection.x > 0f)
        {
            IsLookingRight = true;
            m_Rotation.y = 0f;
        }
        else if (m_MoveDirection.x < 0f)
        {
            IsLookingRight = false;
            m_Rotation.y = 180f;
        }

        if (m_CharacterController.enabled)
            m_CharacterController.Move(m_MoveDirection);

        if (m_IsAlive)
            transform.rotation = Quaternion.Euler(m_Rotation);
    }

    void UpdateAnimation()
    {
        //if (IsCurrentAnimatorState("EnemyAttackAnimation") && m_LaunchDirection == LaunchDirection.NO_LAUNCH)
        //{
        //    m_IsStun = false;
        //    m_Animator.ResetTrigger("DamageTrigger");
        //}

        m_Animator.SetBool("IsStun", m_IsStun);
        m_Animator.SetBool("IsLaunched", m_LaunchDirection != LaunchDirection.NO_LAUNCH);
        m_Animator.SetBool("IsGrounded", m_CharacterController.isGrounded);
    }

    void UpdateKnockback()
    {
        switch(m_LaunchDirection)
        {
            case LaunchDirection.KNOCKBACK:
                {
                    KnockMotion(m_Physics.Knockback);
                    break;
                }

            case LaunchDirection.KNOCKUP:
                {
                    KnockMotion(m_Physics.Knockup);
                    break;
                }

            case LaunchDirection.SPIKE:
                {
                    KnockMotion(m_Physics.Knockdown);
                    break;
                }

            case LaunchDirection.MOVE:
                {
                    ForceMove();
                    break;
                }

            default: break;
        }
    }

    void KnockMotion(GamePhysics.Knock knock)
    {
        Vector3 lastPosition = transform.localPosition;

        Vector3 newPosition = MathHelper.GetBallisticMotion(knock.Angle, knock.Force, -Physics.gravity.y, m_CurrentKnockTime, m_HorizontalDir);
        m_CurrentKnockTime += knock.Speed * Time.deltaTime;

        Vector3 direction = (m_KnockStartPosition + newPosition - lastPosition);
        m_CharacterController.Move(direction);

        if (m_CharacterController.isGrounded)
        {
            m_LaunchDirection = LaunchDirection.NO_LAUNCH;

            if (OnKnockback != null)
                OnKnockback();
        }
    }

    void ForceMove()
    {
        if (m_CharacterController.enabled)
        {
            Vector3 lerp = Vector3.Lerp(transform.position, m_PushTargetPosition, 10f * Time.fixedDeltaTime);
            Vector3 direction = lerp - transform.position;
            direction.y = Physics.gravity.y * Time.fixedDeltaTime;

            m_CharacterController.Move(direction);
        }

        if (!m_IsStun)
        {
            m_LaunchDirection = LaunchDirection.NO_LAUNCH;

            if (OnKnockback != null)
                OnKnockback();
        }
    }

    public override void Move(float x, float y)
    {
        if (m_IsStun || m_LaunchDirection != LaunchDirection.NO_LAUNCH)
        {
            m_Animator.SetBool("IsMoving", false);
            return;
        }

        base.Move(x, y);
        m_MoveDirection.x = x * m_CurrentSpeed * Time.fixedDeltaTime;
        m_MoveDirection.z = y * m_CurrentSpeed * Time.fixedDeltaTime;

        bool isMoving = (x <= -0.1f || x >= 0.1f || y <= -0.1f || y >= 0.1f);
        m_Animator.SetBool("IsMoving", isMoving);
    }

    public override void SetAttack(string name)
    {
        base.SetAttack(m_DamageModifier + name);

        m_AttackBox.Activate(true);
    }

    public override AttackStats GetAttackStats(string attackName)
    {
        if (m_Data != null)
            return m_Data.GetAttack(attackName);

        return null;
    }

    public override void UnsetAttack()
    {
        base.UnsetAttack();

        m_AttackBox.Activate(false);
    }

    public override AttackStats GetCurrentAttack()
    {
        return m_Data.GetAttack(m_CurrentAttack);
    }

    public override void Attack()
    {
        base.Attack();

        m_Animator.SetTrigger("AttackTrigger");

        if (m_CharacterController.isGrounded)
            StartCoroutine(PlayFxInRealTime());
    }

    public void PlayFXAttack()
    {
        if (attack1 != null)
        {
            GameObject main = attack1.GetComponent<ParticleSystem>().gameObject;

            if (IsLookingRight)
            {
                attack1.transform.position = transform.position;
                main.transform.rotation = new Quaternion(0, 0, -0.7071068f, 0.7071068f);
            }

            else if (!IsLookingRight)
            {
                attack1.transform.position = transform.position;
                main.transform.rotation = new Quaternion(-0.7071068f, 0.7071068f, 0, 0);
            }

            attack1.Play(true);
        }
    }

    IEnumerator PlayFxInRealTime()
    {
        yield return new WaitForSeconds(0.5f);

        PlayFXAttack();
    }

    public void DashAttack()
    {
        m_Animator.SetTrigger("Dash");
    }

    IEnumerator EnemyAttack()
    {
        while (true)
        {
            if (!m_IsStun)
            {
                m_Animator.SetTrigger("AttackTrigger");

                yield return new WaitForSeconds(AttackRate);
            }

            yield return null;
        }
    }

    IEnumerator TakeDammageFeedBack()
    {
        m_Renderer.material.color = Color.red;

        yield return new WaitForSeconds(1f);

        m_Renderer.material.color = Color.white;
    }

    public override void TakeDamage(AttackStats attackStats)
    {
        base.TakeDamage(attackStats);

        if (attackStats.launchDirection != LaunchDirection.NO_LAUNCH)
            UnsetAttack();

        SetKnockback(attackStats);
        StartCoroutine(TakeDammageFeedBack());

        StartCoroutine(visualFeedback.ApplyChange());

        if (m_HP == 0 && m_IsAlive)
        {
            m_IsAlive = false;
            StartCoroutine(DeathCoroutine(attackStats));

            if (OnDeath != null)
                OnDeath();
        }
    }

    void SetKnockback(AttackStats stats)
    {
        m_LaunchDirection = stats.launchDirection;
        m_HorizontalDir = stats.horizontalDir;

        m_CurrentKnockTime = 0f;
        m_KnockStartPosition = transform.localPosition;

        if (m_LaunchDirection == LaunchDirection.MOVE)
        {
            m_PushTargetPosition = transform.position;

            switch(stats.axis)
            {
                case AttackStats.Axis.X:
                    {
                        m_PushTargetPosition.x = stats.targetPosition;
                        break;
                    }

                case AttackStats.Axis.Z:
                    {
                        m_PushTargetPosition.z = stats.targetPosition;
                        break;
                    }

                default: break;
            }
        }
    }

    public void GetMarked()
    {
        m_MarkGO.SetActive(true);
        m_IsMarked = true;
    }

    public void ConsumeMark(Entity killer, AttackStats stats, bool clearMark)
    {
        if (!m_IsMarked)
            return;

        //AttackStats stats = new AttackStats();
        //stats.name = "";
        //stats.damage = 200;
        //stats.launchDirection = LaunchDirection.KNOCKUP;
        //stats.horizontalDir = 1f;
        //stats.stunDuration = 70;
        //stats.type = AttackType.HEAVY;
        //stats.damageProvider = killer;

        TakeDamage(stats);

        if (clearMark)
        {
            m_MarkGO.SetActive(false);
            m_IsMarked = false;
        }
    }

    public void TryToSlow()
    {

    }

    public void PlaySong_Attack()
    {
        m_SoundManager.PlaySong("event:/SFX/Attack/Attack_ES", 4, transform.position);
    }

    public void PlaySong_TakeDamage()
    {
        m_SoundManager.PlaySong("event:/SFX/HitDeath_And_Fusion/Take_Attack_ES", 4, transform.position);
    }

    public void PlaySong_Death()
    {
        m_SoundManager.PlaySong("event:/SFX/HitDeath_And_Fusion/Death_ES", 4, transform.position);
    }

    public void PlaySong_FootSteps()
    {
        m_SoundManager.PlaySong("event:/SFX/HitDeath_And_Fusion/Footsteps_ES", 4, transform.position);
    }

    public void FaceDirection(Vector3 direction)
    {
        if (direction.x > 0f)
        {
            IsLookingRight = true;
            m_Rotation.y = 0f;
        }
        else if (direction.x < 0f)
        {
            IsLookingRight = false;
            m_Rotation.y = 180f;
        }
    }

    public int GetAgressivity()
    {
        if (m_Data == null)
            return 0;

        return m_Data.Agressivity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.gameObject.layer == LayerMask.NameToLayer("PlayerHitBox"))
            m_Target = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.gameObject.layer == LayerMask.NameToLayer("PlayerHitBox"))
            m_Target = null;
    }

    IEnumerator DeathCoroutine(AttackStats attackStats)
    {
        while (!m_CharacterController.isGrounded || m_LaunchDirection != LaunchDirection.NO_LAUNCH)
            yield return null;

        m_CharacterController.enabled = false;

        m_EnemyManager.ReportEnemyDeath(this, attackStats.damageProvider, m_Data.VitaeDropQuantity);

        yield return new WaitForSeconds(m_DelayToDestroy);
        FXDeath = Instantiate(m_FXDeath);

        if (IsLookingRight)
        {
            FXDeath.transform.position = new Vector3(transform.position.x + 1.0f, transform.position.y - 1.0f, transform.position.z);
        }

        else if (!IsLookingRight)
        {
            FXDeath.transform.position = new Vector3(transform.position.x - 1.0f, transform.position.y - 1.0f, transform.position.z);
        }

        FXDeath.Play(true);
        Destroy(gameObject);
        Destroy(attack1);
    }

    IEnumerator DestructionCountdown()
    {
        yield return new WaitForSeconds(m_DelayToDestroy);
        Destroy(gameObject);
        Destroy(attack1);
    }
}
