using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private float counttime;
    private float minute;
    private float second;
    // Start is called before the first frame update
    void Start()
    {
        counttime = GameManager.instance.LimitTime;
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
