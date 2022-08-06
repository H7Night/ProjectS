using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartPanel : MonoBehaviour
{
    GameObject startPanel;
    //public GameObject historyPanel;
    Transform startPanelTransform;

    Button startButton;
    Button exitButton;
    Button optionButton;
    //Button historyButton;

    void Start()
    {
        startPanel = this.gameObject;
        startPanelTransform = gameObject.GetComponent<Transform>();

        startButton = startPanelTransform.Find("StartButton").GetComponent<Button>();
        //historyButton = startPanelTransform.Find("HistoryButton").GetComponent<Button>();
        exitButton = startPanelTransform.Find("ExitButton").GetComponent<Button>();

        startButton.onClick.AddListener(StartGame);
        //historyButton.onClick.AddListener(HistoryGame);
        exitButton.onClick.AddListener(ExitGame);

        //historyPanel.SetActive(false);

        //BGMSource = GetComponent<AudioSource>();
    }
    void StartGame()
    {
        SceneManager.LoadScene("01_Game");
    }
    void ExitGame()
    {
        Application.Quit();
    }
    void HistoryGame()
    {
        startPanel.SetActive(false);
        //historyPanel.SetActive(true);
    }

}
