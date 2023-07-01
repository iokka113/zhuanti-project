using UnityEngine;
using UnityEngine.EventSystems;

public class UIOpenURL : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private string _url = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        Application.OpenURL(_url);
    }
}
