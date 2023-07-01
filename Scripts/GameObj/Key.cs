using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Key : MonoBehaviour
{
    private SpriteRenderer _ren;
    private CircleCollider2D _colli;

    private PropType _type;
    public PropType Type
    {
        get { return _type; }
        private set
        {
            _type = value;
            _ren.sprite = PrefabsManager.Instance.PropGetPic(_type);
        }
    }

    private void Start()
    {
        _ren = GetComponent<SpriteRenderer>();
        _colli = GetComponent<CircleCollider2D>();
        _colli.isTrigger = true;
        Type = PropType.Roomkey;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCtrl.Instance.Key = this;
            _colli.enabled = false;
        }
    }
}
