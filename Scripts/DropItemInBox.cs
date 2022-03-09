using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class DropItemInBox : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    //float n = 149.52f;
    string itemDropURL = "Images/itemDropImages/";

    public GameObject emptyObjectWithScripts; 

    void Awake()
    {
        emptyObjectWithScripts = GameObject.Find("Scripts");
    }
    public void OnDrop(PointerEventData eventData)
    {
        //print("OnDrop");

        if (eventData.pointerDrag != null)
        { 

            string itemDropName = eventData.pointerDrag.GetComponent<RawImage>().texture.name;
            string blockName = itemDropName.Substring(0, itemDropName.Length - 9);

            if(!blockName.Equals("none"))
            {

                RawImage image = GetComponent<RawImage>();
                string ownBlock = image.texture.name.Substring(0, image.texture.name.Length - 9);
                int amountOfBlocks = int.Parse(eventData.pointerDrag.transform.parent.transform.GetChild(1).GetComponent<Text>().text);

                if (ownBlock.Equals("none"))
                {
                    image.texture = Resources.Load<Texture2D>(itemDropURL + blockName + "-itemDrop");
                    Text text = transform.parent.transform.GetChild(1).GetComponent<Text>();
                    text.text = "1";

                    if (amountOfBlocks > 1)
                    {
                        text.enabled = true;
                        text.text = amountOfBlocks + "";
                    }

                    eventData.pointerDrag.GetComponent<RawImage>().texture = Resources.Load<Texture2D>(itemDropURL + "none-itemDrop");
                    eventData.pointerDrag.transform.parent.transform.GetChild(1).GetComponent<Text>().text = "0"; 
                    eventData.pointerDrag.transform.parent.transform.GetChild(1).GetComponent<Text>().enabled = false; 
                }
                else if(ownBlock.Equals(blockName))
                {
                    Text text = transform.parent.transform.GetChild(1).GetComponent<Text>();
                    text.enabled = true;
                    int blockRightNow = int.Parse(text.text);
                    text.text = (blockRightNow + amountOfBlocks) + "";

                    eventData.pointerDrag.GetComponent<RawImage>().texture = Resources.Load<Texture2D>(itemDropURL + "none-itemDrop");
                    eventData.pointerDrag.transform.parent.transform.GetChild(1).GetComponent<Text>().text = "0";
                    eventData.pointerDrag.transform.parent.transform.GetChild(1).GetComponent<Text>().enabled = false;

                }

            }

            //eventData.pointerDrag.GetComponent<DragItem>().defaultPos1 += new Vector2(n, 0);
            //eventData.pointerDrag.GetComponent<DragItem>().defaultPos2 += new Vector2(n, 0);

        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        GameObject dragObject = emptyObjectWithScripts.GetComponent<Crafting>().objectThatIsBeingDragged;
        GameObject dropObject = eventData.pointerClick; 

        if (dragObject != null)
        {

            string dragItemDropName = dragObject.GetComponent<RawImage>().texture.name;
            string dragName = dragItemDropName.Substring(0, dragItemDropName.Length - 9);

            if (!dragName.Equals("none"))
            {

                RawImage dropImage = dropObject.GetComponent<RawImage>();
                RawImage dragImage = dragObject.GetComponent<RawImage>();

                string dropName = dropImage.texture.name.Substring(0, dropImage.texture.name.Length - 9);

                int dragAmountOfBlocks = int.Parse(dragObject.transform.parent.transform.GetChild(1).GetComponent<Text>().text);
                if(!dragObject.transform.parent.transform.GetChild(1).GetComponent<Text>().enabled)
                {
                    dragAmountOfBlocks = 1; 
                }

                if (dropName.Equals("none"))
                {

                    dropImage.texture = Resources.Load<Texture2D>(itemDropURL + dragName + "-itemDrop");
                    dropObject.transform.parent.transform.GetChild(1).GetComponent<Text>().text = "1";

                    if (dragAmountOfBlocks == 1)
                    {
                        dragImage.texture = Resources.Load<Texture2D>(itemDropURL + "none-itemDrop");
                        dragObject.transform.parent.transform.GetChild(1).GetComponent<Text>().text = "0";
                        dragObject.transform.parent.transform.GetChild(1).GetComponent<Text>().enabled = false;
                    }
                    else
                    {
                        dragObject.transform.parent.transform.GetChild(1).GetComponent<Text>().text = (dragAmountOfBlocks - 1) + "";
                    }

                    if(int.Parse(dragObject.transform.parent.transform.GetChild(1).GetComponent<Text>().text) == 1)
                    {
                        dragObject.transform.parent.transform.GetChild(1).GetComponent<Text>().enabled = false; 
                    }

                }
                else if (dropName.Equals(dragName))
                {

                    print("same object");

                    Text text = dropObject.transform.parent.transform.GetChild(1).GetComponent<Text>();
                    text.enabled = true;

                    int dropBlocksRightNow = int.Parse(text.text);
                    text.text = (dropBlocksRightNow + 1) + "";

                    if (dragAmountOfBlocks == 1)
                    {
                        print("one block move");

                        dragImage.texture = Resources.Load<Texture2D>(itemDropURL + "none-itemDrop");
                        dragObject.transform.parent.transform.GetChild(1).GetComponent<Text>().text = "0";
                        dragObject.transform.parent.transform.GetChild(1).GetComponent<Text>().enabled = false;

                        dragObject.GetComponent<DragItem>().OnEndDrag(eventData);

                    }
                    else
                    {
                        dragObject.transform.parent.transform.GetChild(1).GetComponent<Text>().text = (dragAmountOfBlocks - 1) + "";
                    }

                    if (int.Parse(dragObject.transform.parent.transform.GetChild(1).GetComponent<Text>().text) == 1)
                    {
                        dragObject.transform.parent.transform.GetChild(1).GetComponent<Text>().enabled = false;
                    }

                }

            }

        }

    }

}
