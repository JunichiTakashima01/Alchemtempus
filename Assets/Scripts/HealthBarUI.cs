using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    public RectTransform healthFillerRectTransform;
    private RectTransform healthFrameRectTransform;
    public TMP_Text healthBarText;

    private float maxHealth = 1;
    private float currHealth = 1;

    void Start()
    {
        healthFrameRectTransform = this.GetComponent<RectTransform>();
    }

    public void setCurrAndMaxHealth(float currHealth, float maxHealth)
    {
        this.currHealth = currHealth;
        this.maxHealth = maxHealth;
        setHealthBarText(currHealth, maxHealth);
    }

    public void SetHealthFiller(float currHealth, float maxHealth)
    {
        setCurrAndMaxHealth(currHealth, maxHealth);

        float newWidth = currHealth / maxHealth * healthFrameRectTransform.rect.width;

        healthFillerRectTransform.sizeDelta = new Vector2(newWidth, healthFillerRectTransform.rect.height);
    }

    private void setHealthBarText(float currHealth, float maxHealth)
    {
        healthBarText.text = currHealth + " / " + maxHealth;
    }

}
