using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int starterMoves = 5;
    [SerializeField] private int amountOfBlocks = 30;
    [SerializeField] private Block[] availableBlocks;
    [SerializeField] private Transform blockParent;
    [SerializeField] private Image blockPrefab;

    [Header("References")]
    [SerializeField] private UIManager uiManager;

    private const int Rows = 6;
    private const int Cols = 5;
    private const float CellWidth = 128f;
    private const float CellHeight = 112f;
    private const float BlockSize = 128f;

    private BlockView[,] _grid;
    private int _movesLeft;
    private bool _isProcessing;

    private WaitForSeconds _wait = new WaitForSeconds(1);

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        _movesLeft = starterMoves;
        _isProcessing = false;
        uiManager.UpdateMoves(_movesLeft);
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        ClearGrid();
        _grid = new BlockView[Rows, Cols];

        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                SpawnBlock(r, c);
    }

    //Usually I would do some pooling, but I am trying to finish this assignment as quickly as possible :)
    private void ClearGrid()
    {
        if (_grid == null) return;

        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (_grid[r, c] != null)
                    Destroy(_grid[r, c].gameObject);
    }

    private void SpawnBlock(int row, int col)
    {
        Block data = availableBlocks[Random.Range(0, availableBlocks.Length)];
        Image instance = Instantiate(blockPrefab, blockParent);

        RectTransform rt = instance.GetComponent<RectTransform>();
        rt.anchoredPosition = GetCellPosition(row, col);
        rt.sizeDelta = new Vector2(BlockSize, BlockSize);

        BlockView view = instance.GetComponent<BlockView>() ?? instance.gameObject.AddComponent<BlockView>();
        view.Initialize(data, row, col);
        _grid[row, col] = view;
    }

    private Vector2 GetCellPosition(int row, int col)
    {
        return new Vector2(
            col * CellWidth + CellWidth * 0.5f,
            -(row * CellHeight + CellHeight * 0.5f)
        );
    }

    public void OnBlockClicked(int row, int col)
    {
        if (_isProcessing || _movesLeft <= 0) return;
        if (_grid[row, col] == null) return;

        List<Vector2Int> connected = GetConnectedBlocks(row, col);

        _isProcessing = true;

        CollectBlocks(connected);
        StartCoroutine(ProcessBlockMovement());
    }

    //There are several ways to make the wait. Invoke, async Task...
    //Here I choose the coroutines as a basic Unity flow
    IEnumerator ProcessBlockMovement()
    {
        yield return _wait;
        
        ApplyGravity();
        FillEmptySpaces();

        _movesLeft--;
        uiManager.UpdateMoves(_movesLeft);

        if (_movesLeft <= 0)
            uiManager.GameOver();

        _isProcessing = false;
    }

    private List<Vector2Int> GetConnectedBlocks(int startRow, int startCol)
    {
        Block target = _grid[startRow, startCol].BlockData;
        bool[,] visited = new bool[Rows, Cols];
        List<Vector2Int> result = new List<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        Enqueue(startRow, startCol, target, visited, queue);

        while (queue.Count > 0)
        {
            Vector2Int curr = queue.Dequeue();
            result.Add(curr);

            Enqueue(curr.x - 1, curr.y, target, visited, queue); // up
            Enqueue(curr.x + 1, curr.y, target, visited, queue); // down
            Enqueue(curr.x, curr.y - 1, target, visited, queue); // left
            Enqueue(curr.x, curr.y + 1, target, visited, queue); // right
        }

        return result;
    }

    private void Enqueue(int r, int c, Block target, bool[,] visited, Queue<Vector2Int> queue)
    {
        if (r < 0 || r >= Rows || c < 0 || c >= Cols) return;
        if (visited[r, c]) return;
        if (_grid[r, c] == null || _grid[r, c].BlockData != target) return;

        visited[r, c] = true;
        queue.Enqueue(new Vector2Int(r, c));
    }

    private void CollectBlocks(List<Vector2Int> positions)
    {
        uiManager.UpdateScore(positions.Count);

        foreach (Vector2Int pos in positions)
        {
            Destroy(_grid[pos.x, pos.y].gameObject);
            _grid[pos.x, pos.y] = null;
        }
    }
    
    private void ApplyGravity()
    {
        for (int c = 0; c < Cols; c++)
        {
            int writeRow = Rows - 1;
            for (int r = Rows - 1; r >= 0; r--)
            {
                if (_grid[r, c] == null) continue;

                if (r != writeRow)
                {
                    _grid[writeRow, c] = _grid[r, c];
                    _grid[r, c] = null;
                    _grid[writeRow, c].SetGridPosition(writeRow, c);
                    _grid[writeRow, c].GetComponent<RectTransform>().anchoredPosition = GetCellPosition(writeRow, c);
                }
                writeRow--;
            }
        }
    }

    private void FillEmptySpaces()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (_grid[r, c] == null)
                    SpawnBlock(r, c);
    }
}
