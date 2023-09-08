using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static GameState;

public class WinManager : MonoBehaviour
{
    [SerializeField]
    GameObject EscapeHintText;
    [SerializeField]
    List<BounceAnimation> foundationAnimation;
    [SerializeField]
    GameObject WinDialogue;

    bool animationsStarted;
    bool animationsFinished;

    Board board;

    public static WinManager Instance { get; private set; }
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
        board = Camera.main.GetComponent<Board>();
    }
    private void FixedUpdate()
    {
        if (!board.IsGameOver) return;

        if (foundationAnimation.Any(x => x.IsAnimating) && !animationsStarted) { animationsStarted = true; animationsFinished = false; }
        if (foundationAnimation.All(x => !x.IsAnimating) && !animationsFinished) AnimationEnd();
        if (animationsStarted && !animationsFinished && Input.GetKeyDown(KeyCode.Escape)) AnimationEnd();
    }

    void AnimationEnd()
    {
        animationsFinished = true;
        animationsStarted = false;

        EscapeHintText.SetActive(false);
        foreach(var anim in foundationAnimation) anim.ResetAnimaiton();
        // Won();
        SetScore();
        WinDialogue.SetActive(true);
    }

    void SetScore()
    {
        var labels = WinDialogue.GetComponentsInChildren<TextMeshProUGUI>();
        labels.First(l => l.name == "ScoreValue").text = ScoreManager.FinalScore.ToString("N0");
        labels.First(l => l.name == "TimeValue").text =  GameObject.Find("Timer").GetComponent<Timer>().TotalTime.ToString("N0");
        ScoreManager.CalculateBonus();
        labels.First(l => l.name == "BonusValue").text = ScoreManager.BonusScore.ToString("N0");
        labels.First(l => l.name == "TotalValue").text = ScoreManager.GetFinalScore().ToString("N0");
    }
    public void StartAnimations()
    {
        if (!board.IsGameOver) return;

        for (int i = 0; i < Board.FOUNDATION_COUNT; i++)
        {
            foundationAnimation[i].StartAnimating((CardSuits)i);
        }
    }
    public void Won()
    {
        if (!board.IsGameOver) return;

        foreach (var anim in foundationAnimation) anim.ResetAnimaiton();
        EscapeHintText.SetActive(true);
        StartAnimations();
    }

    public void ResetAnimations()
    {
        WinDialogue.SetActive(false);
        foreach (var a in foundationAnimation) a.ResetAnimaiton();
    }
}
