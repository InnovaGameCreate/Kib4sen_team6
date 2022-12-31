using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSet_Test : MonoBehaviour
{
    Vector2 displayCenter;

    // ブロックを設置する位置を一応リアルタイムで格納
    private Vector3 pos;

    [SerializeField]
    private GameObject blockPrefab;

    // Use this for initialization
    void Start()
    {
        //画面中央の平面座標を取得する
        displayCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        //「カメラからのレイ」を画面中央の平面座標から飛ばす
        Ray ray = Camera.main.ScreenPointToRay(displayCenter);
        //当たったオブジェクト情報を格納する変数
        RaycastHit hit;

        //Physics.Raycast() でレイを飛ばす
        if (Physics.Raycast(ray, out hit))
        {
            //衝突した面の方向+ブロックの座標
            pos = hit.normal/2 + hit.collider.transform.position;

            //右クリック
            if (Input.GetMouseButtonDown(1))
            {
                //生成位置の変数の座標にブロックを生成
                Instantiate(blockPrefab, pos, Quaternion.identity);
            }

            //左クリック
            if (Input.GetMouseButtonDown(0))
            {
                if(hit.collider.gameObject.tag == "Snow")
                Destroy(hit.collider.gameObject);   //対称ブロックを破壊
            }
        }
    }
}
