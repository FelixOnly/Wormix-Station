using Content.Shared._Wormix.MessageOverlay;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Server._Wormix.MessageOverlay;
public sealed class MessageOverlaySystem : EntitySystem
{

    public void CallMessage(NewOverlayMessage msg, Filter filter)
    {

        RaiseNetworkEvent(msg, filter);

    }
}
