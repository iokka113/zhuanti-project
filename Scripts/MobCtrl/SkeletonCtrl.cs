using ZhuanTiNanMin.FSMachine;
using ZhuanTiNanMin.Mathematics;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonCtrl : MobCtrl
{
    public bool AttackIsStarted { get; private set; }

    public float rangeTargetInAttackFar;
    public float rangeTargetInAttackNear;

    public override void Attack()
    {
        return;
    }

    [SerializeField]
    private float _attackFarCD = 4f;
    private float _attackFarCDTimer;

    public void AttackFar()
    {
        _attackFarCDTimer += Time.deltaTime;
        if (_attackFarCDTimer > _attackFarCD)
        {
            _attackFarCDTimer = 0f;
            FarGo();
        }
    }
    
    public void FarGo()
    {
        AttackObjInfo info = new AttackObjInfo
        {
            Target = PlayerCtrl.Instance.transform.position,
            TargetLayer = LayerMask.NameToLayer("Player"),
            Center = transform.position,
            Radius = 0.2f,
            Power = _attackPower,
            Speed = 2f,
        };
        PrefabsManager.Instance.PoolSpawn(PoolObjType.Magic, info);
    }

    [SerializeField]
    private float _attackNearIdleTime = 0.4f;
    private float _attackNearIdleTimer;
    [SerializeField]
    private float _attackNearTime = 0.2f;
    private float _attackNearTimer;
    [SerializeField]
    private float _attackNearSpeed = 0f;
    private Vector2 _attackDir;
    private bool _attacked;

    public void AttackNear()
    {
        if (!AttackIsStarted)
        {
            _attackNearIdleTimer += Time.deltaTime;
            if (_attackNearIdleTimer > _attackNearIdleTime)
            {
                _attackNearIdleTimer = 0f;
                AttackIsStarted = true;
                _attackDir = (PlayerCtrl.Instance.transform.position - transform.position).normalized;
            }
        }
        else
        {
            _attackNearTimer += Time.deltaTime;
            if (_attackNearTimer > _attackNearTime)
            {
                _attackNearTimer = 0f;
                AttackIsStarted = false;
                _attacked = false;
            }
            else
            {
                transform.Translate(Time.deltaTime * _attackDir.x * _attackNearSpeed, Time.deltaTime * _attackDir.y * _attackNearSpeed, 0f);
                if (Vector2.Distance(PlayerCtrl.Instance.transform.position, transform.position) < 0.1f)
                {
                    if (!PlayerCtrl.Instance.IsDead && !_attacked)
                    {
                        PlayerCtrl.Instance.Damage(_attackPower);
                        _attacked = true;
                    }
                }
            }
        }
    }

    public override void Move(int mode)
    {
        if (mode == 0)
        {
            if (Vector2.Distance(_randomMovePos, transform.position) > 0.01f)
            {
                Vector2 dir = (_randomMovePos - (Vector2)transform.position).normalized;
                transform.Translate(Time.deltaTime * _moveSpeed * dir.x, Time.deltaTime * _moveSpeed * dir.y, 0f);
            }
            else
            {
                NewRandomMovePos();
            }
        }
        if (mode == 1)
        {
            if (Vector2.Distance(PlayerCtrl.Instance.transform.position, transform.position) > 0.01f)
            {
                Vector2 dir = (PlayerCtrl.Instance.transform.position - transform.position).normalized;
                transform.Translate(Time.deltaTime * _moveSpeed * dir.x, Time.deltaTime * _moveSpeed * dir.y, 0f);
            }
        }
    }

    private readonly NonRepeating _relife = new NonRepeating(4);

    public override void Die()
    {
        if (_relife.NextValue == 0)
        {
            HpColorChange(new Color32(128, 0, 128, 255));
            _hp = _hpMax;
        }
        else
        {
            base.Die();
        }
    }
}

/// <summary>
/// 狀態[骷髏巡邏]
/// </summary>
public class StateSkeletonPatrol : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionHealing(),
        new ActionMobRandomMove(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionMobFoundTargetOrAttackRare(),
        new DecisionCheckPlayerDead(),
        new DecisionCheckDeath(),
    };
}

