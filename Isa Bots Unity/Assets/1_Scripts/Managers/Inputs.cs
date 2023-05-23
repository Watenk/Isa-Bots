using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Inputs : BaseClass
{
    public float ScrollSpeed;
    public int minCamSize;
    public int maxCamSize;

    private Vector2 referenceMousePos;

    //references
    private InputManager inputManager;
    private TileGrid tileGrid;
    private Tasks tasks;

    public override void OnAwake()
    {
        inputManager = FindObjectOfType<InputManager>();
        tileGrid = FindObjectOfType<TileGrid>();
        tasks = FindObjectOfType<Tasks>();
    }

    public override void OnUpdate()
    {
        Camera();
        MouseInput();
        //Overlays();
    }

    private void Camera()
    {
        //Mouse
        if (inputManager.MiddleMouseDown)
        {
            referenceMousePos = Input.mousePosition;
            referenceMousePos = UnityEngine.Camera.main.ScreenToWorldPoint(referenceMousePos);
        }

        if (inputManager.MiddleMouse)
        {
            //Get mousepos and calc newPos
            Vector2 currentMousePos = Input.mousePosition;
            currentMousePos = UnityEngine.Camera.main.ScreenToWorldPoint(currentMousePos);
            float xDifference = currentMousePos.x - referenceMousePos.x;
            float yDifference = currentMousePos.y - referenceMousePos.y;
            float newXPos = UnityEngine.Camera.main.transform.position.x - xDifference;
            float newYPos = UnityEngine.Camera.main.transform.position.y - yDifference;

            //Set newPos
            Vector3 newPos = new Vector3(newXPos, newYPos, -10);
            UnityEngine.Camera.main.transform.position = newPos;
        }

        //Scroll up
        if (inputManager.ScrollMouseDelta > 0f && UnityEngine.Camera.main.orthographicSize > minCamSize && Input.GetMouseButton(2) == false)
        {
            UnityEngine.Camera.main.orthographicSize -= UnityEngine.Camera.main.orthographicSize * ScrollSpeed * 0.01f;
        }

        //Scroll down
        if (inputManager.ScrollMouseDelta < 0f && UnityEngine.Camera.main.orthographicSize < maxCamSize && Input.GetMouseButton(2) == false)
        {
            UnityEngine.Camera.main.orthographicSize += UnityEngine.Camera.main.orthographicSize * ScrollSpeed * 0.01f;
        }
    }

    private void MouseInput()
    {
        if (inputManager.LeftMouseDown)
        {
            if (tileGrid.IsInGridBounds(inputManager.mousePosGrid))
            {
                Tile currentTile = tileGrid.GetTile(inputManager.mousePosGrid);
                
                if (currentTile.MainID != MainID.none)
                {
                    tasks.AddTask(new Task(TaskActivity.mine, currentTile.Pos));
                }
            }
        }

        if (inputManager.RightMouseDown)
        {
            if (tileGrid.IsInGridBounds(inputManager.mousePosGrid))
            {
                Tile currentTile = tileGrid.GetTile(inputManager.mousePosGrid);

                if (currentTile.MainID == MainID.none)
                {
                    tasks.AddTask(new Task(TaskActivity.move, currentTile.Pos));
                }
            }
        }
    }

    //private void Overlays()
    //{
    //    //TempOverlay
    //    if (inputManager.F1 == true)
    //    {
    //        if (tempRenderer.activeSelf == false)
    //        {
    //            tempRenderer.SetActive(true);
    //        }
    //        else
    //        {
    //            tempRenderer.SetActive(false);
    //        }
    //    }

    //    //AmountOverlay
    //    if (inputManager.F2 == true)
    //    {
    //        if (amountRenderer.activeSelf == false)
    //        {
    //            amountRenderer.SetActive(true);
    //        }
    //        else
    //        {
    //            amountRenderer.SetActive(false);
    //        }
    //    }
    //}
}