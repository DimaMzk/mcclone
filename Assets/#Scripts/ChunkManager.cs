using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
[System.Serializable]
public class ChunkManager : MonoBehaviour
{

    public static Chunk[] chunks;
    public static Queue<string> chunksToLoad = new Queue<string>();

    public static List<string> loadedChunkList = new List<string>();

    static object[] gameObjects = new object[99999999]; // Loaded Gameobejcts to Destroy upon unload

    private static int load_chunk_is_running = 0;
    private static int MAX_CHUNK_LOADERS = 2;

    public static Chunk getChunk(int x, int z)
    {
        //Is in memory?
        foreach (Chunk chunk in chunks)
        {
            if (chunk.getIdX() == x && chunk.getIdZ() == z)
            {
                return chunk;
            }
        }

        // Nope! Has it been generated?
        if (!Directory.Exists("Chunks"))
        {
            // Chunk Guarenteed does not exist
            Directory.CreateDirectory("Chunks");
            arraytoterraintest.generateChunkData(x, z);
            return getChunk(x, z);
        }
        else if (!File.Exists("Chunks/" + x.ToString() + "#" + z.ToString() + ".json"))
        {
            // Chunk has never been generated
            Directory.CreateDirectory("Chunks");
            arraytoterraintest.generateChunkData(x, z);
            return getChunk(x, z);
        }
        else
        {

            //Chunk Has Been Generated Before

            //TODO: CALL FOR JSON -> CHUNK CONVERSION
            string jsonData = ""; //TODO: assign jsonData, with data from disk
            return jsonToChunk(jsonData);

            //TODO: RETURN CONVERTED CHUNK}        
        }
    }



    public static void setChunk(Chunk chunk)
    {

        if (!Directory.Exists("Chunks"))
            Directory.CreateDirectory("Chunks");

        BinaryFormatter formatter = new BinaryFormatter();
        int x = chunk.getIdX();
        int z = chunk.getIdZ();
        string filename = x.ToString() + "#" + z.ToString() + ".json";

        FileStream saveFile = File.Create("Chunks/" + filename);

        //formatter.Serialize(saveFile, JsonUtility.ToJson(chunk));

        StreamWriter m_WriterParameter = new StreamWriter(saveFile);
        m_WriterParameter.BaseStream.Seek(0, SeekOrigin.End);
        m_WriterParameter.Write(chunk.ToString());
        UnityEngine.Debug.Log(chunk.ToString());
        m_WriterParameter.Flush();
        m_WriterParameter.Close();

        saveFile.Close();


        // If Chunk is Currently in memory, Update the in memory version

        // TODO: UPDATE IN MEMORY LISTING


    }

    public static Chunk jsonToChunk(string jsonData){
        // Deserialize JSON to object

        // int idX = JSONOBJ.idX;
        // int idZ = JSONOBJ.idZ;
        // bool isloaded = false; //If it's being read from disk, it is probably not loaded lmao
        // int[,,] = JSONOBJ.chunkData;

        

        //
    }

    private static Block blockIdToBlock(int ID){
        switch (ID)
        {
            case 0:
                return arraytoterraintest.AIR;
                break;
            case 1: 
                return arraytoterraintest.DIRT;
                break;
            //TODO: The Rest of These
            default:
        }
    }

    // public static bool isLoaded(string chunkID)
    // {
    //     return loadedChunks[int.Parse(chunkID)];
    // }

