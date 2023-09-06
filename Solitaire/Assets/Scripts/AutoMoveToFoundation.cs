using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoMoveToFoundation : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    GameObject winWindow;

    Board board;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        if (eventData.clickCount < 2) return;
        
        board.Win();
    }

    // Start is called before the first frame update
    void Start()
    {
        board = Camera.main.GetComponent<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
