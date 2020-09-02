using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
[System.Serializable]
public class ChunkManager : MonoBehaviour
{

    public static List<Chunk> chunks = new List<Chunk>();
    public static Queue<Chunk> chunksToLoad = new Queue<Chunk>();
    public static Queue<Chunk> chunksToUnload = new Queue<Chunk>();
    private static int load_chunk_is_running = 0;
    private static int unload_chunk_is_running = 0;
    private static int MAX_CHUNK_LOADERS = 1;
    private static int MAX_CHUNK_UNLOADERS = 1;
    private static bool DEBUG = false;


    public static Chunk getChunk(int x, int z)
    {
        //Is in memory?
        if (chunks.Count != 0) 
        {
            List<Chunk> chunksCopy = new List<Chunk>(chunks);
            foreach (Chunk chunk in chunksCopy)
            {

                if (chunk != null && chunk.getIdX() == x && chunk.getIdZ() == z)
                {   

                    if(DEBUG){
                        //Uncomment line below only if you really need this information, A single chunk gets called several thousand times per chunk load.
                        //UnityEngine.Debug.Log("Returned Chunk " + x.ToString() +"#"+ z.ToString() + " From Memory"); 
                    }

                    return chunk;
                }
            }
        }


        // Nope! Has it been generated?
        if (!Directory.Exists("Chunks"))
        {
            // Chunk Guarenteed does not exist
            Directory.CreateDirectory("Chunks");

            if(DEBUG){
                //Uncomment line below only if you really need this information, A single chunk gets called several thousand times per chunk load.
                //UnityEngine.Debug.Log("Returned Chunk " + x.ToString() +"#"+ z.ToString() + " From generateChunkData");
            }

            return arraytoterraintest.generateChunkData(x, z);
        }

        else if (!File.Exists("Chunks/" + x.ToString() + "#" + z.ToString() + ".json"))
        {
            // Chunk has never been generated
            Directory.CreateDirectory("Chunks");

            if(DEBUG){
                //Uncomment line below only if you really need this information, A single chunk gets called several thousand times per chunk load.
               // UnityEngine.Debug.Log("Returned Chunk " + x.ToString() +"#"+ z.ToString() + " From generateChunkData");
            }

            return arraytoterraintest.generateChunkData(x, z);
        }

        else
        {
            //Chunk Has Been Generated Before
            FileStream file;

            if (File.Exists("Chunks/" + x.ToString() + "#" + z.ToString() + ".json"))
                file = File.OpenRead("Chunks/" + x.ToString() + "#" + z.ToString() + ".json");
            else
                throw new Exception("File that existed a second ago, no longer exists. This is a real bruh moment.");

            BinaryFormatter bf = new BinaryFormatter();

            string data = (string)bf.Deserialize(file);
            file.Close();

            if(DEBUG){
                //Uncomment line below only if you really need this information, A single chunk gets called several thousand times per chunk load.
                //UnityEngine.Debug.Log("Returned Chunk " + x.ToString() +"#"+ z.ToString() + " From Disk");
            }

            return jsonToChunk(data);
        }
    }


