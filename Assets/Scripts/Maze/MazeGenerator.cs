using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public abstract class BaseItemOption
{
    public GameObject itemPrefab;

    [Range(0, 1)]
    [SerializeField]
    protected float spawnRate;

    public abstract float RateByDifficulty(DifficultyType difficulty);
}

public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator Instance { get; private set; }

    class StackItem
    {
        public MazeCell cell;
        public int x;
        public int z;
        public List<int[]> directions = new() { new int[] { 1, 0 }, new int[] { -1, 0 }, new int[] { 0, 1 }, new int[] { 0, -1 } };
    }

    [Serializable]
    class ItemOptions : BaseItemOption
    {
        public override float RateByDifficulty(DifficultyType difficulty)
        {
            return difficulty switch
            {
                DifficultyType.Easy => spawnRate,
                DifficultyType.Normal => spawnRate * 0.75f,
                DifficultyType.Hard => spawnRate * 0.5f,
                _ => spawnRate,
            };
        }
    }

    [Serializable]
    class ObstacleOptions : BaseItemOption
    {
        public override float RateByDifficulty(DifficultyType difficulty)
        {
            return difficulty switch
            {
                DifficultyType.Easy => spawnRate,
                DifficultyType.Normal => spawnRate * 1.25f,
                DifficultyType.Hard => spawnRate * 1.5f,
                _ => spawnRate,
            };
        }
    }

    [SerializeField] private GameObject mazeCellPrefab;
    [SerializeField] private GameObject mazeExitCellPrefab;
    [SerializeField] private Transform mazeContainer;
    [SerializeField] private ItemOptions[] items;
    [SerializeField] private ObstacleOptions[] obstacles;

    // [SerializeField]
    // private int mazeSize;

    [Range(0, 1)]
    [SerializeField]
    private float movingCellPercentage;

    private BaseItemOption[,] itemGrid;
    private MazeCell[,] mazeGrid;
    private List<StackItem>[] stacks;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        int mazeSize = GameManager.Instance.MazeSize;
        Create(mazeSize, mazeSize);
        CreateItem(mazeSize, mazeSize);
        GameManager.Instance.OnMazeGenerated();

        // var path = GetShortestDistance(0, 0);
        // foreach (var cell in path)
        // {
        //     cell.ShowFloor();
        // }
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
        itemGrid = new BaseItemOption[width, height];

        foreach (var item in items)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    BaseItemOption cellItem = itemGrid[x, z];
                    if (cellItem != null) continue;

                    float spawnRate = item.RateByDifficulty((DifficultyType)GameManager.Instance.Difficulty);
                    float randValue = UnityEngine.Random.Range(0f, 1f);
                    if (randValue < spawnRate)
                    {
                        Instantiate(item.itemPrefab, new Vector3(x, 0f, z), Quaternion.identity, mazeContainer);
                        itemGrid[x, z] = item;
                    }
                }
            }
        }

        foreach (var obstacle in obstacles)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    BaseItemOption cellItem = itemGrid[x, z];
                    if (cellItem != null) continue;

                    float spawnRate = obstacle.RateByDifficulty((DifficultyType)GameManager.Instance.Difficulty);
                    float randValue = UnityEngine.Random.Range(0f, 1f);
                    if (randValue < spawnRate)
                    {
                        Instantiate(obstacle.itemPrefab, new Vector3(x, 0f, z), Quaternion.identity, mazeContainer);
                        itemGrid[x, z] = obstacle;
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

    class BFSCell
    {
        public int x;
        public int z;
        public BFSCell prev;
    }

    public MazeCell[] GetShortestDistance(int x, int y)
    {
        int mazeSize = GameManager.Instance.MazeSize;
        BFSCell[,] visited = new BFSCell[mazeSize, mazeSize];
        Queue<BFSCell> cellList = new();

        var hostCell = new BFSCell { x = x, z = y, prev = null };
        visited[x, y] = hostCell;
        cellList.Enqueue(hostCell);
        // Debug.Log("Test : " + visited[11, 11].x);

        int[] dirX = new int[] { 1, -1, 0, 0 };
        int[] dirZ = new int[] { 0, 0, 1, -1 };

        while (cellList.Count != 0)
        {
            var cell = cellList.Dequeue();
            if (cell.x == mazeSize - 1 && cell.z == mazeSize - 1)
            {
                List<MazeCell> path = new();
                while (cell != null)
                {
                    path.Add(mazeGrid[cell.x, cell.z]);
                    cell = cell.prev;
                }
                return path.ToArray();
            }

            for (int i = 0; i < 4; i++)
            {
                int nx = cell.x + dirX[i];
                int nz = cell.z + dirZ[i];

                if (nx < 0 || nx >= mazeSize || nz < 0 || nz >= mazeSize) continue;
                if (visited[nx, nz] != default) continue;
                if (CheckWall(mazeGrid[cell.x, cell.z], mazeGrid[nx, nz])) continue;

                var newCell = new BFSCell { x = nx, z = nz, prev = cell };
                visited[nx, nz] = newCell;
                cellList.Enqueue(newCell);
            }
        }

        return default;
    }

    bool CheckWall(MazeCell curr, MazeCell post)
    {
        if (curr.transform.position.x < post.transform.position.x)
        {
            return curr.rightWall.activeSelf;
        }

        if (curr.transform.position.x > post.transform.position.x)
        {
            return curr.leftWall.activeSelf;
        }

        if (curr.transform.position.z < post.transform.position.z)
        {
            return curr.frontWall.activeSelf;
        }

        if (curr.transform.position.z > post.transform.position.z)
        {
            return curr.backWall.activeSelf;
        }

        return false;
    }

    public void ShowHint(int x, int y)
    {
        StartCoroutine(IEShowHint(x, y));
    }

    IEnumerator IEShowHint(int x, int y)
    {
        var path = GetShortestDistance(x, y);
        foreach (var cell in path)
        {
            cell.ShowFloor();
        }
        yield return new WaitForSeconds(3.0f);
        foreach (var cell in path)
        {
            cell.HideFloor();
        }
    }
}
