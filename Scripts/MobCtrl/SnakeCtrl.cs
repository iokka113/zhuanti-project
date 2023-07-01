using ZhuanTiNanMin.FSMachine;
using System.Collections.Generic;
using UnityEngine;

public class SnakeCtrl : MobCtrl
{
    [SerializeField]
    private float _attackCD = 1.5f;
    private float _attackCDTimer;

    //[SerializeField]
    //private float _skillCD = 6f;
    //private float _skillCDTimer;
    //private bool _skillTrigger;
    private int _skillTrigger;

    public override void Attack()
    {
        //_skillCDTimer += Time.deltaTime;
        //if (_skillCDTimer > _skillCD)
        //{
        //    _skillCDTimer = 0f;
        //    _skillTrigger = true;
        //}
        _attackCDTimer += Time.deltaTime;
        if (_attackCDTimer > _attackCD)
        {
            _attackCDTimer = 0f;
            Spit();
        }
    }

    private void Spit()
    {
        if (_skillTrigger < 3)
        {
            AttackObjInfo info = new AttackObjInfo
            {
                Target = PlayerCtrl.Instance.transform.position,
                TargetLayer = LayerMask.NameToLayer("Player"),
                Center = transform.position,
                Radius = 0.2f,
                Power = _attackPower,
                Speed = 10f,
            };
            PrefabsManager.Instance.PoolSpawn(PoolObjType.Venom, info);
            _skillTrigger++;
        }
        else
        {
            AttackObjInfo info = new AttackObjInfo
            {
                Target = PlayerCtrl.Instance.transform.position,
                TargetLayer = LayerMask.NameToLayer("Player"),
                Center = transform.position,
                Radius = 0.2f,
                Power = _attackPower * 1.5f,
                Speed = 10 * 5f,
                Buff = BuffType.Poisoning,
            };
            PrefabsManager.Instance.PoolSpawn(PoolObjType.Venom, info);
            _skillTrigger = 0;
        }
    }

    public override void Move(int mode)
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

/// <summary>
/// 狀態[蛇巡邏]
/// </summary>
public class StateSnakePatrol : StateBase
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
/// 狀態[蛇攻擊]
/// </summary>
public class StateSnakeAttack : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionMobTargetMove(),
        new ActionMobAttack(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionMobNoFoundTarget(),
        new DecisionCheckPlayerDead(),
        new DecisionCheckDeath(),
    };
}
