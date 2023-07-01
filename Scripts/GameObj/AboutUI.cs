using UnityEngine;

public class AboutUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _aboutPanel = null;

    public void ActivateUI(bool value)
    {
        _aboutPanel.SetActive(value);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { UIManager.Instance.OnButtonClick(UIButtonType.AboutUsClose); }
    }
}
