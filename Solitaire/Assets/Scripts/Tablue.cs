using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static GameState;

public class Tablue : CardGroup
{
    [SerializeField]
    private int Index;
    public int InitialHiddenCardsCount => Math.Max(0, Index);
    public List<CardSpace> ColumnSpaces = new();
    public List<CardSpace> MoveableCards;
    public List<CardSpace> GetMoveableCards() => ColumnSpaces.Where(c => c.CardData?.IsFaceUp ?? false)?.ToList();
    public List<CardSpace> GetHiddenCards() => ColumnSpaces.Where(c => !c.CardData?.IsFaceUp ?? false).ToList();
    public void Awake()
    {
        ColumnSpaces = GetComponentsInChildren<CardSpace>().ToList();
    }
     void Start()
    {
        base.Start();

        EventManager.AddCardListener(CardAdded);
        EventManager.AddCardListListener(CardListAdded);

        EventManager.CardLeftListener(CardLeft);
        EventManager.CardLeftListListener(CardListLeft);

        EventManager.CardBackChanged(UpdateHiddenSprites);
    }

    #region Event Methods
    private void CardLeft(CardSpace card, CardGroup src)
    {
        if (src != this) return; 
        if (TopCard != card.CardData) return; //Should only remove top single card

        CardList.Remove(card.CardData);

        ColumnSpaces.Remove(card);
        Destroy(card.gameObject);

        if (CardList.Count == 0)
        {
            foreach (Transform child in transform) Destroy(child.gameObject);
            IsEmpty = true;

            ColumnSpaces.Clear();
            CardList.Clear();
            TopCard = null;

            var emptyCard = Resources.Load("Empty");
            var space = Instantiate((GameObject)emptyCard, gameObject.transform);
            space.transform.position = new Vector3(transform.position.x, 0, transform.position.z);

            ColumnSpaces.Add(space.GetComponent<CardSpace>());

            var img = ColumnSpaces[0].GetComponent<Image>();
            img.sprite = ColumnSpaces[0].CardData.GetEmptySprite();
            return;
        }

        TopCard = CardList.Last();
        TopCard.IsFaceUp = true;
        ColumnSpaces.Last().CardData.IsFaceUp = true;
        cardTurned.Invoke();


        var nextCard = ColumnSpaces.Last();
        ColumnSpaces.Last().CardData.IsFaceUp = true;
        ColumnSpaces.Last().SetCardSprite();
    }
    private void CardListLeft(List<CardSpace> cards, CardGroup src)
    {
        if (src != this) return;
        
        foreach(CardSpace card in cards)
        {
            CardList.Remove(CardList.FirstOrDefault(c => c == card.CardData));
            
            ColumnSpaces.Remove(card);
            Destroy(card.gameObject);
            
        }
        
        if(CardList.Count == 0)
        {

            IsEmpty = true;

            ColumnSpaces.Clear();
            CardList.Clear();
            TopCard = null;

            var emptyCard = Resources.Load("Empty");
            var space = Instantiate((GameObject)emptyCard, gameObject.transform);
            space.transform.position = new Vector3(transform.position.x, 0, transform.position.z);

            ColumnSpaces.Add(space.GetComponent<CardSpace>());

            var img = ColumnSpaces[0].GetComponent<Image>();
            img.sprite = ColumnSpaces[0].CardData.GetEmptySprite();
            return;
        }

        TopCard = CardList.Last();
        TopCard.IsFaceUp = true;
        ColumnSpaces.Last().CardData.IsFaceUp = true;
        cardTurned.Invoke();


        var nextCard = ColumnSpaces.Last();
        ColumnSpaces.Last().CardData.IsFaceUp = true;
        ColumnSpaces.Last().SetCardSprite();
    }
    private void CardAdded(CardSpace card, CardGroup dst)
    {
        if (dst != this) return;

        var board = Camera.main.GetComponent<Board>();
        if (board is null) return;
        if (!board.ValidCardMove(card, this)) return;


        if (IsEmpty)
        {
            ColumnSpaces.Clear();
            CardList.Clear();

            IsEmpty = false;
            foreach (Transform child in transform) Destroy(child.gameObject);
        }

        CardList.Add(card.CardData);

        var emptyCard = Resources.Load("Empty");
        var space = Instantiate((GameObject)emptyCard, gameObject.transform);
        space.transform.position = new Vector3(transform.position.x, (33 * (Index + 1)) * -1, transform.position.z);
        space.name = $"{card.CardData.CardValue} of {card.CardData.CardSuit}";

        var spaceData = space.GetComponent<CardSpace>();
        spaceData.CardData = card.CardData;
        spaceData.IsOccupied = true;
        card.CardData.IsFaceUp = true;
        spaceData.SetCardSprite();
        ColumnSpaces.Add(spaceData);

        TopCard = CardList.Last();
        TopCard.IsFaceUp = true;

        recievedCard.Invoke(card, card.GetComponentInParent<Tablue>());
    }
    private void CardListAdded(List<CardSpace> cards, CardGroup dst)
    {
        if (dst != this) return;

        var board = Camera.main.GetComponent<Board>();
        if (board is null) return;
        if (!board.ValidCardMove(cards[0], this)) return;


        if (IsEmpty)
        {
            IsEmpty = false;
            ColumnSpaces.Clear();
            CardList.Clear();
            foreach (Transform child in transform) Destroy(child.gameObject);
            
        }

        foreach(CardSpace card in cards)
        {
            CardList.Add(card.CardData);

            var emptyCard = Resources.Load("Empty");
            var space = Instantiate((GameObject)emptyCard, gameObject.transform);
            space.transform.position = new Vector3(transform.position.x, (33 * (Index+1)) *-1, transform.position.z);
            space.name = $"{card.CardData.CardValue} of {card.CardData.CardSuit}";

            var spaceData = space.GetComponent<CardSpace>();
            spaceData.CardData = card.CardData;
            spaceData.IsOccupied = true;
            card.CardData.IsFaceUp = true;
            spaceData.SetCardSprite();
            ColumnSpaces.Add(spaceData);
        }

        TopCard = CardList.Last();
        TopCard.IsFaceUp = true;


        recievedList.Invoke(cards, cards[0].GetComponentInParent<Tablue>());
    }
    private void  UpdateHiddenSprites() => GetHiddenCards().ForEach(x=> x.SetCardSprite());    
    
