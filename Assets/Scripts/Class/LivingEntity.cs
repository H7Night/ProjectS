using UnityEngine;
using System;

//该脚本为playerController和Enemy的父类
//任何有生命的实体
public class LivingEntity : MonoBehaviour, IDamageable
{
    public float maxHealth;//最大生命值
    public float health { get; protected set;}//当前生命值  //如果写{ get; protected set;}[SerializeField]也不能展现--debug完再设置
    protected bool isDead;//判断是否死亡

    public event Action onDeath;//委托

    protected virtual void Start()//虚方法，子类可以重写
    {
        //游戏开始，当前生命值等于最大生命值
        health = maxHealth;
    }

    //死亡
    [ContextMenu("Self Destruct")]
    protected void Die()
    {
        isDead = true;
        Destroy(gameObject);

        if(onDeath != null)
            onDeath();
    }

    //实现接口TakeHit -- 攻击
    public virtual void TakeHit(float _damageAmount, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakenDamage(_damageAmount);
    }

    //受伤,实现接口TakeDamage -- 伤害
    public virtual void TakenDamage(float _damageAmount)
    {
        health -= _damageAmount;

        if(health <= 0 && isDead == false)
        {
            Die();
        }
    }
}
