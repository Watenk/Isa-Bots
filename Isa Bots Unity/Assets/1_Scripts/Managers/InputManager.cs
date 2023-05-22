using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : BaseClass
{
    //keyboard
    public bool W;
    public bool D;
    public bool S;
    public bool A;
    public bool F;
    public bool space;
    public bool tab;
    public bool one;
    public bool two;
    public bool three;
    public bool four;
    public bool five;
    public bool six;
    public bool seven;
    public bool eight;
    public bool nein;
    public bool zero;
    public bool F1;
    public bool F2;
    public bool F3;
    //Mouse
    public Vector2 mousePos;
    public Vector2Int mousePosGrid;
    public bool LeftMouse;
    public bool RightMouse;
    public bool MiddleMouse;
    public bool LeftMouseDown;
    public bool RightMouseDown;
    public bool MiddleMouseDown;
    public float ScrollMouseDelta;
    public float VerticalAxis;
    public float HorizontalAxis;

    void Update()
    {
        //Keyboard
        W = Input.GetKeyDown(KeyCode.W);
        D = Input.GetKeyDown(KeyCode.D);
        S = Input.GetKeyDown(KeyCode.S);
        A = Input.GetKeyDown(KeyCode.A);
        F = Input.GetKeyDown(KeyCode.F);
        one = Input.GetKeyDown(KeyCode.Alpha1);
        two = Input.GetKeyDown(KeyCode.Alpha2);
        three = Input.GetKeyDown(KeyCode.Alpha3);
        four = Input.GetKeyDown(KeyCode.Alpha4);
        five = Input.GetKeyDown(KeyCode.Alpha5);
        six = Input.GetKeyDown(KeyCode.Alpha6);
        seven = Input.GetKeyDown(KeyCode.Alpha7);
        eight = Input.GetKeyDown(KeyCode.Alpha8);
        nein = Input.GetKeyDown(KeyCode.Alpha9);
        zero = Input.GetKeyDown(KeyCode.Alpha0);
        space = Input.GetKeyDown(KeyCode.Space);
        tab = Input.GetKeyDown(KeyCode.Tab);
        F1 = Input.GetKeyDown(KeyCode.F1);
        F2 = Input.GetKeyDown(KeyCode.F2);
        F3 = Input.GetKeyDown(KeyCode.F3);

        //Mouse
        mousePos = Input.mousePosition;
        Vector2 tempMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePosGrid = new Vector2Int(Mathf.RoundToInt(tempMousePos.x), Mathf.RoundToInt(-tempMousePos.y));
        LeftMouse = Input.GetKey(KeyCode.Mouse0);
        RightMouse = Input.GetKey(KeyCode.Mouse1);
        MiddleMouse = Input.GetKey(KeyCode.Mouse2);
        LeftMouseDown = Input.GetKeyDown(KeyCode.Mouse0);
        RightMouseDown = Input.GetKeyDown(KeyCode.Mouse1);
        MiddleMouseDown = Input.GetKeyDown(KeyCode.Mouse2);
        ScrollMouseDelta = Input.mouseScrollDelta.y;
        VerticalAxis = Input.GetAxis("Vertical");
        HorizontalAxis = Input.GetAxis("Horizontal");
    }
}