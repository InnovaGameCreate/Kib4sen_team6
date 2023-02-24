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
    private float waitTime;
    [SerializeField]
    private int HP;
    [SerializeField]
    private float distance; //��������������͈�
    private bool isVisible;
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
    public struct AreaInfo
    {
        public float Probabillity; //���̃G���A�̎��O�m��
        public float After_Probabillity;
        public float Count; //���̃G���A�ɂ�����

    }

    private float After_ProbabillitySum; //����m���̑���
    AreaInfo[] AreaInfos = new AreaInfo[6]; //�e�G���A�̏��

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
        HP = 10;
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;  //�ړI�n�ɋ߂Â��Ƃ��ɑ��x�������Ȃ��悤�ɂ���
        agent.updateRotation = false;   //��]���Ȃ��悤�ɂ���
        OnceFlag = false;
        IntFlag = true;
        TurnFlag = true;
        TargetRigid = Target.GetComponent<Rigidbody>();
        HavingBallNum = 10;
        for (int i = 0; i < AreaInfos.Length; i++)
        {
            AreaInfos[i].Probabillity = 1f / 6f; //���O�m����������
            AreaInfos[i].After_Probabillity = 1f / 6f;
            AreaInfos[i].Count = 0f;
        }
        characterAnim = GetComponent<Animator>();
    }

    // ���E����̌��ʂ�GUI�o��
    private void Update()
    {
        stateTime += Time.deltaTime;    //���݂̃X�e�[�g�ɂȂ��Ă���̎��Ԃ��v��
        // ���E����
        isVisible = IsVisible();
        StateManager();
        if (Input.GetKey(KeyCode.I))
        {
            HP = 0;
            if (HP == 0)
            {
                ChangeState(State.Dead);
            }
        }

    }
    public bool IsVisible() //���E�ɓ����Ă��邩����
    {
        var SelfPos = Self.position;    //���g�̈ʒu
        var TargetPos = Target.position;    //�^�[�Q�b�g�̈ʒu

        var SelfDir = Self.forward; //���g�̌���

        var TargetDir = TargetPos - SelfPos;    //�������猩���G�̕���
        var TargetDirnol = TargetDir;
        TargetDirnol.y = 0f;
        TargetDistance = TargetDir.magnitude;   //�^�[�Q�b�g�Ƃ̋���

        var CosHalf = Mathf.Cos(SightAngle / 2 * Mathf.Deg2Rad);    //cos(��/2)���v�Z
        var InnerProduct = Vector3.Dot(SelfDir, TargetDirnol.normalized);  //���ς��v�Z


        CosDiv1 = Mathf.Cos(SightAngle / 6 * Mathf.Deg2Rad);    //cos(��/6)���v�Z
        CosDiv2 = Mathf.Cos(2 * SightAngle / 6 * Mathf.Deg2Rad);    //cos(2��/6)���v�Z
        CosDiv3 = Mathf.Cos(3 * SightAngle / 6 * Mathf.Deg2Rad);    //cos(3��/6)���v�Z

        var diff = TargetPos - SelfPos;

        var axis = Vector3.Cross(transform.forward, diff);  //�O�ς����߂�

        var angle = Vector3.Angle(transform.forward, diff) * (axis.y < 0 ? -1 : 1); //�^�[�Q�b�g�Ƃ̊p�x��-180�`180�ɕϊ�

        //�^�[�Q�b�g���ǂ̈ʒu�ɂ��邩���t���[���m�F
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

        Debug.Log("CosHalf" + CosHalf);
        Debug.Log("InnerProduct" + InnerProduct);
        NearFlag = TargetDistance <= distance;
        var visiable = InnerProduct > CosHalf && TargetDistance < MaxDistance;  //�p�x���肩��������
        if (visiable)
            visiable = JudgWall(TargetDir, TargetDistance);
        if(NearFlag)
            NearFlag = JudgWall(TargetDir, TargetDistance);

        return visiable;
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
                if (isVisible)
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

                if(!isVisible)  //�������ƒT���֖߂�
                {
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

    /*private IEnumerator BallShot()
    {
        while (true)
        {
            if (isVisible)
            {
                //characterAnim.SetBool("ShotFlag",true);   //������Ƃ��͌������Œ�
                TurnFlag = false;
                GameObject ball = (GameObject)Instantiate(ballPrefab, childObj.transform.position, Quaternion.identity);
                Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
                yield return new WaitForSeconds(0.3f);    //�ˌ�������0.3�b��~
                ShotDir = RandomAngle(Bayesian()); //�x�C�Y����
                //ShotDir = RandomAngle();
                ballRigidbody.AddForce(ShotDir.normalized * speed, ForceMode.Impulse);
                AreaInfos[AreaNum].Count++;   //������u�Ԃ̃^�[�Q�b�g�̈ʒu��ۑ�
                //characterAnim.SetBool("ShotFlag", false);
                TurnFlag = true;
                HavingBallNum--;    //�����Ă����ʂ̐���1���炷
                var rnd = Random.Range(1, 11);�@// �� 1�`10�͈̔͂Ń����_���Ȑ����l���Ԃ�
                if (rnd == 1 || rnd == 2)    //�����_���Ȋm���Œe�𔭎˂��Ȃ���ԂɈڍs
                {
                    //ChangeState(State.doNothing);
                    //yield break;
                }
            }
            yield return new WaitForSeconds(0.5f);    //0.5�b�ҋ@
        }
    }*/
    private void BallShot()
    {
            if (isVisible)
            {
                var CosDiv = CosDiv1 = Mathf.Cos(SightAngle / 12 * Mathf.Deg2Rad);    //cos(��/6)���v�Z
                var InnerProduct = Vector3.Dot(Self.forward, (Target.position - Self.position).normalized);  //���ς��v�Z
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
                if(AnimInfo.normalizedTime >= 1f && AnimInfo.IsName("Snowman_double_Throw"))   //�I��������
                {
                    characterAnim.Play("Snowman_double_Idle");
                    TurnFlag = true;    //�����̌Œ����
                    ShotFlag = false;
                    IntFlag = true;
                    OnceFlag = false;
                }
                    
                if (ShotFlag)
                {
                //ShotDir = RandomAngle(Bayesian()); //�x�C�Y����
                    ShotDir = AccuracyController(PredictShot());
                    ball.transform.parent = null;
                    ballRigidbody.AddForce(ShotDir.normalized * speed, ForceMode.Impulse);
                    ball.GetComponent<SphereCollider>().enabled = true;
                    AreaInfos[AreaNum].Count++;   //������u�Ԃ̃^�[�Q�b�g�̈ʒu��ۑ�
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

    private Vector3 RandomAngle(int Area)  //���E���Ń����_���Ƀ{�[���𓊂���
    {
        Debug.Log(Area);
        if (Area == 0)
        {
            var Deg = Mathf.Acos(CosDiv1) * Mathf.Rad2Deg;
            var rand = Random.Range(0, Deg);    //0�`��/6�͈̔͂Ń����_���Ȋp�x���l��
            var RandomDir = Quaternion.Euler(0, rand, 0) * transform.forward;   //�����_���Ȕ��˕���
            return RandomDir;
        }
        if (Area == 1)
        {
            var MaxDeg = Mathf.Acos(CosDiv2) * Mathf.Rad2Deg;
            var MinDeg = Mathf.Acos(CosDiv1) * Mathf.Rad2Deg;
            var rand = Random.Range(MinDeg, MaxDeg);    //MinDeg�`MaxDig�͈̔͂Ń����_���Ȋp�x���l��
            var RandomDir = Quaternion.Euler(0, rand, 0) * transform.forward;   //�����_���Ȕ��˕���
            return RandomDir;
        }
        if (Area == 2)
        {
            var MaxDeg = Mathf.Acos(CosDiv3) * Mathf.Rad2Deg;
            var MinDeg = Mathf.Acos(CosDiv2) * Mathf.Rad2Deg;
            var rand = Random.Range(MinDeg, MaxDeg);    //MinDeg�`MaxDig�͈̔͂Ń����_���Ȋp�x���l��
            //var RandomDir = transform.forward + new Vector3(Mathf.Sin(RandRad), 0, Mathf.Cos(RandRad));   //�����_���Ȕ��˕���
            var RandomDir = Quaternion.Euler(0, rand, 0) * transform.forward;   //�����_���Ȕ��˕���
            return RandomDir;
        }
        if (Area == 3)
        {
            var Deg = Mathf.Acos(CosDiv1) * Mathf.Rad2Deg;
            var rand = Random.Range(-Deg, 0);    //-��/6�`0�͈̔͂Ń����_���Ȋp�x���l��
            var RandomDir = Quaternion.Euler(0, rand, 0) * transform.forward;   //�����_���Ȕ��˕���
            return RandomDir;
        }
        if (Area == 4)
        {
            var MaxDeg = Mathf.Acos(CosDiv2) * Mathf.Rad2Deg;
            var MinDeg = Mathf.Acos(CosDiv1) * Mathf.Rad2Deg;
            var rand = Random.Range(MinDeg, MaxDeg);    //MinDeg�`MaxDig�͈̔͂Ń����_���Ȋp�x���l��
            var RandomDir = Quaternion.Euler(0, -rand, 0) * transform.forward;   //�����_���Ȕ��˕���
            return RandomDir;
        }
        if (Area == 5)
        {
            var MaxDeg = Mathf.Acos(CosDiv3) * Mathf.Rad2Deg;
            var MinDeg = Mathf.Acos(CosDiv2) * Mathf.Rad2Deg;
            var rand = Random.Range(MinDeg, MaxDeg);    //MinDeg�`MaxDig�͈̔͂Ń����_���Ȋp�x���l��
            var RandomDir = Quaternion.Euler(0, -rand, 0) * transform.forward;   //�����_���Ȕ��˕���
            return RandomDir;
        }

        return transform.forward;

    }

    private int Bayesian() //�x�C�Y����
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
                var likelihood = AreaInfos[i].Count / CountSum; //�ޓx�����߂�
                AreaInfos[i].After_Probabillity = AreaInfos[i].Probabillity * likelihood; //����m�������߂�
                After_ProbabillitySum += AreaInfos[i].After_Probabillity;  //����m���̑��a�����߂�
            }
            for (int i = 0; i < AreaInfos.Length; i++)
            {
                AreaInfos[i].After_Probabillity = AreaInfos[i].After_Probabillity / After_ProbabillitySum;   //�e����m���̐��K��
                if (AreaInfos[i].After_Probabillity >= 0.5f) //�m�����z���������Ă�����
                {

                    ShotArea = i;   //��Ɋm�����ł������G���A���i�[
                    After_ProbabillitySum = 0;
                    return ShotArea;
                }

            }
            float rand = float.MaxValue;   //0.0~1.0���擾
            for (int i = 0; i < AreaInfos.Length; i++)
            {
                Temp_prob += AreaInfos[i].After_Probabillity;   //�e�G���A�̊m�������Ԃɑ���
                if (Temp_prob >= rand) //�a��rand�𒴂������_�ł̃G���A��Ԃ�
                {
                    ShotArea = i;
                    break;
                }
            }
            //������
            After_ProbabillitySum = 0;
            Temp_prob = 0;
        }   
        else  //�f�[�^���Ȃ���
        {
            float rand = float.MaxValue;   //0.0~1.0���擾
            for (int i = 0; i < AreaInfos.Length; i++)
            {
                Temp_prob += AreaInfos[i].Probabillity;   //�e�G���A�̊m�������Ԃɑ���
                if (Temp_prob >= rand) //�a��rand�𒴂������_�ł̃G���A��Ԃ�
                {
                    ShotArea = i;
                    break;
                }
            }
        }
        return ShotArea;   //�m�����ł������G���A��Ԃ�
    }


     private void SetFlag()
    {
        TurnFlag = true;    //�����̌Œ����
        ShotFlag = false;
        IntFlag = true;
        OnceFlag = false;
    }

    /*
    private IEnumerator CalcArriveTime()    //�ړ��\���n�_�̃G���A���擾
    {
        var Distance = Vector3.Distance(Self.position, Target.position);   //�^�[�Q�b�g�Ƃ̋������v�Z
        var ArrivalTime = Distance / speed; //�����܂ł̎��Ԃ��v�Z
        yield return new WaitForSeconds(ArrivalTime);   //��������܂őҋ@
        AreaInfos[AreaNum].Count++;   //���̎��_�ł̃G���A���L��
    }*/

    /*
    private void GotoNextPoint()
    {
        //NavMeshAgent�̃X�g�b�v������
        agent.isStopped = false;

        //�����_���Ȓn�_���擾
        float posX = Random.Range(1, 20);
        float posY = Random.Range(1, 20);

        Vector3 direction = new Vector3(posX, this.transform.position.y, posY);

        Quaternion rotation =
            Quaternion.LookRotation(direction - transform.position, Vector3.up);
        //���̃I�u�W�F�N�g�̌�����ւ���
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
    */

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "SnowBall")
        {
            HP--;
            Debug.Log(HP);
            if(HP == 0)
            {
                ChangeState(State.Dead);
            }
        }
    }

    private Vector3 PredictShot()
    {
        var TargetPoint = Target.transform.position;    //�Ώۂ̈ʒu
        TargetPoint.y += 0.5f;
        var arrivalTime = Vector3.Distance(TargetPoint, ball.transform.position) / speed;   //�Ώۂɓ��B����܂ł̎���
        var TargetVelocity = new Vector3(TargetRigid.velocity.x, 0, TargetRigid.velocity.z);   //�������ւ̈ړ����x
        var predictionPosXZ = TargetVelocity * arrivalTime; //�ړ��������v�Z
        var predictionCharaPoint = TargetPoint + predictionPosXZ;   //�ړ���̈ʒu���v�Z
        var direction = (predictionCharaPoint - ball.transform.position).normalized;    //���˕���
        return direction;
    }

    private void RandumRotate() //�����_���ȕ���������
    {
        if (SerchTurnFlag)
        {
            Rand = Random.Range(-360, 360);
            Vector = Quaternion.Euler(0, Rand, 0) * transform.forward;
            Vector.y = 0f;
            SerchTurnFlag = false;
        }
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(Vector),
            0.5f);
        if(Vector3.Angle(transform.forward, Vector) <= 5f)
        {
            SerchTurnFlag = true;
        }
            
    }

    private void LookTargetDirection()
    {
        if (!isVisible && NearFlag)  //���E�ɂ͓����Ă��Ȃ��������������邮�炢�߂���
            TurnToTarget(0.03f);
        else if (isVisible) //���E�ɓ����Ă���Ƃ�
            TurnToTarget(0.5f);
    }

    private bool JudgWall(Vector3 Direction, float Distance) //�ǂ����邩����
    {
        Ray ray;
        RaycastHit hit;

        ray = new Ray(this.transform.position, Direction);  //�v���C���[�̕�����Ray���Ƃ΂�
        
        if(Physics.Raycast(ray.origin, ray.direction * Distance, out hit))
        {
            if(hit.collider.CompareTag("Player"))
            {
                //Debug.Log("������");
                return true;
            }
            else
            {
                //Debug.Log("�݂��Ȃ�");
                return false;
            }
        }
        return false;
        
    }

    private Vector3 AccuracyController(Vector3 Dir)   //�������x�ݒ�
    {
        float rnd = Random.Range(0f, 1f);�@// �� 1�`10�͈̔͂Ń����_���Ȑ����l���Ԃ�
        if(rnd >= 0.2f)
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