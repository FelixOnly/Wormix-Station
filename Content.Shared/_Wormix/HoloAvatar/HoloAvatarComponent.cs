using Robust.Shared.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared._Wormix.HoloAvatar;

[RegisterComponent, NetworkedComponent]
public sealed partial class HoloAvatarComponent : Component
{
    /// <summary>
    /// Name of the shader to use
    /// </summary>
    [DataField]
    public string ShaderName = "Hologram";

    /// <summary>
    /// The primary color
    /// </summary>
    [DataField]
    public Color Color1 = new Color(80, 255, 160, 255);

    /// <summary>
    /// The secondary color
    /// </summary>
    [DataField]
    public Color Color2 = new Color(20, 50, 45, 255);

    /// <summary>
    /// The shared color alpha
    /// </summary>
    [DataField]
    public float Alpha = 0.9f;

    /// <summary>
    /// The color brightness
    /// </summary>
    [DataField]
    public float Intensity = 2f;

    /// <summary>
    /// The scroll rate of the hologram shader
    /// </summary>
    [DataField]
    public float ScrollRate = 0.125f;

}
