using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_to_Result : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //����̓��U���g��ʂւ̑J�ڂ��邽�߂̏����ł͂���܂���B
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene("���U���g���");
        }
    }
}
