using ZhuanTiNanMin.ObjectPool;
using ZhuanTiNanMin.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class VenomObj : MonoBehaviour, IPoolObject
{
    private Vector2 _dir;
    private float _speed;
    private float _power;
    //private bool _buff;
    public void Init(ObjInfoBase info)
    {
        AttackObjInfo i = info as AttackObjInfo;
        _power = i.Power;
        _speed = i.Speed;
        _dir = (i.Target - i.Center).normalized;
        transform.position = i.Center + _dir * 0.5f;
        transform.rotation = Funclib.LookRotation2D(i.Center, i.Target);
        transform.localScale = new Vector3(i.Radius * 2f, i.Radius * 2f, 1f);
        //if (i.Buff == BuffType.Poisoning) { _buff = true; }
        //else { _buff = false; }
    }

    protected void Recycle()
    {
        PrefabsManager.Instance.PoolRecycle(PoolObjType.Venom, gameObject);
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerCtrl.Instance.Damage(_power);
            //if (_buff) { MainUI.Instance.PoisoningCount += 3; }
            Recycle();
        }
    }
}
