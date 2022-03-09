using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlaceAndDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform cameraPos;
    float rayCastDistance = 5;

    bool pressDestroy = false;
    bool pressPlace = false;

    Material[,] overlays = new Material[0, 0];
    List<List<Material>> overlaysList = new List<List<Material>>();

    float waitingTime = 10;
    bool lastBreakingOverlay = false;
    bool isBreakingABlock = false;

    float itemDropWidth = 0.2f;
    public Transform dirtTopSpawnParent;

    int inventoryPos = 0;
    float boxSwitchDelay = 3;
    float boxSwitchTime;
    public RectTransform inventorySelectedPos;

    string[] inventoryObjectArray = new string[9];

    string overlaysURL = "brokenOverlays/";
    string itemDropsURL = "Images/itemDropImages/";
    string itemsURL = "Prefabs/Items/";
    string particlesURL = "Images/particleImages/";
   
    public GameObject box1Parent;
    public GameObject box2Parent;
    public GameObject box3Parent;
    public GameObject box4Parent;
    public GameObject box5Parent;
    public GameObject box6Parent;
    public GameObject box7Parent;
    public GameObject box8Parent;
    public GameObject box9Parent;

    public GameObject[] boxParentArray = new GameObject[9]; 

    RawImage box1;
    RawImage box2;
    RawImage box3;
    RawImage box4;
    RawImage box5;
    RawImage box6;
    RawImage box7;
    RawImage box8;
    RawImage box9;

    Text box1Text;
    Text box2Text;
    Text box3Text;
    Text box4Text;
    Text box5Text;
    Text box6Text;
    Text box7Text;
    Text box8Text;
    Text box9Text;

    //public int[,] defaultCraftingArray = { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } }; // new int[9, 2];

    bool isCollectingItemDrop = false;

    public Material pig_defaultMaterial, pig_redMaterial, zombie_defaultMaterial, zombie_redMaterial;
    public AudioSource eatingSound;
    float eatingTime = 3; //????

    bool hasEaten = false;

    public Transform itemDropSpawnParent;

    public Transform particleSpawnParent; 
    public ParticleSystem dirtParticles;
    public Material particleMaterial;

    Player steve;
    string steveName = "stevemc11";

    public GameObject emptyObjectWithScripts;

    public RawImage craftingTableScreen, craftingScreen;
    bool isMovable = true;

    PublicInfo publicInfo; 

    void Start()
    {

        overlays = new Material[6, 10];

        publicInfo = emptyObjectWithScripts.GetComponent<PublicInfo>();

        steve = GameObject.Find(steveName).GetComponent<Player>();

        box1 = box1Parent.transform.GetChild(0).gameObject.GetComponent<RawImage>();
        box2 = box2Parent.transform.GetChild(0).gameObject.GetComponent<RawImage>();
        box3 = box3Parent.transform.GetChild(0).gameObject.GetComponent<RawImage>();
        box4 = box4Parent.transform.GetChild(0).gameObject.GetComponent<RawImage>();
        box5 = box5Parent.transform.GetChild(0).gameObject.GetComponent<RawImage>();
        box6 = box6Parent.transform.GetChild(0).gameObject.GetComponent<RawImage>();
        box7 = box7Parent.transform.GetChild(0).gameObject.GetComponent<RawImage>();
        box8 = box8Parent.transform.GetChild(0).gameObject.GetComponent<RawImage>();
        box9 = box9Parent.transform.GetChild(0).gameObject.GetComponent<RawImage>();

        box1Text = box1Parent.transform.GetChild(1).gameObject.GetComponent<Text>();
        box2Text = box2Parent.transform.GetChild(1).gameObject.GetComponent<Text>();
        box3Text = box3Parent.transform.GetChild(1).gameObject.GetComponent<Text>();
        box4Text = box4Parent.transform.GetChild(1).gameObject.GetComponent<Text>();
        box5Text = box5Parent.transform.GetChild(1).gameObject.GetComponent<Text>();
        box6Text = box6Parent.transform.GetChild(1).gameObject.GetComponent<Text>();
        box7Text = box7Parent.transform.GetChild(1).gameObject.GetComponent<Text>();
        box8Text = box8Parent.transform.GetChild(1).gameObject.GetComponent<Text>();
        box9Text = box9Parent.transform.GetChild(1).gameObject.GetComponent<Text>();

        box1Text.enabled = false; 
        box2Text.enabled = false;
        box3Text.enabled = false; 
        box4Text.enabled = false; 
        box5Text.enabled = false; 
        box6Text.enabled = false; 
        box7Text.enabled = false; 
        box8Text.enabled = false; 
        box9Text.enabled = false; 

        //starta från dirt, inte none
        for(int i = 0; i < publicInfo.blockNames.Length; i++)
        {
            overlaysList.Add(new List<Material>());

            for(int j = 0; j < 10; j++)
            {
                overlaysList[i].Add(Resources.Load<Material>(overlaysURL + publicInfo.blockNames[i] + "Overlays/" + publicInfo.blockNames[i] + "-material-" + j));

                //overlays[i, j] = Resources.Load<Material>(overlaysURL +  publicInfo.blockNames[i] + "Overlays/" + publicInfo.blockNames[i] + "-material-" + j);
            }
        }

        inventorySelectedPos.position = box1.transform.position;

        boxParentArray[0] = box1Parent;
        boxParentArray[1] = box2Parent;
        boxParentArray[2] = box3Parent;
        boxParentArray[3] = box4Parent;
        boxParentArray[4] = box5Parent;
        boxParentArray[5] = box6Parent;
        boxParentArray[6] = box7Parent;
        boxParentArray[7] = box8Parent;
        boxParentArray[8] = box9Parent;

        //for(int i = 0; i < 9; i++)
        //{
        //    boxParentArray[i].transform.GetChild(0).gameObject.AddComponent<CanvasGroup>();
        //    boxParentArray[i].transform.GetChild(0).gameObject.AddComponent<DragItem>();
        //    boxParentArray[i].transform.GetChild(0).gameObject.AddComponent<DropItemInBox>();
        //}

    }

    void Update()
    {

        if (craftingTableScreen.enabled || craftingScreen.enabled)
            isMovable = false;
        else
            isMovable = true;

        if (craftingScreen.enabled)
        {
            for (int i = 0; i < 9; i++)
            {
                GameObject box = emptyObjectWithScripts.GetComponent<Crafting>().crafting_smallInventory.transform.GetChild(i).gameObject;
                RawImage image = box.transform.GetChild(0).GetComponent<RawImage>();
                Text text = box.transform.GetChild(1).GetComponent<Text>();

                int amountOfBlocks = int.Parse(text.text);

                boxParentArray[i].transform.GetChild(0).GetComponent<RawImage>().texture = image.texture;
                boxParentArray[i].transform.GetChild(1).GetComponent<Text>().text = amountOfBlocks + "";

                if(amountOfBlocks == 0 || amountOfBlocks == 1)
                {
                    boxParentArray[i].transform.GetChild(1).GetComponent<Text>().enabled = false;
                }
                else
                {
                    boxParentArray[i].transform.GetChild(1).GetComponent<Text>().enabled = true; 
                }
                
            }
        }
        else if(craftingTableScreen.enabled)
        {
            for (int i = 0; i < 9; i++)
            {
                GameObject box = emptyObjectWithScripts.GetComponent<Crafting>().craftingTable_smallInventory.transform.GetChild(i).gameObject;
                RawImage image = box.transform.GetChild(0).GetComponent<RawImage>();
                Text text = box.transform.GetChild(1).GetComponent<Text>();

                int amountOfBlocks = int.Parse(text.text);

                boxParentArray[i].transform.GetChild(0).GetComponent<RawImage>().texture = image.texture;
                boxParentArray[i].transform.GetChild(1).GetComponent<Text>().text = amountOfBlocks + "";

                if (amountOfBlocks == 0 || amountOfBlocks == 1)
                {
                    boxParentArray[i].transform.GetChild(1).GetComponent<Text>().enabled = false;
                }
                else
                {
                    boxParentArray[i].transform.GetChild(1).GetComponent<Text>().enabled = true;
                }

            }
        }


        hasEaten = false; 

        if (Input.GetMouseButton(0) && !isBreakingABlock && isMovable)
            pressDestroy = true;
        else
            pressDestroy = false;

        if (Input.GetMouseButtonDown(1) && !pressDestroy && isMovable)
            pressPlace = true;
        else
            pressPlace = false;

        if(Input.GetKey(KeyCode.RightArrow) && boxSwitchTime == 0 && isMovable) //flytta markör i inventory
        {
            if(inventoryPos == 8)
            {
                inventoryPos = 0;
                inventorySelectedPos.position = box1.transform.position;
            }
            else
            {
                inventoryPos++;
                switch(inventoryPos)
                {
                    case 0:
                        inventorySelectedPos.position = box1.transform.position;
                        break; 
                    case 1:
                        inventorySelectedPos.position = box2.transform.position;
                        break;
                    case 2:
                        inventorySelectedPos.position = box3.transform.position;
                        break;
                    case 3:
                        inventorySelectedPos.position = box4.transform.position;
                        break;
                    case 4:
                        inventorySelectedPos.position = box5.transform.position;
                        break;
                    case 5:
                        inventorySelectedPos.position = box6.transform.position;
                        break;
                    case 6:
                        inventorySelectedPos.position = box7.transform.position;
                        break;
                    case 7:
                        inventorySelectedPos.position = box8.transform.position;
                        break;
                    case 8:
                        inventorySelectedPos.position = box9.transform.position;
                        break;
                }
            }

            boxSwitchTime = boxSwitchDelay;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && boxSwitchTime == 0 && isMovable) // flytta markör i inventory
        {
            if (inventoryPos == 0)
            {
                inventoryPos = 8;
                inventorySelectedPos.position = box9.transform.position; 
            }
            else
            {
                inventoryPos--;
                switch (inventoryPos)
                {
                    case 0:
                        inventorySelectedPos.position = box1.transform.position;
                        break;
                    case 1:
                        inventorySelectedPos.position = box2.transform.position;
                        break;
                    case 2:
                        inventorySelectedPos.position = box3.transform.position;
                        break;
                    case 3:
                        inventorySelectedPos.position = box4.transform.position;
                        break;
                    case 4:
                        inventorySelectedPos.position = box5.transform.position;
                        break;
                    case 5:
                        inventorySelectedPos.position = box6.transform.position;
                        break;
                    case 6:
                        inventorySelectedPos.position = box7.transform.position;
                        break;
                    case 7:
                        inventorySelectedPos.position = box8.transform.position;
                        break;
                    case 8:
                        inventorySelectedPos.position = box9.transform.position;
                        break;
                }
            }

            boxSwitchTime = boxSwitchDelay;
        }

        if (boxSwitchTime > 0)
            boxSwitchTime--;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            inventorySelectedPos.position = new Vector3(box1.transform.position.x, inventorySelectedPos.position.y, inventorySelectedPos.position.z);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            inventorySelectedPos.position = new Vector3(box2.transform.position.x, inventorySelectedPos.position.y, inventorySelectedPos.position.z);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            inventorySelectedPos.position = new Vector3(box3.transform.position.x, inventorySelectedPos.position.y, inventorySelectedPos.position.z);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            inventorySelectedPos.position = new Vector3(box4.transform.position.x, inventorySelectedPos.position.y, inventorySelectedPos.position.z);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            inventorySelectedPos.position = new Vector3(box5.transform.position.x, inventorySelectedPos.position.y, inventorySelectedPos.position.z);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            inventorySelectedPos.position = new Vector3(box6.transform.position.x, inventorySelectedPos.position.y, inventorySelectedPos.position.z);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            inventorySelectedPos.position = new Vector3(box7.transform.position.x, inventorySelectedPos.position.y, inventorySelectedPos.position.z);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            inventorySelectedPos.position = new Vector3(box8.transform.position.x, inventorySelectedPos.position.y, inventorySelectedPos.position.z);
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            inventorySelectedPos.position = new Vector3(box9.transform.position.x, inventorySelectedPos.position.y, inventorySelectedPos.position.z);

        Debug.DrawRay(cameraPos.position, cameraPos.TransformDirection(Vector3.forward), Color.cyan);

        RaycastHit hit;
        bool objectWasHit = Physics.Raycast(cameraPos.position, cameraPos.TransformDirection(Vector3.forward), out hit, rayCastDistance); //om någonting inom avståndet har träffats
        
        if(objectWasHit && pressDestroy && !isBreakingABlock && (hit.collider.gameObject.tag == "Block" || hit.collider.gameObject.tag == "Non-Placeable Block"))
        { 
            isBreakingABlock = true;
            StartCoroutine(WaitThenDestroy(0.075f/waitingTime, hit)); 
            isBreakingABlock = false;
        }
        else if(objectWasHit && pressDestroy && hit.collider.gameObject.tag == "Animal")
        {
            hit.collider.gameObject.GetComponent<MeshRenderer>().material = pig_redMaterial;
        }
        else if(objectWasHit && pressDestroy && hit.collider.gameObject.tag == "Mob") //OBS! fungerar hitills bara för zombies 
        {
            hit.collider.gameObject.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().material = zombie_redMaterial;
        }

        if(pressPlace)
        {
            if(hit.collider.gameObject.name.Length >= 13)
            {
                if(hit.collider.gameObject.name.Substring(0, 13).Equals("craftingTable"))
                {
                    emptyObjectWithScripts.GetComponent<Crafting>().OpenCraftingTable();
                }
                else
                {
                    PlaceOrEat(hit);
                }
            }
            else
            {
                PlaceOrEat(hit);
            }
        }

        if(hasEaten)
        {
            steve.Hunger += 3;
            steve.Saturation += 1.8f; 
        }

        if(particleSpawnParent.childCount > 10)
        {
            for (int i = 0; i < particleSpawnParent.childCount - 3; i++)
                Destroy(particleSpawnParent.GetChild(i).gameObject);
        }


    }

    void OnCollisionEnter(Collision collision)
    {

        GameObject otherObject = collision.gameObject;

        if (otherObject.tag == "ItemDrop" && !isCollectingItemDrop)
        {
            isCollectingItemDrop = true;
            CollectItemDrop(otherObject);
        }

        isCollectingItemDrop = false;
        
    }
    void CollectItemDrop(GameObject go)
    {
        FindObjectOfType<AudioManager>().Play("Collect");

        bool objectIsInInventory = false;

        for (int i = 0; i < 9; i++) // om det finns lediga plaster = collecta, annars gör inget 
        {
            if (inventoryObjectArray[i] == go.name)
            {
                bool boxHasBlock = false; 
                switch(i)
                {
                    case 0: if (!box1.texture.name.Substring(0, 4).Equals("none")) boxHasBlock = true; break; 
                    case 1: if (!box2.texture.name.Substring(0, 4).Equals("none")) boxHasBlock = true; break; 
                    case 2: if (!box3.texture.name.Substring(0, 4).Equals("none")) boxHasBlock = true; break;
                    case 3: if (!box4.texture.name.Substring(0, 4).Equals("none")) boxHasBlock = true; break; 
                    case 4: if (!box5.texture.name.Substring(0, 4).Equals("none")) boxHasBlock = true; break; 
                    case 5: if (!box6.texture.name.Substring(0, 4).Equals("none")) boxHasBlock = true; break; 
                    case 6: if (!box7.texture.name.Substring(0, 4).Equals("none")) boxHasBlock = true; break; 
                    case 7: if (!box8.texture.name.Substring(0, 4).Equals("none")) boxHasBlock = true; break; 
                    case 8: if (!box9.texture.name.Substring(0, 4).Equals("none")) boxHasBlock = true; break; 
                }

                bool isFull = false;
                switch (i)
                {
                    case 0: if (box1Text.text.Equals("64")) isFull = true; break;
                    case 1: if (box2Text.text.Equals("64")) isFull = true; break;
                    case 2: if (box3Text.text.Equals("64")) isFull = true; break;
                    case 3: if (box4Text.text.Equals("64")) isFull = true; break;
                    case 4: if (box5Text.text.Equals("64")) isFull = true; break;
                    case 5: if (box6Text.text.Equals("64")) isFull = true; break;
                    case 6: if (box7Text.text.Equals("64")) isFull = true; break;
                    case 7: if (box8Text.text.Equals("64")) isFull = true; break;
                    case 8: if (box9Text.text.Equals("64")) isFull = true; break;
                }


                if (!isFull && boxHasBlock)
                {
                    objectIsInInventory = true;
                    AddToInventory(i);
                    Destroy(go);
                    return;
                }
            }
        }

        if (!objectIsInInventory)
        {
            for (int i = 0; i < 9; i++)
            {
                if (inventoryObjectArray[i] == null)
                {
                    inventoryObjectArray[i] = go.name;
                    PutInInventory(go, i);
                    Destroy(go);
                    return;
                }
            }
        }
        

    }
    void AddToInventory(int i)
    {
        int number = 0;

        switch (i)
        {
            case 0:
                if (!box1Text.enabled) { box1Text.text = "2"; box1Text.enabled = true; }
                else { number = Int32.Parse(box1Text.text); number++; box1Text.text = number + ""; }
                break;
            case 1:
                if (!box2Text.enabled) { box2Text.text = "2"; box2Text.enabled = true; }
                else { number = Int32.Parse(box2Text.text); number++; box2Text.text = number + ""; }
                break;
            case 2:
                if (!box3Text.enabled) { box3Text.text = "2"; box3Text.enabled = true; }
                else { number = Int32.Parse(box3Text.text); number++; box3Text.text = number + ""; }
                break;
            case 3:
                if (!box4Text.enabled) { box4Text.text = "2"; box4Text.enabled = true; }
                else { number = Int32.Parse(box4Text.text); number++; box4Text.text = number + ""; }
                break;
            case 4:
                if (!box5Text.enabled) { box5Text.text = "2"; box5Text.enabled = true; }
                else { number = Int32.Parse(box5Text.text); number++; box5Text.text = number + ""; }
                break;
            case 5:
                if (!box6Text.enabled) { box6Text.text = "2"; box6Text.enabled = true; }
                else { number = Int32.Parse(box6Text.text); number++; box6Text.text = number + ""; }
                break;
            case 6:
                if (!box7Text.enabled) { box7Text.text = "2"; box7Text.enabled = true; }
                else { number = Int32.Parse(box7Text.text); number++; box7Text.text = number + ""; }
                break;
            case 7:
                if (!box8Text.enabled) { box8Text.text = "2"; box8Text.enabled = true; }
                else { number = Int32.Parse(box8Text.text); number++; box8Text.text = number + ""; }
                break;
            case 8:
                if (!box9Text.enabled) { box9Text.text = "2"; box9Text.enabled = true; }
                else { number = Int32.Parse(box9Text.text); number++; box9Text.text = number + ""; }
                break;
            default: break; 
        }

    }
    void PutInInventory(GameObject go, int i)
    {
        string itemDropName = "";

        if(go.name == "dirtTopPrefab(Clone)")
        {
            itemDropName = "dirt-itemDrop";
        }
        else
        {
            string gameObjectName = go.name.Substring(0, go.name.Length - 13);
            itemDropName = gameObjectName + "-itemDrop"; 
        }

        /*
     * 0: dirt
     * 1: dirtTop
     * 2: sand
     * 3: oakLog
     * 4: oakLeaves
     * 5: cactus
     */

        Texture2D itemDropTexture = Resources.Load<Texture2D>(itemDropsURL + itemDropName);

        switch(i)
        {
            case 0:
                box1.texture = itemDropTexture;
                break;
            case 1:
                box2.texture = itemDropTexture;
                break;
            case 2:
                box3.texture = itemDropTexture;
                break;
            case 3:
                box4.texture = itemDropTexture;
                break;
            case 4:
                box5.texture = itemDropTexture;
                break;
            case 5:
                box6.texture = itemDropTexture;
                break;
            case 6:
                box7.texture = itemDropTexture;
                break;
            case 7:
                box8.texture = itemDropTexture;
                break;
            case 8:
                box9.texture = itemDropTexture;
                break;
        }

    }

    void ChangeBlockMaterial(RaycastHit hit, int i)
    {
        try
        {
            if (hit.collider.gameObject.name.Substring(hit.collider.gameObject.name.Length - 7).Equals("(Clone)"))
                hit.collider.gameObject.name = hit.collider.gameObject.name.Substring(0, hit.collider.gameObject.name.Length - 7);
        } catch(NullReferenceException ex) { }

        try
        {
            int arrayPos = 0; 
            for(int j = 0; j < publicInfo.blockNames.Length; j++)
            {
                if (hit.collider.gameObject.name.Substring(0, hit.collider.gameObject.name.Length - 6).Equals(publicInfo.blockNames[j]))
                    arrayPos = j; 
            }
            hit.collider.GetComponent<Renderer>().material = overlaysList[arrayPos][i];
        }
        catch(NullReferenceException ex) {
            
        }
    }

    void ChangeToDefaultMaterial(Collider collider, RaycastHit hit) // vid item drop, dvsa ändringar för dirtTop
    {
        int arrayPos = 0;
        for (int j = 0; j < publicInfo.blockNames.Length; j++)
        {
            if (hit.collider.gameObject.name.Substring(0, hit.collider.gameObject.name.Length - 6).Equals(publicInfo.blockNames[j]))
            {
                
                if(j == System.Array.IndexOf(publicInfo.blockNames, "dirtTop"))
                {
                    arrayPos = System.Array.IndexOf(publicInfo.blockNames, "dirt");
                }
                else
                {
                    arrayPos = j;
                }
                    
                
            }
        }
        collider.GetComponent<Renderer>().material = overlaysList[arrayPos][9];
    }

    IEnumerator WaitThenDestroy(float sec, RaycastHit hit)
    {

        string objectName = hit.collider.gameObject.name.Substring(0, hit.collider.gameObject.name.Length - 6);

        //if(pressDestroy)
        //    FindObjectOfType<AudioManager>().Play(objectName + " Break");

        for (int i = 0; i < 9; i++)
        {
            if(pressDestroy)
            {
                yield return new WaitForSeconds(sec);
                ChangeBlockMaterial(hit, i);

                if (i == 8)
                    lastBreakingOverlay = true;
                else
                    lastBreakingOverlay = false; 
            }
            else
            {
                ChangeBlockMaterial(hit, 9);
                //FindObjectOfType<AudioManager>().Stop(objectName + " Break");
            }
        }

        if (pressDestroy && lastBreakingOverlay)
        {
            DestoryBlock(hit);
        }
    }

    void DestoryBlock(RaycastHit hit) //hit = det som ska förstöras (måste vara ett block eftersom att destroyblock endast kallas som sist efter alla ChangeBlockMaterial()
    {

        Collider collider = hit.collider;

        int type = 0; 

        try
        {
            Vector3 colCenter = collider.transform.position;

            particleMaterial.SetTexture("_MainTex", Resources.Load<Texture2D>(particlesURL + hit.collider.gameObject.name.Substring(0, hit.collider.gameObject.name.Length-6) + "-particles"));
            ParticleSystem particles = Instantiate(dirtParticles, collider.transform.position, collider.transform.rotation, particleSpawnParent);

            if(hit.collider.gameObject.tag == "Block")
            {
                Collider clone = new Collider();

                if(hit.collider.name.Length >= 13)
                {
                    if (hit.collider.gameObject.name.Substring(0, 13).Equals("dirtTopPrefab"))
                    {
                        clone = Instantiate(Resources.Load<GameObject>(itemsURL + "dirtPrefab").GetComponent<Collider>(), new Vector3(colCenter.x, colCenter.y, colCenter.z), collider.transform.rotation, itemDropSpawnParent);

                        type = System.Array.IndexOf(publicInfo.blockNames, "dirtTop");

                    }
                }
                else
                {
                    clone = Instantiate(collider, new Vector3(colCenter.x, colCenter.y, colCenter.z), collider.transform.rotation, itemDropSpawnParent);

                    type = System.Array.IndexOf(publicInfo.blockNames, hit.collider.gameObject.name.Substring(0, hit.collider.gameObject.name.Length - 6));

                }

                ChangeToDefaultMaterial(clone, hit);

                clone.gameObject.tag = "ItemDrop";
                clone.gameObject.AddComponent<ItemDrops>();

                BoxCollider boxCol = clone.gameObject.GetComponent<BoxCollider>();

                boxCol.size = new Vector3(boxCol.size.x, boxCol.size.y, boxCol.size.z);

                float sizeX = clone.GetComponent<Renderer>().bounds.size.x;
                float sizeY = clone.GetComponent<Renderer>().bounds.size.y;
                float sizeZ = clone.GetComponent<Renderer>().bounds.size.z;

                Vector3 rescale = clone.transform.localScale;

                rescale.x = itemDropWidth * rescale.x / sizeX;
                rescale.y = itemDropWidth * rescale.y / sizeY;
                rescale.z = itemDropWidth * rescale.z / sizeZ;

                clone.transform.localScale = rescale;

                Rigidbody boxRig = clone.gameObject.AddComponent<Rigidbody>();
                boxRig.freezeRotation = true;

                float forceX = UnityEngine.Random.Range(-2f, 2f);
                float forceZ = UnityEngine.Random.Range(-2f, 2f);
                boxRig.AddForce(forceX, 2f, forceZ, ForceMode.Impulse);
            }

            Destroy(collider.gameObject);

            emptyObjectWithScripts.GetComponent<TerrainGenerator>().RemoveBlockFromList(colCenter, type);

            //print("pos: " + new Vector3(colCenter.x, colCenter.y + 1, colCenter.z));
            emptyObjectWithScripts.GetComponent<TerrainGenerator>().ShowBlock(new Vector3(colCenter.x, colCenter.y + 1, colCenter.z));

            //print("pos: " + new Vector3(colCenter.x, colCenter.y - 1, colCenter.z));
            emptyObjectWithScripts.GetComponent<TerrainGenerator>().ShowBlock(new Vector3(colCenter.x, colCenter.y - 1, colCenter.z));

            //print("pos: " + new Vector3(colCenter.x + 1, colCenter.y, colCenter.z));
            emptyObjectWithScripts.GetComponent<TerrainGenerator>().ShowBlock(new Vector3(colCenter.x + 1, colCenter.y, colCenter.z));

            //print("pos: " + new Vector3(colCenter.x - 1, colCenter.y, colCenter.z));
            emptyObjectWithScripts.GetComponent<TerrainGenerator>().ShowBlock(new Vector3(colCenter.x - 1, colCenter.y, colCenter.z));

            //print("pos: " + new Vector3(colCenter.x, colCenter.y, colCenter.z + 1));
            emptyObjectWithScripts.GetComponent<TerrainGenerator>().ShowBlock(new Vector3(colCenter.x, colCenter.y, colCenter.z + 1));

            //print("pos: " + new Vector3(colCenter.x, colCenter.y, colCenter.z - 1));
            emptyObjectWithScripts.GetComponent<TerrainGenerator>().ShowBlock(new Vector3(colCenter.x, colCenter.y, colCenter.z - 1));


            steve.Exhaustion += 0.005f; 


        }
        catch (NullReferenceException nre) { }
        
    }

    void PlaceOrEat(RaycastHit hit)
    {

        int num = 0; 
        if(inventorySelectedPos.position == box1.transform.position) {
            num = 0; 
        }
        else if (inventorySelectedPos.position == box2.transform.position)
        {
            num = 1;
        }
        else if (inventorySelectedPos.position == box3.transform.position)
        {
            num = 2;
        }
        else if (inventorySelectedPos.position == box4.transform.position)
        {
            num = 3;
        }
        else if (inventorySelectedPos.position == box5.transform.position)
        {
            num = 4;
        }
        else if (inventorySelectedPos.position == box6.transform.position)
        {
            num = 5;
        }
        else if (inventorySelectedPos.position == box7.transform.position)
        {
            num = 6;
        }
        else if (inventorySelectedPos.position == box8.transform.position)
        {
            num = 7;
        }
        else if (inventorySelectedPos.position == box9.transform.position)
        {
            num = 8;
        }
        string objectName = "";

        switch (num)
        {
            case 0:
                objectName = box1.texture.name.Substring(0, box1.texture.name.Length - 9); // ex. dirt-itemDrop => dirt
                break;
            case 1:
                objectName = box2.texture.name.Substring(0, box2.texture.name.Length - 9);
                break;
            case 2:
                objectName = box3.texture.name.Substring(0, box3.texture.name.Length - 9);
                break;
            case 3:
                objectName = box4.texture.name.Substring(0, box4.texture.name.Length - 9);
                break;
            case 4:
                objectName = box5.texture.name.Substring(0, box5.texture.name.Length - 9);
                break;
            case 5:
                objectName = box6.texture.name.Substring(0, box6.texture.name.Length - 9);
                break;
            case 6:
                objectName = box7.texture.name.Substring(0, box7.texture.name.Length - 9);
                break;
            case 7:
                objectName = box8.texture.name.Substring(0, box8.texture.name.Length - 9);
                break;
            case 8:
                objectName = box9.texture.name.Substring(0, box9.texture.name.Length - 9);
                break;
            default:
                Debug.Log("något gick snett i switch sats 'objectName = box1.texture'...");
                break; 
        }

        bool continueMethod = true;

        switch (num)
        {
            case 0:
                if (box1.texture.name.Substring(0, 4).Equals("none"))
                {
                    continueMethod = false;
                    Debug.Log("end method");
                }
                break;
            case 1:
                if (box2.texture.name.Substring(0, 4).Equals("none"))
                {
                    continueMethod = false;
                    Debug.Log("end method");
                }
                break;
            case 2:
                if (box3.texture.name.Substring(0, 4).Equals("none"))
                {
                    continueMethod = false;
                    Debug.Log("end method");
                }
                break;
            case 3:
                if (box4.texture.name.Substring(0, 4).Equals("none"))
                {
                    continueMethod = false;
                    Debug.Log("end method");
                }
                break;
            case 4:
                if (box5.texture.name.Substring(0, 4).Equals("none"))
                {
                    continueMethod = false;
                    Debug.Log("end method");
                }
                break;
            case 5:
                if (box6.texture.name.Substring(0, 4).Equals("none"))
                {
                    continueMethod = false;
                    Debug.Log("end method");
                }
                break;
            case 6:
                if (box7.texture.name.Substring(0, 4).Equals("none"))
                {
                    continueMethod = false;
                    Debug.Log("end method");
                }
                break;
            case 7:
                if (box8.texture.name.Substring(0, 4).Equals("none"))
                {
                    continueMethod = false;
                    Debug.Log("end method");
                }
                break;
            case 8:
                if (box9.texture.name.Substring(0, 4).Equals("none"))
                {
                    continueMethod = false;
                    Debug.Log("end method");
                }
                break;
            default:
                Debug.Log("fel värde på num i PlaceBlock()");
                break; 
        }

        if(continueMethod) // placera block i världen eller ät
        {

            if (Resources.Load<GameObject>(itemsURL + objectName + "Prefab").tag == "Block")
            {
               
                Vector3 hitPosition = hit.point;
                float hitPos_x = hitPosition.x;
                float hitPos_y = hitPosition.y;
                float hitPos_z = hitPosition.z;

                Vector3 centerOfBlock = hit.collider.gameObject.transform.position;
                float center_x = centerOfBlock.x;
                float center_y = centerOfBlock.y;
                float center_z = centerOfBlock.z;

                float x = Math.Abs(hitPos_x - center_x);
                float y = Math.Abs(hitPos_y - center_y);
                float z = Math.Abs(hitPos_z - center_z);

                float highestValue = x;
                int n = 0;
                if (y > highestValue)
                {
                    highestValue = y;
                    n = 1;
                }
                if (z > highestValue)
                {
                    n = 2;
                }

                Vector3 blockPosition = new Vector3();
                int i = 0;

                switch (n)
                {
                    case 0:
                        if ((hitPos_x - center_x) < 0)
                            i--;
                        else
                            i++;
                        blockPosition = new Vector3(center_x + (1f * i), center_y, center_z);
                        break;
                    case 1:
                        if ((hitPos_y - center_y) < 0)
                            i--;
                        else
                            i++;
                        blockPosition = new Vector3(center_x, center_y + (1f * i), center_z);
                        break;
                    case 2:
                        if ((hitPos_z - center_z) < 0)
                            i--;
                        else
                            i++;
                        blockPosition = new Vector3(center_x, center_y, center_z + (1f * i));
                        break;
                    default:
                        break;
                }

                Collider collider = Resources.Load<GameObject>(itemsURL + objectName + "Prefab").GetComponent<BoxCollider>();
                Collider clone = Instantiate(collider, blockPosition, collider.transform.rotation, dirtTopSpawnParent);

                FindObjectOfType<AudioManager>().Play(objectName + " Place");
            }
            else if(Resources.Load<GameObject>(itemsURL + objectName + "Prefab").tag == "Food")
            {
                //eatingSound.Play(0);
                //Wait(eatingTime);
                hasEaten = true; 
            }
            else
            {
                Debug.Log("något gick snett i if(continuteMethod) {} sats i PlaceOrEat()");
            }

            switch (num)
            {
                case 0:
                    if (box1Text.enabled)
                    {
                        if (box1Text.text == "2")
                        {
                            box1Text.enabled = false;
                            box1Text.text = "1";
                        }
                        else
                        {
                            int boxText = Int32.Parse(box1Text.text);
                            boxText--;
                            box1Text.text = "" + boxText;
                        }
                    }
                    else
                    {
                        box1.texture = Resources.Load<Texture2D>(itemDropsURL + "none-itemDrop");
                        inventoryObjectArray[0] = null;
                    }
                    break;
                case 1:
                    if (box2Text.enabled)
                    {
                        if (box2Text.text == "2")
                        {
                            box2Text.enabled = false;
                            box2Text.text = "1";
                        }
                        else
                        {
                            int boxText = Int32.Parse(box2Text.text);
                            boxText--;
                            box2Text.text = "" + boxText;
                        }
                    }
                    else
                    {
                        box2.texture = Resources.Load<Texture2D>(itemDropsURL + "none-itemDrop");
                        inventoryObjectArray[1] = null;
                    }
                    break;
                case 2:
                    if (box3Text.enabled)
                    {
                        if (box3Text.text == "2")
                        {
                            box3Text.enabled = false;
                            box3Text.text = "1";
                        }
                        else
                        {
                            int boxText = Int32.Parse(box3Text.text);
                            boxText--;
                            box3Text.text = "" + boxText;
                        }
                    }
                    else
                    {
                        box3.texture = Resources.Load<Texture2D>(itemDropsURL + "none-itemDrop");
                        inventoryObjectArray[2] = null;
                    }
                    break;
                case 3:
                    if (box4Text.enabled)
                    {
                        if (box4Text.text == "2")
                        {
                            box4Text.enabled = false;
                            box4Text.text = "1";
                        }
                        else
                        {
                            int boxText = Int32.Parse(box4Text.text);
                            boxText--;
                            box4Text.text = "" + boxText;
                        }
                    }
                    else
                    {
                        box4.texture = Resources.Load<Texture2D>(itemDropsURL + "none-itemDrop");
                        inventoryObjectArray[3] = null;
                    }
                    break;
                case 4:
                    if (box5Text.enabled)
                    {
                        if (box5Text.text == "2")
                        {
                            box5Text.enabled = false;
                            box5Text.text = "1";
                        }
                        else
                        {
                            int boxText = Int32.Parse(box5Text.text);
                            boxText--;
                            box5Text.text = "" + boxText;
                        }
                    }
                    else
                    {
                        box5.texture = Resources.Load<Texture2D>(itemDropsURL + "none-itemDrop");
                        inventoryObjectArray[4] = null;
                    }
                    break;
                case 5:
                    if (box6Text.enabled)
                    {
                        if (box6Text.text == "2")
                        {
                            box6Text.enabled = false;
                            box6Text.text = "1";
                        }
                        else
                        {
                            int boxText = Int32.Parse(box6Text.text);
                            boxText--;
                            box6Text.text = "" + boxText;
                        }
                    }
                    else
                    {
                        box6.texture = Resources.Load<Texture2D>(itemDropsURL + "none-itemDrop");
                        inventoryObjectArray[5] = null;
                    }
                    break;
                case 6:
                    if (box7Text.enabled)
                    {
                        if (box7Text.text == "2")
                        {
                            box7Text.enabled = false;
                            box7Text.text = "1";
                        }
                        else
                        {
                            int boxText = Int32.Parse(box7Text.text);
                            boxText--;
                            box7Text.text = "" + boxText;
                        }
                    }
                    else
                    {
                        box7.texture = Resources.Load<Texture2D>(itemDropsURL + "none-itemDrop");
                        inventoryObjectArray[6] = null;
                    }
                    break;
                case 7:
                    if (box8Text.enabled)
                    {
                        if (box8Text.text == "2")
                        {
                            box8Text.enabled = false;
                            box8Text.text = "1";
                        }
                        else
                        {
                            int boxText = Int32.Parse(box8Text.text);
                            boxText--;
                            box8Text.text = "" + boxText;
                        }
                    }
                    else
                    {
                        box8.texture = Resources.Load<Texture2D>(itemDropsURL + "none-itemDrop");
                        inventoryObjectArray[7] = null;
                    }
                    break;
                case 8:
                    if (box9Text.enabled)
                    {
                        if (box9Text.text == "2")
                        {
                            box9Text.enabled = false;
                            box9Text.text = "1";
                        }
                        else
                        {
                            int boxText = Int32.Parse(box9Text.text);
                            boxText--;
                            box9Text.text = "" + boxText;
                        }
                    }
                    else
                    {
                        box9.texture = Resources.Load<Texture2D>(itemDropsURL + "none-itemDrop");
                        inventoryObjectArray[8] = null;
                    }
                    break;
                default:
                    Debug.Log("fel värde på num i PlaceBlock()");
                    break;
            }

        } //if continue method 

    }
}
