// SPDX-FileCopyrightText: 2026 FelixOnly <62942680+felixonly@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;

namespace Content.Server._Wormix.StationGoals;

[RegisterComponent]
public sealed partial class DepartmentGoalComponent: Component
{
    [DataField]
    public List<ProtoId<DepartmentGoalPrototype>> Goals = new();
}
