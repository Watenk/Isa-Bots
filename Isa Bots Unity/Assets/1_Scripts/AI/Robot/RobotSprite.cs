using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSprite : BaseClass
{
    public Robot robot;
    public GameObject sprite;

    public float slerpAmount;
    public float stopDistance;

    public override void OnUpdate()
    {
        if (Vector3.Distance(sprite.transform.position, new Vector3(robot.Pos.x, -robot.Pos.y, -2)) > stopDistance)
        {
            Vector3 newPos = Vector3.Slerp(sprite.transform.position, new Vector3(robot.Pos.x, -robot.Pos.y, -2), slerpAmount);
            sprite.transform.position = newPos;
        }
    }
}