    // public static IEnumerator loadNextChunk()
    // {
    //     if (load_chunk_is_running >= MAX_CHUNK_LOADERS) { yield break; }
    //     load_chunk_is_running++;
    //     if (chunksToLoad.Count == 0)
    //     {
    //         load_chunk_is_running--;
    //         yield break;
    //     }
    //     string chunkID = chunksToLoad.Dequeue();
    //     if (int.Parse(chunkID) > 99999999 || int.Parse(chunkID) < 0)
    //     {
    //         load_chunk_is_running--;
    //         yield break;
    //     }
    //     if (getChunk(chunkID) == null)
    //     {
    //         yield return arraytoterraintest.generateChunkData(int.Parse(chunkID));
    //     }
    //     //Make Sure Surrounding Chunks Have Loaded Data
    //     string zChunk = chunkID.Substring(0, 4);
    //     string xchunk = chunkID.Substring(4,4);
    //     if (getChunk(arraytoterraintest.incrementChunk(zChunk) + xchunk) == null)
    //     {
    //         yield return arraytoterraintest.generateChunkData(int.Parse(arraytoterraintest.incrementChunk(zChunk) + xchunk));
    //     }
    //     if (getChunk(arraytoterraintest.decrementChunk(zChunk) + xchunk) == null)
    //     {
    //         yield return arraytoterraintest.generateChunkData(int.Parse(arraytoterraintest.decrementChunk(zChunk) + xchunk));
    //     }
    //     if (getChunk(zChunk + arraytoterraintest.incrementChunk(xchunk)) == null)
    //     {
    //         yield return arraytoterraintest.generateChunkData(int.Parse(zChunk + arraytoterraintest.incrementChunk(xchunk)));
    //     }
    //     if (getChunk(zChunk + arraytoterraintest.decrementChunk(xchunk)) == null)
    //     {
    //         yield return arraytoterraintest.generateChunkData(int.Parse(zChunk + arraytoterraintest.decrementChunk(xchunk)));
    //     }

    //     if (isLoaded(chunkID)) // Don't double load the same chunk, punk!
    //     {
    //         //UnityEngine.Debug.Log("Not Rendering chunk " + chunkID + " beacause it is already loaded...");
    //         load_chunk_is_running--;
    //         yield break;
    //     }

    //     // how chunks work
    //     // 1000 1000 -> Z: 0-15 X: 0-15 .. For every number we add +1 +16 so if we had 1000 1001 -> Z: 0-15 X: 16-31
    //     //  Z   X

    //     // 1000 1000
    //     // ^+- ^+-




    //     // Step one: Get XZ Range from chunkID
    //     int[] XZRange = new int[4];

    //     int chunkZ = int.Parse(chunkID.Substring(1, 3));
    //     int chunkX = int.Parse(chunkID.Substring(5, 3));

    //     int chunkZPOSITIVE = int.Parse(chunkID.Substring(0, 1));
    //     int chunkXPOSITIVE = int.Parse(chunkID.Substring(4, 1));

    //     XZRange = getCoordRange(chunkID);
    //     int XStart = XZRange[1];
    //     int ZStart = XZRange[0];

    //     int XEnd = XZRange[3];
    //     int ZEnd = XZRange[2];

    //     // //Bad way of converting object to array lmao
    //     Block[,,] chunkBuffer = (Block[,,])getChunk(chunkID);

    //     GameObject[,,] loadedGameObjects = new GameObject[255, 16, 16];

    //     for (int y = 0; y < 255; y++)
    //     {
    //         int XS = XStart;
    //         if (chunkXPOSITIVE == 0 && XS > 0)
    //         {
    //             XS = XEnd * (-1);
    //         }

    //         for (int x = 0; x < 16; x++)
    //         {
    //             int ZS = ZStart;
    //             if (chunkZPOSITIVE == 0 && ZS > 0)
    //             {
    //                 ZS = ZEnd * (-1);
    //             }

    //             for (int z = 0; z < 16; z++)
    //             {

    //                 if (blockShouldRender(y, x, z, chunkID, chunkBuffer))
    //                 {

    //                     if (chunkBuffer[y, x, z].getHasGameObject())
    //                     {
    //                         //UnityEngine.Debug.Log("Checking Coord: y:" + y + " x: " + x + " z: " + z + "isTransparent?:" + chunkBuffer[y, x, z].getIsTransparent());
    //                         if(chunkBuffer[y, x, z].getHasCoordModifier()){
    //                             loadedGameObjects[y, x, z] = Instantiate(chunkBuffer[y, x, z].getGameObject(), new Vector3(XS + chunkBuffer[y, x, z].getxModifier(), y + chunkBuffer[y, x, z].getyModifier(), ZS + chunkBuffer[y, x, z].getzModifier()), Quaternion.identity);
    //                         }else{
    //                             loadedGameObjects[y, x, z] = Instantiate(chunkBuffer[y, x, z].getGameObject(), new Vector3(XS, y, ZS), Quaternion.identity);
    //                         }

    //                         //chunkBuffer[y, x, z].getGameObject().transform.position = new Vector3(y, x, z);
    //                     }

    //                 }
    //                 ZS++;
    //             }
    //             XS++;
    //         }
    //     }

    //     //// getChunk(chunkID); //TODO: WHAT DOES THIS DO? AT WHAT POINT OF INSANITY DID I ADD THIS?

