﻿using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Storage.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chaos.Scripts.DialogScripts.Mileth
{
    public class TeleportToTerrorBossScript : DialogScriptBase
    {
        private readonly ISimpleCache SimpleCache;

        /// <inheritdoc />
        public TeleportToTerrorBossScript(Dialog subject, ISimpleCache simpleCache)
            : base(subject) =>
            SimpleCache = simpleCache;

        public override void OnDisplaying(Aisling source)
        {
            var point = new Point(source.X, source.Y);
            var group = source.Group?.Where(x => x.WithinRange(point));

            if (group is null)
            {
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure you group your companions for this quest!");
                Subject.Text = "What? You don't have any friends with you.. who are you talking to?";
            }
            if (group is not null)
            {
                int groupCount = 0;
                foreach (var member in group)
                {
                    if (member.WithinLevelRange(source))
                        ++groupCount;
                }
                if (groupCount.Equals(group.Count()))
                {
                    Subject.Close(source);
                    foreach (var member in group)
                    {
                        MapInstance mapInstance;
                        mapInstance = SimpleCache.Get<MapInstance>("cryptTerror");
                        Point pointS = new Point(13, 8);
                        member.TraverseMap(mapInstance, pointS);
                    }
                }
                else
                {
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure your companions are within level range.");
                    Subject.Text = "Some of your companions are not within your level range.";
                }
            }
        }
    }
}