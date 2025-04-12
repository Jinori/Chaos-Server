using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Utilities;

namespace Chaos.Scripting.DialogScripts;

public class KnightScript : DialogScriptBase
{
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    private readonly IItemFactory ItemFactory;
    private readonly IEffectFactory EffectFactory;

    public KnightScript(Dialog subject, IClientRegistry<IChaosWorldClient> clientRegistry, IItemFactory itemFactory, IEffectFactory effectFactory)
        : base(subject)
    {
        ClientRegistry = clientRegistry;
        ItemFactory = itemFactory;
        EffectFactory = effectFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        if (source.IsAdmin)
        {
            switch (Subject.Template.TemplateKey.ToLower())
            {
                case "caelion_initial":
                {
                    var option = new DialogOption
                    {
                        DialogKey = "caelion_knightcivics",
                        OptionText = "Knight Civics"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "caelion_silenceplayer",
                        OptionText = "Silence an Aisling"
                    };
                    
                    var option2 = new DialogOption
                    {
                        DialogKey = "caelion_grantspeech",
                        OptionText = "Grant Speech"
                    };
                    
                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                    
                    if (!Subject.HasOption(option1.OptionText))
                        Subject.Options.Add(option1);
                    
                    if (!Subject.HasOption(option2.OptionText))
                        Subject.Options.Add(option2);
                }
                    break;
            }   
        }

        if (source.IsKnight)
        {
            switch (Subject.Template.TemplateKey.ToLower())
            {
                case "caelion_initial":
                {
                    var option1 = new DialogOption
                    {
                        DialogKey = "caelion_silenceplayer",
                        OptionText = "Silence an Aisling"
                    };
                    
                    var option = new DialogOption
                    {
                        DialogKey = "caelion_grantspeech",
                        OptionText = "Grant Speech"
                    };
                    
                    if (!Subject.HasOption(option1.OptionText))
                        Subject.Options.Add(option1);
                    
                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }
                    break;
            }   
        }

    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLowerInvariant())
        {
            case "caelion_admitknight":
                HandleKnightPromotion(source);
                break;

            case "caelion_retireknight":
                HandleKnightRetirement(source);
                break;

            case "caelion_removeknight":
                HandleKnightRemoval(source);
                break;
            
            case "caelion_silenceplayer":
                HandleSilencePlayer(source);
                break;
            
            case "caelion_grantspeech":
                HandleGrantSpeech(source);
                break;
        }
    }

    private void HandleGrantSpeech(Aisling source)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var aislingName))
        {
            source.SendOrangeBarMessage(DialogString.UnknownInput);
            Subject.Close(source);
            return;
        }
        
        var target = ClientRegistry.FirstOrDefault(client => client.Aisling.Name.EqualsI(aislingName))?.Aisling;

        if (target is null)
        {
            Subject.Reply(source, $"{aislingName} is not currently available or in the realm.");
            return;
        }

        target.Effects.Dispel("KnightSilence");
        Subject.Reply(source, $"{target.Name}'s silence has been dispelled.");  
    }
    
    private void HandleSilencePlayer(Aisling source)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var aislingName))
        {
            source.SendOrangeBarMessage(DialogString.UnknownInput);
            Subject.Close(source);
            return;
        }
        
        var target = ClientRegistry.FirstOrDefault(client => client.Aisling.Name.EqualsI(aislingName))?.Aisling;

        if (target is null)
        {
            Subject.Reply(source, $"{aislingName} is not currently available or in the realm.");
            return;
        }

        var silence = EffectFactory.Create("KnightSilence");
        target.Effects.Apply(source, silence);
        
        Subject.Reply(source, $"{target.Name} has been silenced for one hour.");  
    }
    
    private void HandleKnightRemoval(Aisling source)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var aislingName))
        {
            source.SendOrangeBarMessage(DialogString.UnknownInput);
            Subject.Close(source);
            return;
        }
        
        var target = ClientRegistry.FirstOrDefault(client => client.Aisling.Name.EqualsI(aislingName))?.Aisling;

        if (target is null)
        {
            Subject.Reply(source, $"{aislingName} is not currently available or in the realm.");
            return;
        }

        if (target.IsKnight)
        {
            TryRemoveKnightLegendMark(target);
            TryRemoveKnightHelmet(target);
            TryRemoveKnightOutfit(target);
            TryRemoveKnightTitle(target);
            AnnounceKnightRemovalToUnora(target);
        
            Subject.Reply(source, $"{target.Name} has been removed in the name of the Everlight.");   
        }
    }
    
    private void HandleKnightRetirement(Aisling source)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var aislingName))
        {
            source.SendOrangeBarMessage(DialogString.UnknownInput);
            Subject.Close(source);
            return;
        }
        
        var target = ClientRegistry.FirstOrDefault(client => client.Aisling.Name.EqualsI(aislingName))?.Aisling;

        if (target is null)
        {
            Subject.Reply(source, $"{aislingName} is not currently available or in the realm.");
            return;
        }

        if (target.IsKnight)
        {
            TryRemoveKnightLegendMark(target);
            TryRemoveKnightHelmet(target);
            TryRemoveKnightOutfit(target);
            TryGiveKnightRetirementMark(target);
            TryGiveRetirementTitle(target);
            AnnounceKnightRetirementToUnora(target);
        
            Subject.Reply(source, $"{target.Name} has been retired in the name of the Everlight.");   
        }
    }
    
    private void HandleKnightPromotion(Aisling source)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var aislingName))
        {
            source.SendOrangeBarMessage(DialogString.UnknownInput);
            Subject.Close(source);
            return;
        }

        var target = ClientRegistry.FirstOrDefault(client => client.Aisling.Name.EqualsI(aislingName))?.Aisling;

        if (target is null)
        {
            Subject.Reply(source, $"{aislingName} is not currently available or in the realm.");
            return;
        }

        if (!target.IsKnight)
        {
            PromoteToKnight(target);
            Subject.Reply(source, $"{target.Name} has been knighted in the name of the Everlight.");    
        }
    }

    private void PromoteToKnight(Aisling aisling)
    {
        aisling.IsKnight = true;

        TryGiveKnightOutfit(aisling);
        TryGiveKnightHelmet(aisling);
        TryGiveKnightLegendMark(aisling);
        TryGiveKnightTitle(aisling);
        AnnounceKnightToUnora(aisling);
    }

    private void TryGiveKnightLegendMark(Aisling aisling) => aisling.Legend.AddUnique(new LegendMark("Knight of Unora", "unoraKnight", MarkIcon.Warrior, MarkColor.Yellow, 1, GameTime.Now));

    private void TryRemoveKnightLegendMark(Aisling aisling)
    {
        aisling.Legend.Remove("unoraKnight", out _);
        aisling.Legend.Remove("ServedKnights", out _);
    }
    
    private void TryGiveKnightRetirementMark(Aisling aisling) => aisling.Legend.AddOrAccumulate(new LegendMark("Served Knights of Everlight", "ServedKnights", MarkIcon.Heart, MarkColor.Yellow, 1, GameTime.Now));


    private void AnnounceKnightToUnora(Aisling aisling)
    {
        var message = $"With sword in hand, {aisling.Name} rises as a Knight of Unora.";

        foreach (var player in ClientRegistry.Select(c => c.Aisling))
        {
            player.SendServerMessage(ServerMessageType.OrangeBar2, message);
            player.Animate(new Animation()
            {
                AnimationSpeed = 100,
                TargetAnimation = 93,
                Priority = 100
            });
        }
    }
    
    private void AnnounceKnightRemovalToUnora(Aisling aisling)
    {
        var message = $"{aisling.Name} has been stripped of their blade of Everlight.";

        foreach (var player in ClientRegistry.Select(c => c.Aisling))
        {
            player.SendServerMessage(ServerMessageType.OrangeBar2, message);

            player.Animate(new Animation
            {
                AnimationSpeed = 100,
                TargetAnimation = 7,
                Priority = 100
            });
        }
    }
    
    private void AnnounceKnightRetirementToUnora(Aisling aisling)
    {
        var message = $"{aisling.Name} has laid down their blade of Everlight.";

        foreach (var player in ClientRegistry.Select(c => c.Aisling))
        {
            player.SendServerMessage(ServerMessageType.OrangeBar2, message);

            player.Animate(new Animation
            {
                AnimationSpeed = 100,
                TargetAnimation = 93,
                Priority = 100
            });
        }
    }
    
    private void TryGiveKnightTitle(Aisling aisling)
    {
        const string KNIGHT_TITLE = "Knight of Unora";

        // Remove any existing instance
        aisling.Titles.Remove(KNIGHT_TITLE);

        // Insert at the top
        aisling.Titles.Insert(0, KNIGHT_TITLE);

        // Update client
        aisling.Client.SendSelfProfile();
    }

    private void TryRemoveKnightTitle(Aisling aisling)
    {
        const string KNIGHT_TITLE = "Knight of Unora";
        const string RETIRED_KNIGHT_TITLE = "Retired Knight";

        // Remove any existing instance
        aisling.Titles.Remove(KNIGHT_TITLE);
        aisling.Titles.Remove(RETIRED_KNIGHT_TITLE);
        
        // Update client
        aisling.Client.SendSelfProfile();
    }
    
    private void TryGiveRetirementTitle(Aisling aisling)
    {
        const string KNIGHT_TITLE = "Knight of Unora";
        const string RETIRED_KNIGHT_TITLE = "Retired Knight";

        // Remove any existing instance
        aisling.Titles.Remove(KNIGHT_TITLE);
        aisling.Titles.Remove(RETIRED_KNIGHT_TITLE);

        // Insert at the top
        aisling.Titles.Insert(0, RETIRED_KNIGHT_TITLE);

        // Update client
        aisling.Client.SendSelfProfile();
    }
    

    private void TryGiveKnightHelmet(Aisling aisling)
    {
        var helm = ItemFactory.Create("knighthat");
        aisling.GiveItemOrSendToBank(helm);
    }
    private void TryRemoveKnightHelmet(Aisling aisling)
    {
        const string HELM_TEMPLATE_KEY = "knighthat";
        const string HELM_DISPLAY_NAME = "Knight Chainmail Hood";

        // Try to remove from inventory by template key
        if (aisling.Inventory.RemoveByTemplateKey(HELM_TEMPLATE_KEY))
            return;

        // Try to remove by name from bank
        if (aisling.Bank.Contains(HELM_DISPLAY_NAME))
        {
            aisling.Bank.TryWithdraw(HELM_DISPLAY_NAME, 1, out var item);
            if (item is not null)
                aisling.Inventory.RemoveByTemplateKey(HELM_TEMPLATE_KEY);
            return;
        }

        // Try to remove from equipped gear
        aisling.Equipment.TryGetRemove(HELM_DISPLAY_NAME, out _);
    }

    
    private void TryGiveKnightOutfit(Aisling aisling)
    {
        var itemKey = aisling.UserStatSheet.BaseClass switch
        {
            BaseClass.Warrior => aisling.Gender == Gender.Male ? "malewarriorknight" : "femalewarriorknight",
            BaseClass.Rogue   => aisling.Gender == Gender.Male ? "malerogueknight" : "femalerogueknight",
            BaseClass.Wizard  => aisling.Gender == Gender.Male ? "malewizardknight" : "femalewizardknight",
            BaseClass.Priest  => aisling.Gender == Gender.Male ? "malepriestknight" : "femalepriestknight",
            BaseClass.Monk  => aisling.Gender == Gender.Male ? "malemonkknight" : "femalemonkknight",
            _                 => string.Empty
        };

        if (!string.IsNullOrEmpty(itemKey))
        {
            var outfit = ItemFactory.Create(itemKey);
            aisling.GiveItemOrSendToBank(outfit);
        }
    }
    
    private void TryRemoveKnightOutfit(Aisling aisling)
    {
        var outfitKey = aisling.UserStatSheet.BaseClass switch
        {
            BaseClass.Warrior => aisling.Gender == Gender.Male ? "malewarriorknight" : "femalewarriorknight",
            BaseClass.Rogue   => aisling.Gender == Gender.Male ? "malerogueknight" : "femalerogueknight",
            BaseClass.Wizard  => aisling.Gender == Gender.Male ? "malewizardknight" : "femalewizardknight",
            BaseClass.Priest  => aisling.Gender == Gender.Male ? "malepriestknight" : "femalepriestknight",
            BaseClass.Monk    => aisling.Gender == Gender.Male ? "malemonkknight" : "femalemonkknight",
            _                 => string.Empty
        };

        if (string.IsNullOrEmpty(outfitKey))
            return;

        // Attempt to remove from inventory
        if (aisling.Inventory.RemoveByTemplateKey(outfitKey))
            return;

        // Attempt to remove from bank
        if (aisling.Bank.Contains(outfitKey))
        {
            aisling.Bank.TryWithdraw(outfitKey, 1, out var item);

            if (item is not null)
                aisling.Inventory.RemoveByTemplateKey(outfitKey);
            return;
        }
        
        // Try to remove from equipped gear
        aisling.Equipment.RemoveByTemplateKey(outfitKey);
    }

}
