using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Transform Self; //���g
    [SerializeField]
    private Transform Target;   //�^�[�Q�b�g
    [SerializeField]
    private float SightAngle;   //����p
    [SerializeField]
    private float MaxDistance = float.PositiveInfinity; //���E�̍ő勗��
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
        var SelfPos = Self.position;    //���g�̈ʒu

        TargetPos = Target.position;    //�^�[�Q�b�g�̈ʒu

        var SelfDir = Self.forward; //���g�̌���

        var TargetDir = TargetPos - SelfPos;    //�������猩���G�̕���
        var TargetDistance = TargetDir.magnitude;   //�^�[�Q�b�g�Ƃ̋���

        var CosHalf = Mathf.Cos(SightAngle / 2 * Mathf.Deg2Rad);    //cos(��/2)���v�Z
        var InnerProduct = Vector3.Dot(SelfDir, TargetDir.normalized);  //���ς��v�Z


        return InnerProduct > CosHalf && TargetDistance < MaxDistance;  //�p�x���肩��������
    }

    private void Start()
    {
        childObj = transform.GetChild(0).gameObject;    //�ŏ��̎q�I�u�W�F�N�g�̍��W���擾
        StartCoroutine("BallShot");
    }

    // ���E����̌��ʂ�GUI�o��
    private void Update()
    {
        // ���E����
        isVisible = IsVisible();
        if (isVisible)
        {
            TurnToTarget();
        }

    }

    private  void TurnToTarget()    //�^�[�Q�b�g�̕�������
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(Target.transform.position - transform.position),
            1);
    }


    private IEnumerator BallShot()  //0.5�b�Ԋu�Ŕ���
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
