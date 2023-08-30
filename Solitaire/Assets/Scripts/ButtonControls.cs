using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonControls : MonoBehaviour
{
    int currentScreenHeight;
    int currentScreenWidth;
    int fullScreenHeight; 
    int fullScreenWidth;

    bool isMaximized = false;

    void Start()
    {
        currentScreenHeight = Screen.height;
        currentScreenWidth = Screen.width;
        fullScreenHeight = Screen.currentResolution.height;
        fullScreenWidth = Screen.currentResolution.width;

    }
    public void CloseButton() 
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void MaximizeButton()
    {
        var toHeight = Screen.height;
        var toWidth = Screen.width;

        if(!isMaximized)
        {
            currentScreenHeight = toHeight;
            currentScreenWidth = toWidth;

            toHeight = fullScreenHeight;
            toWidth = fullScreenWidth;
        }
        
        isMaximized = !isMaximized;

        Screen.SetResolution(toWidth, toHeight, false);
    }
}
