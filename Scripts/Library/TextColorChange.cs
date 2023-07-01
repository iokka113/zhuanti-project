using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Text))]
public class TextColorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text _text = null;

    [SerializeField]
    private Color _origColor = Color.white;
    [SerializeField]
    private Color _focusColor = Color.white;

    private void Start()
    {
        _text = GetComponent<Text>();
        _text.color = _origColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _text.color = _focusColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _text.color = _origColor;
    }
}
