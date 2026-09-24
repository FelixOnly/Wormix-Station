using Robust.Shared.Prototypes;

namespace Content.Server._Wormix.StationGoals;

[RegisterComponent]
public sealed partial class DepartmentGoalComponent: Component
{
    [DataField]
    public List<ProtoId<DepartmentGoalPrototype>> Goals = new();
}
