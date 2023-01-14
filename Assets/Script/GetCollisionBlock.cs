using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCollisionBlock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)  //衝突した地面ブロックの位置を取得
    {
        if(collision.gameObject.tag == "Grand")
        {
            MapManager.instance.ChangeBlock(collision.gameObject, collision.gameObject.transform);
        }
    }
}
