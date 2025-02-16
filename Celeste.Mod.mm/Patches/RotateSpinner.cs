﻿#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

using Microsoft.Xna.Framework;
using Monocle;
using MonoMod;
using System;

namespace Celeste {
    class patch_RotateSpinner : RotateSpinner {

        public new float Angle {
            [MonoModReplace]
            get => fixAngle ?
                MathHelper.Lerp(MathHelper.Pi, -MathHelper.Pi, Easer(rotationPercent)) :
                MathHelper.Lerp(MathHelper.Pi + MathHelper.Pi / 2, -MathHelper.Pi / 2, Easer(rotationPercent));
        }

        private bool fixAngle;

        private Vector2 center;
        private float rotationPercent;
        private float length;

        private Vector2 startCenter;
        private Vector2 startPosition;

        public patch_RotateSpinner(EntityData data, Vector2 offset)
            : base(data, offset) {
            // no-op. MonoMod ignores this - we only need this to make the compiler shut up.
        }

        public extern void orig_ctor(EntityData data, Vector2 offset);
        [MonoModConstructor]
        public void ctor(EntityData data, Vector2 offset) {
            orig_ctor(data, offset);

            startCenter = data.Nodes[0] + offset;
            startPosition = data.Position + offset;
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);

            fixAngle = (Scene as Level).Session.Area.GetLevelSet() != "Celeste";

            if (fixAngle) {
                float angle = Calc.Angle(startCenter, startPosition);
                angle = Calc.WrapAngle(angle);
                rotationPercent = EaserInverse(Calc.Percent(angle, MathHelper.Pi, -MathHelper.Pi));
                Position = center + Calc.AngleToVector(Angle, length);
            }
        }

        [MonoModIgnore]
        private extern float Easer(float v);

        [MonoModIgnore]
        private extern float EaserInverse(float v);

    }
}
