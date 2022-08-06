using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
    public int score; //{ get; private set; }//外界只能获取，不能修改
    [SerializeField]float lastPressTime;
    [SerializeField] int streakCount;
    float streakExpiredTime = 4.0f;

    public Text scoreText;

    public GameObject comboText;


    [Header("Score")]
    [SerializeField] float fromScore;
    [SerializeField] float toScore;
    [SerializeField] float rollingTime;
    [SerializeField] public int endScore;



    void Start()
    {
        EnemyController.onDeathStatic += KillEnemy;
        FindObjectOfType<PlayerController>().onDeath += PlayerDeath;

        streakCount = 0;
        comboText.SetActive(false);
    }

    void KillEnemy()
    {
        //Time.time:游戏开始运行以后到当前时间一共经历多少秒
        if(Time.time < lastPressTime + streakExpiredTime)
        {
            streakCount++;
            StartCoroutine(UpdateCombo());
        }
        else
        {
            //重置连击
            streakCount = 0;
            streakCount++;
            StartCoroutine("UpdateCombo");
        }
        comboText.SetActive(true);
        comboText.GetComponent<Text>().text = "Combo × " + "<color=red>"+(+streakCount)+"</color>";
        lastPressTime = Time.time;
        RollingScore();
    }
    void RollingScore()
    {
        fromScore = score;
        toScore = fromScore + streakCount * 100;
        LeanTween.value(fromScore, toScore, rollingTime)
                  .setEase(LeanTweenType.easeOutQuart)
                  .setOnUpdate((float obj) => 
                  {
                      //lamda表达式
                      fromScore = obj;
                      scoreText.text = "Score:" + obj.ToString("0");
                  });
        score = (int)toScore;
        endScore = score;
    }

    IEnumerator UpdateCombo()
    {
        float percent = 1;
        int currentStreakCount = streakCount;
        while(percent>0)
        {
            if(currentStreakCount == streakCount)
            {
                percent -= Time.deltaTime / streakExpiredTime;//游戏时间/
                //comboImage.fillAmount = percent;
            }
            else
            {
                //StartCoroutine(UpdateCombo());
                StopCoroutine("UpdateCombo");
                StartCoroutine("UpdateCombo");
            }
            yield return null;
        }
        streakCount = 0;
        comboText.gameObject.SetActive(false);
    }
    void PlayerDeath()
    {
        EnemyController.onDeathStatic -= KillEnemy;
    }
}
