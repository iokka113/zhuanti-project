using ZhuanTiNanMin.FSMachine;
using ZhuanTiNanMin.Mathematics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCtrl : RoleCtrl
{
    [SerializeField]
    private float _randomMoveRadius = 2f;
    private Vector2 _randomMovePos;
    private float _moveTimer;

    protected override void Awake()
    {
        _fsm = new FSMachine(this, new StateEnemyPatrol());
        base.Awake();
        _hp = _hpMax;
    }

    protected override void Update()
    {
        base.Update();
        if(Time.time > _moveTimer)
        {
            _moveTimer = Time.time + Random.Range(1f, 3f);
            _randomMovePos = Funclib.RandomInsideCircle(transform.position, _randomMoveRadius);
        }
        if (Time.time > _attackRareTime)
        {
            AttackRare = false;
        }
    }

    public void Move(int mode)
    {
        if (mode == 0)
        {
            if (Vector2.Distance(transform.position, _randomMovePos) < 0.1f)
            {
                PlayAnime(AnimeType.Idle);
            }
            else
            {
                PlayAnime(AnimeType.Run);
                Vector2 dir = (_randomMovePos - (Vector2)transform.position).normalized;
                transform.Translate(dir.x * Time.deltaTime * _moveSpeed, dir.y * Time.deltaTime * _moveSpeed, 0f);
            }
        }
        if (mode == 1)
        {
            if (Vector2.Distance(PlayerCtrl.Instance.transform.position, transform.position) < 1f)
            {
                PlayAnime(AnimeType.Idle);
            }
            else
            {
                PlayAnime(AnimeType.Run);
                Vector2 dir = (PlayerCtrl.Instance.transform.position - transform.position).normalized;
                transform.Translate(dir.x * Time.deltaTime * _moveSpeed, dir.y * Time.deltaTime * _moveSpeed, 0f);
            }
        }
        if (mode == 2)
        {
            PlayAnime(AnimeType.Run);
            Vector2 dir = -(PlayerCtrl.Instance.transform.position - transform.position).normalized;
            transform.Translate(dir.x * Time.deltaTime * _moveSpeed, dir.y * Time.deltaTime * _moveSpeed, 0f);
        }
    }

    //case State.Attack:
    //    _atkCount++;
    //    _atkCrgStarted = false;
    //    _state = State.Idle;

    //[Tooltip("備戰範圍(半徑)")]
    //private float _atkRange = 4f;

    //private bool _atkReady;

    //[SerializeField]
    //private bool _atkKeep;
    //private int _atkCount;

    //private float _atkCtdBuffer;
    //private int _atkCtdCount;
    //private bool _atkCtdStarted;

    ///// <summary>
    ///// 次數攻擊:
    ///// <br>每秒 1/n 機率觸發</br>
    ///// <br>當觸發時發動 count 次攻擊</br>
    ///// </summary>
    //private void AttackCounted(int n, int count)
    //{
    //    if (Time.time > _atkCtdBuffer)
    //    {
    //        _atkCtdBuffer = Time.time + 1f;
    //        if (!_atkCtdStarted && n != 0 && Random.Range(0, n) == 0)
    //        {
    //            _atkCtdStarted = true;
    //            _atkCtdCount = count;
    //        }
    //    }
    //    if (_atkCtdStarted)
    //    {
    //        if (_atkCount < _atkCtdCount)
    //        {
    //            //OnAttack();
    //        }
    //        else
    //        {
    //            _atkCount = 0;
    //            _atkCtdStarted = false;
    //        }
    //    }
    //}

    //private bool _atkCrgStarted;
    //private float _atkCrgTimer;
    //private float _atkCrgTime = 3f;

    //private void AttackCharge()
    //{
    //    _atkCrgTimer += Time.deltaTime;
    //    if (_atkCrgTimer > _atkCrgTime)
    //    {
    //        _atkCrgTimer = 0f;
    //        //
    //        Debug.Log("生成");
    //        _atkCrgStarted = false;
    //        //
    //    }
    //}

    //public void AttackCrgFinish()
    //{

    //}

    [SerializeField]
    private GameObject _hpBar = null;
    [SerializeField]
    private Image _hpFill = null;

    [SerializeField]
    private float _hpMax = 0;
    private float _hp = 0;

    public bool Dying { get; private set; }
    public bool AttackRare { get; private set; }
    private float _attackRareTime;

    public void DamageTsume(float value)
    {
        if (!IsDead)
        {
            PlayAnime(AnimeType.Tsume);
            Damage(value);
        }
    }

    public override void Damage(float value)
    {
        _attackRareTime = Time.time + 10f;
        AttackRare = true;
        _hp -= value;
        _hp = Mathf.Clamp(_hp, 0f, _hpMax);
        _hpFill.fillAmount = _hp / _hpMax;
        if (_hpFill.fillAmount < 0.35f) { Dying = true; }
        if (_hp == 0f) { Die(); }
    }

    [SerializeField]
    private DropInfo[] _drops = null;

    public override void Die()
    {
        base.Die();
        _hpBar.SetActive(false);
        PrefabsManager.DropSpawn(_drops, transform.position);
    }

    public override void Attack()
    {
        if (_attackType == AttackType.Sword)
        {
            PlayAnime(AnimeType.AttackSword);
            PlayAnime(AnimeType.Mushroom);
            UIManager.Instance.MainUI.PlaySound(MainUI.SoundType.AttackSword);
            AttackObjInfo info = new AttackObjInfo
            {
                Target = (Vector2)PlayerCtrl.Instance.transform.position + _colli.offset,
                TargetLayer = LayerMask.NameToLayer("Player"),
                Center = (Vector2)transform.position + _colli.offset,
                Radius = 0.75f,
                Power = _attackPower,
            };
            PrefabsManager.Instance.PoolSpawn(PoolObjType.Sword, info);
        }
        if (_attackType == AttackType.Bow)
        {
            PlayAnime(AnimeType.AttackBow);
            UIManager.Instance.MainUI.PlaySound(MainUI.SoundType.AttackBow);
            AttackObjInfo info = new AttackObjInfo
            {
                Target = (Vector2)PlayerCtrl.Instance.transform.position + _colli.offset,
                TargetLayer = LayerMask.NameToLayer("Player"),
                Center = (Vector2)transform.position + _colli.offset,
                Radius = 0.05f,
                Power = _attackPower,
                Speed = 1.5f,
            };
            PrefabsManager.Instance.PoolSpawn(PoolObjType.Bow, info);
        }
        if (_attackType == AttackType.Magic)
        {
            PlayAnime(AnimeType.AttackMagic);
            UIManager.Instance.MainUI.PlaySound(MainUI.SoundType.AttackMagic);
            AttackObjInfo info = new AttackObjInfo
            {
                Target = (Vector2)PlayerCtrl.Instance.transform.position + _colli.offset,
                TargetLayer = LayerMask.NameToLayer("Player"),
                Center = (Vector2)transform.position + _colli.offset,
                Radius = 0.2f,
                Power = _attackPower,
                Speed = 2f,
            };
            PrefabsManager.Instance.PoolSpawn(PoolObjType.Magic, info);
        }
    }
}

