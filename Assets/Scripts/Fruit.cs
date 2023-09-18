using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private EndGameManager endGameManager;
    private FindMatches findMatches;
    private Board board;

    private GameObject otherFruit;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    public float swipeAngle = 0;
    public float swipeResist = 1f;


    // Start is called before the first frame update
    void Start()
    {
        endGameManager = FindObjectOfType<EndGameManager>();
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        // targetX=(int)transform.position.x;
        // targetY= (int)transform.position.y;
        //// row = targetY;
        //column = targetX;
        //   previousColumn = column;
        // previousRow = row;
    }

    // Update is called once per frame
    void Update()
    {
    // FindMatches();
        if (isMatched) // ≈сли найдет совпадени€, закрасит их в черный цвет
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .5f); //6
            if (board.allFruits[column, row] != this.gameObject)
            {
                board.allFruits[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();

        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .5f); //6
            if (board.allFruits[column, row] != this.gameObject)
            {
                board.allFruits[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;

        }
    }
    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if(otherFruit != null)
        {
            if(!isMatched && !otherFruit.GetComponent<Fruit>().isMatched)
            {
                otherFruit.GetComponent<Fruit>().row = row;
                otherFruit.GetComponent<Fruit>().column = column;
                column = previousColumn;
                row = previousRow;
                yield return new WaitForSeconds(.5f);
                board.currentState = GameState.move;
            }
            else
            {
                if (endGameManager != null)
                {
                    if (endGameManager.requirements.gameType == GameType.Moves)
                    {
                        endGameManager.DecreaseCounterValue();
                    }
                }
                board.DestroyMatches();
            }
            otherFruit =  null;
        }
       
    }
    
    
    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
      //  Debug.Log(firstTouchPosition); // ќтслеживаем местоположение позиции

    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }
    void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentState = GameState.wait;
        }
        else
        {
            board.currentState = GameState.move;
        }
            
    }
    void MovePiecesActual(Vector2 direction)
    {
        otherFruit = board.allFruits[column+(int)direction.x, row + (int)direction.y];
        previousColumn = column;
        previousRow = row;
        if (otherFruit != null)
        {

            otherFruit.GetComponent<Fruit>().row += -1 * (int)direction.y;
            otherFruit.GetComponent<Fruit>().column += -1 * (int)direction.x;
            column += (int)direction.x;
            row += (int)direction.y;
            StartCoroutine(CheckMoveCo());
        }
        else
        {
            board.currentState = GameState.move;
        }
    }


    void MovePieces()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width-1){ // Rigth Swipe
            //otherFruit = board.allFruits[column + 1, row];
            //previousColumn = column;
            //previousRow = row;
            //otherFruit.GetComponent<Fruit>().column -= 1;
            //column += 1;
            MovePiecesActual(Vector2.right);
        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height-1)
        {//Up Swipe
            //otherFruit = board.allFruits[column, row + 1];
            //previousColumn = column;
            //previousRow = row;
            //otherFruit.GetComponent<Fruit>().row -= 1;
            //row += 1;
            MovePiecesActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        { //Left Swipe
            //otherFruit = board.allFruits[column - 1, row];
            //previousColumn = column;
            //previousRow = row;
            //otherFruit.GetComponent<Fruit>().column += 1;
            //column -= 1;
            MovePiecesActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {//Down 
            //otherFruit = board.allFruits[column, row - 1];
            //previousColumn = column;
            //previousRow = row;
            //otherFruit.GetComponent<Fruit>().row += 1;
            //row -= 1;
            MovePiecesActual(Vector2.down);
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void FindMatches() //Ќайти совпадени€
    {
        if (column > 0 && column < board.width - 1) //горизонтальные совпадени€
        {
            GameObject leftFruit1 = board.allFruits[column - 1, row];
            GameObject rightFruit1 = board.allFruits[column + 1, row];
            if (leftFruit1 != null && rightFruit1 != null)
            {

                if (leftFruit1.tag == this.gameObject.tag && rightFruit1.tag == this.gameObject.tag)
                {
                    leftFruit1.GetComponent<Fruit>().isMatched = true;
                    rightFruit1.GetComponent<Fruit>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1) //горизонтальные совпадени€
        {
            GameObject upFruit1 = board.allFruits[column, row + 1];
            GameObject downFruit1 = board.allFruits[column, row - 1];
            if (upFruit1 != null && downFruit1 != null)
            {
                if (upFruit1.tag == this.gameObject.tag && downFruit1.tag == this.gameObject.tag)
                {
                    upFruit1.GetComponent<Fruit>().isMatched = true;
                    downFruit1.GetComponent<Fruit>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

}
