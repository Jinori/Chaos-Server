using Chaos.Collections;
using Chaos.Collections.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Map;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.Panel.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.Components.Execution;

public class ComponentVars : StaticVars
{
    private const string CASCADE_ALL_POINTS_KEY = "all_points";
    private const string CASCADE_STAGE_KEY = "cascade_stage";
    private const string OPTIONS_KEY = "options";
    private const string POINTS_KEY = "points";
    private const string TARGETS_KEY = "targets";
    private const string SUBJECT_KEY = "subject";

    public virtual List<IPoint> GetAllPoints() => GetRequired<List<IPoint>>(CASCADE_ALL_POINTS_KEY);

    public virtual TOptions GetOptions<TOptions>() => GetRequired<TOptions>(OPTIONS_KEY);

    public virtual T GetSubject<T>() => GetRequired<T>(SUBJECT_KEY);

    public virtual IReadOnlyCollection<IPoint> GetPoints() => GetRequired<IReadOnlyCollection<IPoint>>(POINTS_KEY);

    public virtual int GetStage() => GetRequired<int>(CASCADE_STAGE_KEY);

    public virtual IReadOnlyCollection<T> GetTargets<T>()
        => GetRequired<IReadOnlyCollection<MapEntity>>(TARGETS_KEY)
           .OfType<T>()
           .ToList();

    public virtual void SetAllPoints(List<IPoint> points) => Set(CASCADE_ALL_POINTS_KEY, points);
    public virtual void SetOptions(object options)
    {
        switch (options)
        {
            case SubjectiveScriptBase<Spell> spellScript:
                SetSubject(spellScript.Subject);
                
                break;
            case SubjectiveScriptBase<Item> itemScript:
                SetSubject(itemScript.Subject);
                
                break;
            case SubjectiveScriptBase<Skill> skillScript:
                SetSubject(skillScript.Subject);
                
                break;
            case SubjectiveScriptBase<ReactorTile> reactorScript:
                SetSubject(reactorScript.Subject);
                
                break;
                
            case SubjectiveScriptBase<MapInstance> mapInstanceScript:
                SetSubject(mapInstanceScript.Subject);
                
                break;
            
            case SubjectiveScriptBase<BulletinBoard> bulletinScript:
                SetSubject(bulletinScript.Subject);
                break;
            
            case SubjectiveScriptBase<Monster> monsterScript:
                SetSubject(monsterScript.Subject);
                break;
            
            case SubjectiveScriptBase<Aisling> aislingScript:
                SetSubject(aislingScript.Subject);
                break;
            
            case SubjectiveScriptBase<Merchant> merchantScript:
                SetSubject(merchantScript.Subject);
                break;
            
            case SubjectiveScriptBase<Dialog> dialogScript:
                SetSubject(dialogScript.Subject);
                break;
            
            
        }
        
        Set(OPTIONS_KEY, options);
    }

    public virtual void SetPoints(IReadOnlyCollection<IPoint> points) => Set(POINTS_KEY, points);

    public virtual void SetStage(int stage) => Set(CASCADE_STAGE_KEY, stage);

    public virtual void SetTargets(IReadOnlyCollection<MapEntity> targets) => Set(TARGETS_KEY, targets);

    public virtual void SetSubject(object subject) => Set(SUBJECT_KEY, subject);
}