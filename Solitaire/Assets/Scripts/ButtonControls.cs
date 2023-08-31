using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControls : MonoBehaviour
{
    int currentScreenHeight;
    int currentScreenWidth;
    int fullScreenHeight; 
    int fullScreenWidth;

    bool isMaximized = false;

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [SerializeField]
    GameObject GameOptions;
    [SerializeField]
    GameObject CardOptions;
    [SerializeField]
    TMP_Dropdown GameDropDown;
    [SerializeField]
    TMP_Dropdown HelpDropDown;
    [SerializeField]
    Sprite[] Sprites;
    void Start()
    {
        currentScreenHeight = Screen.height;
        currentScreenWidth = Screen.width;
        fullScreenHeight = Screen.currentResolution.height;
        fullScreenWidth = Screen.currentResolution.width;
        GameDropDown.captionText.text = "Game";
        HelpDropDown.captionText.text = "Help";
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
        isMaximized = !isMaximized;
        GameObject.Find("Expand").transform.GetChild(0).GetComponent<Image>().sprite = isMaximized ? Sprites[1] : Sprites[0];
    #if UNITY_EDITOR
            var windows = (UnityEditor.EditorWindow[])Resources.FindObjectsOfTypeAll(typeof(UnityEditor.EditorWindow));
            foreach (var window in windows)
            {
                if (window != null && window.GetType().FullName == "UnityEditor.GameView")
                {
                    window.maximized = isMaximized;
                    break;
                }
            }

            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    #else

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
#endif
    }

    public void MinimizeButton()
    {
        ShowWindow(GetActiveWindow(), 2);
    }

    public void ShowGameOptionsWindow()
    {
        GameOptions.SetActive(true);
        CardOptions.SetActive(false);
    }

    public void ShowCardOptionsWindow()
    {
        CardOptions.SetActive(true);
        GameOptions.SetActive(false);
    }
    public void HideGameOptionsWindow()
    {
        GameOptions.SetActive(false);
    }

    public void HideCardOptionsWindow()
    {
        CardOptions.SetActive(false);
    }

    public void GameDropDownSelect()
    {

        Debug.Log(GameDropDown.value);  

        switch (GameDropDown.value)
        {
            case 0: Camera.main.GetComponent<Board>().NewGame(); break;
            case 1: ShowCardOptionsWindow(); break;
            case 2: ShowGameOptionsWindow(); break;
            case 3: CloseButton(); break;
        }

        GameDropDown.SetValueWithoutNotify(-1);
        GameDropDown.captionText.text = "Game";
    }
    public void HelpDropDownSelect()
    {
        switch (HelpDropDown.value)
        {
            case 0: HelpDropDown.captionText.text = "Help"; return;
            case 1: HelpDropDown.captionText.text = "Help"; break;
        }
    }
}
