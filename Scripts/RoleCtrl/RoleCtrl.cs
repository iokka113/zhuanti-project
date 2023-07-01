using ZhuanTiNanMin.FSMachine;
using ZhuanTiNanMin.Mathematics;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 腳色控制
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class RoleCtrl : MonoBehaviour
{
    protected FSMachine _fsm;

    public SPUM_Prefabs Root { get; private set; }

    protected BoxCollider2D _colli;

    [SerializeField]
    private Effect _effect = null;
    private Animator _animMush = null;
    private Animator _animHawk = null;
    private Animator _animBat = null;
    private Animator _animTsu = null;

    [Space]
    [SerializeField]
    protected AttackType _attackType;
    [SerializeField]
    protected float _attackPower;

    [Space]
    [SerializeField]
    protected float _moveSpeed = 1f;

    public bool IsDead { get; protected set; }

    public bool StunStarted { get; private set; }
    private float _stunEndTime;
    protected bool _antiStun;

    protected virtual void Awake()
    {
        Root = GetComponentInChildren<SPUM_Prefabs>();
        _colli = GetComponent<BoxCollider2D>();
        int order = GetComponentInChildren<UnityEngine.Rendering.SortingGroup>().sortingOrder;
        _animMush = InitEffect(_effect.Mushroom, Root.transform, new Vector3(0f, 0.35f, 0f), new Vector3(0.45f, 0.45f, -2f), order + 1);
        _animHawk = InitEffect(_effect.HawkEyes, transform, new Vector3(0f, 1.2f, 0f), new Vector3(0.7f, 0.7f, 1f), order + 1);
        _animBat = InitEffect(_effect.BatBite, transform, new Vector3(0f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 1f), order + 1);
        _animTsu = InitEffect(_effect.Tsume, transform, new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f), order + 1);
    }

    protected virtual void Update()
    {
        StunEndCheck();
        _fsm.UpdateFSM();
    }

    /// <summary>
    /// 攻擊
    /// </summary>
    public abstract void Attack();

    /// <summary>
    /// 被攻擊
    /// </summary>
    public abstract void Damage(float value);

    /// <summary>
    /// 死亡
    /// </summary>
    public virtual void Die()
    {
        IsDead = true;
        //PlaySound(SoundType.Die);
        _colli.enabled = false;
        Funclib.AdjustColor(gameObject, Color.gray, Operation.Assignment);
    }

    /// <summary>
    /// <br>持續 sec 秒眩暈</br>
    /// <br>不計算累加</br>
    /// <br>如果 _antiStun 為真</br>
    /// <br>免疫此次眩暈</br>
    /// </summary>
    public void Stun(float sec)
    {
        if (!StunStarted && !_antiStun)
        {
            _stunEndTime = Time.time + sec;
            StunStarted = true;
        }
    }

    /// <summary>
    /// 檢查眩暈結束時間
    /// </summary>
    private void StunEndCheck()
    {
        if (Time.time > _stunEndTime)
        {
            StunStarted = false;
        }
    }

    public void PlayAnime(AnimeType type)
    {
        switch (type)
        {
            case AnimeType.Idle:
                Root._anim.SetFloat("RunState", 0f);
                break;
            case AnimeType.Death:
                Root._anim.SetTrigger("Die");
                break;
            case AnimeType.Run:
                Root._anim.SetFloat("RunState", 0.5f);
                break;
            case AnimeType.Stun:
                Root._anim.SetFloat("RunState", 1.0f);
                break;
            case AnimeType.AttackSword:
                Root._anim.SetTrigger("Attack");
                Root._anim.SetFloat("AttackState", 0.0f);
                Root._anim.SetFloat("NormalState", 0.0f);
                break;
            case AnimeType.AttackBow:
                Root._anim.SetTrigger("Attack");
                Root._anim.SetFloat("AttackState", 0.0f);
                Root._anim.SetFloat("NormalState", 0.5f);
                break;
            case AnimeType.AttackMagic:
                Root._anim.SetTrigger("Attack");
                Root._anim.SetFloat("AttackState", 0.0f);
                Root._anim.SetFloat("NormalState", 1.0f);
                break;
            case AnimeType.SkillSword:
                Root._anim.SetTrigger("Attack");
                Root._anim.SetFloat("AttackState", 1.0f);
                Root._anim.SetFloat("SkillState", 0.0f);
                break;
            case AnimeType.SkillBow:
                Root._anim.SetTrigger("Attack");
                Root._anim.SetFloat("AttackState", 1.0f);
                Root._anim.SetFloat("SkillState", 0.5f);
                break;
            case AnimeType.SkillMagic:
                Root._anim.SetTrigger("Attack");
                Root._anim.SetFloat("AttackState", 1.0f);
                Root._anim.SetFloat("SkillState", 1.0f);
                break;
            case AnimeType.Mushroom:
                _animMush.SetTrigger("Play");
                break;
            case AnimeType.HawkEyes:
                _animHawk.SetTrigger("Play");
                break;
            case AnimeType.BatBite:
                _animBat.SetTrigger("Play");
                break;
            case AnimeType.Tsume:
                _animTsu.SetTrigger("Play");
                break;
            default:
                return;
        }
    }

    private static Animator InitEffect(GameObject effect, Transform transform, Vector3 localPosition, Vector3 localScale, int sortingOrder)
    {
        GameObject newEffect = Instantiate(effect, transform);
        newEffect.transform.localPosition = localPosition;
        newEffect.transform.localScale = localScale;
        newEffect.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        return newEffect.GetComponent<Animator>();
    }

    [System.Serializable]
    public class Effect
    {
        public GameObject Mushroom;
        public GameObject HawkEyes;
        public GameObject BatBite;
        public GameObject Tsume;
    }

    public enum AnimeType
    {
        Idle,
        Death,
        Run,
        Stun,
        AttackSword,
        AttackBow,
        AttackMagic,
        SkillSword,
        SkillBow,
        SkillMagic,
        Mushroom,
        HawkEyes,
        BatBite,
        Tsume,
    }
}

/// <summary>
/// 狀態[眩暈]
/// </summary>
public class StateStun : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionStun(),
    };

    protected override List<DecisionBase> Decisions { get; } = new List<DecisionBase>
    {
        new DecisionCheckDeath(),
        new DecisionCheckUnStun(),
    };
}

/// <summary>
/// 動作[眩暈]
/// </summary>
public class ActionStun : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        RoleCtrl ctrl = fsm.Controller as RoleCtrl;
        if (ctrl != null)
        {
            ctrl.PlayAnime(RoleCtrl.AnimeType.Stun);
        }
    }
}

/// <summary>
/// 判斷[眩暈檢查]
/// </summary>
public class DecisionCheckStun : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        RoleCtrl ctrl = fsm.Controller as RoleCtrl;
        if (ctrl != null && ctrl.StunStarted)
        {
            return new StateStun();
        }
        return null;
    }
}

/// <summary>
/// 判斷[停止眩暈檢查]
/// </summary>
public class DecisionCheckUnStun : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        RoleCtrl ctrl = fsm.Controller as RoleCtrl;
        if (ctrl != null && !ctrl.StunStarted)
        {
            return fsm.InitialState;
        }
        return null;
    }
}
