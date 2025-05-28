using UnityEngine;

public class Collector : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        IItem iItem = collision.gameObject.GetComponent<IItem>();
        if (iItem != null)
        {
            iItem.Collect();
        }
    }
}
