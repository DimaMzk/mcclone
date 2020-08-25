using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;

public class arraytoterraintest : MonoBehaviour
{

    public GameObject player;

    public Text statusText;
    
    public static Block DIRT;
    public static Block GRASS;
    public static Block SNOW_GRASS;
    public static Block WOOD;
    public static Block LEAF;
    public static Block SNOW_LEAF;
    public static Block AIR;
    public static Block WATER_FULL;
    public static Block SAND;
    public static Block GRASS18;
    public static Block GRASS28;
    public static Block GRASS38;
    public static Block GRASS48;
    public static Block GRASS58;
    public static Block GRASS68;
    public static Block GRASS78;
    //public static Block WATER_TOP;

    public String currentCenterChunk;

    private static System.Random SEED_GENERATOR = new System.Random();
    public static int SEED = SEED_GENERATOR.Next(999999); 

    public static float[,] noiseMap;
    //private static FastNoise fNoise = new FastNoise(6969);

    private System.Random rand = new System.Random(SEED);

    public static int[,] treeMap;

    public static int[,] bushMap; 

    private static bool readyToGo = false;

    private static bool initializing = false;



    public int RENDERDISTANCE = 5;

    // Set GO Objectsdd



    // Start is called before the first frame update
    void Start()
    {
        // SET BLOCKS
        AIR = new Block(true, 0);
        DIRT = new Block(Resources.Load<GameObject>("prefabs/testDirt"), 1);
        GRASS = new Block(Resources.Load<GameObject>("prefabs/Grass"), 2);
        WOOD = new Block(Resources.Load<GameObject>("prefabs/wood"), 3);
        LEAF = new Block(true, Resources.Load<GameObject>("prefabs/leaf"), false, 4);
        SNOW_LEAF = new Block(true, Resources.Load<GameObject>("prefabs/snow_leaf"), false, 5);
        WATER_FULL = new Block(true, Resources.Load<GameObject>("prefabs/waterFull"), true, 6);
        SAND = new Block(Resources.Load<GameObject>("prefabs/Sand"), 7);
        SNOW_GRASS = new Block(Resources.Load<GameObject>("prefabs/SnowGrass"), 8);

        //GRASS 1 / 8th BLOCKS 
        // GRASS18 = new Block(true, Resources.Load<GameObject>("prefabs/GRASS-1-8"), false, 0.0f, 0.0f, -.875f);
        // GRASS28 = new Block(true, Resources.Load<GameObject>("prefabs/GRASS-2-8"), false, 0.0f, 0.0f, -.75f);
        // GRASS38 = new Block(true, Resources.Load<GameObject>("prefabs/GRASS-3-8"), false, 0.0f, 0.0f, -.625f);
        // GRASS48 = new Block(true, Resources.Load<GameObject>("prefabs/GRASS-4-8"), false, 0.0f, 0.0f, -.5f);
        // GRASS58 = new Block(true, Resources.Load<GameObject>("prefabs/GRASS-5-8"), false, 0.0f, 0.0f, -.375f);
        // GRASS68 = new Block(true, Resources.Load<GameObject>("prefabs/GRASS-6-8"), false, 0.0f, 0.0f, -.25f);
        // GRASS78 = new Block(true, Resources.Load<GameObject>("prefabs/GRASS-7-8"), false, 0.0f, 0.0f, -.125f);
        //WATER_TOP = new Block(true, Resources.Load<GameObject>("prefabs/waterTop"), true);

        //TESTCHUNK GENERATION START
        Block[, ,] testChunk = new Block[255, 16, 16];

        for (int y = 0; y < 255; y++)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {

                    // When placing block, we much add .5 to each axis

                    // Creating a 3d array to hold a single chunk
                    // block maps;
                    // 0 = air
                    // 1 = dirt
                    // 2 = grass
                    // 3 = wood
                    // 4 = leaf

                    if(y < 56)
                    {
                        testChunk[y, i, j] = DIRT;
                    }

                    else if(y == 56)
                    {
                        testChunk[y, i, j] = GRASS;
                    }

                    else
                    {
                        testChunk[y, i, j] = AIR;
                    }
                }
            }
            
        }

        //Manual Blocks

        testChunk[57, 5, 7] = WOOD;
        testChunk[58, 5, 7] = WOOD;
        testChunk[59, 5, 7] = WOOD;
        
        testChunk[60, 5, 7] = LEAF;
        testChunk[60, 5, 8] = LEAF;
        testChunk[60, 5, 6] = LEAF;
        testChunk[60, 4, 7] = LEAF;
        testChunk[60, 4, 8] = LEAF;
        testChunk[60, 4, 6] = LEAF;
        testChunk[60, 6, 7] = LEAF;
        testChunk[60, 6, 8] = LEAF;
        testChunk[60, 6, 6] = LEAF;

        testChunk[61, 5, 7] = LEAF;
        testChunk[61, 5, 8] = LEAF;
        testChunk[61, 5, 6] = LEAF;
        testChunk[61, 4, 7] = LEAF;
        testChunk[61, 6, 7] = LEAF;


        testChunk[62, 5, 7] = LEAF;

        testChunk[56, 5, 5] = AIR;
        testChunk[56, 5, 4] = AIR;
        testChunk[55, 5, 4] = AIR;
        testChunk[55, 5, 5] = AIR;
        testChunk[54, 5, 5] = AIR;
        testChunk[53, 5, 5] = AIR;
        testChunk[53, 5, 5] = AIR;



       //TESTCHUNK GENERATION END

       Chunk lechunk = new Chunk(testChunk, 0, 0);
       ChunkManager.setChunk(lechunk);



    }
        
    // Update is called once per frame
    // void Update()
    // {
    //     if(!initializing){
    //         initializing = true;
    //         StartCoroutine(loadInitialData());
    //     }
    //     int currentXCoord = (int)player.transform.position.x;
    //     int currentZCoord = (int)player.transform.position.z;

    //     string[] chunkIDData = getChunkID(currentXCoord, currentZCoord);



    //     string xchunk = chunkIDData[1];
    //     string zchunk = chunkIDData[2];

    //     string startChunk = chunkIDData[0];
    //     if(readyToGo){
    //         if (startChunk != currentCenterChunk)
    //         {
    //             currentCenterChunk = startChunk;
    //             StartCoroutine(renderRadius(xchunk, zchunk, RENDERDISTANCE));
    //         }
        
    //         StartCoroutine(ChunkManager.loadNextChunk());
    //     }
        

    // }

    // private IEnumerator loadInitialData(){
    //     statusText.text = "Generating Map Data...";
    //     noiseMap = Noise.GenerateNoiseMap(32000, 32000, SEED, 350, 2, .1f, 1.0f, new Vector2(0, 0));
    //     statusText.text = "Allocation Memory for Foliage (Trees)";
    //     treeMap = new int[32000, 32000]; 
    //     statusText.text = "Allocation Memory for Foliage (Bushes)";
    //     bushMap = new int[32000, 32000];
    //     statusText.text = "Allocation Memory for Chunk Data";
    //     ChunkManager.chunks = new Chunk[4096];
    //     statusText.text = "Allocation Memory for Loaded Chunk Data";
    //     ChunkManager.loadedChunks = new bool[4096]; 

    //     for (int i = 0; i < 32000; i++)
    //     {
    //         for (int j = 0; j < 32000; j++)
    //         {

    //             if (rand.Next(300) == 45)
    //             {
    //                 treeMap[i, j] = 1;
    //             }
    //             else
    //             {
    //                 treeMap[i, j] = 0;
    //             }
    //             if (rand.Next(500) == 45)
    //             {
    //                 bushMap[i, j] = 1;
    //             }
    //             else
    //             {
    //                 bushMap[i, j] = 0;
    //             }
    //             //treeMap[i, j] = (int)Math.Round(fNoise.GetNoise(i, j));
    //         }
    //         statusText.text = "Planting Trees - " + (i / 32000) * 100 + "%";
    //     }

    //     int currentXCoord = (int)player.transform.position.x;
    //     int currentZCoord = (int)player.transform.position.z;

    //     string[] chunkIDData = getChunkID(currentXCoord, currentZCoord);

    //     string startChunk = chunkIDData[0];

    //     ChunkManager.chunksToLoad.Enqueue(startChunk); // Render the chunk directly below the player first
    //     player.transform.position = new Vector3(currentXCoord, 150, currentZCoord);
    //     readyToGo = true;
    //     statusText.text = "Done.";
    //     yield break;
    // }


    // private static int getStep(float value)
    // {
    //     UnityEngine.Debug.Log(value);
    //     if (value < 0.125f)
    //     {
    //         return 0;
    //     }
    //     else if (value < 0.24f)
    //     {
    //         return 1;
    //     }
    //     else if (value < 0.375f)
    //     {
    //         return 2;
    //     }
    //     else if (value < 0.5f)
    //     {
    //         return 3;
    //     }
    //     else if (value < 0.625f)
    //     {
    //         return 4;
    //     }
    //     else if (value < 0.75f)
    //     {
    //         return 5;
    //     }
    //     else if (value < 0.875f)
    //     {
    //         return 6;
    //     }
    //     else if (value < 1)
    //     {
    //         return 7;
    //     }
    //     else
    //     {
    //         return 8;
    //     }
    // }

    // public static IEnumerator generateChunkData(int chunkID)
    // {
    //     //UnityEngine.Debug.Log("Generating Chunk Data for:" + chunkID);

    //     int[] XZRange = ChunkManager.getCoordRange(insureEightDigits(chunkID));
    //     int XStart = XZRange[1];
    //     int ZStart = XZRange[0];

    //     int XEnd = XZRange[3];
    //     int ZEnd = XZRange[2];

    //     int XS = XStart;
    //     int chunkZPOSITIVE = int.Parse(insureEightDigits(chunkID).Substring(0, 1));
    //     int chunkXPOSITIVE = int.Parse(insureEightDigits(chunkID).Substring(4, 1));

    //     // Create a chunk
    //     Block[,,] tempChumk = new Block[255, 16, 16];
    //     if (chunkXPOSITIVE == 0 && XS > 0)
    //     {
    //         XS = XEnd * (-1);
    //     }

    //     for (int x = 0; x < 16; x++)
    //     {
    //         int ZS = ZStart;
    //         if (chunkZPOSITIVE == 0 && ZS > 0)
    //         {
    //             ZS = ZEnd * (-1);
    //         }

    //         for (int z = 0; z < 16; z++)
    //         {
    //             // Noise map effect range of y: 35 - 150
    //             int deferAir = 0;
    //             bool skipGrass = false;
    //             float yTop = (arraytoterraintest.noiseMap[XS + 1600, ZS + 1600] * 115) + 35;
    //             int ytopInt = (int)Math.Floor(yTop);
    //             // if(getStep(yTop - (int)Math.Floor(yTop)) != 0 && getStep(yTop - (int)Math.Floor(yTop)) != 8){
    //             //     switch (getStep(yTop - (int)Math.Floor(yTop)))
    //             //     {
    //             //         case 1:
    //             //             tempChumk[ytopInt + 1, x, z] = GRASS18;
    //             //             deferAir = 1;
    //             //             break;
    //             //         case 2:
    //             //             tempChumk[ytopInt + 1, x, z] = GRASS28;
    //             //             deferAir = 1;
    //             //             break;
    //             //         case 3:
    //             //             tempChumk[ytopInt + 1, x, z] = GRASS38;
    //             //             deferAir = 1;
    //             //             break;
    //             //         case 4:
    //             //             tempChumk[ytopInt + 1, x, z] = GRASS48;
    //             //             deferAir = 1;
    //             //             break;
    //             //         case 5:
    //             //             tempChumk[ytopInt + 1, x, z] = GRASS58;
    //             //         deferAir = 1;
    //             //             break;
    //             //         case 6:
    //             //             tempChumk[ytopInt + 1, x, z] = GRASS68;
    //             //             deferAir = 1;
    //             //             break;
    //             //         case 7:
    //             //             tempChumk[ytopInt + 1, x, z] = GRASS78;
    //             //             deferAir = 1;
    //             //             break;
    //             //         default:
    //             //             UnityEngine.Debug.Log("Setting SKip Grass");
    //             //             //skipGrass = true;
    //             //             break;
    //             //     }
    //             // }
    //             if (ytopInt < 68)
    //             {
    //                 tempChumk[ytopInt, x, z] = DIRT;
    //             }
    //             else if (ytopInt >= 68 && ytopInt < 72)
    //             {
    //                 tempChumk[ytopInt, x, z] = SAND;
    //             }
    //             else if (ytopInt > 100)
    //             {
    //                 tempChumk[ytopInt, x, z] = SNOW_GRASS;

    //                 if (bushMap[XS + 1600, ZS + 1600] == 1)
    //                 {
    //                     ytopInt++;
    //                     tempChumk[ytopInt, x, z] = WOOD;
    //                     tempChumk[ytopInt + 1, x, z] = SNOW_LEAF;
    //                     deferAir = 1;
    //                 }
    //                 else if (treeMap[XS + 1600, ZS + 1600] == 1)
    //                 {
    //                     ytopInt++;
    //                     tempChumk[ytopInt, x, z] = WOOD;
    //                     tempChumk[ytopInt + 1, x, z] = WOOD;
    //                     tempChumk[ytopInt + 2, x, z] = WOOD;
    //                     tempChumk[ytopInt + 3, x, z] = WOOD;
    //                     tempChumk[ytopInt + 4, x, z] = SNOW_LEAF;
    //                     tempChumk[ytopInt + 5, x, z] = SNOW_LEAF;
    //                     deferAir = 5;
    //                 }
    //             }
    //             else if (bushMap[XS + 1600, ZS + 1600] == 1)
    //             {
    //                 ytopInt++;
    //                 tempChumk[ytopInt, x, z] = WOOD;
    //                 tempChumk[ytopInt + 1, x, z] = LEAF;
    //                 deferAir = 1;
    //             }
    //             else if (treeMap[XS + 1600, ZS + 1600] == 1)
    //             {
    //                 ytopInt++;
    //                 tempChumk[ytopInt, x, z] = WOOD;
    //                 tempChumk[ytopInt + 1, x, z] = WOOD;
    //                 tempChumk[ytopInt + 2, x, z] = WOOD;
    //                 tempChumk[ytopInt + 3, x, z] = WOOD;
    //                 tempChumk[ytopInt + 4, x, z] = LEAF;
    //                 tempChumk[ytopInt + 5, x, z] = LEAF;
    //                 deferAir = 5;

    //             }
    //             else
    //             {
    //                 if (!skipGrass)
    //                 {
    //                     tempChumk[ytopInt, x, z] = GRASS;
    //                 }
    //             }
    //             if (ytopInt < 70)
    //             {
    //                 //anything higher until 70 is water
    //                 for (int y = ytopInt + 1; y <= 70; y++)
    //                 {
    //                     tempChumk[y, x, z] = WATER_FULL;
    //                 }
    //                 // Anything higher than 70 is air
    //                 for (int y = 71; y < 255; y++)
    //                 {
    //                     tempChumk[y, x, z] = AIR;
    //                 }
    //             }
    //             else
    //             {

    //                 for (int y = ytopInt + 1 + deferAir; y < 255; y++)
    //                 {
    //                     tempChumk[y, x, z] = AIR;
    //                 }
    //             }
    //             //Anything Below Dirt
    //             for (int y = ytopInt - 1; y >= 0; y--)
    //             {
    //                 tempChumk[y, x, z] = DIRT;
    //             }


    //             // After the fact checks
    //             // Is there a chair to the left right north or south us this block?
    //             //Z Axis
    //             // Can we Check Z?
    //             if ((treeMap[XS + 1600, (ZS + 1600) + 1] == 1))
    //             {
    //                 float yTopt = (arraytoterraintest.noiseMap[XS + 1600, (ZS + 1600) + 1] * 115) + 35;
    //                 int ytopIntt = (int)Math.Floor(yTopt);
    //                 if (ytopIntt >= 72 && ytopIntt <= 100)
    //                 {
    //                     tempChumk[ytopIntt + 3, x, z] = LEAF;
    //                     tempChumk[ytopIntt + 4, x, z] = LEAF;
    //                     tempChumk[ytopIntt + 5, x, z] = LEAF;
    //                 }
    //                 else if (ytopIntt > 100)
    //                 {
    //                     tempChumk[ytopIntt + 3, x, z] = SNOW_LEAF;
    //                     tempChumk[ytopIntt + 4, x, z] = SNOW_LEAF;
    //                     tempChumk[ytopIntt + 5, x, z] = SNOW_LEAF;
    //                 }

    //             }
    //             if ((treeMap[XS + 1600, (ZS + 1600) - 1] == 1))
    //             {
    //                 float yTopt = (arraytoterraintest.noiseMap[XS + 1600, (ZS + 1600) - 1] * 115) + 35;
    //                 int ytopIntt = (int)Math.Floor(yTopt);
    //                 if (ytopIntt >= 72 && ytopIntt <= 100)
    //                 {
    //                     tempChumk[ytopIntt + 3, x, z] = LEAF;
    //                     tempChumk[ytopIntt + 4, x, z] = LEAF;
    //                     tempChumk[ytopIntt + 5, x, z] = LEAF;
    //                 }
    //                 else if (ytopIntt > 100)
    //                 {
    //                     tempChumk[ytopIntt + 3, x, z] = SNOW_LEAF;
    //                     tempChumk[ytopIntt + 4, x, z] = SNOW_LEAF;
    //                     tempChumk[ytopIntt + 5, x, z] = SNOW_LEAF;
    //                 }
    //             }
    //             if ((treeMap[(XS + 1600) + 1, (ZS + 1600)] == 1))
    //             {
    //                 float yTopt = (arraytoterraintest.noiseMap[(XS + 1600) + 1, ZS + 1600] * 115) + 35;
    //                 int ytopIntt = (int)Math.Floor(yTopt);
    //                 if (ytopIntt >= 72 && ytopIntt <= 100)
    //                 {
    //                     tempChumk[ytopIntt + 3, x, z] = LEAF;
    //                     tempChumk[ytopIntt + 4, x, z] = LEAF;
    //                     tempChumk[ytopIntt + 5, x, z] = LEAF;
    //                 }
    //                 else if (ytopIntt > 100)
    //                 {
    //                     tempChumk[ytopIntt + 3, x, z] = SNOW_LEAF;
    //                     tempChumk[ytopIntt + 4, x, z] = SNOW_LEAF;
    //                     tempChumk[ytopIntt + 5, x, z] = SNOW_LEAF;
    //                 }
    //             }
    //             if ((treeMap[(XS + 1600) - 1, (ZS + 1600)] == 1))
    //             {
    //                 float yTopt = (arraytoterraintest.noiseMap[(XS + 1600) - 1, ZS + 1600] * 115) + 35;
    //                 int ytopIntt = (int)Math.Floor(yTopt);
    //                 if (ytopIntt >= 72 && ytopIntt <= 100)
    //                 {
    //                     tempChumk[ytopIntt + 3, x, z] = LEAF;
    //                     tempChumk[ytopIntt + 4, x, z] = LEAF;
    //                     tempChumk[ytopIntt + 5, x, z] = LEAF;
    //                 }
    //                 else if (ytopIntt > 100)
    //                 {
    //                     tempChumk[ytopIntt + 3, x, z] = SNOW_LEAF;
    //                     tempChumk[ytopIntt + 4, x, z] = SNOW_LEAF;
    //                     tempChumk[ytopIntt + 5, x, z] = SNOW_LEAF;
    //                 }
    //             }
    //             ZS++;
    //         }
    //         XS++;
    //     }
    //     ChunkManager.setChunk(chunkID, tempChumk);
    //     yield break;
    // }

    // private string[] getChunkID(int x, int z)
    // {
    //     int currentXCoord = x;
    //     int currentZCoord = z;
    //     //UnityEngine.Debug.Log((int) currentXCoord);
    //     //UnityEngine.Debug.Log((int) currentZCoord);
    //     string xchunk = "0000";
    //     string zchunk = "0000";
    //     int xchunkI = 0;
    //     int zchunkI = 0;
    //     if (currentXCoord >= 0 && currentXCoord <= 15)
    //     {
    //         xchunkI = 1000;
    //     }
    //     else if (currentXCoord <= -1 && currentXCoord >= -16)
    //     {
    //         xchunkI = 0000;
    //     }

    //     if (currentZCoord >= 0 && currentZCoord <= 15)
    //     {
    //         zchunkI = 1000;
    //     }
    //     else if (currentZCoord <= -1 && currentZCoord >= -16)
    //     {
    //         zchunkI = 0000;
    //     }

    //     // Positive Axis

    //     if (currentZCoord >= 0)
    //     {
    //         zchunkI = (currentZCoord / 16) + 1000;
    //     }
    //     if (currentXCoord >= 0)
    //     {
    //         xchunkI = (currentXCoord / 16) + 1000;
    //     }

    //     // Negative Axis
    //     if (currentZCoord < 0)
    //     {
    //         zchunkI = ((currentZCoord * -1) - 1) / 16;
    //     }
    //     if (currentXCoord < 0)
    //     {
    //         xchunkI = ((currentXCoord * -1) - 1) / 16;
    //     }


    //     xchunk = insureQuadDigit(xchunkI);
    //     zchunk = insureQuadDigit(zchunkI);

    //     string startChunk = zchunk + xchunk;

    //     string[] returnString = new string[3];

    //     returnString[0] = startChunk;
    //     returnString[1] = xchunk;
    //     returnString[2] = zchunk;

    //     return returnString;

    // }

    // IEnumerator renderRadius(string xchunk, string zchunk, int radius)
    // {



    //     string firstChunkXAxisToRender = xchunk;
    //     string firstChunkZAxisToRender = zchunk;

    //     for (int i = 0; i < radius; i++)
    //     {
    //         firstChunkXAxisToRender = incrementChunk(firstChunkXAxisToRender);
    //         firstChunkZAxisToRender = decrementChunk(firstChunkZAxisToRender);
    //     }

    //     string lastChunkXAxisToRender = xchunk;
    //     string lastChunkZAxisToRender = zchunk;

    //     for (int i = 0; i < radius; i++)
    //     {
    //         lastChunkXAxisToRender = decrementChunk(lastChunkXAxisToRender);
    //         lastChunkZAxisToRender = incrementChunk(lastChunkZAxisToRender);
    //     }



    //     // Make an array of chunks we want rendered.

    //     string[] chunksWeWantRendered = new string[(radius * 2) * (radius * 2)]; // 32 * 32 render distance max
    //     string[] chunksWeWantUnrendered = new string[1024];
    //     int counter = 0;
    //     for (string xAxis = firstChunkXAxisToRender; xAxis != lastChunkXAxisToRender; xAxis = decrementChunk(xAxis))
    //     {
    //         for (string zAxis = firstChunkZAxisToRender; zAxis != lastChunkZAxisToRender; zAxis = incrementChunk(zAxis))
    //         {
    //             chunksWeWantRendered[counter] = zAxis + xAxis;
    //             counter++;
    //         }
    //     }

    //     //UnityEngine.Debug.Log(counter);

    //     string[] loadedChunkList = new string[ChunkManager.loadedChunkList.Count];
    //     ChunkManager.loadedChunkList.CopyTo(loadedChunkList);

    //     // Render those chunks

    //     foreach (string chunk in chunksWeWantRendered)
    //     {
    //         bool match = false;
    //         foreach (string wantedChunk in loadedChunkList)
    //         {
    //             if (wantedChunk == chunk)
    //             {
    //                 match = true;
    //             }
    //         }
    //         if (!match)
    //         {
    //             //UnityEngine.Debug.Log("Rendering Chunk" + chunk);
    //             //yield return Chunk.loadChunk(chunk);
    //             if (!ChunkManager.chunksToLoad.Contains(chunk))
    //             {
    //                 ChunkManager.chunksToLoad.Enqueue(chunk);
    //             }

    //         }
    //     }
    //     // for(int j = 0; j < counter; j++){

    //     //     yield return Chunk.loadChunk(chunksWeWantRendered[j]);
    //     //     //generateChunkData(int.Parse(chunksWeWantRendered[j]));
    //     // }


    //     foreach (string chunk in loadedChunkList)
    //     {
    //         bool matchFound = false;
    //         for (int j = 0; j < counter; j++)
    //         {
    //             if (chunk == chunksWeWantRendered[j])
    //             {
    //                 matchFound = true;
    //             }
    //         }
    //         if (!matchFound)
    //         {
    //             //UnityEngine.Debug.Log("UNLOADING CHUNK!");
    //             yield return ChunkManager.unloadChunk(chunk);
    //         }
    //     }

    //     // for(int j = 0; j < subCounter; j++){
    //     //     yield return Chunk.unloadChunk(chunksWeWantUnrendered[j]);
    //     // }
    //     yield break;
    // }

    // public static string insureQuadDigit(int num)
    // {
    //     if (num < 10)
    //     {
    //         return "000" + num;
    //     }
    //     if (num < 100)
    //     {
    //         return "00" + num;
    //     }
    //     if (num < 1000){
    //         return "0" + num;
    //     }

    //     return num.ToString();
    // }

    // public static string insureEightDigits(int num)
    // {
    //     if (num < 10)
    //     {
    //         return "0000000" + num;
    //     }
    //     if (num < 100)
    //     {
    //         return "000000" + num;
    //     }
    //     if (num < 1000)
    //     {
    //         return "00000" + num;
    //     }
    //     if (num < 10000)
    //     {
    //         return "0000" + num;
    //     }
    //     if (num < 100000)
    //     {
    //         return "000" + num;
    //     }if(num < 1000000){
    //         return "00" + num;
    //     }if(num < 10000000){
    //         return "0" + num;
    //     }
    //     return num.ToString();

    // }

    // public static string decrementChunk(string chunk)
    // {

    //     string chunkF = insureQuadDigit(int.Parse(chunk));

    //     int cPOSITIVE = int.Parse(chunkF.Substring(0, 1));
    //     int c = int.Parse(chunkF.Substring(1, 3));

    //     if (c == 0 && cPOSITIVE == 1)
    //     {
    //         cPOSITIVE = 0;
    //     }
    //     else if (cPOSITIVE == 1)
    //     {
    //         c--;
    //     }
    //     else if (cPOSITIVE == 0)
    //     {
    //         c++;
    //     }
    //     else
    //     {
    //         UnityEngine.Debug.Log("Decrement Chunk Unexpected Outcome.. cXPOS: " + cPOSITIVE + " c: " + c);
    //     }

    //     if (cPOSITIVE == 1)
    //     {
    //         return insureQuadDigit(c + 1000);
    //     }
    //     return insureQuadDigit(c);
    // }


    // public static string incrementChunk(string chunk)
    // {
    //     string chunkF = insureQuadDigit(int.Parse(chunk));

    //     int cPOSITIVE = int.Parse(chunkF.Substring(0, 1));
    //     int c = int.Parse(chunkF.Substring(1, 3));


    //     if (c == 0 && cPOSITIVE == 0)
    //     {
    //         cPOSITIVE = 1;
    //     }
    //     else if (cPOSITIVE == 1)
    //     {
    //         c++;
    //     }
    //     else if (cPOSITIVE == 0)
    //     {
    //         c--;
    //     }
    //     else
    //     {
    //         UnityEngine.Debug.Log("Increment Chunk Unexpected Outcome.. cXPOS: " + cPOSITIVE + " cX: " + c);
    //     }

    //     if (cPOSITIVE == 1)
    //     {
    //         return insureQuadDigit(c + 1000);
    //     }
    //     return insureQuadDigit(c);

    // }


}


