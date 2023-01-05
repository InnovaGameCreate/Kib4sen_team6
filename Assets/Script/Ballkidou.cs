using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballkidou : MonoBehaviour
{
    private GameObject snowball;
    [SerializeField] private float speed;
    private Vector3 pos;
    private Ray kidou;
    private Vector3 accel;
    [SerializeField] private float waittime;
    private Vector3 force;
    [SerializeField] float t;

    // Start is called before the first frame update
    void Start()
    {
        pos = new Vector3(Screen.width / 2, Screen.height / 2, 0.1f); //投げる方向の調整
        kidou = Camera.main.ScreenPointToRay(pos);
    }

    // Update is called once per frame
    void Update()
    {
        //雪玉にくっつけるスクリプト

        force = kidou.direction * speed;
        this.gameObject.GetComponent<Rigidbody>().AddForce(force); //雪玉が真っすぐ飛んでいく

        StartCoroutine(wait(kidou));
    }

    private IEnumerator wait(Ray kidou)
    {
        yield return new WaitForSeconds(waittime); //waittime秒後に直線運動をやめて落下を始める

        for (int i = 0; i < t; i++)  //徐々に減速
        {
            yield return new WaitForSeconds(0.1f);
            accel = new Vector3(kidou.direction.x * speed * -1 / t, kidou.direction.y * speed * -1 / t, kidou.direction.z * speed * -1 / t); 
            this.gameObject.GetComponent<Rigidbody>().AddForce(accel);
        }

        while (this.transform.position.y > 0)  //雪玉のy座標が0になるまでy軸方向の負の方向に加速
        {
            yield return new WaitForSeconds(0.1f);
            fall(kidou);
        }
    }

    void fall(Ray kidou)
    {
        accel = new Vector3(0, -1, 0);
        this.gameObject.GetComponent<Rigidbody>().AddForce(accel);
    }

    private void OnCollisionEnter(Collision collision)  //雪玉が何かに衝突したら消える
    {
        Destroy(this.gameObject);
    }

}
