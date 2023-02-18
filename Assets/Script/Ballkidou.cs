using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballkidou : MonoBehaviour
{
    //雪玉にくっつけるスクリプト
    private GameObject snowball;
    [SerializeField] private float speed;
    private Vector3 pos;
    private float tyusinkyori;
    [SerializeField] private float okuyuki; //画面中心の奥行き。カメラのxの角度の値が0以下の時どれくらいの奥行きで雪玉が画面中心を通るかの値
    [SerializeField] private float sitanageRot; //カメラのxの角度がこの値以上になったら地面に沿うように雪玉を投げなくなる
    private float x;
    private Vector3 uiballpos;
    private Ray kidou;
    private Vector3 accel;
    [SerializeField] private float waittime;
    private Vector3 force;
    [SerializeField] private float gensoku; //最終的にどれ位減速するか
    [SerializeField] private float gravity; //重力の数値
    [SerializeField] private float gkasokukankaku; //どれくらいの時間の間隔で減速するか
    [SerializeField] [Tooltip("BreakSnowEffect")] private ParticleSystem particle;
    private ParticleSystem effinst;
    const int UP = 1;   //盛り上がる時のフラグ(SnowBallのものと同一)

    // Start is called before the first frame update
    void Start()   //雪玉が画面中心に飛んでいくための前準備
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
        kidou = new Ray(transform.position, pos - transform.position);   //雪玉の発射位置から画面中心方向に飛んでいくray

        force = kidou.direction * speed;
        if (0 < Camera.main.transform.rotation.x && Camera.main.transform.rotation.x < sitanageRot*Mathf.PI / 180f)
        {
            force.y = 0;
            Debug.Log("a");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
        if (other.gameObject.tag == "Grand") //SnowBallと同一のもの
        {
            MapManager.instance.ChangeBlock(other.gameObject, other.gameObject.transform, UP);  //ブロックの設置
        }
        Destroy(this.gameObject);
    }
}
