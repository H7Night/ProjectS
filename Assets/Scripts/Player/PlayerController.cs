using UnityEngine;

public class PlayerController : LivingEntity
{
    Rigidbody rb;
    Vector3 moveInput;
    [SerializeField] float moveSpeed;

    public CursorContorller cursor;//鼠标指针

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        //移动
        moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        //方向对着鼠标指针
        LookAtCursor();

        if (transform.position.y < -10)//如果玩家掉下去的话，扣除血量GameOver
            TakenDamage(health);
    }
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
        //rb.velocity = new Vector3(moveInput.normalized.x * moveSpeed, rb.velocity.y, moveInput.normalized.z * moveSpeed);
    }

    //改变朝向
    private void LookAtCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        
        float distToGround;
        if(plane.Raycast(ray,out distToGround))
        {
            Vector3 point = ray.GetPoint(distToGround);
            Vector3 rightPoint = new Vector3(point.x, transform.position.y, point.z);
            
            transform.LookAt(rightPoint);
            cursor.transform.position = point;// mouse position
            cursor.ToTarget(ray);
        }
    }
}
