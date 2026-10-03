using Content.Client.Stylesheets;
using Content.Shared._Wormix.HoloAvatar;
using Content.Shared.Chat.TypingIndicator;
using Content.Shared.Holopad;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Content.Client.Options.OptionsVisualizerComponent;
using DrawDepth = Content.Shared.DrawDepth.DrawDepth;



namespace Content.Client._Wormix.HoloAvatar;


public sealed class HoloAvatarSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;


    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HoloAvatarComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<HoloAvatarComponent, BeforePostShaderRenderEvent>(OnShaderRender);

    }

    private void OnShaderRender(Entity<HoloAvatarComponent> entity, ref BeforePostShaderRenderEvent ev)
    {
        if (ev.Sprite.PostShader == null)
            return;

        UpdateHologramSprite(entity);
    }

    private void OnComponentStartup(Entity<HoloAvatarComponent> entity, ref ComponentStartup ev)
    {





        UpdateHologramSprite(entity);
    }

    private void UpdateHologramSprite(EntityUid avatar)
    {
        // Get required components
        if (!TryComp<SpriteComponent>(avatar, out var avatarSprite) ||
            !TryComp<HoloAvatarComponent>(avatar, out var avatarHologram))
            return;

        // Override specific values
        _sprite.SetColor((avatar, avatarSprite), Color.White);
        _sprite.SetDrawDepth((avatar, avatarSprite), (int)DrawDepth.Mobs);

        // Remove shading from all layers (except displacement maps)
        for (var i = 0; i < avatarSprite.AllLayers.Count(); i++)
        {
            if (_sprite.TryGetLayer((avatar, avatarSprite), i, out var layer, false) && layer.ShaderPrototype != "DisplacedDraw")
                avatarSprite.LayerSetShader(i, "unshaded");
        }

        UpdateHologramShader(avatar, avatarSprite, avatarHologram);
    }

    private void UpdateHologramShader(EntityUid uid, SpriteComponent sprite, HoloAvatarComponent holoAvatar)
    {
        // Find the texture height of the largest layer
        float texHeight = sprite.AllLayers.Max(x => x.PixelSize.Y);

        var instance = _prototypeManager.Index<ShaderPrototype>(holoAvatar.ShaderName).InstanceUnique();
        instance.SetParameter("color1", new Vector3(holoAvatar.Color1.R, holoAvatar.Color1.G, holoAvatar.Color1.B));
        instance.SetParameter("color2", new Vector3(holoAvatar.Color2.R, holoAvatar.Color2.G, holoAvatar.Color2.B));
        instance.SetParameter("alpha", holoAvatar.Alpha);
        instance.SetParameter("intensity", holoAvatar.Intensity);
        instance.SetParameter("texHeight", texHeight);
        instance.SetParameter("t", (float)_timing.CurTime.TotalSeconds * holoAvatar.ScrollRate);

        sprite.PostShader = instance;
        sprite.RaiseShaderEvent = true;
    }

    public record SpriteSnapshot(
            ResPath? RsiPath,
            Color Color,
            bool Visible);

    public List<SpriteSnapshot> SaveSprite(EntityUid uid)
    {
        var snapshot = new List<SpriteSnapshot>();
        if (!TryComp<SpriteComponent>(uid, out var sprite)) return snapshot;

        foreach (var layer in sprite.AllLayers)
        {
            snapshot.Add(new SpriteSnapshot(
                layer.Rsi?.Path,
                layer.Color,
                layer.Visible
            ));
        }
        return snapshot;
    }

}
