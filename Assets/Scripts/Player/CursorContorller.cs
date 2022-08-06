using UnityEngine;

public class CursorContorller : MonoBehaviour
{
    public LayerMask targetMask;
    
    private SpriteRenderer spriteRenderer;
    public Color highlightColor;
    private Color originColor;

    [SerializeField]private float rotateSpeed = 75;

    private void Start()
    {
        Cursor.visible = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotateSpeed * Time.deltaTime), Space.Self);
    }

    public void ToTarget(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            //ray检测到敌人变换颜色
            spriteRenderer.color = highlightColor;
        }
        else
        {
            spriteRenderer.color = originColor;
        }
    }
}
