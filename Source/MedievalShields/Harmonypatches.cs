using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatShields
{
    [StaticConstructorOnStartup]
    internal class Harmonypatches
    {
        private static readonly Type shieldPatchType = typeof(Harmonypatches);

        static Harmonypatches()
        {
            var h = new Harmony("ShieldHarmony");

            h.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.AddEquipment)),
                postfix: new HarmonyMethod(shieldPatchType, nameof(ShieldPatchAddEquipment)));

            h.Patch(AccessTools.Method(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Wear)),
                postfix: new HarmonyMethod(shieldPatchType, nameof(ShieldPatchWearApparel)));
        }

        public static void ShieldPatchAddEquipment(Pawn_EquipmentTracker __instance, ThingWithComps newEq)
        {
            var owner = __instance.pawn;

            // must have picked up a weapon
            if (!PawnHasShieldEquiped(owner) && !PawnHasShieldInInventory(owner))
            {
                return;
            }

            if (!newEq.def.IsWeapon)
            {
                return;
            }

            if (PawnHasValidEquipped(owner) && PawnHasShieldInInventory(owner))
            {
                for (var i = 0; i < owner.inventory.innerContainer.Count; i++)
                {
                    if (owner.inventory.innerContainer[i].def.thingCategories == null)
                    {
                        continue;
                    }

                    foreach (var ApparelItem in owner.inventory.innerContainer[i].def.thingCategories)
                    {
                        if (ApparelItem.defName != "Shield")
                        {
                            continue;
                        }

                        __instance.pawn.inventory.innerContainer.TryDrop(owner.inventory.innerContainer[i],
                            ThingPlaceMode.Direct, out var whocares);
                        owner.apparel.Wear(whocares as Apparel);
                    }
                }
            }
            else
            {
                if (!PawnHasShieldEquiped(owner) || PawnHasValidEquipped(owner))
                {
                    return;
                }

                Apparel shield = null;
                // do we have a shield equipped

                for (var i = 0; i < owner.inventory.innerContainer.Count; i++)
                {
                    if (owner.inventory.innerContainer[i].def.thingCategories == null)
                    {
                        continue;
                    }

                    foreach (var ApparelItem in owner.inventory.innerContainer[i].def.thingCategories)
                    {
                        if (ApparelItem.defName != "Shield")
                        {
                            continue;
                        }

                        __instance.pawn.inventory.innerContainer.TryDrop(
                            owner.inventory.innerContainer[i], ThingPlaceMode.Direct, out _);
                    }
                }

                for (var i = 0; i < owner.apparel.WornApparelCount; i++)
                {
                    if (owner.apparel.WornApparel[i].def.thingCategories == null)
                    {
                        continue;
                    }

                    foreach (var ApparelItem in owner.apparel.WornApparel[i].def.thingCategories)
                    {
                        if (ApparelItem.defName == "Shield")
                        {
                            shield = owner.apparel.WornApparel[i];
                        }
                    }
                }

                // we have a shield equipped
                if (shield == null)
                {
                    return;
                }

                owner.apparel.Remove(shield);
                owner.inventory.innerContainer.TryAddOrTransfer(shield, false);
            }
        }

        public static void ShieldPatchWearApparel(Pawn_EquipmentTracker __instance, Apparel newApparel)
        {
            var shield = false;
            // for apparel with no thingcategory defined
            if (newApparel.def.thingCategories == null)
            {
                return;
            }

            foreach (var ApparelItem in newApparel.def.thingCategories)
            {
                // we have a shield in the inventory
                if (ApparelItem.defName == "Shield")
                {
                    shield = true;
                }
            }

            if (!shield)
            {
                return;
            }

            var owner = __instance.pawn;

            // must have picked up a weapon
            if (PawnHasShieldInInventory(owner))
            {
                // do we have a shield equipped

                for (var i = 0; i < owner.inventory.innerContainer.Count; i++)
                {
                    if (owner.inventory.innerContainer[i].def.thingCategories == null)
                    {
                        continue;
                    }

                    foreach (var ApparelItem in owner.inventory.innerContainer[i].def.thingCategories)
                    {
                        if (ApparelItem.defName != "Shield")
                        {
                            continue;
                        }

                        __instance.pawn.inventory.innerContainer.TryDrop(owner.inventory.innerContainer[i],
                            ThingPlaceMode.Direct, out _);
                    }
                }

                Apparel wornshield = null;

                for (var i = 0; i < owner.apparel.WornApparelCount; i++)
                {
                    if (owner.apparel.WornApparel[i].def.thingCategories == null)
                    {
                        continue;
                    }

                    foreach (var ApparelItem in owner.apparel.WornApparel[i].def.thingCategories)
                    {
                        if (ApparelItem.defName == "Shield")
                        {
                            wornshield = owner.apparel.WornApparel[i];
                        }
                    }
                }

                // we have a shield equipped
                if (wornshield == null)
                {
                    return;
                }

                owner.apparel.Remove(wornshield);
                owner.inventory.innerContainer.TryAddOrTransfer(wornshield, false);
            }
            else
            {
                if (PawnHasValidEquipped(owner))
                {
                    return;
                }

                Apparel wornshield = null;

                for (var i = 0; i < owner.apparel.WornApparelCount; i++)
                {
                    if (owner.apparel.WornApparel[i].def.thingCategories == null)
                    {
                        continue;
                    }

                    foreach (var ApparelItem in owner.apparel.WornApparel[i].def.thingCategories)
                    {
                        if (ApparelItem.defName == "Shield")
                        {
                            wornshield = owner.apparel.WornApparel[i];
                        }
                    }
                }

                // we have a shield equipped
                if (wornshield == null)
                {
                    return;
                }

                owner.apparel.Remove(wornshield);
                owner.inventory.innerContainer.TryAddOrTransfer(wornshield, false);
            }
        }

        // check if a pawn has a shield equipped
        public static bool PawnHasShieldEquiped(Pawn pawn)
        {
            var revalue = false;

            Apparel shield = null;
            // do we have a shield equipped
            foreach (var a in pawn.apparel.WornApparel)
            {
                if (a.def.thingCategories == null)
                {
                    continue;
                }

                foreach (var ApparelItem in a.def.thingCategories)
                {
                    if (ApparelItem.defName == "Shield")
                    {
                        shield = a;
                    }
                }
            }

            // we have a shield equipped
            if (shield != null)
            {
                revalue = true;
            }

            return revalue;
        }

        // check if a pawn has a shield equipped
        public static Apparel GetPawnSheild(Pawn pawn)
        {
            Apparel shield = null;
            // do we have a shield equipped
            foreach (var a in pawn.apparel.WornApparel)
            {
                foreach (var ApparelItem in a.def.thingCategories)
                {
                    if (ApparelItem.defName == "Shield")
                    {
                        shield = a;
                    }
                }
            }

            return shield;
        }

        // check if a pawn has a shield in inventory
        public static bool PawnHasShieldInInventory(Pawn pawn)
        {
            var revalue = false;

            foreach (var a in pawn.inventory.innerContainer)
            {
                if (a.def.thingCategories == null)
                {
                    continue;
                }

                foreach (var ApparelItem in a.def.thingCategories)
                {
                    // we have a shield in the inventory
                    if (ApparelItem.defName == "Shield")
                    {
                        revalue = true;
                    }
                }
            }

            return revalue;
        }

        // check if the pawn picked up a shield
        public static bool PawnPickedUpAShield(ThingWithComps newEquipment)
        {
            var reValue = false;

            if (newEquipment.def.thingCategories == null)
            {
                return false;
            }

            foreach (var ApparelItem in newEquipment.def.thingCategories)
            {
                if (ApparelItem.defName == "Shield")
                {
                    reValue = true;
                }
            }

            return reValue;
        }

        // check if equiped weapon can be used with shield
        public static bool PawnHasValidEquipped(Pawn pawn)
        {
            if (pawn.equipment == null)
            {
                // can use shield without a weapon
                return false;
            }

            if (pawn.equipment.Primary == null)
            {
                // can use shield without a weapon
                return true;
            }

            if (pawn.equipment.Primary.def.weaponTags.Any(t => t == "Shield_Sidearm"))
            {
                // if a weapon is a light sidearm and a shield is a light shield return true
                return true;
            }

            if (GetPawnSheild(pawn)?.def.apparel.tags.Contains("Light_Shield") ?? false)
            {
                // if this is a light shield only allow light sidearms
                return pawn.equipment.Primary.def.weaponTags.Any(t => t == "LightShield_Sidearm");
            }

            // by default don't allow ranged weapons or weapons with Shield_NoSidearm or "LightShield_Sidearm without a light shield"
            return !pawn.equipment.Primary.def.IsRangedWeapon &&
                   !pawn.equipment.Primary.def.weaponTags.Any(
                       t => t == "Shield_NoSidearm" || t == "LightShield_Sidearm");
        }
    }
}