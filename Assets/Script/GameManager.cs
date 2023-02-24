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
    private float starttime;    //�J�E���g�_�E���̎���
    private float counttime;    //��������
    private float minute;
    private float second;
    private bool CountDownFlag;
    private GameObject[] EnemySpownPos;
    [SerializeField]
    private GameObject EnemyPrefab;
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
        if(EnemyCount != 0) //�G�����ʂ��тɐ����}�C�i�X
            EnemyCount--;
            Debug.Log(EnemyCount);
        if (EnemyCount == 0)    //����0�ɂȂ�ƃN���A��ʂ̕\��
            ClearScene();
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "�Q�[�����(��)")
        {
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
                Time.timeScale = 1; //�Q�[�����̎��Ԃ��ēx������
                Player.GetComponent<PlayerController_Test>().enabled = true;
                Player.GetComponent<CameraMove>().enabled = true;
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
            Timer.GetComponent<Text>().text = minute.ToString() + "��" + second.ToString() + "�b";
        else
            Timer.GetComponent<Text>().text = minute.ToString() + "�� " + second.ToString() + "�b";
    }


}