    #endregion Event methods

    #region Initialization
    public void Initialize()
    {
        FillColumn();
        DrawCards(CardList, Deck);
        MoveableCards = ColumnSpaces.Where(c => c.CardData?.IsFaceUp ?? false)?.ToList();
    }

    public static void DrawCards(List<CardData> cardList, List<CardData> fromList)
    {
        foreach (CardData card in cardList)
        {

            DrawCard(card, fromList);
        }
    }

    private void FillColumn()
    {
        AddHiddenCards();
        AddTopCard();
    }

    private void AddHiddenCards()
    {
        var hiddenCards = Deck.BuildUniqueCardListToSize(InitialHiddenCardsCount);
        CardList.AddRange(hiddenCards);
        for (int i = 0; i < InitialHiddenCardsCount; i++)
        {
            var emptyCard = Resources.Load("Empty");
            var space = Instantiate<GameObject>((GameObject)emptyCard, gameObject.transform);
            space.transform.position = new Vector3(transform.position.x, ((63 * (i+1)) * -1), transform.position.z);
            space.name = $"{CardList[i].CardValue} of {CardList[i].CardSuit}";
            
            var spaceScript = space.GetComponent<CardSpace>();
            spaceScript.CardData = CardList[i];
            spaceScript.IsOccupied = true;
            spaceScript.SetCardSprite();
            ColumnSpaces.Add(spaceScript);
            IsEmpty = false;
        }
    }

    private void AddTopCard()
    {
        IsEmpty = false;
        TopCard = CardList.GetNewCardNoRepeat(Deck);
        CardList.Add(TopCard);

        var emptyCard = Resources.Load("Empty");
        var space = Instantiate(emptyCard, new Vector3(transform.position.x, ((63 * (Index+1)) * -1), transform.position.z), Quaternion.identity, gameObject.transform);
        space.name = $"{TopCard.CardValue} of {TopCard.CardSuit}";

        var lastSpace = space.GetComponent<CardSpace>();
        
        lastSpace.CardData = TopCard;
        lastSpace.Expose();
        lastSpace.IsOccupied = true;
        lastSpace.SetCardSprite();

        ColumnSpaces.Add(lastSpace);
    }

    private void HideRemainingSpaces()
    {
        var emptyChildren = ColumnSpaces.Where(c=> !c.IsOccupied).ToList();
        foreach (var space in emptyChildren)
        {
            var img = space.GetComponent<Image>();
        }
    }
    #endregion Initialization
    public override string ToString() => $"Tab index: {Index},\nCard Count: {TotalCount}, " +
        $"{InitialHiddenCardsCount} cards face down, {TopCard?.ToString()?.Split(",")?.FirstOrDefault()} Face up";
}
