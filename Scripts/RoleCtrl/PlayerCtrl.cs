using ZhuanTiNanMin.Singleton;
using ZhuanTiNanMin.FSMachine;
using ZhuanTiNanMin.Mathematics;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : RoleCtrl
{
    public static PlayerCtrl Instance { get => Singleton<PlayerCtrl>.Instance; }

    //[System.Serializable]
    //public class KeyCode
    //{
    //    [Tooltip("拾取道具")]
    //    public UnityEngine.KeyCode PickUp;
    //    [Tooltip("切換武器")]
    //    public UnityEngine.KeyCode Weapon;

    //    public UnityEngine.KeyCode SkillA;
    //    public UnityEngine.KeyCode SkillB;
    //    public UnityEngine.KeyCode SkillC;
    //    public UnityEngine.KeyCode SkillD;
    //}

    //public KeyCode keyCode;

    protected override void Awake()
    {
        Singleton<PlayerCtrl>.Instance = this;
        _fsm = new FSMachine(this, new StatePlayerIdle());
        base.Awake();
        if (Instance._attackType == AttackType.Sword) { UIManager.Instance.MainUI.HideMpSlider(); }
        //_weaponRightRen = _weaponRight.GetComponent<SpriteRenderer>();
        //_weaponLeftRen = _weaponLeft.GetComponent<SpriteRenderer>();
    }

    //[SerializeField]
    //private GameObject _weaponRight;
    //private SpriteRenderer _weaponRightRen;
    //[SerializeField]
    //private GameObject _weaponLeft;
    //private SpriteRenderer _weaponLeftRen;

    private Vector2 _mouseClickPos;

    private float _knightAttackCD;

    private float _archerMpHealingCD;
    private bool _archerMpHealingCheck;
    private float _archerMpHealingCheckCD;

    protected override void Update()
    {
        base.Update();
        if (Input.GetMouseButtonUp(0)) { _mouseClickPos = Input.mousePosition; }
        if (Key != null) { Key.transform.position = transform.position + _keyOffset; }
        if (Input.GetMouseButtonUp(2)) { InventoryManager.Instance.PlayerHealingHP(); }
        if (_archerMpHealingCheck)
        {
            if (Time.time > _archerMpHealingCheckCD)
            {
                _archerMpHealingCheck = false;
            }
        }
        else
        {
            if (Time.time > _archerMpHealingCD)
            {
                _archerMpHealingCD = Time.time + 0.2f;
                InventoryManager.Instance.PlayerHealingMP(1f);
            }
        }
        if (!IsDead && Input.GetKeyDown(KeyCode.F))
        {
            if (_treasureOpenReady != null)
            {
                //有75%機率找到藥水
                int i = Random.Range(0, 4);
                //如果沒找到
                if (i == 0)
                {
                    UIManager.Instance.MainUI.TutorialTextUpdate(Funclib.AddColorToString("沒有找到任何物品…", Color.yellow));
                    Destroy(_treasureOpenReady);
                }
                //如果找到
                else
                {
                    //隨機獲得1~3瓶藥水
                    int j = Random.Range(1, 4);
                    InventoryManager.Instance.PotionHpGET(j);
                    Destroy(_treasureOpenReady);
                }
            }
        }
        if (IsDead && Input.GetKeyDown(KeyCode.Escape)) { UIManager.Instance.OnButtonClick(UIButtonType.BackToHome); }
        //if (!_weaponSwitched) { MainUI.Instance.DisplayWeapon(_weaponMajor, _weaponMinor); }
        //else { MainUI.Instance.DisplayWeapon(_weaponMinor, _weaponMajor); }
        //if (!IsDead)
        //{
        //    if (_prePickUp != null && Input.GetKeyUp(keyCode.PickUp))
        //    {
        //        MainUI.Instance.PickUp(_prePickUp);
        //    }
        //    if (Input.GetKeyUp(keyCode.Weapon))
        //    {
        //        WeaponSwitch();
        //    }
        //    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        //    {
        //        MainUI.Instance.Use("PotionHP");
        //    }
        //    if (Input.GetKeyUp(keyCode.SkillA))
        //    {
        //        MainUI.Instance.Use("Skill_0");
        //    }
        //    if (Input.GetKeyUp(keyCode.SkillB))
        //    {
        //        MainUI.Instance.Use("Skill_1");
        //    }
        //}
        //if (Input.GetKeyDown(UnityEngine.KeyCode.Keypad1))
        //{
        //    PositionOnlyObjInfo info = new PositionOnlyObjInfo
        //    {
        //        Center = Funclib.RandomInsideCircle((Vector2)transform.position + _colli.offset, 1f)
        //    };
        //    PrefabsManager.Instance.PoolSpawn(PoolObjType.Bat, info);
        //}
        //if (Input.GetKeyDown(UnityEngine.KeyCode.Keypad2))
        //{
        //    PositionOnlyObjInfo info = new PositionOnlyObjInfo
        //    {
        //        Center = (Vector2)transform.position + _colli.offset
        //    };
        //    PrefabsManager.Instance.PoolSpawn(PoolObjType.Range, info, 5);
        //}
    }

    public override void Attack()
    {
        if (_attackType == AttackType.Sword)
        {
            if (Time.time > _knightAttackCD)
            {
                PlayAnime(AnimeType.AttackSword);
                PlayAnime(AnimeType.Mushroom);
                UIManager.Instance.MainUI.PlaySound(MainUI.SoundType.AttackSword);
                AttackObjInfo info = new AttackObjInfo
                {
                    Target = Camera.main.ScreenToWorldPoint(_mouseClickPos),
                    TargetLayer = LayerMask.NameToLayer("Enemy"),
                    Center = (Vector2)transform.position + _colli.offset,
                    Radius = 0.75f,
                    Power = _attackPower,
                };
                PrefabsManager.Instance.PoolSpawn(PoolObjType.Sword, info);

                _knightAttackCD = Time.time + 1f / 3f;
            }
        }
        if (_attackType == AttackType.Bow)
        {
            if (InventoryManager.Instance.GetCurrentMP() > 0f)
            {
                PlayAnime(AnimeType.AttackBow);
                UIManager.Instance.MainUI.PlaySound(MainUI.SoundType.AttackBow);
                AttackObjInfo info = new AttackObjInfo
                {
                    Target = Camera.main.ScreenToWorldPoint(_mouseClickPos),
                    TargetLayer = LayerMask.NameToLayer("Enemy"),
                    Center = (Vector2)transform.position + _colli.offset,
                    Radius = 0.05f,
                    Power = _attackPower,
                    Speed = 3f,
                };
                PrefabsManager.Instance.PoolSpawn(PoolObjType.Bow, info);

                InventoryManager.Instance.PlayerDamageMP(1f);
                _archerMpHealingCheck = true;
                _archerMpHealingCheckCD = Time.time + 1f;
            }
            else
            {
                UIManager.Instance.MainUI.TutorialTextUpdate(Funclib.AddColorToString("沒有弓箭了…", Color.yellow));
            }
        }
        if (_attackType == AttackType.Magic)
        {
            PlayAnime(AnimeType.AttackMagic);
            UIManager.Instance.MainUI.PlaySound(MainUI.SoundType.AttackMagic);
            AttackObjInfo info = new AttackObjInfo
            {
                Target = Camera.main.ScreenToWorldPoint(_mouseClickPos),
                TargetLayer = LayerMask.NameToLayer("Enemy"),
                Center = (Vector2)transform.position + _colli.offset,
                Radius = 0.2f,
                Power = _attackPower,
                Speed = 2f,
            };
            PrefabsManager.Instance.PoolSpawn(PoolObjType.Magic, info);
        }
    }

    public override void Damage(float value)
    {
        UIManager.Instance.MainUI.PlaySound(MainUI.SoundType.AttackRare);
        InventoryManager.Instance.PlayerDamageHP(value);
    }

    public void DamageBite(float value)
    {
        if (!IsDead)
        {
            PlayAnime(AnimeType.BatBite);
            Damage(value);
        }
    }

    public void Run()
    {
        PlayAnime(AnimeType.Run);
        float x = Time.deltaTime * _moveSpeed * Input.GetAxis("Horizontal");
        float y = Time.deltaTime * _moveSpeed * Input.GetAxis("Vertical");
        transform.Translate(x, y, 0f);
    }



    public Key Key { get; set; }
    [SerializeField]
    private Vector3 _keyOffset = new Vector3(0f, 1.2f, 0f);

    //public GameObject[] clothes;


    //[SerializeField]
    //private Sprite _weaponMajor;
    //[SerializeField]
    //private Sprite _weaponMinor;

    //private bool _weaponSwitched;

    //private void WeaponSwitch()
    //{
    //    _weaponSwitched = !_weaponSwitched;
    //    PlaySound(SoundType.SwitchWeapon);
    //    if (!_weaponSwitched)
    //    {
    //        _weaponRightRen.sprite = _weaponMajor;
    //        _attackType = AttackType.Sword;
    //    }
    //    else
    //    {
    //        _weaponRightRen.sprite = _weaponMinor;
    //        _attackType = AttackType.Magic;
    //    }
    //}

    //[SerializeField]
    //private GameObject _pickUpTooltip;
    //private GameObject _prePickUp;

    private GameObject _treasureOpenReady;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Treasure"))
        {
            _treasureOpenReady = collision.gameObject;
            UIManager.Instance.MainUI.TutorialTextUpdate(Funclib.AddColorToString("發現寶箱！！！點撃F鍵打開…", Color.blue));
        }

        //if (collision.CompareTag("Drop"))
        //{
        //    _pickUpTooltip.SetActive(true);
        //    _prePickUp = collision.gameObject;
        //}
        //if (collision.CompareTag("PotionHP"))
        //{
        //    //MainUI.Instance.Receive("PotionHP");
        //    Destroy(collision.gameObject);
        //}
        //if (collision.CompareTag("Skill_0"))
        //{
        //    //MainUI.Instance.Receive("Skill_0");
        //    Destroy(collision.gameObject);
        //}
        //if (collision.CompareTag("Skill_1"))
        //{
        //    //MainUI.Instance.Receive("Skill_1");
        //    Destroy(collision.gameObject);
        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Treasure"))
        {
            _treasureOpenReady = null;
        }

        //if (collision.CompareTag("Drop"))
        //{
        //    _pickUpTooltip.SetActive(false);
        //    _prePickUp = null;
        //}
        //if (collision.CompareTag("Door"))
        //{
        //    if (transform.position.x > _enterPos.x)
        //    {
        //        collision.isTrigger = false;
        //        Destroy(Key.gameObject);
        //        Key = null;
        //    }
        //}
    }

    //private Vector3 _enterPos;

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    //if (collision.collider.CompareTag("Door"))
    //    //{
    //    //    if (Key != null)
    //    //    {
    //    //        _enterPos = transform.position;
    //    //        collision.collider.isTrigger = true;
    //    //    }
    //    //}
    //}

    //public void SkillGo(int skill)
    //{
    //    if (skill == 0)
    //    {
    //        PlayAnime(AnimeType.HawkEyes);
    //    }
    //    if (skill == 1)
    //    {
    //        List<Collider2D> colli = new List<Collider2D>();
    //        ContactFilter2D filter = new ContactFilter2D();
    //        filter.NoFilter();
    //        if (Physics2D.OverlapCircle((Vector2)transform.position + _colli.offset, 2f, filter, colli) > 0)
    //        {
    //            foreach (Collider2D col in colli)
    //            {
    //                if (col.gameObject.TryGetComponent(out EnemyCtrl mob))
    //                {
    //                    mob.DamageTsume(7f);
    //                }
    //            }
    //        }
    //    }
    //}
}

