using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Inputs : BaseClass
{
    public float ScrollSpeed;
    public int minCamSize;
    public int maxCamSize;

    public GameObject FactoryScript;
    public GameObject factoryPlacementParticle;

    private Vector2 referenceMousePos;

    //References
    private InputManager inputManager;
    private TileGrid tileGrid;
    private Tasks tasks;
    private InventoryManager inventory;
    private FactoryManager factoryManager;

    public override void OnAwake()
    {
        inputManager = FindObjectOfType<InputManager>();
        tileGrid = FindObjectOfType<TileGrid>();
        tasks = FindObjectOfType<Tasks>();
        inventory = FindObjectOfType<InventoryManager>();
        factoryManager = FindObjectOfType<FactoryManager>();
    }

    public override void OnUpdate()
    {
        Camera();
        MouseInput();
        KeyboardInput();
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
        if (inputManager.LeftMouse)
        {
            if (tileGrid.IsInGridBounds(inputManager.mousePosGrid))
            {
                Tile currentTile = tileGrid.GetTile(inputManager.mousePosGrid);
                Task mineTask = new Task(TaskActivity.mine, currentTile.Pos);

                if (currentTile.MainID != MainID.none && currentTile.MainID != MainID.factory && !IsPosPresentInATask(currentTile.Pos))
                {
                    tasks.AddTask(mineTask);
                }

                if (currentTile.MainID == MainID.factory && !IsPosPresentInATask(currentTile.Pos))
                {
                    Factory currentFactory = factoryManager.GetFactory(currentTile.Pos);
                    if (currentFactory != null)
                    {
                        factoryManager.RemoveFactory(currentFactory);
                        tasks.AddTask(mineTask);
                    }
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

    private void KeyboardInput()
    {
        if (inputManager.space)
        {
            if (tileGrid.IsInGridBounds(inputManager.mousePosGrid))
            {
                Tile currentTile = tileGrid.GetTile(inputManager.mousePosGrid);

                if (currentTile.MainID == MainID.none)
                {
                    if (inventory.ironAmount >= 3 && inventory.woodAmount >= 8)
                    {
                        inventory.ironAmount -= 3;
                        inventory.woodAmount -= 10;
                        Instantiate(factoryPlacementParticle, new Vector3(currentTile.Pos.x, -currentTile.Pos.y, -5), Quaternion.identity);
                        tileGrid.SetTile(currentTile.Pos, MainID.factory, currentTile.GroundID, true);
                        GameObject newFactoryGameObject = Instantiate(FactoryScript, new Vector3(currentTile.Pos.x, -currentTile.Pos.y, -2), Quaternion.identity);
                        Factory newFactory = newFactoryGameObject.GetComponent<Factory>();
                        newFactory.Pos = currentTile.Pos;
                        factoryManager.AddFactory(newFactory);
                    }
                }

                if (currentTile.MainID == MainID.factory)
                {
                    if (inventory.ironAmount >= 5 && inventory.fiberAmount >= 13 && inventory.woodAmount >= 2)
                    {
                        inventory.ironAmount -= 5;
                        inventory.woodAmount -= 2;
                        inventory.fiberAmount -= 13;
                        factoryManager.StartProduction(inputManager.mousePosGrid);
                    }
                }
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