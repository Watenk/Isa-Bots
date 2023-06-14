using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FactoryManager : BaseClass
{
    public List<Factory> factories = new List<Factory>();

    private GameManager gameManager;

    public override void OnAwake()
    {
        base.OnAwake();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void StartProduction(Vector2Int startPos)
    {
        for (int i = 0; i < factories.Count; i++)
        {
            if (factories[i].Pos == startPos)
            {
                factories[i].StartProduction();
            }
        }
    }

    public void AddFactory(Factory newFactory)
    {
        if (!factories.Contains(newFactory))
        {
            factories.Add(newFactory);
            gameManager.AddObject(newFactory);
            newFactory.transform.SetParent(this.gameObject.transform);
        }
    }

    public void RemoveFactory(Factory removeFactory)
    {
        factories.Remove(removeFactory);
        gameManager.RemoveObject(removeFactory);
        Destroy(removeFactory.gameObject);
    }

    public Factory GetFactory(Vector2Int pos)
    {
        for (int i = 0; i < factories.Count; i++)
        {
            if (factories[i].Pos == pos)
            {
                return factories[i];
            }
        }

        return null;
    }
}