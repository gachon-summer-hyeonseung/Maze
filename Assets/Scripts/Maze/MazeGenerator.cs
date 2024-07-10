using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    class StackItem
    {
        public MazeCell cell;
        public int x;
        public int z;
        public List<int[]> directions = new() { new int[] { 1, 0 }, new int[] { -1, 0 }, new int[] { 0, 1 }, new int[] { 0, -1 } };
    }

    [SerializeField] private GameObject mazeCellPrefab;
    [SerializeField] private Transform mazeContainer;

    [SerializeField]
    private int mazeSize;

    [Range(0, 1)]
    [SerializeField]
    private float movingCellPercentage;

    private MazeCell[,] mazeGrid;
    private List<StackItem>[] stacks;

    void Start()
    {
        Create(mazeSize, mazeSize);
    }

    public void Create(int width, int height)
    {
        // mazeWidth = width;
        // mazeHeight = height;
        mazeGrid = new MazeCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                MazeCell cell = Instantiate(mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity, mazeContainer).GetComponent<MazeCell>();
                mazeGrid[x, z] = cell;
                float randValue = UnityEngine.Random.Range(0f, 1f);
                if (randValue < movingCellPercentage)
                {
                    cell.Moving();
                }
            }
        }

        // int starts = (int)Math.Truncate((double)width / 6);
        // if (starts > 2) starts = 2;
        int starts = 1;
        stacks = new List<StackItem>[starts];

        for (int i = 0; i < starts; i++)
        {
            var stack = stacks[i] = new List<StackItem>();
            int[] cellPos = new int[] { UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height) };
            var startItem = new StackItem { cell = mazeGrid[cellPos[0], cellPos[1]], x = cellPos[0], z = cellPos[1] };
            stack.Add(startItem);
        }


        while (true)
        {
            bool status = Step();
            Debug.Log(status);
            Debug.Log("Step");
            if (!status) break;
        }

        // GenerateMaze(null, mazeGrid[0, 0]);
        // mazeGrid[0, 0].ClearBackWall();
        mazeGrid[width - 1, height - 1].ClearFrontWall();
    }

    private bool Step()
    {
        bool activated = false;

        for (int i = 0; i < stacks.Length; i++)
        {
            List<StackItem> stack = stacks[i];
            if (stack.Count == 0) continue;

            activated = true;

            StackItem currentCell = stack[stack.Count - 1];
            currentCell.cell.Visit();

            MazeCell nextCell = default;

            while (currentCell.directions.Count != 0)
            {
                int index = UnityEngine.Random.Range(0, currentCell.directions.Count);
                int[] direction = currentCell.directions[index];
                int x = currentCell.x + direction[0];
                int z = currentCell.z + direction[1];

                if (x >= 0 && x < mazeSize && z >= 0 && z < mazeSize)
                {
                    nextCell = mazeGrid[x, z];
                    if (nextCell.IsVisited == false)
                    {
                        ClearWalls(currentCell.cell, nextCell);
                        stack.Add(new StackItem { cell = nextCell, x = x, z = z });
                        break;
                    }
                }

                currentCell.directions.RemoveAt(index);
            }

            if (nextCell == null) stacks[i].RemoveAt(stack.Count - 1);
        }

        return activated;
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }
}
