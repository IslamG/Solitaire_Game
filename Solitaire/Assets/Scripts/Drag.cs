using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CardSpace))]
public class Drag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public int yOffset = 30; //in pixels //for debugging
    public Vector3 startPosition;
    public Image thisImage;
    public CardSpace thisCard;
    public List<CardSpace> lowerSiblings;

    public OnCardListMovedToTablue<List<CardSpace>, CardGroup> listToTab = new();
    public OnCardMovedToTablue<CardSpace, CardGroup> movedToTab = new();
    public OnCardMovedToFoundation<CardSpace, CardGroup> movedToFoundation = new();
    public void Start()
    {
        startPosition = transform.localPosition;
        thisImage = GetComponent<Image>();
        thisCard = GetComponent<CardSpace>();

        EventManager.AddCardInvoker(this);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!thisCard.CardData.IsFaceUp) return;

        thisImage.raycastTarget = false;

        if (!thisCard.CompareTag("DiscardCard"))
        {
            var moveable = GetComponentInParent<Tablue>().GetMoveableCards();
            lowerSiblings = moveable.Where(c => moveable.IndexOf(thisCard) < moveable.IndexOf(c)).ToList();
            if (lowerSiblings.Count == 0) return;
            for (int i = 0; i < lowerSiblings.Count; i++)
            {
                lowerSiblings[i].GetComponent<Image>().raycastTarget = false;
            }
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!thisCard.CardData.IsFaceUp) return;

        transform.position = eventData.position;

        if (!thisCard.CompareTag("DiscardCard"))
        {
            var moveable = GetComponentInParent<Tablue>().GetMoveableCards();

            if (lowerSiblings.Count == 0) return;
            for (int i = 0; i < lowerSiblings.Count; i++)
            {

                yOffset *= (i + 1);
                lowerSiblings[i].transform.position = new Vector3(eventData.position.x, eventData.position.y - yOffset, lowerSiblings[i].transform.position.z);


            }
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ResetPosition();
        thisImage.raycastTarget = true;
        if (!thisCard.CompareTag("DiscardCard"))
        {
            foreach (var card in lowerSiblings)
            {
                card.GetComponent<Image>().raycastTarget = true;

            }
        }
        UpdateLayoutGroup();
    }
    public void ResetPosition()
    {
        Canvas.ForceUpdateCanvases();
        transform.localPosition = startPosition;
        if (!thisCard.CompareTag("DiscardCard"))
        {
            for (int i = 0; i < lowerSiblings.Count; i++)
            {
                yOffset *= (i + 1);
                lowerSiblings[i].transform.localPosition = new Vector3(startPosition.x, startPosition.y - yOffset, lowerSiblings[i].transform.position.z);
            }
        }

        UpdateLayoutGroup();
    }
    public void Dropped(CardGroup src, CardGroup dst)
    {
        //I am the dropped, src is my parent group, destination is where i was dropped
        var tag = dst.tag;
        switch (tag)
        {
            case "Column":
                HandleMoveToSpace(src, dst);
                break;
            case "Foundation":
                HandleMoveToFoundation(src, dst);
                break;
                
            default:
                ResetPosition();
                break;
        }
        //case "DiscardPile"://non case
        //    {
        //        //return;
        //        break;
        //    }
        
        UpdateLayoutGroup();
    }
    private void HandleMoveToSpace(CardGroup src, CardGroup dst)//Source is currently tab only, but in future might be from discard
    {
        List<CardSpace> moveable = new();
        if(gameObject.CompareTag("CardSpace"))
            moveable =GetComponentInParent<Tablue>().GetMoveableCards();
        lowerSiblings = moveable.Where(c => moveable.IndexOf(thisCard) < moveable.IndexOf(c)).ToList();
        lowerSiblings.Insert(0, thisCard);
        if (lowerSiblings.Count < 2)
        {
            movedToTab.Invoke(thisCard, dst);
            Destroy(gameObject);
        }
        else
        {
            listToTab.Invoke(lowerSiblings, dst);
            foreach(CardSpace card in lowerSiblings)
            {
                Destroy(card.gameObject);
                Destroy(gameObject);
            }
        }
    }

    private void HandleMoveToFoundation(CardGroup src, CardGroup dst)//Source is currently tab only, but in future might be from discard
    {
        if (thisCard.CardData != src.TopCard) return;
        movedToFoundation.Invoke(thisCard, dst);
        Destroy(gameObject);
    }
    
    public void AddListener(UnityAction<List<CardSpace>, CardGroup> handler)
    {
        listToTab.AddListener(handler);
    }
    public void AddListener(UnityAction<CardSpace, CardGroup> handler)
    {
        movedToFoundation.AddListener(handler);
        movedToTab.AddListener(handler);
    }
    
    void UpdateLayoutGroup()
    {
        Canvas.ForceUpdateCanvases();
        var layoutGroupComponent = gameObject.GetComponentInParent<LayoutGroup>();
        layoutGroupComponent.enabled = false;
        //yield return new WaitForEndOfFrame();
        layoutGroupComponent.enabled = true;
        //EditorApplication.QueuePlayerLoopUpdate();
        
    }
}
