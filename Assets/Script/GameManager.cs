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
    [SerializeField] private int EnemyNum;    //敵の最大数を常に保存
    [SerializeField] private float SaveTime;    //カウントダウンの開始時間を常に保存
    private GameObject Player;
    [SerializeField]
    public GameObject GameOverCanvas;
    private GameObject Result;
    private GameObject MainUI;
    private GameObject CountDown;
    private GameObject Timer;
    private float starttime;    //カウントダウンの時間
    private float counttime;    //制限時間
    private float minute;
    private float second;
    private bool CountDownFlag;
    private GameObject[] EnemySpownPos;
    [SerializeField]
    private GameObject EnemyPrefab;
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

    public void GameOverScene()
    {
        EndConduct();
        GameOverCanvas.SetActive(true);
    }

    public void ClearScene()
    {
        EndConduct();
        Result.SetActive(true);
    }

    public void EnemyDeath()
    {
        if(EnemyCount != 0) //敵が死ぬたびに数をマイナス
            EnemyCount--;
            Debug.Log(EnemyCount);
        if (EnemyCount == 0)    //数が0になるとクリア画面の表示
            ClearScene();
    }

    public void RetryButton()
    {
        SceneManager.LoadScene("ゲーム画面(仮)");
        EnemyCount = EnemyNum;
        starttime = SaveTime;
    }

    public void StartButton()
    {
        SceneManager.LoadScene("ゲーム画面(仮)");
        EnemyCount = EnemyNum;
        starttime = SaveTime;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "ゲーム画面(仮)")
        {
            Initialize();
        }
    }

    public void EndButton()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;   // UnityEditorの実行を停止する処理
#endif
#if UNITY_WEBGL
            SceneManager.LoadScene("スタート画面");
#else
            Application.Quit();                                // ゲームを終了する処理
#endif
    }

    private void MainUIController()
    {
        if(CountDownFlag)
        {
            CountDown.GetComponent<Text>().text = starttime.ToString("0");    //整数で時間を表示
            starttime -= Time.deltaTime;
            if (starttime.ToString("0") == "0") //0が表示されると
            {  
                CountDown.GetComponent<Text>().text = "Start!"; //startを表示
                CountDownFlag = false;
                Time.timeScale = 1; //ゲーム内の時間を再度動かす
                Player.GetComponent<PlayerController_Test>().enabled = true;
                Player.GetComponent<CameraMove>().enabled = true;
                CountDown.SetActive(false); //カウントダウンを非表示にする
            }
        }

            

        if (!Result.activeSelf && !GameOverCanvas.activeSelf && !CountDown.activeSelf)
        {
            counttime -= Time.deltaTime;
            CalcTime();
            if (counttime <= 0) //時間が切れたら
                GameOverScene();
        }

    }

    private void Initialize()
    {
        EnemySpownPos = GameObject.FindGameObjectsWithTag("EnemySpown");    //特定のタグのオブジェクトを格納
        for (int i = 0; i < EnemyNum; i++)
        {
            var EnemyPos = EnemySpownPos[i].transform.position;
            Destroy(EnemySpownPos[i]);
            Instantiate(EnemyPrefab, EnemyPos, Quaternion.identity);
        }
        CountDownFlag = true;
        EnemyCount = EnemyNum;
        counttime = LimitTime;
        MainUI = GameObject.Find("StartUI");
        Timer = GameObject.Find("Timer");
        CountDown = GameObject.Find("StartCountdown");
        Player = GameObject.Find("Player");
        Result = GameObject.Find("ClearCanvas");
        GameOverCanvas = GameObject.Find("GameOverCanvas");
        Result.SetActive(false);
        GameOverCanvas.SetActive(false);
        Player.GetComponent<PlayerController_Test>().enabled = false;
        Player.GetComponent<CameraMove>().enabled = false;
        starttime = SaveTime;
        CalcTime();
    }

    private void EndConduct()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Player.GetComponent<PlayerController_Test>().enabled = false;
        Player.GetComponent<CameraMove>().enabled = false;
        Player.GetComponent<Taion>().enabled = false;
    }

    private void CalcTime()
    {
        minute = (int)counttime / 60;
        second = (int)counttime % 60;
        if (second > 10)
            Timer.GetComponent<Text>().text = minute.ToString() + "分" + second.ToString() + "秒";
        else
            Timer.GetComponent<Text>().text = minute.ToString() + "分 " + second.ToString() + "秒";
    }


}

