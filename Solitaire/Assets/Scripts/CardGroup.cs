using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine;

public class CardGroup : MonoBehaviour
{
    public int TotalCount => CardList.Count;
    public List<CardData> CardList { get; set; } = new();
    public CardData TopCard { get; set; }
    public bool IsEmpty { get; set; } = true;

    protected void PopLast() => CardList.RemoveAt(TotalCount - 1);

    public void Start()
    {
        EventManager.CardLeftInvoker(this);
        EventManager.CardUpInvokers(this);
    }
    public void ResetGroup()
    {
        CardList.Clear();
        TopCard= null;
        IsEmpty= true;
    }
    public OnCardExposed cardTurned = new();
    public OnCardLeftTablue<CardSpace, CardGroup> recievedCard = new();
    public OnCardListLeftTablue<List<CardSpace>, CardGroup> recievedList = new();
    public void AddListener(UnityAction handler)
    {
        cardTurned.AddListener(handler);
    }
    public void AddListener(UnityAction<List<CardSpace>, CardGroup> handler)
    {
        recievedList.AddListener(handler);
    }
    public void AddListener(UnityAction<CardSpace, CardGroup> handler)
    {
        recievedCard.AddListener(handler);
    }
    public override string ToString() => $"CardGroup: Count: {TotalCount}, {TopCard?.ToString()?.Split(",")?.FirstOrDefault()} Face up";
}
