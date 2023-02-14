using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    private int HavingBallNum;
    [SerializeField]
    private Transform Self; //自身
    [SerializeField]
    private Transform Target;   //ターゲット
    [SerializeField]
    GameObject childObj;
    public GameObject ballPrefab;

    [Header("敵のパラメータ")]
    [SerializeField,Range(30,180)]
    private int SightAngle;   //視野角
    [SerializeField]
    private float MaxDistance = float.PositiveInfinity; //視界の最大距離
    [SerializeField]
    private int speed;
    private Vector3 TargetPos;
    private bool isVisible;
    private Coroutine shotCoroutine;
    private float TargetDistance;
    private Rigidbody TargetRigid;
    private LineRenderer line;
    private Vector3 ShotDir;
    private int AreaNum;
    private int ShotArea;
    const int Right = 0;
    const int Left = 1;
    const int Center = 3;
    //private Vector3 PreTragetPos;   //1フレーム前のターゲットの位置
    Vector3 Moving_Distance;    //1秒間の移動量
    private bool TurnFlag;  //ターゲットの方向を向き続けるか
    float CosDiv1;
    float CosDiv2;
    float CosDiv3;
    public struct AreaInfo
    {
        public float Probabillity; //そのエリアの事前確率
        public float After_Probabillity;
        public float Count; //そのエリアにきた回数

    }

    private float After_ProbabillitySum; //事後確率の総数
    List<int> TargetMovedArea = new List<int>();    //ターゲットが移動したエリアを格納
    AreaInfo[] AreaInfos = new AreaInfo[7]; //各エリアの情報


    enum State
    {
        Serch,
        Attack,
        Reload,
        doNothing,
    }

    private State currentState = State.Serch;
    private bool stateEnter = false;
    private float stateTime = 0f;

    void ChangeState(State _nextState)
    {
        currentState = _nextState;
        stateEnter = true;
        stateTime = 0f;
    }

    public bool IsVisible() //視界に入っているか判定
    {
        var SelfPos = Self.position;    //自身の位置

        TargetPos = Target.position;    //ターゲットの位置

        var SelfDir = Self.forward; //自身の向き

        var TargetDir = TargetPos - SelfPos;    //自分から見た敵の方向
        TargetDistance = TargetDir.magnitude;   //ターゲットとの距離

        var CosHalf = Mathf.Cos(SightAngle / 2 * Mathf.Deg2Rad);    //cos(θ/2)を計算
        var InnerProduct = Vector3.Dot(SelfDir, TargetDir.normalized);  //内積を計算


        CosDiv1 = Mathf.Cos(SightAngle / 6 * Mathf.Deg2Rad);    //cos(θ/6)を計算
        CosDiv2 = Mathf.Cos(2 * SightAngle / 6 * Mathf.Deg2Rad);    //cos(2θ/6)を計算
        CosDiv3 = Mathf.Cos(3 * SightAngle / 6 * Mathf.Deg2Rad);    //cos(3θ/6)を計算

        var diff = TargetPos - SelfPos;

        var axis = Vector3.Cross(transform.forward, diff);  //外積を求める

        var angle = Vector3.Angle(transform.forward, diff) * (axis.y < 0 ? -1 : 1); //ターゲットとの角度を-180～180に変換

        /*
        Debug.Log(Mathf.Acos(CosDiv1) * Mathf.Rad2Deg);
        Debug.Log(Mathf.Acos(CosDiv2));
        Debug.Log(Mathf.Acos(CosDiv3));
        */
        
        //ターゲットがどの位置にいるか毎フレーム確認
        if (InnerProduct > CosDiv1) 
        {
            AreaNum = 0;
            //ShotDir = RandomAngle(CosDiv1, CosDiv1,Center);
        }
        if (InnerProduct < CosDiv1 && InnerProduct > CosDiv2)
        {
            if (angle > 0)
            {
                AreaNum = 1;
            }
            else
            {
                AreaNum = 4;
            }
        }
        if (InnerProduct < CosDiv2 && InnerProduct > CosDiv3)
        {
            if (angle > 0)
            {
                AreaNum = 2;
            }
            else
            {
                AreaNum = 5;
            }
            
        }
        if (InnerProduct < CosDiv3)
        {
            if (angle > 0)
            {
                AreaNum = 3;
            }
            else
            {
                AreaNum = 6;
            }
        }


            //ShotDir = RandomAngle(CosDiv1);

        return InnerProduct > CosHalf && TargetDistance < MaxDistance;  //角度判定かつ距離判定
    }

    private void Start()
    {
        TurnFlag = true;
        line = this.GetComponent<LineRenderer>();
        TargetRigid = Target.GetComponent<Rigidbody>();
        HavingBallNum = 10;
        childObj = transform.GetChild(0).gameObject;    //最初の子オブジェクトの座標を取得
        for(int i = 0; i < AreaInfos.Length; i++)
        {
            AreaInfos[i].Probabillity = 1f/7f; //事前確率を初期化
            AreaInfos[i].After_Probabillity = 1f/7f;
            AreaInfos[i].Count = 0f;
        }
    }

    // 視界判定の結果をGUI出力
    private void Update()
    {
        line.SetPosition(0, childObj.transform.position);
        line.SetPosition(1, transform.forward * 1000);
        
        stateTime += Time.deltaTime;    //現在のステートになってからの時間を計測
        // 視界判定
        isVisible = IsVisible();
        StateManager();

    }

    private  void TurnToTarget()    //ターゲットの方を向く
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(TargetPos - transform.position),
            1);

    }


    private IEnumerator BallShot() 
    {
        while (true)
        {
            if (isVisible)
            {
                TurnFlag = false;   //投げるときは向きを固定
                GameObject ball = (GameObject)Instantiate(ballPrefab, childObj.transform.position, Quaternion.identity);
                Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
                yield return new WaitForSeconds(0.5f);    //射撃準備で0.5秒停止
                                                          //ballRigidbody.AddForce(transform.forward * speed, ForceMode.Impulse);
                if (TargetMovedArea.Count != 0)//1回目以外の時
                    ShotDir = RandomAngle(Bayesian()); //ベイズ推定
                else
                    ShotDir = transform.forward;
                
                //ShotDir = RandomAngle();
                ballRigidbody.AddForce(ShotDir.normalized * speed, ForceMode.Impulse);
                TargetMovedArea.Add(AreaNum);   //投げる瞬間のターゲットの位置を保存
                TurnFlag = true;
                HavingBallNum--;    //持っている雪玉の数を1減らす
                var rnd = Random.Range(1, 11);　// ※ 1～10の範囲でランダムな整数値が返る
                if(rnd == 1 || rnd == 2)    //ランダムな確率で弾を発射しない状態に移行
                {
                    //ChangeState(State.doNothing);
                   // yield break;
                }
            }
                yield return new WaitForSeconds(0.5f);    //1秒待機
        }
    }

    private void StateManager()
    {
        switch (currentState)
        {
            case State.Serch:
                if (stateEnter) //この状態になってから最初のフレームだけ実行
                {}
                if (isVisible)
                {
                    ChangeState(State.Attack);
                    return;
                }
                break;
            case State.Attack:
                if (stateEnter) //この状態になってから最初のフレームだけ実行
                {
                    shotCoroutine = StartCoroutine("BallShot");
                }
                if(TurnFlag)    //フラグがtrueなら
                    TurnToTarget(); //ターゲットの方を向く
                break;
            case State.doNothing:
                Debug.Log("やることなくなった");
                break;
        }
    }

    private void LateUpdate()
    {
        if(stateTime != 0)
            stateEnter = false;
    }

    private Vector3 RandomAngle(int Area)  //視界内でランダムにボールを投げる
    {
        Debug.Log(Area);
        if (Area == 0)
        {
           var Deg = Mathf.Acos(CosDiv1) * Mathf.Rad2Deg;
           var rand  = Random.Range(-Deg, Deg);    //-θ/6～θ/6の範囲でランダムな角度を獲得
           var RandRad = rand * Mathf.Deg2Rad; //degをradに変換
           var RandomDir = Quaternion.Euler(0, rand, 0) * transform.forward;   //ランダムな発射方向
            return RandomDir;
        }
        if (Area == 1)
        {
            var MaxDeg = Mathf.Acos(CosDiv2) * Mathf.Rad2Deg;
            var MinDeg = Mathf.Acos(CosDiv1) * Mathf.Rad2Deg;
            var rand = Random.Range(MinDeg, MaxDeg);    //MinDeg～MaxDigの範囲でランダムな角度を獲得
            var RandRad = rand * Mathf.Deg2Rad; //degをradに変換
            var RandomDir = Quaternion.Euler(0, rand, 0) * transform.forward;   //ランダムな発射方向
            return RandomDir;
        }
        if(Area == 2)
        {

            var MaxDeg = Mathf.Acos(CosDiv3) * Mathf.Rad2Deg;
            var MinDeg = Mathf.Acos(CosDiv2) * Mathf.Rad2Deg;
            var rand = Random.Range(MinDeg, MaxDeg);    //MinDeg～MaxDigの範囲でランダムな角度を獲得
            var RandRad = rand * Mathf.Deg2Rad; //degをradに変換
            //var RandomDir = transform.forward + new Vector3(Mathf.Sin(RandRad), 0, Mathf.Cos(RandRad));   //ランダムな発射方向
            var RandomDir = Quaternion.Euler(0, rand, 0) * transform.forward;   //ランダムな発射方向
            return RandomDir;
        }
        if (Area == 4)
        {
            var MaxDeg = Mathf.Acos(CosDiv2) * Mathf.Rad2Deg;
            var MinDeg = Mathf.Acos(CosDiv1) * Mathf.Rad2Deg;
            var rand = Random.Range(MinDeg, MaxDeg);    //MinDeg～MaxDigの範囲でランダムな角度を獲得
            var RandRad = rand * Mathf.Deg2Rad; //degをradに変換
            var RandomDir = Quaternion.Euler(0, -rand, 0) * transform.forward;   //ランダムな発射方向
            return RandomDir;
        }
        if (Area == 5)
        {
            var MaxDeg = Mathf.Acos(CosDiv3) * Mathf.Rad2Deg;
            var MinDeg = Mathf.Acos(CosDiv2) * Mathf.Rad2Deg;
            var rand = Random.Range(MinDeg, MaxDeg);    //MinDeg～MaxDigの範囲でランダムな角度を獲得
            var RandRad = rand * Mathf.Deg2Rad; //degをradに変換
            var RandomDir = Quaternion.Euler(0, -rand, 0) * transform.forward;   //ランダムな発射方向
            return RandomDir;
        }

        return transform.forward;
        
    }

    private int Bayesian() //ベイズ推定
    {
        var CountNum = TargetMovedArea.Count - 1;
        AreaInfos[TargetMovedArea[CountNum]].Count++;   //これまでに各エリアを何回通ったかカウント
        for (int i = 0; i < AreaInfos.Length; i++)
        {
            var likelihood = AreaInfos[i].Count / (float)TargetMovedArea.Count; //尤度を求める
            AreaInfos[i].After_Probabillity = AreaInfos[i].Probabillity * likelihood; //事後確率を求める
            After_ProbabillitySum += AreaInfos[i].After_Probabillity;  //事後確率の総和を求める
        }
        for (int i = 0; i < AreaInfos.Length; i++)
        {
            AreaInfos[i].After_Probabillity = AreaInfos[i].After_Probabillity / After_ProbabillitySum;   //事後確率の正規化
            if (i == 0)
                ShotArea = 0; 
            else
            {
                if (AreaInfos[i].After_Probabillity > AreaInfos[ShotArea].After_Probabillity)
                    ShotArea = i;   //常に確率が最も高いエリアを格納
            }
        }
        After_ProbabillitySum = 0;
        return ShotArea;   //確率が最も高いエリアを返す
    }


}
