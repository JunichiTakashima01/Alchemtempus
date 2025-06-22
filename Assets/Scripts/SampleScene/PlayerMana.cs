using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    public float maxMana = 1000;
    private float currMana;
    public ManaBarUI manaBarUI;
    public Shield shield;


    void Awake()
    {
        currMana = maxMana;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        manaBarUI.SetManaFiller(currMana, maxMana);
    }

    public void UseMana(float mana)
    {
        currMana -= mana;
    }

    public void StartShield()
    {
        UseMana(50);
        this.GetComponent<PlayerHealth>().OnShielding(true);
        shield.ToggleShield(true);
    }

    public void StopShield()
    {
        this.GetComponent<PlayerHealth>().OnShielding(false);
        shield.ToggleShield(false);
    }
}
