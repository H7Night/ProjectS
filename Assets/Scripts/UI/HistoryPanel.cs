using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HistoryPanel : MonoBehaviour
{
    GameObject historypanel;
    public GameObject startPanel;
    Transform historyTransform;
    Transform buttonTransform;

    Button killButton;
    Button waveButton;
    Button timeButton;
    Button backButton;
    void Start()
    {
        historypanel = this.gameObject;
        historyTransform = gameObject.GetComponent<Transform>();
        buttonTransform = historyTransform.Find("Button").GetComponent<Transform>();
        killButton = buttonTransform.Find("KillButton").GetComponent<Button>();
        waveButton = buttonTransform.Find("WaveButton").GetComponent<Button>();
        timeButton = buttonTransform.Find("TimeButton").GetComponent<Button>();
        backButton = buttonTransform.Find("BackButton").GetComponent<Button>();

        backButton.onClick.AddListener(Back);
    }


    
    void Back()
    {
        historypanel.SetActive(false);
        startPanel.SetActive(true);
    }
}
