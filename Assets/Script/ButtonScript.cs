using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Retry()
    {
        GameManager.instance.RetryButton();
    }

    public void End()
    {
        GameManager.instance.EndButton();
    }

    public void Title()
    {
        GameManager.instance.StartButton();
    }

    public void Tutorial()
    {
        GameManager.instance.TutorialButton();
    }
}
