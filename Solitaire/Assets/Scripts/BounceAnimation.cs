using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEngine.ParticleSystem;
//using Random = System.Random;

//using Random = System.Random;

public class BounceAnimation : MonoBehaviour
{
    [SerializeField]
    RectTransform cardPrefab;
    [SerializeField]
    int cloneLimit;
    [SerializeField]
    RectTransform animationArea;

    private bool isAnimating;
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
    //float lowerBound = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        isAnimating = true;
        position = transform.position;

        animationDelay = Random.Range(0f, 0.15f);
        animationSpeed = Random.Range(0.05f, 0.15f);

        bounceDirection = Random.Range(0, 2) * 2 - 1; //-1 || 1 for R/L X
        initialArchHeight = Random.Range(5, bounceArchHeight); //??
        yOffset = Random.Range(-12, 3); //Initiated ascending
        xOffset= Random.Range(-16, -7);//??

        bottomBorder = animationArea.rect.height;

        //lowerBound = (Screen.height ) - cardPrefab.GetComponent<Image>().sprite.rect.height;
        //Debug.Log("bottom screen " + bottomBorder + " lower bound " + lowerBound);
        //Debug.Log($"area local {animationArea.localPosition} area position  {animationArea.position} area rect {animationArea.rect}");

        InvokeRepeating("Clone", .5f, 0.05f);//animationDelay, animationSpeed
    }
    enum ArchStates
    {
        Init, Asc, Desc, InitBounce, BounceBottom, BounceSide
    }
    void InitializeCard()
    {

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
                Debug.Log("bouncing " + yOffset + " y pos " + pos.y);
                yOffset = Mathf.Abs(yOffset)/Random.Range(2, Random.Range(2,4));
                xOffset += 2;
                initialArchHeight = Random.Range(5, bounceArchHeight); //bound or sum??
                archState = ArchStates.Asc;
                break;
            case ArchStates.BounceBottom: //Non case
                yOffset += 5;
                if (bounceArchHeight >= 10)
                    archState = ArchStates.Desc;
                //yOffset *= -1;
                break;
        }
        //Debug.Log("y pos " + pos.y);
        //yOffset *= bounceDirection;
        xOffsetSum += xOffset;
        yOffsetSum += yOffset;

        var childPosition = new Vector3(position.x + xOffsetSum, position.y + yOffsetSum, position.z);
        clone = cloneCount > 0 ? 
            Instantiate(cardPrefab, childPosition, Quaternion.identity, transform) : 
            Instantiate(cardPrefab, position, Quaternion.identity, transform);
        
        clone.name = "Clone";

        if (archState is ArchStates.Asc && initialArchHeight <= bounceArchHeight) initialArchHeight++;
        //if(archState is ArchStates.BounceBottom && bounceArchHeight < 13) bounceArchHeight++;
        
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
        //if (clone is not null) Debug.Log($"clone position {clone.position} clone local {clone.localPosition}");
        if (isAnimating)
        {
            bool isFullyVisible = clone is not null ? IsInsideRect(clone) : true;
            if (cloneCount >= cloneLimit || !isFullyVisible) CancelInvoke("Clone");
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

//    public bool Particle(int id, float x, float y, float sx, float sy)
//    {

//        if (sx == 0) sx = 2;

//        var cx = (id % 4) * width;
//        var cy = Mathf.Floor(id / 4) * height;

//        //this.update = function() {

//        x += sx;
//        y += sy;

//        if (x < (-cwidthhalf) || x > (canvas.width + cwidthhalf))
//        {

//            var index = particles.indexOf(this);
//            particles.splice(index, 1);

//            return false;

//        }

//        if (y > canvas.height - cheighthalf)
//        {

//            y = canvas.height - cheighthalf;
//            sy = -sy * 0.85f;

//        }

//        sy += 0.98f;

//        context.drawImage(image, cx, cy, width, height, Mathf.Floor(x - cwidthhalf), Mathf.Floor(y - cheighthalf), cwidth, cheight);

//        return true;

//        //}

//    }

//    //var image = document.createElement('img');
//    //image.src = "";
//	public void throwCard(int x, int y)
//    {

//        id = id > 0 ? id-- : 51;

//        var particle = new Particle(id, x, y, Mathf.Floor(Random.Next() * 6 - 3) * 2, - Random.Next() * 16);
//        particles.Add(particle);

//    }