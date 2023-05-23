using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class SummonMerchantScript : ConfigurableSpellScriptBase
{
    private readonly IMerchantFactory MerchantFactory;
    
    /// <inheritdoc />
    public SummonMerchantScript(Spell subject, IMerchantFactory merchantFactory)
        : base(subject) =>
        MerchantFactory = merchantFactory;

    public override void OnUse(SpellContext context)
    {
        var merchantsNear = context.Source.MapInstance.GetEntities<Merchant>();
        var mapEntities = merchantsNear as Merchant[] ?? merchantsNear.ToArray();
        
        foreach (var gloop in mapEntities)
        {
            if (gloop.Template.TemplateKey == "Gloop" && gloop.Name.Contains(context.Source.Name))
            {
                gloop.MapInstance.RemoveObject(gloop);
            }
        }

        var merchant = MerchantFactory.Create("gloop", context.SourceMap, context.SourcePoint);
        merchant.Name = context.SourceAisling?.Name + "'s Gloop";
        context.Source.MapInstance.AddObject(merchant, new Point(context.Source.X, context.Source.Y));
    }
}