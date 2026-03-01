using System.ComponentModel.DataAnnotations;

using Beutl.Graphics;
using Beutl.Graphics.Effects;
using Beutl.Graphics.Particles;
using Beutl.Graphics.Transformation;
using Beutl.Language;
using Beutl.Media;
using Beutl.Operation;

namespace Beutl.Operators.Source;

[Display(Name = nameof(Strings.ParticleEmitter), ResourceType = typeof(Strings))]
public sealed class ParticleEmitterOperator : PublishOperator<ParticleEmitter>
{
    protected override void FillProperties()
    {
        // Emitter
        AddProperty(Value.Seed);
        AddProperty(Value.EmitterShape);
        AddProperty(Value.EmitterWidth, 100f);
        AddProperty(Value.EmitterHeight, 100f);
        AddProperty(Value.MaxParticles, 5000);

        // Emission
        AddProperty(Value.EmissionRate, 60f);
        AddProperty(Value.Lifetime, 2f);
        AddProperty(Value.LifetimeRandom);

        // Velocity
        AddProperty(Value.Speed, 200f);
        AddProperty(Value.SpeedRandom);
        AddProperty(Value.Direction, -90f);
        AddProperty(Value.Spread, 30f);

        // Physics
        AddProperty(Value.Gravity, 200f);
        AddProperty(Value.AirResistance);
        AddProperty(Value.TurbulenceStrength);
        AddProperty(Value.TurbulenceScale, 0.01f);
        AddProperty(Value.TurbulenceSpeed, 1f);

        // Visual
        AddProperty(Value.ParticleDrawable);
        AddProperty(Value.ParticleSize, 10f);
        AddProperty(Value.SizeRandom);
        AddProperty(Value.ParticleColor, Colors.White);
        AddProperty(Value.ParticleOpacity, 100f);

        // Rotation
        AddProperty(Value.InitialRotation);
        AddProperty(Value.InitialRotationRandom);
        AddProperty(Value.AngularVelocity);

        // Over Life
        AddProperty(Value.EndSizeMultiplier, 1f);
        AddProperty(Value.EndOpacityMultiplier);
        AddProperty(Value.EndColor, Colors.White);
        AddProperty(Value.UseEndColor, false);

        // Standard Drawable properties
        AddProperty(Value.Transform, new TransformGroup());
        AddProperty(Value.AlignmentX);
        AddProperty(Value.AlignmentY);
        AddProperty(Value.TransformOrigin);
        AddProperty(Value.Fill, new SolidColorBrush(Colors.White));
        AddProperty(Value.FilterEffect, new FilterEffectGroup());
        AddProperty(Value.BlendMode);
        AddProperty(Value.Opacity);
    }
}
