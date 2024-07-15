using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MazeCellType
{
    DEFAULT,
    MOVING
}

public class MazeCell : MonoBehaviour
{
    [SerializeField] private GameObject leftWall;
    [SerializeField] private GameObject rightWall;
    [SerializeField] private GameObject frontWall;
    [SerializeField] private GameObject backWall;
    [SerializeField] private GameObject unusedCell;

    public bool IsVisited { get; private set; }
    public MazeCellType CellType { get; private set; }
    private GameObject movingWall;
    private Vector3 movingDirection = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;
    private Vector3 currentPosition = Vector3.zero;

    [SerializeField] private float coolTime = 5.0f;
    private float currCoolTime = 0.0f;

    private int mazeSize;

    private void Awake()
    {
        CellType = MazeCellType.DEFAULT;
    }

    private void Start()
    {
        mazeSize = GameManager.Instance.MazeSize;
        if (CellType == MazeCellType.MOVING)
        {
            Debug.Log("Moving Cell");
            movingWall = GetMovingWall();
            if (movingWall == null)
            {
                CellType = MazeCellType.DEFAULT;
                return;
            }
            movingDirection = GetMovingDirection();
            targetPosition = movingWall.transform.position + movingDirection;
            currentPosition = movingWall.transform.position;
        }
    }

    void Update()
    {
        if (CellType == MazeCellType.MOVING)
        {
            if (currCoolTime < coolTime)
            {
                currCoolTime += Time.deltaTime;
                return;
            }

            if (Vector3.Distance(movingWall.transform.position, targetPosition) <= 0)
            {
                currCoolTime = 0.0f;
                targetPosition = currentPosition;
                currentPosition = movingWall.transform.position;
            }
            // if (movingWall == null) SetMovingWall();
            // Debug.Log(movingWall.transform.position.x + " | " + movingWall.transform.position.z);
            movingWall.transform.position = Vector3.MoveTowards(movingWall.transform.position, targetPosition, Time.deltaTime * 1);
        }
    }

    void SetMovingWall()
    {
        movingWall = GetMovingWall();
        movingDirection = movingWall.transform.position - gameObject.transform.position;
    }

    GameObject GetMovingWall()
    {
        if (leftWall.activeSelf && gameObject.transform.position.x != 0) return leftWall;
        if (rightWall.activeSelf && gameObject.transform.position.x != mazeSize) return rightWall;
        if (frontWall.activeSelf && gameObject.transform.position.z != mazeSize) return frontWall;
        if (backWall.activeSelf && gameObject.transform.position.z != 0) return backWall;

        Debug.Log("moving wall not found");

        return default;
    }

    Vector3 GetMovingDirection()
    {
        if (Random.Range(0, 2) == 0) return Random.Range(0, 2) == 0 ? Vector3.left : Vector3.right;
        return Random.Range(0, 2) == 0 ? Vector3.forward : Vector3.back;
    }

    public void Moving()
    {
        CellType = MazeCellType.MOVING;
    }

    #region Clear Walls
    public void Visit()
    {
        IsVisited = true;
        unusedCell.SetActive(false);
    }

    public void ClearLeftWall()
    {
        leftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        backWall.SetActive(false);
    }
    #endregion
}
