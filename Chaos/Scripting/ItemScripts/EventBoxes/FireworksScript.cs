using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ItemScripts.EventBoxes;

public class FireworksScript : ItemScriptBase
{

    private readonly IIntervalTimer FireworkDelay;
    private int AnimationCount;
    private bool FireworksBigBoom;
    private bool FireworksBigBurst;
    private bool FireworksBigSwirl;
    private bool FireworksBiggerBang;
    private bool FireworksBiggerBoom;
    private bool FireworksBiggerBurst;
    private bool FireworksBiggerSwirl;
    private bool FireworksBiggestBang;
    private bool FireworksBiggestBoom;
    private bool FireworksBiggestBurst;
    private bool FireworksBiggestSwirl;
    private Creature? SourceOfFirework;

    public FireworksScript(Item subject)
        : base(subject)
    {
        FireworkDelay = new IntervalTimer(TimeSpan.FromMilliseconds(500));
    }

    protected Animation Firework1 { get; } = new()
    {
        AnimationSpeed = 200,
        TargetAnimation = 294
    };
    
    protected Animation Firework2 { get; } = new()
    {
        AnimationSpeed = 200,
        TargetAnimation = 289
    };
    
    protected Animation Firework3 { get; } = new()
    {
        AnimationSpeed = 200,
        TargetAnimation = 304
    };
    
    protected Animation Firework4 { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 359
    };

    public override void OnUse(Aisling source)
    {
        var itemKey = Subject.Template.TemplateKey;
        SourceOfFirework = source;
        
        if (SourceOfFirework == null)
            return;

        if (source.Trackers.TimedEvents.HasActiveEvent("firework", out _))
            return;
        
        source.Inventory.RemoveQuantity(Subject.Slot, 1);
        
        switch (itemKey)
        {
            case "fireworksboom":
            {
                source.MapInstance.ShowAnimation(Firework1.GetPointAnimation(source));
                source.Trackers.TimedEvents.AddEvent("firework", TimeSpan.FromMilliseconds(350), true);

                break;
            }
            case "fireworksburst":
            {
                source.MapInstance.ShowAnimation(Firework2.GetPointAnimation(source));
                source.Trackers.TimedEvents.AddEvent("firework", TimeSpan.FromMilliseconds(350), true);
                
                break;
            }
            case "fireworksswirl":
            {
                source.MapInstance.ShowAnimation(Firework3.GetPointAnimation(source));
                source.Trackers.TimedEvents.AddEvent("firework", TimeSpan.FromMilliseconds(350), true);
                

                break;
            }
            case "fireworksbigbang":
            {
                source.MapInstance.ShowAnimation(Firework4.GetPointAnimation(source));
                source.Trackers.TimedEvents.AddEvent("firework", TimeSpan.FromMilliseconds(350), true);

                break;
            }
            case "fireworksbigboom":
            {
                FireworksBigBoom = true;
                source.Trackers.TimedEvents.AddEvent("firework", TimeSpan.FromMilliseconds(350), true);

                break;
            }
            case "fireworksbigburst":
            {
                FireworksBigBurst = true;
                source.Trackers.TimedEvents.AddEvent("firework", TimeSpan.FromMilliseconds(350), true);

                break;
            }
            case "fireworksbigswirl":
            {
                FireworksBigSwirl = true;
                source.Trackers.TimedEvents.AddEvent("firework", TimeSpan.FromMilliseconds(350), true);

                break;
            }
            case "fireworksbiggerbang":
            {
                FireworksBiggerBang = true;
                source.Trackers.TimedEvents.AddEvent("firework", TimeSpan.FromMilliseconds(350), true);

                break;
            }
            case "fireworksbiggerboom":
            {
                FireworksBiggerBoom = true;
                source.Trackers.TimedEvents.AddEvent("firework", TimeSpan.FromMilliseconds(350), true);

                break;
            }
            case "fireworksbiggerburst":
            {
                FireworksBiggerBurst = true;
                source.Trackers.TimedEvents.AddEvent("firework", TimeSpan.FromMilliseconds(350), true);

                break;
            }
            case "fireworksbiggerswirl":
            {
                FireworksBiggerSwirl = true;
                source.Trackers.TimedEvents.AddEvent("firework", TimeSpan.FromMilliseconds(350), true);
                break;
            }
            case "fireworksbiggestbang":
            {
                FireworksBiggestBang = true;
                source.Trackers.TimedEvents.AddEvent("firework", TimeSpan.FromMilliseconds(350), true);
                break;
            }
            case "fireworksbiggestboom":
            {
                FireworksBiggestBoom = true;
                source.Trackers.TimedEvents.AddEvent("firework", TimeSpan.FromMilliseconds(350), true);
                break;
            }
            case "fireworksbiggestburst":
            {
                FireworksBiggestBurst = true;
                source.Trackers.TimedEvents.AddEvent("firework", TimeSpan.FromMilliseconds(350), true);
                break;
            }
            case "fireworksbiggestswirl":
            {
                FireworksBiggestSwirl = true;
                source.Trackers.TimedEvents.AddEvent("firework", TimeSpan.FromMilliseconds(350), true);
                break;
            }
        }
    }

