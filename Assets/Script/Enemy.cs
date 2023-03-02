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
    private int HP;
    [SerializeField]
    private float distance; //足音が聞こえる範囲
    private bool Visible;
    private float preShotTime;
    private Coroutine shotCoroutine;
    private float TargetDistance;
    private Rigidbody TargetRigid;
    private Vector3 ShotDir;
    private int AreaNum;
    private int ShotArea;
    private bool TurnFlag;  //ターゲットの方向を向き続けるか
    private bool SerchTurnFlag;
    float CosDiv1;
    float CosDiv2;
    float CosDiv3;
    float Temp_prob;
    bool DamageFlag;
    bool IntFlag;
    bool ShotFlag;
    bool OnceFlag;
    bool WalkFlag;
    bool NearFlag; //足音が聞こえる範囲にいるか
    float Rand;
    Vector3 Vector;
    float time;
    GameObject ball;
    Rigidbody ballRigidbody;
    private NavMeshAgent agent;

    private Animator characterAnim;
    private AnimatorStateInfo AnimInfo;

    enum State
    {
        Serch,
        Attack,
        Reload,
        Dead,
        doNothing,
    }

    private State currentState = State.Serch;
    private bool stateEnter = false;
    private float stateTime = 0f;


    private void Start()
    {
        Target = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;  //目的地に近づくときに速度が落ちないようにする
        agent.updateRotation = false;   //回転しないようにする
        OnceFlag = false;
        IntFlag = true;
        TurnFlag = true;
        DamageFlag = false;
        TargetRigid = Target.GetComponent<Rigidbody>();
        HavingBallNum = 10;
        characterAnim = GetComponent<Animator>();
    }

    // 視界判定の結果をGUI出力
    private void Update()
    {
        stateTime += Time.deltaTime;    //現在のステートになってからの時間を計測
        // 視界判定
        Visible = IsVisible();
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



        //ターゲットがどの位置にいるか毎フレーム確認



        NearFlag = TargetDistance <= distance;
        var visible = InnerProduct > CosHalf && TargetDistance < MaxDistance;  //角度判定かつ距離判定
        if (visible)
        {
            JudgWall(TargetDir, TargetDistance);
        }
        if (NearFlag)
            JudgWall(TargetDir, TargetDistance);

        return visible;
    }

    private void TurnToTarget(float t)    //ターゲットの方を向く
    {
        var Vector = Target.position - transform.position;
        Vector.y = 0f;
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(Vector),
            t);

    }

    private void StateManager()
    {
        Debug.Log(currentState);
        switch (currentState)
        {
            case State.Serch:
                if (stateEnter) //この状態になってから最初のフレームだけ実行
                {
                    /*
                    GotoNextPoint();    //ランダムな地点を取得
                    */
                    characterAnim.Play("Snowman_double_Idle");
                }
                /*
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    StopHere();
                */
                LookTargetDirection();
                if (Visible)
                {
                    ChangeState(State.Attack);
                    return;
                }
                else
                {
                    //RandumRotate();
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

                    preShotTime = 0f;
                    ChangeState(State.Reload);
                    //shotCoroutine = StopCoroutine("BallShot");
                }

                if (TurnFlag)
                    LookTargetDirection(); //ターゲットの方を向く

                if (!Visible)  //見失うと探索へ戻る
                {
                    Debug.Log("Qq");
                    ChangeState(State.Serch);
                }

                break;
            case State.doNothing:
                Debug.Log("やることなくなった");
                break;
            case State.Reload:
                if (stateEnter) //この状態になってから最初のフレームだけ実行
                    characterAnim.Play("Snowman_double_Idle");
                if (stateTime >= ReloadTime)
                {
                    HavingBallNum = 10;
                    ChangeState(State.Serch);
                }
                break;
            case State.Dead:
                if (stateEnter) //この状態になってから最初のフレームだけ実行
                {
                    characterAnim.Play("Snowman_double_Defeat");
                }
                AnimInfo = characterAnim.GetCurrentAnimatorStateInfo(0);
                if (AnimInfo.normalizedTime >= 1f && AnimInfo.IsName("Snowman_double_Defeat"))   //終了したら
                {
                    this.gameObject.SetActive(false);
                    GameManager.instance.EnemyDeath();
                }
                break;
        }
    }

    void ChangeState(State _nextState)
    {
        //agent.isStopped = true;
        currentState = _nextState;
        stateEnter = true;
        stateTime = 0f;
    }

    private void LateUpdate()
    {
        if (stateTime != 0)
            stateEnter = false;
    }

    private void BallShot()
    {
        if (Visible)
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
            {
                if (AnimInfo.normalizedTime >= 0.7f && AnimInfo.IsName("Snowman_double_Throw"))    //アニメーションが7割再生されたら射撃
                {
                    ShotFlag = true;
                    OnceFlag = true;
                }
            }
            if (AnimInfo.normalizedTime >= 1f && AnimInfo.IsName("Snowman_double_Throw"))   //終了したら
            {
                characterAnim.Play("Snowman_double_Idle");
                SetFlag();
            }

            if (ShotFlag)
            {
                //ShotDir = RandomAngle(Bayesian()); //ベイズ推定
                ShotDir = AccuracyController(PredictShot());
                ball.transform.parent = null;
                ballRigidbody.AddForce(ShotDir.normalized * speed, ForceMode.Impulse);
                ball.GetComponent<SphereCollider>().enabled = true;
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




    private void SetFlag()
    {
        TurnFlag = true;    //向きの固定解除
        ShotFlag = false;
        IntFlag = true;
        OnceFlag = false;
        DamageFlag = false;
    }


    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "SnowBall")
        {
            HP--;
            if (!Visible)   //視界に入っていなければ
                DamageFlag = true;
            if (HP == 0)
            {
                ChangeState(State.Dead);
            }
        }
    }

    private Vector3 PredictShot()
    {
        var TargetPoint = Target.transform.position;    //対象の位置
        float rndpos = Random.Range(0.5f, 1.7f);
        TargetPoint.y += rndpos;
        var arrivalTime = Vector3.Distance(TargetPoint, ball.transform.position) / speed;   //対象に到達するまでの時間
        var TargetVelocity = new Vector3(TargetRigid.velocity.x, 0, TargetRigid.velocity.z);   //横方向への移動速度
        var predictionPosXZ = TargetVelocity * arrivalTime; //移動距離を計算
        var predictionCharaPoint = TargetPoint + predictionPosXZ;   //移動後の位置を計算
        var direction = (predictionCharaPoint - ball.transform.position).normalized;    //発射方向
        return direction;
    }

    private void LookTargetDirection()
    {
        if (!Visible && NearFlag)  //視界には入っていないが音が聞こえるぐらい近い時
            TurnToTarget(0.03f);
        else if (Visible) //視界に入っているとき
            TurnToTarget(0.5f);
        else if (DamageFlag)    //ダメージを受けたとき
            TurnToTarget(0.1f);
    }

    private void JudgWall(Vector3 Direction, float Distance) //壁があるか判定
    {
        Ray ray;
        RaycastHit hit;
        var pos = Self.position;
        pos.y = 1.5f;
        ray = new Ray(pos, Direction);  //プレイヤーの方向にRayをとばす

        if (Physics.Raycast(ray.origin, ray.direction * Distance, out hit))
        {
            Debug.Log(hit.collider.gameObject.layer);
            if (hit.collider.CompareTag("Player"))
            {
                //Debug.Log("見える");
                Visible = true;
            }
            else
            {
                //Debug.Log("みえない");
                Visible = false;
                NearFlag = false;
            }

        }

    }

    private Vector3 AccuracyController(Vector3 Dir)   //命中精度設定
    {
        float rnd = Random.Range(0f, 1f);　// ※ 1～10の範囲でランダムな整数値が返る
        if (rnd >= 0.4f)
        {
            Vector3 RandomDir;
            var pm = Random.Range(0, 2);
            if (pm == 0)
                RandomDir = Quaternion.Euler(0, rnd, 0) * Dir;   //ランダムな発射方向
            else
                RandomDir = Quaternion.Euler(0, -rnd, 0) * Dir;
            return RandomDir;
        }
        return Dir;
    }
}