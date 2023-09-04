using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Random = System.Random;

public class BounceAnimation : MonoBehaviour
{
    [SerializeField]
    Transform cardPrefab;
    [SerializeField]
    int cloneLimit;

    private int yOffset = -9;
    private int xOffset = -9;
    private int yOffsetSum = 0;
    private int xOffsetSum = 0;

    private int initialArchHeight = 1;
    private int bounceArchHeight = 1;
    private int bounceDirection = 1;

    private float topBorder;
    private float bottomBorder;
    private float leftBorder;
    private float rightBorder;

    //Random random = new Random();
    Vector3 position;
    //Vector3 scale;
    Transform clone;
    ArchStates archState;

    int cloneCount = 0;
    float lowerBound = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        //scale = transform.localScale;
        bounceDirection = Random.Range(0, 2) * 2 - 1; //-1 || 1 for R/L X
        initialArchHeight = Random.Range(5, 10); //??
        yOffset = Random.Range(-12, 3); //Initiated ascending
        xOffset= Random.Range(-16, -7);//??

        topBorder = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).y; //top right
        rightBorder = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x; //top right
        bottomBorder = Camera.main.ScreenToWorldPoint(Vector3.zero).y; //bottom left
        leftBorder = Camera.main.ScreenToWorldPoint(Vector3.zero).x; //bottom left

        archState = ArchStates.Init;

         
        lowerBound = (Screen.height ) - cardPrefab.GetComponent<Image>().sprite.rect.height;
        Debug.Log("bottom screen " + bottomBorder + " lower bound " + lowerBound);
        //topBorder= stageDimensions.
        InvokeRepeating("Clone", .5f, 0.05f);
    }
    enum ArchStates
    {
        Init, Asc, Desc, InitBounce, BounceBottom, BounceSide
    }
    void Clone()
    {
        //Vector3 pos = Camera.main.WorldToViewportPoint(clone?.localPosition ?? new Vector3());
        //Debug.Log("W2VP " + pos);
        //pos = Camera.main.WorldToScreenPoint(clone?.localPosition ?? new Vector3());
        //Debug.Log("W2SP " + pos);
        //pos = Camera.main.ScreenToViewportPoint(clone?.localPosition ?? new Vector3());
        //Debug.Log("S2VP " + pos);
        var pos = Camera.main.ScreenToWorldPoint(clone?.position ?? new Vector3(1,1,1));
        Debug.Log("S2WP " + pos);
        //pos = Camera.main.ViewportToWorldPoint(clone?.localPosition ?? new Vector3());
        //Debug.Log("V2WP " + pos);
        //pos = Camera.main.ViewportToScreenPoint(clone?.localPosition ?? new Vector3());
        //Debug.Log("V2SP " + pos);
        //Debug.Log($"pos {pos} Screen h {Screen.height}");
        switch (archState)
        {
            case ArchStates.Init: 
                xOffset *= bounceDirection;
                archState = ArchStates.Asc;
                break;
            case ArchStates.Asc:
                yOffset += 3;
                if (initialArchHeight >= 15) //var upperBound?
                    archState = ArchStates.Desc;
                break;
            case ArchStates.Desc:
                yOffset -= 3;
                //KeepFullyOnScreen(clone.gameObject, new Vector3(position.x + xOffsetSum, position.y + yOffsetSum, position.z));
                if (pos.y <= bottomBorder+1.71)
                {
                    archState = ArchStates.InitBounce;
                    bounceArchHeight = 0;
                }
                break;
            case ArchStates.InitBounce:
                Debug.Log("bouncing " + yOffset + " y pos " + pos.y);
                yOffset = 0;
                xOffset += 2;
                bounceArchHeight = Random.Range(7, 12); //bound or sum??
                archState = ArchStates.BounceBottom;
                break;
            case ArchStates.BounceBottom:
                yOffset += 5;
                if (bounceArchHeight >= 13)
                    archState = ArchStates.Desc;
                //yOffset *= -1;
                break;
        }
        //if (archHeight < 10 && yOffset < 0) yOffset *= -1;
        // if (archHeight >= 10 && pos.y > bottomBorder)  yOffset -=1; //&& yOffsetSum > bottomBorder
        //else if (archHeight >= 10 && pos.y < bottomBorder)  yOffset +=1;

        Debug.Log("y pos " + pos.y);
        //yOffset *= bounceDirection;
        xOffsetSum += xOffset;
        yOffsetSum += yOffset;

        var childPosition = new Vector3(position.x + xOffsetSum, position.y + yOffsetSum, position.z);
        clone = cloneCount > 0 ? Instantiate(cardPrefab, childPosition, Quaternion.identity, transform) : Instantiate(cardPrefab, position, Quaternion.identity, transform);
        //clone.transform.localPosition = childPosition;
        clone.name = "Clone";
        // clone.GetComponent<Transform>().SetParent(transform);

        if (archState is not ArchStates.BounceBottom && initialArchHeight < 15) initialArchHeight++;
        if(archState is ArchStates.BounceBottom && bounceArchHeight < 13) bounceArchHeight++;
        
        cloneCount++;

    }

    Vector3 KeepFullyOnScreen(GameObject panel, Vector3 newPos)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();
        RectTransform CanvasRect = GameObject.Find("MainCanvas").GetComponent<RectTransform>();

        float minX = (CanvasRect.sizeDelta.x - rect.sizeDelta.x) * -0.5f;
        float maxX = (CanvasRect.sizeDelta.x - rect.sizeDelta.x) * 0.5f;
        float minY = (CanvasRect.sizeDelta.y - rect.sizeDelta.y) * -0.5f;
        float maxY = (CanvasRect.sizeDelta.y - rect.sizeDelta.y) * 0.5f;

        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        return newPos;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        //cloneCount = GameObject.FindGameObjectsWithTag("Clone").Length;
        //while (cloneCount < 15)
        //{
        //    StartCoroutine("Clone");
        //    //Debug.Break();
        //}
        if (cloneCount >= cloneLimit) CancelInvoke("Clone");
    }
}
