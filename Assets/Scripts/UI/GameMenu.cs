using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class GameMenu : MonoBehaviour
{
    public Transform gamePanelTransform;
    // public Transform gameMenuTransform;

    public Image gameMenuImage;
    public Toggle BGMToggle;

    AudioSource BGMSource;
    PlayerController player;

    public Button optionsButton;

    public Button restartButton;
    public Button saveButton;
    public Button backGameButton;
    public Button exitButtton;
    

    private void Start() 
    {
        gameMenuImage.gameObject.SetActive(false);

        BGMSource = GetComponent<AudioSource>();
        player = FindObjectOfType<PlayerController>();

        
        optionsButton.onClick.AddListener(ToGameMenu);//游戏界面进入GameMenu
        restartButton.onClick.AddListener(RestartButton);
        saveButton.onClick.AddListener(SaveButton);
        backGameButton.onClick.AddListener(BackGame);
        exitButtton.onClick.AddListener(ExitGame);
        

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameManager.instance.isPaused)
            {
                Resume();
            }
            else
            {
                Paused();
            }
        }
        BGManger();
    }
    
    public void Resume()
    {
        gameMenuImage.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
        GameManager.instance.isPaused = false;
    }
    private void Paused()
    {
        gameMenuImage.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
        GameManager.instance.isPaused = true;
    }

    void ExitGame()
    {
        Application.Quit();
    }
    void RestartButton()
    {
        SceneManager.LoadScene("01_Game");
    }
    void SaveButton()
    {
        if(BGMToggle.isOn)
        {
            PlayerPrefs.SetInt("BGM", 1);
            Debug.Log(PlayerPrefs.GetInt("BGM")+"BGM set true");
        }
        else
        {
            PlayerPrefs.SetInt("BGM", 0);
            Debug.Log(PlayerPrefs.GetInt("BGM")+"BGM set flase");
        }
    }
    void ToGameMenu()
    {
        Paused();
        gameMenuImage.gameObject.SetActive(true);
        gamePanelTransform.gameObject.SetActive(false);
    }
    void BackGame()
    {
        Resume();
        gameMenuImage.gameObject.SetActive(false);
        gamePanelTransform.gameObject.SetActive(true);
    }

    public void BGMToggleButton()
    {
        if(BGMToggle.isOn)
        {
            //OPEN THE BGM
            PlayerPrefs.SetInt("BGM", 1);//1 means open and 0 means close.(CUSTOMIZED)
            //Debug.Log(PlayerPrefs.GetInt("BGM"));
        }
        else
        {
            //CLOSE THE BGM
            PlayerPrefs.SetInt("BGM", 0);
            //Debug.Log(PlayerPrefs.GetInt("BGM"));
        }
    }
    void BGManger()
    {
        if(PlayerPrefs.GetInt("BGM") == 1)
        {
            BGMToggle.isOn = true;
            BGMSource.enabled = true;
        }
        else if(PlayerPrefs.GetInt("BGM") == 0)
        {
            BGMToggle.isOn = false;
            BGMSource.enabled = false;
        }
    }

}
