using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    static public MapManager instance;
    [SerializeField]
    private GameObject[] CBlockPrefab;  //中央
    [SerializeField]
    private GameObject[] LBlockPrefab;  //左
    [SerializeField]
    private GameObject[] RBlockPrefab;  //右
    [SerializeField]
    private GameObject[] UBlockPrefab;  //上
    [SerializeField]
    private GameObject[] DBlockPrefab;  //下
    private MapInfo[,] mapinfos;
    private Vector3 Temp_Pos;
    public struct MapInfo
    {
        public int y;   //y軸方向の情報
        public int SnowCount;   //雪を掘った回数
        public int SnowState;   //雪の積もり具合の状態
        //0:デフォルト, -1:1段階凹み, -2:2段階凹み, -3:3段階凹み, -4:4段階(破壊)//
        public MapInfo(int y = 0, int SnowCount = 0, int SnowState = 0)    //コンストラクタ
        {
            this.y = y;
            this.SnowCount = SnowCount;
            this.SnowState = SnowState;
        }
    }

    private void Awake()    //シングルトン
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
        mapinfos = new MapInfo[100,100];  //100×100の配列(x軸×z軸)
        for(int i = 0; i < 100; i++)
        {
            for(int j = 0; j < 100; j++)
            {
                mapinfos[i,j] = new MapInfo(0, 0, 0);  //メンバを0で初期化
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

        if (mapinfos[(int)blockPos.position.x,(int)blockPos.position.y].SnowState == -1) //凹むようにブロックを置き換え
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

    private void ChangeSnowState(GameObject block, Transform blockPos)  //雪玉が当たるたびに状態を変更
    {
        mapinfos[(int)blockPos.position.x, (int)blockPos.position.y].SnowCount--;   //雪玉が当たるたびにマイナス
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
