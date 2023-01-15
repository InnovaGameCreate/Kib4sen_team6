using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBall : MonoBehaviour
{
    const int UP = 1;   //����オ�鎞�̃t���O
    private Vector3 pos; //�u���b�N��ݒu����ʒu
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
        if (collision.gameObject.tag == "Grand")
        {
            MapManager.instance.ChangeBlock(collision.gameObject, collision.gameObject.transform, UP);
        }
    }
}
