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

    //References
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
                Task mineTask = new Task(TaskActivity.mine, currentTile.Pos);

                if (currentTile.MainID != MainID.none && !IsPosPresentInATask(currentTile.Pos))
                {
                    tasks.AddTask(mineTask);
                }
            }
        }

        if (inputManager.RightMouseDown)
        {
            if (tileGrid.IsInGridBounds(inputManager.mousePosGrid))
            {
                Tile currentTile = tileGrid.GetTile(inputManager.mousePosGrid);

                tasks.AddTask(new Task(TaskActivity.move, currentTile.Pos));
            }
        }
    }

    private bool IsPosPresentInATask(Vector2Int pos)
    {
        for (int i = 0; i < tasks.activeTaskList.Count; i++)
        {
            if (tasks.activeTaskList[i].Pos == pos)
            {
                return true;
            }
        }

        for (int i = 0; i < tasks.pendingTaskList.Count; i++)
        {
            if (tasks.pendingTaskList[i].Pos == pos)
            {
                return true;
            }
        }

        for (int i = 0; i < tasks.failedTasksList.Count; i++)
        {
            if (tasks.failedTasksList[i].Pos == pos)
            {
                return true;
            }
        }

        return false;
    }
}