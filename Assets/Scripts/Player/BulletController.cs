using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float shootSpeed;//速度
    [SerializeField] private float damage = 1.0f;//伤害
    [SerializeField] private float lifetime;//生命周期
    
    public LayerMask collisionMask;

    void Start()
    {
        Destroy(gameObject, lifetime);
        
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);//防止武器在敌人身体内部不造成伤害
        if(initialCollisions.Length > 0)
        {
            HitEnemy(initialCollisions[0], transform.position);
        }
    }
    void Update()
    {
        transform.Translate(Vector3.forward * shootSpeed * Time.deltaTime);
        CheckCollision();
    }
    void CheckCollision()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;//结构体类型，射线击中目标的具体信息

        if(Physics.Raycast(ray, out hitInfo,shootSpeed*Time.deltaTime,collisionMask,QueryTriggerInteraction.Collide))
        {
            HitEnemy(hitInfo, hitInfo.point);
        }
    }
    private void HitEnemy(RaycastHit _hitInfo, Vector3 _hitPoint)
    {
        IDamageable damageableObject = _hitInfo.collider.GetComponent<IDamageable>();
        if (damageableObject != null)
            damageableObject.TakeHit(damage, _hitPoint, transform.forward);
        Destroy(gameObject);//击中敌人后，子弹销毁
    }

    private void HitEnemy(Collider _collider, Vector3 _hitPoint)
    {
        IDamageable damageableObject = _collider.GetComponent<IDamageable>();
        if(damageableObject != null)
            damageableObject.TakeHit(damage, _hitPoint, transform.forward);
        Destroy(gameObject);
    }
}
