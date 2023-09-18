using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private Board board;
    public Text scoreText;
    public int score;
    public Image ScoreBar;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();

    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "" + score;
 
    }
    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        if(board != null && ScoreBar !=null) {
            int length = board.scoreGoals.Length;

            ScoreBar.fillAmount = (float)score / (float)board.scoreGoals[length-1];
        }
    }

}
