using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Test : MonoBehaviour
{
    private Rigidbody myrigidbody;
    [SerializeField]
    private GameObject ballPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();

    }
    private void PlayerMove()
    {
        if(Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(-0.01f, 0.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(0.01f, 0.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(0.0f, 0.0f, 0.01f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(0.0f, 0.0f, -0.01f);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            ShotSnowBall();
        }
    }
    
    private void ShotSnowBall()
    {
        Vector3 ShotPos = this.transform.position + new Vector3(0.5f, 0, 0.3f);
        Instantiate(ballPrefab, ShotPos, Quaternion.identity);
    }
}
