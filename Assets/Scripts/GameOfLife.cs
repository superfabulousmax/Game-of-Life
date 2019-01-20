/**
* Game of Life Implementation
* @author Sinead Urisohn
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameOfLife : MonoBehaviour
{
    [SerializeField]
    private int gridSize = 8;
    [SerializeField]
    private float timeStep = 2f;
    private float timer = 0;
    private Tilemap tileMap;
    private List<Cell> cells;
    private int xMin, yMin, xMax, yMax;
    private Color[] OnColors = {Color.white, Color.green, Color.yellow, Color.cyan, Color.yellow};
    private Color[] OffColors = {Color.black, Color.red, Color.black, Color.black, Color.red};
    private int colorPos;

	void Start ()
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
                Vector2Int minBounds = new Vector2Int(xMin, yMin);
                Vector2Int maxBounds = new Vector2Int(xMax, yMax);
                Cell c = new Cell(value, position, minBounds, maxBounds);
                cells.Add(c);
            }
        }
    }
	
	void Update ()
    {
        timer += Time.deltaTime;
        if(timer >= timeStep)
        {
            foreach (var cell in cells)
            {
                PlayGameOfLife(cell);
            }
            timer = 0;
        }
	}

    private void PlayGameOfLife(Cell cell)
    {
        if(NumberOfNeighersInStateOne(cell) == 3)
        {
            cell.SetCellState(1);
            tileMap.SetColor(cell.GetCellPosition(), OnColors[colorPos]);

        }
        else if(cell.GetCellState() == 1 && NumberOfNeighersInStateOne(cell) == 2)
        {
            return;
        }
        else
        {
            cell.SetCellState(0);
            tileMap.SetColor(cell.GetCellPosition(), OffColors[colorPos]);
        }
    }

    private int GetRandomStartValue()
    {
        return Random.Range(-40, 2);
    }

    private int NumberOfNeighersInStateOne(Cell cell)
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
        return Random.Range(0, OffColors.Length);
    }
}
