using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class Vase : MonoBehaviour
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

    private AudioSource _audio = null;
    [SerializeField]
    private AudioClip _broke = null;

    [SerializeField]
    private DropInfo[] _drops = null;

    private int _atkRareTime;

    private void Start()
    {
        _ren = GetComponent<SpriteRenderer>();
        _colli = GetComponent<CircleCollider2D>();
        _colli.isTrigger = true;
        _audio = GetComponent<AudioSource>();
        Type = PropType.VaseNormal;
    }

    public void Damage()
    {
        _atkRareTime++;
        if (_atkRareTime == 1)
        {
            _atkRareTime = 0;
            _colli.enabled = false;
            Type = PropType.VaseBroken;
            _audio.PlayOneShot(_broke);
            PrefabsManager.DropSpawn(_drops, transform.position);
        }
    }
}
