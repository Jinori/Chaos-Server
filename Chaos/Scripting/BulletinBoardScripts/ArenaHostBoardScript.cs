using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Board;
using Chaos.Models.World;
using Chaos.Scripting.BulletinBoardScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.BulletinBoardScripts;

/// <summary>
///     A board script that allows anyone to post.
/// </summary>
/// <remarks>
///     Permissions:
///     <br />
///     Read: Anyone
///     <br />
///     Write: Arena Hosts
///     <br />
///     Delete: Post Owner / Moderator
///     <br />
///     Highlight: Moderator
///     <br />
///     Retention: Configurable
/// </remarks>
public sealed class ArenaHostBoardScript : ConfigurableBulletinBoardScriptBase
{
    private readonly TimeSpan PostRetentionTime;
    private readonly IIntervalTimer PostRetentionTimer;
    public ICollection<string> Moderators { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     The number of hours a post will last before being automatically deleted. Use -1 to never delete posts
    /// </summary>
    public int PostRetentionHours { get; init; }

    /// <inheritdoc />
    public ArenaHostBoardScript(BulletinBoard subject)
        : base(subject)
    {
        PostRetentionTime = TimeSpan.FromHours(PostRetentionHours);
        PostRetentionTimer = new IntervalTimer(TimeSpan.FromMinutes(5));
    }

    public override bool AllowedToDelete(Aisling aisling, Post post)
        => aisling.Name.EqualsI(post.Author) || aisling.IsAdmin || IsHost(aisling);

    /// <inheritdoc />
    public override bool AllowedToHighlight(Aisling aisling) => Moderators.ContainsI(aisling.Name) || IsHost(aisling);

    /// <inheritdoc />
    public override bool AllowedToPost(Aisling aisling) => IsHost(aisling);

    /// <inheritdoc />
    public override bool AllowedToView(Aisling aisling) => true;

    private bool IsHost(Aisling aisling)
        => aisling.Trackers.Enums.TryGetValue(out ArenaHost value) && value is ArenaHost.Host or ArenaHost.MasterHost;

    /// <inheritdoc />
    public override bool ShouldRejectPost(Aisling aisling, Post post, [MaybeNullWhen(false)] out string reason)
    {
        reason = null;

        return false;
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        if (PostRetentionHours == -1)
            return;

        PostRetentionTimer.Update(delta);

        if (!PostRetentionTimer.IntervalElapsed)
            return;

        //prune old posts based on retention time
        foreach (var post in Subject.ToList())
            if (DateTime.UtcNow.Subtract(post.CreationDate) > PostRetentionTime)
                Subject.Posts.Remove(post.PostId, out _);
    }
}