using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveSlotUiButton : MonoBehaviour, IPointerClickHandler, ISelectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image background;
    [SerializeField] public TMP_Text text;
    private Color unselectedColor = new Color(0.2f, 0.2f, 0.2f, 0.1f);
    private Color unselectedTextColor = new Color(0.85f, 0.85f, 0.85f, 1f);
    private Color selectedColor = Color.royalBlue;

    

    public event Action<int> OnClick;
    private int _index;



    private bool _isSelected;

    public void SetIndex(int index)
    {
        _index = index;
        _isSelected = false;
        background.color = unselectedColor;
        text.color = unselectedTextColor;
    }

    

    public void OnPointerClick(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);

        OnClick?.Invoke(_index);
        Highlight();
    }

    public void OnSelect(BaseEventData eventData)
    {
        _isSelected = true;
        Highlight();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;
        ResetColors();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isSelected)
        {
            ResetColors();
        }
    }

    public void Highlight()
    {
        background.color = selectedColor;
        text.color = Color.white;
    }

    public void ResetColors()
    {
        _isSelected = false;
        background.color = unselectedColor;
        text.color = unselectedTextColor;
    }

}
