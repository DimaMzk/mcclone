using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;

public class arraytoterraintest : MonoBehaviour
{

    public GameObject player;

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

    public int currentXchunk;
    public int currentZchunk;

    private static System.Random SEED_GENERATOR = new System.Random();
    public static int SEED = SEED_GENERATOR.Next(999999); 

    public static float[,] noiseMap;
    //private static FastNoise fNoise = new FastNoise(6969);

    private System.Random rand = new System.Random(SEED);

    public static int[,] treeMap;

    public static int[,] bushMap; 

    private static bool readyToGo = false;

    private static bool initializing = false;



    public int RENDERDISTANCE = 3; 

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

    //     //TESTCHUNK GENERATION START
    //   

        int currentXCoord = (int)player.transform.position.x;
        int currentZCoord = (int)player.transform.position.z;

        int[] chunkIDData = coordToChunk(currentXCoord, currentZCoord);



        int xchunk = chunkIDData[0];
        int zchunk = chunkIDData[1];

        Chunk chunk = ChunkManager.getChunk(xchunk, zchunk);


        ChunkManager.chunksToLoad.Enqueue(chunk); // Render the chunk directly below the player first
        player.transform.position = new Vector3(currentXCoord, 150, currentZCoord);
        currentXchunk = xchunk;
        currentZchunk = zchunk;
        StartCoroutine(renderRadius(xchunk, zchunk, RENDERDISTANCE));



    }
        
    //Update is called once per frame
    void Update()
    {

        int currentXCoord = (int)player.transform.position.x;
        int currentZCoord = (int)player.transform.position.z;

        int[] chunkIDData = coordToChunk(currentXCoord, currentZCoord);



        int xchunk = chunkIDData[0];
        int zchunk = chunkIDData[1];


    if (xchunk != currentXchunk || zchunk != currentZchunk)
    {
        currentXchunk = xchunk;
        currentZchunk = zchunk;
        StartCoroutine(renderRadius(xchunk, zchunk, RENDERDISTANCE));
    }

    StartCoroutine(ChunkManager.loadNextChunk());
        
        

    }

    private IEnumerator loadInitialData(){

        
        readyToGo = true;
        //statusText.text = "Done.";
        yield break;
    }


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

    public static Chunk generateChunkData(int posX, int posZ)
    {
        //UnityEngine.Debug.Log("Generating Chunk Data for:" + chunkID);

        int[] XZRange = ChunkManager.getCoordRange(posX, posZ);
        int XStart = XZRange[1];
        int ZStart = XZRange[0];

        int XEnd = XZRange[3];
        int ZEnd = XZRange[2];

        //Need XStarts for Agasent Chunks
        int[] XZRangezp = ChunkManager.getCoordRange(posX, posZ + 1);
        int XStartzp = XZRange[1];
        int ZStartzp = XZRange[0];

        int[] XZRangezn = ChunkManager.getCoordRange(posX, posZ - 1);
        int XStartzn = XZRange[1];
        int ZStartzn = XZRange[0];

        int[] XZRangexp = ChunkManager.getCoordRange(posX + 1, posZ);
        int XStartxp = XZRange[1];
        int ZStartxp = XZRange[0];

        int[] XZRangexn = ChunkManager.getCoordRange(posX - 1, posZ);
        int XStartxn = XZRange[1];
        int ZStartxn = XZRange[0];

        int[] XZRangezpxp = ChunkManager.getCoordRange(posX + 1, posZ + 1);
        int XStartzpxp = XZRange[1];
        int ZStartzpxp = XZRange[0];

        int[] XZRangezpxn = ChunkManager.getCoordRange(posX - 1, posZ + 1);
        int XStartzpxn = XZRange[1];
        int ZStartzpxn = XZRange[0];

        int[] XZRangeznxn = ChunkManager.getCoordRange(posX - 1, posZ - 1);
        int XStartznxn = XZRange[1];
        int ZStartznxn = XZRange[0];

        int[] XZRangeznxp = ChunkManager.getCoordRange(posX + 1, posZ - 1);
        int XStartznxp = XZRange[1];
        int ZStartznxp = XZRange[0];

        // Need to generate heightmap for chunk area

        //float[,] noiseMap = Noise.GenerateNoiseMap(48, 48, SEED, 350, 1, .1f, 1.0f, new Vector2(XStart, ZStart));
        float[,] noiseMap = Noise.GenerateNoiseMap(48, 48, SEED, 0, 1, .1f, 1.0f, new Vector2(XStart, ZStart));

        
        // Need to generate Treemap AND Bushmap

        int[,] treeMap = new int[48, 48];
        int[,] bushMap = new int[48, 48];

        //Insure that the same treemap will be generate for this chunk everytime, for any given seed
        System.Random rand = new System.Random(SEED + (XStart.ToString() + ZStart.ToString()).GetHashCode());

        // Create Treemaps for agasent chunks
        System.Random randzp = new System.Random(SEED + (XStartzp.ToString() + ZStartzp.ToString()).GetHashCode());
        System.Random randzn = new System.Random(SEED + (XStartzn.ToString() + ZStartzn.ToString()).GetHashCode());
        System.Random randxp = new System.Random(SEED + (XStartxp.ToString() + ZStartxp.ToString()).GetHashCode());
        System.Random randxn = new System.Random(SEED + (XStartxn.ToString() + ZStartxn.ToString()).GetHashCode());

        System.Random randzpxp = new System.Random(SEED + (XStartzpxp.ToString() + ZStartzpxp.ToString()).GetHashCode());
        System.Random randznxp = new System.Random(SEED + (XStartznxp.ToString() + ZStartznxp.ToString()).GetHashCode());
        System.Random randzpxn = new System.Random(SEED + (XStartzpxn.ToString() + ZStartzpxn.ToString()).GetHashCode());
        System.Random randznxn = new System.Random(SEED + (XStartznxn.ToString() + ZStartznxn.ToString()).GetHashCode());


        for(int i = 0; i < 48; i++){
            for(int j = 0; j < 48; j++){

                if(i < 16 && j < 16){
                    //      +
                    //   |# O O
                    // - |O O O
                    //   |O O O
                    if(randznxp.Next(300) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }

                    if(randznxp.Next(500) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }

                }

                if(i >= 16 && i < 32 && j < 16){
                    // O O O
                    // # O O
                    // O O O
                    if(randzn.Next(300) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }

                    if(randzn.Next(500) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }
                }

                if(i >= 32 && j < 16){
                    // O O O
                    // O O O
                    // # O O
                    if(randznxn.Next(300) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }

                    if(randznxn.Next(500) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }
                }

                if(i < 16 && j >= 16 && j < 32){
                    // O # O
                    // O O O
                    // O O O
                    if(randxp.Next(300) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }

                    if(randxp.Next(500) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }
                }

                if(i >= 16 && i < 32 && j >= 16 && j < 32){
                    // O O O
                    // O # O
                    // O O O
                    if(rand.Next(300) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }

                    if(rand.Next(500) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }
                }

                if(i >= 32 && j >= 16 && j < 32){
                    // O O O
                    // O O O
                    // O # O
                    if(randxn.Next(300) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }

                    if(randxn.Next(500) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }
                }

                if (i < 16 && j >= 32){
                    // O O #
                    // O O O
                    // O O O
                    if(randzpxp.Next(300) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }

                    if(randzpxp.Next(500) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }
                }

                if(i >= 16 && i < 32 && j >= 32){
                    // O O O
                    // O O #
                    // O O O
                    if(randzp.Next(300) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }

                    if(randzp.Next(500) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }
                }

                if(i >= 32 && j >= 32){
                    // O O O
                    // O O O
                    // O O #
                    if(randzpxn.Next(300) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }

                    if(randzpxn.Next(500) == 45)
                    {
                        treeMap[i, j] = 1;
                    }
                    else
                    {
                        treeMap[i, j] = 0;
                    }
                }
            }
        }

        int XS = XStart;

        // Create a chunk
        Block[,,] tempChumk = new Block[255, 16, 16];
        if (XS < 0)
        {
            XS = XEnd * (-1);
        }

        for (int x = 0; x < 16; x++)
        {
            int ZS = ZStart;
            if (ZS < 0)
            {
                ZS = ZEnd * (-1);
            }

            for (int z = 0; z < 16; z++)
            {
                // Noise map effect range of y: 35 - 150
                int deferAir = 0;
                float yTop = (noiseMap[x + 16, z + 16] * 115) + 35;
                int ytopInt = (int)Math.Floor(yTop);
                
                if (ytopInt < 68)
                {
                    tempChumk[ytopInt, x, z] = DIRT;
                }
                else if (ytopInt >= 68 && ytopInt < 72)
                {
                    tempChumk[ytopInt, x, z] = SAND;
                }
                else if (ytopInt > 100)
                {
                    tempChumk[ytopInt, x, z] = SNOW_GRASS;

                    if (bushMap[x + 16, z + 16] == 1)
                    {
                        ytopInt++;
                        tempChumk[ytopInt, x, z] = WOOD;
                        tempChumk[ytopInt + 1, x, z] = SNOW_LEAF;
                        deferAir = 1;
                    }
                    else if (treeMap[x + 16, z + 16] == 1)
                    {
                        ytopInt++;
                        tempChumk[ytopInt, x, z] = WOOD;
                        tempChumk[ytopInt + 1, x, z] = WOOD;
                        tempChumk[ytopInt + 2, x, z] = WOOD;
                        tempChumk[ytopInt + 3, x, z] = WOOD;
                        tempChumk[ytopInt + 4, x, z] = SNOW_LEAF;
                        tempChumk[ytopInt + 5, x, z] = SNOW_LEAF;
                        deferAir = 5;
                    }
                }
                else if (bushMap[x + 16, z + 16] == 1)
                {
                    ytopInt++;
                    tempChumk[ytopInt, x, z] = WOOD;
                    tempChumk[ytopInt + 1, x, z] = LEAF;
                    deferAir = 1;
                }
                else if (treeMap[x + 16, z + 16] == 1)
                {
                    ytopInt++;
                    tempChumk[ytopInt, x, z] = WOOD;
                    tempChumk[ytopInt + 1, x, z] = WOOD;
                    tempChumk[ytopInt + 2, x, z] = WOOD;
                    tempChumk[ytopInt + 3, x, z] = WOOD;
                    tempChumk[ytopInt + 4, x, z] = LEAF;
                    tempChumk[ytopInt + 5, x, z] = LEAF;
                    deferAir = 5;

                }
                else
                {
                    
                    
                        tempChumk[ytopInt, x, z] = GRASS;
                
                }
                if (ytopInt < 70)
                {
                    //anything higher until 70 is water
                    for (int y = ytopInt + 1; y <= 70; y++)
                    {
                        tempChumk[y, x, z] = WATER_FULL;
                    }
                    // Anything higher than 70 is air
                    for (int y = 71; y < 255; y++)
                    {
                        tempChumk[y, x, z] = AIR;
                    }
                }
                else
                {

                    for (int y = ytopInt + 1 + deferAir; y < 255; y++)
                    {
                        tempChumk[y, x, z] = AIR;
                    }
                }
                //Anything Below Dirt
                for (int y = ytopInt - 1; y >= 0; y--)
                {
                    tempChumk[y, x, z] = DIRT;
                }


                // After the fact checks
                // Is there a chair to the left right north or south us this block?
                //Z Axis
                // Can we Check Z?
                if ((treeMap[x + 16, (z + 16) + 1] == 1))
                {
                    float yTopt = (noiseMap[x + 16, (z + 16) + 1] * 115) + 35;
                    int ytopIntt = (int)Math.Floor(yTopt);
                    if (ytopIntt >= 72 && ytopIntt <= 100)
                    {
                        tempChumk[ytopIntt + 3, x, z] = LEAF;
                        tempChumk[ytopIntt + 4, x, z] = LEAF;
                        tempChumk[ytopIntt + 5, x, z] = LEAF;
                    }
                    else if (ytopIntt > 100)
                    {
                        tempChumk[ytopIntt + 3, x, z] = SNOW_LEAF;
                        tempChumk[ytopIntt + 4, x, z] = SNOW_LEAF;
                        tempChumk[ytopIntt + 5, x, z] = SNOW_LEAF;
                    }

                }
                if ((treeMap[x + 16, (z + 16) - 1] == 1)) // This Looks Bad, But I need to redo it When I do biomes anyways, just need inifnite chunk system to WORK, before I refine this code lmao.
                {
                    float yTopt = (noiseMap[x + 16, (z + 16) - 1] * 115) + 35;
                    int ytopIntt = (int)Math.Floor(yTopt);
                    if (ytopIntt >= 72 && ytopIntt <= 100)
                    {
                        tempChumk[ytopIntt + 3, x, z] = LEAF;
                        tempChumk[ytopIntt + 4, x, z] = LEAF;
                        tempChumk[ytopIntt + 5, x, z] = LEAF;
                    }
                    else if (ytopIntt > 100)
                    {
                        tempChumk[ytopIntt + 3, x, z] = SNOW_LEAF;
                        tempChumk[ytopIntt + 4, x, z] = SNOW_LEAF;
                        tempChumk[ytopIntt + 5, x, z] = SNOW_LEAF;
                    }
                }
                if ((treeMap[(x + 16) + 1, (z + 16)] == 1))
                {
                    float yTopt = (noiseMap[(x + 16) + 1, z + 16] * 115) + 35;
                    int ytopIntt = (int)Math.Floor(yTopt);
                    if (ytopIntt >= 72 && ytopIntt <= 100)
                    {
                        tempChumk[ytopIntt + 3, x, z] = LEAF;
                        tempChumk[ytopIntt + 4, x, z] = LEAF;
                        tempChumk[ytopIntt + 5, x, z] = LEAF;
                    }
                    else if (ytopIntt > 100)
                    {
                        tempChumk[ytopIntt + 3, x, z] = SNOW_LEAF;
                        tempChumk[ytopIntt + 4, x, z] = SNOW_LEAF;
                        tempChumk[ytopIntt + 5, x, z] = SNOW_LEAF;
                    }
                }
                if ((treeMap[(x + 16) - 1, (z + 16)] == 1))
                {
                    float yTopt = (noiseMap[(x + 16) - 1, z + 16] * 115) + 35;
                    int ytopIntt = (int)Math.Floor(yTopt);
                    if (ytopIntt >= 72 && ytopIntt <= 100)
                    {
                        tempChumk[ytopIntt + 3, x, z] = LEAF;
                        tempChumk[ytopIntt + 4, x, z] = LEAF;
                        tempChumk[ytopIntt + 5, x, z] = LEAF;
                    }
                    else if (ytopIntt > 100)
                    {
                        tempChumk[ytopIntt + 3, x, z] = SNOW_LEAF;
                        tempChumk[ytopIntt + 4, x, z] = SNOW_LEAF;
                        tempChumk[ytopIntt + 5, x, z] = SNOW_LEAF;
                    }
                }
                ZS++;
            }
            XS++;
        }

        Chunk chunkToSet = new Chunk(tempChumk, posX, posZ);
        ChunkManager.chunks.Add(chunkToSet);
        return chunkToSet;
    }

    private int[] coordToChunk(int x, int z)
    {
        int currentXCoord = x;
        int currentZCoord = z;
        int xchunkI = 0;
        int zchunkI = 0;
        if (currentXCoord >= 0 && currentXCoord <= 15)
        {
            xchunkI = 0;
        }
        else if (currentXCoord <= -1 && currentXCoord >= -16)
        {
            xchunkI = -1;
        }

        if (currentZCoord >= 0 && currentZCoord <= 15)
        {
            zchunkI = 0;
        }
        else if (currentZCoord <= -1 && currentZCoord >= -16)
        {
            zchunkI = -1;
        }

        // Positive Axis

        if (currentZCoord >= 0)
        {
            zchunkI = (currentZCoord / 16);
        }
        if (currentXCoord >= 0)
        {
            xchunkI = (currentXCoord / 16);
        }

        // Negative Axis
        if (currentZCoord < 0)
        {
            zchunkI = (currentZCoord) / 16;
        }
        if (currentXCoord < 0)
        {
            xchunkI = (currentXCoord) / 16;
        }
        int[] returnString = new int[2];

        returnString[0] = xchunkI;
        returnString[1] = zchunkI;

        return returnString;

    }

    IEnumerator renderRadius(int xchunk, int zchunk, int radius)
    {
        int firstChunkXAxisToRender = xchunk;
        int firstChunkZAxisToRender = zchunk;

        for (int i = 0; i < radius; i++)
        {
            firstChunkXAxisToRender++;
            firstChunkZAxisToRender--;
        } 
        UnityEngine.Debug.Log("First Chunk: " + firstChunkXAxisToRender + ", " + firstChunkZAxisToRender);

        int lastChunkXAxisToRender = xchunk;
        int lastChunkZAxisToRender = zchunk;

        for (int i = 0; i < radius; i++)
        {
            lastChunkXAxisToRender--;
            lastChunkZAxisToRender++;
        }
        UnityEngine.Debug.Log("Last Chunk: " + lastChunkXAxisToRender + ", " + lastChunkZAxisToRender);


        // Make an array of chunks we want rendered.
        int[,] chunksWeWantRendered = new int[1024, 2];
        int counter = 0;
        for (int xAxis = firstChunkXAxisToRender; xAxis != lastChunkXAxisToRender; xAxis--)
        {
            for (int zAxis = firstChunkZAxisToRender; zAxis != lastChunkZAxisToRender; zAxis++)
            {
                chunksWeWantRendered[counter, 0] = xAxis;
                chunksWeWantRendered[counter, 1] = zAxis;
                Chunk tChunk = ChunkManager.getChunk(xAxis, zAxis);
                if(!tChunk.getIsLoaded()){
                    ChunkManager.chunksToLoad.Enqueue(tChunk);
                }
                counter++;
            }
        }
        List<Chunk> chunksCopy = new List<Chunk>(ChunkManager.chunks);
        foreach (Chunk chunk in chunksCopy){
            bool match = false;
            for (int j = 0; j < counter; j++){
                if(chunk.getIdX() == chunksWeWantRendered[j, 0] && chunk.getIdZ() == chunksWeWantRendered[j, 1]){
                    match = true;
                }
            }
            if(!match){
                yield return ChunkManager.unloadChunk(chunk);
            }
        }
        //yield return ChunkManager.flushUnloadedChunks();
        yield break;
    }
}


