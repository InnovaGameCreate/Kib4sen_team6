using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.AI;

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
    [SerializeField, Range(30, 180)]
    private int SightAngle;   //視野角
    [SerializeField]
    private float MaxDistance = float.PositiveInfinity; //視界の最大距離
    [SerializeField]
    private int speed;
    [SerializeField]
    private float ReloadTime;
    [SerializeField]
    private float waitTime;
    private bool isVisible;
    private float preShotTime;
    private Coroutine shotCoroutine;
    private float TargetDistance;
    private Rigidbody TargetRigid;
    private Vector3 ShotDir;
    private int AreaNum;
    private int ShotArea;
    private bool TurnFlag;  //ターゲットの方向を向き続けるか
    float CosDiv1;
    float CosDiv2;
    float CosDiv3;
    float Temp_prob;
    bool IntFlag;
    bool ShotFlag;
    bool OnceFlag;
    bool WalkFlag;
    float time;
    GameObject ball;
    Rigidbody ballRigidbody;
    private NavMeshAgent agent;
    public struct AreaInfo
    {
        public float Probabillity; //そのエリアの事前確率
        public float After_Probabillity;
        public float Count; //そのエリアにきた回数

    }

    private float After_ProbabillitySum; //事後確率の総数
    AreaInfo[] AreaInfos = new AreaInfo[6]; //各エリアの情報

    private Animator characterAnim;
    private AnimatorStateInfo AnimInfo;

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


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;  //目的地に近づくときに速度が落ちないようにする
        agent.updateRotation = false;   //回転しないようにする
        OnceFlag = false;
        IntFlag = true;
        TurnFlag = true;
        TargetRigid = Target.GetComponent<Rigidbody>();
        HavingBallNum = 10;
        for (int i = 0; i < AreaInfos.Length; i++)
        {
            AreaInfos[i].Probabillity = 1f / 6f; //事前確率を初期化
            AreaInfos[i].After_Probabillity = 1f / 6f;
            AreaInfos[i].Count = 0f;
        }
        characterAnim = GetComponent<Animator>();
    }

    // 視界判定の結果をGUI出力
    private void Update()
    {
        stateTime += Time.deltaTime;    //現在のステートになってからの時間を計測
        // 視界判定
        isVisible = IsVisible();
        StateManager();

    }

    public bool IsVisible() //視界に入っているか判定
    {
        var SelfPos = Self.position;    //自身の位置

        var TargetPos = Target.position;    //ターゲットの位置

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
            if (angle > 0)
            {
                AreaNum = 0;
            }
            else
            {
                AreaNum = 3;
            }
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


        //ShotDir = RandomAngle(CosDiv1);

        return InnerProduct > CosHalf && TargetDistance < MaxDistance;  //角度判定かつ距離判定
    }

    private void TurnToTarget()    //ターゲットの方を向く
    {
        var Vector = Target.position - transform.position;
        Vector.y = 0f;
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(Vector),
            1);

    }

    private void StateManager()
    {
        Debug.Log(currentState);
        switch (currentState)
        {
            case State.Serch:
                if (stateEnter) //この状態になってから最初のフレームだけ実行
                {
                    GotoNextPoint();    //ランダムな地点を取得
                }
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    StopHere();
                if (isVisible)
                {
                    ChangeState(State.Attack);
                    return;
                }
                break;
            case State.Attack:
                if (stateEnter) //この状態になってから最初のフレームだけ実行
                {
                    SetFlag();
                    //shotCoroutine = StartCoroutine("BallShot");
                }

                //if(stateTime - preShotTime >= Shot_CD)  //一定間隔で射撃
                BallShot();

                if (HavingBallNum <= 0) //残弾数がなくなったらリロード
                {
                    characterAnim.Play("Snowman_double_Idle");
                    preShotTime = 0f;
                    ChangeState(State.Reload);
                    //shotCoroutine = StopCoroutine("BallShot");
                }

                if (TurnFlag)
                    TurnToTarget(); //ターゲットの方を向く
                
                break;
            case State.doNothing:
                Debug.Log("やることなくなった");
                break;
            case State.Reload:
                if(stateTime >= ReloadTime)
                {
                    HavingBallNum = 10;
                    ChangeState(State.Serch);
                }
                break;
        }
    }

    void ChangeState(State _nextState)
    {
        agent.isStopped = true;
        currentState = _nextState;
        stateEnter = true;
        stateTime = 0f;
    }

    private void LateUpdate()
    {
        if (stateTime != 0)
            stateEnter = false;
    }

    /*private IEnumerator BallShot()
    {
        while (true)
        {
            if (isVisible)
            {
                //characterAnim.SetBool("ShotFlag",true);   //投げるときは向きを固定
                TurnFlag = false;
                GameObject ball = (GameObject)Instantiate(ballPrefab, childObj.transform.position, Quaternion.identity);
                Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
                yield return new WaitForSeconds(0.3f);    //射撃準備で0.3秒停止
                ShotDir = RandomAngle(Bayesian()); //ベイズ推定
                //ShotDir = RandomAngle();
                ballRigidbody.AddForce(ShotDir.normalized * speed, ForceMode.Impulse);
                AreaInfos[AreaNum].Count++;   //投げる瞬間のターゲットの位置を保存
                //characterAnim.SetBool("ShotFlag", false);
                TurnFlag = true;
                HavingBallNum--;    //持っている雪玉の数を1減らす
                var rnd = Random.Range(1, 11);　// ※ 1～10の範囲でランダムな整数値が返る
                if (rnd == 1 || rnd == 2)    //ランダムな確率で弾を発射しない状態に移行
                {
                    //ChangeState(State.doNothing);
                    //yield break;
                }
            }
            yield return new WaitForSeconds(0.5f);    //0.5秒待機
        }
    }*/
    private void BallShot()
    {
            if (isVisible)
            {
                if (IntFlag)    //アニメーションを起動する一回だけ起動
                { 
                    characterAnim.Play("Snowman_double_Throw");
                    TurnFlag = false;   //投げるときは向きを固定
                    ball = (GameObject)Instantiate(ballPrefab, childObj.transform.position, Quaternion.identity);
                    ballRigidbody = ball.GetComponent<Rigidbody>();
                    ball.transform.parent = childObj.gameObject.transform;
                    IntFlag = false;
                }
                AnimInfo = characterAnim.GetCurrentAnimatorStateInfo(0);
            if (!OnceFlag)
                if (AnimInfo.normalizedTime >= 0.7f && AnimInfo.IsName("Snowman_double_Throw"))    //アニメーションが7割再生されたら射撃
                {
                    ShotFlag = true;
                    OnceFlag = true;
                }
                if(AnimInfo.normalizedTime >= 1f && AnimInfo.IsName("Snowman_double_Throw"))   //終了したら
                {
                    characterAnim.Play("Snowman_double_Idle");
                    TurnFlag = true;    //向きの固定解除
                    ShotFlag = false;
                    IntFlag = true;
                    OnceFlag = false;
                }
                    
                if (ShotFlag)
                {
                    ShotDir = RandomAngle(Bayesian()); //ベイズ推定
                    ball.transform.parent = null;
                    ballRigidbody.AddForce(ShotDir.normalized * speed, ForceMode.Impulse);
                    AreaInfos[AreaNum].Count++;   //投げる瞬間のターゲットの位置を保存
                    
                    HavingBallNum--;    //持っている雪玉の数を1減らす
                /*var rnd = Random.Range(1, 11);　// ※ 1～10の範囲でランダムな整数値が返る
                if (rnd == 1 || rnd == 2)    //ランダムな確率で弾を発射しない状態に移行
                {
                    //ChangeState(State.doNothing);
                    //yield break;
                }*/
                    ShotFlag = false;
                }
                preShotTime = stateTime;
            }
    }

    private Vector3 RandomAngle(int Area)  //視界内でランダムにボールを投げる
    {
        Debug.Log(Area);
        if (Area == 0)
        {
            var Deg = Mathf.Acos(CosDiv1) * Mathf.Rad2Deg;
            var rand = Random.Range(0, Deg);    //0～θ/6の範囲でランダムな角度を獲得
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
        if (Area == 2)
        {
            var MaxDeg = Mathf.Acos(CosDiv3) * Mathf.Rad2Deg;
            var MinDeg = Mathf.Acos(CosDiv2) * Mathf.Rad2Deg;
            var rand = Random.Range(MinDeg, MaxDeg);    //MinDeg～MaxDigの範囲でランダムな角度を獲得
            var RandRad = rand * Mathf.Deg2Rad; //degをradに変換
            //var RandomDir = transform.forward + new Vector3(Mathf.Sin(RandRad), 0, Mathf.Cos(RandRad));   //ランダムな発射方向
            var RandomDir = Quaternion.Euler(0, rand, 0) * transform.forward;   //ランダムな発射方向
            return RandomDir;
        }
        if (Area == 3)
        {
            var Deg = Mathf.Acos(CosDiv1) * Mathf.Rad2Deg;
            var rand = Random.Range(-Deg, 0);    //-θ/6～0の範囲でランダムな角度を獲得
            var RandRad = rand * Mathf.Deg2Rad; //degをradに変換
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
        float CountSum = 0;
        for(int i = 0; i < AreaInfos.Length; i++)
        {
            CountSum += AreaInfos[i].Count;
        }
        if (CountSum != 0)
        {
            for (int i = 0; i < AreaInfos.Length; i++)
            {
                var likelihood = AreaInfos[i].Count / CountSum; //尤度を求める
                AreaInfos[i].After_Probabillity = AreaInfos[i].Probabillity * likelihood; //事後確率を求める
                After_ProbabillitySum += AreaInfos[i].After_Probabillity;  //事後確率の総和を求める
            }
            for (int i = 0; i < AreaInfos.Length; i++)
            {
                AreaInfos[i].After_Probabillity = AreaInfos[i].After_Probabillity / After_ProbabillitySum;   //各事後確率の正規化
                if (AreaInfos[i].After_Probabillity >= 0.5f) //確率分布が収束していたら
                {

                    ShotArea = i;   //常に確率が最も高いエリアを格納
                    After_ProbabillitySum = 0;
                    return ShotArea;
                }

            }
            float rand = float.MaxValue;   //0.0~1.0を取得
            for (int i = 0; i < AreaInfos.Length; i++)
            {
                Temp_prob += AreaInfos[i].After_Probabillity;   //各エリアの確率を順番に足す
                if (Temp_prob >= rand) //和がrandを超えた時点でのエリアを返す
                {
                    ShotArea = i;
                    break;
                }
            }
            //初期化
            After_ProbabillitySum = 0;
            Temp_prob = 0;
        }   
        else  //データがない時
        {
            float rand = float.MaxValue;   //0.0~1.0を取得
            for (int i = 0; i < AreaInfos.Length; i++)
            {
                Temp_prob += AreaInfos[i].Probabillity;   //各エリアの確率を順番に足す
                if (Temp_prob >= rand) //和がrandを超えた時点でのエリアを返す
                {
                    ShotArea = i;
                    break;
                }
            }
        }
        return ShotArea;   //確率が最も高いエリアを返す
    }


     private void SetFlag()
    {
        TurnFlag = true;    //向きの固定解除
        ShotFlag = false;
        IntFlag = true;
        OnceFlag = false;
    }
    private IEnumerator CalcArriveTime()    //移動予測地点のエリアを取得
    {
        var Distance = Vector3.Distance(Self.position, Target.position);   //ターゲットとの距離を計算
        var ArrivalTime = Distance / speed; //到着までの時間を計算
        yield return new WaitForSeconds(ArrivalTime);   //到着するまで待機
        AreaInfos[AreaNum].Count++;   //その時点でのエリアを記憶
    }

    private void GotoNextPoint()
    {
        //NavMeshAgentのストップを解除
        agent.isStopped = false;

        //ランダムな地点を取得
        float posX = Random.Range(1, 20);
        float posY = Random.Range(1, 20);

        Vector3 direction = new Vector3(posX, this.transform.position.y, posY);

        Quaternion rotation =
            Quaternion.LookRotation(direction - transform.position, Vector3.up);
        //このオブジェクトの向きを替える
        transform.rotation = rotation;

        agent.destination = direction;
        WalkFlag = false;
    }

    private void StopHere()
    {
        agent.isStopped = true;
        time += Time.deltaTime;

        if (time > waitTime)
        {
            GotoNextPoint();
            time = 0;

        }
    }
}