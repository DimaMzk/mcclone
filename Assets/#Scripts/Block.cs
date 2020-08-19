using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block
{
    // Object Variables
    private bool isTransparent;
    private GameObject objVar;

    public Block(bool isTransparent, GameObject objVar)
    {
        setTransparent(isTransparent);
        setGameObject(objVar);
    }

    public Block(GameObject objVar)
    {
        setGameObject(objVar);
    }

    public Block(bool isTransparent)
    {
        setTransparent(isTransparent);
    }

    public void setTransparent(bool isTransparent)
    {
        this.isTransparent = isTransparent;
    }

    public void setGameObject(GameObject objVar)
    {
        this.objVar = objVar;
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
