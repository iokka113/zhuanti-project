using ZhuanTiNanMin.Mathematics;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Room : MonoBehaviour
{
    public CircleCollider2D Colli { get; private set; }

    [SerializeField]
    private List<RoomTrigger> _triggers = null;

    [SerializeField]
    private List<RoomDoor> _doors = null;

    private void Awake()
    {
        Colli = GetComponent<CircleCollider2D>();
        Colli.isTrigger = true;
        foreach (RoomTrigger trigger in _triggers)
        {
            trigger.Room = this;
            trigger.Colli = trigger.GetComponent<BoxCollider2D>();
            trigger.Colli.isTrigger = true;
        }
        foreach (RoomDoor door in _doors)
        {
            door.Colli = door.GetComponent<BoxCollider2D>();
            door.Colli.isTrigger = true;
            door.Pic = door.GetComponentInChildren<SpriteRenderer>();
            door.Pic.enabled = false;
        }
    }

    [SerializeField]
    private bool _lockable = false;

    public void OnPlayerEnter()
    {
        foreach (RoomTrigger trigger in _triggers)
        {
            trigger.gameObject.SetActive(false);
        }
        SpawnListMobs();
    }

    [SerializeField]
    private List<MobType> _mobs = null;

    private void SpawnListMobs()
    {
        if (_mobs != null)
        {
            foreach (MobType type in _mobs)
            {
                Instantiate(PrefabsManager.Instance.GetMobPrefab(type)).GetComponent<MobCtrl>().Init(this, Funclib.RandomInsideCircle(transform.position, Colli.radius));
            }
            UIManager.Instance.MainUI.TutorialTextUpdate(Funclib.AddColorToString("怪物出現了！！點撃滑鼠左鍵撃退它…", Color.blue));
            if (_lockable) { DoorLock(true); }
        }
    }

    private int _mobCount;

    public int MobCount
    {
        get { return _mobCount; }
        set
        {
            _mobCount = value;
            if (_lockable && _mobCount <= 0)
            {
                DoorLock(false);
            }
        }
    }

    /// <summary>
    /// 窄口上鎖
    /// <br>isLocking = true 上鎖</br>
    /// <br>isLocking = false 解鎖</br>
    /// </summary>
    private void DoorLock(bool isLocking)
    {
        if (isLocking)
        {
            foreach (RoomDoor door in _doors)
            {
                door.Colli.isTrigger = false;
                door.Pic.enabled = true;
            }
            UIManager.Instance.MainUI.PlaySound(MainUI.SoundType.DoorLock);
            UIManager.Instance.MainUI.TutorialTextUpdate("路口被封住了…試試消滅地牢中的怪物。");
        }
        else
        {
            foreach (RoomDoor door in _doors)
            {
                door.Colli.isTrigger = true;
                door.Pic.enabled = false;
            }
            UIManager.Instance.MainUI.PlaySound(MainUI.SoundType.DoorOpen);
            UIManager.Instance.MainUI.TutorialTextUpdate(Funclib.AddColorToString("路口被打開了！", Color.green));
        }
    }

    public System.Action MobUpdate { get; set; }

    private void Update()
    {
        MobUpdate?.Invoke();
    }
}