/// <summary>
/// 判斷[玩家死亡]
/// </summary>
public class DecisionCheckPlayerDead : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        EnemyCtrl enemy = fsm.Controller as EnemyCtrl;
        if (enemy != null && PlayerCtrl.Instance.IsDead)
        {
            return fsm.InitialState;
        }
        MobCtrl mob = fsm.Controller as MobCtrl;
        if (mob != null && PlayerCtrl.Instance.IsDead)
        {
            return fsm.InitialState;
        }
        return null;
    }
}

/// <summary>
/// 狀態[玩家待機]
/// </summary>
public class StatePlayerIdle : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionSpriteFlip(),
        new ActionPlayerIdle(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionPlayerCheckInput(),
        new DecisionCheckStun(),
        new DecisionCheckDeath(),
    };
}

/// <summary>
/// 動作[玩家待機]
/// </summary>
public class ActionPlayerIdle : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        PlayerCtrl player = fsm.Controller as PlayerCtrl;
        if (player != null)
        {
            player.PlayAnime(RoleCtrl.AnimeType.Idle);
        }
    }
}

/// <summary>
/// 判斷[玩家回到待機]
/// </summary>
public class DecisionPlayerGotoIdle : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        PlayerCtrl player = fsm.Controller as PlayerCtrl;
        if (player != null)
        {
            return new StatePlayerIdle();
        }
        return null;
    }
}

