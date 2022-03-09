using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* idé optimering
     * boxcollider för bara hela blocket 
     * kolla alla sidor av block
     * om någon sida kolliderar med annat block --> ta bort sida 
     * vid collisionexit ta tillbaka alla sidor 
 */

[System.Serializable]
public class Wave
{
    public float seed;
    public float frequency;
    public float amplitude;
}

public class TerrainGenerator : MonoBehaviour
{

    public int width = 16; // 16
    int height = 1; // 1
    float scaler = 40;
    float multiplier = 10;

    public GameObject dirtTop;
    public GameObject dirt;
    public Transform blockSpawnParent;

    float offsetX = 0f, offsetZ = 0f;

    int minTrees = 0, maxTrees = 6;
    public GameObject oakLog;
    public GameObject oakLeaves;
    public GameObject sand;
    public GameObject cactus;

    int minX_Discovered = 0, maxX_Discovered = 0, minZ_Discovered = 0, maxZ_Discovered = 0;
    int start_XZ_Index = 2;
    int discoverMargin = 10; // 10 (tror att discoverMargin måste vara större än width), ny som funkar: 50

    [SerializeField]
    Wave[] waves;

    float biomeOffsetX = 0f, biomeOffsetZ = 0f;
    float biomeScaler = 150;
    float desertPossibility = 0.3f;

    float x_RotationBlock = -90; //-90 för prefab, 0 för test 
    

    public List<List<List<List<List<List<List<bool>>>>>>> xpos_zpos = new List<List<List<List<List<List<List<bool>>>>>>>();
    public List<List<List<List<List<List<List<bool>>>>>>> xpos_zneg = new List<List<List<List<List<List<List<bool>>>>>>>();
    public List<List<List<List<List<List<List<bool>>>>>>> xneg_zpos = new List<List<List<List<List<List<List<bool>>>>>>>();
    public List<List<List<List<List<List<List<bool>>>>>>> xneg_zneg = new List<List<List<List<List<List<List<bool>>>>>>>();
    /*
    - fjärdedel av "graf" dvsa xneg_zpos, xneg_zneg osv.
        - chunks x "rank" (nu har man en lista med chunks som delar x position)
            - chunks z "rank" (nu har man en exakt chunk) == mer organiserat sätt att dela upp miljontals block (för 16x16x256 chunk = 65 536 block)
                - en för positiva y värden, en för negativa  OBS! NEG FÖRST, SEN POS 
                    - x position för block i chunk 
                            - y position för block i chunk
                                - z position för block i chunk = exakt position för block i exakt vald chunk 
     */

    /*
     * 0 = dirt 
     * 1 = dirtTop
     * 2 = sand 
     * 3 = oak
     * 4 = oak leaves
     * 5 = cactus
     */

    //från timeandspawn.cs
    //ta bort block som är för långt borta + ev.block optimering rendering 
    int despawnBlockDistance = 40;
    float positionMargin = 0.1f;

    public GameObject player;

    float boxCollideMargin = 0.49f; // 0.2f
    float halfOfBoxWidth_plusMargin = 0.3f; 

    int type = 0;
    PublicInfo publicInfo;

    string itemsURL = "Prefabs/Items/";

    void Start()
    {
        
        publicInfo = GetComponent<PublicInfo>();

        offsetX = Random.Range(0, 99999f);
        offsetZ = Random.Range(0, 99999f);

        print("offsets: " + offsetX + ", " + offsetZ);

        biomeOffsetX = Random.Range(0, 99999f);
        biomeOffsetZ = Random.Range(0, 99999f);

        for (int i = -start_XZ_Index; i <= start_XZ_Index; i++)
        {
            for (int j = -start_XZ_Index; j <= start_XZ_Index; j++)
            {
                CreateChunk(new Vector3(i * width, 0, j * width)); // chunks skapas RUNT positonen dvsa om position = (0,0,0) kommer chunk runt den positionen 
            }
        }

        //CreateChunk(new Vector3(0, 0, 0));

        minX_Discovered = -start_XZ_Index;
        maxX_Discovered = start_XZ_Index;
        minZ_Discovered = -start_XZ_Index;
        maxZ_Discovered = start_XZ_Index;


    }

