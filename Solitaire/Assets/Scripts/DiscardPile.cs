using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DiscardPile : CardGroup
{
    private DrawDeck DrawDeck;
    private Board Board;
    private Image thisImage;
    int xOffset = 33; //in pixels

    public OnCardLeftDiscard<CardSpace, CardGroup> movedCard = new();
    public OnDiscardReset pileReset = new();
    public static DiscardPile PileInstance { get; private set; }

    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (PileInstance != null && PileInstance != this)
        {
            Destroy(this);
        }
        else
        {
            PileInstance = this;
        }

        Board = Camera.main.GetComponent<Board>();
        if (Board is null) return;

        CardList = new();
        IsEmpty= true;

        thisImage = GetComponent<Image>();
    }

    void Start()
    {
        DrawDeck = Board.DrawDeck;
        UpdateSprite();

        EventManager.AddCardListener(CardMoved);
        
        EventManager.DiscardInvoker(this);
        EventManager.DiscardResetInvoker(this);
    }

    private void CardMoved(CardSpace card, CardGroup dst)
    {
        if (!CardList.Contains(card.CardData)) return;
        CardList.Remove(card.CardData);
        if (CardList.Count == 0)
        {
            IsEmpty = true;
            TopCard = null;
            CardList.Clear();
            return;
        }

        TopCard = CardList.Last();

        if (OptionsManager.DrawCount is not DrawType.Single)
        {
           
            var topCardSpace = GameObject.Find($"{TopCard.CardValue} of {TopCard.CardSuit}");
            topCardSpace.GetComponent<Drag>().enabled = true;
        }
        Debug.Log("Discard after move top = " + TopCard);
    }
    public void ResetDiscardPile()
    {
        if (!DrawDeck.IsEmpty)
        {
            DrawDeck.DiscardCard();
            UpdateSprite();
            return;
        }

        if (CardList.Count == 0) return; 
        
        CardList.Reverse();
        DrawDeck.CardList.AddRange(CardList);
        DrawDeck.IsEmpty = false;
        DrawDeck.TopCard = DrawDeck.CardList.Last<CardData>();
        DrawDeck.TopCard.IsFaceUp = true;

        CardList.Clear();
        IsEmpty = true;
        UpdateSprite();

        pileReset.Invoke();

        foreach (Transform child in transform) Destroy(child.gameObject);
    }
    public void SpawnCard(int index)
    {
        var pos = OptionsManager.DrawCount is DrawType.Single ? transform.position : 
            new Vector3 (transform.position.x + (xOffset * index), transform.position.y, transform.position.z);
        var emptyCard = Resources.Load("CardUp");
        var space = Instantiate(emptyCard, pos, Quaternion.identity, gameObject.transform);
        space.name = $"{TopCard.CardValue} of {TopCard.CardSuit}";

        var spaceData = space.GetComponent<CardSpace>();
        spaceData.CardData = TopCard;
        spaceData.SetCardSprite();

        if (index+1 != OptionsManager.DrawCount.DrawCountAsValue())
            space.GetComponent<Drag>().enabled = false;

    }
    public void UpdateSprite() => thisImage.sprite = this.GetSpriteForDiscard();

    public void AddListener(UnityAction<CardSpace, CardGroup> handler)
    {
        movedCard.AddListener(handler);
    }
    public void AddListener(UnityAction handler)
    {
        pileReset.AddListener(handler);
    }
}
