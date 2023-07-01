using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private UIButtonType _type = UIButtonType.ExitTheApp;

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.Instance.OnButtonClick(_type);
    }
}
