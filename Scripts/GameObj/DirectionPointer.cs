using ZhuanTiNanMin.Mathematics;
using UnityEngine;

public class DirectionPointer : MonoBehaviour
{
    private void Update()
    {
        transform.position = PlayerCtrl.Instance.transform.position;
        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
        Quaternion rot = Funclib.LookRotation2D(pos, Input.mousePosition, 90f);
        transform.rotation = rot;
    }
}
