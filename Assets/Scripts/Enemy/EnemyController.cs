using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]//为避免缺少组件，添加该脚本会自动添加此组件
public class EnemyController : LivingEntity
{
    NavMeshAgent navMeshAgent;//导航网格
    public enum EnemyState { Idle, Chasing, Attacking };//枚举，敌人状态
    public EnemyState currentState;//敌人当前状态
    private Transform target;//目标：player
    [SerializeField] private float updateRate;//更新延迟

    #region Attack
    [SerializeField]float attackDistanceThreshold = .5f;//攻击距离
    float timeBetweenAttacks = 1.0f;//攻击时间间隔
    float nextAttackTime = 2.0f;//下一次攻击时间
    public float damage = 1.0f;//敌人攻击伤害
    #endregion
    
    bool hasTarget;//玩家是否存在

    #region Color
    Material skinMaterial;//皮肤材质
    Color originalColor;//初始颜色
    #endregion
    
    #region EnemyDeath
    public ParticleSystem deathEffect;//粒子系统
    public static event Action onDeathStatic;//事件
    #endregion
    
    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();//开始就获取，navMeshAgent
        if (GameObject.FindGameObjectWithTag("Player") != null)//因为之后敌人生成时，很可能Player已经阵亡了，首先要判断限制
        {
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;//target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
    }
    
    protected override void Start()
    {
        base.Start();//LivingEntity类中的Start方法
        
        base.Start();

        if (hasTarget)//因为之后敌人生成时，很可能Player已经阵亡了，首先要判断限制
        {
            currentState = EnemyState.Chasing;
            target.GetComponent<LivingEntity>().onDeath += OnTargetDeath;//target.GetComponent<PlayerController>().onDeath += TargetDeath;

            StartCoroutine(UpdatePath());
        }
    }
   
    void Update()
    {
        //navMeshAgent.SetDestination(target.position);//AI 每帧跟随
        if(hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrtDstToTarget = (target.position - transform.position).sqrMagnitude;//两者距离的平方
                if (sqrtDstToTarget < Mathf.Pow(attackDistanceThreshold, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    //MARK 反应迟钝敌人
    IEnumerator UpdatePath()
    {
        while (hasTarget)
        {
            if (currentState == EnemyState.Chasing)
            {
                if(isDead == false)
                {
                    Vector3 preTargetPos = new Vector3(target.position.x, 0, target.position.z);//玩家坐标
                    navMeshAgent.SetDestination(preTargetPos);
                }
            }
            yield return new WaitForSeconds(updateRate);//updateRate(*s)后更新，*s后再遍历上面几行
        }
    }
    
    IEnumerator Attack()
    {
        currentState = EnemyState.Attacking;//敌人状态 -->Attacking
        navMeshAgent.enabled = false;//关闭navMeshAgent，确保攻击时不寻路

        Vector3 originalPos = transform.position;//初始位置
        Vector3 attackPos = target.position;

        float attackSpeed = 3;//攻击速度
        float percent = 0;

        skinMaterial.color = Color.red;//攻击时颜色变为红色
        bool hasAttacked = false;

        while (percent <= 1)
        {
            if(percent >= .5f && !hasAttacked)
            {
                hasAttacked = true;
                target.GetComponent<IDamageable>().TakenDamage(damage);
                FindObjectOfType<GameUI>().UpdateHealth();//血条扣血
                Debug.Log("Enemy Attack!");
            }
            percent += Time.deltaTime * attackSpeed;//将攻击动画速度，由attackSpeed控制
            //FIXME toknow what the fuck is the <interpolation>  
            float interpolation = 4 * (-percent * percent + percent);//这里使用 y = 4 ( -x^2 + x ) x轴为时间，Y轴为移动攻击的距离
            transform.position = Vector3.Lerp(originalPos, attackPos, interpolation);//攻击动画，从初始位置到攻击位置
            
            yield return null;
        }
        //攻击结束，把颜色，状态，寻路打开
        skinMaterial.color = originalColor;
        currentState = EnemyState.Chasing;
        navMeshAgent.enabled = true;
    }
    void OnTargetDeath()//player death
    {
        hasTarget = false;
        currentState = EnemyState.Idle;
    }
    //
    public override void TakeHit(float _damageAmount, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (_damageAmount >= health)
        {
            if(onDeathStatic != null)
            {
                onDeathStatic();
            }
            GameObject spawnEffect = Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection));
            Destroy(spawnEffect, deathEffect.startLifetime);//1.0f是PS的Start Lifetime数值
        }
        base.TakeHit(_damageAmount, hitPoint, hitDirection);
    }
    //难度设置
    public void SetDifficulty(float _speed, float _damage, float _health, Color _color)
    {
        navMeshAgent.speed = _speed;

        damage = _damage;
        maxHealth = _health;

        deathEffect.startColor = new Color(_color.r, _color.g, _color.b, 1);
        skinMaterial = GetComponent<MeshRenderer>().material;
        skinMaterial.color = _color;
        originalColor = skinMaterial.color;
    }
}
