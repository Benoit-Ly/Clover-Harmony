using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {

    #region Events
    public event Action<int> OnHPChange;
    #endregion

    #region Vars
    protected Animator m_Animator;

    protected SoundManager m_SoundManager;

    [SerializeField]
    protected Transform m_Sprite = null;

    protected Vector3 m_MoveDirection = Vector3.zero;
    protected Vector3 m_SpriteRotation = Vector3.zero;
    protected Vector3 m_Rotation = Vector3.zero;

    protected int m_HP = 100;           public int HP { get { return m_HP; } }
    protected string m_CurrentAttack;   public string CurrentAttack { get { return m_CurrentAttack; } }

    int m_CurrentStunFrame = 0;
    protected bool m_IsStun = false;

    public bool IsLookingRight = true;

    protected float m_CurrentSpeed;
    protected float m_CurrentVerticalSpeed;

    int m_DamageCoeff = 1;           public int DamageCoeff { get { return m_DamageCoeff; } set { m_DamageCoeff = value; } }

    bool m_FreezeFrames = false;
    #endregion

    // Use this for initialization
    public virtual void Start () {
        m_Animator = GetComponent<Animator>();
        m_SoundManager = ServiceLocator.Instance.GetService<SoundManager>(ManagerType.SOUND_MANAGER);
	}
	
	// Update is called once per frame
	public virtual void Update () {
		
	}

    public virtual void FixedUpdate()
    {

    }

    public virtual void SetAttack(string name)
    {
        AttackStats attack = GetAttackStats(name);

        if (attack == null)
        {
            Debug.LogError("The attack \"" + name + "\" does not exist. Did you mispell it somewhere ?");
            return;
        }

        m_CurrentAttack = name;
    }

    public virtual void UnsetAttack()
    {
        
    }

    public virtual AttackStats GetCurrentAttack()
    {
        return null;
    }

    public virtual AttackStats GetAttackStats(string attackName)
    {
        return null;
    }

    public virtual void Move(float x, float y)
    {

    }

    public virtual void Attack()
    {

    }

    public virtual void TakeDamage(AttackStats attackStats)
    {
        m_HP -= attackStats.damage * m_DamageCoeff;

        if (m_HP < 0)
            m_HP = 0;

        m_CurrentStunFrame = attackStats.stunDuration;

        if (!m_IsStun)
        {
            m_IsStun = true;
            StartCoroutine(StunCountdown());
        }

        m_Animator.SetTrigger("DamageTrigger");
        QuickFreeze();
        RaiseHPChangeEvent();
    }

    // To remove
    public void TakeDamage(int dmg)
    {
        m_HP -= dmg;
    }

    IEnumerator StunCountdown()
    {
        while (m_CurrentStunFrame > 0)
        {
            yield return new WaitForEndOfFrame();
            m_CurrentStunFrame--;
        }

        m_IsStun = false;
    }

    protected void RaiseHPChangeEvent()
    {
        if (OnHPChange != null)
            OnHPChange(m_HP);
    }

    public void QuickFreeze()
    {
        if (m_Animator && !m_FreezeFrames)
        {
            m_FreezeFrames = true;
            StartCoroutine(FreezeAnimation(2));
        }
    }

    IEnumerator FreezeAnimation(int nbFrames)
    {
        float initialSpeed = m_Animator.speed;
        m_Animator.speed = 0;

        for (int i = 0; i < nbFrames; i++)
            yield return new WaitForEndOfFrame();

        m_Animator.speed = initialSpeed;
        m_FreezeFrames = false;
    }

    protected bool IsCurrentAnimatorState(string name)
    {
        return m_Animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }
}
