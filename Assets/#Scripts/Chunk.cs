using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

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

    private GameObject[,,] GameObjects = new GameObject[255,16,16];


    public Chunk(Block[,,] chunkData, int idX, int idZ)
    {
        this.chunkData = chunkData;
        this.idX = idX;
        this.idZ = idZ;
        this.isLoaded = false;
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

    public void setGameObjects(GameObject[,,] gameObjects){
        this.GameObjects = gameObjects;
    }

    public GameObject[,,] GetGameObjects(){
        return GameObjects;
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

    
    public string ToJSON()
    {
        StringBuilder sb = new StringBuilder(139310, 150000);
        sb.Append("{");
        sb.Append("\"idX\": " + idX.ToString() + ",");
        sb.Append("\"idZ\": " + idZ.ToString() + ",");
        sb.Append("\"isLoaded\" :" + isLoaded.ToString().ToLower() + ",");
        sb.Append("\"chunkData\": ["); // Y

        for (int y = 0; y < 255; y++)
        {
            sb.Append("[");

            for (int x = 0; x < 16; x++)
            {
                sb.Append("[");
                
                for (int z = 0; z < 16; z++)
                {
                    sb.Append(chunkData[y, x, z].getID().ToString());

                    if (z != 15)
                    {
                        sb.Append(",");
                    }
                }

                sb.Append("]");

                if (x != 15)
                {
                    sb.Append(",");
                }
            }
            sb.Append("]");

            if (y != 254)
            {
                sb.Append(",");
            }
        }

        sb.Append("]");
        sb.Append("}");
        return sb.ToString();
    }

    public bool Equals(Chunk obj){
        return obj.ToString().Equals(ToString());
    }

    public override string ToString(){
        return "idZ:" + idZ.ToString() + "//idX:" + idX.ToString();
    }
}