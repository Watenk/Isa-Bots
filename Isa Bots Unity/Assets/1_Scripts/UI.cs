using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI : BaseClass
{
    public Text AverageFrameRate;
    public Text LowestFrame;

    public Text MouseMainID;
    public Text MouseGroundID;
    public Text MouseTemp;

    private int frameRateFrameAmount = 30;
    private int averageFPS;
    private int lowestFrame;
    private float[] frames;
    private int frameCounter;

    //References
    private InputManager inputManager;
    private TileGrid tileGrid;

    public override void OnAwake()
    {
        inputManager = FindObjectOfType<InputManager>();
        tileGrid = FindObjectOfType<TileGrid>();
    }

    public override void OnStart()
    {
        frames = new float[frameRateFrameAmount];
    }

    public override void OnUpdate()
    {
        CalcFPS();
        DrawUI();
    }

    private void DrawUI()
    {
        if (tileGrid.IsInGridBounds(inputManager.mousePosGrid))
        {
            Tile currentTile = tileGrid.GetTile(inputManager.mousePosGrid);

            //ID
            MouseMainID.text = currentTile.MainID.ToString() + " : MainID";
            MouseGroundID.text = currentTile.GroundID.ToString() + " : GroundID";
            
            //Temp
            string temp = currentTile.Temp.ToString().PadLeft(6);
            MouseTemp.text = temp.Insert(temp.Length - 3, ".") + " : Temp";
        }
    }

    private void CalcFPS()
    {
        if (frameCounter != frameRateFrameAmount) //Count
        {
            frames[frameCounter] = 1.0f / Time.deltaTime;
            frameCounter += 1;
        }
        else //Set Result
        {
            averageFPS = (int)frames.Sum() / frames.Length;
            lowestFrame = (int)frames.Min();
            AverageFrameRate.text = "AverageFPS: " + averageFPS.ToString();
            LowestFrame.text = "LowestFrame: " + lowestFrame.ToString();
            frameCounter = 0;

            //Colors
            if (averageFPS <= 59)
            {
                if (averageFPS <= 29)
                {
                    AverageFrameRate.color = Color.red;
                }
                else
                {
                    AverageFrameRate.color = Color.yellow;
                }
            }
            else
            {
                AverageFrameRate.color = Color.white;
            }

            if (lowestFrame <= 59)
            {
                if (lowestFrame <= 29)
                {
                    LowestFrame.color = Color.red;
                }
                else
                {
                    LowestFrame.color = Color.yellow;
                }
            }
            else
            {
                LowestFrame.color = Color.white;
            }
        }
    }
}