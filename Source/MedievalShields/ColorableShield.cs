using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;

namespace CombatShields;

public class ColorableShield : Apparel_Shield
{
    private Graphic wornGraphic;

    public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
    {
        if (dinfo.Instigator == null || dinfo.Def == DamageDefOf.Extinguish || dinfo.Def == DamageDefOf.Smoke ||
            dinfo.Def == DamageDefOf.ToxGas)
        {
            return false;
        }

        var melee = Wearer.skills.GetSkill(SkillDefOf.Melee);
        var chance = Rand.Range(0, 21);

        if (chance >= melee.levelInt)
        {
            var i = (int)dinfo.Amount;
            i /= 4;

            HitPoints -= i;
        }
        else
        {
            var i = (int)dinfo.Amount;
            i /= 10;

            HitPoints -= i;
        }

        return false;
    }


    [DebuggerHidden]
    public override IEnumerable<Gizmo> GetWornGizmos()
    {
        yield return new Gizmo_ShieldStatus
        {
            shield = this
        };
    }

    public override void DrawWornExtras()
    {
        if (!ShouldDisplay)
        {
            return;
        }

        var vector = Wearer.Drawer.DrawPos;
        vector.y = AltitudeLayer.Pawn.AltitudeFor();
        var rotation = Rot4.North;
        if (Wearer.Rotation == Rot4.North)
        {
            vector.y = AltitudeLayer.Item.AltitudeFor();
            vector.x -= 0.2f;
            vector.z -= 0.2f;
        }
        else if (Wearer.Rotation == Rot4.South)
        {
            vector.y += 0.033f;
            vector.x += 0.2f;
            vector.z -= 0.2f;
        }
        else if (Wearer.Rotation == Rot4.East)
        {
            vector.z -= 0.2f;
            rotation = Rot4.East;
        }
        else if (Wearer.Rotation == Rot4.West)
        {
            vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            vector.z -= 0.2f;
            rotation = Rot4.West;
        }

        if (wornGraphic == null)
        {
            var wornGraphicData = new GraphicData();
            wornGraphicData.CopyFrom(def.graphicData);
            wornGraphicData.onGroundRandomRotateAngle = 0f;
            wornGraphicData.drawRotated = true;
            wornGraphic = wornGraphicData.GraphicColoredFor(this);
        }

        wornGraphic.Draw(vector, rotation, this);
    }
}