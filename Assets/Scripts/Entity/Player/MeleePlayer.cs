using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePlayer : Player {

    #region Events
    public event Action<int> OnComboChange;
    #endregion

    #region Vars

    bool m_AirAttackAvailable = true;

    int m_ComboCount = 0;
    int m_MaxCombo = 30;
    float[] m_DamagePowerUp = { 1f, 1.5f, 2f, 2.5f };

    bool m_EnergyUsage = false;

    [SerializeField]
    ParticleSystem m_heavyAttack;
    [SerializeField]
    ParticleSystem m_FXAirAttack;

    ParticleSystem heavyAttackFX;
    ParticleSystem FXAirAttack;
    #endregion

    // Use this for initialization
    public override void Start() {
        base.Start();

        if (heavyAttackFX == null)
            heavyAttackFX = Instantiate(m_heavyAttack);
        if (FXAirAttack == null)
            FXAirAttack = Instantiate(m_FXAirAttack);
    }

    // Update is called once per frame
    public override void Update() {
        base.Update();

        if (m_CharacterController.isGrounded)
            m_AirAttackAvailable = true;
    }

    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();

        m_Animator.SetBool("AerialAvailable", m_AirAttackAvailable);
    }

    public override void Attack()
    {
        base.Attack();

        if (!m_CharacterController.isGrounded)
        {
            m_AirAttackAvailable = false;
            m_JumpSpeed = -m_Data.JumpSpeed * 2f;
        }

    }

    public override void HeavyAttack()
    {
        if (m_Energy > 0)
        {
            m_EnergyUsage = true;
            AttackStats heavyAttack = m_Data.GetAttack(m_HeavyAttackName);

            if (heavyAttack != null)
            {
                int index = m_ComboCount / 10;

                float damageRate = 1f;

                if (m_PowerUpsLevels.PowerAttack > 0)
                    damageRate = (1f + m_PowerUpsData.PowerAttack.GetMeleeValue(m_PowerUpsLevels.PowerAttack) / 100f);

                float damage = heavyAttack.damage * damageRate * m_DamagePowerUp[index];
                heavyAttack.damage = (int)damage;
            }

            if (heavyAttackFX != null)
            {
                GameObject main = heavyAttackFX.GetComponent<ParticleSystem>().gameObject;

                if (IsLookingRight)
                {
                    heavyAttackFX.transform.position = transform.position + new Vector3(3.0f, 0, 0);
                    main.transform.rotation = new Quaternion(-0.7071068f, 0, 0, 0.7071068f);
                }

                else if (!IsLookingRight)
                {
                    heavyAttackFX.transform.position = transform.position + new Vector3(-3.0f, 0, 0);
                    main.transform.rotation = new Quaternion(-0.7071068f, 0, 0, 0.7071068f);
                }

                if (m_CharacterController.isGrounded)
                    heavyAttackFX.Play(true);
            }
        }

        base.HeavyAttack();
    }

    public override void OnLightAttackHit(Entity target)
    {
        base.OnLightAttackHit(target);

        m_SoundManager.PlaySong("event:/SFX/HitDeath_And_Fusion/Attack_Impact_C", 4, target.transform.position);

        if (target.gameObject.layer == LayerMask.NameToLayer("EnemyHitBox") && m_ComboCount < m_MaxCombo)
        {
            m_ComboCount++;

            if (OnComboChange != null)
                OnComboChange(m_ComboCount);
        }
    }

    public override void PlayImpact(Vector3 brickPosition)
    {
        m_SoundManager.PlaySong("event:/SFX/HitDeath_And_Fusion/Attack_Impact_C", 4, brickPosition);
        base.PlayImpact(brickPosition);
    }

    public override void OnHeavyAttackHit(Entity target)
    {
        base.OnHeavyAttackHit(target);

        m_SoundManager.PlaySong("event:/SFX/HitDeath_And_Fusion/Attack_Impact_C", 4, target.transform.position);

        if (m_EnergyUsage)
        {
            UseEnergy();
            m_EnergyUsage = false;
        }

        UseComboCounter();
    }

    void UseComboCounter()
    {
        int amount = m_MaxCombo;

        if (m_PowerUpsLevels.PowerRetention > 0)
            amount = (int)m_PowerUpsData.PowerRetention.GetMeleeValue(m_PowerUpsLevels.PowerRetention) * 10;

        m_ComboCount -= amount;

        if (m_ComboCount < 0)
            m_ComboCount = 0;

        if (OnComboChange != null)
            OnComboChange(m_ComboCount);
    }

    public override AttackStats GetCurrentAttack()
    {
        AttackStats stats = base.GetCurrentAttack();

        if (m_PowerUpsLevels.CrowdControl > 0 && m_CurrentAttack == "LightAttack1")
        {
            stats.launchDirection = LaunchDirection.MOVE;
            stats.axis = AttackStats.Axis.Z;
        }

        if (m_PowerUpsLevels.LightAttack > 0 && (m_CurrentAttack == "LightAttack1" || m_CurrentAttack == "LightAttack2" || m_CurrentAttack == "LightAttack3"))
        {
            float rate = m_PowerUpsData.LightAttack.GetMeleeValue(m_PowerUpsLevels.LightAttack) / 100f;

            stats.damage *= (int)(1f + rate);
        }

        if (m_PowerUpsLevels.AirAttack > 0 && (m_CurrentAttack == "AirAttack" || m_CurrentAttack == "AirKnockdown"))
        {
            float rate = m_PowerUpsData.AirAttack.GetMeleeValue(m_PowerUpsLevels.LightAttack) / 100f;

            stats.damage *= (int)(1f + rate);
        }

        return stats;
    }

    public override void TakeDamage(AttackStats attackStats)
    {
        base.TakeDamage(attackStats);
        m_ComboCount = 0;

        if (OnComboChange != null)
            OnComboChange(m_ComboCount);
    }

    public override void OnAttackStateEnter(AnimatorStateInfo stateInfo)
    {
        base.OnAttackStateEnter(stateInfo);

        if (stateInfo.IsName("LightAttack1"))
        {
            GameObject main = Attack1.GetComponent<ParticleSystem>().gameObject;

            if (IsLookingRight)
            {
                Attack1.transform.position = transform.position;
                main.transform.rotation = new Quaternion(0, 1, 0, 0);
            }

            else if (!IsLookingRight)
            {
                Attack1.transform.position = transform.position;
                main.transform.rotation = new Quaternion(0, 0, 0, 1);
            }

            Attack1.Play(true);
        }

        else if (stateInfo.IsName("LightAttack2"))
        {
            GameObject main = Attack2.GetComponent<ParticleSystem>().gameObject;

            if (IsLookingRight)
            {
                Attack2.transform.position = transform.position + new Vector3(3.0f, 0, 0);
                main.transform.rotation = new Quaternion(0, 0, -0.7071068f, 0.7071068f);
            }

            else if (!IsLookingRight)
            {
                Attack2.transform.position = transform.position + new Vector3(-3.0f, 0, 0);
                main.transform.rotation = new Quaternion(-0.7071068f, 0.7071068f, 0, 0);
            }

            Attack2.Play(true);
        }

        else if (stateInfo.IsName("LightAttack3"))
        {
            GameObject main = Attack3.GetComponent<ParticleSystem>().gameObject;

            if (IsLookingRight)
            {
                Attack3.transform.position = transform.position;
                main.transform.rotation = new Quaternion(0, 0, -0.7071068f, 0.7071068f);
            }

            else if (!IsLookingRight)
            {
                Attack3.transform.position = transform.position;
                main.transform.rotation = new Quaternion(-0.7071068f, 0.7071068f, 0, 0);
            }

            Attack3.Play(true);
        }

        if ( (stateInfo.IsName("AirAttack1")) || (stateInfo.IsName("AirAttack2")) || (stateInfo.IsName("AirAttack3")) || (stateInfo.IsName("AirAttack4")) || (stateInfo.IsName("AirAttack5")) )
        {
            GameObject main = FXAirAttack.GetComponent<ParticleSystem>().gameObject;

            if (IsLookingRight)
            {
                FXAirAttack.transform.position = transform.position;
                main.transform.rotation = new Quaternion(0, 1, 0, 0);
            }

            else if (!IsLookingRight)
            {
                FXAirAttack.transform.position = transform.position;
                main.transform.rotation = new Quaternion(0, 0, 0, 1);
            }

            FXAirAttack.Play(true);
        }

        else if (stateInfo.IsName("AirAttack6"))
        {
            GameObject mainHeavy = heavyAttackFX.GetComponent<ParticleSystem>().gameObject;

            if (IsLookingRight)
            {
                heavyAttackFX.transform.position = transform.position + new Vector3(3.0f, 0, 0);
                mainHeavy.transform.rotation = new Quaternion(0.7071068f, 0, 0, 0.7071068f);
            }

            else if (!IsLookingRight)
            {
                heavyAttackFX.transform.position = transform.position + new Vector3(-3.0f, 0, 0);
                mainHeavy.transform.rotation = new Quaternion(0.7071068f, 0, 0, 0.7071068f);
            }

            heavyAttackFX.Play(true);
        }
    }
}
