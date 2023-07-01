using ZhuanTiNanMin.FSMachine;
using System.Collections.Generic;
using UnityEngine;

public class MushroomCtrl : MobCtrl
{
    [SerializeField]
    private float _attackCD = 2f;
    private float _attackCDTimer;

    public override void Attack()
    {
        _attackCDTimer += Time.deltaTime;
        if (_attackCDTimer > _attackCD)
        {
            _attackCDTimer = 0f;
            Spit();
        }
    }

    private void Spit()
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

    public override void Move(int mode)
    {
        return;
    }

    public override void Die()
    {
        for(int i = 0; i < 2; i++)
        {
            Instantiate(PrefabsManager.Instance.GetMobPrefab(MobType.Spore)).GetComponent<Spore>().Init(_room, transform.position);
        }
        base.Die();
    }
}

/// <summary>
/// 狀態[蘑菇怪待機]
/// </summary>
public class StateMushroomIdle : StateBase
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
/// 狀態[蘑菇怪攻擊]
/// </summary>
public class StateMushroomAttack : StateBase
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
    };
}
