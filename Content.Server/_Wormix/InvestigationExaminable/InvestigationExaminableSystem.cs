using Content.Server.EUI;
using Content.Server.Mind;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.InvestigationExaminable;
using Content.Shared.Verbs;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server._Wormix.InvestigationExaminable;
public sealed class InvestigationExaminableSystem : EntitySystem
{
    [Dependency] private readonly ISharedPlayerManager _player = default!;
    [Dependency] private readonly ExamineSystemShared _examine = default!;
    [Dependency] private readonly EuiManager _euiMan = default!;
    [Dependency] private readonly MindSystem _mind = default!;


    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<InvestigationExaminableComponent, GetVerbsEvent<ExamineVerb>>(OnGetInvestigateVerbs);

    }

    private void OnGetInvestigateVerbs(Entity<InvestigationExaminableComponent> ent, ref GetVerbsEvent<ExamineVerb> args)
    {
        if (Identity.Name(args.Target, EntityManager) != MetaData(args.Target).EntityName)
            return;

        var detailsRange = _examine.IsInDetailsRange(args.User, ent);

        var user = args.User;

        var verb = new ExamineVerb
        {
            Act = () => OpenEui(user, ent.Owner),
            Text = Loc.GetString("detail-investigate-verb-text"),
            Disabled = !detailsRange,
            Message = detailsRange ? null : Loc.GetString("detail-investigate-verb-disabled"),
            Icon = new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/sentient.svg.192dpi.png")),
        };

        args.Verbs.Add(verb);
    }

    private void OpenEui(EntityUid user, EntityUid target)
    {
        if (!TryComp<InvestigationExaminableComponent>(target, out var detail))
            return;

        if (!_mind.TryGetMind(user, out _, out var mind)
            || mind is not { UserId: not null } || !_player.TryGetSessionById(mind.UserId, out var session))
            return;

        var state = new InvestigationExaminableEuiState(
            GetNetEntity(user),
            detail.InvestigateContent
        );

        var window = new InvestigationExaminableEui(state);
        _euiMan.OpenEui(window, session);
        window.StateDirty();
    }


}