    public override void Update(TimeSpan delta)
    {
        FireworkDelay.Update(delta);
        
        if (SourceOfFirework == null)
            return;
        
        if (FireworksBigBoom && FireworkDelay.IntervalElapsed)
        {
            var rectangle = new Rectangle(
                SourceOfFirework.X - 2,
                SourceOfFirework.Y - 2,
                4,
                4);
            // Execute the animation at a random point
            var randomPoint = rectangle.GetRandomPoint();
            SourceOfFirework.MapInstance.ShowAnimation(Firework1.GetPointAnimation(randomPoint));

            // Increment the animation count
            AnimationCount++;

            // Check if we've reached the desired number of repetitions
            if (AnimationCount >= 3)
            {
                // Reset the state
                FireworksBigBoom = false;
                AnimationCount = 0;
            }
        }
        
        if (FireworksBigBurst && FireworkDelay.IntervalElapsed)
        {
            var rectangle = new Rectangle(
                SourceOfFirework.X - 2,
                SourceOfFirework.Y - 2,
                4,
                4);
            // Execute the animation at a random point
            var randomPoint = rectangle.GetRandomPoint();
            SourceOfFirework.MapInstance.ShowAnimation(Firework2.GetPointAnimation(randomPoint));

            // Increment the animation count
            AnimationCount++;

            // Check if we've reached the desired number of repetitions
            if (AnimationCount >= 3)
            {
                // Reset the state
                FireworksBigBurst = false;
                AnimationCount = 0;
            }
        }
        
        if (FireworksBigSwirl && FireworkDelay.IntervalElapsed)
        {
            var rectangle = new Rectangle(
                SourceOfFirework.X - 2,
                SourceOfFirework.Y - 2,
                4,
                4);
            // Execute the animation at a random point
            var randomPoint = rectangle.GetRandomPoint();
            SourceOfFirework.MapInstance.ShowAnimation(Firework3.GetPointAnimation(randomPoint));

            // Increment the animation count
            AnimationCount++;

            // Check if we've reached the desired number of repetitions
            if (AnimationCount >= 3)
            {
                // Reset the state
                FireworksBigSwirl = false;
                AnimationCount = 0;
            }
        }
        
        if (FireworksBiggerBang && FireworkDelay.IntervalElapsed)
        {
            var rectangle = new Rectangle(
                SourceOfFirework.X - 2,
                SourceOfFirework.Y - 2,
                4,
                4);
            // Execute the animation at a random point
            var randomPoint = rectangle.GetRandomPoint();
            SourceOfFirework.MapInstance.ShowAnimation(Firework4.GetPointAnimation(randomPoint));

            // Increment the animation count
            AnimationCount++;

            // Check if we've reached the desired number of repetitions
            if (AnimationCount >= 3)
            {
                // Reset the state
                FireworksBiggerBang = false;
                AnimationCount = 0;
            }
        }
        
        if (FireworksBiggerBoom && FireworkDelay.IntervalElapsed)
        {
            var rectangle = new Rectangle(
                SourceOfFirework.X - 2,
                SourceOfFirework.Y - 2,
                4,
                4);
            // Execute the animation at a random point
            var randomPoint = rectangle.GetRandomPoint();
            SourceOfFirework.MapInstance.ShowAnimation(Firework1.GetPointAnimation(randomPoint));

            // Increment the animation count
            AnimationCount++;

            // Check if we've reached the desired number of repetitions
            if (AnimationCount >= 7)
            {
                // Reset the state
                FireworksBiggerBoom = false;
                AnimationCount = 0;
            }
        }
        
        if (FireworksBiggerBurst && FireworkDelay.IntervalElapsed)
        {
            var rectangle = new Rectangle(
                SourceOfFirework.X - 2,
                SourceOfFirework.Y - 2,
                4,
                4);
            // Execute the animation at a random point
            var randomPoint = rectangle.GetRandomPoint();
            SourceOfFirework.MapInstance.ShowAnimation(Firework2.GetPointAnimation(randomPoint));

            // Increment the animation count
            AnimationCount++;

            // Check if we've reached the desired number of repetitions
            if (AnimationCount >= 7)
            {
                // Reset the state
                FireworksBiggerBurst = false;
                AnimationCount = 0;
            }
        }
        
        if (FireworksBiggerSwirl && FireworkDelay.IntervalElapsed)
        {
            var rectangle = new Rectangle(
                SourceOfFirework.X - 2,
                SourceOfFirework.Y - 2,
                4,
                4);
            // Execute the animation at a random point
            var randomPoint = rectangle.GetRandomPoint();
            SourceOfFirework.MapInstance.ShowAnimation(Firework3.GetPointAnimation(randomPoint));

            // Increment the animation count
            AnimationCount++;

            // Check if we've reached the desired number of repetitions
            if (AnimationCount >= 7)
            {
                // Reset the state
                FireworksBiggerSwirl = false;
                AnimationCount = 0;
            }
        }
        
        if (FireworksBiggestBang && FireworkDelay.IntervalElapsed)
        {
            var rectangle = new Rectangle(
                SourceOfFirework.X - 2,
                SourceOfFirework.Y - 2,
                4,
                4);
            // Execute the animation at a random point
            var randomPoint = rectangle.GetRandomPoint();
            SourceOfFirework.MapInstance.ShowAnimation(Firework4.GetPointAnimation(randomPoint));

            // Increment the animation count
            AnimationCount++;

            // Check if we've reached the desired number of repetitions
            if (AnimationCount >= 7)
            {
                // Reset the state
                FireworksBiggestBang = false;
                AnimationCount = 0;
            }
        }
        
        if (FireworksBiggestBoom && FireworkDelay.IntervalElapsed)
        {
            var rectangle = new Rectangle(
                SourceOfFirework.X - 2,
                SourceOfFirework.Y - 2,
                4,
                4);
            // Execute the animation at a random point
            var randomPoint = rectangle.GetRandomPoint();
            SourceOfFirework.MapInstance.ShowAnimation(Firework1.GetPointAnimation(randomPoint));

            // Increment the animation count
            AnimationCount++;

            // Check if we've reached the desired number of repetitions
            if (AnimationCount >= 12)
            {
                // Reset the state
                FireworksBiggestBoom = false;
                AnimationCount = 0;
            }
        }
        
        if (FireworksBiggestBurst && FireworkDelay.IntervalElapsed)
        {
            var rectangle = new Rectangle(
                SourceOfFirework.X - 2,
                SourceOfFirework.Y - 2,
                4,
                4);
            // Execute the animation at a random point
            var randomPoint = rectangle.GetRandomPoint();
            SourceOfFirework.MapInstance.ShowAnimation(Firework2.GetPointAnimation(randomPoint));

            // Increment the animation count
            AnimationCount++;

            // Check if we've reached the desired number of repetitions
            if (AnimationCount >= 12)
            {
                // Reset the state
                FireworksBiggestBurst = false;
                AnimationCount = 0;
            }
        }
        
        if (FireworksBiggestSwirl && FireworkDelay.IntervalElapsed)
        {
            var rectangle = new Rectangle(
                SourceOfFirework.X - 2,
                SourceOfFirework.Y - 2,
                4,
                4);
            // Execute the animation at a random point
            var randomPoint = rectangle.GetRandomPoint();
            SourceOfFirework.MapInstance.ShowAnimation(Firework3.GetPointAnimation(randomPoint));

            // Increment the animation count
            AnimationCount++;

            // Check if we've reached the desired number of repetitions
            if (AnimationCount >= 12)
            {
                // Reset the state
                FireworksBiggestSwirl = false;
                AnimationCount = 0;
            }
        }
    }
}