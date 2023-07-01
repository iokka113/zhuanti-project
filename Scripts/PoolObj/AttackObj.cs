using ZhuanTiNanMin.ObjectPool;
using ZhuanTiNanMin.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class AttackObj : MonoBehaviour, IPoolObject
{
    [SerializeField]
    private AttackType _type = AttackType.Sword;
    private Vector2 _dir;
    private float _speed;
    private float _power;
    private LayerMask _layer;

    public void Init(ObjInfoBase info)
    {
        AttackObjInfo i = info as AttackObjInfo;
        _power = i.Power;
        _speed = i.Speed;
        _dir = (i.Target - i.Center).normalized;
        transform.position = i.Center + _dir * 0.5f;
        transform.rotation = Funclib.LookRotation2D(i.Center, i.Target);
        transform.localScale = new Vector3(i.Radius * 2f, i.Radius * 2f, 1f);
        _layer = i.TargetLayer;
    }

    protected void Recycle()
    {
        switch (_type)
        {
            case AttackType.Sword:
                PrefabsManager.Instance.PoolRecycle(PoolObjType.Sword, gameObject);
                break;
            case AttackType.Bow:
                PrefabsManager.Instance.PoolRecycle(PoolObjType.Bow, gameObject);
                break;
            case AttackType.Magic:
                PrefabsManager.Instance.PoolRecycle(PoolObjType.Magic, gameObject);
                break;
            default:
                return;
        }
    }

    protected virtual void Update()
    {
        transform.position += (Vector3)_dir * Time.deltaTime * _speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Recycle();
        }
        if (collision.gameObject.layer == _layer && collision.gameObject.TryGetComponent(out RoleCtrl role))
        {
            role.Damage(_power);
            Recycle();
        }
        if (_layer == LayerMask.NameToLayer("Enemy"))
        {
            if(collision.gameObject.TryGetComponent(out MobCtrl mob))
            {
                mob.Damage(_power);
                Recycle();
            }
            if (collision.gameObject.TryGetComponent(out Vase vase))
            {
                vase.Damage();
            }
            if (collision.gameObject.TryGetComponent(out BatObj bat))
            {
                bat.Damage();
            }
            if (collision.gameObject.TryGetComponent(out Spore spore))
            {
                spore.Damage(_power);
            }
        }
    }
}

public enum AttackType
{
    Sword,
    Bow,
    Magic,
}
