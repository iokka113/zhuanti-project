using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private void LateUpdate()
    {
        Vector3 pos = new Vector3(PlayerCtrl.Instance.transform.position.x, PlayerCtrl.Instance.transform.position.y, -10f);
        transform.position = pos;
    }
}
