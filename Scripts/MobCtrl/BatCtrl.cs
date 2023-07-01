using ZhuanTiNanMin.FSMachine;
using System.Collections.Generic;
using UnityEngine;

public class BatCtrl : MobCtrl
{
    [SerializeField]
    private float _attackIdleTime = 0.3f;
    private float _attackIdleTimer;
    //[SerializeField]
    private float _attackTime = 0.1f;
    private float _attackTimer;
    [SerializeField]
    private float _attackSpeed = 20f;
    private Vector2 _attackDir;

    public bool AttackIsStarted { get; private set; }
    private bool _attacked;

    public override void Attack()
    {
        if (!AttackIsStarted)
        {
            _attackIdleTimer += Time.deltaTime;
            if (_attackIdleTimer > _attackIdleTime)
            {
                _attackIdleTimer = 0f;
                AttackIsStarted = true;
                _attackDir = (PlayerCtrl.Instance.transform.position - transform.position).normalized;
            }
        }
        else
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer > _attackTime)
            {
                _attackTimer = 0f;
                AttackIsStarted = false;
                _attacked = false;
            }
            transform.Translate(Time.deltaTime * _attackDir.x * _attackSpeed, Time.deltaTime * _attackDir.y * _attackSpeed, 0f);
            if (Vector2.Distance(PlayerCtrl.Instance.transform.position, transform.position) < 0.1f)
            {
                if (!PlayerCtrl.Instance.IsDead && !_attacked)
                {
                    PlayerCtrl.Instance.Damage(_attackPower);
                    Damage(_hpMax / -6f);
                    _attacked = true;
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
    }
}

/// <summary>
/// 狀態[蝙蝠巡邏]
/// </summary>
public class StateBatPatrol : StateBase
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
/// 狀態[蝙蝠攻擊]
/// </summary>
public class StateBatAttack : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionMobAttack(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionBatNoFoundTarget(),
        new DecisionCheckPlayerDead(),
        new DecisionCheckDeath(),
    };
}

/// <summary>
/// 判斷[蝙蝠未發現目標]
/// </summary>
public class DecisionBatNoFoundTarget : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        BatCtrl bat = fsm.Controller as BatCtrl;
        if (bat != null && !bat.AttackIsStarted)
        {
            if (Vector2.Distance(PlayerCtrl.Instance.transform.position, bat.transform.position) > bat.rangeFindTarget)
            {
                return fsm.InitialState;
            }
        }
        return null;
    }
}
