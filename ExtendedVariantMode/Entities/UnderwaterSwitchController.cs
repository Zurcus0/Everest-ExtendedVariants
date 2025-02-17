﻿using Celeste;
using ExtendedVariants.Module;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;
using System.Reflection;

namespace ExtendedVariants.Entities {
    /// <summary>
    /// Heavily based on https://github.com/EverestAPI/SpringCollab2020/blob/master/Entities/UnderwaterSwitchController.cs,
    /// except controlled by the extended variant setting instead of a session flag.
    /// </summary>
    public class UnderwaterSwitchController : Entity {
        private static FieldInfo waterFill = typeof(Water).GetField("fill", BindingFlags.NonPublic | BindingFlags.Instance);

        private ExtendedVariantsSettings settings;
        private Water water;

        public UnderwaterSwitchController(ExtendedVariantsSettings settings) : base(Vector2.Zero) {
            this.settings = settings;

            Add(new Coroutine(Routine()));
            AddTag(Tags.TransitionUpdate);
        }

        public IEnumerator Routine() {
            Session session = SceneAs<Level>().Session;
            while (true) {
                // wait until the variant is enabled.
                while (!settings.EverythingIsUnderwater) {
                    yield return null;
                }

                // spawn water.
                if (water == null) {
                    spawnWater(session.LevelData.Bounds);
                }

                // wait until the variant is disabled, or the mod is turned off.
                while (settings.EverythingIsUnderwater && settings.MasterSwitch) {
                    yield return null;
                }

                // make water go away.
                Scene.Remove(water);
                water = null;

                // if the mod was turned off, destroy the controller.
                if (!settings.MasterSwitch) {
                    RemoveSelf();
                    yield break;
                }
            }
        }

        private void spawnWater(Rectangle levelBounds) {
            // flood the room with water, make the water 10 pixels over the top to prevent a "splash" effect when going in a room above.
            water = new Water(new Vector2(levelBounds.Left, levelBounds.Top - 10),
                false, false, levelBounds.Width, levelBounds.Height + 10);

            // but we don't want the water to render off-screen, because it is visible on upwards transitions.
            Rectangle fill = (Rectangle) waterFill.GetValue(water);
            fill.Y += 10;
            fill.Height -= 10;
            waterFill.SetValue(water, fill);

            Scene.Add(water);
        }
    }
}
