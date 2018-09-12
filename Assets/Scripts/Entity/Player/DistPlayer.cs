using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistPlayer : Player {

    #region Vars
    List<Entity> m_MarkedTargets;
    List<ParticleSystem> m_ListFXHeavy;
    [SerializeField]
    ParticleSystem m_heavyAttack;
    [SerializeField]
    ParticleSystem m_FXFinal1;
    [SerializeField]
    ParticleSystem m_FXFinal2;
    [SerializeField]
    ParticleSystem m_FXFinal3;
    [SerializeField]
    ParticleSystem m_FXAirAttack;

    ParticleSystem heavyAttack;
    ParticleSystem FXFinal1;
    ParticleSystem FXFinal2;
    ParticleSystem FXFinal3;
    ParticleSystem FXAirAttack;
    #endregion

    // Use this for initialization
    public override void Start () {
        base.Start();

        m_MarkedTargets = new List<Entity>();
        m_ListFXHeavy = new List<ParticleSystem>();


        if (FXFinal1 == null)
            FXFinal1 = Instantiate(m_FXFinal1);
        if (FXFinal2 == null)
            FXFinal2 = Instantiate(m_FXFinal2);
        if (FXFinal3 == null)
            FXFinal3 = Instantiate(m_FXFinal3);
        if (FXAirAttack == null)
            FXAirAttack = Instantiate(m_FXAirAttack);
    }
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
	}

    public override void Attack()
    {
        base.Attack();

        if (!m_CharacterController.isGrounded)
            m_JumpSpeed = -m_Data.JumpSpeed * 2f;
    }

    public override void HeavyAttack()
    {
        if (!m_CharacterController.isGrounded)
            return;

        // Remove all marked enemies that got killed beforehand (for ex : killed by Clover)
        m_MarkedTargets.RemoveAll(IsMarkedEnemyDead);
        int nbMarks = m_MarkedTargets.Count;

        if (nbMarks == 0)
            return;

        if (m_Energy > 0)
        {
            float rate = GetMarkConsumptionRate();
            float nbMarksToRemove = nbMarks * rate;
            int roundedNbMarks;

            if (nbMarks % 2 == 0)
                roundedNbMarks = (int)nbMarksToRemove;
            else
                roundedNbMarks = Mathf.CeilToInt(nbMarksToRemove);

            for (int i = 0; i < m_MarkedTargets.Count; ++i)
            {
                if (m_ListFXHeavy.Count < m_MarkedTargets.Count)
                {
                    ParticleSystem heavyAttackFX = Instantiate(m_heavyAttack);
                    m_ListFXHeavy.Add(heavyAttackFX);
                }

                if (m_MarkedTargets[i] != null)
                {
                    GameObject main = m_ListFXHeavy[i].GetComponent<ParticleSystem>().gameObject;
                    main.transform.position = m_MarkedTargets[i].transform.position;
                    main.GetComponent<ParticleSystem>().Play(true);
                }
            }

            AttackStats stats = m_Data.GetAttack(m_HeavyAttackName);
            stats.damageProvider = this;

            for (int i = 0; i < nbMarks; i++)
            {
                bool clearMark = (i < roundedNbMarks);
                Enemy enemy = m_MarkedTargets[i] as Enemy;
                if (enemy)
                {
                    float damageRate = 1f;
                    if (m_PowerUpsLevels.PowerAttack > 0)
                        damageRate = (1f + m_PowerUpsData.PowerAttack.GetDistanceValue(m_PowerUpsLevels.PowerAttack) / 100f);

                    stats.damage *= (int)damageRate;

                    enemy.ConsumeMark(this, stats, clearMark);
                }
            }

            m_MarkedTargets.RemoveRange(0, roundedNbMarks);
            //m_MarkedTargets.Clear();

            base.HeavyAttack();



            UseEnergy();
        }
    }

    float GetMarkConsumptionRate()
    {
        float rate = 1f;

        if (m_PowerUpsLevels.PowerRetention > 0)
            rate = m_PowerUpsData.PowerRetention.GetDistanceValue(m_PowerUpsLevels.PowerRetention) / 100f;

        return rate;
    }

    public override void OnLightAttackHit(Entity target)
    {
        m_SoundManager.PlaySong("event:/SFX/HitDeath_And_Fusion/Attack_Impact_A", 4, target.transform.position);

        if (target.gameObject.layer == LayerMask.NameToLayer("EnemyHitBox"))
        {
            Enemy enemy = target as Enemy;

            if (enemy && !enemy.IsMarked)
            {
                enemy.GetMarked();
                m_MarkedTargets.Add(target);
            }
        }

        base.OnLightAttackHit(target);
    }

    public override void PlayImpact(Vector3 brickPosition)
    {
        base.PlayImpact(brickPosition);
        m_SoundManager.PlaySong("event:/SFX/HitDeath_And_Fusion/Attack_Impact_A", 4, brickPosition);
    }

    public override void OnHeavyAttackHit(Entity target)
    {
        m_SoundManager.PlaySong("event:/SFX/HitDeath_And_Fusion/Attack_Impact_A", 4, target.transform.position);

        base.OnHeavyAttackHit(target);
    }

    public override AttackStats GetCurrentAttack()
    {
        AttackStats stats = base.GetCurrentAttack();

        //if (m_PowerUpsLevels.CrowdControl > 0 && m_CurrentAttack == "LightAttack1")
        //    stats.launchDirection = LaunchDirection.KNOCKBACK;

        if (m_PowerUpsLevels.LightAttack > 0 && m_CurrentAttack == "LightAttack")
        {
            float rate = m_PowerUpsData.LightAttack.GetDistanceValue(m_PowerUpsLevels.LightAttack) / 100f;

            stats.damage *= (int)(1f + rate);
        }

        if (m_PowerUpsLevels.AirAttack > 0 && m_CurrentAttack == "AirAttack")
        {
            float rate = m_PowerUpsData.AirAttack.GetDistanceValue(m_PowerUpsLevels.AirAttack) / 100f;

            stats.damage *= (int)(1f + rate);
        }

        return stats;
    }

    public static bool IsMarkedEnemyDead(Entity target)
    {
        return (target == null || target.HP == 0);
    }

    public override void OnAttackStateEnter(AnimatorStateInfo stateInfo)
    {
        base.OnAttackStateEnter(stateInfo);

        if (stateInfo.IsName("LightAttack1"))
        {
            GameObject main = Attack1.GetComponent<ParticleSystem>().gameObject;

            if (IsLookingRight)
            {
                Attack1.transform.position = transform.position + new Vector3(2.5f, 0, 0);
                main.transform.rotation = new Quaternion(0, 0, -0.7071068f, 0.7071068f);
            }

            else if (!IsLookingRight)
            {
                Attack1.transform.position = transform.position + new Vector3(-2.5f, 0, 0);
                main.transform.rotation = new Quaternion(-0.7071068f, 0.7071068f, 0, 0);
            }

            Attack1.Play(true);
        }

        else if (stateInfo.IsName("LightAttack2"))
        {
            GameObject main = Attack2.GetComponent<ParticleSystem>().gameObject;

            if (IsLookingRight)
            {
                Attack2.transform.position = transform.position + new Vector3(2.5f, 0, 0);
                main.transform.rotation = new Quaternion(0, 0, -0.7071068f, 0.7071068f);
            }

            else if (!IsLookingRight)
            {
                Attack2.transform.position = transform.position + new Vector3(-2.5f, 0, 0);
                main.transform.rotation = new Quaternion(-0.7071068f, 0.7071068f, 0, 0);
            }

            Attack2.Play(true);
        }

        else if (stateInfo.IsName("LightAttack3"))
        {
            GameObject main = Attack3.GetComponent<ParticleSystem>().gameObject;

            if (IsLookingRight)
            {
                Attack3.transform.position = transform.position + new Vector3(2.5f, 0, 0); ;
                main.transform.rotation = new Quaternion(0, 0, -0.7071068f, 0.7071068f);
            }

            else if (!IsLookingRight)
            {
                Attack3.transform.position = transform.position + new Vector3(-2.5f, 0, 0); ;
                main.transform.rotation = new Quaternion(-0.7071068f, 0.7071068f, 0, 0);
            }

            Attack3.Play(true);
        }

        if (stateInfo.IsName("Final1"))
        {
            GameObject main = FXFinal1.GetComponent<ParticleSystem>().gameObject;

            if (IsLookingRight)
            {
                FXFinal1.transform.position = transform.position + new Vector3(2.5f, 0, 0); ;
                main.transform.rotation = new Quaternion(0, 0, -0.7071068f, 0.7071068f);
            }

            else if (!IsLookingRight)
            {
                FXFinal1.transform.position = transform.position + new Vector3(-2.5f, 0, 0); ;
                main.transform.rotation = new Quaternion(-0.7071068f, 0.7071068f, 0, 0);
            }

            FXFinal1.Play(true);
        }

        else if (stateInfo.IsName("Final2"))
        {
            GameObject main = FXFinal2.GetComponent<ParticleSystem>().gameObject;

            if (IsLookingRight)
            {
                FXFinal2.transform.position = transform.position + new Vector3(2.5f, 0, 0); ;
                main.transform.rotation = new Quaternion(-0.4984911f, 0.5015043f, -0.5015043f, 0.4984911f);
            }

            else if (!IsLookingRight)
            {
                FXFinal2.transform.position = transform.position + new Vector3(-2.5f, 0, 0); ;
                main.transform.rotation = new Quaternion(-0.4984911f, -0.5015043f, 0.5015043f, 0.4984911f);
            }

            FXFinal2.Play(true);
        }

        else if (stateInfo.IsName("Final3"))
        {
            GameObject main = FXFinal3.GetComponent<ParticleSystem>().gameObject;

            if (IsLookingRight)
            {
                FXFinal3.transform.position = transform.position + new Vector3(2.5f, 0, 0); ;
                main.transform.rotation = new Quaternion(0, 0, -0.7071068f, 0.7071068f);
            }

            else if (!IsLookingRight)
            {
                FXFinal3.transform.position = transform.position + new Vector3(-2.5f, 0, 0); ;
                main.transform.rotation = new Quaternion(-0.7071068f, 0.7071068f, 0, 0);
            }

            FXFinal3.Play(true);
        }

        if (stateInfo.IsName("AirAttack"))
        {
            GameObject main = FXAirAttack.GetComponent<ParticleSystem>().gameObject;

            if (IsLookingRight)
            {
                FXAirAttack.transform.position = transform.position + new Vector3(0.0f, 0, 0); ;
                main.transform.rotation = new Quaternion(0, 0, -0.7071068f, 0.7071068f);
            }

            else if (!IsLookingRight)
            {
                FXAirAttack.transform.position = transform.position + new Vector3(0.0f, 0, 0); ;
                main.transform.rotation = new Quaternion(-0.7071068f, 0.7071068f, 0, 0);
            }

            FXAirAttack.Play(true);
        }
    }
}
