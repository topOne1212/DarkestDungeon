using UnityEngine;

public class PushEffect : SubEffect
{
    public override EffectSubType Type { get { return EffectSubType.Push; } }
    private int PushParam { get; set; }

    public PushEffect(int pushParam)
    {
        PushParam = pushParam;
    }

    public override bool ApplyInstant(FormationUnit performer, FormationUnit target, Effect effect)
    {
        if (target == null)
            return false;

        float moveChance = effect.IntegerParams[EffectIntParams.Chance].HasValue ?
            (float)effect.IntegerParams[EffectIntParams.Chance].Value / 100 : 1;

        moveChance -= target.Character.GetSingleAttribute(AttributeType.Move).ModifiedValue;
        if (performer != null && performer.Character is Hero)
            moveChance += performer.Character.GetSingleAttribute(AttributeType.MoveChance).ModifiedValue;

        moveChance = Mathf.Clamp(moveChance, 0, 0.95f);
        if (RandomSolver.CheckSuccess(moveChance))
        {
            target.Push(PushParam);
            return true;
        }
        return false;
    }

    public override bool ApplyQueued(FormationUnit performer, FormationUnit target, Effect effect)
    {
        if (target == null)
            return false;

        if (ApplyInstant(performer, target, effect))
            return true;
        else
        {
            RaidSceneManager.RaidEvents.ShowPopupMessage(target, PopupMessageType.MoveResist);
            return false;
        }
    }

    public override string Tooltip(Effect effect)
    {
        return string.Format(LocalizationManager.GetString("effect_tooltip_move_backward"), PushParam);
    }
}