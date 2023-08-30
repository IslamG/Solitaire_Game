using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
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

    public void Awake()
    {
        ColumnSpaces = GetComponentsInChildren<CardSpace>().ToList();
        //MoveableCards = ColumnSpaces.Where(c => c.CardData.IsFaceUp)?.ToList();
    }
     void Start()
    {
        base.Start();

        EventManager.AddCardListener(CardAdded);
        EventManager.AddCardListListener(CardListAdded);

        EventManager.CardLeftListener(CardLeft);
        EventManager.CardLeftListListener(CardListLeft);
    }

    #region Event Methods
    private void CardLeft(CardSpace card, CardGroup src)
    {
        //Debug.Log("Src " + src + " this " + this);
        if (src != this) return; 
        //if (!MoveableCards.Contains(card)) return;
        if (TopCard != card.CardData) return; //Should only remove top single card

        CardList.Remove(card.CardData);

        ColumnSpaces.Remove(card);
        //Debug.Log("Destroying " + card);
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
            //img.EnableImage(ColumnSpaces[0].CardData);
            return;
        }

        TopCard = CardList.Last();
        TopCard.IsFaceUp = true;
        ColumnSpaces.Last().CardData.IsFaceUp = true;
        cardTurned.Invoke();
        //Debug.Log("Card left Card last " + CardList.Last() + " top card " + TopCard + " card by index " + CardList[CardList.Count - 1]);


        var nextCard = ColumnSpaces.Last();
        //Debug.Log("Next card " + nextCard);
        ColumnSpaces.Last().CardData.IsFaceUp = true;
        ColumnSpaces.Last().SetCardSprite();
    }
    private void CardListLeft(List<CardSpace> cards, CardGroup src)
    {
        if (src != this) return;
        //if (!MoveableCards.Intersect(cards).Any()) return;
        //Debug.Log("count of cards left " + cards.Count);
        
        foreach(CardSpace card in cards)
        {
            CardList.Remove(CardList.FirstOrDefault(c => c == card.CardData));
            
            ColumnSpaces.Remove(card);
           // Debug.Log("Destroying " + card);
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
            //img.EnableImage(ColumnSpaces[0].CardData);
            return;
        }

        TopCard = CardList.Last();
        TopCard.IsFaceUp = true;
        ColumnSpaces.Last().CardData.IsFaceUp = true;
        cardTurned.Invoke();
        //Debug.Log("List left Card last " + CardList.Last() + " top card " + TopCard + " card by index " + CardList[CardList.Count - 1]);


        var nextCard = ColumnSpaces.Last();
        ColumnSpaces.Last().CardData.IsFaceUp = true;
        ColumnSpaces.Last().SetCardSprite();
    }
    private void CardAdded(CardSpace card, CardGroup dst)
    {
        if (dst != this) return;
        //if (dst.CompareTag("Column") && ((Tablue)dst).Index != Index) return; //destination is tab and is me
        //if (ColumnSpaces.Contains(card)) return; //Already in pile

        var board = Camera.main.GetComponent<Board>();
        if (board is null) return;
        if (!board.ValidCardMove(card, this)) return;

        //Debug.Log(this.name + " Added " + card);

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
        //Debug.Log("Card Added Card last " + CardList.Last() + " top card " + TopCard + " card by index " + CardList[CardList.Count - 1]);
        TopCard.IsFaceUp = true;

        recievedCard.Invoke(card, card.GetComponentInParent<Tablue>());
    }
    private void CardListAdded(List<CardSpace> cards, CardGroup dst)
    {
        if (dst != this) return;
        //if (dst.CompareTag("Column") && ((Tablue)dst).Index != Index) return;//destination is tab and is me
       // if (MoveableCards.Any(c=> cards.Contains(c))) return; //Cards that are moving

        var board = Camera.main.GetComponent<Board>();
        if (board is null) return;
        if (!board.ValidCardMove(cards[0], this)) return;

        //Debug.Log(this.name + " Added " + cards[0]);
        //Debug.Log("count of cards added " + cards.Count);

        if (IsEmpty)
        {
            IsEmpty = false;
            ColumnSpaces.Clear();
            CardList.Clear();
            foreach (Transform child in transform) Destroy(child.gameObject);
            
        }

        foreach(CardSpace card in cards)
        {
            //Debug.Log("card adding " + card.CardData);
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
            //Debug.Log("Count "+ ColumnSpaces.Count + " count "+ CardList.Count);
        }

        TopCard = CardList.Last();
        TopCard.IsFaceUp = true;
        //Debug.Log("Add List Card last " + CardList.Last() + " top card " + TopCard + " card by index " + CardList[CardList.Count - 1]);


        recievedList.Invoke(cards, cards[0].GetComponentInParent<Tablue>());
    }
    
    //public void AddListener(UnityAction<CardSpace, CardGroup> handler)
    //{
    //    AddListener(handler);
    //}
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
        //ColumnSpaces.Reverse();
        //HideRemainingSpaces();
    }

    private void AddHiddenCards()
    {
        var hiddenCards = Deck.BuildUniqueCardListToSize(InitialHiddenCardsCount);
        CardList.AddRange(hiddenCards);
        for (int i = 0; i < InitialHiddenCardsCount; i++)
        {
            var emptyCard = Resources.Load("Empty");
            var space = Instantiate<GameObject>((GameObject)emptyCard, gameObject.transform);
            space.transform.position = new Vector3(transform.position.x, ((33 * (i+1)) * -1)+142, transform.position.z);
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
        var space = Instantiate(emptyCard, new Vector3(transform.position.x, ((33 * (Index+1)) * -1)+142, transform.position.z), Quaternion.identity, gameObject.transform);
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
            //img.DisableImage();
        }
    }
    #endregion Initialization
    public override string ToString() => $"Tab index: {Index},\nCard Count: {TotalCount}, " +
        $"{InitialHiddenCardsCount} cards face down, {TopCard?.ToString()?.Split(",")?.FirstOrDefault()} Face up";
}
