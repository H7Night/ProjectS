using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isPaused;

    public int kill, wave;
    public float time;


    public Text hintText;
    public Text scoreText, waveText, timeText;
    List<Score> scoreList = new List<Score>();


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            if(instance != null)
            {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start() 
    {
        hintText.gameObject.SetActive(false);

        StreamReader sr = new StreamReader(Application.dataPath + "/JSONData.text");
        string nextLine;
        while ((nextLine = sr.ReadLine()) != null)
        {
            scoreList.Add(JsonUtility.FromJson<Score>(nextLine));
        } 
        sr.Close();//将所有存储的分数全部存到list中
    }
    void Update()
    {
        // scoreText.text = kill.ToString();
        // waveText.text = wave.ToString();
        // timeText.text = time.ToString();
    }
    
    public void SaveJSON()
    {
        SaveByJSON();
    }
    //Object(Save Type) --> JSON(String)
    void SaveByJSON()
    {
        Score save = createSaveGameObject();
        
        string JsonString = JsonUtility.ToJson(save);

        StreamWriter sw = new StreamWriter(Application.dataPath + "/JSONData.text");
        sw.Write(JsonString);//Write a string to a stream

        sw.Close();
        Debug.Log("--JSON saved--");
        StartCoroutine(DisplayHint("Game Saved!"));
    }

    private Score createSaveGameObject()
    {
        Score save = new Score();

        save.killNum = GameManager.instance.kill;
        save.waveNum = GameManager.instance.wave;

        return save;
    }
    IEnumerator  DisplayHint(string message)
    {
        hintText.gameObject.SetActive(true);
        hintText.text = message;

        yield return new WaitForSeconds(2);
        
        hintText.gameObject.SetActive(false);
    }
}
