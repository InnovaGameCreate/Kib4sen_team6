using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static public GameManager instance;
    private int EnemyCount;
    public float LimitTime; //�Q�[���̐�������
    [SerializeField] private int EnemyNum;    //�G�̍ő吔����ɕۑ�
    [SerializeField] private float SaveTime;    //�J�E���g�_�E���̊J�n���Ԃ���ɕۑ�
    private GameObject Player;
    [SerializeField]
    public GameObject GameOverCanvas;
    private GameObject Result;
    private GameObject MainUI;
    private GameObject CountDown;
    private GameObject Timer;
    private GameObject PoseUI;
    private GameObject SettingUI;
    private GameObject Tutorial;
    private GameObject CheckList;
    private Text TutoText;
    private Text CheckText;
    private bool TutorialFlag;
    private float starttime;    //�J�E���g�_�E���̎���
    private float counttime;    //��������
    private float minute;
    private float second;
    private bool CountDownFlag;
    private Animator PlayerAnim;
    private GameObject[] EnemySpownPos;
    [SerializeField]
    private GameObject EnemyPrefab;

    private bool TitleFlag = true;

    private bool DownFlag;
    private bool StartFlag;
    private bool MoveFlag;
    private bool HealFlag;
    private bool AttackFlag;
    private bool GetFlag;
    private bool First;
    private bool Second;
    private bool Third;
    private bool Fourth;

    bool PushEscape = false;
    private void Awake()    //�V���O���g��
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
        DownFlag = false;
        MoveFlag = false;
        HealFlag = false;
        AttackFlag = false;
        GetFlag = false;
        First = true;
        Second = false;
        Third = false;
        Fourth = false;
        //TutorialFlag = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        //Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !PoseUI.activeInHierarchy && !PushEscape)    //�|�[�Y��ʂ���A�N�e�B�u�̎���ESC�������ꂽ��
        {
            PushEscape = true;
            MainUI.SetActive(false);
            PoseUI.SetActive(true);
            Time.timeScale = 0;
            EndConduct();
        }
        if(Input.GetKeyDown(KeyCode.Escape) && PoseUI.activeInHierarchy && !PushEscape)
        {
            MainUI.SetActive(true);
            PoseUI.SetActive(false);
            Time.timeScale = 1;
            Player.GetComponent<PlayerController_Test>().enabled = true;
            Player.GetComponent<CameraMove>().enabled = true;
            Player.GetComponent<Taion>().enabled = true;
        }
        if(Input.GetKeyUp(KeyCode.Escape))
            PushEscape = false;

        if (!TitleFlag)
            MainUIController();
        if (TutorialFlag)
        {
            FlagManager();
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

    public void ReturnTitle()
    {
        SceneManager.LoadScene("�X�^�[�g���");
        Time.timeScale = 1;
    }

    public void EnemyDeath()
    {
        if(EnemyCount != 0) //�G�����ʂ��тɐ����}�C�i�X
            EnemyCount--;
            Debug.Log(EnemyCount);
        if (EnemyCount == 0)    //����0�ɂȂ�ƃN���A��ʂ̕\��
            ClearScene();
    }

    public void SettingButton()
    {
        PoseUI.SetActive(false);
        SettingUI.SetActive(true);
    }
    public void BackToGameButton()
    {
        MainUI.SetActive(true);
        PoseUI.SetActive(false);
        Time.timeScale = 1;
        Player.GetComponent<PlayerController_Test>().enabled = true;
        Player.GetComponent<CameraMove>().enabled = true;
        Player.GetComponent<Taion>().enabled = true;
    }
    public void BackButton()
    {
        PoseUI.SetActive(true);
        SettingUI.SetActive(false);
    }
    public void RetryButton()
    {
        SceneManager.LoadScene("�Q�[�����(��)");
        EnemyCount = EnemyNum;
        starttime = SaveTime;
    }

    public void StartButton()
    {
        SceneManager.LoadScene("�Q�[�����(��)");
        EnemyCount = EnemyNum;
        starttime = SaveTime;
    }

    public void TutorialButton()
    {
        SceneManager.LoadScene("�`���[�g���A��");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "�X�^�[�g���")
            TitleFlag = true;
        else
            TitleFlag = false;
        if (scene.name == "�Q�[�����(��)")
        {
            Initialize();
        }
        if (scene.name == "�`���[�g���A��")
        {
            TutorialFlag = true;
            Initialize();
        }

    }

    public void EndButton()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;   // UnityEditor�̎��s���~���鏈��
#endif
#if UNITY_WEBGL
            SceneManager.LoadScene("�X�^�[�g���");
#else
            Application.Quit();                                // �Q�[�����I�����鏈��
