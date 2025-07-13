using System.Collections.Generic;
using UnityEngine;

public class AttackCollisionBox : MonoBehaviour
{
    private List<IEnemy> collisions = new List<IEnemy>();

    // void Awake()
    // {
    //     Enemy.OnEnemyKilled += OnEnemyKilled;
    // }

    // void OnDestroy()
    // {
    //     Enemy.OnEnemyKilled -= OnEnemyKilled;
    // }

    void OnTriggerEnter2D(Collider2D collision)
    {
        IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
        if (enemy != null)
        {
            collisions.Add(enemy);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
        if (enemy != null)
        {
            collisions.Remove(enemy);
        }
    }

    public List<IEnemy> GetCollisionEnemies()
    {
        return collisions;
    }

    private void OnEnemyKilled(Enemy enemy)
    {
        collisions.Remove(enemy);
    }
}
