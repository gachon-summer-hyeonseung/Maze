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

    [Serializable]
    class ItemOptions
    {
        public GameObject itemPrefab;

        [Range(0, 1)]
        [SerializeField]
        private float spawnRate;

        public float RateByDifficulty(DifficultyType difficulty)
        {
            return difficulty switch
            {
                DifficultyType.Easy => spawnRate,
                DifficultyType.Normal => spawnRate * 0.25f,
                DifficultyType.Hard => spawnRate * 0.5f,
                _ => spawnRate,
            };
        }
    }

    [SerializeField] private GameObject mazeCellPrefab;
    [SerializeField] private GameObject mazeExitCellPrefab;
    [SerializeField] private Transform mazeContainer;
    [SerializeField] private ItemOptions[] items;

    // [SerializeField]
    // private int mazeSize;

    [Range(0, 1)]
    [SerializeField]
    private float movingCellPercentage;

    private MazeCell[,] mazeGrid;
    private List<StackItem>[] stacks;

    void Start()
    {
        int mazeSize = GameManager.Instance.MazeSize;
        Create(mazeSize, mazeSize);
        CreateItem(mazeSize, mazeSize);
        GameManager.Instance.OnMazeGenerated();
    }

    public void Create(int width, int height)
    {
        // mazeWidth = width;
        // mazeHeight = height;
        mazeGrid = new MazeCell[width, height];

        float movingWallRate = RateByDifficulty((DifficultyType)GameManager.Instance.Difficulty);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GameObject prefab = x == width - 1 && z == height - 1 ? mazeExitCellPrefab : mazeCellPrefab;
                MazeCell cell = Instantiate(prefab, new Vector3(x, 0, z), Quaternion.identity, mazeContainer).GetComponent<MazeCell>();
                mazeGrid[x, z] = cell;

                if (x == 0 || x == width - 1 || z == 0 || z == height - 1) continue;
                float randValue = UnityEngine.Random.Range(0f, 1f);
                if (randValue < movingWallRate)
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
            bool status = Step(width, height);
            // Debug.Log(status);
            // Debug.Log("Step");
            if (!status) break;
        }

        // GenerateMaze(null, mazeGrid[0, 0]);
        // mazeGrid[0, 0].ClearBackWall();
        // mazeGrid[width - 1, height - 1].ClearFrontWall();
    }

    private bool Step(int width, int height)
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

                if (x >= 0 && x < width && z >= 0 && z < height)
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

    private void CreateItem(int width, int height)
    {
        foreach (var item in items)
        {
            float spawnRate = item.RateByDifficulty((DifficultyType)GameManager.Instance.Difficulty);
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    float randValue = UnityEngine.Random.Range(0f, 1f);
                    if (randValue < spawnRate)
                    {
                        Instantiate(item.itemPrefab, new Vector3(x, 0f, z), Quaternion.identity, mazeContainer);
                    }
                }
            }
        }
    }

    public float RateByDifficulty(DifficultyType difficulty)
    {
        return difficulty switch
        {
            DifficultyType.Easy => movingCellPercentage,
            DifficultyType.Normal => movingCellPercentage * 1.25f,
            DifficultyType.Hard => movingCellPercentage * 1.5f,
            _ => movingCellPercentage,
        };
    }
}
