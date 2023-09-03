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

        var draw = OptionsManager.DrawCount is DrawType.Single ? 1 : 3;
        int count = 0;
        Debug.Log("Discarding card " + draw);
        if (OptionsManager.DrawCount is not DrawType.Single)
        {
            var deckPosition = DiscardPile.gameObject.transform.position;
            Debug.Log("get children " + DiscardPile.transform);
            foreach (Transform child in DiscardPile.transform)
            {
                Debug.Log($"the child {child} deck position {deckPosition}");
                child.transform.position = new Vector3(deckPosition.x, child.position.y, child.position.z);
                child.GetComponent<Drag>().enabled = true;
                Debug.Log("child position after " + child.position);
            }
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
            UpdateSprite();
            //Debug.Log ("here " + draw);
            count++;
        }
        if (OptionsManager.DrawCount is not DrawType.Single)
        {

            var topCardSpace = GameObject.Find($"{DiscardPile.TopCard.CardValue} of {DiscardPile.TopCard.CardSuit}");
            topCardSpace.GetComponent<Drag>().enabled = true;
        }
    }
    

    public void UpdateSprite() => thisImage.sprite = this.GetSpriteForDrawDeck();
}
