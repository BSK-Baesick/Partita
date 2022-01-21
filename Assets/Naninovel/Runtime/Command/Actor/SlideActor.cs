// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using UniRx.Async;
using UnityEngine;

namespace Naninovel.Commands
{
    /// <summary>
    /// Slides (moves between two positions) an actor (character, background, text printer or choice handler) with the provided ID and optionally changes actor visibility and appearance.
    /// Can be used instead of multiple [@char] or [@back] commands to reveal or hide an actor with a slide animation.
    /// </summary>
    /// <remarks>
    /// Be aware, that this command searches for an existing actor with the provided ID over all the actor managers, 
    /// and in case multiple actors with the same ID exist (eg, a character and a text printer), this will affect only the first found one.
    /// Make sure the actor exist on scene before referencing it with this command; 
    /// eg, if it's a character, you can add it on scene imperceptibly to player with `@char CharID visible:false time:0`.
    /// </remarks>
    /// <example>
    /// ; Given `Jenna` actor is not currently visible, reveal it with an `Angry` appearance
    /// ; and slide to the center of the screen from either left or right border of the screen.
    /// @slide Jenna.Angry to:50
    /// 
    /// ; Given `Sheba` actor is currently visible,
    /// ; hide and slide it out of the screen over the left border.
    /// @slide Sheba to:-10 visible:false
    /// 
    /// ; Slide `Mia` actor from left-center side of the screen to the right-bottom 
    /// ; over 5 seconds using `EaseOutBounce` animation easing.
    /// @slide Sheba from:15,50 to:85,0 time:5 easing:EaseOutBounce
    /// </example>
    [CommandAlias("slide")]
    public class SlideActor : Command
    {
        /// <summary>
        /// ID of the actor to slide and (optionally) appearance to set.
        /// </summary>
        [ParameterAlias(NamelessParameterAlias), RequiredParameter, IDEActor(namedIndex: 0), IDEAppearance(1)]
        public NamedStringParameter IdAndAppearance;
        /// <summary>
        /// Position in scene space to slide the actor from (slide start position).
        /// Described as follows: `0,0` is the bottom left, `50,50` is the center and `100,100` is the top right corner of the screen; Z-component (depth) is in world space.
        /// When not provided, will use current actor position in case it's visible and a random off-screen position otherwise (could slide-in from left or right borders).
        /// </summary>
        [ParameterAlias("from")]
        public DecimalListParameter FromPosition;
        /// <summary>
        /// Position in scene space to slide the actor to (slide finish position).
        /// </summary>
        [ParameterAlias("to"), RequiredParameter]
        public DecimalListParameter ToPosition;
        /// <summary>
        /// Change visibility status of the actor (show or hide).
        /// When not set and target actor is hidden, will still automatically show it.
        /// </summary>
        public BooleanParameter Visible;
        /// <summary>
        /// Name of the easing function to use for the modifications.
        /// <br/><br/>
        /// Available options: Linear, SmoothStep, Spring, EaseInQuad, EaseOutQuad, EaseInOutQuad, EaseInCubic, EaseOutCubic, EaseInOutCubic, EaseInQuart, EaseOutQuart, EaseInOutQuart, EaseInQuint, EaseOutQuint, EaseInOutQuint, EaseInSine, EaseOutSine, EaseInOutSine, EaseInExpo, EaseOutExpo, EaseInOutExpo, EaseInCirc, EaseOutCirc, EaseInOutCirc, EaseInBounce, EaseOutBounce, EaseInOutBounce, EaseInBack, EaseOutBack, EaseInOutBack, EaseInElastic, EaseOutElastic, EaseInOutElastic.
        /// <br/><br/>
        /// When not specified, will use a default easing function set in the actor's manager configuration settings.
        /// </summary>
        [ParameterAlias("easing"), IDEConstant(IDEConstantAttribute.Easing)]
        public StringParameter EasingTypeName;
        /// <summary>
        /// Duration (in seconds) of the slide animation. Default value: 0.35 seconds.
        /// </summary>
        [ParameterAlias("time")]
        public DecimalParameter Duration = .35f;

        public override async UniTask ExecuteAsync (CancellationToken cancellationToken = default)
        {
            var actorId = IdAndAppearance.Name;
            var manager = Engine.GetAllServices<IActorManager>(c => c.ActorExists(actorId)).FirstOrDefault();

            if (manager is null)
            {
                LogErrorWithPosition($"Can't find a manager with `{actorId}` actor.");
                return;
            }

            var tasks = new List<UniTask>();

            var cameraConfig = Engine.GetConfiguration<CameraConfiguration>();
            var actor = manager.GetActor(actorId);

            var fromPos = new Vector3(
                FromPosition?.ElementAtOrNull(0)?.HasValue ?? false ? cameraConfig.SceneToWorldSpace(new Vector2(FromPosition[0] / 100f, 0)).x : 
                    actor.Visible ? actor.Position.x : cameraConfig.SceneToWorldSpace(new Vector2(Random.value > .5f ? -.1f : 1.1f, 0)).x,
                FromPosition?.ElementAtOrNull(1)?.HasValue ?? false ? cameraConfig.SceneToWorldSpace(new Vector2(0, FromPosition[1] / 100f)).y : actor.Position.y,
                FromPosition?.ElementAtOrNull(2) ?? actor.Position.z);

            var toPos = new Vector3(
                ToPosition.ElementAtOrNull(0)?.HasValue ?? false ? cameraConfig.SceneToWorldSpace(new Vector2(ToPosition[0] / 100f, 0)).x : actor.Position.x,
                ToPosition.ElementAtOrNull(1)?.HasValue ?? false ? cameraConfig.SceneToWorldSpace(new Vector2(0, ToPosition[1] / 100f)).y : actor.Position.y,
                ToPosition.ElementAtOrNull(2) ?? actor.Position.z);

            var easingType = manager.ActorManagerConfiguration.DefaultEasing;
            if (Assigned(EasingTypeName) && !System.Enum.TryParse(EasingTypeName, true, out easingType))
                LogWarningWithPosition($"Failed to parse `{EasingTypeName}` easing.");

            actor.Position = fromPos;

            if (!actor.Visible)
            {
                if (IdAndAppearance.NamedValue.HasValue)
                    actor.Appearance = IdAndAppearance.NamedValue;
                Visible = true;
            }
            else if (IdAndAppearance.NamedValue.HasValue)
                tasks.Add(actor.ChangeAppearanceAsync(IdAndAppearance.NamedValue, Duration, easingType, null, cancellationToken));

            if (Assigned(Visible)) tasks.Add(actor.ChangeVisibilityAsync(Visible, Duration, easingType, cancellationToken));

            tasks.Add(actor.ChangePositionAsync(toPos, Duration, easingType, cancellationToken));

            await UniTask.WhenAll(tasks);
        }
    } 
}
