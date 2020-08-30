using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class JsonChunk
{
public int idX { get; set; } 
public int idZ { get; set; } 
public bool isLoaded { get; set; } 
public List<List<List<int>>> chunkData { get; set; } 
    
public Chunk toChunk()
{   Block[,,] blocks = new Block[255,16,16];
    int[,,] IDS = new int[255,16,16];
    for(int y = 0; y < 255; y++){
        for(int x = 0; x < 16; x++){
            for(int z = 0; z < 16; z++){
                blocks[y,x,z] = IdToBlock(chunkData[y][x][z]);
            }
        }
    }

    return new Chunk(blocks, idX, idZ);    
}

private Block IdToBlock(int ID){
     switch (ID)
        {
            case 0:
                return arraytoterraintest.AIR;
            case 1: 
                return arraytoterraintest.DIRT;
                
            case 2: 
                return arraytoterraintest.GRASS;
            case 3: 
                return arraytoterraintest.WOOD;
            case 4: 
                return arraytoterraintest.LEAF;
            case 5: 
                return arraytoterraintest.SNOW_LEAF;
            case 6: 
                return arraytoterraintest.WATER_FULL;
            case 7: 
                return arraytoterraintest.SAND;
            case 8: 
                return arraytoterraintest.SNOW_GRASS;
            default:
                throw new Exception("Block with ID " + ID.ToString() + " Does not exist");
        }
}
}