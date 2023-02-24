using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBall : MonoBehaviour
{
    const int UP = 1;   //ê∑ÇËè„Ç™ÇÈéûÇÃÉtÉâÉO
    [SerializeField]
    private float LifeTime;
    public GameObject particle;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, LifeTime);
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
            Debug.Log("hit");
        var pos = collision.transform.position + collision.transform.forward;
        Instantiate(particle, pos, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
