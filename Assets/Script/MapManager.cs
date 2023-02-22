using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    static public MapManager instance;
    private bool DownFlag;
    private bool UpFlag;
    [SerializeField]    //D:凹む, U:盛り上がる
    private GameObject[] D_CBlockPrefab;  //中央
    [SerializeField]
    private GameObject[] D_LBlockPrefab;  //左
    [SerializeField]
    private GameObject[] D_RBlockPrefab;  //右
    [SerializeField]
    private GameObject[] D_UBlockPrefab;  //上
    [SerializeField]
    private GameObject[] D_DBlockPrefab;  //下
    [SerializeField]
    private GameObject[] U_CBlockPrefab;  //中央
    [SerializeField]
    private GameObject[] U_LBlockPrefab;  //左
    [SerializeField]
    private GameObject[] U_RBlockPrefab;  //右
    [SerializeField]
    private GameObject[] U_UBlockPrefab;  //上
    [SerializeField]
    private GameObject[] U_DBlockPrefab;  //下
    private MapInfo[,] mapinfos;
    private Vector3 Temp_Pos;
    public struct MapInfo
    {
        public float y;   //y軸方向の情報
        public int SnowCount;   //雪を掘った回数
        public int SnowState;   //雪の積もり具合の状態
        //0:デフォルト, -1:1段階凹み, -2:2段階凹み, -3:3段階凹み, -4:4段階(破壊)//
        //0:デフォルト, 1:1段階盛り, 2:2段階盛り, 3:3段階盛り(ブロック一つ)
        public MapInfo(float y = 0, int SnowCount = 0, int SnowState = 0)    //コンストラクタ
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
        UpFlag = false;
        DownFlag = false;
    }

    public void TakibiAroundChange(GameObject block, Transform blockPos)
    {
        Temp_Pos = blockPos.position;
        Temp_Pos.y = -0.49f;///
        Destroy(block);
        GameObject takibiground = D_CBlockPrefab[4];
        takibiground.tag = "Untagged";
        Instantiate(D_CBlockPrefab[4], Temp_Pos, Quaternion.identity);
    }
        

    public void ChangeBlock(GameObject block, Transform blockPos, int Check)
    {
        if (block != D_CBlockPrefab[4])
        {

            ChangeSnowState(block, blockPos, Check);

            Temp_Pos = blockPos.position;

            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == -1) //凹むようにブロックを置き換え
            {
                Temp_Pos.y = 0;
                Destroy(block);
                Instantiate(D_CBlockPrefab[1], Temp_Pos, Quaternion.identity);
            }
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == -2)
            {
                Temp_Pos.y = 0;///
                Destroy(block);
                Instantiate(D_CBlockPrefab[2], Temp_Pos, Quaternion.identity);
            }
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == -3)
            {
                Temp_Pos.y = 0;///
                Destroy(block);
                Instantiate(D_CBlockPrefab[3], Temp_Pos, Quaternion.identity);
            }
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == -4)
            {
                Temp_Pos.y = -0.49f;///
                Destroy(block);
                Instantiate(D_CBlockPrefab[4], Temp_Pos, Quaternion.identity);
            }

            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == 1)    //盛り上がるようにブロックを設置
            {
                Destroy(block);
                UpFlag = false; //一度だけ実行されるように
                Temp_Pos.y = 0.1666648f;
                Instantiate(U_CBlockPrefab[0], Temp_Pos, Quaternion.identity);
            }
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == 2)
            {
                Temp_Pos.y = 0.3333298f;
                Destroy(block);
                UpFlag = false;
                Instantiate(U_CBlockPrefab[1], Temp_Pos, Quaternion.identity);
            }
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == 3)
            {
                Temp_Pos.y = 0.5f;
                Destroy(block);
                UpFlag = false;
                Instantiate(U_CBlockPrefab[2], Temp_Pos, Quaternion.identity);
            }
        }
    }

    private void ChangeSnowState(GameObject block, Transform blockPos, int Check)  //雪玉が当たるたびに状態を変更
    {
        if(Check == 1 && mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount <= 10)
            mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount++;   //雪玉が当たるたびにプラス
        else
            mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount--;   //右クリックのたびにマイナス

        
        
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
            case -10:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = -4;
                break;
            case 1:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = 1;
                UpFlag = true;
                break;
            case 3:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = 2;
                UpFlag = true;
                break;
            case 7:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = 3;
                UpFlag = true;
                break;
        }
    }
}
