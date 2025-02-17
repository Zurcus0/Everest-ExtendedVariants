﻿using Celeste;
using Celeste.Mod.Entities;
using ExtendedVariants.Module;
using Microsoft.Xna.Framework;
using Monocle;

namespace ExtendedVariants.Entities {
    [CustomEntity("ExtendedVariantMode/TheoCrystal")]
    [Tracked(true)]
    public class ExtendedVariantTheoCrystal : TheoCrystal {
        // true if Theo can be left behind, false if the player is blocked if they leave Theo behind, null if it was spawned through the extended variant
        public bool AllowLeavingBehind { get; private set; } = false;
        public bool SpawnedAsEntity { get; private set; } = false;

        public static Entity Load(Level level, LevelData levelData, Vector2 offset, EntityData entityData) {
            ExtendedVariantTheoCrystal crystal;
            if (entityData.Bool("allowThrowingOffscreen")) {
                crystal = new ExtendedVariantTheoCrystalGoingOffscreen(entityData.Position + offset);
            } else {
                crystal = new ExtendedVariantTheoCrystal(entityData.Position + offset);
            }
            crystal.SpawnedAsEntity = true;
            crystal.AllowLeavingBehind = entityData.Bool("allowLeavingBehind");
            return crystal;
        }

        public ExtendedVariantTheoCrystal(Vector2 position) : base(position) {
            RemoveTag(Tags.TransitionUpdate); // I still don't know why vanilla Theo has this, but this causes issues with leaving him behind.
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            if (SpawnedAsEntity) {
                foreach (ExtendedVariantTheoCrystal entity in Scene.Tracker.GetEntities<ExtendedVariantTheoCrystal>()) {
                    if (entity != this && entity.Hold.IsHeld) {
                        RemoveSelf();
                    }
                }
            }
        }

        public override void Update() {
            Level level = SceneAs<Level>();

            // prevent the crystal from going offscreen by the right as well
            // (that's the only specificity of Extended Variant Theo Crystal.)
            if (Right > level.Bounds.Right) {
                Right = level.Bounds.Right;
                Speed.X *= -0.4f;
            }

            base.Update();

            // commit remove self if the variant is disabled mid-screen and we weren't spawned as an entity
            if (!SpawnedAsEntity && !ExtendedVariantsModule.Settings.TheoCrystalsEverywhere) {
                RemoveSelf();
            }
        }
    }
}
