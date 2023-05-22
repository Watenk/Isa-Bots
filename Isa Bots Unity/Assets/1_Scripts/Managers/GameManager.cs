using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<BaseClass> UpdateOrder = new List<BaseClass>();

    public float UPS; //UPS
    private float upsTimer;

    private void Awake()
    {
        //AutoStart
        List<BaseClass> baseclassList = new List<BaseClass>();
        baseclassList.AddRange(FindObjectsOfType<BaseClass>());

        for (int i = 0; i < baseclassList.Count; i++)
        {
            if (baseclassList[i].AutoStart)
            {
                UpdateOrder.Add(baseclassList[i]);
            }
        }

        for (int i = 0; i < UpdateOrder.Count; i++)
        {
            UpdateOrder[i].OnAwake();
        }
    }

    private void Start()
    {
        //Settings
        Application.targetFrameRate = 144;
        QualitySettings.vSyncCount = 1;

        for (int i = 0; i < UpdateOrder.Count; i++)
        {
            UpdateOrder[i].OnStart();
        }
    }

    private void Update()
    {
        for (int i = 0; i < UpdateOrder.Count; i++)
        {
            UpdateOrder[i].OnUpdate();
        }

        //Calc UPS
        if (upsTimer > 1 / UPS)
        {
            for (int i = 0; i < UpdateOrder.Count; i++)
            {
                UpdateOrder[i].OnUPS();
            }
            upsTimer = 0;
        }
        upsTimer += Time.deltaTime;
    }

    public void AddObject(BaseClass newObject)
    {
        UpdateOrder.Add(newObject);
        newObject.OnAwake();
        newObject.OnStart();
    }

    public void RemoveObject(BaseClass removedObject)
    {
        UpdateOrder.Remove(removedObject);
        Destroy(removedObject.gameObject);
    }
}