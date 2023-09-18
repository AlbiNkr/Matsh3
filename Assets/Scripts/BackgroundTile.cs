using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    // Белые ячейки, скрипт связан с префабом Tile Background
    public int hitPoints;
    private SpriteRenderer sprite; 
    private GoalManager goalManager;
    
    private void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(hitPoints <= 0)
        {
            if(goalManager != null)
            {
                goalManager.CompareGoal(this.gameObject.tag);
                goalManager.UpdateGoals();
            }
            Destroy(this.gameObject); 
            MakeLighter();
        }
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
    }
    void MakeLighter()
    {
        Color color = sprite.color;
        float newAlpha = color.a * 0.5f;
        sprite.color = new Color(color.r,color.g, color.b,newAlpha); 
    }

}
