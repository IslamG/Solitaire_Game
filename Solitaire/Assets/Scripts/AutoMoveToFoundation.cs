using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AutoMoveToFoundation : MonoBehaviour, IPointerClickHandler
{
    Board board;
    bool moveMade = false;
    bool isInit;

    public OnCardMovedToFoundation<CardSpace, CardGroup> movedToFoundation = new();

    void Start()
    {
        board = Camera.main.GetComponent<Board>();
        EventManager.AddCardInvoker(this);
        isInit = true;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        if (eventData.clickCount < 2) return;

        //board.Win();

        isInit = true;
        while (moveMade || isInit)
        {
            moveMade = false;
            isInit = false;

            DetectDiscardMove();
            DetectTabMove();
        }

    }

    void DetectDiscardMove()
    {
        if (Board.DiscardPile is null) return;
        if (Board.DiscardPile.IsEmpty) return;
        if (Board.DiscardPile.TopCard is null) return;

        var topData = Board.DiscardPile.GetGroupCardSpace(Board.DiscardPile.TopCard);

        foreach (var foundation in board.Foundations)
        {
            
            var move = board.ValidCardMove(topData, foundation);
            if (!move) continue;

            moveMade = true;
            movedToFoundation.Invoke(topData, foundation);
            Destroy(topData.gameObject);
        }
    }
    void DetectTabMove() 
    { 
        foreach(var tab in board.Tablues)
        {
            if (tab is null) continue;
            if (tab.IsEmpty || tab.TopCard is null) continue; 

            foreach(var foundation in board.Foundations)
            {
                var topData = tab.GetGroupCardSpace(tab.TopCard);
                var move = board.ValidCardMove(topData, foundation);
                if (!move) continue;

                moveMade = true;
                movedToFoundation.Invoke(topData, foundation);
                Destroy(topData.gameObject);
            }
        }
    }

    public void AddListener(UnityAction<CardSpace, CardGroup> handler)
    {
        movedToFoundation.AddListener(handler);
    }

}
