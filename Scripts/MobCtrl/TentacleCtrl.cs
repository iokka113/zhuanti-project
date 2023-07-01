using ZhuanTiNanMin.FSMachine;
using System.Collections.Generic;
using UnityEngine;

public class TentacleCtrl : MobCtrl
{
    [SerializeField]
    private float _rangeTargetInAttack = 1f;

    public override void Attack()
    {
        CheckGrub();
        Grub();
        Stab();
    }

    [SerializeField]
    private float _grubCDTime = 8f;
    private float _grubCDTimer;

    public bool GrubIsStarted { get; private set; }

    private void CheckGrub()
    {
        if (Vector2.Distance(PlayerCtrl.Instance.transform.position, transform.position) > _rangeTargetInAttack)
        {
            if (Time.time > _grubCDTimer)
            {
                _grubCDTimer = Time.time + _grubCDTime + _grubTime;
                GrubIsStarted = true;
                _colli.enabled = false;
                _body.SetActive(false);
                _hpBar.SetActive(false);
            }
        }
    }

    [SerializeField]
    private float _grubTime = 2f;
    private float _grubTimer;

    private void Grub()
    {
        if (GrubIsStarted)
        {
            _grubTimer += Time.deltaTime;
            if (_grubTimer > _grubTime)
            {
                _grubTimer = 0f;
                _stabTimer = 0f;
                _colli.enabled = true;
                _body.SetActive(true);
                _hpBar.SetActive(true);
                transform.position = PlayerCtrl.Instance.transform.position;
                if (Vector2.Distance(PlayerCtrl.Instance.transform.position, transform.position) < _rangeTargetInAttack)
                {
                    if (!PlayerCtrl.Instance.IsDead)
                    {
                        PlayerCtrl.Instance.Damage(_attackPower);
                    }
                }
                GrubIsStarted = false;
            }
        }
    }

    [SerializeField]
    private float _stabIdleTime = 0.7f;
    private float _stabTimer;

    private void Stab()
    {
        if (!GrubIsStarted)
        {
            if (_stabTimer > _stabIdleTime)
            {
                _stabTimer = 0f;
                if (Vector2.Distance(PlayerCtrl.Instance.transform.position, transform.position) < _rangeTargetInAttack)
                {
                    if (!PlayerCtrl.Instance.IsDead)
                    {
                        PlayerCtrl.Instance.Damage(_attackPower);
                    }
                }
            }
            else
            {
                _stabTimer += Time.deltaTime;
            }
        }
    }

    public override void Move(int mode)
    {
        return;
    }
}

/// <summary>
/// 狀態[觸手待機]
/// </summary>
public class StateTentacleIdle : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionHealing(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionMobFoundTargetOrAttackRare(),
        new DecisionCheckPlayerDead(),
        new DecisionCheckDeath(),
    };
}

/// <summary>
/// 狀態[觸手攻擊]
/// </summary>
public class StateTentacleAttack : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionMobAttack(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionMobNoFoundTarget(),
        new DecisionCheckPlayerDead(),
        new DecisionCheckDeath(),
        new DecisionTentacleIsGrubbing(),
    };
}

/// <summary>
/// 判斷[觸手正在挖掘]
/// </summary>
public class DecisionTentacleIsGrubbing : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        TentacleCtrl tentacle = fsm.Controller as TentacleCtrl;
        if (tentacle != null && tentacle.GrubIsStarted)
        {
            return new StateTentacleAttack();
        }
        return null;
    }
}
