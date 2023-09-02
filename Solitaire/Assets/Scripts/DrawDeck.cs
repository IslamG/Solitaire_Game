using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static GameState;

public class DrawDeck : CardGroup
{
    private DiscardPile DiscardPile;
    private Board Board;
    private Image thisImage;

    public static DrawDeck  DrawInstance { get; private set; }

    void Awake()
    {
    // If there is an instance, and it's not me, delete myself.

        if (DrawInstance != null && DrawInstance != this)
        {
            Destroy(this);
        }
        else
        {
            DrawInstance = this;
        }

        Board = Camera.main.GetComponent<Board>();
        if (Board is null) return;

        thisImage = GetComponent<Image>();
    }
    void Start()
    {
        DiscardPile = Board.DiscardPile;
        EventManager.CardBackChanged(UpdateSprite);
    }
    public void Initialize()
    {
        CardList = new();
        CardList.AddRange(Deck);
        TopCard = CardList?.Last<CardData>();
        IsEmpty = false;
        UpdateSprite();
    }
    public void DiscardCard()
    {
        if (IsEmpty) DiscardPile.ResetDiscardPile();

        DiscardPile.CardList.Add(TopCard);
        DiscardPile.TopCard = TopCard;
        DiscardPile.TopCard.IsFaceUp = true;
        DiscardPile.IsEmpty = false;

        CardList.Remove(TopCard);

        if (TotalCount == 0)
        {
            IsEmpty = true;
            CardList.Clear();
        }
        else
        {
            TopCard =  CardList.Last();
        }
        DiscardPile.SpawnCard();
        UpdateSprite();
    }
    

    public void UpdateSprite() => thisImage.sprite = this.GetSpriteForDrawDeck();
}
