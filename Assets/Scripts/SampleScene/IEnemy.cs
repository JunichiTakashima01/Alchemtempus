using UnityEngine;

public interface IEnemy
{
    public void TakeDamage(float dmg, float knockBackDistance = 0f);
    public void DestroyEnemy();
}
