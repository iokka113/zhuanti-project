using ZhuanTiNanMin.FSMachine;
using ZhuanTiNanMin.Mathematics;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class MobCtrl : MonoBehaviour
{
    protected Room _room;

    private FSMachine _fsm;

    protected BoxCollider2D _colli;

    protected GameObject _body;

    [SerializeField]
    protected GameObject _hpBar;
    [SerializeField]
    private Image _hpFill = null;

    [SerializeField]
    protected float _hpMax;
    protected float _hp;

    public void Init(Room room, Vector2 pos)
    {
        _room = room;
        _colli = GetComponent<BoxCollider2D>();
        transform.position = pos;
        _randomMovePos = Funclib.RandomInsideCircle(_room.transform.position, _room.Colli.radius);
        _hp = _hpMax;
        {
            System.Type t = GetType();
            if (t.Equals(typeof(SlimeCtrl))) { _fsm = new FSMachine(this, new StateSlimePatrol()); }
            else if (t.Equals(typeof(SnakeCtrl))) { _fsm = new FSMachine(this, new StateSnakePatrol()); }
            else if (t.Equals(typeof(MushroomCtrl))) { _fsm = new FSMachine(this, new StateMushroomIdle()); }
            else if (t.Equals(typeof(TentacleCtrl))) { _fsm = new FSMachine(this, new StateTentacleIdle()); }
            else if (t.Equals(typeof(BatCtrl))) { _fsm = new FSMachine(this, new StateBatPatrol()); }
            else if (t.Equals(typeof(StoneCtrl))) { _fsm = new FSMachine(this, new StateStonePatrol()); }
            else if (t.Equals(typeof(SkeletonCtrl))) { _fsm = new FSMachine(this, new StateSkeletonPatrol()); }
        }
        _body = PrefabsManager.Instance.GetMobBody(this);
        _body.transform.SetParent(transform, false);
        _room.MobUpdate += MobUpdate;
        _room.MobCount++;
    }

    private void MobUpdate()
    {
        if (Time.time > _attackRareTime) { AttackRare = false; }
        _fsm.UpdateFSM();
    }

    [SerializeField]
    protected float _moveSpeed = 1f;
    protected Vector2 _randomMovePos;

    protected void NewRandomMovePos()
    {
        _randomMovePos = Funclib.RandomInsideCircle(_room.transform.position, _room.Colli.radius);
    }

    /// <summary>
    /// 移動
    /// </summary>
    public abstract void Move(int mode);

    [Tooltip("回到滿血的秒數")]
    [SerializeField]
    private float _healingTimeRequired = 3f;
    private float _healingCD;

    /// <summary>
    /// 治療
    /// </summary>
    public void Healing()
    {
        if (Time.time > _healingCD && _healingTimeRequired != 0f)
        {
            _healingCD = Time.time + 0.1f;
            _hp += _hpMax / 10f / _healingTimeRequired;
            _hp = Mathf.Clamp(_hp, 0f, _hpMax);
            _hpFill.fillAmount = _hp / _hpMax;
            if (_hp == 0f) { Die(); }
        }
    }

    public float rangeFindTarget;

    [SerializeField]
    protected float _attackPower;

    /// <summary>
    /// 攻擊
    /// </summary>
    public abstract void Attack();

    public bool AttackRare { get; private set; }
    protected float _attackRareTime;

    /// <summary>
    /// 被攻擊
    /// </summary>
    public virtual void Damage(float value)
    {
        _attackRareTime = Time.time + 10f;
        AttackRare = true;
        _hp -= value;
        _hp = Mathf.Clamp(_hp, 0f, _hpMax);
        _hpFill.fillAmount = _hp / _hpMax;
        if (_hp == 0f) { Die(); }
    }

    public bool IsDead { get; protected set; }

    /// <summary>
    /// 死亡
    /// </summary>
    public virtual void Die()
    {
        IsDead = true;
        _colli.enabled = false;
        _hpBar.SetActive(false);
        _room.MobCount--;
        _room.MobUpdate -= MobUpdate;
        Destroy(gameObject);
    }

    protected void HpColorChange(Color color)
    {
        _hpFill.color = color;
    }
}

public enum BuffType
{
    None = 0,
    Poisoning,
}

/// <summary>
/// 動作[回血]
/// </summary>
public class ActionHealing : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        MobCtrl mob = fsm.Controller as MobCtrl;
        if (mob != null)
        {
            mob.Healing();
        }
    }
}

/// <summary>
/// 動作[怪物隨機移動]
/// </summary>
public class ActionMobRandomMove : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        MobCtrl mob = fsm.Controller as MobCtrl;
        if (mob != null)
        {
            mob.Move(0);
        }
    }
}

/// <summary>
/// 動作[怪物目標移動]
/// </summary>
public class ActionMobTargetMove : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        MobCtrl mob = fsm.Controller as MobCtrl;
        if (mob != null)
        {
            mob.Move(1);
        }
    }
}

/// <summary>
/// 動作[怪物攻擊]
/// </summary>
public class ActionMobAttack : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        MobCtrl mob = fsm.Controller as MobCtrl;
        if (mob != null)
        {
            mob.Attack();
        }
    }
}

/// <summary>
/// 判斷[怪物被攻擊或發現目標]
/// </summary>
public class DecisionMobFoundTargetOrAttackRare : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        MobCtrl mob = fsm.Controller as MobCtrl;
        if (mob != null)
        {
            if (mob.AttackRare || Vector2.Distance(PlayerCtrl.Instance.transform.position, mob.transform.position) < mob.rangeFindTarget)
            {
                System.Type t = mob.GetType();
                if (t.Equals(typeof(SlimeCtrl))) { return new StateSlimeChase(); }
                else if (t.Equals(typeof(SnakeCtrl))) { return new StateSnakeAttack(); }
                else if (t.Equals(typeof(MushroomCtrl))) { return new StateMushroomAttack(); }
                else if (t.Equals(typeof(TentacleCtrl))) { return new StateTentacleAttack(); }
                else if (t.Equals(typeof(BatCtrl))) { return new StateBatAttack(); }
                else if (t.Equals(typeof(StoneCtrl))) { return new StateStoneAttack(); }
                else if (t.Equals(typeof(SkeletonCtrl))) { return new StateSkeletonAttackFar(); }
            }
        }
        return null;
    }
}

/// <summary>
/// 判斷[怪物未發現目標]
/// </summary>
public class DecisionMobNoFoundTarget : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        MobCtrl mob = fsm.Controller as MobCtrl;
        if (mob != null && Vector2.Distance(PlayerCtrl.Instance.transform.position, mob.transform.position) > mob.rangeFindTarget)
        {
            return fsm.InitialState;
        }
        return null;
    }
}
