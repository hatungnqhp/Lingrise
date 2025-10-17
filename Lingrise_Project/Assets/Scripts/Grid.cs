using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TGridObject>
{
    private int rows;
    private int cols;
    private float cellSize;
    private Vector3 pos;

    public int Rows => rows;
    public int Cols => cols;

    private TGridObject[,] grid;

    public Grid(int rows, int cols, float cellSize, Vector3 pos,
        Func<Grid<TGridObject>, Vector2Int, TGridObject> CreateGridObject)
    {
        this.rows = rows;
        this.cols = cols;
        this.cellSize = cellSize;
        this.pos = pos;

        grid = new TGridObject[cols, rows];
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                TGridObject gridObject = CreateGridObject(this, new Vector2Int(col, row));
                grid[col, row] = gridObject;
            }
        }
    }

    public Vector3 GetGridObjectPos(Vector2Int gridPos)
    {
        return pos + new Vector3(gridPos.x, gridPos.y, 0) * cellSize;
    }

    public TGridObject GetGridObject(Vector3 worldPos)
    {
        Vector3 gridPoint = worldPos - pos;
        Vector2Int gridPos = GetGridPos(gridPoint);
        return GetGridObject(gridPos);
    }

    public TGridObject GetGridObject(Vector2Int gridPos)
    {
        if (!IsInGrid(gridPos)) return default;
        return grid[gridPos.x, gridPos.y];
    }

    public void SetGridObject(Vector2Int gridPos, TGridObject gridObject)
    {
        if (!IsInGrid(gridPos)) return;
        grid[gridPos.x, gridPos.y] = gridObject;
    }

    public bool IsInGrid(Vector2Int gridPos)
    {
        return 0 <= gridPos.x && gridPos.x < grid.GetLength(0)
            && 0 <= gridPos.y && gridPos.y < grid.GetLength(1);
    }

    private Vector2Int GetGridPos(Vector3 gridPoint)
    {
        int x = Mathf.FloorToInt(gridPoint.x / cellSize);
        int y = Mathf.FloorToInt(gridPoint.y / cellSize);

        return new Vector2Int(x, y);
    }
}