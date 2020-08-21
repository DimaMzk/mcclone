using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public static Block AIR;
    public static Block WATER_FULL;
    public static Block SAND;
    //public static Block WATER_TOP;

    public String currentCenterChunk;

    public static float[,] noiseMap = Noise.GenerateNoiseMap(3200, 3200, 850, 150, 2, .1f, 1.0f, new Vector2(0, 0));
    //private static FastNoise fNoise = new FastNoise(6969);

    private System.Random rand = new System.Random();

    public static int[,] treeMap = new int[3200, 3200];

    public static int[,] bushMap = new int[3200,3200];




    const int RENDERDISTANCE = 4;

    // Set GO Objectsdd



    // Start is called before the first frame update
    void Start()
    {
        // SET BLOCKS
        DIRT = new Block(Resources.Load<GameObject>("prefabs/testDirt"));
        GRASS = new Block(Resources.Load<GameObject>("prefabs/Grass"));
        SNOW_GRASS = new Block(Resources.Load<GameObject>("prefabs/SnowGrass"));
        AIR = new Block(true);
        WATER_FULL = new Block(true, Resources.Load<GameObject>("prefabs/waterFull"), true);
        SAND = new Block(Resources.Load<GameObject>("prefabs/Sand"));
        WOOD = new Block(Resources.Load<GameObject>("prefabs/wood"));
        LEAF = new Block(true, Resources.Load<GameObject>("prefabs/leaf"), false);
        //WATER_TOP = new Block(true, Resources.Load<GameObject>("prefabs/waterTop"), true);


        
        for (int i = 0; i < 3200; i++)
        {
            for (int j = 0; j < 3200; j++)
            {

                if(rand.Next(300) == 45){
                    treeMap[i, j] = 1;
                }
                else{
                    treeMap[i, j] = 0;
                }
                if(rand.Next(500) == 45){
                    bushMap[i, j] = 1;
                }
                else{
                    bushMap[i, j] = 0;
                }
                //treeMap[i, j] = (int)Math.Round(fNoise.GetNoise(i, j));
            }
        }

        int currentXCoord = (int)player.transform.position.x;
        int currentZCoord = (int)player.transform.position.z;

        string[] chunkIDData = getChunkID(currentXCoord, currentZCoord);

        string startChunk = chunkIDData[0];

        Chunk.chunksToLoad.Enqueue(startChunk); // Render the chunk directly below the player first
    }

    // Update is called once per frame
    void Update()
    {
        int currentXCoord = (int)player.transform.position.x;
        int currentZCoord = (int)player.transform.position.z;

        string[] chunkIDData = getChunkID(currentXCoord, currentZCoord);



        string xchunk = chunkIDData[1];
        string zchunk = chunkIDData[2];

        string startChunk = chunkIDData[0];
        if (startChunk != currentCenterChunk)
        {
            currentCenterChunk = startChunk;
            StartCoroutine(renderRadius(xchunk, zchunk, RENDERDISTANCE));
        }

        StartCoroutine(Chunk.loadNextChunk());

    }

    public static IEnumerator generateChunkData(int chunkID)
    {
        //UnityEngine.Debug.Log("Generating Chunk Data for:" + chunkID);

        int[] XZRange = Chunk.getCoordRange(insureSixDigits(chunkID));
        int XStart = XZRange[1];
        int ZStart = XZRange[0];

        int XEnd = XZRange[3];
        int ZEnd = XZRange[2];

        int XS = XStart;
        int chunkZPOSITIVE = int.Parse(insureSixDigits(chunkID).Substring(0, 1));
        int chunkXPOSITIVE = int.Parse(insureSixDigits(chunkID).Substring(3, 1));

        // Create a chunk
        Block[,,] tempChumk = new Block[255, 16, 16];
        if (chunkXPOSITIVE == 0 && XS > 0)
        {
            XS = XEnd * (-1);
        }

        for (int x = 0; x < 16; x++)
        {
            int ZS = ZStart;
            if (chunkZPOSITIVE == 0 && ZS > 0)
            {
                ZS = ZEnd * (-1);
            }

            for (int z = 0; z < 16; z++)
            {
                // Noise map effect range of y: 35 - 150
                int deferAir = 0;
                float yTop = (arraytoterraintest.noiseMap[XS + 1600, ZS + 1600] * 115) + 35;
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
                }else if(bushMap[XS + 1600, ZS + 1600] == 1){
                    ytopInt++;
                    tempChumk[ytopInt, x, z] = WOOD; 
                    tempChumk[ytopInt + 1, x, z] = LEAF;
                    deferAir = 1; 
                }else if(treeMap[XS + 1600, ZS + 1600] == 1){
                    ytopInt++;
                    tempChumk[ytopInt, x, z] = WOOD;                    
                    tempChumk[ytopInt + 1, x, z] = WOOD;
                    tempChumk[ytopInt + 2, x, z] = WOOD;
                    tempChumk[ytopInt + 3, x, z] = WOOD;
                    tempChumk[ytopInt + 4, x, z] = LEAF;
                    tempChumk[ytopInt + 5, x, z] = LEAF;
                    deferAir = 5;

                }else{
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
                if((treeMap[XS + 1600, (ZS + 1600) + 1] == 1)){
                    float yTopt = (arraytoterraintest.noiseMap[XS + 1600, (ZS + 1600) + 1] * 115) + 35;
                    int ytopIntt = (int)Math.Floor(yTopt);
                    if(ytopIntt > 72 && ytopIntt < 100){
                        tempChumk[ytopIntt + 3, x, z] = LEAF;
                        tempChumk[ytopIntt + 4, x, z] = LEAF;
                        tempChumk[ytopIntt + 5, x, z] = LEAF;
                    }
                    
                }
                if((treeMap[XS + 1600, (ZS + 1600) - 1] == 1)){
                    float yTopt = (arraytoterraintest.noiseMap[XS + 1600, (ZS + 1600) - 1] * 115) + 35;
                    int ytopIntt = (int)Math.Floor(yTopt);
                    if(ytopIntt > 72 && ytopIntt < 100){
                        tempChumk[ytopIntt + 3, x, z] = LEAF;
                        tempChumk[ytopIntt + 4, x, z] = LEAF;
                        tempChumk[ytopIntt + 5, x, z] = LEAF;
                    }
                }
                if((treeMap[(XS + 1600) + 1, (ZS + 1600)] == 1)){
                    float yTopt = (arraytoterraintest.noiseMap[(XS + 1600) + 1, ZS + 1600] * 115) + 35;
                    int ytopIntt = (int)Math.Floor(yTopt);
                    if(ytopIntt > 72 && ytopIntt < 100){
                        tempChumk[ytopIntt + 3, x, z] = LEAF;
                        tempChumk[ytopIntt + 4, x, z] = LEAF;
                        tempChumk[ytopIntt + 5, x, z] = LEAF;
                    }
                }
                if((treeMap[(XS + 1600) - 1, (ZS + 1600)] == 1)){
                    float yTopt = (arraytoterraintest.noiseMap[(XS + 1600) - 1, ZS + 1600] * 115) + 35;
                    int ytopIntt = (int)Math.Floor(yTopt);
                    if(ytopIntt > 72 && ytopIntt < 100){
                        tempChumk[ytopIntt + 3, x, z] = LEAF;
                        tempChumk[ytopIntt + 4, x, z] = LEAF;
                        tempChumk[ytopIntt + 5, x, z] = LEAF;
                    }
                }
                ZS++;
            }
            XS++;
        }
        Chunk.setChunk(chunkID, tempChumk);
        yield break;
    }

    private string[] getChunkID(int x, int z)
    {
        int currentXCoord = x;
        int currentZCoord = z;
        //UnityEngine.Debug.Log((int) currentXCoord);
        //UnityEngine.Debug.Log((int) currentZCoord);
        string xchunk = "000";
        string zchunk = "000";
        int xchunkI = 0;
        int zchunkI = 0;
        if (currentXCoord >= 0 && currentXCoord <= 15)
        {
            xchunkI = 100;
        }
        else if (currentXCoord <= -1 && currentXCoord >= -16)
        {
            xchunkI = 000;
        }

        if (currentZCoord >= 0 && currentZCoord <= 15)
        {
            zchunkI = 100;
        }
        else if (currentZCoord <= -1 && currentZCoord >= -16)
        {
            zchunkI = 000;
        }

        // Positive Axis

        if (currentZCoord >= 0)
        {
            zchunkI = (currentZCoord / 16) + 100;
        }
        if (currentXCoord >= 0)
        {
            xchunkI = (currentXCoord / 16) + 100;
        }

        // Negative Axis
        if (currentZCoord < 0)
        {
            zchunkI = ((currentZCoord * -1) - 1) / 16;
        }
        if (currentXCoord < 0)
        {
            xchunkI = ((currentXCoord * -1) - 1) / 16;
        }


        xchunk = insureTripleDigit(xchunkI);
        zchunk = insureTripleDigit(zchunkI);

        string startChunk = zchunk + xchunk;

        string[] returnString = new string[3];

        returnString[0] = startChunk;
        returnString[1] = xchunk;
        returnString[2] = zchunk;

        return returnString;

    }

    IEnumerator renderRadius(string xchunk, string zchunk, int radius)
    {



        string firstChunkXAxisToRender = xchunk;
        string firstChunkZAxisToRender = zchunk;

        for (int i = 0; i < radius; i++)
        {
            firstChunkXAxisToRender = incrementChunk(firstChunkXAxisToRender);
            firstChunkZAxisToRender = decrementChunk(firstChunkZAxisToRender);
        }

        string lastChunkXAxisToRender = xchunk;
        string lastChunkZAxisToRender = zchunk;

        for (int i = 0; i < radius; i++)
        {
            lastChunkXAxisToRender = decrementChunk(lastChunkXAxisToRender);
            lastChunkZAxisToRender = incrementChunk(lastChunkZAxisToRender);
        }



        // Make an array of chunks we want rendered.

        string[] chunksWeWantRendered = new string[(radius * 2) * (radius * 2)]; // 32 * 32 render distance max
        string[] chunksWeWantUnrendered = new string[1024];
        int counter = 0;
        for (string xAxis = firstChunkXAxisToRender; xAxis != lastChunkXAxisToRender; xAxis = decrementChunk(xAxis))
        {
            for (string zAxis = firstChunkZAxisToRender; zAxis != lastChunkZAxisToRender; zAxis = incrementChunk(zAxis))
            {
                chunksWeWantRendered[counter] = zAxis + xAxis;
                counter++;
            }
        }

        //UnityEngine.Debug.Log(counter);

        string[] loadedChunkList = new string[Chunk.loadedChunkList.Count];
        Chunk.loadedChunkList.CopyTo(loadedChunkList);

        // Render those chunks

        foreach (string chunk in chunksWeWantRendered)
        {
            bool match = false;
            foreach (string wantedChunk in loadedChunkList)
            {
                if (wantedChunk == chunk)
                {
                    match = true;
                }
            }
            if (!match)
            {
                //UnityEngine.Debug.Log("Rendering Chunk" + chunk);
                //yield return Chunk.loadChunk(chunk);
                if (!Chunk.chunksToLoad.Contains(chunk))
                {
                    Chunk.chunksToLoad.Enqueue(chunk);
                }

            }
        }
        // for(int j = 0; j < counter; j++){

        //     yield return Chunk.loadChunk(chunksWeWantRendered[j]);
        //     //generateChunkData(int.Parse(chunksWeWantRendered[j]));
        // }


        foreach (string chunk in loadedChunkList)
        {
            bool matchFound = false;
            for (int j = 0; j < counter; j++)
            {
                if (chunk == chunksWeWantRendered[j])
                {
                    matchFound = true;
                }
            }
            if (!matchFound)
            {
                //UnityEngine.Debug.Log("UNLOADING CHUNK!");
                yield return Chunk.unloadChunk(chunk);
            }
        }

        // for(int j = 0; j < subCounter; j++){
        //     yield return Chunk.unloadChunk(chunksWeWantUnrendered[j]);
        // }
        yield break;
    }

    public static string insureTripleDigit(int num)
    {
        if (num < 10)
        {
            return "00" + num;
        }
        if (num < 100)
        {
            return "0" + num;
        }

        return num.ToString();
    }

    public static string insureSixDigits(int num)
    {
        if (num < 10)
        {
            return "00000" + num;
        }
        if (num < 100)
        {
            return "0000" + num;
        }
        if (num < 1000)
        {
            return "000" + num;
        }
        if (num < 10000)
        {
            return "00" + num;
        }
        if (num < 100000)
        {
            return "0" + num;
        }
        return num.ToString();

    }

    public static string decrementChunk(string chunk)
    {

        string chunkF = insureTripleDigit(int.Parse(chunk));

        int cPOSITIVE = int.Parse(chunkF.Substring(0, 1));
        int c = int.Parse(chunkF.Substring(1, 2));

        if (c == 0 && cPOSITIVE == 1)
        {
            cPOSITIVE = 0;
        }
        else if (cPOSITIVE == 1)
        {
            c--;
        }
        else if (cPOSITIVE == 0)
        {
            c++;
        }
        else
        {
            UnityEngine.Debug.Log("Decrement Chunk Unexpected Outcome.. cXPOS: " + cPOSITIVE + " c: " + c);
        }

        if (cPOSITIVE == 1)
        {
            return insureTripleDigit(c + 100);
        }
        return insureTripleDigit(c);
    }


    public static string incrementChunk(string chunk)
    {
        string chunkF = insureTripleDigit(int.Parse(chunk));

        int cPOSITIVE = int.Parse(chunkF.Substring(0, 1));
        int c = int.Parse(chunkF.Substring(1, 2));


        if (c == 0 && cPOSITIVE == 0)
        {
            cPOSITIVE = 1;
        }
        else if (cPOSITIVE == 1)
        {
            c++;
        }
        else if (cPOSITIVE == 0)
        {
            c--;
        }
        else
        {
            UnityEngine.Debug.Log("Increment Chunk Unexpected Outcome.. cXPOS: " + cPOSITIVE + " cX: " + c);
        }

        if (cPOSITIVE == 1)
        {
            return insureTripleDigit(c + 100);
        }
        return insureTripleDigit(c);

    }


}


