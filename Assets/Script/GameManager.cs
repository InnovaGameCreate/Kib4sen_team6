using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static public GameManager instance;
    private int EnemyCount;
    public float LimitTime; //ゲームの制限時間
    [SerializeField] private int SaveInt;    //敵の最大数を常に保存
    [SerializeField] private float SaveTime;    //カウントダウンの開始時間を常に保存
    private GameObject Player;
    [SerializeField]
    private GameObject Result;
    private GameObject MainUI;
    private GameObject CountDown;
    private GameObject Timer;
    private float starttime;    //カウントダウンの時間
    private float counttime;    //制限時間
    private float minute;
    private float second;
    private bool CountDownFlag;
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
        SceneManager.sceneLoaded += OnSceneLoaded;
        //Debug.Log("aa");
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        MainUIController();
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
        starttime = SaveTime;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "大西")
        {
            Initialize();
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

    private void MainUIController()
    {
        if(CountDownFlag)
        {
            Time.timeScale = 0; //ゲーム内の時間を停止
            CountDown.GetComponent<Text>().text = starttime.ToString("0");    //整数で時間を表示
            starttime -= Time.unscaledDeltaTime;
            if (starttime.ToString("0") == "0") //0が表示されると
            {  
                CountDown.GetComponent<Text>().text = "Start!"; //startを表示
                CountDownFlag = false;
                Time.timeScale = 1; //ゲーム内の時間を再度動かす
                Player.GetComponent<PlayerController_Test>().enabled = true;
                Player.GetComponent<CameraMove>().enabled = true;
                CountDown.GetComponent<Text>().enabled = false; //カウントダウンを非表示にする
            }
        }

            

        if (!Result.activeSelf)
        {

            counttime -= Time.deltaTime;
            minute = (int)counttime / 60;
            second = (int)counttime % 60;
            if (second > 10)
                Timer.GetComponent<Text>().text = minute.ToString() + "分" + second.ToString() + "秒";
            else
                Timer.GetComponent<Text>().text = minute.ToString() + "分 " + second.ToString() + "秒";
        }

    }

    private void Initialize()
    {
        
        CountDownFlag = true;
        counttime = LimitTime;
        SaveInt = EnemyCount;
        MainUI = GameObject.Find("StartUI");
        Timer = GameObject.Find("Timer");
        CountDown = GameObject.Find("StartCountdown");
        Player = GameObject.Find("Player");
        Result = GameObject.Find("ClearCanvas");
        Result.SetActive(false);
        Player.GetComponent<PlayerController_Test>().enabled = false;
        Player.GetComponent<CameraMove>().enabled = false;
        starttime = SaveTime;
    }
}

