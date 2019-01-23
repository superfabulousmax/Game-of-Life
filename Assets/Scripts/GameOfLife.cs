/**
* Game of Life Implementation
* @author Sinead Urisohn
*/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameOfLife : MonoBehaviour
{
    public static bool startSim = false;
    [SerializeField]
    private float timeStep = 2f;
    private float timer = 0;
    private Tilemap tileMap;
    private List<Cell> cells;
    private int xMin, yMin, xMax, yMax;
    public static Color[] OnColors = {Color.white, Color.green, Color.yellow, Color.cyan, Color.yellow};
    public static Color[] OffColors = {Color.black, Color.red, Color.black, Color.black, Color.red};
    public static int colorPos;
    public static Vector2Int minBounds, maxBounds;
    public static bool isStable;

    void Awake ()
    {
        tileMap = transform.GetComponentInParent<Tilemap>();
        cells = new List<Cell>();
        xMin = 100000;
        yMin = 100000;
        xMax = -100000;
        yMax = -100000;
        colorPos = GetRandomColorPair();
        foreach (var position in tileMap.cellBounds.allPositionsWithin)
        {
            if(tileMap.GetSprite(position)!=null)
            {
                tileMap.SetTileFlags(position, TileFlags.None);
                tileMap.SetColor(position, OnColors[colorPos]);
                if (position.x < xMin) xMin = position.x;
                if (position.x > xMax) xMax = position.x;
                if (position.y < yMin) yMin = position.y;
                if (position.y > yMax) yMax = position.y;
                
                int value = GetRandomStartValue();
                if (value <= 0)
                {
                    tileMap.SetColor(position, OffColors[colorPos]);
                    value = 0;
                }
                else
                {
                    value = 1;
                }
                
                Cell c = new Cell(value, position);
                cells.Add(c);
            }
        }
        minBounds = new Vector2Int(xMin, yMin);
        maxBounds = new Vector2Int(xMax, yMax);
    }
	
	void Update ()
    {
        if (startSim)
        {
            timer += Time.deltaTime;
            if (timer >= timeStep)
            {
                SetCellValuesFromColors();
                foreach (var cell in cells)
                {
                    PlayGameOfLife(cell);
                }
                isStable = HasStableState();
                UpdateColors();
                
                timer = 0;
            }
        }
	}

    private void SetCellValuesFromColors()
    {
        foreach (var cell in cells)
        {
            cell.SetCellState(0);
            if (tileMap.GetColor(cell.GetCellPosition()) == OnColors[colorPos])
            {
                cell.SetCellState(1);
            }
        }
    }

    private void UpdateColors()
    {
        foreach (var cell in cells)
        {
            tileMap.SetTileFlags(cell.GetCellPosition(), TileFlags.None);
            cell.SetCellState(cell.GetNextCellState());
            if(cell.GetCellState() == 1)
                tileMap.SetColor(cell.GetCellPosition(), OnColors[colorPos]);
            else
                tileMap.SetColor(cell.GetCellPosition(), OffColors[colorPos]);
        }
    }

    private void PlayGameOfLife(Cell cell)
    {
        cell.SetNextCellState(0);
        if (NumberOfNeighboursInStateOne(cell) == 3)
        {
            cell.SetNextCellState(1);
        }
        else if(cell.GetCellState() == 1 && NumberOfNeighboursInStateOne(cell) == 2)
        {
            cell.SetNextCellState(1);
        }
    }

    private int GetRandomStartValue()
    {
        return UnityEngine.Random.Range(-10, 2);
    }

    private int NumberOfNeighboursInStateOne(Cell cell)
    {
        int count = 0;
        foreach (var n in cell.GetCellNeighbours())
        {
            if (tileMap.GetColor(n) == OnColors[colorPos])
            {
                count++;
            }
        }
        return count;
    }

    private int GetRandomColorPair()
    {
        return UnityEngine.Random.Range(0, OffColors.Length);
    }

    private bool HasStableState()
    {
        foreach (var cell in cells)
        {
            if (cell.HasStateChange())
                return false;
        }
        return true;
    }
}
