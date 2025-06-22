using UnityEngine;

public class Gem : MonoBehaviour, IItem
{
    public virtual void Collect()
    {
        Destroy(this.gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
}
