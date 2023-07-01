using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Portal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerCtrl.Instance.gameObject.SetActive(false);
            FindObjectOfType<DirectionPointer>().gameObject.SetActive(false);
            _fadeOutGo = true;
        }
    }

    private bool _fadeOutGo;
    [SerializeField]
    private Image _fadeOutMask = null;

    private void Start()
    {
        _fadeOutMask.color = Color.clear;
    }

    private void Update()
    {
        if (_fadeOutGo)
        {
            _fadeOutMask.color += new Color(0f, 0f, 0f, Time.deltaTime / 2f);
            if (_fadeOutMask.color.a >= 0.9f)
            {
                _fadeOutGo = false;
                LevelManager.Instance.IntoNextLevel();
            }
        }
    }
}
