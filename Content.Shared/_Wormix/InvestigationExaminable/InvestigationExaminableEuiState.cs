using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Shared.Eui;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;



namespace Content.Shared.InvestigationExaminable;

[Serializable, NetSerializable]
public sealed class InvestigationExaminableEuiState : EuiStateBase
{
    public NetEntity Target;
    public string InvestigateContent = string.Empty;

    public InvestigationExaminableEuiState(NetEntity target, string investigateContent)
    {
        Target = target;
        InvestigateContent = investigateContent;
    }


}
