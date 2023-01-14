using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    static public MapManager instance;
    [SerializeField]
    private GameObject[] CBlockPrefab;  //����
    [SerializeField]
    private GameObject[] LBlockPrefab;  //��
    [SerializeField]
    private GameObject[] RBlockPrefab;  //�E
    [SerializeField]
    private GameObject[] UBlockPrefab;  //��
    [SerializeField]
    private GameObject[] DBlockPrefab;  //��
    private MapInfo[,] mapinfos;
    private Vector3 Temp_Pos;
    public struct MapInfo
    {
        public int y;   //y�������̏��
        public int SnowCount;   //����@������
        public int SnowState;   //��̐ς����̏��
        //0:�f�t�H���g, -1:1�i�K����, -2:2�i�K����, -3:3�i�K����, -4:4�i�K(�j��)//
        public MapInfo(int y = 0, int SnowCount = 0, int SnowState = 0)    //�R���X�g���N�^
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SertchAroundBlocks()
    {

    }

    public void ChangeBlock(GameObject block, Transform blockPos)
    {

        ChangeSnowState(block, blockPos);

        if (mapinfos[(int)blockPos.position.x,(int)blockPos.position.y].SnowState == -1) //���ނ悤�Ƀu���b�N��u������
        {
            Destroy(block);
            Instantiate(CBlockPrefab[1], blockPos.position, Quaternion.identity);
        }
        if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.y].SnowState == -2)
        {
            Destroy(block);
            Instantiate(CBlockPrefab[2], Temp_Pos, Quaternion.identity);
        }
        if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.y].SnowState == -3)
        {
            Destroy(block);
            blockPos.position += new Vector3(0, -0.35f, 0);
            Instantiate(CBlockPrefab[3], Temp_Pos, Quaternion.identity);
        }
        if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.y].SnowState == -4)
        {
            Destroy(block);
        }
    }

    private void ChangeSnowState(GameObject block, Transform blockPos)  //��ʂ������邽�тɏ�Ԃ�ύX
    {
        mapinfos[(int)blockPos.position.x, (int)blockPos.position.y].SnowCount--;   //��ʂ������邽�тɃ}�C�i�X
        switch(mapinfos[(int)blockPos.position.x, (int)blockPos.position.y].SnowCount)
        {
            case -1:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.y].SnowState = -1;
                break;
            case -3:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.y].SnowState = -2;
                Temp_Pos = blockPos.position + new Vector3(0, -0.2f, 0);
                break;
            case -7:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.y].SnowState = -3;
                Temp_Pos = blockPos.position + new Vector3(0, -0.35f, 0);
                break;
            case -10:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.y].SnowState = -4;
                break;
        }
    }
}
