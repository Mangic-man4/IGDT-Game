using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonInteractionHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public DifficultySelector difficultySelector;

    public void OnPointerDown(PointerEventData eventData)
    {
        // Check if the pointer event is for the left button
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            difficultySelector.DecreaseDifficulty();
        }
        // Check if the pointer event is for the right button
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            difficultySelector.IncreaseDifficulty();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // You can optionally handle the pointer up event here if needed
    }
}

