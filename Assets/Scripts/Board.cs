using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
} //Нужны, чтобы пока все совпадению не уберутся, играть нельзя было
public enum TileKind
{
    Breakable,
    Blank,
    Normal
}
[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject[] fruits;
    public GameObject destroyEffect;
    public GameObject breakableTilePrefab;
    private bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;
    public TileType[] boardLayout;
    public GameObject[,] allFruits;
    private FindMatches findMatches;
    
    public int basePieceValue = 20;
    private int streakValue = 1;
    private ScoreManager scoreManager;
    private GoalManager goalManager;
    public float refillDelay = 0.5f;
    public int[] scoreGoals;



    // Start is called before the first frame update
    void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        breakableTiles = new BackgroundTile[width, height];
        scoreManager = FindObjectOfType<ScoreManager>();
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        allFruits = new GameObject[width, height];
        SetUp();
        currentState = GameState.pause;
    }


    public void GenerateBlankspaces()
    {
        for(int i= 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakableTales()
    {
        for(int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                Vector2 tempPosition= new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition,Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void SetUp()
    {
        GenerateBlankspaces();
        GenerateBreakableTales();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i,j])
                {
                Vector2 tempPosition = new Vector2(i, j+offSet);
                Vector2 tilePosition = new Vector2(i, j);
                GameObject backgroundTile=Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;   
                backgroundTile.name = "(" + i+ ", " +j + ")";
                int fruitToUse = Random.Range(0, fruits.Length);
                int maxIterations = 0;
                while (MatchesAt(i, j, fruits[fruitToUse]) && maxIterations< 100 ) //Пока есть совпадения и кол-во итераций до 100 заново генерировать без совпадений
                {
                    fruitToUse = Random.Range(0, fruits.Length);
                    maxIterations++;
                    Debug.Log(maxIterations);
                }
                maxIterations = 0;

                GameObject fruit = Instantiate(fruits[fruitToUse], tempPosition, Quaternion.identity);
                fruit.GetComponent<Fruit>().row = j;
                fruit.GetComponent<Fruit>().column = i;
                fruit.transform.parent = this.transform;
                fruit.name = "(" + i + ", " + j + ")";
                allFruits[i,j]= fruit;
                }
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece) // На проверку совпадений
    {
        if(column> 1 && row> 1)
        {
            if (allFruits[column-1,row]!=null && allFruits[column - 2, row] != null) { 
                if (allFruits[column-1,row].tag == piece.tag && allFruits[column-2,row].tag == piece.tag)//для колонок совпадения 
                {
                    return true;
                }
            }
            if (allFruits[column, row - 1] != null && allFruits[column, row - 2] != null)
            {
                if (allFruits[column, row - 1].tag == piece.tag && allFruits[column, row - 2].tag == piece.tag)//для строк совпадения 
                {
                    return true;
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allFruits[column, row-1] != null && allFruits[column, row-2] != null)
                {
                    if (allFruits[column, row - 1].tag == piece.tag && allFruits[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (allFruits[column - 1, row] != null && allFruits[column - 2, row] != null)
                {
                    if (allFruits[column - 1, row].tag == piece.tag && allFruits[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void DestroyMatchesAt(int column, int row) //изчезание совпадений
    {
        if (allFruits[column, row].GetComponent<Fruit>().isMatched)
        {


            if (breakableTiles[column, row] != null)
            {
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column,row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
                
            }
            if(goalManager!= null)
            {
                goalManager.CompareGoal(allFruits[column,row].tag.ToString());
                goalManager.UpdateGoals();
            }
            findMatches.currentMatches.Remove(allFruits[column, row]);
            GameObject practicle = Instantiate(destroyEffect, allFruits[column, row].transform.position, Quaternion.identity);
            Destroy(practicle, 0.5f);
            Destroy(allFruits[column, row]);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            allFruits[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allFruits[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo2());
    }

    private IEnumerator DecreaseRowCo2()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j=0;j< height; j++)
            {
                //если текущее место не пусто это  no пустота
                if (!blankSpaces[i, j] && allFruits[i,j]==null)
                {
                    for(int k = j + 1; k < height; k++)
                    {
                        if (allFruits[i,k]!= null)
                        {
                            allFruits[i, k].GetComponent<Fruit>().row = j;
                            allFruits[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay*0.5f);
        StartCoroutine(FillBoardCo());
    }



    private IEnumerator DecreaseRowCo() // опускание итемов вниз
    {
        int nullCount = 0;
        for (int i=0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allFruits[i, j] == null)
                {
                    nullCount++;
                }
                else if(nullCount >0)
                {
                    allFruits[i, j].GetComponent<Fruit>().row -= nullCount;
                    allFruits[i, j] = null;
                }
            }
            nullCount = 0;
        }

        yield return new WaitForSeconds(refillDelay); //4
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard() // заполнение итемами
    {
        for(int i = 0; i < width; i++)
        {
            for(int j=0; j < height; j++)
            {
                if (allFruits[i,j] == null && !blankSpaces[i,j])
                {
                    Vector2 tempPosition = new Vector2(i, j+offSet);
                    int fruitToUse = Random.Range(0, fruits.Length);
                    int maxIteration = 0;

                    while(MatchesAt(i,j, fruits[fruitToUse]) && maxIteration < 100)
                    {
                        maxIteration++;
                        fruitToUse = Random.Range(0, fruits.Length);
                    }
                    maxIteration = 0;
                    GameObject piece = Instantiate(fruits[fruitToUse], tempPosition, Quaternion.identity);
                    allFruits[i, j] = piece;
                    piece.GetComponent<Fruit>().row = j;
                    piece.GetComponent<Fruit>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allFruits[i, j] != null)
                {
                    if (allFruits[i, j].GetComponent<Fruit>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);

        while (MatchesOnBoard())
        {
            streakValue ++;
            yield return new WaitForSeconds(2* refillDelay);
            DestroyMatches();
            
        }

        if (IsDeadLocked())
        {
            ShuffleBoard();
            Debug.Log("Deadlocked!!!");
        }
        yield return new WaitForSeconds(refillDelay);
        currentState = GameState.move;
        streakValue = 1;
    }

    // ----Обрабатываем отсутствие ходов
    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        GameObject holder = allFruits[column +(int)direction.x,row + (int)direction.y] as GameObject;
        allFruits[column + (int)direction.x, row + (int)direction.y] = allFruits[column, row];
        allFruits[column,row] = holder;


    }

    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        { 
            for (int j = 0; j < height; j++)
            {
                if (allFruits[i, j] != null)
                {
                    //Проверка на совпадение рядом находящихся точек по горизонтали

                    if (i < width - 2)
                    {
                        if (allFruits[i + 1, j] != null && allFruits[i + 2, j] != null)
                        {
                            if (allFruits[i + 1, j].tag == allFruits[i, j].tag && allFruits[i + 2, j].tag == allFruits[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }

                    if (j < height - 2)
                    {

                        //Проверка на совпадение рядом находящихся точек по вертикали
                        if (allFruits[i, j + 1] != null && allFruits[i, j + 2] != null)
                        {
                            if (allFruits[i, j + 1].tag == allFruits[i, j].tag && allFruits[i, j + 2].tag == allFruits[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
    private bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadLocked()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allFruits[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if(SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
    // -- Обновляем поле
   private void ShuffleBoard()
    {
        
        List<GameObject> newBoard = new List<GameObject>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allFruits[i, j] != null)
                {
                    newBoard.Add(allFruits[i, j]);
                }
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i,j])
                {
                    int pieceToUse =Random.Range(0,newBoard.Count);

                    int maxIterations = 0;
                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100) //Пока есть совпадения и кол-во итераций до 100 заново генерировать без совпадений
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                        Debug.Log(maxIterations);
                    }

                    Fruit piece = newBoard[pieceToUse].GetComponent<Fruit>();
                    maxIterations = 0;
                    piece.column = i;
                    piece.row = j;
                    allFruits[i, j] = newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                }

            }
        }
        if(IsDeadLocked())
        {
           ShuffleBoard();
        }
    }
}
