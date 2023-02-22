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
    [SerializeField] private int SaveInt;    //�G�̍ő吔����ɕۑ�
    [SerializeField] private float SaveTime;    //�J�E���g�_�E���̊J�n���Ԃ���ɕۑ�
    private GameObject Player;
    [SerializeField]
    private GameObject Result;
    private GameObject MainUI;
    private GameObject CountDown;
    private GameObject Timer;
    private float starttime;    //�J�E���g�_�E���̎���
    private float counttime;    //��������
    private float minute;
    private float second;
    private bool CountDownFlag;
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
        if(EnemyCount != 0) //�G�����ʂ��тɐ����}�C�i�X
            EnemyCount--;
        if (EnemyCount == 0)    //����0�ɂȂ�ƃN���A��ʂ̕\��
            ClearScene();
    }

    public void RetryButton()
    {
        SceneManager.LoadScene("�吼");
        EnemyCount = SaveInt;
        starttime = SaveTime;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "�吼")
        {
            Initialize();
        }
    }

    public void EndButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;   // UnityEditor�̎��s���~���鏈��
        #else
            Application.Quit();                                // �Q�[�����I�����鏈��
        #endif
    }

    private void MainUIController()
    {
        if(CountDownFlag)
        {
            Time.timeScale = 0; //�Q�[�����̎��Ԃ��~
            CountDown.GetComponent<Text>().text = starttime.ToString("0");    //�����Ŏ��Ԃ�\��
            starttime -= Time.unscaledDeltaTime;
            if (starttime.ToString("0") == "0") //0���\��������
            {  
                CountDown.GetComponent<Text>().text = "Start!"; //start��\��
                CountDownFlag = false;
                Time.timeScale = 1; //�Q�[�����̎��Ԃ��ēx������
                Player.GetComponent<PlayerController_Test>().enabled = true;
                Player.GetComponent<CameraMove>().enabled = true;
                CountDown.GetComponent<Text>().enabled = false; //�J�E���g�_�E�����\���ɂ���
            }
        }

            

        if (!Result.activeSelf)
        {

            counttime -= Time.deltaTime;
            minute = (int)counttime / 60;
            second = (int)counttime % 60;
            if (second > 10)
                Timer.GetComponent<Text>().text = minute.ToString() + "��" + second.ToString() + "�b";
            else
                Timer.GetComponent<Text>().text = minute.ToString() + "�� " + second.ToString() + "�b";
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

