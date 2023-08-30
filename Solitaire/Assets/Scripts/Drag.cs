using System.Collections;
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
        transform.localPosition = startPosition;

        if (!thisCard.CompareTag("DiscardCard"))
        {
            for (int i = 0; i < lowerSiblings.Count; i++)
            {
                yOffset *= (i + 1);
                lowerSiblings[i].transform.localPosition = new Vector3(startPosition.x, startPosition.y - yOffset, lowerSiblings[i].transform.position.z);

                //lowerSiblings[i].GetComponent<Drag>().OnDrag(eventData);
            }
        }
        
        UpdateLayoutGroup();
    }
    public void Dropped(CardGroup src, CardGroup dst)
    {
        //I am the dropped, src is my parent group, destination is where i was dropped
       // Debug.Log("src "+ src +" dst "+ dst +" dst tag "+ dst.tag);
        var tag = dst.tag;
        switch (tag)
        {
            case "Column":
                {
                    Debug.Log("Droppped on slot");
                    HandleMoveToSpace(src, dst);
                    break;
                }
            case "Foundation":
                {
                    Debug.Log("Droppped on Foundation");
                    HandleMoveToFoundation(src, dst);
                    break;
                }
            case "DiscardPile"://non case
                {
                    Debug.Log("Droppped on Pile");
                    return;
                    break;
                }
        }
    }
    private void HandleMoveToSpace(CardGroup src, CardGroup dst)//Source is currently tab only, but in future might be from discard
    {
        List<CardSpace> moveable = new();
        if(gameObject.CompareTag("CardSpace"))
            moveable =GetComponentInParent<Tablue>().GetMoveableCards();
        lowerSiblings = moveable.Where(c => moveable.IndexOf(thisCard) < moveable.IndexOf(c)).ToList();
        lowerSiblings.Insert(0, thisCard);
       // Debug.Log(gameObject.name + " first of dropped " + lowerSiblings?.First() + " last of dropped" + lowerSiblings?.Last());
        if (lowerSiblings.Count < 2)
        {
            //Debug.Log("Invoking single from drag ");
            movedToTab.Invoke(thisCard, dst);
            Destroy(gameObject);
        }
        else
        {
            //Debug.Log("Invoking list from drag ");
            listToTab.Invoke(lowerSiblings, dst);
            foreach(CardSpace card in lowerSiblings)
            {
                Destroy(card.gameObject);
                Destroy(gameObject);
            }
        }

        // Destroy(gameObject); ??
    }

    private void HandleMoveToFoundation(CardGroup src, CardGroup dst)//Source is currently tab only, but in future might be from discard
    {
        //var moveable = GetComponentInParent<Tablue>().GetMoveableCards();
        //lowerSiblings = moveable.Where(c => moveable.IndexOf(thisCard) < moveable.IndexOf(c)).ToList();
        //lowerSiblings.Insert(0, thisCard);
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
    
    IEnumerator UpdateLayoutGroup()
    {
        var layoutGroupComponent = gameObject.GetComponent<LayoutGroup>();
        //Debug.Log("tran " + layoutGroupComponent.GetComponent<RectTransform>());
        //LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroupComponent.GetComponent<RectTransform>());
        //return null;
        layoutGroupComponent.enabled = false;
        yield return new WaitForEndOfFrame();
        layoutGroupComponent.enabled = true;
    }
}
