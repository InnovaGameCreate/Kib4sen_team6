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
        //これはリザルト画面への遷移するための条件ではありません。
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene("リザルト画面");
        }
    }
}
