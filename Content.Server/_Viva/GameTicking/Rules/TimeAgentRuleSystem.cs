
using Content.Server._Viva.GameTicking.Rules.Components;
using Content.Server.Antag;
using Content.Server.GameTicking.Rules;
using Content.Server.Mind;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server._Viva.GameTicking.Rules;

public sealed class TimeAgentRuleSystem : GameRuleSystem<TimeAgentRuleComponent>
{

    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly SharedRoleSystem _role = default!;
    [Dependency] private readonly MindSystem _mind = default!;

    [ValidatePrototypeId<EntityPrototype>] static EntProtoId mindRole = "MindRoleTimeAgent";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TimeAgentRuleComponent, AfterAntagEntitySelectedEvent>(AfterSelected);
    }

    private void AfterSelected(Entity<TimeAgentRuleComponent> ent, ref AfterAntagEntitySelectedEvent args)
    {
        TryMakeTimeAgent(args.EntityUid, ent.Comp);
    }

    public bool TryMakeTimeAgent(EntityUid target, TimeAgentRuleComponent rule)
    {
        if (!_mind.TryGetMind(target, out var mindId, out var mind))
            return false;

        _role.MindAddRole(mindId, mindRole.Id, mind, true);

        _antag.SendBriefing(target, Loc.GetString("time-agent-briefing"), null, null);

        return true;
    }
}