/// <summary>
/// 狀態[骷髏追擊]
/// </summary>
public class StateSkeletonChase : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionMobTargetMove(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionSkeletonTargetInRangeNear(),
        new DecisionSkeletonTargetInRangeFar(),
        new DecisionSkeletonNoFoundTarget(),
        new DecisionCheckPlayerDead(),
        new DecisionCheckDeath(),
    };
}

/// <summary>
/// 狀態[骷髏遠程攻擊]
/// </summary>
public class StateSkeletonAttackFar : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionSkeletonAttackFar(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionSkeletonTargetInRangeNear(),
        new DecisionSkeletonTargetIsntInRangeFar(),
        new DecisionSkeletonNoFoundTarget(),
        new DecisionCheckPlayerDead(),
        new DecisionCheckDeath(),
    };
}

/// <summary>
/// 狀態[骷髏近戰攻擊]
/// </summary>
public class StateSkeletonAttackNear : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionSkeletonAttackNear(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionSkeletonTargetInRangeFar(),
        new DecisionSkeletonNoFoundTarget(),
        new DecisionCheckPlayerDead(),
        new DecisionCheckDeath(),
    };
}

/// <summary>
/// 判斷[骷髏未發現目標]
/// </summary>
public class DecisionSkeletonNoFoundTarget : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        SkeletonCtrl skeleton = fsm.Controller as SkeletonCtrl;
        if (skeleton != null && !skeleton.AttackIsStarted)
        {
            if (Vector2.Distance(PlayerCtrl.Instance.transform.position, skeleton.transform.position) > skeleton.rangeFindTarget)
            {
                return fsm.InitialState;
            }
        }
        return null;
    }
}

/// <summary>
/// 動作[骷髏遠程攻擊]
/// </summary>
public class ActionSkeletonAttackFar : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        SkeletonCtrl skeleton = fsm.Controller as SkeletonCtrl;
        if (skeleton != null)
        {
            skeleton.AttackFar();
        }
    }
}

/// <summary>
/// 動作[骷髏近戰攻擊]
/// </summary>
public class ActionSkeletonAttackNear : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        SkeletonCtrl skeleton = fsm.Controller as SkeletonCtrl;
        if (skeleton != null)
        {
            skeleton.AttackNear();
        }
    }
}

/// <summary>
/// 判斷[骷髏在遠程攻擊範圍內]
/// </summary>
public class DecisionSkeletonTargetInRangeFar : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        SkeletonCtrl skeleton = fsm.Controller as SkeletonCtrl;
        if (skeleton != null && !skeleton.AttackIsStarted)
        {
            if (Vector2.Distance(PlayerCtrl.Instance.transform.position, skeleton.transform.position) < skeleton.rangeTargetInAttackFar)
            {
                return new StateSkeletonAttackFar();
            }
        }
        return null;
    }
}

/// <summary>
/// 判斷[骷髏在近戰攻擊範圍內]
/// </summary>
public class DecisionSkeletonTargetInRangeNear : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        SkeletonCtrl skeleton = fsm.Controller as SkeletonCtrl;
        if (skeleton != null)
        {
            if (Vector2.Distance(PlayerCtrl.Instance.transform.position, skeleton.transform.position) < skeleton.rangeTargetInAttackNear)
            {
                return new StateSkeletonAttackNear();
            }
        }
        return null;
    }
}

/// <summary>
/// 判斷[骷髏不在遠程攻擊範圍內]
/// </summary>
public class DecisionSkeletonTargetIsntInRangeFar : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        SkeletonCtrl skeleton = fsm.Controller as SkeletonCtrl;
        if (skeleton != null)
        {
            if (!(Vector2.Distance(PlayerCtrl.Instance.transform.position, skeleton.transform.position) < skeleton.rangeTargetInAttackFar))
            {
                return new StateSkeletonChase();
            }
        }
        return null;
    }
}
