using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBall : MonoBehaviour
{
    const int UP = 1;   //����オ�鎞�̃t���O
    [SerializeField]
    private float LifeTime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, LifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(ballRigid.velocity.magnitude);�@//���x�\��
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            Debug.Log("hit");
        Destroy(this.gameObject);
        /*if (collision.gameObject.tag == "Grand")
        {
            MapManager.instance.ChangeBlock(collision.gameObject, collision.gameObject.transform, UP);  //�u���b�N�̐ݒu
        }*/
    }
}
