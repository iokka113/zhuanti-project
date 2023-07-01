using UnityEngine;

public class EndingText : MonoBehaviour
{
    [SerializeField]
    private TextTyper _typer = null;
    [SerializeField]
    private GameObject _backToHome = null;

    private void Update()
    {
        if (_typer.IsFinished)
        {
            _backToHome.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.Instance.OnButtonClick(UIButtonType.BackToHome);
            }
        }
    }
}
