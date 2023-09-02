using System;
using System.Collections.Generic;
using Random = System.Random;

public static class GameState 
{
    public const int DECK_SIZE = 52;
    public const int SUIT_COUNT = 4;
    public const int TAB_COUNT = 7;
    public const int FOUNDATION_COUNT = 4;
    public const int MAX_CARD_VALUE = 13;
    public const int MIN_CARD_VALUE = 1;
    public static List<CardData> Deck { get; private set;}

    private static Random RandomVal = new Random();

    public enum CardSuits
    {
        Diamonds, Hearts, Clubs, Spades
    };
    public enum CardColors
    {
        Red, Black
    }
    public enum CardValues //NumberCards
    {
        Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
    }
    public static Dictionary<CardColors, List<CardSuits>> SuitColors => suitColors;

    private static readonly Dictionary<CardColors, List<CardSuits>> suitColors = new()
    {
        {
            CardColors.Red, new List<CardSuits>() {CardSuits.Diamonds, CardSuits.Hearts}
        },
        {
            CardColors.Black, new List<CardSuits>() {CardSuits.Clubs, CardSuits.Spades }
        }
    };

    public static void GenerateDeck()
    {
        List<CardData> deck = new();
        foreach (CardSuits suit in Enum.GetValues(typeof(CardSuits)))
        {
            foreach (CardValues value in Enum.GetValues(typeof(CardValues)))
            {
                deck.Add(new CardData(suit, value)); 
            }
        }

        Deck = deck.Shuffle<CardData>();
    }
    private static T PickRandom<T>(this List<T> source)
    {
        int randIndex = RandomVal.Next(source.Count);
        return source[randIndex];
    }
    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }
    public static T Previous<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) - 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }
    public static void DrawCard(CardData card, List<CardData> fromList) => fromList.Remove(card);
    public static CardData GetNewCardNoRepeat(this List<CardData> cardList, List<CardData> source)
    {
        var card = source.PickRandom();
        while (cardList.Contains(card))
        {
            card = source.PickRandom();
        }
        return card;
    }
    public static List<CardData> BuildUniqueCardListToSize(this List<CardData> source, int size)
    {
        List<CardData> cards = new();
        for(int i =0; i< size; i++)
        { 
            cards.Add(cards.GetNewCardNoRepeat(source));
        }
        return cards;
    }
    private static List<T> Shuffle<T>(this List<T> cardList)
    {
        for (int i = cardList.Count - 1; i > 1; i--)
        {
            int rnd = RandomVal.Next(i + 1);

            T value = cardList[rnd];
            cardList[rnd] = cardList[i];
            cardList[i] = value;
        }

        return cardList;
    }


}
