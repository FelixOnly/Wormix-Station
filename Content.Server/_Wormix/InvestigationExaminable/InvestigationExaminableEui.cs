using Content.Shared.Eui;
using Content.Shared.InvestigationExaminable;
using Content.Server.EUI;


namespace Content.Server._Wormix.InvestigationExaminable;
public sealed class InvestigationExaminableEui : BaseEui
{
    private readonly InvestigationExaminableEuiState _state;

    public InvestigationExaminableEui(InvestigationExaminableEuiState state)
    {
        _state = state;
    }

    public override EuiStateBase GetNewState()
    {
        return _state;
    }
}
