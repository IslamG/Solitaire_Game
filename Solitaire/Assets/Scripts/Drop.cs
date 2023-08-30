using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        //pointer is source of drop call
        //This class is destination
        Debug.Log("on drop " + eventData.pointerDrag);
        if (eventData.pointerDrag.CompareTag("CardSpace") || eventData.pointerDrag.CompareTag("DiscardCard"))
        {
            var cardData = eventData.pointerDrag.GetComponent<CardSpace>();
            if (cardData is null) return;
            var dragData = eventData.pointerDrag.GetComponent<Drag>();
            if (dragData is null) return;

            var board = Camera.main.GetComponent<Board>();
            if (board is null) return;

            Debug.Log("the card " + cardData + " dst " + GetComponent<CardGroup>());
            var move = board.ValidCardMove(cardData, GetComponent<CardGroup>());
            if(!move)
            {
                dragData.ResetPosition();
                return;
            }

            //Destination because dropped on top of me
            //Source where the card came from, i.e. where the mouse pulled from
            //Drag data = Me, the card being dragged and dropped
            dragData.Dropped(src: cardData.transform.parent.GetComponent<CardGroup>(), dst: GetComponent<CardGroup>());

        } 
    }

}
