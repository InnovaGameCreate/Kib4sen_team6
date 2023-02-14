using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Test : MonoBehaviour
{
    private Rigidbody myrigidbody;
    [SerializeField]
    private GameObject ballPrefab;
    Vector2 displayCenter;
    private Transform pos;
    private GameObject block;
    const int DOWN = 0;
    int Remaining = 10;

    // Start is called before the first frame update
    void Start()
    {
        displayCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
    }
    private void PlayerMove()
    {
        if (Input.GetMouseButton(0))
        {
            if(Remaining > 0)
            {
                ShotSnowBall();
                Remaining--;
            }
        }
        if (Input.GetMouseButton(1))
        {
            ShotRay();
            if(Remaining < 10)
            {
                Remaining++;
            }
        }
    }
    
    private void ShotSnowBall()
    {
        Vector3 ShotPos = this.transform.position + new Vector3(1f, 0, 1f);
        Instantiate(ballPrefab, ShotPos, Quaternion.identity);
    }
    
    private void ShotRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(displayCenter);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            pos = hit.collider.gameObject.transform;   //rayの当たったオブジェクトの座標を取得
            block = hit.collider.gameObject;
            MapManager.instance.ChangeBlock(block, pos, DOWN);
            
        }
    }
}
