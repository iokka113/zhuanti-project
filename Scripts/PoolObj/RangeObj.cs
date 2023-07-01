using ZhuanTiNanMin.ObjectPool;
using ZhuanTiNanMin.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class RangeObj : MonoBehaviour, IPoolObject
{
    [SerializeField]
    private float _power = 0f;

    public void Init(ObjInfoBase info)
    {
        PositionOnlyObjInfo i = info as PositionOnlyObjInfo;
        transform.position = Funclib.RandomInsideCircle(i.Center, 2f);
        float radius = Random.Range(0.3f, 0.5f);
        transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);
    }

    [SerializeField]
    private float _time = 2f;
    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _time)
        {
            _timer = 0f;
            PrefabsManager.Instance.PoolRecycle(PoolObjType.Range, gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerCtrl.Instance.Damage(_power);
        }
    }
}
