using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    static public MapManager instance;
    [SerializeField]
    private GameObject[] BlockPrefab;
    private MapInfo[,] mapinfos;
    public struct MapInfo
    {
        public int y;   //y�������̏��
        public int SnowCount;   //����@������
        public int SnowState;   //��̐ς����̏��
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
        Destroy(block);
        if(mapinfos[(int)blockPos.position.x,(int)blockPos.position.y].SnowState == 0)
        {
            Instantiate(BlockPrefab[1], blockPos.position, Quaternion.identity);
        }
    }
}
