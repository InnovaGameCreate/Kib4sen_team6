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
        //これはシーン遷移の条件ではありません。あくまで仮にです。もしボタンなどを使用する際は迷わず変更してください。
        if(Input.anyKey)
        {
            SceneManager.LoadScene("ゲーム画面(仮)");
        }
    }
}
