using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotWaitState : BaseState
{
    public int waitAmount;

    private int waitTimer;

    //references
    private Robot robot;

    public override void OnStart()
    {
        robot = GetComponent<Robot>();
    }

    public override void OnUPS()
    {
        if (waitTimer >= waitAmount)
        {
            waitTimer = 0;
            robot.ChooseTask();
        }
        else
        {
            waitTimer++;
        }
    }

    public override void OnExit()
    {

    }
}
