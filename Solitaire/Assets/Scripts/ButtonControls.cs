using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonControls : MonoBehaviour
{
    int currentScreenHeight;
    int currentScreenWidth;
    int fullScreenHeight; 
    int fullScreenWidth;

    bool isMaximized = false;
    public OnCardBackChanged backChanged = new();

    int lastSavedBack;
    DrawType lastDrawType;
    ScoringType lastScoringType;
    bool lastIsTimed;
    bool lastHasStatusBar;
    bool lastHasScoring;


    bool tempIsTimed;
    bool tempHasStatusBar;
    bool tempHasScoring;

    

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [SerializeField]
    GameObject GameOptions;
    [SerializeField]
    GameObject CardOptions;
    [SerializeField]
    GameObject StatusBar;
    [SerializeField]
    GameObject timer;
    [SerializeField]
    GameObject score;
    [SerializeField]
    TMP_Dropdown GameDropDown;
    [SerializeField]
    TMP_Dropdown HelpDropDown;
    [SerializeField]
    Sprite[] Sprites;
    [SerializeField]
    Toggle[] toggles;

    void Start()
    {
        currentScreenHeight = Screen.height;
        currentScreenWidth = Screen.width;
        fullScreenHeight = Screen.currentResolution.height;
        fullScreenWidth = Screen.currentResolution.width;
        GameDropDown.captionText.text = "Game";
        HelpDropDown.captionText.text = "Help";

        lastSavedBack = BoardSpriteManagement.CurrentCardSprite;
        lastDrawType = OptionsManager.DrawCount;
        lastScoringType= OptionsManager.Scoring;
        lastIsTimed = OptionsManager.TimedGame;
        lastHasStatusBar = OptionsManager.HasStatusBar;
        lastHasScoring = OptionsManager.Scoring != ScoringType.None;
      
        // temp
        tempIsTimed = OptionsManager.TimedGame;
        tempHasStatusBar = OptionsManager.HasStatusBar;
        tempHasScoring = OptionsManager.Scoring != ScoringType.None;

        EventManager.CardBackChangedInvoker(this);
    }

    #region WindowHeader
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
    #endregion

    #region DropDown
    public void ShowGameOptionsWindow()
    {
        toggles[0].SetIsOnWithoutNotify(OptionsManager.DrawCount is DrawType.Single);
        toggles[1].SetIsOnWithoutNotify(OptionsManager.DrawCount is DrawType.Three);
        toggles[2].SetIsOnWithoutNotify(OptionsManager.Scoring is ScoringType.Standard);
        toggles[3].SetIsOnWithoutNotify(OptionsManager.Scoring is ScoringType.Vegas);
        toggles[4].SetIsOnWithoutNotify(OptionsManager.Scoring is ScoringType.None);
        toggles[5].SetIsOnWithoutNotify(OptionsManager.TimedGame);
        toggles[6].SetIsOnWithoutNotify(OptionsManager.HasStatusBar);
        toggles[7].SetIsOnWithoutNotify(false);
        toggles[8].SetIsOnWithoutNotify(OptionsManager.Scoring != ScoringType.None);

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
        switch (GameDropDown.value)
        {
            case 0: GameDropDown.SetValueWithoutNotify(4); Camera.main.GetComponent<Board>().NewGame();  break;
            case 1: GameDropDown.SetValueWithoutNotify(4); ShowCardOptionsWindow(); break;
            case 2: GameDropDown.SetValueWithoutNotify(4); ShowGameOptionsWindow(); break;
            case 3: GameDropDown.SetValueWithoutNotify(4); CloseButton();  break;
        }

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
    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
    #endregion

    #region CardOptions
    public void AcceptCardBack()
    {
        lastSavedBack = BoardSpriteManagement.CurrentCardSprite;
        lastSavedBack.ChangeCardBack();
        HideCardOptionsWindow();
    }
    public void CancelCardBack()
    {
        lastSavedBack.ChangeCardBack();
        HideCardOptionsWindow();
        backChanged.Invoke();
    }
    public void BackSelected(int val)
    {
        val.ChangeCardBack();
        backChanged.Invoke();
    }
    #endregion

    #region GameOptions
    public void SelectDrawMode(int val)
    {
        var drawMode = val == 1 ? DrawType.Single : DrawType.Three;
        drawMode.SetDrawType();
    }
    public void SelectScoringMode(int val)
    {
        var scoringMode = val switch
        {
            0 => ScoringType.None,
            1 => ScoringType.Standard,
            2 => ScoringType.Vegas
        };
        scoringMode.SetScoringType();
        StatusBar.SetActive(OptionsManager.HasStatusBar);
    }
    public void GameToggles(int val)
    {
        switch (val)
        {
            case 0: tempIsTimed = !tempIsTimed; timer.SetActive(tempIsTimed); (tempIsTimed).SetTimedGame(); break;
            case 1: tempHasStatusBar= !tempHasStatusBar; StatusBar.SetActive(tempHasStatusBar); (tempHasStatusBar).SetHasStatusBar(); break;
            case 2: break;
            case 3: 
                tempHasScoring= !tempHasScoring;
                score.SetActive(tempHasScoring); 
                if (tempHasScoring) 
                    ScoringType.None.SetScoringType(); 
                else
                    lastScoringType.SetScoringType(); 
                break;
        };
    }
    public void AcceptGameOptions()
    {
        lastIsTimed = OptionsManager.TimedGame;
        timer.SetActive(lastIsTimed);

        lastHasScoring = OptionsManager.Scoring != ScoringType.None;
        score.SetActive(lastHasScoring);

        lastHasStatusBar = OptionsManager.HasStatusBar;
        StatusBar.SetActive(lastHasStatusBar);

        lastDrawType = OptionsManager.DrawCount;
        lastScoringType = OptionsManager.Scoring;

        HideGameOptionsWindow();
    }
    public void CancelGameOptions()
    {
        lastIsTimed.SetTimedGame();
        timer.SetActive(lastIsTimed);
        score.SetActive(lastHasScoring);
        lastHasStatusBar.SetHasStatusBar();
        StatusBar.SetActive(lastHasStatusBar);

        lastDrawType.SetDrawType();
        lastScoringType.SetScoringType();

        HideGameOptionsWindow();
    }
    #endregion

    public void AddListener(UnityAction handler)
    {
        backChanged.AddListener(handler);
    }
}
