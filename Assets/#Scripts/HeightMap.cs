using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HeightMap
{

    // Object Vars

    private float[,] heightMap;

    private int idZ;
    private int idX;

    public HeightMap(float[,] heightMap, int idZ, int idX){
        this.heightMap = heightMap;
        this.idZ = idZ;
        this.idX = idX;
    }

    public void setHeightMap(float[,] heightMap){
        this.heightMap = heightMap;
    }

    public void setIdZ(int idZ){
        this.idZ = idZ;
    }

    public void setIdX(int idX){
        this.idX = idX;
    }

    public float[,] getHeightMap(){
        return heightMap;
    }

    public int getIdZ(){
        return idZ;
    }

    public int getIdX(){
        return idX;
    }

    public override string ToString()
    {
        return "idX:" + idX.ToString() + "//idZ:" + idZ.ToString();
    }

    public override bool Equals(object obj)
    {
        //
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //
        
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        return (obj.ToString().Equals(ToString()));
    }
    


    
}