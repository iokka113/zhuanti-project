using ZhuanTiNanMin.ObjectPool;
using ZhuanTiNanMin.Mathematics;
using UnityEngine;

public class BatObj : MonoBehaviour, IPoolObject
{
    private Vector2 _offset;
    [SerializeField]
    private float _power = 0.5f;
    [SerializeField]
    private float _speed = 5f;

    public void Init(ObjInfoBase info)
    {
        PositionOnlyObjInfo i = info as PositionOnlyObjInfo;
        transform.position = i.Center;
    }

    [SerializeField]
    private GameObject _body = null;
    private float _biteCD;
    private Vector2 _dir;
    private float _dirCD;

    private void Start()
    {
        _offset = PlayerCtrl.Instance.gameObject.GetComponent<BoxCollider2D>().offset;
    }

    private void Update()
    {
        Funclib.AxisFlip(_body.transform, Axis.X, PlayerCtrl.Instance.transform.position.x - transform.position.x, true);
        if (Time.time > _dirCD)
        {
            _dirCD = Time.time + Random.Range(0f, 0.5f);
            _dir = (Vector2)(PlayerCtrl.Instance.transform.position - transform.position) + _offset;
            _dir.Normalize();
            _dir.x += Mathf.Sign(_dir.x) * Random.Range(0f, 1f);
            _dir.y += Mathf.Sign(_dir.y) * Random.Range(0f, 1f);
            _dir.Normalize();
        }
        transform.position += new Vector3(_dir.x * Time.deltaTime * _speed, _dir.y * Time.deltaTime * _speed, 0f);
        if (Vector3.Distance(PlayerCtrl.Instance.transform.position + (Vector3)_offset, transform.position) < 0.3f)
        {
            if (Time.time > _biteCD && !PlayerCtrl.Instance.IsDead)
            {
                _biteCD = Time.time + 1f;
                PlayerCtrl.Instance.DamageBite(_power);
            }
        }
    }

    public void Damage()
    {
        PrefabsManager.Instance.PoolRecycle(PoolObjType.Bat, gameObject);
    }
}
