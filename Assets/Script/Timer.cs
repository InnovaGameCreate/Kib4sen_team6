using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private float counttime;
    private float minute;
    private float second;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        counttime -= Time.deltaTime;
        minute = (int)counttime / 60;
        second = (int)counttime % 60;
        if (second > 10)
            GetComponent<Text>().text = minute.ToString() + "•ª" + second.ToString() + "•b";
        else
            GetComponent<Text>().text = minute.ToString() + "•ª " + second.ToString() + "•b";

    }
}
