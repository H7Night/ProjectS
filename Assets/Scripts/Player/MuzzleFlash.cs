using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public GameObject MuzzleHolder;//枪口的空物体，用于生成枪口火焰
    public Sprite[] flashSprites;//分割后开火图片
    public SpriteRenderer[] spriteRenderers;//场景中开枪火焰的图片

    public float flashTime;

    private void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        MuzzleHolder.SetActive(true);

        int randomNum = Random.Range(0, flashSprites.Length);//分割图片中
        //设置随机开火图片
        for(int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = flashSprites[randomNum];
        }

        Invoke("Deactivate", flashTime);
    }

    private void Deactivate()
    {
        MuzzleHolder.SetActive(false);
    }
}
