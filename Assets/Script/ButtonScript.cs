using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonScript : MonoBehaviour
{
    
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

    public void ReturnTitle()
    {

    }
}