    //     loadedChunks[int.Parse(chunkID)] = true;
    //     gameObjects[int.Parse(chunkID)] = loadedGameObjects;
    //     loadedChunkList.Add(chunkID);
    //     load_chunk_is_running--;
    //     yield break;

    // }


    // public static IEnumerator unloadChunk(string chunkID)
    // {
    //     if (chunkID == null)
    //     {
    //         yield break;
    //     }
    //     if (!isLoaded(chunkID)) // Don't double load the same chunk, punk!
    //     {
    //         //UnityEngine.Debug.Log("Not unloading chunk " + chunkID + " beacause it is already unloaded...");
    //         yield break;
    //     }

    //     GameObject[,,] loadedGameObjects = (GameObject[,,])gameObjects[int.Parse(chunkID)];

    //     for (int y = 0; y < 255; y++)
    //     {

    //         for (int x = 0; x < 16; x++)
    //         {


    //             for (int z = 0; z < 16; z++)
    //             {

    //                 if (loadedGameObjects[y, x, z] != null)
    //                 {
    //                     Destroy(loadedGameObjects[y, x, z]);
    //                 }

    //             }

    //         }
    //     }
    //     loadedChunks[int.Parse(chunkID)] = false;
    //     gameObjects[int.Parse(chunkID)] = null;
    //     loadedChunkList.Remove(chunkID);
    //     yield break;
    // }



    // public static int[] getCoordRange(string chunkID)
    // {

    //     int chunkZ = int.Parse(chunkID.Substring(1, 3));
    //     int chunkX = int.Parse(chunkID.Substring(5, 3));

    //     int chunkZPOSITIVE = int.Parse(chunkID.Substring(0, 1));
    //     int chunkXPOSITIVE = int.Parse(chunkID.Substring(4, 1));

    //     int XStart = 0;
    //     int XEnd = 0;
    //     int ZStart = 0;
    //     int ZEnd = 0;

    //     // Edge cases for 0th chunks
    //     if (chunkX == 0)
    //     {
    //         if (chunkXPOSITIVE == 1)
    //         {
    //             XStart = 0;
    //             XEnd = 15;
    //         }
    //         else
    //         {
    //             XStart = 1;
    //             XEnd = 16;
    //         }

    //     }

    //     if (chunkZ == 0)
    //     {
    //         if (chunkZPOSITIVE == 1)
    //         {
    //             ZStart = 0;
    //             ZEnd = 15;
    //         }
    //         else
    //         {
    //             ZStart = 1;
    //             ZEnd = 16;
    //         }

    //     }

    //     // If Positive Axis
    //     if (chunkZPOSITIVE == 1 && chunkZ != 0)
    //     {

    //         ZStart = (chunkZ * 16);
    //         ZEnd = (chunkZ * 16) + 15;
    //     }
    //     if (chunkXPOSITIVE == 1 && chunkX != 0)
    //     {
    //         XStart = (chunkX * 16);
    //         XEnd = (chunkX * 16) + 15;
    //     }

    //     // If Negative Axis
    //     if (chunkZPOSITIVE == 0 && chunkZ != 0)
    //     {

    //         ZStart = (chunkZ * 16) + 1;
    //         ZEnd = (chunkZ * 16) + 16;
    //     }
    //     if (chunkXPOSITIVE == 0 && chunkX != 0)
    //     {
    //         XStart = (chunkX * 16) + 1;
    //         XEnd = (chunkX * 16) + 16;
    //     }

    //     int[] returnArr = new int[4];
    //     returnArr[0] = ZStart;
    //     returnArr[1] = XStart;
    //     returnArr[2] = ZEnd;
    //     returnArr[3] = XEnd;
    //     return returnArr;

    // }


    // private static bool blockShouldRender(int y, int x, int z, string chunkID, Block[,,] chunkBuffer)
    // {
    //     int chunkZ = int.Parse(chunkID.Substring(1, 3));
    //     int chunkX = int.Parse(chunkID.Substring(5, 3));
    //     int chunkZPOSITIVE = int.Parse(chunkID.Substring(0, 1));
    //     int chunkXPOSITIVE = int.Parse(chunkID.Substring(4, 1));

    //     string chunkXSTR = chunkID.Substring(4, 4);
    //     string chunkZSTR = chunkID.Substring(0, 4);

    //     bool doRender = false;

