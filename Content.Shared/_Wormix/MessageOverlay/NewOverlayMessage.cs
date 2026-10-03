using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared._Wormix.MessageOverlay;

[Serializable, NetSerializable]
public sealed class NewOverlayMessage : EntityEventArgs
{
    public string Name = "";
    public string Description = "";

    public NewOverlayMessage(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
