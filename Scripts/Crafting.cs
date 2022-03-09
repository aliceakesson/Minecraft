using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{

    public RawImage craftingTableScreen;
    public RawImage craftingScreen;
    public Image craftingTableBackground;

    int[,] craftingTableArray = { { 0, 0 }, { 0, 0 }, { 0, 0 },
                             { 0, 0 }, { 0, 0 }, { 0, 0 },
                             { 0, 0 }, { 0, 0 }, { 0, 0 } }; // new int[9, 2];

    int[,] craftingArray = { { 0, 0 }, { 0, 0 },
                             { 0, 0 }, { 0, 0 } }; //new int[4, 2]

    public GameObject[] boxes_craftingTable;
    public GameObject[] boxes_crafting;

    RawImage[] boxImageArray_craftingTable = new RawImage[9];
    RawImage[] boxImageArray_crafting = new RawImage[4];

    Text[] boxTextArray_craftingTable = new Text[9];
    Text[] boxTextArray_crafting = new Text[4];

    RawImage resultImage_craftingTable;
    RawImage resultImage_crafting;

    Text resultText_craftingTable;
    Text resultText_crafting;

    float xPosChange = 51, yPosChange = 53; // samma för craftingtable och crafting
    float startX_craftingTable, startY_craftingTable;
    float startX_crafting, startY_crafting;

    public GameObject player;

    string itemDropURL = "Images/itemDropImages/";
    public GameObject crafting_smallInventory;
    GameObject[] boxes_crafting_smallInventory = new GameObject[9];
    RawImage[] boxImageArray_crafting_smallInventory = new RawImage[9];
    Text[] boxTextArray_crafting_smallInventory = new Text[9];

    public GameObject craftingTable_smallInventory;
    GameObject[] boxes_craftingTable_smallInventory = new GameObject[9];
    RawImage[] boxImageArray_craftingTable_smallInventory = new RawImage[9];
    Text[] boxTextArray_craftingTable_smallInventory = new Text[9];

    PublicInfo publicInfo;

    public GameObject objectThatIsBeingDragged; 

    void Start()
    {

        publicInfo = GetComponent<PublicInfo>();

        craftingTableScreen.enabled = false;
        craftingTableBackground.enabled = false;
        craftingScreen.enabled = false;

        for (int i = 0; i < 9; i++)
        {
            boxImageArray_craftingTable[i] = craftingTableScreen.transform.GetChild(i).transform.GetChild(0).GetComponent<RawImage>();
            boxTextArray_craftingTable[i] = craftingTableScreen.transform.GetChild(i).transform.GetChild(1).GetComponent<Text>();

            boxImageArray_craftingTable[i].enabled = false;
            boxTextArray_craftingTable[i].enabled = false;

            boxImageArray_craftingTable[i].gameObject.AddComponent<CanvasGroup>();

            boxImageArray_craftingTable[i].gameObject.AddComponent<DragItem>();
            boxImageArray_craftingTable[i].gameObject.AddComponent<DropItemInBox>();
        }

        resultImage_craftingTable = craftingTableScreen.transform.GetChild(9).transform.GetChild(0).GetComponent<RawImage>();
        resultText_craftingTable = craftingTableScreen.transform.GetChild(9).transform.GetChild(1).GetComponent<Text>();

        resultImage_craftingTable.enabled = false;
        resultText_craftingTable.enabled = false;

        resultImage_craftingTable.gameObject.AddComponent<CanvasGroup>();

        resultImage_craftingTable.gameObject.AddComponent<DragItem>();
        resultImage_craftingTable.gameObject.AddComponent<DropItemInBox>();

        startX_craftingTable = boxes_craftingTable[0].GetComponent<RectTransform>().anchoredPosition.x;
        startY_craftingTable = boxes_craftingTable[0].GetComponent<RectTransform>().anchoredPosition.y;

        /****************/

        for (int i = 0; i < 4; i++)
        {
            boxImageArray_crafting[i] = craftingScreen.transform.GetChild(i).transform.GetChild(0).GetComponent<RawImage>();
            boxTextArray_crafting[i] = craftingScreen.transform.GetChild(i).transform.GetChild(1).GetComponent<Text>();

            boxImageArray_crafting[i].enabled = false;
            boxTextArray_crafting[i].enabled = false;

            boxImageArray_crafting[i].gameObject.AddComponent<CanvasGroup>();

            boxImageArray_crafting[i].gameObject.AddComponent<DragItem>();
            boxImageArray_crafting[i].gameObject.AddComponent<DropItemInBox>();
        }

        resultImage_crafting = craftingScreen.transform.GetChild(4).transform.GetChild(0).GetComponent<RawImage>();
        resultText_crafting = craftingScreen.transform.GetChild(4).transform.GetChild(1).GetComponent<Text>();

        resultImage_crafting.enabled = false;
        resultText_crafting.enabled = false;

        resultImage_crafting.gameObject.AddComponent<CanvasGroup>();

        resultImage_crafting.gameObject.AddComponent<DragItem>();
        resultImage_crafting.gameObject.AddComponent<DropItemInBox>();

        startX_crafting = boxes_crafting[0].GetComponent<RectTransform>().anchoredPosition.x;
        startY_crafting = boxes_crafting[0].GetComponent<RectTransform>().anchoredPosition.y;

        /*****************/

        for (int i = 0; i < 9; i++)
        {
            boxes_crafting_smallInventory[i] = crafting_smallInventory.transform.GetChild(i).gameObject;

            boxImageArray_crafting_smallInventory[i] = boxes_crafting_smallInventory[i].transform.GetChild(0).GetComponent<RawImage>();
            boxTextArray_crafting_smallInventory[i] = boxes_crafting_smallInventory[i].transform.GetChild(1).GetComponent<Text>();

            boxImageArray_crafting_smallInventory[i].enabled = false;
            boxTextArray_crafting_smallInventory[i].enabled = false;

            boxImageArray_crafting_smallInventory[i].gameObject.AddComponent<CanvasGroup>();

            boxImageArray_crafting_smallInventory[i].gameObject.AddComponent<DragItem>();
            boxImageArray_crafting_smallInventory[i].gameObject.AddComponent<DropItemInBox>();

            boxTextArray_crafting_smallInventory[i].raycastTarget = false;

        }

        /*****************/

        for (int i = 0; i < 9; i++)
        {
            boxes_craftingTable_smallInventory[i] = craftingTable_smallInventory.transform.GetChild(i).gameObject;

            boxImageArray_craftingTable_smallInventory[i] = boxes_craftingTable_smallInventory[i].transform.GetChild(0).GetComponent<RawImage>();
            boxTextArray_craftingTable_smallInventory[i] = boxes_craftingTable_smallInventory[i].transform.GetChild(1).GetComponent<Text>();

            boxImageArray_craftingTable_smallInventory[i].enabled = false;
            boxTextArray_craftingTable_smallInventory[i].enabled = false;

            boxImageArray_craftingTable_smallInventory[i].gameObject.AddComponent<CanvasGroup>();

            boxImageArray_craftingTable_smallInventory[i].gameObject.AddComponent<DragItem>();
            boxImageArray_craftingTable_smallInventory[i].gameObject.AddComponent<DropItemInBox>();

            boxTextArray_craftingTable_smallInventory[i].raycastTarget = false;

        }

    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E) && !craftingTableScreen.enabled)
        {
            craftingScreen.enabled = !craftingScreen.IsActive();

            if (craftingScreen.enabled)
            {

                for (int i = 0; i < 4; i++)
                {
                    boxImageArray_crafting[i].enabled = true;
                    boxTextArray_crafting[i].enabled = false;
                }

                resultImage_crafting.enabled = true;
                resultText_crafting.enabled = false;

                GameObject[] boxParentArray = player.GetComponent<PlaceAndDestroy>().boxParentArray;
                for (int i = 0; i < 9; i++)
                {

                    boxImageArray_crafting_smallInventory[i].enabled = true;

                    boxImageArray_crafting_smallInventory[i].texture = boxParentArray[i].transform.GetChild(0).GetComponent<RawImage>().texture;
                    boxTextArray_crafting_smallInventory[i].text = boxParentArray[i].transform.GetChild(1).GetComponent<Text>().text;

                    int amountOfBlocks = int.Parse(boxTextArray_crafting_smallInventory[i].text);
                    if (amountOfBlocks == 0 || amountOfBlocks == 1)
                    {
                        boxTextArray_crafting_smallInventory[i].enabled = false;
                    }
                    else
                    {
                        boxTextArray_crafting_smallInventory[i].enabled = true;
                    }

                }
            }
            else
            {

                for (int i = 0; i < 4; i++)
                {

                    PutBackInInventory(boxImageArray_crafting[i], boxTextArray_crafting[i]);

                    boxImageArray_crafting[i].enabled = false;
                    boxTextArray_crafting[i].enabled = false;

                }

                PutBackInInventory(resultImage_crafting, resultText_crafting);

                resultImage_crafting.enabled = false;
                resultText_crafting.enabled = false;

                for (int i = 0; i < 9; i++)
                {
                    boxImageArray_crafting_smallInventory[i].enabled = false;
                    boxTextArray_crafting_smallInventory[i].enabled = false;
                }

            }
        }
        else if(Input.GetKeyDown(KeyCode.E) && craftingTableScreen.enabled)
        {
            craftingTableScreen.enabled = false; 

            for(int i = 0; i < 9; i++)
            {

                PutBackInInventory(boxImageArray_craftingTable[i], boxTextArray_craftingTable[i]);

                boxImageArray_craftingTable[i].enabled = false;
                boxTextArray_craftingTable[i].enabled = false; 

            }

            PutBackInInventory(resultImage_craftingTable, resultText_craftingTable);

            resultImage_craftingTable.enabled = false; 
            resultText_craftingTable.enabled = false;

            for (int i = 0; i < 9; i++)
            {
                boxImageArray_craftingTable_smallInventory[i].enabled = false;
                boxTextArray_craftingTable_smallInventory[i].enabled = false;
            }

        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(craftingScreen.enabled)
            {

                craftingScreen.enabled = false; 

                for (int i = 0; i < 4; i++)
                {

                    PutBackInInventory(boxImageArray_crafting[i], boxTextArray_crafting[i]);

                    boxImageArray_crafting[i].enabled = false;
                    boxTextArray_crafting[i].enabled = false;

                }

                PutBackInInventory(resultImage_crafting, resultText_crafting);

                resultImage_crafting.enabled = false;
                resultText_crafting.enabled = false;

                for (int i = 0; i < 9; i++)
                {
                    boxImageArray_crafting_smallInventory[i].enabled = false;
                    boxTextArray_crafting_smallInventory[i].enabled = false;
                }
            }
            else if(craftingTableScreen.enabled)
            {
                craftingTableScreen.enabled = false;

                for (int i = 0; i < 9; i++)
                {

                    PutBackInInventory(boxImageArray_craftingTable[i], boxTextArray_craftingTable[i]);

                    boxImageArray_craftingTable[i].enabled = false;
                    boxTextArray_craftingTable[i].enabled = false;

                }

                PutBackInInventory(resultImage_craftingTable, resultText_craftingTable);

                resultImage_craftingTable.enabled = false;
                resultText_craftingTable.enabled = false;

                for (int i = 0; i < 9; i++)
                {
                    boxImageArray_crafting_smallInventory[i].enabled = false;
                    boxTextArray_crafting_smallInventory[i].enabled = false;
                }
            }
        }

        if (craftingTableScreen.enabled || craftingScreen.enabled)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            craftingTableBackground.enabled = true;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            craftingTableBackground.enabled = false;
        }

        CheckCraft();

    }

    public void OpenCraftingTable()
    {

        craftingTableScreen.enabled = true; 

        for (int i = 0; i < 4; i++)
        {
            boxImageArray_craftingTable[i].enabled = true;
            boxTextArray_craftingTable[i].enabled = false;
        }

        resultImage_craftingTable.enabled = true;
        resultText_craftingTable.enabled = false;

        GameObject[] boxParentArray = player.GetComponent<PlaceAndDestroy>().boxParentArray;
        for (int i = 0; i < 9; i++)
        {

            boxImageArray_craftingTable_smallInventory[i].enabled = true;

            boxImageArray_craftingTable_smallInventory[i].texture = boxParentArray[i].transform.GetChild(0).GetComponent<RawImage>().texture;
            boxTextArray_craftingTable_smallInventory[i].text = boxParentArray[i].transform.GetChild(1).GetComponent<Text>().text;

            int amountOfBlocks = int.Parse(boxTextArray_craftingTable_smallInventory[i].text);
            if (amountOfBlocks == 0 || amountOfBlocks == 1)
            {
                boxTextArray_craftingTable_smallInventory[i].enabled = false;
            }
            else
            {
                boxTextArray_craftingTable_smallInventory[i].enabled = true;
            }

        }
    }

    void PutBackInInventory(RawImage image, Text text)
    {

        if (!image.texture.name.Substring(0, 4).Equals("none"))
        {

            GameObject[] boxParentArray = player.GetComponent<PlaceAndDestroy>().boxParentArray;

            string blockName = image.texture.name.Substring(0, image.texture.name.Length - 9);
            int amountOfBlocks = int.Parse(text.text);

            bool placeWasFound = false;

            for (int i = 0; i < 9; i++)
            {
                string blockName2 = boxParentArray[i].transform.GetChild(0).GetComponent<RawImage>().texture.name.Substring(0, image.texture.name.Length - 9);
                if (blockName.Equals(blockName2))
                {
                    Text text2 = boxParentArray[i].transform.GetChild(1).GetComponent<Text>();
                    int blocksRightNow = int.Parse(text2.text);

                    if (text.enabled) // amountOfBlocks > 1
                    {
                        text2.text = (blocksRightNow + amountOfBlocks) + "";
                    }
                    else // amountOfBlocks == 1
                    {
                        text2.text = (blocksRightNow + 1) + "";
                    }

                    int blocks = int.Parse(text2.text);
                    if (blocks > 1)
                    {
                        text2.enabled = true;
                    }
                    else
                    {
                        text2.enabled = false;
                    }

                    placeWasFound = true;
                }
            }

            if (!placeWasFound)
            {
                for (int i = 0; i < 9; i++)
                {
                    bool placeFound = false;

                    string blockName2 = boxParentArray[i].transform.GetChild(0).GetComponent<RawImage>().texture.name.Substring(0, image.texture.name.Length - 9);
                    if (blockName2.Equals("none"))
                    {

                        Text text2 = boxParentArray[i].transform.GetChild(1).GetComponent<Text>();

                        boxParentArray[i].transform.GetChild(0).GetComponent<RawImage>().texture = image.texture;
                        text2.text = text.text;

                        int blocks = int.Parse(text2.text);
                        if (blocks > 1)
                        {
                            text2.enabled = true;
                        }
                        else
                        {
                            text2.enabled = false;
                        }

                        placeFound = true;
                    }

                    if (placeFound)
                        break;
                }
            }

            image.texture = Resources.Load<Texture2D>(itemDropURL + "none-itemDrop");
            text.text = "0";
            text.enabled = false; 

        }

    }
    public void MoveBackFromCraftingSection(RawImage image, Text text, string menu)
    {
        if (!image.texture.name.Substring(0, 4).Equals("none"))
        {

            GameObject[] inventoryArray = boxes_crafting_smallInventory; 

            if(menu.Equals("craftingTable"))
            {
                inventoryArray = boxes_craftingTable_smallInventory;
            }

            string blockName = image.texture.name.Substring(0, image.texture.name.Length - 9);
            int amountOfBlocks = int.Parse(text.text);

            bool placeWasFound = false;

            for (int i = 0; i < 9; i++)
            {
                string blockName2 = inventoryArray[i].transform.GetChild(0).GetComponent<RawImage>().texture.name.Substring(0, image.texture.name.Length - 9);
                if (blockName.Equals(blockName2))
                {
                    Text text2 = inventoryArray[i].transform.GetChild(1).GetComponent<Text>();
                    int blocksRightNow = int.Parse(text2.text);

                    if (text.enabled) // amountOfBlocks > 1
                    {
                        text2.text = (blocksRightNow + amountOfBlocks) + "";
                    }
                    else // amountOfBlocks == 1
                    {
                        text2.text = (blocksRightNow + 1) + "";
                    }

                    int blocks = int.Parse(text2.text);
                    if (blocks > 1)
                    {
                        text2.enabled = true;
                    }
                    else
                    {
                        text2.enabled = false;
                    }

                    placeWasFound = true;
                }
            }

            if (!placeWasFound)
            {
                for (int i = 0; i < 9; i++)
                {
                    bool placeFound = false;

                    string blockName2 = inventoryArray[i].transform.GetChild(0).GetComponent<RawImage>().texture.name.Substring(0, image.texture.name.Length - 9);
                    if (blockName2.Equals("none"))
                    {

                        Text text2 = inventoryArray[i].transform.GetChild(1).GetComponent<Text>();

                        inventoryArray[i].transform.GetChild(0).GetComponent<RawImage>().texture = image.texture;
                        text2.text = text.text;

                        int blocks = int.Parse(text2.text);
                        if (blocks > 1)
                        {
                            text2.enabled = true;
                        }
                        else
                        {
                            text2.enabled = false;
                        }

                        placeFound = true;
                    }

                    if (placeFound)
                        break;
                }
            }

            image.texture = Resources.Load<Texture2D>(itemDropURL + "none-itemDrop");
            text.text = "0";
            text.enabled = false; 

        }
    }

    void CheckCraft()
    {

        if (craftingScreen.enabled)
        {

            int recipeCount = 0; 
            for(int i = 0; i < publicInfo.amountOfRecipesPerBlock_small.Length; i++)
            {
                recipeCount += publicInfo.amountOfRecipesPerBlock_small[i];
            }

            for (int i = 0; i < recipeCount; i++)
            {

                int[,] positioningRightNow = new int[4, 2];
                int[,] recipe = publicInfo.allRecipes_small[i];

                //print("recipe: " + "{ " + recipe[0, 0] + ", " + recipe[0, 1] + "}"
                //                 + "{ " + recipe[1, 0] + ", " + recipe[1, 1] + "}"
                //                 + "{ " + recipe[2, 0] + ", " + recipe[2, 1] + "}"
                //                 + "{ " + recipe[3, 0] + ", " + recipe[3, 1] + "}");

                bool craftable = true;

                for (int j = 0; j < 4; j++)
                {
                    string blockName = boxImageArray_crafting[j].texture.name.Substring(0, boxImageArray_crafting[j].texture.name.Length - 9); // ex dirt-itemDrop -> dirt 
                    positioningRightNow[j, 0] = System.Array.IndexOf(publicInfo.blockNames, blockName);
                    positioningRightNow[j, 1] = int.Parse(boxTextArray_crafting[j].text);

                    if (recipe[j, 0] != positioningRightNow[j, 0] || positioningRightNow[j, 1] < recipe[j, 1])
                    {
                        craftable = false;
                    }
                }

                //print("positioningRightNow: " + "{ " + positioningRightNow[0, 0] + ", " + positioningRightNow[0, 1] + "}"
                //                              + "{ " + positioningRightNow[1, 0] + ", " + positioningRightNow[1, 1] + "}"
                //                              + "{ " + positioningRightNow[2, 0] + ", " + positioningRightNow[2, 1] + "}"
                //                              + "{ " + positioningRightNow[3, 0] + ", " + positioningRightNow[3, 1] + "}");

                if (craftable)
                {

                    //print("craftable");

                    int k = 0;
                    int index = 0; 
                    while(k < (i + 1))
                    {
                        k += publicInfo.amountOfRecipesPerBlock_small[index];
                        index++; 
                    }

                    if (index > 0)
                        index--;


                    Texture2D texture = Resources.Load<Texture2D>(itemDropURL + publicInfo.blockNames[index] + "-itemDrop");
                    int amountsOfBlocks = publicInfo.itemsPerCraft[index];
                    if(amountsOfBlocks > 1)
                    {
                        resultText_crafting.enabled = true; 
                    }
                    else
                    {
                        resultText_crafting.enabled = false; 
                    }

                    resultImage_crafting.texture = texture;
                    resultImage_craftingTable.enabled = true; 

                    resultText_crafting.text = amountsOfBlocks + "";

                    break;
                }
                else
                {
                    resultImage_crafting.texture = Resources.Load<Texture2D>(itemDropURL + "none-itemDrop");
                    resultText_crafting.text = "0";
                    resultText_crafting.enabled = false; 
                }

            }
        }
        else if (craftingTableScreen.enabled)
        {
            //checkcraft craftingTableScreen 
        }


    }

}
