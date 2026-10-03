using Content.Shared._Wormix.MessageOverlay;
using Robust.Client.Graphics;

namespace Content.Client._Wormix.MessageOverlay;
public sealed class MessageOverlaySystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overMan = default!;

    private MessageOverlay _overlay = default!;


    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<NewOverlayMessage>(OnShow);
        _overlay = new();
        _overMan.AddOverlay(_overlay);
    }

    private void OnShow(NewOverlayMessage ev)
    {
        _overlay.Reset();             //these should be reset as well to match OnSwap
        _overlay.ResetDescription();

        if (_overlay.Text != null) //i dont know why this is here but im not touching it
            return;

        _overlay.Text = ev.Name;
        _overlay.TextDescription = ev.Description; // fallback is "" if no description is found.
        _overlay.CharInterval = TimeSpan.FromSeconds(2f / _overlay.Text.Length);

        if (_overlay.TextDescription == "")
            _overlay.CharIntervalDescription = TimeSpan.Zero; //if this is not done it tries dividing by 0 in the "else" clause
        else
            _overlay.CharIntervalDescription = TimeSpan.FromSeconds(2f / _overlay.TextDescription.Length);

    }
}
