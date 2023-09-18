using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{

    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();   
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }


    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for(int i=0;i<board.width; i++)
        {
            for(int j=0; j<board.height; j++)
            {
                GameObject currentFruit = board.allFruits[i, j];
                if(currentFruit != null)
                { 
                        if(i>0 && i < board.width - 1)
                        {
                            GameObject leftFruit = board.allFruits[i - 1, j];
                            GameObject rightFruit = board.allFruits[i +1, j];
                            if(leftFruit != null && rightFruit != null) {
                                if(leftFruit.tag == currentFruit.tag && rightFruit.tag == currentFruit.tag) {
                                    if (!currentMatches.Contains(leftFruit))
                                    {
                                        currentMatches.Add(leftFruit);
                                    }
                                    leftFruit.GetComponent<Fruit>().isMatched = true;
                                    if (!currentMatches.Contains(rightFruit))
                                    {
                                        currentMatches.Add(rightFruit);
                                    }
                                    rightFruit.GetComponent<Fruit>().isMatched = true;
                                    if (!currentMatches.Contains(currentFruit))
                                    {
                                        currentMatches.Add(currentFruit);
                                    }
                                    currentFruit.GetComponent<Fruit>().isMatched = true;
                                }
                            }
                        }
                        if (j > 0 && j < board.height - 1)
                        {
                            GameObject upFruit = board.allFruits[i, j+1];
                            GameObject downFruit = board.allFruits[i, j-1];
                            if (upFruit != null && downFruit != null)
                            {
                                if (upFruit.tag == currentFruit.tag && downFruit.tag == currentFruit.tag)
                                {
                                    if (!currentMatches.Contains(upFruit))
                                    {
                                        currentMatches.Add(upFruit);
                                    }
                                    upFruit.GetComponent<Fruit>().isMatched = true;
                                    if (!currentMatches.Contains(downFruit))
                                    {
                                        currentMatches.Add(downFruit);
                                    }
                                    downFruit.GetComponent<Fruit>().isMatched = true;
                                    if (!currentMatches.Contains(currentFruit))
                                    {
                                        currentMatches.Add(currentFruit);
                                    }
                                    currentFruit.GetComponent<Fruit>().isMatched = true;
                                }
                            }
                        }
                     
                }
            }
        }
    }

}
