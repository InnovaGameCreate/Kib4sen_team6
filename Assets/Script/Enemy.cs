using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Transform Self; //©g
    [SerializeField]
    private Transform Target;   //^[Qbg
    [SerializeField]
    private float SightAngle;   //ìp
    [SerializeField]
    private float MaxDistance = float.PositiveInfinity; //EÌÅå£
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
        var SelfPos = Self.position;    //©gÌÊu

        TargetPos = Target.position;    //^[QbgÌÊu

        var SelfDir = Self.forward; //©gÌü«

        var TargetDir = TargetPos - SelfPos;    //©ª©ç©½GÌûü
        var TargetDistance = TargetDir.magnitude;   //^[QbgÆÌ£

        var CosHalf = Mathf.Cos(SightAngle / 2 * Mathf.Deg2Rad);    //cos(Æ/2)ðvZ
        var InnerProduct = Vector3.Dot(SelfDir, TargetDir.normalized);  //àÏðvZ


        return InnerProduct > CosHalf && TargetDistance < MaxDistance;  //px»è©Â£»è
    }

    private void Start()
    {
        childObj = transform.GetChild(0).gameObject;    //ÅÌqIuWFNgÌÀWðæ¾
        StartCoroutine("BallShot");
    }

    // E»èÌÊðGUIoÍ
    private void Update()
    {
        // E»è
        isVisible = IsVisible();
        if (isVisible)
        {
            TurnToTarget();
        }

    }

    private  void TurnToTarget()    //^[QbgÌûðü­
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(Target.transform.position - transform.position),
            1);
    }


    private IEnumerator BallShot()  //0.5bÔuÅ­Ë
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
