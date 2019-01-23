/**
* Game of Life Implementation
* @author Sinead Urisohn
*/
using UnityEngine;

public class Cell
{
    private int currentValue;
    private int nextValue;
    private Vector3Int[] neighbours;
    private Vector3Int cellPosition;
    private const int NUMBER_OF_NEIGHBOURS = 8;

    public Cell(int val, Vector3Int cellPos)
    {
        currentValue = val;
        nextValue = -1;
        cellPosition = cellPos;
        neighbours = FindCellNeighbours();
    }

    private Vector3Int[] FindCellNeighbours()
    {
        Vector3Int [] neighbours = new Vector3Int[NUMBER_OF_NEIGHBOURS];
        neighbours[0] = new Vector3Int(cellPosition.x - 1, cellPosition.y, cellPosition.z);
        neighbours[1] = new Vector3Int(cellPosition.x + 1, cellPosition.y, cellPosition.z);
        neighbours[2] = new Vector3Int(cellPosition.x, cellPosition.y - 1, cellPosition.z);
        neighbours[3] = new Vector3Int(cellPosition.x, cellPosition.y + 1, cellPosition.z);
        neighbours[4] = new Vector3Int(cellPosition.x - 1, cellPosition.y - 1, cellPosition.z);
        neighbours[5] = new Vector3Int(cellPosition.x - 1, cellPosition.y + 1, cellPosition.z);
        neighbours[6] = new Vector3Int(cellPosition.x + 1, cellPosition.y + 1, cellPosition.z);
        neighbours[7] = new Vector3Int(cellPosition.x + 1, cellPosition.y - 1, cellPosition.z);
        return WrapNeighbourBounds(neighbours);
    }
    
    private Vector3Int[] WrapNeighbourBounds(Vector3Int [] neighs)
    {
        for(int n = 0; n < NUMBER_OF_NEIGHBOURS; n++)
        {
            if (neighs[n].x > GameOfLife.maxBounds.x) neighs[n].x = GameOfLife.minBounds.x;
            if (neighs[n].x < GameOfLife.minBounds.x) neighs[n].x = GameOfLife.maxBounds.x;
            if (neighs[n].y > GameOfLife.maxBounds.y) neighs[n].y = GameOfLife.minBounds.y;
            if (neighs[n].y < GameOfLife.minBounds.y) neighs[n].y = GameOfLife.maxBounds.y;
        }
        return neighs;
    }

    public Vector3Int[] GetCellNeighbours()
    {
        return neighbours;
    }

    public void SetCellState(int val)
    {
        currentValue = val;
    }

    public void SetNextCellState(int val)
    {
        nextValue = val;
    }

    public int GetNextCellState()
    {
        return nextValue;
    }

    public int GetCellState()
    {
        return currentValue;
    }

    public Vector3Int GetCellPosition()
    {
        return cellPosition;
    }
   
}
