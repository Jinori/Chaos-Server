/*
internal static class ServerPackets
{
    internal static ServerPacket ConnectionInfo(uint tableCheckSum, byte seed, byte[] key)
    {
        var packet = new ServerPacket(ServerOpCode.ConnectionInfo);

        packet.WriteByte(0);
        packet.WriteUInt32(tableCheckSum);
        packet.WriteByte(seed);
        packet.WriteData8(key);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] CHKSUM: {tableCheckSum} | SEED: {seed} | KEY: {Encoding.ASCII.GetString(key)}";
        return packet;
    }
    internal static ServerPacket LoginMessage(LoginMessageType type, string message = "")
    {
        var packet = new ServerPacket(ServerOpCode.LoginMessage);

        packet.WriteByte((byte)type);
        packet.WriteString8((type == LoginMessageType.Confirm) ? "\0" : message);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] TYPE: {type} | MESSAGE: {message}";
        return packet;
    }
    internal static ServerPacket Redirect(Redirect redirect)
    {
        var packet = new ServerPacket(ServerOpCode.Redirect);

        packet.Write(redirect.EndPoint.Address.GetAddressBytes().Reverse().ToArray());
        packet.WriteUInt16((ushort)redirect.EndPoint.Port);
        packet.WriteByte((byte)(redirect.Key.Length + Encoding.GetEncoding(949).GetBytes(redirect.Name).Length + 7));
        packet.WriteByte(redirect.Seed);
        packet.WriteData8(redirect.Key);
        packet.WriteString8(redirect.Name);
        packet.WriteInt32(redirect.Id);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] {redirect}";
        return packet;
    }
    internal static ServerPacket Location(Point point)
    {
        var packet = new ServerPacket(ServerOpCode.Location);

        packet.WritePoint16(point);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] POINT: {point}";
        return packet;
    }
    internal static ServerPacket UserId(int userId, BaseClass userClass)
    {
        var packet = new ServerPacket(ServerOpCode.UserId);

        packet.WriteUInt32((uint)userId);
        packet.WriteInt16(0);//dunno
        packet.WriteByte((byte)userClass);
        packet.WriteInt16(0);//dunno

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ID: {userId} | CLASS: {userClass}";
        return packet;
    }
    internal static ServerPacket DisplayItemMonster(params VisibleObject[] objects)
    {
        var packet = new ServerPacket(ServerOpCode.DisplayVisibleObject);

        packet.WriteUInt16((ushort)objects.Length);
        foreach (var obj in objects)
        {
            packet.WritePoint16(obj.Point);
            packet.WriteInt32(obj.ID);
            packet.WriteUInt16(obj.Sprite);
            if (obj.Sprite < CONSTANTS.ITEM_SPRITE_OFFSET) //monster and merchant sprites
            {
                var newObj = obj as Creature;
                packet.Write(new byte[4]); //dunno
                packet.WriteByte((byte)newObj.Direction);
                packet.WriteByte(0); //dunno
                packet.WriteByte((byte)newObj.Type);
                if (newObj.Type == CreatureType.Merchant) //merchant type
                    packet.WriteString8(newObj.Name);
            }
            else //item sprites
                packet.Write(new byte[3]); //dunno
        }

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] COUNT: {objects.Length}";
        return packet;
    }
    internal static ServerPacket Attributes(bool admin, StatUpdateType updateType, Attributes stats)
    {
        var packet = new ServerPacket(ServerOpCode.Attributes);

        packet.WriteByte((byte)(admin ? updateType |= StatUpdateType.GameMasterA : updateType));
        if (updateType.HasFlag(StatUpdateType.Primary))
        {
            packet.Write(new byte[3]); //dunno
            packet.WriteByte(stats.Level);
            packet.WriteByte(stats.Ability);
            packet.WriteUInt32(stats.MaximumHP);
            packet.WriteUInt32(stats.MaximumMP);
            packet.WriteByte(stats.CurrentStr);
            packet.WriteByte(stats.CurrentInt);
            packet.WriteByte(stats.CurrentWis);
            packet.WriteByte(stats.CurrentCon);
            packet.WriteByte(stats.CurrentDex);
            packet.WriteBoolean(stats.HasUnspentPoints);
            packet.WriteByte(stats.UnspentPoints);
            packet.WriteInt16(stats.MaximumWeight);
            packet.WriteInt16(stats.CurrentWeight);
            packet.Write(new byte[4]); //dunno
        }
        if (updateType.HasFlag(StatUpdateType.Vitality))
        {
            packet.WriteUInt32(stats.CurrentHP);
            packet.WriteUInt32(stats.CurrentMP);
        }
        if (updateType.HasFlag(StatUpdateType.ExpGold))
        {
            packet.WriteUInt32(stats.Experience);
            packet.WriteUInt32(stats.ToNextLevel);
            packet.WriteUInt32(stats.AbilityExp);
            packet.WriteUInt32(stats.ToNextAbility);
            packet.WriteUInt32(stats.GamePoints);
            packet.WriteUInt32(stats.Gold);
        }
        if (updateType.HasFlag(StatUpdateType.Secondary))
        {
            packet.Write(new byte[1]); //dunno
            packet.WriteByte(stats.Blind);
            packet.Write(new byte[3]); //dunno
            packet.WriteByte((byte)stats.MailFlags);
            packet.WriteByte((byte)stats.OffenseElement);
            packet.WriteByte((byte)stats.DefenseElement);
            packet.WriteByte(stats.MagicResistance);
            packet.Write(new byte[1]); //dunno
            packet.WriteSByte(stats.ArmorClass);
            packet.WriteByte(stats.Dmg);
            packet.WriteByte(stats.Hit);
        }

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ADMIN: {admin} | TYPE: {updateType}";
        return packet;
    }
    internal static ServerPacket ServerMessage(ServerMessageType type, string message)
    {
        var packet = new ServerPacket(ServerOpCode.ServerMessage);

        packet.WriteByte((byte)type);
        packet.WriteString16(message);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] TYPE: {type} | MESSAGE: {message}";
        return packet;
    }
    internal static ServerPacket ConfirmClientWalk(Direction direction, Point oldPoint)
    {
        var packet = new ServerPacket(ServerOpCode.ConfirmClientWalk);

        packet.WriteByte((byte)direction);
        packet.WritePoint16(oldPoint);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] DIRECTION: {direction} | OLD_POINT: {oldPoint}";
        return packet;
    }
    internal static ServerPacket CreatureWalk(int id, Point point, Direction direction)
    {
        var packet = new ServerPacket(ServerOpCode.CreatureWalk);

        packet.WriteInt32(id);
        packet.WritePoint16(point);
        packet.WriteByte((byte)direction);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ID: {id} | DIRECTION: {direction} | OLD_POINT: {point}";
        return packet;
    }
    internal static ServerPacket PublicChat(PublicMessageType type, int id, string message)
    {
        var packet = new ServerPacket(ServerOpCode.PublicMessage);

        packet.WriteByte((byte)type);
        packet.WriteInt32(id);
        packet.WriteString8(message);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] TYPE: {type} | SOURCE_ID: {id} | MESSAGE: {message}";
        return packet;
    }
    internal static ServerPacket RemoveObject(VisibleObject obj)
    {
        var packet = new ServerPacket(ServerOpCode.RemoveObject);

        packet.WriteInt32(obj.ID);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ID: {obj.ID}";
        return packet;
    }
    internal static ServerPacket AddItem(Item item)
    {
        var packet = new ServerPacket(ServerOpCode.AddItemToPane);

        packet.WriteByte(item.Slot);
        packet.WriteUInt16(item.ItemSprite.OffsetSprite);
        packet.WriteByte(item.Color);
        packet.WriteString8(item.Name);
        packet.WriteUInt32(item.Count);
        packet.WriteBoolean(item.Stackable);
        packet.WriteUInt32(item.MaxDurability);
        packet.WriteUInt32(item.CurrentDurability);
        if (item.Stackable)
            packet.WriteByte(0);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ITEM: {item}";
        return packet;
    }
    internal static ServerPacket RemoveItem(byte slot)
    {
        var packet = new ServerPacket(ServerOpCode.RemoveItemFromPane);

        packet.WriteByte(slot);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] SLOT: {slot}";
        return packet;
    }
    internal static ServerPacket CreatureTurn(int id, Direction direction)
    {
        var packet = new ServerPacket(ServerOpCode.CreatureTurn);

        packet.WriteInt32(id);
        packet.WriteByte((byte)direction);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ID: {id} | DIRECTION: {direction}";
        return packet;
    }
    internal static ServerPacket HealthBar(Creature obj)
    {
        var packet = new ServerPacket(ServerOpCode.HealthBar);

        packet.WriteInt32(obj.ID);
        packet.WriteByte(0); //i've seen this as 0 if you get hit by someone else, or 2 if you're hitting something else... but it doesnt change anything
        packet.WriteByte(obj.HealthPercent);
        //packet.WriteByte()  This byte indicates a sound to play when the hp bar hits.(1 for normal assail, etc) You can either (255) or leave off this byte entirely for no sound

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ID: {obj.ID} | HEALTH_PERCENT: {obj.HealthPercent}";
        return packet;
    }
    internal static ServerPacket MapInfo(Map map)
    {
        var packet = new ServerPacket(ServerOpCode.MapInfo);

        packet.WriteUInt16(map.Id);
        packet.WriteByte(map.SizeX);
        packet.WriteByte(map.SizeY);
        packet.Write(new byte[3]); //dunno
        packet.WriteByte((byte)(map.CheckSum / 256));
        packet.WriteByte((byte)(map.CheckSum % 256));
        packet.WriteString8(map.Name);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] {map}";
        return packet;
    }
    internal static ServerPacket AddSpell(Spell spell)
    {
        var packet = new ServerPacket(ServerOpCode.AddSpellToPane);

        packet.WriteByte(spell.Slot);
        packet.WriteUInt16(spell.Sprite);
        packet.WriteByte((byte)spell.SpellType);
        packet.WriteString8(spell.Name);
        packet.WriteString8(spell.Prompt);
        packet.WriteByte(spell.CastLines);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] {spell}";
        return packet;
    }
    internal static ServerPacket RemoveSpell(byte slot)
    {
        var packet = new ServerPacket(ServerOpCode.RemoveSpellFromPane);

        packet.WriteByte(slot);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] SLOT: {slot}";
        return packet;
    }
    internal static ServerPacket Sound(byte index)
    {
        var packet = new ServerPacket(ServerOpCode.Sound);

        packet.WriteByte(index);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] INDEX: {index}";
        return packet;
    }
    internal static ServerPacket AnimateCreature(int id, BodyAnimation animation, ushort speed, byte sound = 0xFF)
    {
        var packet = new ServerPacket(ServerOpCode.BodyAnimation);

        packet.WriteInt32(id);
        packet.WriteByte((byte)animation);
        packet.WriteUInt16(speed);
        packet.WriteByte(sound);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ID: {id} | ANIMATION: {animation}";
        return packet;
    }
    internal static ServerPacket MapChangeComplete()
    {
        var packet = new ServerPacket(ServerOpCode.MapChangeComplete);

        packet.Write(new byte[2]); //pretty sure these are nothing

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ";
        return packet;
    }
    internal static ServerPacket LightLevel(LightLevel lightLevel)
    {
        var packet = new ServerPacket(ServerOpCode.LightLevel);

        packet.WriteByte((byte)lightLevel);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] LIGHT_LEVEL: {lightLevel}";
        return packet;
    }
    internal static ServerPacket RefreshResponse()
    {
        var packet = new ServerPacket(ServerOpCode.RefreshResponse);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ";
        return packet;
    }
    internal static ServerPacket Animation(Animation animation)
    {
        var packet = new ServerPacket(ServerOpCode.Animation);

        if (animation.TargetPoint == Point.None)
        {
            packet.WriteInt32(animation.TargetId);
            packet.WriteInt32(animation.SourceId);
            packet.WriteUInt16(animation.TargetAnimation);
            packet.WriteUInt16(animation.SourceAnimation);
            packet.WriteUInt16(animation.AnimationSpeed);
        }
        else
        {
            packet.WriteUInt32(0U);
            packet.WriteUInt16(animation.TargetAnimation);
            packet.WriteUInt16(animation.AnimationSpeed);
            packet.WritePoint16(animation.TargetPoint);
        }

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] {animation}";
        return packet;
    }
    internal static ServerPacket AddSkill(Skill skill)
    {
        var packet = new ServerPacket(ServerOpCode.AddSkillToPane);

        packet.WriteByte(skill.Slot);
        packet.WriteUInt16(skill.Sprite);
        packet.WriteString8(skill.Name);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] {skill}";
        return packet;
    }
    internal static ServerPacket RemoveSkill(byte slot)
    {
        var packet = new ServerPacket(ServerOpCode.RemoveSkillFromPane);

        packet.WriteByte(slot);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] SLOT: {slot}";
        return packet;
    }
    internal static ServerPacket WorldMap(WorldMap worldMap)
    {
        var packet = new ServerPacket(ServerOpCode.WorldMap);

        packet.WriteString8(worldMap.Field);
        packet.WriteByte((byte)worldMap.Nodes.Count);
        packet.WriteByte(1); //image num
        foreach (var node in worldMap.Nodes)
        {
            packet.WritePoint16(node.Position);
            packet.WriteString8(node.Text);
            packet.WriteUInt16(node.CheckSum);
            packet.WriteUInt16(node.DestinationMapId);
            packet.WritePoint16(node.DestinationPoint);
        }

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] FIELD: {worldMap.Field} | NODES: {worldMap.Nodes.Count}";
        return packet;
    }
    internal static ServerPacket DisplayMenu(Client.Client client, Merchant merchant, Dialog dialog = null)
    {
        var packet = new ServerPacket(ServerOpCode.Menu);

        packet.WriteByte((byte)merchant.Menu.Type);
        packet.WriteByte((byte)GameObjectType.Merchant);
        packet.WriteInt32(merchant.ID);
        packet.WriteByte(1);
        packet.WriteUInt16(merchant.Sprite);
        packet.WriteByte(0);
        packet.WriteByte(1);
        packet.WriteUInt16(merchant.Sprite);
        packet.WriteByte(0);
        packet.WriteByte(0); //dunno?
        packet.WriteString8(merchant.Name);
        packet.WriteString16(merchant.Menu.Text);

        switch (merchant.Menu.Type)
        {
            case MenuType.Menu:
                packet.WriteByte((byte)merchant.Menu.Count);
                foreach (var pursuit in merchant.Menu.Pursuits)
                {
                    packet.WriteString8(pursuit.Text);
                    packet.WriteUInt16((ushort)pursuit.PursuitId);
                }
                break;
            case MenuType.MenuWithArgs:
                /*
                    writestring8(args)
                    foreachpursuit
                    {
                        writestring8 text
                        writeuint16 pursuitid
                    }
                
                break;
            case MenuType.TextEntry:
                /*
                    writeuint16 pursuitid
                
                break;
            case MenuType.ShowItems:
                /*
                    writeuint16 pursuitid
                    writeuint16 count

                    foreach item
                        writeuint16 sprite
                        writebyte color
                        writeuint32 itemCount
                        writestring8 name
                        writestring8 unknownStr
                
                break;
            case MenuType.ShowOwnedItems:
                /*
                    writeuint16 pursuitid
                    writeuint16 count

                    foreach occupiedSlot
                        writebyte slotnum
                
                break;
            case MenuType.ShowSpells:
                /*
                    writeuint16 pursuitid
                    writeuint16 count

                    foreach spell
                        writebyte 2
                        writeuint16 sprite
                        writebyte 0
                        writestring8 name
                
                break;
            case MenuType.ShowSkills:
                /*
                    writeuint16 pursuitid
                    writeuint16 count

                    foreach spell
                        writebyte 3
                        writeuint16 sprite
                        writebyte 0
                        writestring8 name
                
                break;
            case MenuType.ShowLearnedSpells:
                /*
                    writeuint16 pursuitid
                
                break;
            case MenuType.ShowLearnedSkills:
                /*
                    writeuint16 pursuitid
                
                break;
            case MenuType.Dialog:
                client.SendDialog(merchant, dialog);
                packet = null;
                break;
        }

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] TYPE: {merchant.Menu.Type} | ID: {merchant.ID} | MERCHANT: {merchant}";
        return packet;
    }
    internal static ServerPacket DisplayDialog(object invoker, Dialog dialog)
    {
        var packet = new ServerPacket(ServerOpCode.Dialog);

        var invokerID = 0;
        ushort invokerSprite = 0;
        var invokerName = "";

        packet.WriteByte((byte)(dialog?.Type ?? DialogType.CloseDialog));

        if (dialog == null || dialog.Type == DialogType.CloseDialog)
            return packet;

        if (invoker is Merchant tMerchant)
        {
            packet.WriteByte((byte)GameObjectType.Merchant);
            invokerID = tMerchant.ID;
            invokerSprite = tMerchant.Sprite;
            invokerName = tMerchant.Name;
        }
        else if (invoker is Item tItem)
        {
            packet.WriteByte((byte)GameObjectType.Item);
            invokerSprite = tItem.ItemSprite.OffsetSprite;
            invokerName = tItem.Name;
        }

        packet.WriteInt32(invokerID);
        packet.WriteByte(0);    //dunno
        packet.WriteUInt16(invokerSprite);
        packet.WriteByte(0);    //dunno
        packet.WriteByte(0);    //dunno - can be 1?
        packet.WriteUInt16(invokerSprite);  //this sprite id is used if non mundane, but can always populate both
        packet.WriteByte(0);    //dunno
        packet.WriteUInt16((ushort)dialog.PursuitId);
        packet.WriteUInt16(dialog.Id);
        packet.WriteBoolean(dialog.PrevBtn);
        packet.WriteBoolean(dialog.NextBtn);
        packet.WriteByte(0);    //dunno
        packet.WriteString8(invokerName);
        packet.WriteString16(dialog.Message);

        switch (dialog.Type)
        {
            case DialogType.Normal:
                break;
            case DialogType.ItemMenu:
                packet.WriteByte((byte)dialog.Menu.Count);

                foreach (var opt in dialog.Menu)
                    packet.WriteString8(opt.Text);
                break;
            case DialogType.TextEntry:
                packet.WriteUInt16(dialog.MaxCharacters);
                break;
            case DialogType.Speak:
                break;
            case DialogType.CreatureMenu:
                packet.WriteByte((byte)dialog.Menu.Count);

                foreach (var opt in dialog.Menu)
                    packet.WriteString8(opt.Text);
                break;
        }

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] TYPE: {dialog.Type} | {invoker}";
        return packet;
    }
    internal static ServerPacket BulletinBoard()
    {
        var packet = new ServerPacket(ServerOpCode.BulletinBoard);

        //boardtype(byte)
        //boardcount(ushort)
        //00?
        //00?
        //string8

        packet.WriteByte(1);
        packet.WriteByte(0);

        return packet;
    }
    internal static ServerPacket Door(params Door[] doors)
    {
        var packet = new ServerPacket(ServerOpCode.Door);

        packet.WriteByte((byte)doors.Length);
        foreach (var door in doors)
        {
            packet.WritePoint8(door.Point);
            packet.WriteBoolean(door.Closed);
            packet.WriteBoolean(door.OpenRight);
        }

        if (doors.Length > 0)
            packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] POINT: {doors[0].Point} CLOSED: {doors[0].Closed}";
        return packet;
    }
    internal static ServerPacket DisplayUser(User user)
    {
        var display = user.DisplayData;
        var packet = new ServerPacket(ServerOpCode.DisplayAisling);

        packet.WritePoint16(user.Point);
        packet.WriteByte((byte)user.Direction);
        packet.WriteInt32(user.ID);

        var flag = Status.Dead;
        var ded = Status.Dead;
            
        if (user.Sprite == 0 && !user.HasFlag(Status.Dead))
        {
            packet.WriteUInt16(display.HeadSprite);
            packet.WriteByte((byte)display.BodySprite);
            packet.WriteUInt16(display.ArmorSprite1);
            packet.WriteByte(display.BootsSprite);
            packet.WriteUInt16(display.ArmorSprite2);
            packet.WriteByte(display.ShieldSprite);
            packet.WriteUInt16(display.WeaponSprite);
            packet.WriteByte(display.HeadColor);
            packet.WriteByte(display.BootsColor);
            packet.WriteByte(display.AccessoryColor1);
            packet.WriteUInt16(display.AccessorySprite1);
            packet.WriteByte(display.AccessoryColor2);
            packet.WriteUInt16(display.AccessorySprite2);
            packet.WriteByte(display.AccessoryColor3);
            packet.WriteUInt16(display.AccessorySprite3);
            packet.WriteByte((byte)display.LanternSize);
            packet.WriteByte((byte)display.RestPosition);
            packet.WriteUInt16(display.OvercoatSprite);
            packet.WriteByte(display.OvercoatColor);
            packet.WriteByte((byte)display.BodyColor);
            packet.WriteBoolean(display.IsHidden);
            packet.WriteByte(display.FaceSprite);
        }
        else if (user.HasFlag(Status.Dead)) //if theyre dead theyre a ghost
        {
            packet.WriteUInt16(display.HairSprite);
            packet.WriteByte((byte)((display.BodySprite == BodySprite.Female) ? BodySprite.FemaleGhost : BodySprite.MaleGhost));
            packet.Write(new byte[25]);
            packet.WriteBoolean(true);
            packet.WriteByte(display.FaceSprite);
        }
        else
        {
            packet.WriteUInt16(ushort.MaxValue);
            packet.WriteUInt16((ushort)(user.Sprite + 16384U));
            packet.WriteByte(display.HeadColor);
            packet.WriteByte(display.BootsColor);
            packet.Write(new byte[6]); //dunno
        }
        packet.WriteByte((byte)display.NameTagStyle);
        packet.WriteString8((display.BodySprite == 0) ? string.Empty : (user.Name ?? string.Empty));
        packet.WriteString8(display.GroupBoxText ?? string.Empty);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] {user}";
        return packet;
    }

    //YES, THIS ONE
    internal static ServerPacket Profile(User user)
    {
        var packet = new ServerPacket(ServerOpCode.Profile);

        packet.WriteInt32(user.ID);
        foreach(var slot in CONSTANTS.PROFILE_EQUIPMENTSLOT_ORDER)
        {
            packet.WriteUInt16(user.Equipment[slot]?.ItemSprite.OffsetSprite ?? 0);
            packet.WriteByte(user.Equipment[slot]?.Color ?? 0);
        }
        packet.WriteByte((byte)user.SocialStatus);
        packet.WriteString8(user.Name);
        packet.WriteByte((byte)user.Nation);
        packet.WriteString8(user.Titles.FirstOrDefault() ?? "");
        packet.WriteBoolean(user.UserOptions.Group);
        packet.WriteString8(user.Guild?.TitleOf(user.Name) ?? "");
        packet.WriteString8((user.AdvClass == AdvClass.None) ? user.BaseClass.ToString() : user.AdvClass.ToString());
        packet.WriteString8(user.Guild?.Name ?? "");
        packet.WriteByte(user.Legend.Count);
        foreach(var mark in user.Legend)
        {
            packet.WriteByte((byte)mark.Icon);
            packet.WriteByte((byte)mark.Color);
            packet.WriteString8(mark.Key);
            packet.WriteString8(mark.ToString());
        }
        packet.WriteUInt16((ushort)(user.Personal.Portrait.Length + user.Personal.Message.Length + 4));
        packet.WriteData16(user.Personal.Portrait);
        packet.WriteString16(user.Personal.Message);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] {user}";
        return packet;
    }
    internal static ServerPacket WorldList(List<User> users, byte UserLevel)
    {
        var packet = new ServerPacket(ServerOpCode.WorldList);

        packet.WriteUInt16((ushort)users.Count);
        packet.WriteUInt16((ushort)users.Count);
        foreach(var user in users)
        {
            var range = (byte)((UserLevel / 5) + 3);
            packet.WriteByte((byte)user.BaseClass);
            packet.WriteByte((byte)((Math.Abs(user.Attributes.Level - UserLevel) <= range) ? 151 : 255)); //84 if guild
            packet.WriteByte((byte)user.SocialStatus);
            packet.WriteString8(user.Titles?.FirstOrDefault() ?? ""); 
            packet.WriteBoolean(user.IsMaster);
            packet.WriteString8(user.Name);
        }

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] COUNT: {users.Count} | CALLER_LEVEL: {UserLevel}";
        return packet;
    }
    internal static ServerPacket AddEquipment(Item item)
    {
        var packet = new ServerPacket(ServerOpCode.Equipment);

        packet.WriteByte((byte)item.EquipmentSlot);
        packet.WriteUInt16(item.ItemSprite.OffsetSprite);
        packet.WriteByte(item.Color);
        packet.WriteString8(item.Name);
        packet.WriteByte(0); //type...?
        packet.WriteUInt32(item.MaxDurability);
        packet.WriteUInt32(item.CurrentDurability);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] {item}";
        return packet;
    }
    internal static ServerPacket RemoveEquipment(EquipmentSlot slot)
    {
        var packet = new ServerPacket(ServerOpCode.Unequip);

        packet.WriteByte((byte)slot);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] SLOT: {slot}";
        return packet;
    }
    internal static ServerPacket ProfileSelf(User user)
    {
        var packet = new ServerPacket(ServerOpCode.SelfProfile);

        packet.WriteByte((byte)user.Nation);
        var s = user?.Guild?.TitleOf(user.Name);
        packet.WriteString8(user.Guild?.TitleOf(user.Name) ?? "");
        packet.WriteString8((user.Titles.Count > 0) ? user.Titles[0] : ""); 
        packet.WriteString8(user.Group?.ToString() ?? ((user.Spouse != null) ? $@"Spouse: {user.Spouse}" : "Adventuring alone"));
        packet.WriteBoolean(user.UserOptions.Group);
        packet.WriteBoolean(user.Group?.Box != null);
        if(user.Group?.Box != null)
        {
            packet.WriteString8(user.Group.Leader.Name);
            packet.WriteString8(user.Group.Box.Text);
            packet.Write(new byte[13]); //other groupbox stuff will add later
        }
        packet.WriteByte((byte)user.BaseClass);
        packet.WriteBoolean(user.AdvClass != AdvClass.None);
        packet.WriteBoolean(user.IsMaster);
        packet.WriteString8((user.AdvClass != AdvClass.None) ? user.AdvClass.ToString() 
            : user.IsMaster                                  ? "Master" 
                                                               : user.BaseClass.ToString()); //class string
        packet.WriteString8(user.Guild?.Name ?? "");
        packet.WriteByte(user.Legend.Count);
        foreach(var mark in user.Legend)
        {
            packet.WriteByte((byte)mark.Icon);
            packet.WriteByte((byte)mark.Color);
            packet.WriteString8(mark.Key);
            packet.WriteString8(mark.ToString());
        }

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] {user}";
        return packet;
    }
    internal static ServerPacket EffectsBar(ushort effect, EffectColor color)
    {
        var packet = new ServerPacket(ServerOpCode.Effect);

        packet.WriteUInt16(effect);
        packet.WriteByte((byte)color);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] EFFECT: {effect} | COLOR: {color}";
        return packet;
    }
    internal static ServerPacket KeepAlive(byte a, byte b)
    {
        var packet = new ServerPacket(ServerOpCode.KeepAlive);

        packet.WriteByte(a);
        packet.WriteByte(b);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] NUM1: {a} | NUM2: {b}";
        return packet;
    }
    internal static ServerPacket[] MapData(Map map)
    {
        var staggeredData = new List<ServerPacket>();
        var key = 0;
            
        for(ushort y = 0; y < map.SizeY; ++y)
        {
            var packet = new ServerPacket(ServerOpCode.MapData);
            packet.WriteUInt16(y);
            for (var x = 0; x < map.SizeX * 6; x += 2)
            {
                packet.WriteByte(map.Data[key + 1]);
                packet.WriteByte(map.Data[key]);
                key += 2;
            }
            packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] X_COUNT: {map.SizeX} | Y_INDEX: {y + 1} of {map.SizeY}";
            staggeredData.Add(packet);
        }

        return staggeredData.ToArray();
    }

    internal static ServerPacket Cooldown(PanelObject obj)
    {

        var packet = new ServerPacket(ServerOpCode.Cooldown);

        var cd = Utilities.Clamp<uint>(obj.Cooldown.Subtract(obj.Elapsed).TotalSeconds, 0d, obj.Cooldown.TotalSeconds);
        if (cd == 0 || obj is Item)
            return null;

        packet.WriteBoolean(obj is Skill);
        packet.WriteByte(obj.Slot);
        packet.WriteUInt32(cd);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] SECONDS: {cd} | {obj}";
        return packet;
    }
    internal static ServerPacket Exchange(ExchangeRequestType requestType, params object[] args)
    {
        var packet = new ServerPacket(ServerOpCode.Exchange);

        packet.WriteByte((byte)requestType);
        switch (requestType)
        {
            case ExchangeRequestType.StartExchange:
                packet.WriteInt32((int)args[0]);
                packet.WriteString8((string)args[1]);
                break;
            case ExchangeRequestType.RequestAmount:
                packet.WriteByte((byte)args[0]);
                break;
            case ExchangeRequestType.AddItem:
                packet.WriteBoolean((bool)args[0]);
                packet.WriteByte((byte)args[1]);
                packet.WriteUInt16((ushort)args[2]);
                packet.WriteByte((byte)args[3]);
                packet.WriteString8((string)args[4]);
                break;
            case ExchangeRequestType.SetGold:
                packet.WriteBoolean((bool)args[0]);
                packet.WriteUInt32((uint)args[1]);
                break;
            case ExchangeRequestType.Cancel:
                packet.WriteBoolean((bool)args[0]);
                packet.WriteString8("Exchange cancelled.");
                break;
            case ExchangeRequestType.Accept:
                packet.WriteBoolean((bool)args[0]);
                packet.WriteString8("You exchanged.");
                break;
        }

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] TYPE: {requestType} | ARGS: {string.Join(", ", args)}";
        return packet;
    }
    internal static ServerPacket CancelCasting()
    {
        var packet = new ServerPacket(ServerOpCode.CancelCasting);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ";
        return packet;
    }
    internal static ServerPacket RequestPersonal()
    {
        var packet = new ServerPacket(ServerOpCode.RequestPersonal);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ";
        return packet;
    }
    internal static ServerPacket ForceClientPacket(ClientPacket packetToForce)
    {
        var packet = new ServerPacket(ServerOpCode.ForceClientPacket);

        packet.WriteUInt16((ushort)(packetToForce.Length + 1));
        packetToForce.Position = 0;
        packet.WriteByte(packetToForce.OpCode);
        packet.WriteData(packetToForce.ReadBytes(packetToForce.Length));

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] OPCODE: {(ClientOpCode)packetToForce.OpCode}";
        return packet;
    }
    internal static ServerPacket ConfirmExit()
    {
        var packet = new ServerPacket(ServerOpCode.ConfirmExit);

        packet.WriteBoolean(true);
        packet.Write(new byte[2]);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ";
        return packet;
    }
    internal static ServerPacket ServerTable(byte[] serverTbl)
    {
        var packet = new ServerPacket(ServerOpCode.ServerTable);

        packet.WriteData16(serverTbl);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ";
        return packet;
    }
    internal static ServerPacket MapLoadComplete()
    {
        var packet = new ServerPacket(ServerOpCode.MapLoadComplete);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ";
        return packet;
    }
    internal static ServerPacket LoginNotification(bool full, uint loginMsgCheckSum = 0, byte[] notif = null)
    {
        var packet = new ServerPacket(ServerOpCode.LoginNotification);

        packet.WriteBoolean(full);
        if (full)
            packet.WriteData16(notif);
        else
            packet.WriteUInt32(loginMsgCheckSum);


        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] " + (full ? $@"REQUEST: {full}" : $@"CHKSUM: {loginMsgCheckSum}");
        return packet;
    }
    internal static ServerPacket GroupRequest(GroupRequestType type, string source)
    {
        var packet = new ServerPacket(ServerOpCode.GroupRequest);

        packet.WriteByte((byte)type);
        packet.WriteString8(source);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] TYPE: {type} | SOURCE: {source}";
        return packet;
    }
    internal static ServerPacket LobbyControls(byte type, string message)
    {
        var packet = new ServerPacket(ServerOpCode.LobbyMessage);

        packet.WriteByte(type);
        packet.WriteString8(message);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] TYPE: {type} | MESSAGE: {message}";
        return packet;
    }
    internal static ServerPacket MapChangePending()
    {
        var packet = new ServerPacket(ServerOpCode.MapChangePending);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ";
        return packet;
    }
    internal static ServerPacket SynchronizeTicks()
    {
        var packet = new ServerPacket(ServerOpCode.SynchronizeTicks);

        var ticks = Environment.TickCount;
        packet.WriteInt32(ticks);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] TICKS: {ticks}";
        return packet;
    }
    internal static ServerPacket[] Metafile(bool requestFile, params MetaFile[] metafiles)
    {
        var packets = new List<ServerPacket>();

        if (!requestFile)
        {
            var packet = new ServerPacket(ServerOpCode.Metafile);
            packet.WriteBoolean(true);
            packet.WriteUInt16((ushort)metafiles.Length);
            foreach (var metafile in metafiles)
            {
                packet.WriteString8(metafile.Name);
                packet.WriteUInt32(Crypto.Generate32(metafile.Data));
            }

            packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] REQUEST_FILE: {requestFile} | COUNT: {metafiles.Length}";
            packets.Add(packet);
        }
        else
        {
            foreach (var metafile in metafiles)
            {
                var packet = new ServerPacket(ServerOpCode.Metafile);
                packet.WriteBoolean(false);
                packet.WriteString8(metafile.Name);
                packet.WriteUInt32(Crypto.Generate32(metafile.Data));
                packet.WriteData16(metafile.Data);

                packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] REQUEST_FILE: {requestFile}";
                packets.Add(packet);
            }
        }

            
        return packets.ToArray();
    }

    internal static ServerPacket AcceptConnection()
    {
        var packet = new ServerPacket(ServerOpCode.AcceptConnection);

        packet.WriteByte(27);
        packet.WriteString("CONNECTED SERVER", true);

        packet.LogString = $@"Send [{(ServerOpCode)packet.OpCode}] ";
        return packet;
    }
}*/

