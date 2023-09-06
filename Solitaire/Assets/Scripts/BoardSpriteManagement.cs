using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameState;

public static class BoardSpriteManagement
{
    static string SpriteSheetName = "PC Computer - Solitaire - Cards";
    static Sprite[] All = Resources.LoadAll<Sprite>(SpriteSheetName);
    static int BackIndex = 52;
    static int EmptyPositiveIndex = 62;
    static int EmptyNegativeIndex = 63;
    static int EmptyIndex = 64;
    //0-12 Spades
    //13-25 Hearts
    //26-38 Clubs
    //39-51 Diamonds
    //52-61 CardBacks
    //62 Empty pos
    //63 Empty neg
    //64 Empty
    //65-73 More
    //74-77 Blank

    public static int CurrentCardSprite => BackIndex;
    public static Sprite GetSpriteForCard(this CardData card) => card.IsFaceUp ? card.GetFaceUpSprite() : card.GetFaceDownSprite();
    public static Sprite GetFaceDownSprite(this CardData card) => LoadByName($"{SpriteSheetName}_{BackIndex}");
    public static Sprite GetFaceUpSprite(this CardData card)
    {
        var range = card.CardSuit switch
        {
            CardSuits.Spades => 0,
            CardSuits.Hearts => 13,
            CardSuits.Clubs => 26,
            CardSuits.Diamonds => 39
        };
        return LoadByName($"{SpriteSheetName}_{(range + (int)card.CardValue - 1)}");
    }
    public static Sprite GetEmptySprite(this CardData card) => LoadByName($"{SpriteSheetName}_{EmptyIndex}");
    public static Sprite GetSpriteForDiscard(this CardGroup pile)
    {
        if (pile.IsEmpty)
            return LoadByName($"{SpriteSheetName}_{EmptyPositiveIndex}");//Always positive?

        return pile.TopCard.GetSpriteForCard();
    }
    public static Sprite GetSpriteForDrawDeck(this CardGroup pile, bool canReset)
    {
        if (pile.IsEmpty)
            return LoadByName($"{SpriteSheetName}_{(canReset? EmptyPositiveIndex : EmptyNegativeIndex)}");

        return LoadByName($"{SpriteSheetName}_{BackIndex}");
    }
    public static void ChangeCardBack(this int index) => BackIndex = index;
    static Sprite LoadByName(string spriteName)
    {
        foreach (var s in All)
        {
            if (s.name == spriteName)
            {
                return s;
            }
        }
        return null;
    }
    static Sprite LoadByIndex(int index) => All[index];

}

