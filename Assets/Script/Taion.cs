using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Taion : MonoBehaviour
{
    [SerializeField] private float taion;  //�V�O���C�h�֐��̂悤�ȓ��������܂�
    [SerializeField] private float time;  //�̉���0�ɂȂ�܂ł̑�̂̎���
    private float a;
    private float x; //�V�O���C�h�֐��ɂ�����x���ɑ����B�̉��𓮂��������Ƃ��͂����𓮂����B
    [SerializeField] private float risespeed; //�̉����オ��X�s�[�h
    [SerializeField] private float dropspeed; //�̉���������X�s�[�h
    private float cp; 
    private float oncp;
    private bool col;
    private float cx;
    private float plux;
    private float taicp;

    [SerializeField] private Slider TaionBar;

    // Start is called before the first frame update
    void Start()
    {
        col = false;
        a = -Mathf.Log(1 / 0.999f - 1, Mathf.Exp(1)) / (time / 2);
        x = - Mathf.Log(1/0.999f-1, Mathf.Exp(1)) / a;
        cp = taion;
        oncp = taion;
        cx = 0;
        TaionBar.value = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(col == false)
        {
            if(taion > 0.5)
            {
                x -= dropspeed * Time.deltaTime;
                taion = oncp / (1 + Mathf.Exp((-x - plux) * a));  //0�ɂȂ�Ȃ��̂ŏ����t������Ƃ��Ȃǂ͒���
                TaionBar.value = taion / cp;
            }
            else
            {
                taion = 0;
                TaionBar.value = 0;
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Takibi"))
        {
            col = true;
            if (x >= 0)
            {
                cx = ((time / 2) - x) / 2;
            }
            else
            {
                cx = ((time / 2) + x) / 2;
            }
            a = -Mathf.Log(1 / 0.999f - 1, Mathf.Exp(1)) / cx;
            oncp = cp - taion;
            taicp = taion;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Takibi"))
        {
            if (taion <= 99.5)
            {
                x += risespeed * Time.deltaTime;
                taion = oncp / (1 + Mathf.Exp((-x + cx) * a)) + taicp;
                TaionBar.value = taion / cp;
            }

            else
            {
                taion = 100;
                TaionBar.value = 1;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Takibi"))
        {
            if (x >= 0)
            {
                plux = (time / 2 - x) / 2;
            }
            else
            {
                plux = (time / 2 + x) / 2;
            }
            cx = (time / 2 + x) / 2;
            a = -Mathf.Log(1 / 0.999f - 1, Mathf.Exp(1)) / cx;
            oncp = taion;
            col = false;
        }    }
}
