using TMPro;
using UnityEngine;

public class ManaBarUI : MonoBehaviour
{
    public RectTransform manaFillerRectTransform;
    private RectTransform manaFrameRectTransform;
    public TMP_Text manaBarText;

    private float maxMana;
    private float currMana;

    void Start()
    {
        manaFrameRectTransform = this.GetComponent<RectTransform>();
        currMana = maxMana;
    }

    public void setCurrAndMaxMana(float currMana, float maxMana)
    {
        this.currMana = currMana;
        this.maxMana = maxMana;
        setManaBarText(currMana, maxMana);
    }

    public void SetManaFiller(float currMana, float maxMana)
    {
        setCurrAndMaxMana(currMana, maxMana);

        float newWidth = currMana / maxMana * manaFrameRectTransform.rect.width;

        manaFillerRectTransform.sizeDelta = new Vector2(newWidth, manaFillerRectTransform.rect.height);
    }

    private void setManaBarText(float currMana, float maxMana)
    {
        manaBarText.text = currMana + " / " + maxMana;
    }
}
