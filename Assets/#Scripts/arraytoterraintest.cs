using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

public class arraytoterraintest : MonoBehaviour
{


    public GameObject dirtPrefab;
    public GameObject grassPrefab;
    public GameObject woodPrefab;
    public GameObject leafPrefab;
    public GameObject player;

    public String currentCenterChunk;

    // Set GO Objectsdd

    

    // Start is called before the first frame update
    void Start()
    {
        Block DIRT = new Block(dirtPrefab);
        Block GRASS = new Block(grassPrefab);
        Block WOOD = new Block(woodPrefab);
        Block LEAF = new Block(true, leafPrefab);
        Block AIR = new Block(true);
        // Make butterfly array

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



        // Set all the chunks as testChunk
        for(int i = 0; i < 999999; i++)
        {
            Chunk.setChunk(i, testChunk);

        }



        //renderRadius(0, 0, 4);




    }

    // Update is called once per frame
    void Update()
    {
        int currentXCoord = (int) player.transform.position.x;
        int currentZCoord = (int) player.transform.position.z;
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
        else if(currentXCoord <= -1 && currentXCoord >= -16)
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
        if(startChunk != currentCenterChunk){
            currentCenterChunk = startChunk;
            StartCoroutine(renderRadius(xchunk,zchunk, 4));
        }
        
    }

   

    IEnumerator renderRadius(string xchunk, string zchunk, int radius)
    {


       

        string firstChunkXAxisToRender = xchunk;
        string firstChunkZAxisToRender = zchunk;

        for (int i = 0; i < radius; i++){
            firstChunkXAxisToRender = incrementChunk(firstChunkXAxisToRender);
            firstChunkZAxisToRender = decrementChunk(firstChunkZAxisToRender);
        }
        
        string lastChunkXAxisToRender = xchunk;
        string lastChunkZAxisToRender = zchunk;

        for (int i = 0; i < radius; i++){
            lastChunkXAxisToRender = decrementChunk(lastChunkXAxisToRender);
            lastChunkZAxisToRender = incrementChunk(lastChunkZAxisToRender);
        }



        // Make an array of chunks we want rendered.

        string[] chunksWeWantRendered = new string[1024]; // 32 * 32 render distance max
        string[] chunksWeWantUnrendered = new string[1024];
        int counter = 0;
        for(string xAxis = firstChunkXAxisToRender; xAxis != lastChunkXAxisToRender; xAxis = decrementChunk(xAxis)){
            for(string zAxis = firstChunkZAxisToRender; zAxis != lastChunkZAxisToRender; zAxis = incrementChunk(zAxis)){
                chunksWeWantRendered[counter] = zAxis + xAxis;
                counter++;
            }
        }

        // Render those chunks

        for(int j = 0; j < counter; j++){
            yield return Chunk.loadChunk(chunksWeWantRendered[j]);
        }
        string[] loadedChunkList = new string[Chunk.loadedChunkList.Count];
        Chunk.loadedChunkList.CopyTo(loadedChunkList);

        foreach(string chunk in loadedChunkList){
            bool matchFound = false;
            for(int j = 0; j < counter; j++){
                if(chunk == chunksWeWantRendered[j]){
                    matchFound = true;
                }
            }
            if(!matchFound){
                UnityEngine.Debug.Log("UNLOADING CHUNK!");
                yield return Chunk.unloadChunk(chunk);
            }
        }

        // for(int j = 0; j < subCounter; j++){
        //     yield return Chunk.unloadChunk(chunksWeWantUnrendered[j]);
        // }
        yield break;
    }

    public string insureTripleDigit(int num)
    {
        if(num < 10)
        {
            return "00" + num;
        }
        if(num < 100)
        {
            return "0" + num;
        }

        return num.ToString();
    }

    private string decrementChunk(string chunk){

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

        if(cPOSITIVE == 1){
            return insureTripleDigit(c + 100);
        }
        return insureTripleDigit(c);
    }


    private string incrementChunk(string chunk){
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

        if(cPOSITIVE == 1){
            return insureTripleDigit(c + 100);
        }
        return insureTripleDigit(c);

    }

    
}


