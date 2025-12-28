

using System.Linq;
using Content.Server._Impstation.Thaven;
using Content.Shared._Impstation.Thaven.Components;
using Content.Shared.Dataset;
using Content.Shared.Implants;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed.TypeParsers;
using Robust.Shared.Utility;

namespace Content.Server._Viva.LoyaltyImplant;

public sealed class LoyaltyImplantSysten : EntitySystem
{
    [Dependency] private readonly ThavenMoodsSystem _moodSystem = default!;

    [ValidatePrototypeId<DatasetPrototype>]
    private const string LoyaltyImplantMoods = "LoyaltyImplantMoods";
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<LoyaltyImplantComponent, ImplantImplantedEvent>(OnImplant);
        SubscribeLocalEvent<LoyaltyImplantComponent, EntGotRemovedFromContainerMessage>(OnRemove);
    }

    private void OnImplant(EntityUid uid, LoyaltyImplantComponent component, ImplantImplantedEvent args)
    {
        if (args.Implanted == null)
            return;

        var target = args.Implanted.Value;
        component.Target = target;

        if (args.Implanted != null)
        {
            if (!EnsureComp<ThavenMoodsBoundComponent>(target, out var moodComp))
            {
                _moodSystem.ToggleEmaggable((target, moodComp));
                _moodSystem.ClearMoods((target, moodComp));
                _moodSystem.ToggleSharedMoods((target, moodComp));
            }

            _moodSystem.TryAddRandomMood(target, LoyaltyImplantMoods, moodComp);
            component.Mood = moodComp.Moods.Last(); //get last mood in list because it should be the one we just added
            Dirty(target, moodComp);
        }
    }

    private void OnRemove(EntityUid uid, LoyaltyImplantComponent component, EntGotRemovedFromContainerMessage args)
    {
        component.Target = args.Container.Owner;

        if (TryComp<ThavenMoodsBoundComponent>(args.Container.Owner, out var moodComp))
            if (component.Mood != null)
            {
                var newSet = moodComp.Moods.ShallowClone();
                newSet.Remove(component.Mood);
                if (newSet.Count > 0)
                {
                    _moodSystem.SetMoods(args.Container.Owner, newSet, moodComp);
                }
                else
                {
                    RemComp<ThavenMoodsBoundComponent>(args.Container.Owner);
                }
            }
    }
}
