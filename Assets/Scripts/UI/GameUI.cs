using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class GameUI : MonoBehaviour
{
    public Image fadeImage;
    public Color fromColor, toColor;
    public GameObject gameOverUI;

    public RectTransform waveBannerTrans;
    public Text waveText;
    
    #region Time
    public Text timeText;
    public int TotalTime = 90;//总时间
    private int mumite;//分
    private int second;//秒
    #endregion

    #region healthBar
    public Text healthText;
    public RectTransform healthBarTrans;//血量底部显示
    public RectTransform healthBarShadowTrans;//缓冲效果
    [SerializeField] private float shadowSpeed = 0.1f;
    #endregion

    #region 
    public Button restartButton;
    // public Button optionButton;
    public Button backButton;
    public Button exitButton;
    #endregion


    PlayerController player;

    public Text gameoverScoreText;


    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        player.onDeath += GameOver;

        restartButton.onClick.AddListener(RestartButton);
        //optionButton.onClick.AddListener(OptionButton);
        backButton.onClick.AddListener(BackButton);
        exitButton.onClick.AddListener(ExitButton);

        healthText.text = player.health + "/" + player.maxHealth;
        StartCoroutine(RollTime());
    }

    #region HealthBar
    public void UpdateHealth()//这个方法将会在人物受伤时调用，用来显示玩家血量
    {
        if(player != null)
        {
            float healthPointPercent = player.health / player.maxHealth;
            healthBarTrans.localScale = new Vector3(healthPointPercent, 1, 1);

            healthText.text = player.health + "/" + player.maxHealth;
            StartCoroutine(ShadowEffectCo());
        }
    }
    IEnumerator ShadowEffectCo()
    {
        float shadowX = healthBarShadowTrans.localScale.x;
        while(shadowX > healthBarTrans.localScale.x)
        {
            shadowX -= shadowSpeed * Time.deltaTime;
            healthBarShadowTrans.localScale = new Vector3(shadowX, 1, 1);
            yield return null;
        }
    }
    #endregion
    
    IEnumerator RollTime()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1);
        while (TotalTime >= 0)
        {
            
            TotalTime--;
            timeText.text = "Time:" + TotalTime;

            if (TotalTime <= 0)
            {                
                Debug.Log("战斗结束！");
            }
            mumite = TotalTime / 60; //输出显示分
            second = TotalTime % 60; //输出显示秒
            string length = mumite.ToString();
            //如果秒大于10的时候，就输出格式为 00：00
            if (second >= 10)
            {
                timeText.text = "0" + mumite + ":" + second;
            }
            //如果秒小于10的时候，就输出格式为 00：00
            else
                timeText.text = "0" + mumite + ":0" + second;
            yield return waitForSeconds;
        }
    }
    

    public void NewWaveBannerUI(int waveIndex)
    {
        string[] numbers = { "1", "2", "3", "4", "5" };
        waveText.text = "Wave " + numbers[waveIndex - 1];

        StartCoroutine(AnimateWaveBanner());
    }

    IEnumerator AnimateWaveBanner()
    {
        float delayTime = 2.0f;
        float floatSpeed = 2.5f;
        float animatePercent = 0;
        int direction = 1;

        float endDelayTime = Time.time + 1 / floatSpeed + delayTime;

        while(animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * floatSpeed * direction;

            if(animatePercent >= 1)
            {
                animatePercent = 1;
                if(Time.time > endDelayTime)
                {
                    direction = -1;
                }
            }

            waveBannerTrans.anchoredPosition = Vector2.up * Mathf.Lerp(200, 15, animatePercent);
            yield return null;
        }
    }

    #region Button
    void RestartButton()
    {
        SceneManager.LoadScene("01_Game");
    }
    void BackButton()
    {
        SceneManager.LoadScene("00_Start");
    }    
    void ExitButton()
    {
        Application.Quit();
    }
    #endregion
    
    void GameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(fromColor, toColor, 1));
        gameoverScoreText.text = FindObjectOfType<ScoreKeeper>().score.ToString();
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color _from, Color _to, float _time)
    {
        float speed = 1 / _time;
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadeImage.color = Color.Lerp(_from, _to, percent);
            yield return null;
        }
    }
}
