using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakibiOndanColider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Grand")
           MapManager.instance.TakibiAroundChange(other.gameObject, other.gameObject.transform);
    }
}
