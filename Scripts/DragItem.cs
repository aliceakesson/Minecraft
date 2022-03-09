using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler/*, IPointerClickHandler*/
{

    RectTransform rectTransform1; 
    RectTransform rectTransform2;

    float scaleFactor = 0.35f;

    CanvasGroup canvasGroup;

    public Vector2 defaultPos1, defaultPos2; 

    GameObject emptyObjectWithScripts; 

    string itemDropURL = "Images/itemDropImages/";

    GameObject obj; 

    void Awake()
    {

        obj = this.gameObject; 

        emptyObjectWithScripts = GameObject.Find("Scripts");

        rectTransform1 = GetComponent<RectTransform>();
        rectTransform2 = transform.parent.transform.GetChild(1).GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        defaultPos1 = rectTransform1.anchoredPosition;
        defaultPos2 = rectTransform2.anchoredPosition;

    }
    public void OnBeginDrag(PointerEventData eventData)
    {

        if(transform.parent.transform.parent.name.Equals("craftingScreen") && transform.parent.transform.parent.GetChild(4).GetChild(0).gameObject.Equals(this.gameObject))
        {
            print("result parent");

            int[,] positioningRightNow = new int[4, 2]; 

            for(int i = 0; i < 4; i++)
            {

                GameObject box = emptyObjectWithScripts.GetComponent<Crafting>().boxes_crafting[i];
                RawImage image = box.transform.GetChild(0).GetComponent<RawImage>();
                positioningRightNow[i, 0] = System.Array.IndexOf(emptyObjectWithScripts.GetComponent<PublicInfo>().blockNames, image.texture.name.Substring(0, image.texture.name.Length - 9));

                Text text = box.transform.GetChild(1).GetComponent<Text>();
                positioningRightNow[i, 1] = int.Parse(text.text);

            }

            int recipeCount = 0;
            for (int i = 0; i < emptyObjectWithScripts.GetComponent<PublicInfo>().amountOfRecipesPerBlock_small.Length; i++)
            {
                recipeCount += emptyObjectWithScripts.GetComponent<PublicInfo>().amountOfRecipesPerBlock_small[i];
            }

            for(int i = 0; i < recipeCount; i ++)
            {

                int[,] recipe = emptyObjectWithScripts.GetComponent<PublicInfo>().allRecipes_small[i];

                bool placeWasFound = true; 

                for(int j = 0; j < 4; j++)
                {
                    if(recipe[j, 0] != positioningRightNow[j, 0] || positioningRightNow[j, 1] < recipe[j, 1])
                    {
                        placeWasFound = false; 
                    }
                }

                if(placeWasFound)
                {
                    for(int j = 0; j < 4; j++)
                    {

                        GameObject box = emptyObjectWithScripts.GetComponent<Crafting>().boxes_crafting[j];
                        RawImage image = box.transform.GetChild(0).GetComponent<RawImage>();
                        Text text = box.transform.GetChild(1).GetComponent<Text>();

                        int blocksToRemove = recipe[j, 1];
                        int blocksRightNow = int.Parse(text.text);
                        int blocksLeft = blocksRightNow - blocksToRemove; 

                        text.text = blocksLeft + "";
                        if(blocksLeft > 1)
                        {
                            text.enabled = true; 
                        }
                        else
                        {
                            text.enabled = false; 
                        }

                        if(blocksLeft == 0)
                        {
                            image.texture = Resources.Load<Texture2D>(itemDropURL + "none-itemDrop");
                        }

                    }

                    break; 

                }

            }

        }

        //print("OnBeginDrag");

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;

        GetComponent<RawImage>().raycastTarget = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        //print("OnDrag");

        rectTransform1.anchoredPosition += eventData.delta / scaleFactor; 
        rectTransform2.anchoredPosition += eventData.delta / scaleFactor;

        emptyObjectWithScripts.GetComponent<Crafting>().objectThatIsBeingDragged = this.gameObject; 

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //print("OnEndDrag");

        canvasGroup.alpha = 1; 
        canvasGroup.blocksRaycasts = true;

        rectTransform1.anchoredPosition = defaultPos1;
        rectTransform2.anchoredPosition = defaultPos2;

        GetComponent<RawImage>().raycastTarget = true;
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{

    //    print("OnPointerClick");

    //    if(transform.parent.transform.parent.gameObject.name.Equals("craftingScreen"))
    //    {
    //        emptyObjectWithScripts.GetComponent<Crafting>().MoveBackFromCraftingSection(GetComponent<RawImage>(),
    //        transform.parent.transform.GetChild(1).gameObject.GetComponent<Text>(), "crafting");
    //    }
    //    else
    //    {
    //        emptyObjectWithScripts.GetComponent<Crafting>().MoveBackFromCraftingSection(GetComponent<RawImage>(),
    //        transform.parent.transform.GetChild(1).gameObject.GetComponent<Text>(), "craftingTable");
    //    }
    //}
}