/// <summary>
/// 狀態[敵人巡邏]
/// </summary>
public class StateEnemyPatrol : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionSpriteFlip(),
        new ActionEnemyRandomMove(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionEnemyCheckAttackRare(),
        new DecisionCheckStun(),
        new DecisionCheckDeath(),
    };
}

/// <summary>
/// 動作[敵人隨機移動]
/// 往隨機方向移動
/// </summary>
public class ActionEnemyRandomMove : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        EnemyCtrl enemy = fsm.Controller as EnemyCtrl;
        if (enemy != null)
        {
            enemy.Move(0);
        }
    }
}

/// <summary>
/// 判斷[敵人被攻擊]
/// 如果被攻擊
/// 切換到狀態[防守]
/// </summary>
public class DecisionEnemyCheckAttackRare : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        EnemyCtrl enemy = fsm.Controller as EnemyCtrl;
        if (enemy != null && enemy.AttackRare)
        {
            return new StateEnemyDefence();
        }
        return null;
    }
}

/// <summary>
/// 狀態[敵人防守]
/// </summary>
public class StateEnemyDefence : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionEnemyTargetMove(),
        new ActionEnemyAttack(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionEnemySafeDistance(),
        new DecisionEnemyCheckHealth(),
        new DecisionCheckStun(),
        new DecisionCheckDeath(),
    };
}

/// <summary>
/// 動作[敵人目標移動]
/// 往目標移動
/// </summary>
public class ActionEnemyTargetMove : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        EnemyCtrl enemy = fsm.Controller as EnemyCtrl;
        if (enemy != null)
        {
            enemy.Move(1);
        }
    }
}

/// <summary>
/// 動作[敵人攻擊]
/// 當與目標小於一定距離
/// 往目標連續攻擊
/// </summary>
public class ActionEnemyAttack : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        EnemyCtrl enemy = fsm.Controller as EnemyCtrl;
        if (enemy != null)
        {
            if (Vector2.Distance(PlayerCtrl.Instance.transform.position, enemy.transform.position) < 3f)
            {
                enemy.Attack();
            }
        }
    }
}

/// <summary>
/// 判斷[敵人生命檢查]
/// 如果生命值過低
/// 切換到狀態[逃跑]
/// </summary>
public class DecisionEnemyCheckHealth : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        EnemyCtrl enemy = fsm.Controller as EnemyCtrl;
        if (enemy != null && enemy.Dying)
        {
            return new StateEnemyEscape();
        }
        return null;
    }
}

/// <summary>
/// 判斷[敵人安全距離]
/// 如果目標大於一定距離
/// 切換到狀態[巡邏]
/// </summary>
public class DecisionEnemySafeDistance : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        EnemyCtrl enemy = fsm.Controller as EnemyCtrl;
        if (enemy != null)
        {
            if (Vector2.Distance(PlayerCtrl.Instance.transform.position, enemy.transform.position) > 7f)
            {
                return new StateEnemyPatrol();
            }
        }
        return null;
    }
}

/// <summary>
/// 狀態[敵人追擊]
/// </summary>
public class StateEnemyChase : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionEnemyTargetMove(),
        new ActionEnemyAttack(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionCheckPlayerDead(),
        new DecisionEnemyCheckHealth(),
        new DecisionCheckStun(),
        new DecisionCheckDeath(),
    };
}

/// <summary>
/// 狀態[敵人逃跑]
/// </summary>
public class StateEnemyEscape : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionEnemyReverseMove(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionEnemySafeDistance(),
        new DecisionCheckStun(),
        new DecisionCheckDeath(),
    };
}

/// <summary>
/// 動作[敵人反目標移動]
/// 往目標反方向移動
/// </summary>
public class ActionEnemyReverseMove : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        EnemyCtrl enemy = fsm.Controller as EnemyCtrl;
        if (enemy != null)
        {
            enemy.Move(2);
        }
    }
}
