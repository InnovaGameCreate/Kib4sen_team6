using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballkidou : MonoBehaviour
{
    private GameObject snowball;
    [SerializeField] private float speed;
    private Vector3 pos;
    private float tyusinkyori;
    [SerializeField] private float okuyuki; //画面中心の奥行き
    private float x;
    private Vector3 uiballpos;
    private Ray kidou;
    private Vector3 accel;
    [SerializeField] private float waittime;
    private Vector3 force;
    [SerializeField] float gensoku;
    [SerializeField] float gravity;
    [SerializeField] float gkasokukankaku;
    [SerializeField] [Tooltip("BreakSnowEffect")] private ParticleSystem particle;
    private ParticleSystem effinst;

    // Start is called before the first frame update
    void Start()
    {
        Quaternion qua = Camera.main.transform.rotation;
        x = qua.eulerAngles.x * 3.14f / 180;
        if (Camera.main.transform.rotation.x > 0)
        {
            tyusinkyori = Camera.main.transform.position.y / Mathf.Sin(x);
        }
        else
        {
            tyusinkyori = okuyuki;
        }
        uiballpos = new Vector3(Screen.width / 2, Screen.height / 2, tyusinkyori);  //画面中心の座標
        pos = Camera.main.ScreenToWorldPoint(uiballpos);
        kidou = new Ray(transform.position, pos - transform.position) ;
    }

    // Update is called once per frame
    void Update()
    {
        //雪玉にくっつけるスクリプト
        force = kidou.direction * speed;
        force.y = 0;
        this.gameObject.GetComponent<Rigidbody>().AddForce(force); //雪玉が真っすぐ飛んでいく

        StartCoroutine(wait(kidou));
    }

    private IEnumerator wait(Ray kidou)
    {
        yield return new WaitForSeconds(waittime); //waittime秒後に直線運動をやめて落下を始める

        for (int i = 0; i < gensoku; i++)  //徐々に減速
        {
            yield return new WaitForSeconds(0.1f);
            accel = new Vector3(kidou.direction.x * speed * -1 / gensoku, 0, kidou.direction.z * speed * -1 / gensoku); 
            this.gameObject.GetComponent<Rigidbody>().AddForce(accel);
        }

        while (this.transform.position.y > 0)  //雪玉のy座標が0になるまでy軸方向の負の方向に加速
        {
            yield return new WaitForSeconds(gkasokukankaku);
            fall(kidou);
        }
    }

    void fall(Ray kidou)
    {
        accel = new Vector3(0, -gravity, 0);
        this.gameObject.GetComponent<Rigidbody>().AddForce(accel);
    }

    private void OnTriggerEnter(Collider other)
    {
        effinst = Instantiate(particle);
        effinst.transform.position = this.transform.position;
        effinst.Play();
        Destroy(effinst.gameObject, 3.0f);

        Destroy(this.gameObject);
    }
}
