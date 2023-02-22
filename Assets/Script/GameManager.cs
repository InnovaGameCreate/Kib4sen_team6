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
        if(EnemyCount != 0) //�G�����ʂ��тɐ����}�C�i�X
            EnemyCount--;
        if (EnemyCount == 0)    //����0�ɂȂ�ƃN���A��ʂ̕\��
            ClearScene();
    }

    public void RetryButton()
    {
        SceneManager.LoadScene("�吼");
        EnemyCount = SaveInt;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "�吼")
        {
            Player = GameObject.Find("Player");
            Result = GameObject.Find("ClearCanvas");
            Result.SetActive(false); 
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
}
