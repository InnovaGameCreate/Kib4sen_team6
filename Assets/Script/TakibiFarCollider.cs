using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakibiFarCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grand") && MapManager.instance.TakibiFarAroundChange(other.gameObject.transform) > -5)
            MapManager.instance.ChangeBlock(other.gameObject, other.gameObject.transform, 2);
    }
}
