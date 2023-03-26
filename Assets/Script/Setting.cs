using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class Setting : MonoBehaviour
{
    CameraMove MainCamSens;
    GameObject Player;
    Slider slider;
    [SerializeField]
    Text X_Value;
    [SerializeField]
    Text Y_Value;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        Player = GameObject.Find("Player");
        MainCamSens = Player.GetComponent<CameraMove>();
        slider.minValue = 0;
        slider.maxValue = 10;
        if (this.gameObject.name == "SensX")
        {
            slider.value = MainCamSens.Xsensityvity;    //���x���X���C�_�[�ŕύX
            X_Value.text = slider.value.ToString(); //���x��\��
        }
        else
        {
            slider.value = MainCamSens.Ysensityvity;
            Y_Value.text = slider.value.ToString();
        }
    }

    public void ChangeSlider()
    {
        if (this.gameObject.name == "SensX")
        {
            MainCamSens.Xsensityvity = slider.value;
            X_Value.text = slider.value.ToString("0.0"); //���x��\��
        }
        else
        {
            MainCamSens.Ysensityvity = slider.value;
            Y_Value.text = slider.value.ToString("0.0");
        }
    }
}
