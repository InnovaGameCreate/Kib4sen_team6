using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
    [SerializeField,Range(30,180)]
    private int SightAngle;   //����p
    [SerializeField]
    private float MaxDistance = float.PositiveInfinity; //���E�̍ő勗��
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
    //private Vector3 PreTragetPos;   //1�t���[���O�̃^�[�Q�b�g�̈ʒu
    Vector3 Moving_Distance;    //1�b�Ԃ̈ړ���
    private bool TurnFlag;  //�^�[�Q�b�g�̕��������������邩
    float CosDiv1;
    float CosDiv2;
    float CosDiv3;
    public struct AreaInfo
    {
        public float Probabillity; //���̃G���A�̎��O�m��
        public float After_Probabillity;
        public float Count; //���̃G���A�ɂ�����

    }

    private float After_ProbabillitySum; //����m���̑���
    List<int> TargetMovedArea = new List<int>();    //�^�[�Q�b�g���ړ������G���A���i�[
    AreaInfo[] AreaInfos = new AreaInfo[7]; //�e�G���A�̏��


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

    public bool IsVisible() //���E�ɓ����Ă��邩����
    {
        var SelfPos = Self.position;    //���g�̈ʒu

        TargetPos = Target.position;    //�^�[�Q�b�g�̈ʒu

        var SelfDir = Self.forward; //���g�̌���

        var TargetDir = TargetPos - SelfPos;    //�������猩���G�̕���
        TargetDistance = TargetDir.magnitude;   //�^�[�Q�b�g�Ƃ̋���

        var CosHalf = Mathf.Cos(SightAngle / 2 * Mathf.Deg2Rad);    //cos(��/2)���v�Z
        var InnerProduct = Vector3.Dot(SelfDir, TargetDir.normalized);  //���ς��v�Z


        CosDiv1 = Mathf.Cos(SightAngle / 6 * Mathf.Deg2Rad);    //cos(��/6)���v�Z
        CosDiv2 = Mathf.Cos(2 * SightAngle / 6 * Mathf.Deg2Rad);    //cos(2��/6)���v�Z
        CosDiv3 = Mathf.Cos(3 * SightAngle / 6 * Mathf.Deg2Rad);    //cos(3��/6)���v�Z

        var diff = TargetPos - SelfPos;

        var axis = Vector3.Cross(transform.forward, diff);  //�O�ς����߂�

        var angle = Vector3.Angle(transform.forward, diff) * (axis.y < 0 ? -1 : 1); //�^�[�Q�b�g�Ƃ̊p�x��-180�`180�ɕϊ�

        /*
        Debug.Log(Mathf.Acos(CosDiv1) * Mathf.Rad2Deg);
        Debug.Log(Mathf.Acos(CosDiv2));
        Debug.Log(Mathf.Acos(CosDiv3));
        */
        
        //�^�[�Q�b�g���ǂ̈ʒu�ɂ��邩���t���[���m�F
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

        return InnerProduct > CosHalf && TargetDistance < MaxDistance;  //�p�x���肩��������
    }

    private void Start()
    {
        TurnFlag = true;
        line = this.GetComponent<LineRenderer>();
        TargetRigid = Target.GetComponent<Rigidbody>();
        HavingBallNum = 10;
        childObj = transform.GetChild(0).gameObject;    //�ŏ��̎q�I�u�W�F�N�g�̍��W���擾
        for(int i = 0; i < AreaInfos.Length; i++)
        {
            AreaInfos[i].Probabillity = 1f/7f; //���O�m����������
            AreaInfos[i].After_Probabillity = 1f/7f;
            AreaInfos[i].Count = 0f;
        }
    }

    // ���E����̌��ʂ�GUI�o��
    private void Update()
    {
        line.SetPosition(0, childObj.transform.position);
        line.SetPosition(1, transform.forward * 1000);
        
        stateTime += Time.deltaTime;    //���݂̃X�e�[�g�ɂȂ��Ă���̎��Ԃ��v��
        // ���E����
        isVisible = IsVisible();
        StateManager();

    }

    private  void TurnToTarget()    //�^�[�Q�b�g�̕�������
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
                TurnFlag = false;   //������Ƃ��͌������Œ�
                GameObject ball = (GameObject)Instantiate(ballPrefab, childObj.transform.position, Quaternion.identity);
                Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
                yield return new WaitForSeconds(0.5f);    //�ˌ�������0.5�b��~
                                                          //ballRigidbody.AddForce(transform.forward * speed, ForceMode.Impulse);
                if (TargetMovedArea.Count != 0)//1��ڈȊO�̎�
                    ShotDir = RandomAngle(Bayesian()); //�x�C�Y����
                else
                    ShotDir = transform.forward;
                
                //ShotDir = RandomAngle();
                ballRigidbody.AddForce(ShotDir.normalized * speed, ForceMode.Impulse);
                TargetMovedArea.Add(AreaNum);   //������u�Ԃ̃^�[�Q�b�g�̈ʒu��ۑ�
                TurnFlag = true;
                HavingBallNum--;    //�����Ă����ʂ̐���1���炷
                var rnd = Random.Range(1, 11);�@// �� 1�`10�͈̔͂Ń����_���Ȑ����l���Ԃ�
                if(rnd == 1 || rnd == 2)    //�����_���Ȋm���Œe�𔭎˂��Ȃ���ԂɈڍs
                {
                    //ChangeState(State.doNothing);
                   // yield break;
                }
            }
                yield return new WaitForSeconds(0.5f);    //1�b�ҋ@
        }
    }

    private void StateManager()
    {
        switch (currentState)
        {
            case State.Serch:
                if (stateEnter) //���̏�ԂɂȂ��Ă���ŏ��̃t���[���������s
                {}
                if (isVisible)
                {
                    ChangeState(State.Attack);
                    return;
                }
                break;
            case State.Attack:
                if (stateEnter) //���̏�ԂɂȂ��Ă���ŏ��̃t���[���������s
                {
                    shotCoroutine = StartCoroutine("BallShot");
                }
                if(TurnFlag)    //�t���O��true�Ȃ�
                    TurnToTarget(); //�^�[�Q�b�g�̕�������
                break;
            case State.doNothing:
                Debug.Log("��邱�ƂȂ��Ȃ���");
                break;
        }
    }

    private void LateUpdate()
    {
        if(stateTime != 0)
            stateEnter = false;
    }

    private Vector3 RandomAngle(int Area)  //���E���Ń����_���Ƀ{�[���𓊂���
    {
        Debug.Log(Area);
        if (Area == 0)
        {
           var Deg = Mathf.Acos(CosDiv1) * Mathf.Rad2Deg;
           var rand  = Random.Range(-Deg, Deg);    //-��/6�`��/6�͈̔͂Ń����_���Ȋp�x���l��
           var RandRad = rand * Mathf.Deg2Rad; //deg��rad�ɕϊ�
           var RandomDir = Quaternion.Euler(0, rand, 0) * transform.forward;   //�����_���Ȕ��˕���
            return RandomDir;
        }
        if (Area == 1)
        {
            var MaxDeg = Mathf.Acos(CosDiv2) * Mathf.Rad2Deg;
            var MinDeg = Mathf.Acos(CosDiv1) * Mathf.Rad2Deg;
            var rand = Random.Range(MinDeg, MaxDeg);    //MinDeg�`MaxDig�͈̔͂Ń����_���Ȋp�x���l��
            var RandRad = rand * Mathf.Deg2Rad; //deg��rad�ɕϊ�
            var RandomDir = Quaternion.Euler(0, rand, 0) * transform.forward;   //�����_���Ȕ��˕���
            return RandomDir;
        }
        if(Area == 2)
        {

            var MaxDeg = Mathf.Acos(CosDiv3) * Mathf.Rad2Deg;
            var MinDeg = Mathf.Acos(CosDiv2) * Mathf.Rad2Deg;
            var rand = Random.Range(MinDeg, MaxDeg);    //MinDeg�`MaxDig�͈̔͂Ń����_���Ȋp�x���l��
            var RandRad = rand * Mathf.Deg2Rad; //deg��rad�ɕϊ�
            //var RandomDir = transform.forward + new Vector3(Mathf.Sin(RandRad), 0, Mathf.Cos(RandRad));   //�����_���Ȕ��˕���
            var RandomDir = Quaternion.Euler(0, rand, 0) * transform.forward;   //�����_���Ȕ��˕���
            return RandomDir;
        }
        if (Area == 4)
        {
            var MaxDeg = Mathf.Acos(CosDiv2) * Mathf.Rad2Deg;
            var MinDeg = Mathf.Acos(CosDiv1) * Mathf.Rad2Deg;
            var rand = Random.Range(MinDeg, MaxDeg);    //MinDeg�`MaxDig�͈̔͂Ń����_���Ȋp�x���l��
            var RandRad = rand * Mathf.Deg2Rad; //deg��rad�ɕϊ�
            var RandomDir = Quaternion.Euler(0, -rand, 0) * transform.forward;   //�����_���Ȕ��˕���
            return RandomDir;
        }
        if (Area == 5)
        {
            var MaxDeg = Mathf.Acos(CosDiv3) * Mathf.Rad2Deg;
            var MinDeg = Mathf.Acos(CosDiv2) * Mathf.Rad2Deg;
            var rand = Random.Range(MinDeg, MaxDeg);    //MinDeg�`MaxDig�͈̔͂Ń����_���Ȋp�x���l��
            var RandRad = rand * Mathf.Deg2Rad; //deg��rad�ɕϊ�
            var RandomDir = Quaternion.Euler(0, -rand, 0) * transform.forward;   //�����_���Ȕ��˕���
            return RandomDir;
        }

        return transform.forward;
        
    }

    private int Bayesian() //�x�C�Y����
    {
        var CountNum = TargetMovedArea.Count - 1;
        AreaInfos[TargetMovedArea[CountNum]].Count++;   //����܂łɊe�G���A������ʂ������J�E���g
        for (int i = 0; i < AreaInfos.Length; i++)
        {
            var likelihood = AreaInfos[i].Count / (float)TargetMovedArea.Count; //�ޓx�����߂�
            AreaInfos[i].After_Probabillity = AreaInfos[i].Probabillity * likelihood; //����m�������߂�
            After_ProbabillitySum += AreaInfos[i].After_Probabillity;  //����m���̑��a�����߂�
        }
        for (int i = 0; i < AreaInfos.Length; i++)
        {
            AreaInfos[i].After_Probabillity = AreaInfos[i].After_Probabillity / After_ProbabillitySum;   //����m���̐��K��
            if (i == 0)
                ShotArea = 0; 
            else
            {
                if (AreaInfos[i].After_Probabillity > AreaInfos[ShotArea].After_Probabillity)
                    ShotArea = i;   //��Ɋm�����ł������G���A���i�[
            }
        }
        After_ProbabillitySum = 0;
        return ShotArea;   //�m�����ł������G���A��Ԃ�
    }


}
