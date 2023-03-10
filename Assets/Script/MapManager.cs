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
    private GameObject Temp_Obj;
    [SerializeField] private GameObject TakibiNearPrefab;
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
                mapinfos[i,j] = new MapInfo(1, 0, 0);  //メンバを0で初期化
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
            var ObjName = blockPos.position.x.ToString() + mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y.ToString() + blockPos.position.z.ToString();  //生成したオブジェクトの名前を座標に変換
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == -1) //凹むようにブロックを置き換え
            {
                if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y == 1) //初回だけそのブロックを破壊
                {
                    Destroy(block);
                    mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y--;
                }
                else
                {
                    var obj = GameObject.Find(ObjName); //当たったXZ座標の一番上のブロックを取得
                    Destroy(obj);
                }
                ObjName = blockPos.position.x.ToString() + mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y.ToString() + blockPos.position.z.ToString();  //生成したオブジェクトの名前を座標に変換
                Temp_Obj = (GameObject)Instantiate(D_CBlockPrefab[0], Temp_Pos, Quaternion.identity);
                Temp_Obj.name = ObjName;    //生成したオブジェクトの名前を座標に変更  
            }
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == -2)
            {
                var obj = GameObject.Find(ObjName); //当たったXZ座標の一番上のブロックを取得
                Destroy(obj);
                Temp_Obj = (GameObject)Instantiate(D_CBlockPrefab[1], Temp_Pos, Quaternion.identity);
                Temp_Obj.name = ObjName;    //生成したオブジェクトの名前を座標に変更  
            }
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == -3)
            {
                var obj = GameObject.Find(ObjName); //当たったXZ座標の一番上のブロックを取得
                Destroy(obj);
                Temp_Obj = (GameObject)Instantiate(D_CBlockPrefab[2], Temp_Pos, Quaternion.identity);
                Temp_Obj.name = ObjName;    //生成したオブジェクトの名前を座標に変更
            }
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == -4)
            {
                var obj = GameObject.Find(ObjName); //当たったXZ座標の一番上のブロックを取得
                Destroy(obj);
                Temp_Pos.y = -0.5f;
                Temp_Obj = (GameObject)Instantiate(D_CBlockPrefab[3], Temp_Pos, Quaternion.identity);
                Temp_Obj.name = ObjName;    //生成したオブジェクトの名前を座標に変更
            }

            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == 1)    //盛り上がるようにブロックを設置
            {
                Temp_Pos.y = mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y;
                Temp_Obj = (GameObject)Instantiate(U_CBlockPrefab[0], Temp_Pos, Quaternion.identity);
                Temp_Obj.name = ObjName;    //生成したオブジェクトの名前を座標に変更
            }
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == 2)
            {
                var obj = GameObject.Find(ObjName); //当たったXZ座標の一番上のブロックを取得
                Destroy(obj);
                Temp_Pos.y = mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y;
                Temp_Obj = (GameObject)Instantiate(U_CBlockPrefab[1], Temp_Pos, Quaternion.identity);
                Temp_Obj.name = ObjName;    //生成したオブジェクトの名前を座標に変更
            }
            if (mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState == 3)
            {
                var obj = GameObject.Find(ObjName); //当たったXZ座標の一番上のブロックを取得
                Destroy(obj);
                Temp_Pos.y = mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y;
                Temp_Obj = (GameObject)Instantiate(U_CBlockPrefab[2], Temp_Pos, Quaternion.identity);
                Temp_Obj.name = ObjName;    //生成したオブジェクトの名前を座標に変更
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].y++;   //一段上がった事を反映
                //各値の初期化
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = 0;
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount = 0;
            }

            if (Check == 2)
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount = -5;

    }

    private void ChangeSnowState(GameObject block, Transform blockPos, int Check)  //雪玉が当たるたびに状態を変更
    {
        if (Check == 1 && mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount <= 10)
            mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount++;   //雪玉が当たるたびにプラス
        else if (Check == 2)
            mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowCount = -7;
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
            case -10:   //10回で1ブロックなくなる
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = -4;
                break;
            case 1:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = 1;
                break;
            case 2:
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = 2;
                break;
            case 4: //4回で1ブロック増える
                mapinfos[(int)blockPos.position.x, (int)blockPos.position.z].SnowState = 3;
                break;
        }
    }
}
