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
        //EventManager.Card(UpdateSprite);
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

        var draw = OptionsManager.DrawCount is DrawType.Single ? 1 : 3;
        int count = 0;
        
        var deckPosition = DiscardPile.gameObject.transform.position;
        foreach (Transform child in DiscardPile.transform)
        {
            child.transform.position = new Vector3(deckPosition.x, child.position.y, child.position.z);
            child.GetComponent<Drag>().enabled = true;
        }
        
        while (count < draw && !IsEmpty)
        {
            
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
                TopCard = CardList.Last();
            }
            DiscardPile.SpawnCard(count);
            count++;
        }
        if (OptionsManager.DrawCount is not DrawType.Single)
        {
            if (DiscardPile.TopCard is null)
                return;
            
            var topCardSpace = DiscardPile.GetGroupCardSpace(DiscardPile.TopCard);
            topCardSpace.GetComponent<Drag>().enabled = true;
        }
        UpdateSprite();
    }
    

    public void UpdateSprite() => thisImage.sprite = this.GetSpriteForDrawDeck(!DiscardPile?.IsEmpty ?? true && !OptionsManager.ExceededResets());
}
