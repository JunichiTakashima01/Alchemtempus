using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseButtonMouseDetect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<bool> OnMouseEnterUIStatus;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        OnMouseEnterUIStatus.Invoke(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        OnMouseEnterUIStatus.Invoke(false);

    }

    public void OnPauseButtonClicked()
    {
        OnMouseEnterUIStatus.Invoke(false);
    }
}