    void FixedUpdate()
    {

        if (transform.position.x > (maxX_Discovered * width - discoverMargin))
        {

            maxX_Discovered++;

            for (int i = minZ_Discovered; i <= maxZ_Discovered; i++)
            {
                CreateChunk(new Vector3(maxX_Discovered * width, 0, i * width));
            }
        }
        else if (transform.position.x < (minX_Discovered * width + discoverMargin))
        {

            minX_Discovered--;

            for (int i = minZ_Discovered; i <= maxZ_Discovered; i++)
            {
                CreateChunk(new Vector3(minX_Discovered * width, 0, i * width));
            }
        }
        else if (transform.position.z > (maxZ_Discovered * width - discoverMargin))
        {

            maxZ_Discovered++;

            for (int i = minX_Discovered; i <= maxX_Discovered; i++)
            {
                CreateChunk(new Vector3(i * width, 0, maxZ_Discovered * width));
            }
        }
        else if (transform.position.z < (minZ_Discovered * width + discoverMargin))
        {

            minZ_Discovered--;

            for (int i = minX_Discovered; i <= maxX_Discovered; i++)
            {
                CreateChunk(new Vector3(i * width, 0, minZ_Discovered * width));
            }
        }

        //int respawnDistance = despawnBlockDistance - 1; // despawnBlockDistance - 1
        //int heightIndex = 2; // height = 2 * heightIndex + 1

        //List<Vector3> positions = new List<Vector3>();

        //for (int x = -respawnDistance; x <= respawnDistance; x++)
        //{
        //    int z = respawnDistance;
        //    for (int y = -heightIndex; y <= heightIndex; y++)
        //    {
        //        if (Mathf.Sqrt(x * x + z * z) <= respawnDistance * Mathf.Sqrt(2))
        //            positions.Add(new Vector3((int)player.transform.position.x + x, (int)player.transform.position.y + y, (int)player.transform.position.z + z));
        //    }

        //    z = -respawnDistance;
        //    for (int y = -heightIndex; y <= heightIndex; y++)
        //    {
        //        if (Mathf.Sqrt(x * x + z * z) <= respawnDistance * Mathf.Sqrt(2))
        //            positions.Add(new Vector3((int)player.transform.position.x + x, (int)player.transform.position.y + y, (int)player.transform.position.z + z));
        //    }
        //}

        //for (int z = -respawnDistance; z <= respawnDistance; z++)
        //{
        //    int x = respawnDistance;
        //    for (int y = -heightIndex; y <= heightIndex; y++)
        //    {
        //        if (Mathf.Sqrt(x * x + z * z) <= respawnDistance * Mathf.Sqrt(2))
        //            positions.Add(new Vector3((int)player.transform.position.x + x, (int)player.transform.position.y + y, (int)player.transform.position.z + z));
        //    }

        //    x = -respawnDistance;
        //    for (int y = -heightIndex; y <= heightIndex; y++)
        //    {
        //        if (Mathf.Sqrt(x * x + z * z) <= respawnDistance * Mathf.Sqrt(2))
        //            positions.Add(new Vector3((int)player.transform.position.x + x, (int)player.transform.position.y + y, (int)player.transform.position.z + z));
        //    }
        //}

        //foreach (Vector3 position in positions)
        //{

        //    //print(position);

        //    int x = (int)(position.x + 0.5);
        //    int y = (int)(position.y + 0.5);
        //    int z = (int)(position.z + 0.5);

        //    if (PositionHasBlock(x, y, z) && !Physics.CheckBox(position, new Vector3(halfOfBoxWidth_plusMargin, halfOfBoxWidth_plusMargin, halfOfBoxWidth_plusMargin)))
        //    {
        //        GameObject obj;

        //        if (type == 0)
        //        {
        //            obj = Instantiate(dirt, new Vector3(x, y, z), Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
        //            HidePlanes(obj);
        //        }
        //        else if (type == 1)
        //        {
        //            obj = Instantiate(dirtTop, new Vector3(x, y, z), Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
        //            HidePlanes(obj);
        //        }
        //        else if (type == 2)
        //        {
        //            obj = Instantiate(sand, new Vector3(x, y, z), Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
        //            HidePlanes(obj);
        //        }
        //        else if (type == 3)
        //        {
        //            obj = Instantiate(oakLog, new Vector3(x, y, z), Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
        //            HidePlanes(obj);
        //        }
        //        else if (type == 4)
        //        {
        //            obj = Instantiate(oakLeaves, new Vector3(x, y, z), Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
        //            HidePlanes(obj);
        //        }
        //        else if (type == 5)
        //        {
        //            obj = Instantiate(cactus, new Vector3(x, y, z), Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
        //            HidePlanes(obj);
        //        }

        //    }
        //}

    }


