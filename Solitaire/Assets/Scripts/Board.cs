using System.Collections.Generic;
using UnityEngine;
using static GameState;

public class Board : MonoBehaviour
{
    public const int FOUNDATION_COUNT = 4;
    public const int TAB_COUNT = 7;

    public static DrawDeck DrawDeck;
    public static DiscardPile DiscardPile;
    public static ScoreManager ScoreManager;
    
    [SerializeField]
    Timer Timer;
    [SerializeField]
    GameObject CardOptionsWindow;
    [SerializeField]
    GameObject GameOptionsWindow;

    public List<Tablue> Tablues = new();
    public List<Foundation> Foundations = new();

    public static Board Instance { get; private set; }

    private int maxedFoundations = 0;

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

    void Start()
    {
        GameState.GenerateDeck();
        DrawDeck = GameObject.FindGameObjectWithTag("DrawDeck")?.GetComponent<DrawDeck>();
        DiscardPile = GameObject.FindGameObjectWithTag("DiscardPile")?.GetComponent<DiscardPile>();
        InitializeTablues();
        InitializeFoundations();
        DrawDeck.Initialize();
        ScoreManager = GameObject.FindGameObjectWithTag("ScoreManager")?.GetComponent<ScoreManager>();
        ScoreManager.AddAsListener();

        EventManager.FoundationMaxed(FoundationMaxed);

    }
    private void FoundationMaxed()
    {
        maxedFoundations++;
        if (maxedFoundations == FOUNDATION_COUNT)
        {
            Debug.Log("Game won, Score " + ScoreManager.GetFinalScore());
            Timer.Stop();
        }
    }
    private void InitializeTablues()
    {
        var tabColumns = GameObject.FindGameObjectsWithTag("Column");
        for (int i = 0; i < TAB_COUNT; i++)
        {
            var t = tabColumns[i].GetComponent<Tablue>();
            t.Initialize();
            Tablues.Add(t);
        }
    }
    private void InitializeFoundations()
    {
        var foundations = GameObject.FindGameObjectsWithTag("Foundation");
        for (int i = 0; i < FOUNDATION_COUNT; i++) 
        {
            var f = foundations[i].GetComponent<Foundation>();
            Foundations.Add(f);
        }
    }

    public void NewGame()
    {
        CardOptionsWindow.SetActive(false);
        GameOptionsWindow.SetActive(false);
        GenerateDeck();
        DiscardPile.ResetGroup();
        foreach (Transform child in DiscardPile.transform) Destroy(child.gameObject);
        DiscardPile.UpdateSprite();
        DrawDeck.ResetGroup();
        DrawDeck.UpdateSprite();

        foreach (var f in Foundations) 
        {
            f.ResetGroup();
            f.UpdateSprite();
            foreach (Transform child in f.transform) Destroy(child.gameObject);
        }
        foreach (var t in Tablues)
        {
            t.ResetGroup();
            foreach (Transform child in t.transform) Destroy(child.gameObject);
        }
        InitializeTablues();
        InitializeFoundations();
        DrawDeck.Initialize();

        Timer.InitializeTimer();
        ScoreManager.InitializeScore();
    }
    public bool ValidCardMove(CardSpace card, CardGroup dst) => dst.tag switch
    {
        "CardSpace" or "Column" => ValidateSlot(card, dst), 
        "Foundation" => ValidateFoundation(card, dst),
        "DiscardPile" => true,
        _ => false
    };
    public bool ValidateFoundation(CardSpace card, CardGroup dst)
    {
        var src = card.GetComponentInParent<CardGroup>();
        if (src == dst) return false;
        if (card.CardData != src.TopCard) return false;
        if (dst.IsEmpty)
        {
            if (card.CardData.CardValue is not (CardValues)MIN_CARD_VALUE) return false;
        }
        else
        {
            if (card.CardData.CardSuit != ((Foundation)dst).Suit) return false;
            if (dst.TopCard.CardValue is (CardValues)MAX_CARD_VALUE) return false;
            if (card.CardData.CardValue != dst.TopCard.CardValue.Next()) return false;
        }
        return true;
    }
    public bool ValidateSlot(CardSpace card, CardGroup dst)
    {
        if (card.GetComponentInParent<CardGroup>() == dst) return false;

        if (dst.IsEmpty)
        {
            if (card.CardData.CardValue is not (CardValues)MAX_CARD_VALUE) return false;
        }
        else
        {
            if (dst.TopCard.CardValue is (CardValues)MIN_CARD_VALUE) return false;//Ace is bottom
            if (card.CardData.CardValue != dst.TopCard.CardValue.Previous()) return false;// val -1
            if ((card.CardData.CardColor == dst.TopCard.CardColor)) return false;//red/black
        }
        return true;
    }
}


