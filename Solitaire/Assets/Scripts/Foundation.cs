using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using static GameState;

public class Foundation : CardGroup
{

    public CardSuits Suit { get; private set; }
    public int Index;

    public OnCardLeftTablue<CardSpace, CardGroup> leftForFoundation = new();
    public OnFoundationMaxed foundationMaxed = new();

    public void Start()
    {
        base.Start();

        EventManager.CardLeftInvoker(this);
        EventManager.MaxFoundationInvokers(this);

        EventManager.AddCardListener(MoveCardToFoundation);
    }
    public void MoveCardToFoundation(CardSpace card, CardGroup dst)
    {
        if (dst != this) return;
        if (!dst.CompareTag("Foundation")) return;
        if (((Foundation)dst).Index != Index) return; 

        var board = Camera.main.GetComponent<Board>();
        if (board is null) return;

        if (!board.ValidCardMove(card, this)) return;

        if(IsEmpty) InitializeSuit(card.CardData.CardSuit);
        var img = card.GetComponent<Image>();
        if (img != null)
        {
            GetComponent<Image>().sprite = img.sprite;
        }
        
        TopCard = card.CardData;
        IsEmpty = false;

        recievedCard.Invoke(card, card.GetComponentInParent<Tablue>());
        if ((int)card.CardData.CardValue == MAX_CARD_VALUE)
        {
            foundationMaxed.Invoke();
            transform.GetComponent<Drop>().enabled= false;
            this.enabled = false;
        }
    }
    
    private void InitializeSuit(CardSuits suit) => Suit = suit;

    public void AddListener(UnityAction handler)
    {
        foundationMaxed.AddListener(handler);
    }

    public override string ToString() => $"Foundation for {Suit}, IsEmpty: {IsEmpty}, Topcard {TopCard?.ToString()?.Split(",")?.FirstOrDefault()}";


}