    bool PositionHasBlock(int x, int y, int z)
    {

        bool positionHasBlock = false;

        int chunk_xRank = 0;
        int chunk_zRank = 0;

        if (x >= 0)
            chunk_xRank = (int)(x / width + 1);
        else
            chunk_xRank = (int)(-x / width + 1);

        if (z >= 0)
            chunk_zRank = (int)(z / width + 1);
        else
            chunk_zRank = (int)(-z / width + 1);

        if (x >= 0)
        {
            if (z >= 0)
            {
                if (y >= 0)
                {
                    try
                    {
                        //if (xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][0] || xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][1]
                        //    || xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][2] || xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][3]
                        //    || xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][4] || xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][6])
                        //{
                        //    positionHasBlock = true;

                        //    for (int i = 0; i < publicInfo.blockNames.Length; i++)
                        //    {
                        //        if (xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][i])
                        //        {
                        //            type = i;
                        //            break;
                        //        }
                        //    }
                        //}

                        for(int i = 0; i < publicInfo.blockNames.Length; i++)
                        {
                            if (xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][i])
                            {
                                positionHasBlock = true;
                            }
                        }

                        if(positionHasBlock)
                        {
                            for (int i = 0; i < publicInfo.blockNames.Length; i++)
                            {
                                if (xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][i])
                                {
                                    type = i;
                                    break;
                                }
                            }
                        }

                    }
                    catch (System.ArgumentOutOfRangeException e) { }
                }
                else
                {
                    y = -y;
                    try
                    {
                        for (int i = 0; i < publicInfo.blockNames.Length; i++)
                        {
                            if (xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][0][x][y][z][i])
                            {
                                positionHasBlock = true;
                            }
                        }

                        if (positionHasBlock)
                        {
                            for (int i = 0; i < publicInfo.blockNames.Length; i++)
                            {
                                if (xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][0][x][y][z][i])
                                {
                                    type = i;
                                    break;
                                }
                            }
                        }
                    }
                    catch (System.ArgumentOutOfRangeException e) { }
                }
            }
            else
            {
                z = -z;
                if (y >= 0)
                {
                    try
                    {
                        for (int i = 0; i < publicInfo.blockNames.Length; i++)
                        {
                            if (xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][i])
                            {
                                positionHasBlock = true;
                            }
                        }

                        if (positionHasBlock)
                        {
                            for (int i = 0; i < publicInfo.blockNames.Length; i++)
                            {
                                if (xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][i])
                                {
                                    type = i;
                                    break;
                                }
                            }
                        }
                    }
                    catch (System.ArgumentOutOfRangeException e) { }
                }
                else
                {
                    y = -y;
                    try
                    {
                        for (int i = 0; i < publicInfo.blockNames.Length; i++)
                        {
                            if (xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][0][x][y][z][i])
                            {
                                positionHasBlock = true;
                            }
                        }

                        if (positionHasBlock)
                        {
                            for (int i = 0; i < publicInfo.blockNames.Length; i++)
                            {
                                if (xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][0][x][y][z][i])
                                {
                                    type = i;
                                    break;
                                }
                            }
                        }
                    }
                    catch (System.ArgumentOutOfRangeException e) { }
                }
            }
        }
        else
        {
            x = -x;
            if (z >= 0)
            {
                if (y >= 0)
                {
                    try
                    {
                        for (int i = 0; i < publicInfo.blockNames.Length; i++)
                        {
                            if (xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][i])
                            {
                                positionHasBlock = true;
                            }
                        }

                        if (positionHasBlock)
                        {
                            for (int i = 0; i < publicInfo.blockNames.Length; i++)
                            {
                                if (xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][i])
                                {
                                    type = i;
                                    break;
                                }
                            }
                        }
                    }
                    catch (System.ArgumentOutOfRangeException e) { }
                }
                else
                {
                    y = -y;
                    try
                    {
                        for (int i = 0; i < publicInfo.blockNames.Length; i++)
                        {
                            if (xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][0][x][y][z][i])
                            {
                                positionHasBlock = true;
                            }
                        }

                        if (positionHasBlock)
                        {
                            for (int i = 0; i < publicInfo.blockNames.Length; i++)
                            {
                                if (xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][0][x][y][z][i])
                                {
                                    type = i;
                                    break;
                                }
                            }
                        }
                    }
                    catch (System.ArgumentOutOfRangeException e) { }
                }
            }
            else
            {
                z = -z;
                if (y >= 0)
                {
                    try
                    {
                        for (int i = 0; i < publicInfo.blockNames.Length; i++)
                        {
                            if (xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][i])
                            {
                                positionHasBlock = true;
                            }
                        }

                        if (positionHasBlock)
                        {
                            for (int i = 0; i < publicInfo.blockNames.Length; i++)
                            {
                                if (xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][i])
                                {
                                    type = i;
                                    break;
                                }
                            }
                        }
                    }
                    catch (System.ArgumentOutOfRangeException e) { }
                }
                else
                {
                    y = -y;
                    try
                    {
                        for (int i = 0; i < publicInfo.blockNames.Length; i++)
                        {
                            if (xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][0][x][y][z][i])
                            {
                                positionHasBlock = true;
                            }
                        }

                        if (positionHasBlock)
                        {
                            for (int i = 0; i < publicInfo.blockNames.Length; i++)
                            {
                                if (xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][0][x][y][z][i])
                                {
                                    type = i;
                                    break;
                                }
                            }
                        }
                    }
                    catch (System.ArgumentOutOfRangeException e) { }
                }
            }
        }

        return positionHasBlock;

    }
    public void CreateChunk(Vector3 startPos)
    {
        float xPos = startPos.x;
        float zPos = startPos.z;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                float perlinNoise = 0f;
                float normalization = 0f;

                float sampleX = (x + offsetX + xPos) / scaler;
                float sampleZ = (z + offsetX + zPos) / scaler;

                foreach (Wave wave in waves)
                {
                    perlinNoise += wave.amplitude * Mathf.PerlinNoise(sampleX * wave.frequency + wave.seed, sampleZ * wave.frequency + wave.seed);
                    normalization += wave.amplitude;
                }
                perlinNoise /= normalization;

                float biomeSampleX = (x + biomeOffsetX + xPos) / biomeScaler;
                float biomeSampleZ = (z + biomeOffsetZ + zPos) / biomeScaler;
                float biomePerlinNoise = Mathf.PerlinNoise(biomeSampleX, biomeSampleZ);

                perlinNoise = Mathf.PerlinNoise(sampleX, sampleZ) * multiplier;
                int y = (int)(perlinNoise + startPos.y);

                GameObject obj; 

                Vector3 pos = new Vector3(x + startPos.x, y, z + startPos.z);
                if (biomePerlinNoise > desertPossibility)
                {
                    obj = Instantiate(dirtTop, pos, Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
                    AddBlockToLists(pos, System.Array.IndexOf(publicInfo.blockNames, "dirtTop"));
                    HidePlanes(obj);
                }
                else
                {
                    obj = Instantiate(sand, pos, Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
                    AddBlockToLists(pos, System.Array.IndexOf(publicInfo.blockNames, "sand"));
                    HidePlanes(obj);
                }

                int difference = (int)(y - startPos.y); // positiv om den är högre upp än startpos
                if (difference < 0)
                    difference = 0; 

                for (int n = 1; n <= (height + difference); n++)
                {
                    pos = new Vector3(x + startPos.x, y - n, z + startPos.z);

                    if (biomePerlinNoise > desertPossibility)
                    {
                        obj = Instantiate(dirt, pos, Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
                        AddBlockToLists(pos, System.Array.IndexOf(publicInfo.blockNames, "dirt"));
                        HidePlanes(obj);
                    }
                    else
                    {
                        obj = Instantiate(sand, pos, Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
                        AddBlockToLists(pos, System.Array.IndexOf(publicInfo.blockNames, "sand"));
                        HidePlanes(obj);
                    }

                }
            }
        }

        SpawnTrees(startPos);
    }

    void CreateChunkUpdate(Vector3 startPos)
    {
        float xPos = startPos.x;
        float zPos = startPos.z;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                float perlinNoise = 0f;
                float normalization = 0f;

                float sampleX = (x + offsetX + xPos) / scaler;
                float sampleZ = (z + offsetX + zPos) / scaler;

                foreach (Wave wave in waves)
                {
                    perlinNoise += wave.amplitude * Mathf.PerlinNoise(sampleX * wave.frequency + wave.seed, sampleZ * wave.frequency + wave.seed);
                    normalization += wave.amplitude;
                }
                perlinNoise /= normalization;

                float biomeSampleX = (x + biomeOffsetX + xPos) / biomeScaler;
                float biomeSampleZ = (z + biomeOffsetZ + zPos) / biomeScaler;
                float biomePerlinNoise = Mathf.PerlinNoise(biomeSampleX, biomeSampleZ);

                perlinNoise = Mathf.PerlinNoise(sampleX, sampleZ) * multiplier;
                int y = (int)(perlinNoise + startPos.y);

                Vector3 pos = new Vector3(x + startPos.x, y, z + startPos.z);
                if (biomePerlinNoise > desertPossibility)
                {
                    //Instantiate(dirtTop, pos, Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
                    AddBlockToLists(pos, 1);
                }
                else
                {
                    //Instantiate(sand, pos, Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
                    AddBlockToLists(pos, 2);
                }

                int difference = (int)(y - startPos.y); // positiv om den är högre upp än startpos
                for (int n = 1; n <= (height + difference); n++)
                {
                    pos = new Vector3(x + startPos.x, y - n, z + startPos.z);
                    if (biomePerlinNoise > desertPossibility)
                    {
                        //Instantiate(dirt, pos, Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
                        AddBlockToLists(pos, 0);
                    }
                    else
                    {
                        //Instantiate(sand, pos, Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
                        AddBlockToLists(pos, 2);
                    }

                }
            }
        }

        SpawnTrees(startPos);
    }

    void AddBlockToLists(Vector3 pos, int type)
    {

        int x = (int)(pos.x + 0.5);
        int y = (int)(pos.y + 0.5);
        int z = (int)(pos.z + 0.5);

        int chunk_xRank = 0;
        int chunk_zRank = 0;

        if (x >= 0)
            chunk_xRank = (int)(x / width + 1); // första rank = 1 (ex den chunk för (0,0,0))
        else
            chunk_xRank = (int)(-x / width + 1);

        if (z >= 0)
            chunk_zRank = (int)(z / width + 1);
        else
            chunk_zRank = (int)(-z / width + 1);

        //if (pos.x < 0)
        //    x = (int)(pos.x - 0.5);
        //if (pos.y < 0)
        //    y = (int)(pos.y - 0.5);
        //if (pos.z < 0)
        //    z = (int)(pos.z - 0.5);

        if (x >= 0)
        {
            if(z >= 0) // både x och z positiva = xpos_zpos listan 
            {
                if(xpos_zpos.Count < chunk_xRank)
                {
                    int k = xpos_zpos.Count; 
                    for (int i = 0; i < (chunk_xRank - k); i++)
                    {
                        List<List<List<List<List<List<bool>>>>>> newChunkRow_x = new  List<List<List<List<List<List<bool>>>>>>();
                        xpos_zpos.Add(newChunkRow_x);
                    }
                }

                if (xpos_zpos[chunk_xRank - 1].Count < chunk_zRank)
                {
                    int k = xpos_zpos[chunk_xRank - 1].Count; 
                    for (int i = 0; i < (chunk_zRank - k); i++)
                    {
                        List<List<List<List<List<bool>>>>> newChunk_z = new List<List<List<List<List<bool>>>>>();
                        xpos_zpos[chunk_xRank - 1].Add(newChunk_z);

                        List<List<List<List<bool>>>> yneg = new List<List<List<List<bool>>>>();
                        List<List<List<List<bool>>>> ypos = new List<List<List<List<bool>>>>();

                        xpos_zpos[chunk_xRank - 1][k + i].Add(yneg);
                        xpos_zpos[chunk_xRank - 1][k + i].Add(ypos);
                    }
                }

                int n = 0;
                if (y >= 0)
                    n = 1;
                else
                    n = 0; 

                if (xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][n].Count < (x + 1))
                {
                    int k = xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][1].Count; 
                    for (int i = 0; i < ((x + 1) - k); i++)
                    {
                        List<List<List<bool>>> newXpos = new List<List<List<bool>>>();
                        xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][n].Add(newXpos);
                    }
                }

                if (xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x].Count < (y + 1))
                {
                    int k = xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x].Count; 
                    for (int i = 0; i < ((y + 1) - k); i++)
                    {
                        List<List<bool>> newYpos = new List<List<bool>>();
                        xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x].Add(newYpos);
                    }
                }

                if (xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x][y].Count < (z + 1))
                {
                    int k = xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x][y].Count; 
                    for (int i = 0; i < ((z + 1) - k); i++)
                    {
                        List<bool> newZpos = new List<bool>();
                        xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x][y].Add(newZpos);

                        for (int j = 0; j < publicInfo.blockNames.Length; j++)
                        {
                            xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x][y][k + i].Add(false);
                        }
                    }
                }

                xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x][y][z][type] = true; 

            }
            else
            {
                z = -z;

                if (xpos_zneg.Count < chunk_xRank)
                {
                    int k = xpos_zneg.Count;
                    for (int i = 0; i < (chunk_xRank - k); i++)
                    {
                        List<List<List<List<List<List<bool>>>>>> newChunkRow_x = new List<List<List<List<List<List<bool>>>>>>();
                        xpos_zneg.Add(newChunkRow_x);
                    }
                }

                if (xpos_zneg[chunk_xRank - 1].Count < chunk_zRank)
                {
                    int k = xpos_zneg[chunk_xRank - 1].Count;
                    for (int i = 0; i < (chunk_zRank - k); i++)
                    {
                        List<List<List<List<List<bool>>>>> newChunk_z = new List<List<List<List<List<bool>>>>>();
                        xpos_zneg[chunk_xRank - 1].Add(newChunk_z);

                        List<List<List<List<bool>>>> yneg = new List<List<List<List<bool>>>>();
                        List<List<List<List<bool>>>> ypos = new List<List<List<List<bool>>>>();

                        xpos_zneg[chunk_xRank - 1][k + i].Add(yneg);
                        xpos_zneg[chunk_xRank - 1][k + i].Add(ypos);
                    }
                }

                int n = 0;
                if (y >= 0)
                    n = 1;
                else
                    n = 0;

                if (xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][n].Count < (x + 1))
                {
                    int k = xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][1].Count;
                    for (int i = 0; i < ((x + 1) - k); i++)
                    {
                        List<List<List<bool>>> newXpos = new List<List<List<bool>>>();
                        xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][n].Add(newXpos);
                    }
                }

                if (xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x].Count < (y + 1))
                {
                    int k = xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x].Count;
                    for (int i = 0; i < ((y + 1) - k); i++)
                    {
                        List<List<bool>> newYpos = new List<List<bool>>();
                        xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x].Add(newYpos);
                    }
                }

                if (xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x][y].Count < (z + 1))
                {
                    int k = xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x][y].Count;
                    for (int i = 0; i < ((z + 1) - k); i++)
                    {
                        List<bool> newZpos = new List<bool>();
                        xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x][y].Add(newZpos);

                        for(int j = 0; j < publicInfo.blockNames.Length; j++)
                        {
                            xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x][y][k + i].Add(false);
                        }

                    }
                }

                xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x][y][z][type] = true;
            }
        }
        else
        {
            x = -x;

            if (z >= 0)
            {
                if (xneg_zpos.Count < chunk_xRank)
                {
                    int k = xneg_zpos.Count;
                    for (int i = 0; i < (chunk_xRank - k); i++)
                    {
                        List<List<List<List<List<List<bool>>>>>> newChunkRow_x = new List<List<List<List<List<List<bool>>>>>>();
                        xneg_zpos.Add(newChunkRow_x);
                    }
                }

                if (xneg_zpos[chunk_xRank - 1].Count < chunk_zRank)
                {
                    int k = xneg_zpos[chunk_xRank - 1].Count;
                    for (int i = 0; i < (chunk_zRank - k); i++)
                    {
                        List<List<List<List<List<bool>>>>> newChunk_z = new List<List<List<List<List<bool>>>>>();
                        xneg_zpos[chunk_xRank - 1].Add(newChunk_z);

                        List<List<List<List<bool>>>> yneg = new List<List<List<List<bool>>>>();
                        List<List<List<List<bool>>>> ypos = new List<List<List<List<bool>>>>();

                        xneg_zpos[chunk_xRank - 1][k + i].Add(yneg);
                        xneg_zpos[chunk_xRank - 1][k + i].Add(ypos);
                    }
                }

                int n = 0;
                if (y >= 0)
                    n = 1;
                else
                    n = 0;

                if (xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][n].Count < (x + 1))
                {
                    int k = xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][1].Count;
                    for (int i = 0; i < ((x + 1) - k); i++)
                    {
                        List<List<List<bool>>> newXpos = new List<List<List<bool>>>();
                        xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][n].Add(newXpos);
                    }
                }

                if (xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x].Count < (y + 1))
                {
                    int k = xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x].Count;
                    for (int i = 0; i < ((y + 1) - k); i++)
                    {
                        List<List<bool>> newYpos = new List<List<bool>>();
                        xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x].Add(newYpos);
                    }
                }

                if (xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x][y].Count < (z + 1))
                {
                    int k = xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x][y].Count;
                    for (int i = 0; i < ((z + 1) - k); i++)
                    {
                        List<bool> newZpos = new List<bool>();
                        xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x][y].Add(newZpos);

                        for(int j = 0; j < publicInfo.blockNames.Length; j++)
                        {
                            xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x][y][k + i].Add(false);
                        }

                    }
                }


                xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x][y][z][type] = true;
            }
            else
            {
                z = -z;

                if (xneg_zneg.Count < chunk_xRank)
                {
                    int k = xneg_zneg.Count;
                    for (int i = 0; i < (chunk_xRank - k); i++)
                    {
                        List<List<List<List<List<List<bool>>>>>> newChunkRow_x = new List<List<List<List<List<List<bool>>>>>>();
                        xneg_zneg.Add(newChunkRow_x);
                    }
                }

                if (xneg_zneg[chunk_xRank - 1].Count < chunk_zRank)
                {
                    int k = xneg_zneg[chunk_xRank - 1].Count;
                    for (int i = 0; i < (chunk_zRank - k); i++)
                    {
                        List<List<List<List<List<bool>>>>> newChunk_z = new List<List<List<List<List<bool>>>>>();
                        xneg_zneg[chunk_xRank - 1].Add(newChunk_z);

                        List<List<List<List<bool>>>> yneg = new List<List<List<List<bool>>>>();
                        List<List<List<List<bool>>>> ypos = new List<List<List<List<bool>>>>();

                        xneg_zneg[chunk_xRank - 1][k + i].Add(yneg);
                        xneg_zneg[chunk_xRank - 1][k + i].Add(ypos);
                    }
                }

                int n = 0;
                if (y >= 0)
                    n = 1;
                else
                    n = 0;

                if (xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][n].Count < (x + 1))
                {
                    int k = xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][1].Count;
                    for (int i = 0; i < ((x + 1) - k); i++)
                    {
                        List<List<List<bool>>> newXpos = new List<List<List<bool>>>();
                        xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][n].Add(newXpos);
                    }
                }

                if (xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x].Count < (y + 1))
                {
                    int k = xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x].Count;
                    for (int i = 0; i < ((y + 1) - k); i++)
                    {
                        List<List<bool>> newYpos = new List<List<bool>>();
                        xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x].Add(newYpos);
                    }
                }

                if (xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x][y].Count < (z + 1))
                {
                    int k = xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x][y].Count;
                    for (int i = 0; i < ((z + 1) - k); i++)
                    {
                        List<bool> newZpos = new List<bool>();
                        xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x][y].Add(newZpos);

                        for(int j = 0; j < publicInfo.blockNames.Length; j++)
                        {
                            xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x][y][k + i].Add(false);
                        }

                    }
                }

                xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x][y][z][type] = true;
            }
        }

    }
    public void RemoveBlockFromList(Vector3 pos, int type)
    {

        int x = 0;
        if (pos.x < 0)
            x = (int)(pos.x - 0.5);
        else
            x = (int)(pos.x + 0.5);

        int y = (int)(pos.y + 0.5);
        if (pos.y < 0)
            y = (int)(pos.y - 0.5);
        else
            y = (int)(pos.y + 0.5);

        int z = 0;
        if (pos.z < 0)
            z = (int)(pos.z - 0.5);
        else
            z = (int)(pos.z + 0.5);

        int n = 0;
        if (y >= 0)
            n = 1;
        else
            n = 0;

        int chunk_xRank = 0;
        int chunk_zRank = 0;

        if (x >= 0)
            chunk_xRank = (int)(x / width + 1); // första rank = 1 (ex den chunk för (0,0,0))
        else
            chunk_xRank = (int)(-x / width + 1);

        if (z >= 0)
            chunk_zRank = (int)(z / width + 1);
        else
            chunk_zRank = (int)(-z / width + 1);

        if (x >= 0)
        {
            if (z >= 0) // både x och z positiva = xpos_zpos listan 
            {
                try
                {

                    xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x][y][z][type] = false;

                } catch(System.ArgumentOutOfRangeException e) { }

            }
            else
            {
                z = -z;

                try
                {

                    xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x][y][z][type] = false;

                }
                catch (System.ArgumentOutOfRangeException e) { }
            }
        }
        else
        {
            x = -x;

            if (z >= 0)
            {

                try
                {

                    xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][n][x][y][z][type] = false;

                }
                catch (System.ArgumentOutOfRangeException e) { }

            }
            else
            {
                z = -z;

                try
                {

                    xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][n][x][y][z][type] = false;

                }
                catch (System.ArgumentOutOfRangeException e) { }

            }
        }

    }

    void SpawnTrees(Vector3 startPos)
    {



        Vector3 pos0 = new Vector3(startPos.x - width / 2, startPos.y, startPos.z + width / 2);
        Vector3 pos1 = new Vector3(startPos.x + width / 2, startPos.y, startPos.z + width / 2);
        Vector3 pos2 = new Vector3(startPos.x - width / 2, startPos.y, startPos.z - width / 2);
        Vector3 pos3 = new Vector3(startPos.x + width / 2, startPos.y, startPos.z - width / 2);

        /*
                pos0 -------------- pos1
                |                   |
                |                   |
                |                   |
                |      startPos     |
                |                   |
                |                   |
                |                   |
                pos2 -------------- pos3
            */

        int amountOfTrees = Random.Range(minTrees, maxTrees);

        if (amountOfTrees > 0)
        {
            Vector3[] takenTreePositions = new Vector3[amountOfTrees];
            int n = 0;

            while (amountOfTrees > 0)
            {
                int xPos = Random.Range((int)pos0.x, (int)pos1.x);
                int zPos = Random.Range((int)pos3.z, (int)pos1.z);
                int yPos = (int)startPos.y; 

                bool positionFound = false;
                bool endLoop = false; 
                int x = xPos;
                int z = zPos;
                int y = 1; 

                int chunk_xRank = 0;
                int chunk_zRank = 0;

                if (x >= 0)
                    chunk_xRank = (int)(x / width + 1); 
                else
                    chunk_xRank = (int)(-x / width + 1);

                if (z >= 0)
                    chunk_zRank = (int)(z / width + 1);
                else
                    chunk_zRank = (int)(-z / width + 1);

                int endWhileLoopCount = 30; 

                if(x >= 0)
                {
                    if(z >= 0)
                    {
                        while (!positionFound && !endLoop)
                        {
                            try
                            {
                                if (!xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][0] && !xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][1]
                                    && !xpos_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][2])
                                {
                                    yPos = y;
                                    positionFound = true;
                                }
                                else
                                    y++;
                            }
                            catch (System.ArgumentOutOfRangeException) { endLoop = true; }

                            if (y > endWhileLoopCount)
                                endLoop = true; 
                        }
                    }
                    else
                    {
                        z = -z;
                        while (!positionFound && !endLoop)
                        {
                            try
                            {
                                if (!xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][0] && !xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][1]
                                    && !xpos_zneg[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][2])
                                {
                                    yPos = y;
                                    positionFound = true;
                                }
                                else
                                    y++; 
                            }
                            catch (System.ArgumentOutOfRangeException) { endLoop = true; }

                            if (y > endWhileLoopCount)
                                endLoop = true;
                        }
                    }
                }
                else
                {
                    x = -x; 
                    if(z >= 0)
                    {
                        while (!positionFound && !endLoop)
                        {
                            try
                            {
                                if (!xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][0] && !xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][1]
                                    && !xneg_zpos[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][2])
                                {
                                    yPos = y;
                                    positionFound = true;
                                }
                                else
                                    y++; 
                            }
                            catch (System.ArgumentOutOfRangeException) { endLoop = true;  }

                            if (y > endWhileLoopCount)
                                endLoop = true;
                        }
                    }
                    else
                    {
                        z = -z;
                        while (!positionFound && !endLoop)
                        {
                            try
                            {
                                if (!xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][0] && !xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][1]
                                    && !xneg_zneg[chunk_xRank - 1][chunk_zRank - 1][1][x][y][z][2])
                                {
                                    yPos = y;
                                    positionFound = true;
                                }
                                else
                                    y++; 
                            }
                            catch (System.ArgumentOutOfRangeException) { endLoop = true; }

                            if (y > endWhileLoopCount)
                                endLoop = true;
                        }
                    }
                }

                if(!endLoop)
                {
                    int minDistance = 2;
                    bool changeValue = false;
                    for (int i = 0; i < takenTreePositions.Length; i++)
                    {
                        if ((Mathf.Abs(takenTreePositions[i].x - xPos) <= minDistance) && (Mathf.Abs(takenTreePositions[i].z - zPos) <= minDistance))
                        {
                            changeValue = true;
                            break;
                        }
                    }
                    while (changeValue)
                    {
                        changeValue = false;
                        xPos = Random.Range((int)pos0.x, (int)pos1.x);
                        zPos = Random.Range((int)pos3.z, (int)pos1.z);

                        for (int i = 0; i < takenTreePositions.Length; i++)
                        {
                            if ((Mathf.Abs(takenTreePositions[i].x - xPos) <= minDistance) && (Mathf.Abs(takenTreePositions[i].z - zPos) <= minDistance))
                            {
                                changeValue = true;
                                break;
                            }
                        }
                    }

                    float biomeSampleX = (biomeOffsetX + xPos) / biomeScaler;
                    float biomeSampleZ = (biomeOffsetZ + zPos) / biomeScaler;
                    float biomePerlinNoise = Mathf.PerlinNoise(biomeSampleX, biomeSampleZ);

                    takenTreePositions[n] = new Vector3(xPos, yPos, zPos);
                    n++;

                    if (biomePerlinNoise > desertPossibility)
                    {
                        Instantiate(oakLog, new Vector3(xPos, yPos, zPos), Quaternion.Euler(-90, 0, 0), blockSpawnParent);
                        //AddBlockToLists(new Vector3(xPos, yPos, zPos), 3);

                        int treeHeight = Random.Range(3, 5);
                        for (int i = 1; i < treeHeight; i++)
                        {
                            Instantiate(oakLog, new Vector3(xPos, yPos + i, zPos), Quaternion.Euler(-90, 0, 0), blockSpawnParent);
                            //AddBlockToLists(new Vector3(xPos, yPos + i, zPos), 3);
                        }

                        Instantiate(oakLeaves, new Vector3(xPos, yPos + treeHeight, zPos), Quaternion.Euler(-90, 0, 0), blockSpawnParent);
                        //AddBlockToLists(new Vector3(xPos, yPos + treeHeight, zPos), 4);

                        Instantiate(oakLeaves, new Vector3(xPos + 1, yPos + treeHeight - 1, zPos), Quaternion.Euler(-90, 0, 0), blockSpawnParent);
                        Instantiate(oakLeaves, new Vector3(xPos - 1, yPos + treeHeight - 1, zPos), Quaternion.Euler(-90, 0, 0), blockSpawnParent);
                        Instantiate(oakLeaves, new Vector3(xPos, yPos + treeHeight - 1, zPos + 1), Quaternion.Euler(-90, 0, 0), blockSpawnParent);
                        Instantiate(oakLeaves, new Vector3(xPos, yPos + treeHeight - 1, zPos - 1), Quaternion.Euler(-90, 0, 0), blockSpawnParent);

                        //AddBlockToLists(new Vector3(xPos + 1, yPos + treeHeight - 1, zPos), 4);
                        //AddBlockToLists(new Vector3(xPos - 1, yPos + treeHeight - 1, zPos), 4);
                        //AddBlockToLists(new Vector3(xPos, yPos + treeHeight - 1, zPos + 1), 4);
                        //AddBlockToLists(new Vector3(xPos, yPos + treeHeight - 1, zPos - 1), 4);

                        Instantiate(oakLeaves, new Vector3(xPos + 1, yPos + treeHeight, zPos + 1), Quaternion.Euler(-90, 0, 0), blockSpawnParent);
                        Instantiate(oakLeaves, new Vector3(xPos + 1, yPos + treeHeight, zPos - 1), Quaternion.Euler(-90, 0, 0), blockSpawnParent);
                        Instantiate(oakLeaves, new Vector3(xPos - 1, yPos + treeHeight, zPos - 1), Quaternion.Euler(-90, 0, 0), blockSpawnParent);
                        Instantiate(oakLeaves, new Vector3(xPos - 1, yPos + treeHeight, zPos + 1), Quaternion.Euler(-90, 0, 0), blockSpawnParent);

                        //AddBlockToLists(new Vector3(xPos + 1, yPos + treeHeight, zPos + 1), 4);
                        //AddBlockToLists(new Vector3(xPos + 1, yPos + treeHeight, zPos - 1), 4);
                        //AddBlockToLists(new Vector3(xPos - 1, yPos + treeHeight, zPos - 1), 4);
                        //AddBlockToLists(new Vector3(xPos - 1, yPos + treeHeight, zPos + 1), 4);

                        Instantiate(oakLeaves, new Vector3(xPos + 1, yPos + treeHeight, zPos), Quaternion.Euler(-90, 0, 0), blockSpawnParent);
                        Instantiate(oakLeaves, new Vector3(xPos - 1, yPos + treeHeight, zPos), Quaternion.Euler(-90, 0, 0), blockSpawnParent);
                        Instantiate(oakLeaves, new Vector3(xPos, yPos + treeHeight, zPos + 1), Quaternion.Euler(-90, 0, 0), blockSpawnParent);
                        Instantiate(oakLeaves, new Vector3(xPos, yPos + treeHeight, zPos - 1), Quaternion.Euler(-90, 0, 0), blockSpawnParent);

                        //AddBlockToLists(new Vector3(xPos + 1, yPos + treeHeight, zPos), 4);
                        //AddBlockToLists(new Vector3(xPos - 1, yPos + treeHeight, zPos), 4);
                        //AddBlockToLists(new Vector3(xPos, yPos + treeHeight, zPos + 1), 4);
                        //AddBlockToLists(new Vector3(xPos, yPos + treeHeight, zPos - 1), 4);

                        Instantiate(oakLeaves, new Vector3(xPos, yPos + treeHeight + 1, zPos), Quaternion.Euler(-90, 0, 0), blockSpawnParent);
                        //AddBlockToLists(new Vector3(xPos, yPos + treeHeight + 1, zPos), 4);
                    }
                    else
                    {
                        int cactusHeight = Random.Range(2, 3);

                        for(int i = 0; i < cactusHeight; i++)
                        {
                            Instantiate(cactus, new Vector3(xPos, yPos + i, zPos), Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
                            //AddBlockToLists(new Vector3(xPos, yPos + i, zPos), 5);
                        }
                    }

                }

                amountOfTrees--;

            }
        }

    }

    void HidePlanes(GameObject obj) 
    {

        /*
         * 
         * syfte: dölj sidor av block som inte syns 
         * 
         * problem just nu: 
         *      1. hur ska jag uppdatera denna metod hela tiden
         *      2. olika if satser för olika sidor på block 
         *      
         */


        Collider ownCollider = obj.GetComponent<BoxCollider>();

        float halfBounds = 0.6f; //0.5f = halva boxen, 0.1f = marginal för att ignorera sin egen collider 
        Collider[] collidersTouchingObject = Physics.OverlapBox(ownCollider.bounds.center, new Vector3(halfBounds, halfBounds, halfBounds), Quaternion.identity, 1 << 8);

        foreach (Collider blockCollider in collidersTouchingObject)
        {
            if (blockCollider != ownCollider)
            {

                Vector3 ownColliderPosition = ownCollider.bounds.center;
                Vector3 otherColliderPosition = blockCollider.bounds.center;
                float margin = 0.5f;

                if (Mathf.Abs(ownColliderPosition.x - otherColliderPosition.x) < margin && Mathf.Abs(ownColliderPosition.z - otherColliderPosition.z) < margin &&
                    (otherColliderPosition.y - ownColliderPosition.y) > margin) // om det finns block över detta block
                {
                    //obj.GetComponent<MeshRenderer>().enabled = false;
                    Destroy(obj);
                }

            }
        }
        

        /*********************************************/

        //Collider ownCollider = obj.GetComponent<BoxCollider>();
        //float halfBounds = 0.6f; //0.5f = halva boxen, 0.1f = marginal för att ignorera sin egen collider 

        //Collider[] collidersTouchingObject = Physics.OverlapBox(ownCollider.bounds.center, new Vector3(halfBounds, halfBounds, halfBounds), Quaternion.identity, 1 << 8);

        //foreach (Collider blockCollider in collidersTouchingObject)
        //{
        //    if (blockCollider != ownCollider)
        //    {

        //        Vector3 ownColliderPosition = ownCollider.bounds.center;
        //        Vector3 otherColliderPosition = blockCollider.bounds.center;
        //        float margin = 0.5f;

        //        if (Mathf.Abs(ownColliderPosition.x - otherColliderPosition.x) < margin && Mathf.Abs(ownColliderPosition.z - otherColliderPosition.z) < margin &&
        //            (otherColliderPosition.y - ownColliderPosition.y) > margin) // om det finns block över detta block
        //        {
        //            //obj.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

        //            //for (int i = 0; i < 6; i++)
        //            //{
        //            //    obj.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
        //            //}

        //            Destroy(obj);
        //        }

        //    }
        //}


    }

    public void ShowBlock(Vector3 position)
    {

        //int x = 0;
        //if (position.x < 0)
        //    x = (int)(position.x - 0.5);
        //else
        //    x = (int)(position.x + 0.5);

        //int y = (int)(position.y + 0.5);
        //if (position.y < 0)
        //    y = (int)(position.y - 0.5);
        //else
        //    y = (int)(position.y + 0.5);

        //int z = 0;
        //if (position.z < 0)
        //    z = (int)(position.z - 0.5);
        //else
        //    z = (int)(position.z + 0.5);

        int x = (int)(position.x + 0.5);
        int y = (int)(position.y + 0.5);
        int z = (int)(position.z + 0.5);

        //print("position: " + new Vector3(x, y, z));

        if (PositionHasBlock(x, y, z) && !Physics.CheckBox(position, new Vector3(halfOfBoxWidth_plusMargin, halfOfBoxWidth_plusMargin, halfOfBoxWidth_plusMargin)))
        {

            GameObject obj;

            for(int i = 0; i < publicInfo.blockNames.Length; i++)
            {
                if(type == i) // && !publicInfo.blockNames[i].Equals("none")
                {
                    obj = Instantiate(Resources.Load<GameObject>(itemsURL + publicInfo.blockNames[i] + "Prefab"), new Vector3(x, y, z), Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent); // blockSpawnParent
                    break; 
                }
            }

            //if (type == 0) //type ändras efter att man kör funktionen PositionHasBlock(int, int, int)
            //{
            //    obj = Instantiate(dirt, new Vector3(x, y, z), Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
            //}
            //else if (type == 1)
            //{
            //    obj = Instantiate(dirtTop, new Vector3(x, y, z), Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
            //}
            //else if (type == 2)
            //{
            //    obj = Instantiate(sand, new Vector3(x, y, z), Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
            //}
            //else if (type == 3)
            //{
            //    obj = Instantiate(oakLog, new Vector3(x, y, z), Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
            //}
            //else if (type == 4)
            //{
            //    obj = Instantiate(oakLeaves, new Vector3(x, y, z), Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
            //}
            //else if (type == 5)
            //{
            //    obj = Instantiate(cactus, new Vector3(x, y, z), Quaternion.Euler(x_RotationBlock, 0, 0), blockSpawnParent);
            //}

        }

    }

} 


/*
    
    if (transform.position.x > (maxX_Discovered*width - discoverMargin))
    {

        Debug.Log("0");
        maxX_Discovered++;

        if (transform.position.z > (maxZ_Discovered*width - discoverMargin))
        {
            Debug.Log("1");
            maxZ_Discovered++;
            CreateChunk(new Vector3(maxX_Discovered * width, 0, maxZ_Discovered * width));
        }
        else if (transform.position.z < (minZ_Discovered*width + discoverMargin))
        {
            Debug.Log("2");
            minZ_Discovered++;
            CreateChunk(new Vector3(maxX_Discovered * width, 0, minZ_Discovered * width));
        }
        else
        {
            Debug.Log("3");
            int z = (int)(transform.position.z + 0.5);
            int zDistanceToNextChunk = z % width; // ex om den är på 40 = ska till 48. 40 % 16 = 8
            CreateChunk(new Vector3(maxX_Discovered * width, 0, z + zDistanceToNextChunk));
        }
    }
 
 */
