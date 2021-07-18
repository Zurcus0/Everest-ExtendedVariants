﻿using Celeste.Mod;
using Monocle;
using MonoMod.Utils;
using NLua;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedVariants {
    // a leak prevention patch that's way too hacky to have its place in Everest.
    // instead of fixing the root issue, it tries to mitigate it by making NLua forget about any entity that isn't in the scene anymore.
    internal static class LeakPreventionHack {
        private static Dictionary<object, int> nluaReferenceMap;

        private static ObjectTranslator nluaObjectTranslator;
        private static MethodInfo nluaCollectObject;

        static LeakPreventionHack() {
            if (Everest.LuaLoader.Context != null) {
                // break NLua open and get its reference map. it stays the same, so we only have to do that once.
                nluaObjectTranslator = new DynData<Lua>(Everest.LuaLoader.Context).Get<ObjectTranslator>("_translator");
                nluaReferenceMap = new DynData<ObjectTranslator>(nluaObjectTranslator).Get<Dictionary<object, int>>("_objectsBackMap");
                nluaCollectObject = typeof(ObjectTranslator).GetMethod("CollectObject", BindingFlags.NonPublic | BindingFlags.Instance, null,
                    CallingConventions.Any, new Type[] { typeof(int) }, null);
            }
        }

        public static void Load() {
            On.Monocle.Entity.Removed += onEntityRemoved;
            On.Celeste.Level.End += onLevelEnd;
        }

        public static void Unload() {
            On.Monocle.Entity.Removed -= onEntityRemoved;
            On.Celeste.Level.End -= onLevelEnd;
        }

        private static void onEntityRemoved(On.Monocle.Entity.orig_Removed orig, Entity self, Scene scene) {
            orig(self, scene);
            clearUpReferencesToEntity(self);
        }

        private static void clearUpReferencesToEntity(Entity self) {
            if (nluaReferenceMap != null && nluaReferenceMap.TryGetValue(self, out int entityRef)) {
                // it seems NLua can't dispose entities by itself, so we need to help it a bit.
                Logger.Log("ExtendedVariantMode/LeakPreventionHack", $"Cleaning up reference of NLua to {self.GetType().FullName} {entityRef}");
                nluaCollectObject.Invoke(nluaObjectTranslator, new object[] { entityRef });
            }
        }

        private static void onLevelEnd(On.Celeste.Level.orig_End orig, Celeste.Level self) {
            foreach (Entity entity in self.Entities) {
                // we're quitting the level, so we need to get rid of all of its entities.
                clearUpReferencesToEntity(entity);
            }

            orig(self);

            if (nluaReferenceMap != null && nluaReferenceMap.TryGetValue(self, out int levelRef)) {
                // it seems NLua can't dispose entities by itself, so we need to help it a bit.
                Logger.Log("ExtendedVariantMode/LeakPreventionHack", $"Cleaning up reference of NLua to {self.GetType().FullName} {levelRef}");
                nluaCollectObject.Invoke(nluaObjectTranslator, new object[] { levelRef });
            }
        }
    }
}
