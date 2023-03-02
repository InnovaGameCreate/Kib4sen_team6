using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private int HavingBallNum;
    [SerializeField]
    private Transform Self; //���g
    [SerializeField]
    private Transform Target;   //�^�[�Q�b�g
    [SerializeField]
    GameObject childObj;
    public GameObject ballPrefab;


    [Header("�G�̃p�����[�^")]
    [SerializeField, Range(30, 180)]
    private int SightAngle;   //����p
    [SerializeField]
    private float MaxDistance = float.PositiveInfinity; //���E�̍ő勗��
    [SerializeField]
    private int speed;
    [SerializeField]
    private float ReloadTime;
    [SerializeField]
    private int HP;
    [SerializeField]
    private float distance; //��������������͈�
    private bool Visible;
    private float preShotTime;
    private Coroutine shotCoroutine;
    private float TargetDistance;
    private Rigidbody TargetRigid;
    private Vector3 ShotDir;
    private int AreaNum;
    private int ShotArea;
    private bool TurnFlag;  //�^�[�Q�b�g�̕��������������邩
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
    bool NearFlag; //��������������͈͂ɂ��邩
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
        agent.autoBraking = false;  //�ړI�n�ɋ߂Â��Ƃ��ɑ��x�������Ȃ��悤�ɂ���
        agent.updateRotation = false;   //��]���Ȃ��悤�ɂ���
        OnceFlag = false;
        IntFlag = true;
        TurnFlag = true;
        DamageFlag = false;
        TargetRigid = Target.GetComponent<Rigidbody>();
        HavingBallNum = 10;
        characterAnim = GetComponent<Animator>();
    }

    // ���E����̌��ʂ�GUI�o��
    private void Update()
    {
        stateTime += Time.deltaTime;    //���݂̃X�e�[�g�ɂȂ��Ă���̎��Ԃ��v��
        // ���E����
        Visible = IsVisible();
        StateManager();

    }
    public bool IsVisible() //���E�ɓ����Ă��邩����
    {
        var SelfPos = Self.position;    //���g�̈ʒu
        var TargetPos = Target.position;    //�^�[�Q�b�g�̈ʒu

        var SelfDir = Self.forward; //���g�̌���

        var TargetDir = TargetPos - SelfPos;    //�������猩���G�̕���
        TargetDistance = TargetDir.magnitude;   //�^�[�Q�b�g�Ƃ̋���

        var CosHalf = Mathf.Cos(SightAngle / 2 * Mathf.Deg2Rad);    //cos(��/2)���v�Z
        var InnerProduct = Vector3.Dot(SelfDir, TargetDir.normalized);  //���ς��v�Z

        CosDiv1 = Mathf.Cos(SightAngle / 6 * Mathf.Deg2Rad);    //cos(��/6)���v�Z
        CosDiv2 = Mathf.Cos(2 * SightAngle / 6 * Mathf.Deg2Rad);    //cos(2��/6)���v�Z
        CosDiv3 = Mathf.Cos(3 * SightAngle / 6 * Mathf.Deg2Rad);    //cos(3��/6)���v�Z



        //�^�[�Q�b�g���ǂ̈ʒu�ɂ��邩���t���[���m�F



        NearFlag = TargetDistance <= distance;
        var visible = InnerProduct > CosHalf && TargetDistance < MaxDistance;  //�p�x���肩��������
        if (visible)
        {
            JudgWall(TargetDir, TargetDistance);
        }
        if (NearFlag)
            JudgWall(TargetDir, TargetDistance);

        return visible;
    }

    private void TurnToTarget(float t)    //�^�[�Q�b�g�̕�������
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
                if (stateEnter) //���̏�ԂɂȂ��Ă���ŏ��̃t���[���������s
                {
                    /*
                    GotoNextPoint();    //�����_���Ȓn�_���擾
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
                if (stateEnter) //���̏�ԂɂȂ��Ă���ŏ��̃t���[���������s
                {
                    SetFlag();
                    //shotCoroutine = StartCoroutine("BallShot");
                }

                //if(stateTime - preShotTime >= Shot_CD)  //���Ԋu�Ŏˌ�
                BallShot();

                if (HavingBallNum <= 0) //�c�e�����Ȃ��Ȃ����烊���[�h
                {

                    preShotTime = 0f;
                    ChangeState(State.Reload);
                    //shotCoroutine = StopCoroutine("BallShot");
                }

                if (TurnFlag)
                    LookTargetDirection(); //�^�[�Q�b�g�̕�������

                if (!Visible)  //�������ƒT���֖߂�
                {
                    Debug.Log("Qq");
                    ChangeState(State.Serch);
                }

                break;
            case State.doNothing:
                Debug.Log("��邱�ƂȂ��Ȃ���");
                break;
            case State.Reload:
                if (stateEnter) //���̏�ԂɂȂ��Ă���ŏ��̃t���[���������s
                    characterAnim.Play("Snowman_double_Idle");
                if (stateTime >= ReloadTime)
                {
                    HavingBallNum = 10;
                    ChangeState(State.Serch);
                }
                break;
            case State.Dead:
                if (stateEnter) //���̏�ԂɂȂ��Ă���ŏ��̃t���[���������s
                {
                    characterAnim.Play("Snowman_double_Defeat");
                }
                AnimInfo = characterAnim.GetCurrentAnimatorStateInfo(0);
                if (AnimInfo.normalizedTime >= 1f && AnimInfo.IsName("Snowman_double_Defeat"))   //�I��������
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
            if (IntFlag)    //�A�j���[�V�������N�������񂾂��N��
            {
                characterAnim.Play("Snowman_double_Throw");
                TurnFlag = false;   //������Ƃ��͌������Œ�
                ball = (GameObject)Instantiate(ballPrefab, childObj.transform.position, Quaternion.identity);
                ballRigidbody = ball.GetComponent<Rigidbody>();
                ball.transform.parent = childObj.gameObject.transform;
                IntFlag = false;
            }
            AnimInfo = characterAnim.GetCurrentAnimatorStateInfo(0);
            if (!OnceFlag)
            {
                if (AnimInfo.normalizedTime >= 0.7f && AnimInfo.IsName("Snowman_double_Throw"))    //�A�j���[�V������7���Đ����ꂽ��ˌ�
                {
                    ShotFlag = true;
                    OnceFlag = true;
                }
            }
            if (AnimInfo.normalizedTime >= 1f && AnimInfo.IsName("Snowman_double_Throw"))   //�I��������
            {
                characterAnim.Play("Snowman_double_Idle");
                SetFlag();
            }

            if (ShotFlag)
            {
                //ShotDir = RandomAngle(Bayesian()); //�x�C�Y����
                ShotDir = AccuracyController(PredictShot());
                ball.transform.parent = null;
                ballRigidbody.AddForce(ShotDir.normalized * speed, ForceMode.Impulse);
                ball.GetComponent<SphereCollider>().enabled = true;
                HavingBallNum--;    //�����Ă����ʂ̐���1���炷
                /*var rnd = Random.Range(1, 11);�@// �� 1�`10�͈̔͂Ń����_���Ȑ����l���Ԃ�
                if (rnd == 1 || rnd == 2)    //�����_���Ȋm���Œe�𔭎˂��Ȃ���ԂɈڍs
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
        TurnFlag = true;    //�����̌Œ����
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
            if (!Visible)   //���E�ɓ����Ă��Ȃ����
                DamageFlag = true;
            if (HP == 0)
            {
                ChangeState(State.Dead);
            }
        }
    }

    private Vector3 PredictShot()
    {
        var TargetPoint = Target.transform.position;    //�Ώۂ̈ʒu
        float rndpos = Random.Range(0.5f, 1.7f);
        TargetPoint.y += rndpos;
        var arrivalTime = Vector3.Distance(TargetPoint, ball.transform.position) / speed;   //�Ώۂɓ��B����܂ł̎���
        var TargetVelocity = new Vector3(TargetRigid.velocity.x, 0, TargetRigid.velocity.z);   //�������ւ̈ړ����x
        var predictionPosXZ = TargetVelocity * arrivalTime; //�ړ��������v�Z
        var predictionCharaPoint = TargetPoint + predictionPosXZ;   //�ړ���̈ʒu���v�Z
        var direction = (predictionCharaPoint - ball.transform.position).normalized;    //���˕���
        return direction;
    }

    private void LookTargetDirection()
    {
        if (!Visible && NearFlag)  //���E�ɂ͓����Ă��Ȃ��������������邮�炢�߂���
            TurnToTarget(0.03f);
        else if (Visible) //���E�ɓ����Ă���Ƃ�
            TurnToTarget(0.5f);
        else if (DamageFlag)    //�_���[�W���󂯂��Ƃ�
            TurnToTarget(0.1f);
    }

    private void JudgWall(Vector3 Direction, float Distance) //�ǂ����邩����
    {
        Ray ray;
        RaycastHit hit;
        var pos = Self.position;
        pos.y = 1.5f;
        ray = new Ray(pos, Direction);  //�v���C���[�̕�����Ray���Ƃ΂�

        if (Physics.Raycast(ray.origin, ray.direction * Distance, out hit))
        {
            Debug.Log(hit.collider.gameObject.layer);
            if (hit.collider.CompareTag("Player"))
            {
                //Debug.Log("������");
                Visible = true;
            }
            else
            {
                //Debug.Log("�݂��Ȃ�");
                Visible = false;
                NearFlag = false;
            }

        }

    }

    private Vector3 AccuracyController(Vector3 Dir)   //�������x�ݒ�
    {
        float rnd = Random.Range(0f, 1f);�@// �� 1�`10�͈̔͂Ń����_���Ȑ����l���Ԃ�
        if (rnd >= 0.4f)
        {
            Vector3 RandomDir;
            var pm = Random.Range(0, 2);
            if (pm == 0)
                RandomDir = Quaternion.Euler(0, rnd, 0) * Dir;   //�����_���Ȕ��˕���
            else
                RandomDir = Quaternion.Euler(0, -rnd, 0) * Dir;
            return RandomDir;
        }
        return Dir;
    }
}