/// <summary>
/// 判斷[檢查玩家輸入]
/// </summary>
public class DecisionPlayerCheckInput : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        PlayerCtrl player = fsm.Controller as PlayerCtrl;
        if (player != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                return new StatePlayerAttack();
            }
            else if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
            {
                return new StatePlayerRun();
            }
            else
            {
                return new StatePlayerIdle();
            }
        }
        return null;
    }
}

/// <summary>
/// 狀態[玩家跑步]
/// </summary>
public class StatePlayerRun : StateBase
{
    public override void OnStateEnter(FSMachine fsm)
    {
        base.OnStateEnter(fsm);
        PlayerCtrl player = fsm.Controller as PlayerCtrl;
        if (player != null)
        {
            UIManager.Instance.MainUI.PlayRunningSound(true);
        }
    }

    public override void OnStateExit(FSMachine fsm)
    {
        base.OnStateExit(fsm);
        PlayerCtrl player = fsm.Controller as PlayerCtrl;
        if (player != null)
        {
            UIManager.Instance.MainUI.PlayRunningSound(false);
        }
    }

    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionSpriteFlip(),
        new ActionPlayerRun(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionPlayerCheckInput(),
        new DecisionCheckStun(),
        new DecisionCheckDeath(),
    };
}

/// <summary>
/// 動作[玩家跑步]
/// </summary>
public class ActionPlayerRun : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        PlayerCtrl player = fsm.Controller as PlayerCtrl;
        if (player != null)
        {
            player.Run();
        }
    }
}

/// <summary>
/// 狀態[玩家攻擊]
/// </summary>
public class StatePlayerAttack : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionPlayerAttack(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionPlayerGotoIdle(),
    };
}

/// <summary>
/// 動作[玩家攻擊]
/// </summary>
public class ActionPlayerAttack : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        PlayerCtrl player = fsm.Controller as PlayerCtrl;
        if (player != null)
        {
            player.Attack();
        }
    }
}
