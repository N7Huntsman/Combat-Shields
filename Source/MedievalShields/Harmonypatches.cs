using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatShields;

[StaticConstructorOnStartup]
internal class Harmonypatches
{
    private static readonly Type shieldPatchType = typeof(Harmonypatches);

    public static readonly List<ThingDef> AllRangedWeapons;
    public static readonly List<ThingDef> AllBaseShieldableWeapons;
    public static readonly List<ThingDef> AllBaseLightShieldableWeapons;
    public static readonly List<ThingDef> AllShields;
    public static readonly List<ThingDef> AllLightShields;


    static Harmonypatches()
    {
        var h = new Harmony("ShieldHarmony");
        h.Patch(AccessTools.Method(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.AddEquipment)),
            postfix: new HarmonyMethod(shieldPatchType, nameof(ShieldPatchAddEquipment)));
        h.Patch(AccessTools.Method(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Wear)),
            postfix: new HarmonyMethod(shieldPatchType, nameof(ShieldPatchWearApparel)));

        AllRangedWeapons = DefDatabase<ThingDef>.AllDefsListForReading
            .Where(def => def.IsRangedWeapon && !def.destroyOnDrop).OrderBy(def => def.label).ToList();
        AllBaseShieldableWeapons =
            AllRangedWeapons.Where(def => def.weaponTags?.Contains("Shield_Sidearm") == true).ToList();
        AllBaseLightShieldableWeapons = AllRangedWeapons
            .Where(def => def.weaponTags?.Contains("LightShield_Sidearm") == true).ToList();
        AllShields = DefDatabase<ThingDef>.AllDefsListForReading.Where(IsShield).OrderBy(def => def.label).ToList();
        AllLightShields = AllShields.Where(def => def.apparel?.tags.Contains("Light_Shield") == true)
            .OrderBy(def => def.label).ToList();

        if (CombatShieldsMod.instance.Settings.ShieldUse?.Any() == true)
        {
            foreach (var shieldUseKey in CombatShieldsMod.instance.Settings.ShieldUse.Keys)
            {
                var weapon = DefDatabase<ThingDef>.GetNamedSilentFail(shieldUseKey);
                if (weapon == null)
                {
                    continue;
                }

                if (CombatShieldsMod.instance.Settings.ShieldUse[shieldUseKey])
                {
                    if (weapon.weaponTags == null)
                    {
                        weapon.weaponTags = new List<string>();
                    }

                    if (weapon.weaponTags.Contains("Shield_Sidearm"))
                    {
                        continue;
                    }

                    weapon.weaponTags.Add("Shield_Sidearm");
                    if (weapon.weaponTags.Contains("LightShield_Sidearm"))
                    {
                        weapon.weaponTags.Remove("LightShield_Sidearm");
                    }

                    continue;
                }

                if (weapon.weaponTags?.Contains("Shield_Sidearm") == true)
                {
                    weapon.weaponTags.Remove("Shield_Sidearm");
                }
            }
        }

        if (CombatShieldsMod.instance.Settings.LightShieldUse?.Any() != true)
        {
            return;
        }

        foreach (var shieldUseKey in CombatShieldsMod.instance.Settings.LightShieldUse.Keys)
        {
            var weapon = DefDatabase<ThingDef>.GetNamedSilentFail(shieldUseKey);
            if (weapon == null)
            {
                continue;
            }

            if (CombatShieldsMod.instance.Settings.LightShieldUse[shieldUseKey])
            {
                if (weapon.weaponTags == null)
                {
                    weapon.weaponTags = new List<string>();
                }

                if (weapon.weaponTags.Contains("LightShield_Sidearm"))
                {
                    continue;
                }

                weapon.weaponTags.Add("LightShield_Sidearm");
                if (weapon.weaponTags.Contains("Shield_Sidearm"))
                {
                    weapon.weaponTags.Remove("Shield_Sidearm");
                }

                continue;
            }

            if (weapon.weaponTags?.Contains("LightShield_Sidearm") == true)
            {
                weapon.weaponTags.Remove("LightShield_Sidearm");
            }
        }
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
                if (!IsShield(owner.inventory.innerContainer[i].def))
                {
                    continue;
                }

                __instance.pawn.inventory.innerContainer.TryDrop(owner.inventory.innerContainer[i],
                    ThingPlaceMode.Direct, out var whocares);
                owner.apparel.Wear(whocares as Apparel);
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
                if (!IsShield(owner.inventory.innerContainer[i].def))
                {
                    continue;
                }

                __instance.pawn.inventory.innerContainer.TryDrop(
                    owner.inventory.innerContainer[i], ThingPlaceMode.Direct, out _);
            }

            for (var i = 0; i < owner.apparel.WornApparelCount; i++)
            {
                if (!IsShield(owner.apparel.WornApparel[i].def))
                {
                    continue;
                }

                shield = owner.apparel.WornApparel[i];
                break;
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
        if (!IsShield(newApparel.def))
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
                if (!IsShield(owner.inventory.innerContainer[i].def))
                {
                    continue;
                }

                __instance.pawn.inventory.innerContainer.TryDrop(owner.inventory.innerContainer[i],
                    ThingPlaceMode.Direct, out _);
            }

            Apparel wornshield = null;

            for (var i = 0; i < owner.apparel.WornApparelCount; i++)
            {
                if (!IsShield(owner.apparel.WornApparel[i].def))
                {
                    continue;
                }

                wornshield = owner.apparel.WornApparel[i];
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
                if (!IsShield(owner.apparel.WornApparel[i].def))
                {
                    continue;
                }

                wornshield = owner.apparel.WornApparel[i];
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
        // do we have a shield equipped
        foreach (var apparel in pawn.apparel.WornApparel)
        {
            if (!IsShield(apparel.def))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    // check if a pawn has a shield equipped
    public static Apparel GetPawnShield(Pawn pawn)
    {
        // do we have a shield equipped
        foreach (var apparel in pawn.apparel.WornApparel)
        {
            if (!IsShield(apparel.def))
            {
                continue;
            }

            return apparel;
        }

        return null;
    }

    // check if a pawn has a shield in inventory
    public static bool PawnHasShieldInInventory(Pawn pawn)
    {
        foreach (var thing in pawn.inventory.innerContainer)
        {
            if (!IsShield(thing.def))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    // check if the pawn picked up a shield
    public static bool PawnPickedUpAShield(ThingWithComps newEquipment)
    {
        return IsShield(newEquipment.def);
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

        if (GetPawnShield(pawn)?.def.apparel.tags.Contains("Light_Shield") ?? false)
        {
            // if this is a light shield only allow light sidearms
            return pawn.equipment.Primary.def.weaponTags.Any(t => t == "LightShield_Sidearm");
        }

        // by default don't allow ranged weapons or weapons with Shield_NoSidearm or "LightShield_Sidearm without a light shield"
        return !pawn.equipment.Primary.def.IsRangedWeapon &&
               !pawn.equipment.Primary.def.weaponTags.Any(
                   t => t is "Shield_NoSidearm" or "LightShield_Sidearm");
    }

    public static bool IsShield(ThingDef thingDef)
    {
        var returnValue = thingDef.thingClass == typeof(ColorableShield);

        if (returnValue)
        {
            return true;
        }

        return thingDef.thingCategories != null &&
               thingDef.thingCategories.Any(thingCategoryDef => thingCategoryDef.defName == "Shield");
    }
}