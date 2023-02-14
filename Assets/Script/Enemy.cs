using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Transform Self; //自身
    [SerializeField]
    private Transform Target;   //ターゲット
    [SerializeField]
    private float SightAngle;   //視野角
    [SerializeField]
    private float MaxDistance = float.PositiveInfinity; //視界の最大距離
    [SerializeField] 
    GameObject childObj;
    [SerializeField]
    private int speed;
    public SnowBall ball;
    public GameObject ballPrefab;
    private Vector3 TargetPos;
    private bool isVisible;
    // Start is called before the first frame update

    public bool IsVisible()
    {
        var SelfPos = Self.position;    //自身の位置

        TargetPos = Target.position;    //ターゲットの位置

        var SelfDir = Self.forward; //自身の向き

        var TargetDir = TargetPos - SelfPos;    //自分から見た敵の方向
        var TargetDistance = TargetDir.magnitude;   //ターゲットとの距離

        var CosHalf = Mathf.Cos(SightAngle / 2 * Mathf.Deg2Rad);    //cos(θ/2)を計算
        var InnerProduct = Vector3.Dot(SelfDir, TargetDir.normalized);  //内積を計算


        return InnerProduct > CosHalf && TargetDistance < MaxDistance;  //角度判定かつ距離判定
    }

    private void Start()
    {
        childObj = transform.GetChild(0).gameObject;    //最初の子オブジェクトの座標を取得
        StartCoroutine("BallShot");
    }

    // 視界判定の結果をGUI出力
    private void Update()
    {
        // 視界判定
        isVisible = IsVisible();
        if (isVisible)
        {
            TurnToTarget();
        }

    }

    private  void TurnToTarget()    //ターゲットの方を向く
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(Target.transform.position - transform.position),
            1);
    }


    private IEnumerator BallShot()  //0.5秒間隔で発射
    {
        while (true)
        {
            if (isVisible)
            {
                GameObject ball = (GameObject)Instantiate(ballPrefab, childObj.transform.position, Quaternion.identity);
                Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
                ballRigidbody.AddForce(transform.forward * speed);
            }
                yield return new WaitForSeconds(0.5f);
            
        }
    }
}
