using ZhuanTiNanMin.FSMachine;
using ZhuanTiNanMin.Mathematics;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 狀態[死亡]
/// </summary>
public class StateDead : StateBase
{
    protected override List<ActionBase> Actions { get; } = new List<ActionBase>
    {
        new ActionDie(),
    };

    protected override List<DecisionBase> Decisions { get; } = null;
}

/// <summary>
/// 動作[死亡]
/// </summary>
public class ActionDie : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        RoleCtrl ctrl = fsm.Controller as RoleCtrl;
        if (ctrl != null)
        {
            ctrl.PlayAnime(RoleCtrl.AnimeType.Death);
        }
        MobCtrl mob = fsm.Controller as MobCtrl;
        if (mob != null)
        {
            return;
        }
    }
}

/// <summary>
/// 判斷[死亡檢查]
/// </summary>
public class DecisionCheckDeath : DecisionBase
{
    public override StateBase Decide(FSMachine fsm)
    {
        RoleCtrl ctrl = fsm.Controller as RoleCtrl;
        if (ctrl != null && ctrl.IsDead)
        {
            return new StateDead();
        }
        MobCtrl mob = fsm.Controller as MobCtrl;
        if (mob != null && mob.IsDead)
        {
            return new StateDead();
        }
        return null;
    }
}

/// <summary>
/// 動作[精靈圖翻轉]
/// </summary>
public class ActionSpriteFlip : ActionBase
{
    public override void Act(FSMachine fsm)
    {
        PlayerCtrl player = fsm.Controller as PlayerCtrl;
        if (player != null)
        {
            Vector2 playerPos = Camera.main.WorldToScreenPoint(player.transform.position);
            Funclib.AxisFlip(player.Root.transform, Axis.X, Input.mousePosition.x - playerPos.x, true);
        }
        EnemyCtrl enemy = fsm.Controller as EnemyCtrl;
        if (enemy != null)
        {
            Funclib.AxisFlip(enemy.Root.transform, Axis.X, PlayerCtrl.Instance.transform.position.x - enemy.transform.position.x, true);
        }
        MobCtrl mob = fsm.Controller as MobCtrl;
        if (mob != null)
        {
            return;
        }
    }
}