    //     if (y == 0 && (chunkBuffer[y + 1, x, z].getIsTransparent())) { doRender = true; }
    //     //Access Neighbouring Chunks [X AXIS]
    //     if (x == 0)
    //     {
    //         // need to decrement chunkX in a temporary variable. in it's 0 -> flip chunkXPOSITIVE

    //         string tempChunk = chunkZSTR + arraytoterraintest.decrementChunk(chunkXSTR);
    //         int tempChunkI = int.Parse(tempChunk);
    //         Block[,,] tempChunkArr = (Block[,,])getChunk(tempChunkI.ToString());

    //         if (tempChunkArr != null) { if (tempChunkArr[y, 15, z].getIsTransparent()) { doRender = true; } }
    //     }

    //     if (x == 15)
    //     {


    //         string tempChunk = chunkZSTR + arraytoterraintest.incrementChunk(chunkXSTR);
    //         int tempChunkI = int.Parse(tempChunk);
    //         Block[,,] tempChunkArr = (Block[,,])getChunk(tempChunkI.ToString());

    //         if (tempChunkArr != null) { if (tempChunkArr[y, 0, z].getIsTransparent()) { doRender = true; } }
    //     }
    //     if (z == 0) // Want to decrement zchunk
    //     {


    //         string tempChunk = arraytoterraintest.decrementChunk(chunkZSTR) + chunkXSTR;
    //         int tempChunkI = int.Parse(tempChunk);
    //         Block[,,] tempChunkArr = (Block[,,])getChunk(tempChunkI.ToString());

    //         if (tempChunkArr != null) { if (tempChunkArr[y, x, 15].getIsTransparent()) { doRender = true; } }
    //     }
    //     if (z == 15)
    //     {

    //         string tempChunk = arraytoterraintest.incrementChunk(chunkZSTR) + chunkXSTR;
    //         int tempChunkI = int.Parse(tempChunk);
    //         Block[,,] tempChunkArr = (Block[,,])getChunk(tempChunkI.ToString());

    //         if (tempChunkArr != null) { if (tempChunkArr[y, x, 0].getIsTransparent()) { doRender = true; } }
    //     }

    //     if (y > 0 && y < 254)
    //     {   
    //         if (chunkBuffer[y + 1, x, z].getIsTransparent() || chunkBuffer[y - 1, x, z].getIsTransparent()) { doRender = true; }
    //     }
    //     if (x > 0)
    //     {
    //         if (chunkBuffer[y, x - 1, z].getIsTransparent()) { doRender = true; }
    //     }
    //     if (x < 15)
    //     {
    //         if (chunkBuffer[y, x + 1, z].getIsTransparent()) { doRender = true; }
    //     }
    //     if (z > 0)
    //     {
    //         if (chunkBuffer[y, x, z - 1].getIsTransparent()) { doRender = true; }
    //     }
    //     if (z < 15)
    //     {
    //         if (chunkBuffer[y, x, z + 1].getIsTransparent()) { doRender = true; }
    //     }


    //     //SHOULD WATER BLOCK RENDER?
    //     if (chunkBuffer[y, x, z].getIsLiquid())
    //     {
    //         doRender = false;
    //         if (y > 0 && y < 254)
    //         {
    //             if (chunkBuffer[y + 1, x, z].getIsTransparent() && !chunkBuffer[y + 1, x, z].getIsLiquid()) { doRender = true; }
    //         }
    //         if (x > 0)
    //         {
    //             if (chunkBuffer[y, x - 1, z].getIsTransparent() && !chunkBuffer[y, x - 1, z].getIsLiquid()) { doRender = true; }

    //         }
    //         if (x < 15)
    //         {
    //             if (chunkBuffer[y, x + 1, z].getIsTransparent() && !chunkBuffer[y, x + 1, z].getIsLiquid()) { doRender = true; }
    //         }
    //         if (z > 0)
    //         {
    //             if (chunkBuffer[y, x, z - 1].getIsTransparent() && !chunkBuffer[y, x, z - 1].getIsLiquid()) { doRender = true; }
    //         }
    //         if (z < 15)
    //         {
    //             if (chunkBuffer[y, x, z + 1].getIsTransparent() && !chunkBuffer[y, x, z + 1].getIsLiquid()) { doRender = true; }
    //         }
    //     }



    //     return doRender;
    // }



    // // Start is called before the first frame update
    // void Start()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }
}
