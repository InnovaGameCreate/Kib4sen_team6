using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Taion : MonoBehaviour
{
    [SerializeField] public float taion;  //�V�O���C�h�֐��̂悤�ȓ��������܂�
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
    [SerializeField] private float YukidamaDamage; //��ʂ��_���[�W���󂯂��Ƃ��ɂǂ�ʌ��邩

    [SerializeField] private Slider TaionBar;
    [SerializeField] private Image TaionColor;
    [SerializeField] private Image TaionBG;
    [SerializeField] private Sprite TemUI1;
    [SerializeField] private Sprite TemUI2;
    [SerializeField] private Sprite TemUI3;
    private Color Tcol;

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
        if (taion <= cp / 3)
        {
            Tcol = new Color32(116, 145, 251,255);
            TaionColor.color = Tcol;
            TaionBG.sprite = TemUI1;
        }
        else if (cp / 3 < taion && taion < 2 * cp / 3)
        {
            Tcol = new Color32(163, 0, 92,255);
            TaionColor.color = Tcol;
            TaionBG.sprite = TemUI2;
        }
        
        else
        {
            Tcol = new Color32(255, 0, 0,255);
            TaionColor.color = Tcol;
            TaionBG.sprite = TemUI3;
        }

        if (col == false)
        {
            if (taion > 0.5)
            {
                x -= dropspeed * Time.deltaTime;
                taion = oncp / (1 + Mathf.Exp((-x - plux) * a));  //0�ɂȂ�Ȃ��̂ŏ����t������Ƃ��Ȃǂ͒���
                TaionBar.value = taion / cp;
            }
            else
            {
                taion = 0;
                TaionBar.value = 0;
                GameManager.instance.GameOverScene();
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

        if (other.CompareTag("EnemyBall"))
        {
            x = -Mathf.Log(((oncp / (taion - YukidamaDamage)) - 1) / Mathf.Exp(-plux * a)) / a;
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
