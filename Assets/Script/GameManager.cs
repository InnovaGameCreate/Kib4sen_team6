using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static public GameManager instance;
    public int EnemyCount;
    private int SaveInt;
    public GameObject Player;
    public GameObject Result;
    private void Awake()    //シングルトン
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Result.SetActive(false);
        SaveInt = EnemyCount;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearScene()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Player.GetComponent<PlayerController_Test>().enabled = false;
        Player.GetComponent<CameraMove>().enabled = false;
        Result.SetActive(true);
    }

    public void EnemyDeath()
    {
        if(EnemyCount != 0) //敵が死ぬたびに数をマイナス
            EnemyCount--;
        if (EnemyCount == 0)    //数が0になるとクリア画面の表示
            ClearScene();
    }

    public void RetryButton()
    {
        SceneManager.LoadScene("大西");
        EnemyCount = SaveInt;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "大西")
        {
            Player = GameObject.Find("Player");
            Result = GameObject.Find("ClearCanvas");
            Result.SetActive(false); 
        }
    }

    public void EndButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;   // UnityEditorの実行を停止する処理
        #else
            Application.Quit();                                // ゲームを終了する処理
        #endif
    }
}
