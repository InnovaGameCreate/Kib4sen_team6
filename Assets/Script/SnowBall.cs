using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBall : MonoBehaviour
{
    const int UP = 1;   //盛り上がる時のフラグ
    [SerializeField]
    private float LifeTime;
    //private Vector3 prePosition;
    ///private Vector3 startPosition;
    //public float Timer;
    //private bool Flag;
    // Start is called before the first frame update
    void Start()
    {
        //startPosition = this.transform.position;
        Destroy(this.gameObject, LifeTime);
        //prePosition = this.transform.position;
        //Timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Vector3.Distance(this.transform.position, prePosition) >= 0.5)
        {
            Flag = true;
        }*/
        //Debug.Log(ballRigid.velocity.magnitude);　//速度表示
        /*if (Flag)
        {
            Timer += Time.deltaTime;
            if(Timer >= 1f)
            {
                Debug.Log(Vector3.Distance(this.transform.position, startPosition));
            }
        }
        */
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            Debug.Log("hit");
        Destroy(this.gameObject);
        /*if (collision.gameObject.tag == "Grand")
        {
            MapManager.instance.ChangeBlock(collision.gameObject, collision.gameObject.transform, UP);  //ブロックの設置
        }*/
    }
}
