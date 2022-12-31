using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBall : MonoBehaviour
{
    private Vector3 pos; //ブロックを設置する位置
    [SerializeField]
    private GameObject blockPrefab;
    private Rigidbody ballRigid;
    [SerializeField]
    private float force = 1f;
    // Start is called before the first frame update
    void Start()
    {
        ballRigid = this.GetComponent<Rigidbody>();
        ballRigid.AddForce(new Vector3(force, 0, 0));

    }

    // Update is called once per frame
    void Update()
    {
        //ボールの位置から速度方向へ飛ばす
        Ray ray = new Ray(this.transform.position,ballRigid.velocity);
        //当たったオブジェクト情報を格納する変数
        RaycastHit hit;

        //Physics.Raycast() でレイを飛ばす
        if (Physics.Raycast(ray, out hit))
        {
            //衝突した面の方向+ブロックの座標
            pos = hit.normal / 2 + hit.collider.transform.position;

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
        Instantiate(blockPrefab, pos, Quaternion.identity);

    }
}
