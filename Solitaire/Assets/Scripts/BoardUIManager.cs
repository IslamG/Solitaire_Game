using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUIManager : MonoBehaviour
{
    //?? Z index??

    //Screen padding
    //Row padding
    //Column padding

    //Playable area
    //Draw
    //Discard
    //Foundation 
    //Tab?

    //Screen == playable area! remaining after padding?
    //CardW = ScreenW/8
    //CardH = CardW * 1.66666 (1 2/3) ?? 1/3
    //stacked yOffset = CardH/10  <- min
    //draw stack xOffset = CardW/4 <- max

    //screen columns = 7 + padding = (1/8 ScreenW / 6)
    //screen rows = 4 + padding = (1/4 ScreenH / 4)
    public static BoardUIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    private void CalculateCardDimensions() { }
    private void InstantiateDiscardPile() { }
    private void InstantiateDrawDeck() { }       
    private void InstantiateFoundation() { }
    private void InstantiateTablue() { }
}
