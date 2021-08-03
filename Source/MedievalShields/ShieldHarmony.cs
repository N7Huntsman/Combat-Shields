using System;
using Verse;
using RimWorld;
using HarmonyLib;

namespace CombatShields
{
    [StaticConstructorOnStartup]
    class Harmonypatches
    {
        private static readonly Type shieldPatchType = typeof(Harmonypatches);

        static Harmonypatches()
        {
            Harmony h = new Harmony("ShieldHarmony");

            h.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.AddEquipment)),
            postfix: new HarmonyMethod(shieldPatchType, nameof(ShieldPatchAddEquipment)));

            h.Patch(AccessTools.Method(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Wear)),
           postfix: new HarmonyMethod(shieldPatchType, nameof(ShieldPatchWearApparel)));
        }

        public static void ShieldPatchAddEquipment(Pawn_EquipmentTracker __instance, ThingWithComps newEq)
        {
            Pawn owner = __instance.pawn;

            // must have picked up a weapon
            if (PawnHasShieldEquiped(owner) || PawnHasShieldInInventory(owner))
            {
                if (newEq.def.IsWeapon)
                {
                    if (PawnHasValidEquipped(owner) && PawnHasShieldInInventory(owner))
                    {
                        for (int i = 0; i < owner.inventory.innerContainer.Count; i++)
                        {
                            if (owner.inventory.innerContainer[i].def.thingCategories == null)
                            {
                                continue;
                            }
                            foreach (ThingCategoryDef ApparelItem in owner.inventory.innerContainer[i].def.thingCategories)
                            {
                                if (ApparelItem.defName == "Shield")
                                {
                                    Thing whocares;
                                    __instance.pawn.inventory.innerContainer.TryDrop(owner.inventory.innerContainer[i], ThingPlaceMode.Direct, out whocares, null, null);
                                    owner.apparel.Wear(whocares as Apparel);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (PawnHasShieldEquiped(owner) && !PawnHasValidEquipped(owner))
                        {
                            Apparel shield = null;
                            // do we have a shield equipped

                            for (int i = 0; i < owner.inventory.innerContainer.Count; i++)
                            {
                                if (owner.inventory.innerContainer[i].def.thingCategories == null)
                                {
                                    continue;
                                }
                                foreach (ThingCategoryDef ApparelItem in owner.inventory.innerContainer[i].def.thingCategories)
                                {
                                    if (ApparelItem.defName == "Shield")
                                    {
                                        Thing whocares;
                                        __instance.pawn.inventory.innerContainer.TryDrop(owner.inventory.innerContainer[i], ThingPlaceMode.Direct, out whocares, null, null);
                                    }
                                }
                            }

                            for (int i = 0; i < owner.apparel.WornApparelCount; i++)
                            {
                                if (owner.apparel.WornApparel[i].def.thingCategories == null)
                                {
                                    continue;
                                }
                                foreach (ThingCategoryDef ApparelItem in owner.apparel.WornApparel[i].def.thingCategories)
                                {
                                    if (ApparelItem.defName == "Shield")
                                    {
                                        shield = owner.apparel.WornApparel[i];
                                    }
                                }
                            }

                            // we have a shield equipped
                            if (shield != null)
                            {
                                owner.apparel.Remove(shield);
                                owner.inventory.innerContainer.TryAddOrTransfer(shield, false);
                            }
                        }

                    }
                }
            }
        }

        public static void ShieldPatchWearApparel(Pawn_EquipmentTracker __instance, Apparel newApparel)
        {
            bool shield = false;
            // for apparel with no thingcategory defined
            if(newApparel.def.thingCategories == null)
            {
                return;
            }

            foreach (ThingCategoryDef ApparelItem in newApparel.def.thingCategories)
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

            Pawn owner = __instance.pawn;

            // must have picked up a weapon
            if (PawnHasShieldInInventory(owner))
            {
                // do we have a shield equipped

                for (int i = 0; i < owner.inventory.innerContainer.Count; i++)
                {
                    if(owner.inventory.innerContainer[i].def.thingCategories == null)
                    {
                        continue;
                    }
                    foreach (ThingCategoryDef ApparelItem in owner.inventory.innerContainer[i].def.thingCategories)
                    {
                        if (ApparelItem.defName == "Shield")
                        {
                            Thing whocares;
                            __instance.pawn.inventory.innerContainer.TryDrop(owner.inventory.innerContainer[i], ThingPlaceMode.Direct, out whocares, null, null);
                        }
                    }
                }
                Apparel wornshield = null;

                for (int i = 0; i < owner.apparel.WornApparelCount; i++)
                {
                    if (owner.apparel.WornApparel[i].def.thingCategories == null)
                    {
                        continue;
                    }
                    foreach (ThingCategoryDef ApparelItem in owner.apparel.WornApparel[i].def.thingCategories)
                    {
                        if (ApparelItem.defName == "Shield")
                        {
                            wornshield = owner.apparel.WornApparel[i];
                        }
                    }
                }

                // we have a shield equipped
                if (wornshield != null)
                {
                    owner.apparel.Remove(wornshield);
                    owner.inventory.innerContainer.TryAddOrTransfer(wornshield, false);
                }
            }
            else
            {
                if (!PawnHasValidEquipped(owner))
                {
                    Apparel wornshield = null;

                    for (int i = 0; i < owner.apparel.WornApparelCount; i++)
                    {
                        if (owner.apparel.WornApparel[i].def.thingCategories == null)
                        {
                            continue;
                        }
                        foreach (ThingCategoryDef ApparelItem in owner.apparel.WornApparel[i].def.thingCategories)
                        {
                            if (ApparelItem.defName == "Shield")
                            {
                                wornshield = owner.apparel.WornApparel[i];
                            }
                        }
                    }

                    // we have a shield equipped
                    if (wornshield != null)
                    {
                        owner.apparel.Remove(wornshield);
                        owner.inventory.innerContainer.TryAddOrTransfer(wornshield, false);
                    }
                }
            }
        }

        // check if a pawn has a shield equipped
        public static bool PawnHasShieldEquiped(Pawn pawn)
        {
            bool revalue = false;

            Apparel shield = null;
            // do we have a shield equipped
            foreach (Apparel a in pawn.apparel.WornApparel)
            {
                if (a.def.thingCategories == null)
                {
                    continue;
                }
                foreach (ThingCategoryDef ApparelItem in a.def.thingCategories)
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
            foreach (Apparel a in pawn.apparel.WornApparel)
            {
                foreach (ThingCategoryDef ApparelItem in a.def.thingCategories)
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
            bool revalue = false;

            foreach (Thing a in pawn.inventory.innerContainer)
            {
                if (a.def.thingCategories == null)
                {
                    continue;
                }
                foreach (ThingCategoryDef ApparelItem in a.def.thingCategories)
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
            bool reValue = false;

            if (newEquipment.def.thingCategories == null)
            {
                return false;
            }
            foreach (ThingCategoryDef ApparelItem in newEquipment.def.thingCategories)
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
            if ((GetPawnSheild(pawn)?.def.apparel.tags.Contains("Light_Shield") ?? false))
            {
                // if this is a light shield only allow light sidearms
                return pawn.equipment.Primary.def.weaponTags.Any(t => t == "LightShield_Sidearm");
            }
            // by default don't allow ranged weapons or weapons with Shield_NoSidearm or "LightShield_Sidearm without a light shield"
            return !pawn.equipment.Primary.def.IsRangedWeapon && !pawn.equipment.Primary.def.weaponTags.Any(t => t == "Shield_NoSidearm" || t == "LightShield_Sidearm");
        }
    }
}

