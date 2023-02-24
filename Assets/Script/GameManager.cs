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
    private GameObject Tutorial;
    private GameObject CheckList;
    private Text TutoText;
    private Text CheckText;
    private bool TutorialFlag;
    private float starttime;    //カウントダウンの時間
    private float counttime;    //制限時間
    private float minute;
    private float second;
    private bool CountDownFlag;
    private Animator PlayerAnim;
    private GameObject[] EnemySpownPos;
    [SerializeField]
    private GameObject EnemyPrefab;

    private bool StartFlag;
    private bool MoveFlag;
    private bool HealFlag;
    private bool AttackFlag;
    private bool GetFlag;
    private bool First;
    private bool Second;
    private bool Third;
    private bool Fourth;
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
        StartFlag = true;
        MoveFlag = false;
        HealFlag = false;
        AttackFlag = false;
        GetFlag = false;
        First = true;
        Second = false;
        Third = false;
        Fourth = false;
        TutorialFlag = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        MainUIController();
        if (TutorialFlag)
        {
            FlagManager();
            if(!Second)
                Player.GetComponent<Taion>().taion = 100f;
        }
    }

    public void GameOverScene()
    {
        PlayerAnim.Play("Lose");
        EndConduct();
        GameOverCanvas.SetActive(true);
    }

    public void ClearScene()
    {
         
        PlayerAnim.Play("Win");
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
        if (scene.name == "チュートリアル")
        {
            TutorialFlag = true;
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
                Player.GetComponent<PlayerController_Test>().enabled = true;
                Player.GetComponent<CameraMove>().enabled = true;
                if (TutorialFlag)
                {
                    Tutorial.SetActive(true);
                    StartCoroutine(TutorialManager());
                }
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
        PlayerAnim = Player.GetComponent<Animator>();
        PlayerAnim.SetBool("Win", false);
        PlayerAnim.SetBool("Lose", false);
        starttime = SaveTime;
        CalcTime();
        if (TutorialFlag)
        {
            EnemyCount = 1;
            counttime = 600;
            Tutorial = GameObject.Find("ExplanationText");
            TutoText = Tutorial.GetComponent<Text>();
            CheckList = GameObject.Find("CheckBox");
            CheckText = CheckList.GetComponent<Text>();
            Tutorial.SetActive(false);
        }
    }

    private void FlagManager()
    {
        if(Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.A)|| Input.GetKeyDown(KeyCode.S)|| Input.GetKeyDown(KeyCode.D))
        {
            MoveFlag = true;
        }
        if(Input.GetMouseButton(0))
        {
            AttackFlag = true;
        }
        if(Input.GetMouseButton(1) && Third)
        {
            GetFlag = true;
        }
        if(Player.GetComponent<Taion>().taion >= 100f && Second)
        {
            HealFlag = true;
        }

    }

    IEnumerator TutorialManager()
    {
        while (true)
        {
            if (StartFlag)
            {
                TutoText.text = "SnowBallへようこそ!\n今からチュートリアルを始めるよ!";
                StartFlag = false;
            }
            yield return new WaitForSeconds(2f);
            if (First && !CountDownFlag)
            {
                TutoText.text = "移動はWASD\nジャンプはスペースで行うよ\n左クリックで雪玉を投げて攻撃するよ!\n雪玉が落ちた場所には雪が積もるよ";
                yield return new WaitForSeconds(2f);
                CheckText.text = "移動\n攻撃";
                string str = CheckText.text;
                if (MoveFlag)
                {
                    str = str.Replace("移動", "");
                    CheckText.text = str;
                }
                if (AttackFlag)
                {
                    str = str.Replace("攻撃", "");
                    CheckText.text = str;
                }
                if (MoveFlag && AttackFlag)
                {
                    First = false;
                    Second = true;
                    Player.GetComponent<Taion>().taion = 90f;
                }
            }
            if (Second)
            {
                TutoText.text = "体温が減ってきたね\n体温が0になるか時間切れになると\nゲームオーバーだから気を付けてね!\n体温は焚火の近くに行くと回復するよ!";
                yield return new WaitForSeconds(2f);
                CheckText.text = "体温を回復しよう";
                string str2 = CheckText.text;
                if (HealFlag)
                {
                    str2 = str2.Replace("体温を回復しよう", "");
                    CheckText.text = str2;
                    Third = true;
                    Second = false;
                }
            }
            if(Third)
            {
                TutoText.text = "次に右クリック長押しで雪玉を集めよう";
                yield return new WaitForSeconds(2f);
                CheckText.text = "雪玉を集めよう";
                string str3 = CheckText.text;
                if (GetFlag)
                { 
                    str3 = str3.Replace("雪玉を集めよう", "");
                    CheckText.text = str3;
                    Third = false;
                    Fourth = true;
                }
            }
            if(Fourth)
            {
                TutoText.text = "最後に今まで学んだことを活かして雪だるまを倒してみよう!";
                yield return new WaitForSeconds(2f);
                TutoText.text = "";
                CheckText.text = "雪だるまを倒そう";
            }
            yield return new WaitForSeconds(1.0f);
        }
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

