using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartCountdown : MonoBehaviour
{
    [SerializeField] private float starttime;
    [SerializeField] private Text StartCount;
    [SerializeField] private CameraMove cameramove;
    [SerializeField] private PlayerController_Test playerController_Test;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (starttime + 1 >= 0)
        {
            cameramove.enabled = false;
            playerController_Test.enabled = false;
            Time.timeScale = 0;
            GetComponent<Text>().text = starttime.ToString("0");
            starttime -= Time.unscaledDeltaTime;
        }

        if(starttime.ToString("0") == "0")
            GetComponent<Text>().text = "Start!";


        if (starttime < 0)
        {
            Time.timeScale = 1;
            cameramove.enabled = true;
            playerController_Test.enabled = true;

            StartCount.enabled = false;
        }
    }
}
