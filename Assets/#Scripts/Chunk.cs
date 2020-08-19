﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    static object[] chunks = new object[999999];
    static bool[] loadedChunks = new bool[999999]; // Hard limit of all the chunks can be loaded, thought i would not recomment it

    public static List<string> loadedChunkList = new List<string>();

    static object[] gameObjects = new object[999999]; // Loaded Gameobejcts to Destroy upon unload

    public static object getChunk(string chunkID)
    {
        return chunks[int.Parse(chunkID)];
    }

    public static void setChunk(int chunkID, object chunkData)
    {
        chunks[chunkID] = chunkData;
    }

    public static bool isLoaded(string chunkID)
    {
        return loadedChunks[int.Parse(chunkID)];
    }

    public static IEnumerator loadChunk(string chunkID)
    {
        if(getChunk(chunkID) == null)
        {
            yield break;
        }
        if (isLoaded(chunkID)) // Don't double load the same chunk, punk!
        {
            //UnityEngine.Debug.Log("Not Rendering chunk " + chunkID + " beacause it is already loaded...");
            yield break;
        }
        
        // how chunks work
        // 100 100 -> Z: 0-15 X: 0-15 .. For every number we add +1 +16 so if we had 100 101 -> Z: 0-15 X: 16-31
        //  Z   X

        // 100 100
        // ^+- ^+-


        // Step one: Get XZ Range from chunkID
        int[] XZRange = new int[4];

        int chunkZ = int.Parse(chunkID.Substring(1, 2));
        int chunkX = int.Parse(chunkID.Substring(4, 2));

        int chunkZPOSITIVE = int.Parse(chunkID.Substring(0, 1));
        int chunkXPOSITIVE = int.Parse(chunkID.Substring(3, 1));

        XZRange = getCoordRange(chunkID);
        int XStart = XZRange[1];
        int ZStart = XZRange[0];

        int XEnd = XZRange[3];
        int ZEnd = XZRange[2];

        //Bad way of converting object to array lmao
        Block[,,] chunkBuffer = (Block[,,])chunks[int.Parse(chunkID)];

        GameObject[,,] loadedGameObjects = new GameObject[255,16,16];

        for (int y = 0; y < 255; y++)
        {
            int XS = XStart;
            if(chunkXPOSITIVE == 0 && XS > 0)
            {
                XS = XEnd * (-1);
            }

            for (int x = 0; x < 16; x++)
            {
                int ZS = ZStart;
                if(chunkZPOSITIVE == 0 && ZS > 0)
                {
                    ZS = ZEnd * (-1);
                }

                for (int z = 0; z < 16; z++)
                {

                    if(blockShouldRender(y, x, z, chunkID, chunkBuffer))
                    {
                        //UnityEngine.Debug.Log("Checking Coord: y:" + y + " x: " + x + " z: " + z + "isTransparent?:" + chunkBuffer[y, x, z].getIsTransparent());
                        if(chunkBuffer[y, x, z].getHasGameObject())
                        {
                            loadedGameObjects[y,x,z] = Instantiate(chunkBuffer[y, x, z].getGameObject(), new Vector3(XS, y, ZS), Quaternion.identity);
                        }
                        
                    }
                    ZS++;
                }
                XS++;
            }
        }

        loadedChunks[int.Parse(chunkID)] = true;
        gameObjects[int.Parse(chunkID)] = loadedGameObjects;
        loadedChunkList.Add(chunkID);
        yield break;

    }

    public static IEnumerator unloadChunk(string chunkID)
    {
        if(chunkID == null){
            yield break;
        }
        if (!isLoaded(chunkID)) // Don't double load the same chunk, punk!
        {
            UnityEngine.Debug.Log("Not unloading chunk " + chunkID + " beacause it is already unloaded...");
            yield break;
        }

        GameObject[,,] loadedGameObjects = (GameObject[,,]) gameObjects[int.Parse(chunkID)];

        for (int y = 0; y < 255; y++)
        {
            
            for (int x = 0; x < 16; x++)
            {
                

                for (int z = 0; z < 16; z++)
                {

                    if(loadedGameObjects[y,x,z] != null){
                        Destroy(loadedGameObjects[y,x,z]);
                    }
               
                }
           
            }
        }
        loadedChunks[int.Parse(chunkID)] = false;
        gameObjects[int.Parse(chunkID)] = null;
        loadedChunkList.Remove(chunkID);
        yield break;
    }



    private static int[] getCoordRange(string chunkID)
    {

        int chunkZ = int.Parse(chunkID.Substring(1, 2));
        int chunkX = int.Parse(chunkID.Substring(4, 2));

        int chunkZPOSITIVE = int.Parse(chunkID.Substring(0, 1));
        int chunkXPOSITIVE = int.Parse(chunkID.Substring(3, 1));

        int XStart = 0;
        int XEnd = 0;
        int ZStart = 0;
        int ZEnd = 0;

        // Edge cases for 0th chunks
        if (chunkX == 0)
        {
            if (chunkXPOSITIVE == 1)
            {
                XStart = 0;
                XEnd = 15;
            }
            else
            {
                XStart = 1;
                XEnd = 16;
            }

        }

        if (chunkZ == 0)
        {
            if (chunkZPOSITIVE == 1)
            {
                ZStart = 0;
                ZEnd = 15;
            }
            else
            {
                ZStart = 1;
                ZEnd = 16;
            }

        }

        // If Positive Axis
        if (chunkZPOSITIVE == 1 && chunkZ != 0)
        {

            ZStart = (chunkZ * 16);
            ZEnd = (chunkZ * 16) + 15;
        }
        if (chunkXPOSITIVE == 1 && chunkX != 0)
        {
            XStart = (chunkX * 16);
            XEnd = (chunkX * 16) + 15;
        }

        // If Negative Axis
        if (chunkZPOSITIVE == 0 && chunkZ != 0)
        {

            ZStart = (chunkZ * 16) + 1;
            ZEnd = (chunkZ * 16) + 16;
        }
        if (chunkXPOSITIVE == 0 && chunkX != 0)
        {
            XStart = (chunkX * 16) + 1;
            XEnd = (chunkX * 16) + 16;
        }

        int[] returnArr = new int[4];
        returnArr[0] = ZStart;
        returnArr[1] = XStart;
        returnArr[2] = ZEnd;
        returnArr[3] = XEnd;
        return returnArr;

    }


    private static bool blockShouldRender(int y, int x, int z, string chunkID, Block[,,] chunkBuffer)
    {
        int chunkZ = int.Parse(chunkID.Substring(1, 2));
        int chunkX = int.Parse(chunkID.Substring(4, 2));

        int chunkZPOSITIVE = int.Parse(chunkID.Substring(0, 1));
        int chunkXPOSITIVE = int.Parse(chunkID.Substring(3, 1));

        bool doRender = false;

        //print("X: " + x + " Y: " + y + " Z: " + z);
        //if (((x == 15 || x == 0) || (z == 15 || z == 0) || (y == 254 || y == 0)) || (chunkBuffer[y - 1, x, z] == 0 || chunkBuffer[y - 1, x - 1, z] == 0 || chunkBuffer[y - 1, x + 1, z] == 0 || chunkBuffer[y - 1, x - 1, z - 1] == 0 || chunkBuffer[y - 1, x - 1, z + 1] == 0 || chunkBuffer[y - 1, x + 1, z - 1] == 0 || chunkBuffer[y - 1, x + 1, z + 1] == 0 || chunkBuffer[y - 1, x, z + 1] == 0 || chunkBuffer[y - 1, x, z - 1] == 0 ||            chunkBuffer[y + 1, x, z] == 0 || chunkBuffer[y + 1, x - 1, z] == 0 || chunkBuffer[y + 1, x + 1, z] == 0 || chunkBuffer[y + 1, x - 1, z - 1] == 0 || chunkBuffer[y + 1, x - 1, z + 1] == 0 || chunkBuffer[y + 1, x + 1, z - 1] == 0 || chunkBuffer[y + 1, x + 1, z + 1] == 0 || chunkBuffer[y + 1, x, z + 1] == 0 || chunkBuffer[y + 1, x, z - 1] == 0 ||                                    chunkBuffer[y, x, z] == 0 || chunkBuffer[y, x - 1, z] == 0 || chunkBuffer[y, x + 1, z] == 0 || chunkBuffer[y, x - 1, z - 1] == 0 || chunkBuffer[y, x - 1, z + 1] == 0 || chunkBuffer[y, x + 1, z - 1] == 0 || chunkBuffer[y, x + 1, z + 1] == 0 || chunkBuffer[y, x, z - 1] == 0 || chunkBuffer[y, x, z + 1] == 0))
        //only render block if attached to air
        //if bottom block has air beside or above
        if (y == 0 && (chunkBuffer[y + 1, x, z].getIsTransparent())) { doRender = true; }
        //Access Neighbouring Chunks [X AXIS]
        if (x == 0)
        {
            // need to decrement chunkX in a temporary variable. in it's 0 -> flip chunkXPOSITIVE
            int cX = chunkX;
            int cXPOS = chunkXPOSITIVE;

            if (cX == 0 && cXPOS == 1)
            {
                cXPOS = 0;
            }
            else if (cXPOS == 1)
            {
                cX--;
            }
            else if (cXPOS == 0)
            {
                cX++;
            }
            else
            {
                UnityEngine.Debug.Log("Decrement Chunk Unexpected Outcome.. cXPOS: " + cXPOS + " cX: " + cX);
            }

            string cZstr = "";
            if (chunkZ < 10)
            {
                cZstr = "0" + chunkZ.ToString();
            }
            else
            {
                cZstr = chunkZ.ToString();
            }

            string cXstr = "";
            if (cX < 10)
            {
                cXstr = "0" + cX.ToString();
            }
            else
            {
                cXstr = cX.ToString();
            }

            string tempChunk = chunkZPOSITIVE.ToString() + cZstr + cXPOS.ToString() + cXstr;
            //UnityEngine.Debug.Log("x is 0: " + tempChunk);
            int tempChunkI = int.Parse(tempChunk);



            Block[,,] tempChunkArr = (Block[,,])chunks[tempChunkI];


            //if (tempChunkArr == null || isTransparent(tempChunkArr[y, 15, z])) { doRender = true; }
            if (tempChunkArr != null) { if (tempChunkArr[y, 15, z].getIsTransparent()) { doRender = true; } }



        }

        if (x == 15)
        {
            // need to increment chunkX in a temp variable
            int cX = chunkX;
            int cXPOS = chunkXPOSITIVE;

            if (cX == 0 && cXPOS == 0)
            {
                cXPOS = 1;
            }
            else if (cXPOS == 1)
            {
                cX++;
            }
            else if (cXPOS == 0)
            {
                cX--;
            }
            else
            {
                UnityEngine.Debug.Log("Increment Chunk Unexpected Outcome.. cXPOS: " + cXPOS + " cX: " + cX);
            }


            string cZstr = "";
            if (chunkZ < 10)
            {
                cZstr = "0" + chunkZ.ToString();
            }
            else
            {
                cZstr = chunkZ.ToString();
            }

            string cXstr = "";
            if (cX < 10)
            {
                cXstr = "0" + cX.ToString();
            }
            else
            {
                cXstr = cX.ToString();
            }

            string tempChunk = chunkZPOSITIVE.ToString() + cZstr + cXPOS.ToString() + cXstr;
            //UnityEngine.Debug.Log("x is  15: " + tempChunk);
            int tempChunkI = int.Parse(tempChunk);


            Block[,,] tempChunkArr = (Block[,,])chunks[tempChunkI];



            //if (tempChunkArr == null || isTransparent(tempChunkArr[y, 0, z])) { doRender = true; }
            if (tempChunkArr != null) { if (tempChunkArr[y, 0, z].getIsTransparent()) { doRender = true; } }



        }

        if (z == 0) // Want to decrement zchunk
        {
            int cZ = chunkZ;
            int cZPOS = chunkZPOSITIVE;

            if (cZ == 0 && cZPOS == 1)
            {
                cZPOS = 0;
            }
            else if (cZPOS == 1)
            {
                cZ--;
            }
            else if (cZPOS == 0)
            {
                cZ++;
            }
            else
            {
                UnityEngine.Debug.Log("Decrement Chunk Unexpected Outcome.. cZPOS: " + cZPOS + " cZ: " + cZ);
            }

            string cZstr = "";
            if (cZ < 10)
            {
                cZstr = "0" + cZ.ToString();
            }
            else
            {
                cZstr = cZ.ToString();
            }

            string cXstr = "";
            if (chunkX < 10)
            {
                cXstr = "0" + chunkX.ToString();
            }
            else
            {
                cXstr = chunkX.ToString();
            }

            string tempChunk = cZPOS.ToString() + cZstr + chunkXPOSITIVE.ToString() + cXstr;
            //UnityEngine.Debug.Log("z is 0: " + tempChunk);
            int tempChunkI = int.Parse(tempChunk);



            Block[,,] tempChunkArr = (Block[,,])chunks[tempChunkI];
            //if (tempChunkArr == null || isTransparent(tempChunkArr[y, x, 15])) { doRender = true; }
            if (tempChunkArr != null) { if (tempChunkArr[y, x, 15].getIsTransparent()) { doRender = true; } }
        }
        if (z == 15)
        {
            // need to increment chunkX in a temp variable
            int cZ = chunkZ;
            int cZPOS = chunkZPOSITIVE;

            if (cZ == 0 && cZPOS == 0)
            {
                cZPOS = 1;
            }
            else if (cZPOS == 1)
            {
                cZ++;
            }
            else if (cZPOS == 0)
            {
                cZ--;
            }
            else
            {
                UnityEngine.Debug.Log("Increment Chunk Unexpected Outcome.. cZPOS: " + cZPOS + " cZ: " + cZ);
            }


            string cZstr = "";
            if (cZ < 10)
            {
                cZstr = "0" + cZ.ToString();
            }
            else
            {
                cZstr = cZ.ToString();
            }

            string cXstr = "";
            if (chunkX < 10)
            {
                cXstr = "0" + chunkX.ToString();
            }
            else
            {
                cXstr = chunkX.ToString();
            }

            string tempChunk = cZPOS.ToString() + cZstr + chunkXPOSITIVE.ToString() + cXstr;
            //UnityEngine.Debug.Log("z is  15: " + tempChunk);
            int tempChunkI = int.Parse(tempChunk);


            Block[,,] tempChunkArr = (Block[,,])chunks[tempChunkI];

            //if (tempChunkArr == null || isTransparent(tempChunkArr[y, x, 0])) { doRender = true; }
            if (tempChunkArr != null) { if (tempChunkArr[y, x, 0].getIsTransparent()) { doRender = true; } }
        }

        //UNCOMMENT TO FORCE RENDER ALL BLOCKS.. NOT RECOMENEDED
        //doRender = true;

        if (y > 0 && y < 254)
        {
            if (chunkBuffer[y + 1, x, z].getIsTransparent() || chunkBuffer[y - 1, x, z].getIsTransparent()) { doRender = true; }
        }

        if (x > 0 && x < 15 && z > 0 && z < 15)
        {
            if (chunkBuffer[y, x + 1, z].getIsTransparent()) { doRender = true; }
            if (chunkBuffer[y, x - 1, z].getIsTransparent()) { doRender = true; }
            if (chunkBuffer[y, x, z + 1].getIsTransparent()) { doRender = true; }
            if (chunkBuffer[y, x, z - 1].getIsTransparent()) { doRender = true; }
        }

        return doRender;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
