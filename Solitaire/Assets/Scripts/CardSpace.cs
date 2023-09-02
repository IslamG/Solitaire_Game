using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static GameState;

public class CardData
{
    public CardSuits CardSuit { get; set; }

    public CardValues CardValue { get; set; }

    public Sprite faceDownSprite => this.GetFaceDownSprite();

    public Sprite faceUpSprite => this.GetFaceUpSprite();
    public CardColors CardColor { get; private set; }
    public bool IsFaceUp { get;  set; } = false;

    public CardData(CardSuits suit, CardValues value)
    {
        CardSuit = suit;
        CardValue = value;
        CardColor = SuitColors.FirstOrDefault(x => x.Value.Contains(suit)).Key;
    }
    public override string ToString()
    {
        return $"{CardValue} of {CardSuit}, IsFaceUp: {IsFaceUp}";
    }

}
public class CardSpace : MonoBehaviour
{
    public CardData CardData;

    private Image thisImage;

    public bool IsOccupied = false;

    public void Awake()
    {
        thisImage = GetComponent<Image>();
    }
    public void Expose() 
    {
        CardData.IsFaceUp = true;
        if(thisImage != null && CardData.faceUpSprite != null)
            thisImage.sprite = CardData.faceUpSprite;

    }
    public void Hide()
    {
        CardData.IsFaceUp = false;
        if (thisImage != null && CardData.faceDownSprite != null)
            thisImage.sprite = CardData.faceDownSprite;
    }
    public void SetCardSprite()
    {
        var theSprite = CardData.GetSpriteForCard();

        thisImage.sprite = theSprite;
    }
    public override string ToString()
    {
        return $"{CardData.CardValue} of {CardData.CardSuit}, IsFaceUp: {CardData.IsFaceUp}";//, Sprite {thisImage.sprite}
        //return $"{CardValue} of {CardSuit}";
    }


}