    public static void setChunk(Chunk chunk)
    {

        if (!Directory.Exists("Chunks"))
            Directory.CreateDirectory("Chunks");

        int x = chunk.getIdX();
        int z = chunk.getIdZ();
        string filename = x.ToString() + "#" + z.ToString() + ".json";

        string destination = ("Chunks/" + filename);
        if (DEBUG)
        {
            // Uncomment Line Below Only If You Really Need This Information, Is Outputs A Lot, And Gives The Console A Pretty Bad Time :)
            //UnityEngine.Debug.Log(chunk.ToJSON());
        }
        FileStream file;

        if (File.Exists(destination))
            file = File.OpenWrite(destination);
        else
            file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, chunk.ToJSON());
        file.Close();
    }


    public static Chunk jsonToChunk(string jsonData)
    {
        JsonChunk myDeserializedClass = JsonConvert.DeserializeObject<JsonChunk>(jsonData);
        return myDeserializedClass.toChunk();
    }


    public static IEnumerator loadNextChunk()
    {
        if (load_chunk_is_running >= MAX_CHUNK_LOADERS) yield break; 
        load_chunk_is_running++;

        if (chunksToLoad.Count == 0)
        {
            load_chunk_is_running--;
            yield break;
        }

        Chunk toLoad = chunksToLoad.Dequeue();

        if (toLoad == null)
            throw new Exception("Invalid Chunk. Chunk was null");

        if (toLoad.getIsLoaded()) // Don't double load the same chunk, punk!
        {
            load_chunk_is_running--;
            yield break;
        }

        //Make Sure Surrounding Chunks Have Loaded Data are in memory
        loadIntoMemory(toLoad.getIdX(), toLoad.getIdZ() + 1);
        loadIntoMemory(toLoad.getIdX(), toLoad.getIdZ() - 1);
        loadIntoMemory(toLoad.getIdX() + 1, toLoad.getIdZ());
        loadIntoMemory(toLoad.getIdX() - 1, toLoad.getIdZ());

        int[] XZRange = getCoordRange(toLoad.getIdX(), toLoad.getIdZ());
        int XStart = XZRange[1];
        int ZStart = XZRange[0];

        int XEnd = XZRange[3];
        int ZEnd = XZRange[2];

        Block[,,] chunkBuffer = toLoad.getChunk();
        GameObject[,,] loadedGameObjects = new GameObject[255, 16, 16];

        for (int y = 0; y < 255; y++)
        {
            int XS = XStart;

            for (int x = 0; x < 16; x++)
            {
                int ZS = ZStart;

                for (int z = 0; z < 16; z++)
                {

                    if (blockShouldRender(y, x, z, toLoad.getIdX(), toLoad.getIdZ(), chunkBuffer))
                    {

                        if (chunkBuffer[y, x, z].getHasGameObject())
                        {
                            if (chunkBuffer[y, x, z].getHasCoordModifier())
                                loadedGameObjects[y, x, z] = Instantiate(chunkBuffer[y, x, z].getGameObject(), new Vector3(XS + chunkBuffer[y, x, z].getxModifier(), y + chunkBuffer[y, x, z].getyModifier(), ZS + chunkBuffer[y, x, z].getzModifier()), Quaternion.identity);
                            else
                                loadedGameObjects[y, x, z] = Instantiate(chunkBuffer[y, x, z].getGameObject(), new Vector3(XS, y, ZS), Quaternion.identity);
                        }
                    }
                    ZS++;
                }
                XS++;
            }
        }


        toLoad.setGameObjects(loadedGameObjects);
        toLoad.setIsLoaded(true);
        setChunkMem(toLoad);
        load_chunk_is_running--;
        if(DEBUG) UnityEngine.Debug.Log("Loaded Chunk: " + toLoad.ToString());
        yield break;
    }


    public static IEnumerator unloadChunk()
    {
        if (unload_chunk_is_running >= MAX_CHUNK_UNLOADERS) yield break;

        unload_chunk_is_running++;

        if (chunksToUnload.Count == 0)
        {
            unload_chunk_is_running--;
            yield break;
        }

        Chunk chunk = chunksToUnload.Dequeue();
        GameObject[,,] loadedGameObjects = chunk.GetGameObjects();

        if (!chunk.getIsLoaded()) // Don't double unload load the same chunk, punk!
        {
            unload_chunk_is_running--;
            yield break;
        }
        
        if(loadedGameObjects != null){

            for (int y = 0; y < 255; y++)
            {

                for (int x = 0; x < 16; x++)
                {


                    for (int z = 0; z < 16; z++)
                    {

                        if (loadedGameObjects[y, x, z] != null)
                            Destroy(loadedGameObjects[y, x, z]);
                        

                    }

                }
            }
        }
        
        chunk.setIsLoaded(false);
        chunk.setGameObjects(null);
        setChunkMem(chunk);
        chunks.Remove(chunk);
        setChunk(chunk);
        unload_chunk_is_running--;

        if(DEBUG)
            UnityEngine.Debug.Log("Unloaded chunk: " + chunk.ToString());
        yield break;
    }


    public static void setChunkMem(Chunk chunk)
    {   
        if(DEBUG)
            UnityEngine.Debug.Log("Received Request to Update: "  + chunk.ToString()); 
        bool updated = false;
        List<Chunk> chunksCopy = new List<Chunk>(chunks);

        foreach (Chunk chunkk in chunksCopy)
        {

            if (chunkk.getIdX() == chunk.getIdX() && chunkk.getIdZ() == chunk.getIdZ())
            {   
                chunks.Remove(chunkk);
                chunkk.setChunk(chunk.getChunk());
                chunkk.setIsLoaded(chunk.getIsLoaded());
                chunkk.setGameObjects(chunk.GetGameObjects());
                chunks.Add(chunkk);
                if(DEBUG)
                    UnityEngine.Debug.Log("Updated Chunk in Memory" + chunkk.ToString());
                updated = true;
            }
        }

        if(!updated){
            if(DEBUG) UnityEngine.Debug.Log(chunk.ToString() + "Did not appear to be in memory :/ Adding to memory. Double Checking, Then Adding");
            
            if(!chunks.Contains(chunk)){
                chunks.Add(chunk);
                if(DEBUG) UnityEngine.Debug.Log(chunk.ToString() + " Has been added to memory");
            }
            else{
                chunks.Remove(chunk);
                chunks.Add(chunk);
                if(DEBUG) UnityEngine.Debug.Log("Was in chunks, updated " + chunks.ToString());
            }
        }
    }


    public static void loadIntoMemory(int idx, int idz)
    {
        Chunk chunk = getChunk(idx, idz);
        bool found = false;
        List<Chunk> chunksCopy = new List<Chunk>(chunks);

        foreach (Chunk chunkk in chunksCopy)
        {
            if (chunkk.getIdX() == chunk.getIdX() && chunkk.getIdZ() == chunk.getIdZ())
            {
                found = true;
            }
        }

        if (!found)
        {
            chunks.Add(chunk);
        }
    }


    public static int[] getCoordRange(int chunkIDX, int chunkIDZ)
    {
        int XStart = 0;
        int XEnd = 0;
        int ZStart = 0;
        int ZEnd = 0;

        // Edge cases for 0th chunks
        if (chunkIDX == 0)
        {
            XStart = 0;
            XEnd = 15;
        }

        if (chunkIDZ == 0)
        {
            ZStart = 0;
            ZEnd = 15;
        }

        // If Positive Axis
        if (chunkIDZ > 0)
        {
            ZStart = (chunkIDZ * 16);
            ZEnd = (chunkIDZ * 16) + 15;
        }

        if (chunkIDX > 0)
        {
            XStart = (chunkIDX * 16);
            XEnd = (chunkIDX * 16) + 15;
        }

        // If Negative Axis
        if (chunkIDZ < 0)
        {
            ZStart = (chunkIDZ * 16);
            ZEnd = (chunkIDZ * 16) - 16;
        }

        if (chunkIDX < 0)
        {
            XStart = (chunkIDX * 16);
            XEnd = (chunkIDX * 16) - 16;
        }

        int[] returnArr = new int[4];
        returnArr[0] = ZStart;
        returnArr[1] = XStart;
        returnArr[2] = ZEnd;
        returnArr[3] = XEnd;
        return returnArr;
    }


    private static bool blockShouldRender(int y, int x, int z, int IDX, int IDZ, Block[,,] chunkBuffer)
    {
        bool doRender = false;

        if (y == 0 && (chunkBuffer[y + 1, x, z].getIsTransparent())) { doRender = true; }
        //Access Neighbouring Chunks [X AXIS]
        if (x == 0)
        {
            Block[,,] tempChunkArr = getChunk(IDX - 1, IDZ).getChunk();
            if (tempChunkArr != null) { if (tempChunkArr[y, 15, z].getIsTransparent()) { doRender = true; } }
        }

        if (x == 15)
        {
            Block[,,] tempChunkArr = getChunk(IDX + 1, IDZ).getChunk();
            if (tempChunkArr != null) { if (tempChunkArr[y, 0, z].getIsTransparent()) { doRender = true; } }
        }
        //Access Neighbouring Chunks [Z AXIS]
        if (z == 0) // Want to decrement zchunk
        {
            Block[,,] tempChunkArr = getChunk(IDX, IDZ - 1).getChunk();
            if (tempChunkArr != null) { if (tempChunkArr[y, x, 15].getIsTransparent()) { doRender = true; } }
        }
        if (z == 15)
        {
            Block[,,] tempChunkArr = getChunk(IDX, IDZ + 1).getChunk();
            if (tempChunkArr != null) { if (tempChunkArr[y, x, 0].getIsTransparent()) { doRender = true; } }
        }

        //Transparent Block Above/Below?
        if (y > 0 && y < 254)
            if (chunkBuffer[y + 1, x, z].getIsTransparent() || chunkBuffer[y - 1, x, z].getIsTransparent())
                doRender = true;

        if (x > 0)
            if (chunkBuffer[y, x - 1, z].getIsTransparent()) 
                doRender = true; 

        if (x < 15)
            if (chunkBuffer[y, x + 1, z].getIsTransparent()) 
                doRender = true; 

        if (z > 0)
            if (chunkBuffer[y, x, z - 1].getIsTransparent()) 
                doRender = true; 

        if (z < 15)
            if (chunkBuffer[y, x, z + 1].getIsTransparent()) 
                doRender = true; 

        //SHOULD WATER BLOCK RENDER?
        if (chunkBuffer[y, x, z].getIsLiquid())
        {
            doRender = false;
            if (y > 0 && y < 254)
                if (chunkBuffer[y + 1, x, z].getIsTransparent() && !chunkBuffer[y + 1, x, z].getIsLiquid()) 
                    doRender = true; 

            if (x > 0)
                if (chunkBuffer[y, x - 1, z].getIsTransparent() && !chunkBuffer[y, x - 1, z].getIsLiquid()) 
                    doRender = true; 

            if (x < 15)
                if (chunkBuffer[y, x + 1, z].getIsTransparent() && !chunkBuffer[y, x + 1, z].getIsLiquid()) 
                    doRender = true; 

            if (z > 0)
                if (chunkBuffer[y, x, z - 1].getIsTransparent() && !chunkBuffer[y, x, z - 1].getIsLiquid()) 
                    doRender = true; 
            if (z < 15)
                if (chunkBuffer[y, x, z + 1].getIsTransparent() && !chunkBuffer[y, x, z + 1].getIsLiquid()) 
                    doRender = true; 
        }

        return doRender;
    }

}
