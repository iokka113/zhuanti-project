using UnityEngine.UI;
using UnityEngine;

public class TextTyper : MonoBehaviour
{
    [SerializeField]
    private Text _text = null;
    private string _words;

    private float _timer = 0f;
    [SerializeField]
    private float _speed = 0.1f;

    private int _position = 0;
    public bool IsFinished { get; private set; }

    private void Start()
    {
        _words = _text.text;
        _text.text = "";
    }

    private void Update()
    {
        if (_position < _words.Length)
        {
            _timer += Time.deltaTime;
            if (_timer >= _speed)
            {
                _position++;
                _text.text = _words.Substring(0, _position);
                _timer = 0f;
            }
        }
        else
        {
            IsFinished = true;
        }
    }
}
