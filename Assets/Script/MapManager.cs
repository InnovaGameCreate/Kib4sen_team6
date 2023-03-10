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
    private GameObject Temp_Obj;
    [SerializeField] private GameObject TakibiNearPrefab;
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
                mapinfos[i,j] = new MapInfo(1, 0, 0);  //�����o��0�ŏ�����
            }
        }
        UpFlag = false;
        DownFlag = false;
    }

    public void TakibiAroundChange(GameObject block, Transform blockPos)
    {
        Temp_Pos = blockPos.position;
        Temp_Pos.y = -0.49f;///
        Destroy(block);
        Instantiate(TakibiNearPrefab, Temp_Pos, Quaternion.identity);
    }

    public int TakibiFarAroundChange(Transform blockPos)
    {
        return mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount;
    }
        

    public void ChangeBlock(GameObject block, Transform blockPos, int Check)
    {

            ChangeSnowState(block, blockPos, Check);

            Temp_Pos = blockPos.position;
            var ObjName = blockPos.position.x.ToString() + mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y.ToString() + blockPos.position.z.ToString();  //���������I�u�W�F�N�g�̖��O�����W�ɕϊ�
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == -1) //���ނ悤�Ƀu���b�N��u������
            {
                if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y == 1) //���񂾂����̃u���b�N��j��
                {
                    Destroy(block);
                    mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y--;
                }
                else
                {
                    var obj = GameObject.Find(ObjName); //��������XZ���W�̈�ԏ�̃u���b�N���擾
                    Destroy(obj);
                }
                ObjName = blockPos.position.x.ToString() + mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y.ToString() + blockPos.position.z.ToString();  //���������I�u�W�F�N�g�̖��O�����W�ɕϊ�
                Temp_Obj = (GameObject)Instantiate(D_CBlockPrefab[0], Temp_Pos, Quaternion.identity);
                Temp_Obj.name = ObjName;    //���������I�u�W�F�N�g�̖��O�����W�ɕύX  
            }
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == -2)
            {
                var obj = GameObject.Find(ObjName); //��������XZ���W�̈�ԏ�̃u���b�N���擾
                Destroy(obj);
                Temp_Obj = (GameObject)Instantiate(D_CBlockPrefab[1], Temp_Pos, Quaternion.identity);
                Temp_Obj.name = ObjName;    //���������I�u�W�F�N�g�̖��O�����W�ɕύX  
            }
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == -3)
            {
                var obj = GameObject.Find(ObjName); //��������XZ���W�̈�ԏ�̃u���b�N���擾
                Destroy(obj);
                Temp_Obj = (GameObject)Instantiate(D_CBlockPrefab[2], Temp_Pos, Quaternion.identity);
                Temp_Obj.name = ObjName;    //���������I�u�W�F�N�g�̖��O�����W�ɕύX
            }
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == -4)
            {
                var obj = GameObject.Find(ObjName); //��������XZ���W�̈�ԏ�̃u���b�N���擾
                Destroy(obj);
                Temp_Pos.y = -0.5f;
                Temp_Obj = (GameObject)Instantiate(D_CBlockPrefab[3], Temp_Pos, Quaternion.identity);
                Temp_Obj.name = ObjName;    //���������I�u�W�F�N�g�̖��O�����W�ɕύX
            }

            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == 1)    //����オ��悤�Ƀu���b�N��ݒu
            {
                Temp_Pos.y = mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y;
                Temp_Obj = (GameObject)Instantiate(U_CBlockPrefab[0], Temp_Pos, Quaternion.identity);
                Temp_Obj.name = ObjName;    //���������I�u�W�F�N�g�̖��O�����W�ɕύX
            }
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == 2)
            {
                var obj = GameObject.Find(ObjName); //��������XZ���W�̈�ԏ�̃u���b�N���擾
                Destroy(obj);
                Temp_Pos.y = mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y;
                Temp_Obj = (GameObject)Instantiate(U_CBlockPrefab[1], Temp_Pos, Quaternion.identity);
                Temp_Obj.name = ObjName;    //���������I�u�W�F�N�g�̖��O�����W�ɕύX
            }
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == 3)
            {
                var obj = GameObject.Find(ObjName); //��������XZ���W�̈�ԏ�̃u���b�N���擾
                Destroy(obj);
                Temp_Pos.y = mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y;
                Temp_Obj = (GameObject)Instantiate(U_CBlockPrefab[2], Temp_Pos, Quaternion.identity);
                Temp_Obj.name = ObjName;    //���������I�u�W�F�N�g�̖��O�����W�ɕύX
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y++;   //��i�オ�������𔽉f
                //�e�l�̏�����
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = 0;
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount = 0;
            }

            if (Check == 2)
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount = -5;

    }

    private void ChangeSnowState(GameObject block, Transform blockPos, int Check)  //��ʂ������邽�тɏ�Ԃ�ύX
    {
        if (Check == 1 && mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount <= 10)
            mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount++;   //��ʂ������邽�тɃv���X
        else if (Check == 2)
            mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount = -7;
        else
            mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount--;   //�E�N���b�N�̂��тɃ}�C�i�X

        
        
        switch (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount)
        {
            case -1:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = -1;
                break;
            case -3:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = -2;
                break;
            case -7:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = -3;
                break;
            case -10:   //10���1�u���b�N�Ȃ��Ȃ�
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = -4;
                break;
            case 1:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = 1;
                break;
            case 2:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = 2;
                break;
            case 4: //4���1�u���b�N������
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = 3;
                break;
        }
    }
}
