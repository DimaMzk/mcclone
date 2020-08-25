using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Chunk
{

    [SerializeField]
    private Block[,,] chunkData;

    [SerializeField]
    private int idX;

    [SerializeField]
    private int idZ;

    [SerializeField]
    private bool isLoaded;


    public Chunk(Block[,,] chunkData, int idX, int idZ)
    {
        this.chunkData = chunkData;
        this.idX = idX;
        this.idZ = idZ;
    }

    public void setChunk(Block[,,] chunkData)
    {
        this.chunkData = chunkData;
    }

    public void setIdX(int idX)
    {
        this.idX = idX;
    }

    public void setIdZ(int idZ)
    {
        this.idZ = idZ;
    }

    public void setIsLoaded(bool isLoaded)
    {
        this.isLoaded = isLoaded;
    }



    public bool getIsLoaded()
    {
        return isLoaded;
    }
    public int getIdX()
    {
        return idX;
    }

    public int getIdZ()
    {
        return idZ;
    }

    public Block[,,] getChunk()
    {
        return chunkData;
    }

    override
    public string ToString()
    {

        string returnString = "{";
        returnString += "\"idX\": " + idX.ToString() + ",";
        returnString += "\"idZ\": " + idZ.ToString() + ",";
        returnString += "\"isLoaded\" :" + isLoaded.ToString().ToLower() + ",";
        returnString += "\"chunkData\": ["; // Y

        for (int y = 0; y < 255; y++)
        {
            returnString += "[";

            for (int x = 0; x < 16; x++)
            {
                returnString += "[";
                
                for (int z = 0; z < 16; z++)
                {
                    returnString += chunkData[y, x, z].getID().ToString();

                    if (z != 15)
                    {
                        returnString += ",";
                    }
                }

                returnString += "]";

                if (x != 15)
                {
                    returnString += ",";
                }
            }
            returnString += "]";

            if (y != 254)
            {
                returnString += ",";
            }
        }

        returnString += "]";
        returnString += "}";
        return returnString;
    }
}