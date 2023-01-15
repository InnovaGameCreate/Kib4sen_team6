using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    static public MapManager instance;
    private bool DownFlag;
    private bool UpFlag;
    [SerializeField]    //D:����, U:����オ��
    private GameObject[] D_CBlockPrefab;  //����
    [SerializeField]
    private GameObject[] D_LBlockPrefab;  //��
    [SerializeField]
    private GameObject[] D_RBlockPrefab;  //�E
    [SerializeField]
    private GameObject[] D_UBlockPrefab;  //��
    [SerializeField]
    private GameObject[] D_DBlockPrefab;  //��
    [SerializeField]
    private GameObject[] U_CBlockPrefab;  //����
    [SerializeField]
    private GameObject[] U_LBlockPrefab;  //��
    [SerializeField]
    private GameObject[] U_RBlockPrefab;  //�E
    [SerializeField]
    private GameObject[] U_UBlockPrefab;  //��
    [SerializeField]
    private GameObject[] U_DBlockPrefab;  //��
    private MapInfo[,] mapinfos;
    private Vector3 Temp_Pos;
    public struct MapInfo
    {
        public float y;   //y�������̏��
        public int SnowCount;   //����@������
        public int SnowState;   //��̐ς����̏��
        //0:�f�t�H���g, -1:1�i�K����, -2:2�i�K����, -3:3�i�K����, -4:4�i�K(�j��)//
        //0:�f�t�H���g, 1:1�i�K����, 2:2�i�K����, 3:3�i�K����(�u���b�N���)
        public MapInfo(float y = 0, int SnowCount = 0, int SnowState = 0)    //�R���X�g���N�^
        {
            this.y = y;
            this.SnowCount = SnowCount;
            this.SnowState = SnowState;
        }
    }

    private void Awake()    //�V���O���g��
    {
        if(instance == null)
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
        mapinfos = new MapInfo[100,100];  //100�~100�̔z��(x���~z��)
        for(int i = 0; i < 100; i++)
        {
            for(int j = 0; j < 100; j++)
            {
                mapinfos[i,j] = new MapInfo(0, 0, 0);  //�����o��0�ŏ�����
            }
        }
        UpFlag = false;
        DownFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SertchAroundBlocks()
    {

    }

    public void ChangeBlock(GameObject block, Transform blockPos, int Check)
    {

        ChangeSnowState(block, blockPos, Check);

        if (mapinfos[(int)blockPos.position.x,(int)blockPos.position.z].SnowState == -1) //���ނ悤�Ƀu���b�N��u������
        {
            Destroy(block);
            Instantiate(D_CBlockPrefab[1], blockPos.position, Quaternion.identity);
        }
        if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == -2)
        {
            Destroy(block);
            Instantiate(D_CBlockPrefab[2], Temp_Pos, Quaternion.identity);
        }
        if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == -3)
        {
            Destroy(block);
            Instantiate(D_CBlockPrefab[3], Temp_Pos, Quaternion.identity);
        }
        if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == -4)
        {
            Destroy(block);
        }

        if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == 1)    //����オ��悤�Ƀu���b�N��ݒu
        {
            UpFlag = false; //��x�������s�����悤��
            Instantiate(U_CBlockPrefab[0], Temp_Pos, Quaternion.identity);
        }
        if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == 2)
        {
            Destroy(block);
            UpFlag = false;
            Instantiate(U_CBlockPrefab[1], Temp_Pos, Quaternion.identity);
        }
        if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == 3)
        {
            Destroy(block);
            UpFlag = false;
            Instantiate(U_CBlockPrefab[2], Temp_Pos, Quaternion.identity);
        }

    }

    private void ChangeSnowState(GameObject block, Transform blockPos, int Check)  //��ʂ������邽�тɏ�Ԃ�ύX
    {
        if(Check == 1)
            mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount++;   //��ʂ������邽�тɃv���X
        else
            mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount--;   //�E�N���b�N�̂��тɃ}�C�i�X
        
        switch (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount)
        {
            case -1:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = -1;
                break;
            case -3:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = -2;
                Temp_Pos = blockPos.position + new Vector3(0, -0.2f, 0);
                break;
            case -7:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = -3;
                Temp_Pos = blockPos.position + new Vector3(0, -0.35f, 0);
                break;
            case -10:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = -4;
                break;
            case 1:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = 1;
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y += 1; //�u���b�N��u��y���W���v�Z
                Temp_Pos = blockPos.position + new Vector3(0, mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y, 0);  //�u���b�N��ݒu����y���W��ݒ�
                UpFlag = true;
                break;
            case 3:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = 2;
                UpFlag = true;
                break;
            case 7:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = 3;
                UpFlag = true;
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount = 0; //�J�E���g�����Z�b�g
                break;
        }
    }
}
