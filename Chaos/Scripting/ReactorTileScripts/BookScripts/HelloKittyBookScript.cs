using Chaos.Common.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.BookScripts;

public sealed class HelloKittyBookScript : ReactorTileScriptBase
{
    /// <inheritdoc />
    public HelloKittyBookScript(ReactorTile subject)
        : base(subject) {}

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        source.Client.SendServerMessage(
            ServerMessageType.ScrollWindow,
            "{=q     ~THE STORY OF HELLO KITTY & HELLO DOGGY~\n\n   {=gOnce upon a time, in the charming village of Sanrio Land, where adorable characters roamed and colorful buildings stood, lived two best friends: Hello Kitty and Hello Doggy.\n\n   Hello Kitty was known for her kindness, sweet nature, and her signature bow on her ear. She loved to help her friends and spread happiness wherever she went. On the other hand, Hello Doggy was playful, energetic, and loved to explore every corner of the village. With her wagging tail and friendly bark, she was always ready for an adventure.\n\n    For years, Hello Kitty and Hello Doggy were inseparable. They went on picnics, played games, and shared secrets. The village adored their friendship, and their joyous laughter filled the air. One day, a misunderstanding sparked a little rivalry between them. It all began with a game of hide-and-seek.\n\n   Hello Doggy thought Hello Kitty had tricked her, and Hello Kitty believed Hello Doggy was being unfair. The situation escalated, and their friendship was put to the test. The village was saddened to see their beloved friends at odds. They tried to mediate, but Hello Kitty and Hello Doggy refused to speak to each other. The once-happy village was now divided. As days turned into weeks, the separation took a toll on both Hello Kitty and Hello Doggy. They missed each other's company and realized that their friendship was far more valuable than any misunderstanding. They knew they had to make things right.\n\n   One sunny afternoon, Hello Kitty and Hello Doggy bumped into each other at the village square. Their eyes met, and a mixture of sadness and longing filled the air. Without saying a word, they rushed toward each other and embraced tightly. Tears streamed down their cheeks as they apologized and shared their feelings. The village watched in awe as the two friends reconciled, their laughter echoing through Sanrio Land once more. From that day forward, Hello Kitty and Hello Doggy learned that misunderstandings were part of life, but their bond was unbreakable.\n\n   They became advocates of open communication and resolved differences with understanding and compassion. Their story served as a reminder to the entire village that true friendship can overcome any challenge. And so, the village of Sanrio Land continued to thrive, with Hello Kitty, Hello Doggy, and their enduring friendship at its heart.");

        return;
    }
}