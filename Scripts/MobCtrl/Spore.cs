using ZhuanTiNanMin.Mathematics;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CircleCollider2D))]
public class Spore : MonoBehaviour
{
    private Room _room;

    [SerializeField]
    private Image _hpFill = null;
    [SerializeField]
    private float _hpMax = 0;
    private float _hp = 0;

    private bool _growing;
    private float _timer;
    private Vector2 _posStay;

    [SerializeField]
    private float _ripeningTime = 8f;

    public void Init(Room room, Vector2 pos)
    {
        _room = room;
        transform.position = pos;
        _hp = _hpMax;
        _hpFill.fillAmount = 1f;
        _posStay = Funclib.RandomInsideCircle(pos, 2f);
        _growing = false;
        _room.MobUpdate += MobUpdate;
        _room.MobCount++;
    }

    private void MobUpdate()
    {
        if (Vector2.Distance(transform.position, _posStay) < 0.01f)
        {
            Grow();
        }
        else
        {
            if (!_growing)
            {
                Move();
            }
        }
        if (_growing && Time.time > _timer)
        {
            Instantiate(PrefabsManager.Instance.GetMobPrefab(MobType.Mushroom)).GetComponent<MushroomCtrl>().Init(_room, transform.position);
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Grow();
        }
    }

    private void Grow()
    {
        if (!_growing)
        {
            _timer = Time.time + _ripeningTime;
            _growing = true;
        }
    }

    private void Move()
    {
        Vector2 dir = (_posStay - (Vector2)transform.position).normalized;
        transform.position += (Vector3)dir * Time.deltaTime;
    }

    public void Damage(float value)
    {
        if (_growing)
        {
            _hp -= value;
            _hp = Mathf.Clamp(_hp, 0f, _hpMax);
            _hpFill.fillAmount = _hp / _hpMax;
            if (_hp == 0f) { Die(); }
        }
    }

    private void Die()
    {
        _room.MobCount--;
        _room.MobUpdate -= MobUpdate;
        Destroy(gameObject, 0.25f);
    }
}
