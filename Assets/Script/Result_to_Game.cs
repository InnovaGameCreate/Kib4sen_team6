using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Result_to_Game : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //����̓V�[���J�ڂ̏����ł͂���܂���B�����܂ŉ��ɂł��B�����{�^���Ȃǂ��g�p����ۂ͖��킸�ύX���Ă��������B
        if(Input.anyKey)
        {
            SceneManager.LoadScene("�Q�[�����(��)");
        }
    }
}
