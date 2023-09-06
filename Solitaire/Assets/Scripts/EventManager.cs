using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    static List<Drag> cardDroppedInvokers = new List<Drag>();
    static List<CardGroup> cardLeftInvokers = new List<CardGroup>();
    static List<CardGroup> cardUpInvokers = new List<CardGroup>();//For scoring  (?)
    static DiscardPile discardInvoker = new DiscardPile(); 
    static List<Foundation> maxInvokers= new List<Foundation>();
    static ButtonControls cardChangeInvoker = new ButtonControls();
    static AutoMoveToFoundation autoMoverInvoker = new AutoMoveToFoundation();

    static List<UnityAction<CardSpace, CardGroup>> discardMovedListeners = new List<UnityAction<CardSpace, CardGroup>>();
    static List<UnityAction> cardExposedListeners = new List<UnityAction>();
    static List<UnityAction> discardResetListners = new List<UnityAction>();
    static List<UnityAction> maxedListeners = new List<UnityAction>();
    static List<UnityAction> cardChangedListeners = new List<UnityAction>();

    static List<UnityAction<CardSpace, CardGroup>> droppedOnListeners = new List<UnityAction<CardSpace, CardGroup>>(); 
    static List<UnityAction<List<CardSpace>, CardGroup>> listDroppedOnListeners = new List<UnityAction<List<CardSpace>, CardGroup>>();

    static List<UnityAction<CardSpace, CardGroup>> cardLeftListeners = new List<UnityAction<CardSpace, CardGroup>>();
    static List<UnityAction<List<CardSpace>, CardGroup>> cardListLeftListeners = new List<UnityAction<List<CardSpace>, CardGroup>>();

    //public delegate void OnCardDropped(Drag card);
    //public static event OnCardDropped onCardDropped;
    public static void CardUpInvokers(CardGroup cardSource)
    {
        cardUpInvokers.Add(cardSource);
        foreach(var listener in cardExposedListeners)
        {
            cardSource.AddListener(listener);
        }
    }
    public static void MaxFoundationInvokers(Foundation foundation)
    {
        maxInvokers.Add(foundation);

        foreach (var listener in maxedListeners)
        {
            foundation.AddListener(listener);
        }
    }
    /// <summary>
    /// For discard to register itself for waste exclusive events, moving from discard & resetting pile
    /// </summary>
    /// <param name="waste">discard pile, should be a singleton</param>
    public static void DiscardInvoker(DiscardPile waste) 
    { 
        discardInvoker = waste;

        foreach (var listener in discardMovedListeners)
        {
            waste.AddListener(listener);
        }
    }
    public static void DiscardResetInvoker(DiscardPile waste)
    {
        discardInvoker = waste;

        foreach (var listener in discardResetListners)
        {
            waste.AddListener(listener);
        }
    }
    /// <summary>
    /// A card that has been moved, yelling
    /// Who wants to know that a card is gonna drop
    /// </summary>
    /// <param name="card">Drag script that invokes the event when dropped</param>
    public static void AddCardInvoker(Drag card)
    {
        //A card that has been moved, yelling
        //Who wants to know that a card is gonna drop
        cardDroppedInvokers.Add(card);
        foreach (UnityAction<CardSpace, CardGroup> listener in droppedOnListeners)
        {
            card.AddListener(listener);
        }
        foreach (UnityAction<List<CardSpace>, CardGroup> listener in listDroppedOnListeners)
        {
            card.AddListener(listener);
        }
    }
    public static void AddCardInvoker(AutoMoveToFoundation autoMover)
    {
        //A card that has been moved, yelling
        //Who wants to know that a card is gonna drop
        autoMoverInvoker = autoMover;
        foreach (UnityAction<CardSpace, CardGroup> listener in droppedOnListeners)
        {
            autoMover.AddListener(listener);
        }
    }
    /// <summary>
    /// A card that was recieved, telling its place has been changed
    /// Who wants to know that a card is no longer home
    /// </summary>
    /// <param name="reciever">Card group that get cards dropped on them letting others know</param>
    public static void CardLeftInvoker(CardGroup reciever)
    {
        //A card that was recieved, telling its place has been changed
        //Who wants to know that a card is no longer home
        cardLeftInvokers.Add(reciever);
        foreach (UnityAction<CardSpace, CardGroup> listener in cardLeftListeners)
        {
            reciever.AddListener(listener);
        }
        foreach (UnityAction<List<CardSpace>, CardGroup> listener in cardListLeftListeners)
        {
            reciever.AddListener(listener);
        }
    }
    public static void CardBackChangedInvoker(ButtonControls btnCtrl)
    {
        cardChangeInvoker = btnCtrl;

        foreach (var listener in cardChangedListeners)
        {
            btnCtrl.AddListener(listener);
        }
    }
    /// <summary>
    /// A single card is dropping, am I, as a group relevant?
    /// </summary>
    /// <param name="handler">entitiy to register to the invokers</param>
    public static void AddCardListener(UnityAction<CardSpace, CardGroup> handler)
    {
        //A single card is dropping, am I, as a group relevant?
        droppedOnListeners.Add(handler);
        foreach (Drag card in cardDroppedInvokers)
        {
            card.AddListener(handler);
        }
        autoMoverInvoker.AddListener(handler);
    }
    /// <summary>
    /// A list of cards is dropping, am I, as a group relevant?
    /// </summary>
    /// <param name="handler">entitiy to register to the invokers</param>
    public static void AddCardListListener(UnityAction<List<CardSpace>, CardGroup> handler)
    {
        //A list of cards is dropping, am I, as a group relevant?
        listDroppedOnListeners.Add(handler);
        foreach (Drag card in cardDroppedInvokers)
        {
            card.AddListener(handler);
        }
    }
    /// <summary>
    /// A single card has gone elsewhere, am I, as a group relevant?
    /// </summary>
    /// <param name="handler">entitiy to register to the invokers</param>
    public static void CardLeftListener(UnityAction<CardSpace, CardGroup> handler)
    {
        //A single card has gone elsewhere, am I, as a group relevant?
        cardLeftListeners.Add(handler);
        //Possible sources it left from
        foreach (CardGroup src in cardLeftInvokers)
        {
            src.AddListener(handler);
        }
    }
    /// <summary>
    /// A list of cards has gone elsewhere, am I, as a group relevant?
    /// </summary>
    /// <param name="handler">entitiy to register to the invokers</param>
    public static void CardLeftListListener(UnityAction<List<CardSpace>, CardGroup> handler)
    {
        //A list of cards has gone elsewhere, am I, as a group relevant?
        cardListLeftListeners.Add(handler);
        //Possible srouces it left from
        foreach (CardGroup src in cardLeftInvokers)
        {
            src.AddListener(handler);
        }
    }
    //Listen to score and else ?
    public static void CardMovedFromDiscard(UnityAction<CardSpace, CardGroup> handler)
    {
        discardMovedListeners.Add(handler);
        discardInvoker.AddListener(handler);
    }
    public static void DiscardReset(UnityAction handler)
    {
        discardResetListners.Add(handler);
        discardInvoker.AddListener(handler);
    }
    public static void FoundationMaxed(UnityAction handler)
    {
        maxedListeners.Add(handler);
        foreach (Foundation src in maxInvokers)
        {
            src.AddListener(handler);
        }
    }
    public static void CardExposed(UnityAction handler)
    {
        cardExposedListeners.Add(handler);
        foreach(CardGroup src in cardUpInvokers)
        {
            src.AddListener(handler);
        }
    }
    public static void CardBackChanged(UnityAction handler)
    {
        cardChangedListeners.Add(handler);
        cardChangeInvoker.AddListener(handler);
    }

}