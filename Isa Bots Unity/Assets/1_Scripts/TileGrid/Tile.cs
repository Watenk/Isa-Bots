using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Vector2Int Pos { get; set; }
    public MainID MainID { get; set; }
    public GroundID GroundID { get; set; }
    public int Temp { get; set; }
    public int ThermalConductivity { get; set; }

    public Tile(Vector2Int pos, MainID mainID, GroundID groundID, int temp, int thermalConductivity)
    {
        this.Pos = pos;
        this.MainID = mainID;
        this.GroundID = groundID;
        this.Temp = temp;
        this.ThermalConductivity = thermalConductivity;
    }
}