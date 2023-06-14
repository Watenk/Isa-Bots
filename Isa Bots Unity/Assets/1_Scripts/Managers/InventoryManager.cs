using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : BaseClass
{
    public int ironAmount;
    public int woodAmount;
    public int fiberAmount;

    public void AddResource(MainID newResource)
    {
        if (newResource == MainID.ironOre)
        {
            ironAmount++;
        }
        else if (newResource == MainID.tree)
        {
            woodAmount++;
        }
        else if (newResource == MainID.flowers || newResource == MainID.tallGrass)
        {
            fiberAmount++;
        }
    }

    public void RemoveResource(MainID newResource)
    {
        if (newResource == MainID.ironOre)
        {
            ironAmount--;
        }
        else if (newResource == MainID.tree)
        {
            woodAmount--;
        }
        else if (newResource == MainID.flowers || newResource == MainID.tallGrass)
        {
            fiberAmount--;
        }
    }
}