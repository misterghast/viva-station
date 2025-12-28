

using Content.Shared._Impstation.Thaven;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Server._Viva.LoyaltyImplant;

[RegisterComponent, NetworkedComponent]
public sealed partial class LoyaltyImplantComponent : Component
{
    [DataField]
    public EntityUid? Target = null;

    public ThavenMood? Mood = null;
}
