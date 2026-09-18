// SPDX-FileCopyrightText: 2026 sablerti <work.feliks@proton.me>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Preferences;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Roles;

/// <summary>
/// Requires the character to be within a certain age range
/// </summary>
[UsedImplicitly]
[Serializable, NetSerializable]
public sealed partial class MaturityRequirement : JobRequirement
{
    [DataField(required: true)] public int MinAge;
    [DataField] public int? MaxAge;

    public override bool Check(IEntityManager entManager,
        IPrototypeManager protoManager,
        HumanoidCharacterProfile? profile,
        IReadOnlyDictionary<string, TimeSpan> playTimes,
        [NotNullWhen(false)] out FormattedMessage? reason)
    {
        reason = new FormattedMessage();

        if (profile is null) //the profile could be null if the player is a ghost. In this case we don't need to block the role selection for ghostrole
            return true;

        var inRange = profile.Age >= MinAge;

        if (MaxAge != null)
        {
            inRange = profile.Age >= MinAge && profile.Age <= MaxAge;
        }

        if (!Inverted)
        {
            if (!inRange)
            {
                reason = FormattedMessage.FromMarkupPermissive(
                    MaxAge > 0
                        ? Loc.GetString("role-age-not-in-range",
                            ("minAge", MinAge),
                            ("maxAge", MaxAge))
                        : Loc.GetString("role-age-too-young",
                            ("minAge", MinAge)));

                return false;
            }
        }
        else
        {
            if (inRange)
            {
                reason = FormattedMessage.FromMarkupPermissive(
                    MaxAge > 0
                        ? Loc.GetString("role-age-in-range",
                            ("minAge", MinAge),
                            ("maxAge", MaxAge))
                        : Loc.GetString("role-age-old-enough",
                            ("minAge", MinAge)));

                return false;
            }
        }

        return true;
    }


}
