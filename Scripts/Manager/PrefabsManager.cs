using ZhuanTiNanMin.Singleton;
using ZhuanTiNanMin.ObjectPool;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsManager : MonoBehaviour, IManager
{
    public static PrefabsManager Instance => Singleton<PrefabsManager>.Instance;

    public bool InitHasFinished { get; private set; }

    private void Awake()
    {
        Singleton<PrefabsManager>.Instance = this;
        DontDestroyOnLoad(Instance.gameObject);
        _poolDict.Add(PoolObjType.Sword, Instance._sword);
        _poolDict.Add(PoolObjType.Bow, Instance._bow);
        _poolDict.Add(PoolObjType.Magic, Instance._magic);
        _poolDict.Add(PoolObjType.Bat, Instance._bat);
        _poolDict.Add(PoolObjType.Range, Instance._range);
        _poolDict.Add(PoolObjType.Venom, Instance._venom);
        InitHasFinished = true;
    }

    public void OnSceneLevelMapLoaded()
    {
        PoolClearCache();
    }

    #region Pool

    [Header("Object Pool Prefabs")]

    [SerializeField]
    private PoolIDInfo _sword = null;
    [SerializeField]
    private PoolIDInfo _bow = null;
    [SerializeField]
    private PoolIDInfo _magic = null;
    [SerializeField]
    private PoolIDInfo _bat = null;
    [SerializeField]
    private PoolIDInfo _range = null;
    [SerializeField]
    private PoolIDInfo _venom = null;

    private readonly Dictionary<PoolObjType, PoolIDInfo> _poolDict = new Dictionary<PoolObjType, PoolIDInfo>();

    public void PoolSpawn(PoolObjType type, ObjInfoBase info, int count = 1)
    {
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                ObjectPool.Spawn(_poolDict[type], info);
            }
        }
    }

    public void PoolRecycle(PoolObjType type, GameObject gameObject)
    {
        ObjectPool.Recycle(_poolDict[type], gameObject);
    }

    private void PoolClearCache()
    {
        Instance._sword.Pool.Clear();
        Instance._bow.Pool.Clear();
        Instance._magic.Pool.Clear();
        Instance._bat.Pool.Clear();
        Instance._range.Pool.Clear();
        Instance._venom.Pool.Clear();
    }

    #endregion

    #region Mob

    [System.Serializable]
    public class MobPrefab
    {
        public GameObject slime;
        public GameObject snake;
        public GameObject mushroom;
        public GameObject spore;
        public GameObject tentacle;
        public GameObject bat;
        public GameObject stone;
        public GameObject skeleton;
    }

    [Header("Mob Prefab")]

    [SerializeField]
    private MobPrefab _mobPrefab = null;

    public GameObject GetMobPrefab(MobType type)
    {
        if (type == MobType.Slime) { return _mobPrefab.slime; }
        if (type == MobType.Snake) { return _mobPrefab.snake; }
        if (type == MobType.Mushroom) { return _mobPrefab.mushroom; }
        if (type == MobType.Spore) { return _mobPrefab.spore; }
        if (type == MobType.Tentacle) { return _mobPrefab.tentacle; }
        if (type == MobType.Bat) { return _mobPrefab.bat; }
        if (type == MobType.Stone) { return _mobPrefab.stone; }
        if (type == MobType.Skeleton) { return _mobPrefab.skeleton; }
        return null;
    }

    [Header("Mob Skin")]

    [SerializeField]
    private GameObject _bodyCanvas = null;

    [SerializeField]
    private List<GameObject> _skinSlime = null;
    [SerializeField]
    private List<GameObject> _skinSnake = null;
    [SerializeField]
    private List<GameObject> _skinMushroom = null;
    [SerializeField]
    private List<GameObject> _skinTentacle = null;
    [SerializeField]
    private List<GameObject> _skinBat = null;
    [SerializeField]
    private List<GameObject> _skinStone = null;
    [SerializeField]
    private List<GameObject> _skinSkeleton = null;

    public GameObject GetMobBody(MobCtrl mob)
    {
        System.Type t = mob.GetType();
        GameObject skin = null;
        if (t.Equals(typeof(SlimeCtrl))) { skin = RandomSkin(_skinSlime); }
        else if (t.Equals(typeof(SnakeCtrl))) { skin = RandomSkin(_skinSnake); }
        else if (t.Equals(typeof(MushroomCtrl))) { skin = RandomSkin(_skinMushroom); }
        else if (t.Equals(typeof(TentacleCtrl))) { skin = RandomSkin(_skinTentacle); }
        else if (t.Equals(typeof(BatCtrl))) { skin = RandomSkin(_skinBat); }
        else if (t.Equals(typeof(StoneCtrl))) { skin = RandomSkin(_skinStone); }
        else if (t.Equals(typeof(SkeletonCtrl))) { skin = RandomSkin(_skinSkeleton); }
        GameObject body = Instantiate(_bodyCanvas);
        if (skin != null)
        {
            GameObject newSkin = Instantiate(skin);
            newSkin.transform.SetParent(body.transform, false);
        }
        return body;
    }

    private GameObject RandomSkin(List<GameObject> skinList)
    {
        if (skinList != null)
        {
            return skinList[Random.Range(0, skinList.Count)];
        }
        return null;
    }

    #endregion

    #region Drop

    public static void DropSpawn(DropInfo drop, Vector3 position)
    {
        if (drop.Has)
        {
            float x = Random.Range(position.x - 0.5f, position.x + 0.5f);
            float y = Random.Range(position.y - 0.5f, position.y + 0.5f);
            Instantiate(drop.Prefab, new Vector3(x, y, 0f), Quaternion.identity);
        }
    }

    public static void DropSpawn(DropInfo[] drops, Vector3 position)
    {
        for (int i = 0; i < drops.Length; i++)
        {
            DropSpawn(drops[i], position);
        }
    }

    #endregion

    [Header("Prop")]

    [SerializeField]
    private Sprite[] _propPic = null;

    public Sprite PropGetPic(PropType type)
    {
        int no = type.GetHashCode();
        if (no < 0 || no >= _propPic.Length) { return null; }
        else { return _propPic[no]; }
    }
}

public enum MobType
{
    Slime,
    Snake,
    Mushroom,
    Spore,
    Tentacle,
    Bat,
    Stone,
    Skeleton,
}

public enum PoolObjType
{
    Sword,
    Bow,
    Magic,
    Bat,
    Range,
    Venom,
}

public class AttackObjInfo : ObjInfoBase
{
    public Vector2 Center { get; set; }
    public float Radius { get; set; } = 0.5f;
    public float Speed { get; set; } = 1f;
    public float Power { get; set; } = 1f;
    public Vector2 Target { get; set; }
    public LayerMask TargetLayer { get; set; }
    public BuffType Buff { get; set; } = BuffType.None;
}

public class PositionOnlyObjInfo : ObjInfoBase
{
    public Vector2 Center { get; set; }
}

[System.Serializable]
public class DropInfo
{
    public GameObject Prefab;
    public bool Has;
}

public enum PropType
{
    VaseNormal,
    VaseBroken,
    Roomkey,
}
