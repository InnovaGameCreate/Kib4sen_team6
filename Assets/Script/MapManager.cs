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
        public int y;   //y軸方向の情報
        public int SnowCount;   //雪を掘った回数
        public int SnowState;   //雪の積もり具合の状態
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
        Destroy(block);
        if(mapinfos[(int)blockPos.position.x,(int)blockPos.position.y].SnowState == 0)
        {
            Instantiate(BlockPrefab[1], blockPos.position, Quaternion.identity);
        }
    }
}
