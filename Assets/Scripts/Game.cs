using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    private int width;
    private int height;
    private int mineCount;
    private bool gameOver;
    private Board board;
    private Cell[,] state;
    public Text textGameOver;
    public Text textWin;
    public Text textFlag;
    public Button btnEasy;
    public Button btnNormal;
    public Button btnHard;
    private int flagCount;
    private DifficultConfig diffConfig;
    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }
    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }
    public void Start()
    {
        btnEasy.gameObject.SetActive(true);
        btnNormal.gameObject.SetActive(true);
        btnHard.gameObject.SetActive(true);
    }
    public void RestartGame()
    {
        NewGame();
        textGameOver.gameObject.SetActive(false);
       // btnRestart.gameObject.SetActive(false);
        gameOver = false;
        btnEasy.gameObject.SetActive(false);
        btnNormal.gameObject.SetActive(false);
        btnHard.gameObject.SetActive(false);
    }
    private void NewGame()
    {
        flagCount = mineCount;
        state = new Cell[width, height];
        GenerateCells();
        GenerateMines();
        GenerateNumber();
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        board.Draw(state);
        //btnEasy.gameObject.SetActive(false);
        //btnNormal.gameObject.SetActive(false);
        //btnHard.gameObject.SetActive(false);
        textFlag.gameObject.SetActive(true);
    }
    public void SetUpDifficult(int diff)
    {
        diffConfig = DifficultConfigConstance.p[diff];
        width = diffConfig.width;
        height = diffConfig.height;
        mineCount = diffConfig.mineCount;
        NewGame();
    }
    private void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                cell.type = Cell.Type.Empty;
                state[x, y] = cell;
            }
        }
    }
    private void GenerateMines()
    {
        for (int i = 0; i < mineCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            while (state[x, y].type == Cell.Type.Mine)
            {
                x++;
                if (x >= width)
                {
                    x = 0;
                    y++;
                }
            }
            state[x, y].type = Cell.Type.Mine;
            // state[x, y].revealed = true;
        }
    }
    private void GenerateNumber()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                if (cell.type == Cell.Type.Mine)
                {
                    continue;
                }
                cell.number = CountMines(x, y);
                if (cell.number > 0)
                {
                    cell.type = Cell.Type.Number;
                }
                //cell.revealed = true;
                state[x, y] = cell;
            }
        }
    }
    private int CountMines(int cellX, int cellY)
    {
        int count = 0;
        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0)
                {
                    continue;
                }
                int x = cellX + adjacentX;
                int y = cellY + adjacentY;

                if (GetCell(x, y).type == Cell.Type.Mine)
                {
                    count++;
                }
            }
        }
        return count;
    }
    private void Update()
    {
        textFlag.text = "Flag: " + flagCount;
        if (!gameOver)
        {
            if (Input.GetMouseButtonDown(1) && flagCount > 0)
            {
                Flag();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Reveal();
            }
        }
    }
    private void Flag()
    {
        Vector3 wordPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(wordPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);
        if (cell.type == Cell.Type.Invalid || cell.revealed)
        {
            return;
        }
        cell.flagged = !cell.flagged;
        // if (cell.flagged)
        // {
        //     flagCount--;
        // }
        // else
        // {
        //     flagCount++;
        // }
        flagCount = cell.flagged == true ? flagCount - 1 : flagCount + 1;
        state[cellPosition.x, cellPosition.y] = cell;

        board.Draw(state);
    }
    private void Reveal()
    {
        Vector3 wordPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(wordPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);
        if (cell.type == Cell.Type.Invalid || cell.revealed || cell.flagged)
        {
            return;
        }
        switch (cell.type)
        {
            case Cell.Type.Mine:
                Explode(cell);
                break;
            case Cell.Type.Empty:
                Flood(cell);
                CheckWinCondition();
                break;
            default:
                cell.revealed = true;
                state[cellPosition.x, cellPosition.y] = cell;
                CheckWinCondition();
                break;
        }


        board.Draw(state);

    }
    private void Flood(Cell cell)
    {
        //if (cell.revealed) return;
        if (cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid || cell.revealed) return;
        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;
        if (cell.type == Cell.Type.Empty)
        {
            Flood(GetCell(cell.position.x - 1, cell.position.y));
            Flood(GetCell(cell.position.x + 1, cell.position.y));
            Flood(GetCell(cell.position.x, cell.position.y - 1));
            Flood(GetCell(cell.position.x, cell.position.y + 1));
        }
    }
    private void Explode(Cell cell)
    {
        gameOver = true;
        textGameOver.gameObject.SetActive(true);
        btnEasy.gameObject.SetActive(true);
        btnNormal.gameObject.SetActive(true);
        btnHard.gameObject.SetActive(true);
        //btnRestart.gameObject.SetActive(true);
        textFlag.gameObject.SetActive(false);
        cell.revealed = true;
        cell.exploded = true;
        state[cell.position.x, cell.position.y] = cell;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = state[x, y];
                if (cell.type == Cell.Type.Mine)
                {
                    cell.revealed = true;
                    state[x, y] = cell;
                }
            }
        }
    }
    private void CheckWinCondition()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Cell cell = state[i, j];
                if (cell.type != Cell.Type.Mine && !cell.revealed)
                {
                    return;
                }
            }
        }
        //Debug.Log("Win!");
        textWin.gameObject.SetActive(true);
        gameOver = true;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                if (cell.type == Cell.Type.Mine)
                {
                    cell.flagged = true;
                    state[x, y] = cell;
                }
            }
        }
    }
    private Cell GetCell(int x, int y)
    {
        if (IsValid(x, y))
        {
            return state[x, y];
        }
        else return new Cell();
    }
    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
