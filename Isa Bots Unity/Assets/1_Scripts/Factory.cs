using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Factory : BaseClass
{
    public Vector2Int Pos { get; set; }
    public float productionTime;
    public GameObject robot;
    public GameObject ProducingParticle;
    
    private GameObject Bots;
    private float productionTimer;
    private bool isProducing;
    private GameManager gameManager;

    public override void OnAwake()
    {
        base.OnAwake();
        gameManager = FindObjectOfType<GameManager>();
        Bots = FindObjectOfType<Bots>().gameObject;
    }

    public void StartProduction()
    {
        isProducing = true;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (isProducing)
        {
            if (productionTimer >= productionTime)
            {
                GameObject newRobot = Instantiate(robot, new Vector3(Pos.x, -Pos.y, -2), Quaternion.identity);
                newRobot.transform.parent = Bots.transform;
                Robot newRobotClass = newRobot.GetComponent<Robot>();
                RobotSprite newRobotSprite = newRobot.GetComponentInChildren<RobotSprite>();
                gameManager.AddObject(newRobotClass);
                gameManager.AddObject(newRobotSprite);
                newRobotClass.SetPos(Pos);
                productionTimer = 0;
                isProducing = false;
            }
            else
            {
                productionTimer += Time.deltaTime;
            }
        }
    }

    public override void OnUPS()
    {
        base.OnUPS();
        if (isProducing)
        {
            Instantiate(ProducingParticle, new Vector3(Pos.x, -Pos.y, -5), Quaternion.identity);
        }
    }
}