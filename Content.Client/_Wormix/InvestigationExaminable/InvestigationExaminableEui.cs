using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.InvestigationExaminable;
using System.Numerics;

namespace Content.Client._Wormix.InvestigationExaminable;
public sealed class InvestigationExaminableEui : BaseEui
{
    [Dependency] private readonly IEntityManager _entManager = default!;

    private readonly InvestigationExaminableWindow _window;

 

    public InvestigationExaminableEui()
    {
        _window = new InvestigationExaminableWindow();
    }

    public override void Opened()
    {
        _window.OpenCenteredAt(new Vector2(.5f, .5f));
    }

    public override void Closed()
    {
        _window.Close();
    }

    public override void HandleState(EuiStateBase state)
    {
        base.HandleState(state);

        if (state is InvestigationExaminableEuiState examinableState)
            _window.UpdateState(examinableState, _entManager);
    }

}
