using UnityEngine;
using UnityEngine.UI;
using static GameState;

public class BounceAnimation : MonoBehaviour
{
    [SerializeField]
    RectTransform cardPrefab;
    [SerializeField]
    int cloneLimit;
    [SerializeField]
    RectTransform animationArea;
    float minAnimationDelay;
    float maxAnimationSpeed;

    Sprite theSprite;

    CardSuits FoundationSuit;
    int StartValue = MAX_CARD_VALUE; 

    private bool isAnimating = true;
    public bool IsAnimating => isAnimating;

    float animationDelay; 
    float animationSpeed; 

    private int yOffset = -9;
    private int xOffset = -9;
    private int yOffsetSum = 0;
    private int xOffsetSum = 0;

    private int initialArchHeight = 1; //For desc
    private int bounceArchHeight = 10; //For asc
    private int bounceDirection = 1; //R/L for X

    //private float topBorder;
    private float bottomBorder;
    private float leftBorder;
    private float rightBorder;

    Vector3 position;
    RectTransform clone;
    ArchStates archState;

    int cloneCount = 0;
    enum ArchStates
    {
        Init, Asc, Desc, InitBounce, BounceBottom, BounceSide
    }
    void Start()
    {
        //isAnimating = true;
        position = transform.position;

        bottomBorder = animationArea.rect.height;

        InitializeAnimation();


    }
    void InitializeAnimation()
    {
        //animationDelay = Random.Range(minAnimationDelay, 0.5f);
        //animationSpeed = Random.Range(0.01f, maxAnimationSpeed);  //animationDelay, animationSpeed  0.5f, 0.02f
        minAnimationDelay = Random.Range(0.1f, 0.8f);
        maxAnimationSpeed = Random.Range(0.02f, 0.05f);

        animationDelay = Random.Range(minAnimationDelay, 0.5f);
        animationSpeed = Random.Range(0.01f, maxAnimationSpeed);

        bounceDirection = Random.Range(0, 2) * 2 - 1; //-1 || 1 for R/L X
        initialArchHeight = Random.Range(5, bounceArchHeight); //??
        yOffset = Random.Range(-12, 3); //Initiated ascending
        xOffset = Random.Range(-16, -7);//??
        yOffsetSum = 0;
        xOffsetSum = 0;
        cloneCount = 0;
        clone = null;
    }
    public void StartAnimating(CardSuits suit)
    {
        FoundationSuit = suit;
        InitializeCard();
        if (StartValue >= 0)
        {
            isAnimating = true;
            InvokeRepeating("Clone", animationDelay, animationSpeed);//animationDelay, animationSpeed //0.5f, 0.02f
        }
    }
    public void ResetAnimaiton()
    {
        CancelInvoke("Clone");
        isAnimating = false;
        StartValue = MAX_CARD_VALUE;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        InitializeAnimation();
    }
    
    void InitializeCard()
    {
        var tempData = new CardData(FoundationSuit, (CardValues)StartValue);
        tempData.IsFaceUp= true;
        theSprite = tempData.GetSpriteForCard();
        StartValue--;
        if (StartValue < 0) 
        { 
            isAnimating = false; 
        }
    }
    void Clone()
    {
        var pos = clone?.localPosition ?? new Vector3();
        switch (archState)
        {
            case ArchStates.Init: 
                xOffset *= bounceDirection;
                archState = ArchStates.Asc;
                break;
            case ArchStates.Asc:
                yOffset += 3;
                if (initialArchHeight >= 10) //var upperBound?
                    archState = ArchStates.Desc;
                break;
            case ArchStates.Desc:
                yOffset -= 3;
                if (Mathf.Abs(pos.y) >= (bottomBorder -(clone?.rect.height * 1.5 ?? 0))) //Change for better bottom check
                {
                    archState = ArchStates.InitBounce;
                    bounceArchHeight = 15;
                }
                break;
            case ArchStates.InitBounce:
                yOffset = Mathf.Abs(yOffset)/Random.Range(2, Random.Range(2,4));
                xOffset += 2;
                initialArchHeight = Random.Range(5, bounceArchHeight); //bound or sum??
                archState = ArchStates.Asc;
                break;
            case ArchStates.BounceBottom: //Non case
                yOffset += 5;
                if (bounceArchHeight >= 10)
                    archState = ArchStates.Desc;
                break;
        }
        xOffsetSum += xOffset;
        yOffsetSum += yOffset;

        var childPosition = new Vector3(position.x + xOffsetSum, position.y + yOffsetSum, position.z);
        clone = cloneCount > 0 ? 
            Instantiate(cardPrefab, childPosition, Quaternion.identity, transform) : 
            Instantiate(cardPrefab, position, Quaternion.identity, transform);
        
        clone.name = "Clone";
        clone.GetComponent<Image>().sprite = theSprite;

        if (archState is ArchStates.Asc && initialArchHeight <= bounceArchHeight) initialArchHeight++;
        
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
    private void FixedUpdate()
    {
        if (isAnimating)
        {
            bool isFullyVisible = clone is not null ? IsInsideRect(clone) : true;
            if (cloneCount >= cloneLimit || !isFullyVisible)
            {
                CancelInvoke("Clone");
                InitializeAnimation();
                //InitializeCard();
                StartAnimating(FoundationSuit);
            }
        }
        
    }
    private bool IsInsideRect(RectTransform clone)
    {
        var areaX = Mathf.Abs(animationArea.rect.x);
        var areaY = Mathf.Abs(animationArea.rect.y);
        var cloneX = Mathf.Abs(clone.localPosition.x);
        var cloneY = Mathf.Abs(clone.localPosition.y);

        //Debug.Log($"vars {areaX}, {areaY}  > {cloneX}, {cloneY}");
        if(cloneX > areaX + (clone.rect.width * .6) )
            if(cloneY > areaY + (clone.rect.height * .6))
                return false;
        return true;
    }
}