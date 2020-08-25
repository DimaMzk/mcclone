using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Block
{
    // Object Variables
    [SerializeField]
    private bool isTransparent;
    
    [SerializeField]
    private GameObject objVar;

    [SerializeField]
    private bool isLiquid;

    [SerializeField]
    private bool hasCoordModifier;

    [SerializeField]
    private float xModifier;

    [SerializeField]
    private float zModifier;

    [SerializeField]
    private float yModifier;

    [SerializeField]
    private int ID;


    public Block(bool isTransparent, GameObject objVar, bool isLiquid, int ID)
    {
        setTransparent(isTransparent);
        setGameObject(objVar);
        setIsLiquid(isLiquid);
        UnityEngine.Debug.Log("Created New Block Object.... IS TRANSPARENT: " + getIsTransparent() + " HAS GAME OBEJECT: " + getHasGameObject());
    }

    public Block(bool isTransparent, GameObject objVar, bool isLiquid, float xModifier, float zModifier, float yModifier, int ID){
        setTransparent(isTransparent);
        setGameObject(objVar);
        setIsLiquid(isLiquid);
        setHasCoordModifier(true);
        setxModifier(xModifier);
        setyModifier(yModifier);
        setzModifier(zModifier);
        setID(ID);
    }

    public Block(GameObject objVar, int ID)
    {
        setGameObject(objVar);
        setTransparent(false);
        setID(ID);
        UnityEngine.Debug.Log("Created New Block Object.... IS TRANSPARENT: " + getIsTransparent() + " HAS GAME OBEJECT: " + getHasGameObject());
    }

    public Block(bool isTransparent, int ID)
    {
        setTransparent(isTransparent);
        setGameObject(null);
        setID(ID);
        UnityEngine.Debug.Log("Created New Block Object.... IS TRANSPARENT: " + getIsTransparent() + " HAS GAME OBEJECT: " + getHasGameObject());
    }


    public void setTransparent(bool isTransparent)
    {
        this.isTransparent = isTransparent;
    }

    public void setHasCoordModifier(bool hasCoordModifier){
        this.hasCoordModifier = hasCoordModifier;
    }

    public void setxModifier(float xModifier){
        this.xModifier = xModifier;
    }

    public void setzModifier(float zModifier){
        this.zModifier = zModifier;
    }

    public void setyModifier(float yModifier){
        this.yModifier = yModifier;
    }

    public void setGameObject(GameObject objVar)
    {
        this.objVar = objVar;
    }

    public void setIsLiquid(bool isLiquid)
    {
        this.isLiquid = isLiquid;
    }

    public void setID(int ID){
        this.ID = ID;
    }

    public int getID(){
        return ID;
    }

    public bool getIsLiquid()
    {
        return isLiquid;
    }

    public bool getIsTransparent()
    {
        return isTransparent;
    }

    public GameObject getGameObject()
    {
        return objVar;
    }

    public bool getHasGameObject()
    {
        return (objVar != null);
    }

    public bool getHasCoordModifier(){
        return hasCoordModifier;
    }

    public float getzModifier(){
        return zModifier;
    }

    public float getxModifier(){
        return xModifier;
    }

    public float getyModifier(){
        return yModifier;
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
