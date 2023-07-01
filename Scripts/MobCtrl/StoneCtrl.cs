using ZhuanTiNanMin.FSMachine;
using ZhuanTiNanMin.Mathematics;
using System.Collections.Generic;
using UnityEngine;

public class StoneCtrl : MobCtrl
{
    public bool AttackIsStarted { get; private set; }
    private bool _attacked;

    [SerializeField]
    private float _attackIdleTime = 2f;
    private float _attackIdleTimer;

    [SerializeField]
    private float _attackSpeed = 10f;
    private Vector2 _attackPos;

    public override void Attack()
    {
        if (!AttackIsStarted)
        {
            _attackIdleTimer += Time.deltaTime;
            if (_attackIdleTimer > _attackIdleTime)
            {
                _attackIdleTimer = 0f;
                AttackIsStarted = true;
                _attackPos = PlayerCtrl.Instance.transform.position;
            }
        }
        else
        {
            if (Vector2.Distance(_attackPos, transform.position) > 0.1f)
            {
                Vector2 dir = (_attackPos - (Vector2)transform.position).normalized;
                transform.Translate(dir * Time.deltaTime * _attackSpeed);
                if (Vector2.Distance(PlayerCtrl.Instance.transform.position, transform.position) < 0.1f)
                {
                    if (!PlayerCtrl.Instance.IsDead && !_attacked)
                    {
                        PlayerCtrl.Instance.Damage(_attackPower);
                        _attacked = true;
                    }
                }
            }
            else
            {
                AttackIsStarted = false;
                _attacked = false;
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
    }
}

/// <summary>
/// 狀態[岩石怪巡邏]
/// </summary>
public class StateStonePatrol : StateBase
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
/// 狀態[岩石怪攻擊]
/// </summary>
public class StateStoneAttack : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionMobAttack(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionStoneNoFoundTarget(),
        new DecisionCheckPlayerDead(),
        new DecisionCheckDeath(),
    };
}

/// <summary>
/// 判斷[岩石怪未發現目標]
/// </summary>
public class DecisionStoneNoFoundTarget : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        StoneCtrl stone = fsm.Controller as StoneCtrl;
        if (stone != null && !stone.AttackIsStarted)
        {
            if (Vector2.Distance(PlayerCtrl.Instance.transform.position, stone.transform.position) > stone.rangeFindTarget)
            {
                return fsm.InitialState;
            }
        }
        return null;
    }
}
