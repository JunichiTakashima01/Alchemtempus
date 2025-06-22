using UnityEngine;

public class Shield : MonoBehaviour
{
    void Awake()
    {
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.GetComponent<CircleCollider2D>().enabled = false;
        this.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    public void ToggleShield(bool shielding)
    {
        this.GetComponent<CircleCollider2D>().enabled = shielding;
        this.GetComponent<SpriteRenderer>().enabled = shielding;
    }
}
