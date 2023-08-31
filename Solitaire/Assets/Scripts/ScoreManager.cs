using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI ScoreTitle;

    private const int DISCARD_TO_TAB = 5;
    private const int DISCARD_TO_FOUNDATION = 10;
    private const int TAB_TO_FOUNDATION = 10;
    //private const int TAB_TO_TAB = 5;
    private const int EXPOSE_CARD_TAB = 5;
    private const int FOUNDATION_TO_TAB = -15; //Currently non-case
    private const int RESET_DISCARD_SINGLE = -100; //Conditional on single flip
    private const int RESET_DISCARD_THREE = -20; //Consitional on three card flip

    public static float FinalScore => Mathf.Max(0, currentScore);
    private static float currentScore;
    public static float BonusScore;
    public static Timer Timer;
    public static bool HasScoring;
    public static int StartingScore;//-52 for vegas
    //public static ScoringType Type; 

    public static ScoreManager Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

    }

    private void Start()
    {
        InitializeScore();
        ScoreTitle.text = currentScore.ToString();
        //Listen for waste -> tab
        //Listen for waste -> Found
        //Listen for Tab -> Found
        //Listen for cardUp
        //Listen for foundation to tab //Non case
        //listen for reset pile

    }
    public void AddAsListener()
    {
        EventManager.CardMovedFromDiscard(HandleDiscardMove);
        EventManager.CardExposed(HandleCardExposed);
        EventManager.DiscardReset(HandleDiscardReset);
        EventManager.AddCardListener(HandleCardMoved);
    }
    private void HandleCardMoved(CardSpace card, CardGroup dst)
    {
        var isFoundation = dst.GetComponentInParent<Foundation>() != null;
        var isTablue = dst.GetComponentInParent<Tablue>() != null;
        //Debug.Log("is foundation " + isFoundation + " id tab " + isTablue);
        if (!isFoundation && !isTablue) return;
        currentScore += isFoundation ? TAB_TO_FOUNDATION : DISCARD_TO_TAB;
        //Debug.Log("current score " + currentScore);
        ScoreTitle.text = currentScore.ToString();
    }

    private  void HandleDiscardReset()
    {
        currentScore += OptionsManager.DrawCount is DrawType.Single ? RESET_DISCARD_SINGLE : RESET_DISCARD_THREE;
        ScoreTitle.text = currentScore.ToString();
    }

    private void HandleCardExposed()
    {
        //Debug.Log("Exposed");
        currentScore += EXPOSE_CARD_TAB;
        ScoreTitle.text = currentScore.ToString();
    }

    private void HandleDiscardMove(CardSpace card, CardGroup dst)
    {
        var isFoundation = dst.GetComponentInParent<Foundation>() != null;
        var isTablue = dst.GetComponentInParent<Tablue>() != null;
        //Debug.Log("is foundation " + isFoundation+ " id tab " + isTablue);
        if (!isFoundation && !isTablue) return;
        currentScore += isFoundation ? DISCARD_TO_FOUNDATION : 0;
        //Debug.Log("current score " + currentScore);
        ScoreTitle.text = currentScore.ToString();
    }

    public void InitializeScore()
    {
       // Debug.Log("Initializing score");
        currentScore = 0;
        BonusScore = 0;
        HasScoring = true;// OptionsManager.HasStatusBar;
        StartingScore = OptionsManager.Scoring is ScoringType.Standard ? 0 : -52;
        ScoreTitle.text = currentScore.ToString();
    }
    public static void CalculateBonus()
    {
        if (Timer.TotalTime < 30) BonusScore= 0;
        BonusScore = 700000 / Timer.TotalTime;
    }
    public static float GetFinalScore()
    {
        //CalculateBonus();
        Debug.Log("getting score " + currentScore);
        currentScore += BonusScore;
        return FinalScore;
    }

}
