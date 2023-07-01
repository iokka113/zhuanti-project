using ZhuanTiNanMin.FSMachine;
using System.Collections.Generic;
using UnityEngine;

public class SlimeCtrl : MobCtrl
{
    public float rangeTargetInAttack = 1.5f;

    [SerializeField]
    private float _attackIdleTime = 0.4f;
    private float _attackIdleTimer;
    [SerializeField]
    private float _attackTriggerTime = 0.1f;
    private float _attackTriggerTimer;

    [SerializeField]
    private float _attackSpeed = 2f;
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
                _attackTriggerTimer = Time.time + _attackTriggerTime;
                _attackDir = (PlayerCtrl.Instance.transform.position - transform.position).normalized;
                AttackIsStarted = true;
                _attacked = false;
            }
        }
        else
        {
            if (Time.time < _attackTriggerTimer)
            {
                transform.Translate(Time.deltaTime * _attackDir.x * _attackSpeed, Time.deltaTime * _attackDir.y * _attackSpeed, 0f);
            }
            else
            {
                AttackIsStarted = false;
            }
        }
        if (AttackIsStarted && !_attacked)
        {
            if (Vector2.Distance(PlayerCtrl.Instance.transform.position, transform.position) < 0.1f)
            {
                if (!PlayerCtrl.Instance.IsDead)
                {
                    PlayerCtrl.Instance.Damage(_attackPower);
                    _attacked = true;
                }
            }
        }
    }

    [SerializeField]
    private float _moveIdleTime = 0.2f;
    private float _moveIdleTimer;
    [SerializeField]
    private float _moveWalkTime = 0.2f;
    private float _moveWalkTimer;

    private bool _isWalking;

    private void Move(Vector2 targetPos)
    {
        if (_isWalking)
        {
            Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
            transform.Translate(Time.deltaTime * _moveSpeed * dir.x, Time.deltaTime * _moveSpeed * dir.y, 0f);

            _moveWalkTimer += Time.deltaTime;
            if (_moveWalkTimer > _moveWalkTime)
            {
                _moveWalkTimer = 0f;
                _isWalking = false;
            }
        }
        else
        {
            _moveIdleTimer += Time.deltaTime;
            if (_moveIdleTimer > _moveIdleTime)
            {
                _moveIdleTimer = 0f;
                _isWalking = true;
            }
        }
    }

    public override void Move(int mode)
    {
        if (mode == 0)
        {
            if (Vector2.Distance(_randomMovePos, transform.position) > 0.01f)
            {
                Move(_randomMovePos);
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
                Move(PlayerCtrl.Instance.transform.position);
            }
        }
    }

    [SerializeField]
    private float _hpOrigin = 0f;
    private bool _divisionTriggerA;
    private bool _divisionTriggerB;

    public override void Damage(float value)
    {
        base.Damage(value);
        CheckDivision();
    }

    private void CheckDivision()
    {
        if (!_divisionTriggerA && _hp < _hpOrigin * 0.65f)
        {
            _divisionTriggerA = true;
            SlimeCtrl ctrl = Instantiate(PrefabsManager.Instance.GetMobPrefab(MobType.Slime)).GetComponent<SlimeCtrl>();
            ctrl._divisionTriggerA = true;
            ctrl._hp = ctrl._hpMax = _hpOrigin * 0.5f;
            ctrl.transform.localScale = new Vector3(0.75f, 0.75f, 0f);
            float posX = transform.position.x + Random.Range(-1f, 1f);
            float posY = transform.position.y + Random.Range(-1f, 1f);
            ctrl.Init(_room, new Vector3(posX, posY, 0f));
        }
        if (!_divisionTriggerB && _hp < _hpOrigin * 0.4f)
        {
            _divisionTriggerB = true;
            SlimeCtrl ctrl = Instantiate(PrefabsManager.Instance.GetMobPrefab(MobType.Slime)).GetComponent<SlimeCtrl>();
            ctrl._divisionTriggerA = true;
            ctrl._divisionTriggerB = true;
            ctrl._hp = ctrl._hpMax = _hpOrigin * 0.25f;
            ctrl.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
            float posX = transform.position.x + Random.Range(-1f, 1f);
            float posY = transform.position.y + Random.Range(-1f, 1f);
            ctrl.Init(_room, new Vector3(posX, posY, 0f));
        }
    }
}

/// <summary>
/// 狀態[史萊姆巡邏]
/// </summary>
public class StateSlimePatrol : StateBase
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
/// 狀態[史萊姆追擊]
/// </summary>
public class StateSlimeChase : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionMobTargetMove(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionSlimeTargetInAttackRange(),
        new DecisionSlimeNoFoundTarget(),
        new DecisionCheckPlayerDead(),
        new DecisionCheckDeath(),
    };
}

/// <summary>
/// 狀態[史萊姆攻擊]
/// </summary>
public class StateSlimeAttack : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionMobAttack(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionSlimeTargetInAttackRange(),
        new DecisionSlimeNoFoundTarget(),
        new DecisionCheckPlayerDead(),
        new DecisionCheckDeath(),
    };
}

/// <summary>
/// 判斷[史萊姆未發現目標]
/// </summary>
public class DecisionSlimeNoFoundTarget : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        SlimeCtrl slime = fsm.Controller as SlimeCtrl;
        if (slime != null && !slime.AttackIsStarted)
        {
            if (Vector2.Distance(PlayerCtrl.Instance.transform.position, slime.transform.position) > slime.rangeFindTarget)
            {
                return fsm.InitialState;
            }
        }
        return null;
    }
}

/// <summary>
/// 判斷[史萊姆在攻擊範圍內]
/// </summary>
public class DecisionSlimeTargetInAttackRange : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        SlimeCtrl slime = fsm.Controller as SlimeCtrl;
        if (slime != null && !slime.AttackIsStarted)
        {
            if (Vector2.Distance(PlayerCtrl.Instance.transform.position, slime.transform.position) < slime.rangeTargetInAttack)
            {
                return new StateSlimeAttack();
            }
            else
            {
                return new StateSlimeChase();
            }
        }
        return null;
    }
}