#endif
    }

    private void MainUIController()
    {
        if(CountDownFlag)
        {
            CountDown.GetComponent<Text>().text = starttime.ToString("0");    //�����Ŏ��Ԃ�\��
            starttime -= Time.deltaTime;
            if (starttime.ToString("0") == "0") //0���\��������
            {  
                CountDown.GetComponent<Text>().text = "Start!"; //start��\��
                CountDownFlag = false;
                Player.GetComponent<PlayerController_Test>().enabled = true;
                Player.GetComponent<CameraMove>().enabled = true;
                if (TutorialFlag)
                {
                    Tutorial.SetActive(true);
                    StartCoroutine(TutorialManager());
                }
                CountDown.SetActive(false); //�J�E���g�_�E�����\���ɂ���
            }
        }

            

        if (!Result.activeSelf && !GameOverCanvas.activeSelf && !CountDown.activeSelf)
        {
            counttime -= Time.deltaTime;
            CalcTime();
            if (counttime <= 0) //���Ԃ��؂ꂽ��
                GameOverScene();
        }

    }

    private void Initialize()
    {
        EnemySpownPos = GameObject.FindGameObjectsWithTag("EnemySpown");    //����̃^�O�̃I�u�W�F�N�g���i�[
        for (int i = 0; i < EnemySpownPos.Length; i++)
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
        SettingUI = GameObject.Find("SettingCanvas");
        PoseUI = GameObject.Find("PoseCanvas");
        Result.SetActive(false);
        GameOverCanvas.SetActive(false);
        SettingUI.SetActive(false);
        PoseUI.SetActive(false);
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
        if(Second)
        {
            HealFlag = true;
        }

    }

    IEnumerator TutorialManager()
    {
        while (true)
        {
            if (First && !CountDownFlag)
            {
                if (StartFlag)
                {
                    TutoText.text = "SnowBall�ւ悤����!\n������`���[�g���A�����n�߂��!";
                    yield return new WaitForSeconds(2f);
                    TutoText.text = "�ړ���WASD\n�W�����v�̓X�y�[�X�ōs����\n���N���b�N�Ő�ʂ𓊂��čU�������!\n��ʂ��������ꏊ�ɂ͐Ⴊ�ς����";
                    CheckText.text = "�ړ�\n�U��";
                    yield return new WaitForSeconds(2f);
                    TutoText.text = "";
                    StartFlag = false;
                }
                string str = CheckText.text;
                if (MoveFlag)
                {
                    str = str.Replace("�ړ�", "");
                    CheckText.text = str;
                }
                if (AttackFlag)
                {
                    str = str.Replace("�U��", "");
                    CheckText.text = str;
                }
                if (MoveFlag && AttackFlag)
                {
                    First = false;
                    Second = true;
                    StartFlag = true;
                }
            }
            if (Second)
            {
                if (StartFlag)
                {
                    DownFlag = true;
                    TutoText.text = "�̉��������Ă�����\n�̉���0�ɂȂ邩���Ԑ؂�ɂȂ��\n�Q�[���I�[�o�[������C��t���Ă�!\n�̉��͕��΂̋߂��ɍs���Ɖ񕜂����!";
                    CheckText.text = "�̉����񕜂��悤";
                    yield return new WaitForSeconds(5f);
                    TutoText.text = "";
                    StartFlag = false;
                }
                
                string str2 = CheckText.text;
                if (HealFlag)
                {
                    str2 = str2.Replace("�̉����񕜂��悤", "");
                    CheckText.text = str2;
                    Third = true;
                    Second = false;
                    StartFlag = true;
                }
            }
            if(Third)
            {
                if (StartFlag)
                {
                    TutoText.text = "���΂ɋ߂Â��Ǝ莝���̐�ʂ��S���n�����������";
                    yield return new WaitForSeconds(4f);
                    TutoText.text = "��̏�ŉE�N���b�N�������Ő�ʂ��W�߂悤";
                    yield return new WaitForSeconds(2f);
                    TutoText.text = "";
                    CheckText.text = "��ʂ��W�߂悤";
                    StartFlag = false;
                }
                
                string str3 = CheckText.text;
                if (GetFlag)
                { 
                    str3 = str3.Replace("��ʂ��W�߂悤", "");
                    CheckText.text = str3;
                    Third = false;
                    Fourth = true;
                    StartFlag = true;
                }
            }
            if(Fourth)
            {
                if (StartFlag)
                {
                    TutoText.text = "�Ō�ɍ��܂Ŋw�񂾂��Ƃ��������ĐႾ��܂�|���Ă݂悤!";
                    CheckText.text = "�Ⴞ��܂�|����";
                    yield return new WaitForSeconds(2f);
                    TutoText.text = "";
                    StartFlag = false;
                }
                

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
            Timer.GetComponent<Text>().text = minute.ToString() + "��" + second.ToString() + "�b";
        else
            Timer.GetComponent<Text>().text = minute.ToString() + "�� " + second.ToString() + "�b";
    }


}

