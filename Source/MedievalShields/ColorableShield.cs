using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using UnityEngine;
using Verse;
using Random = System.Random;

namespace CombatShields;

public class ColorableShield : Apparel_Shield
{
    public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
    {
        if (dinfo.Instigator == null)
        {
            return false;
        }

        var melee = Wearer.skills.GetSkill(SkillDefOf.Melee);
        var random = new Random();
        var chance = random.Next(0, 21);

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
        base.DrawWornExtras();

        if (!ShouldDisplay)
        {
            return;
        }

        var num = 0f;
        var vector = Wearer.Drawer.DrawPos;
        vector.y = AltitudeLayer.Pawn.AltitudeFor();
        var s = new Vector3(1f, 1f, 1f);
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
            num = 90f;
        }
        else if (Wearer.Rotation == Rot4.West)
        {
            vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            vector.z -= 0.2f;
            num = 270f;
        }

        if (shieldMat == null)
        {
            shieldMat = MaterialPool.MatFrom(def.graphicData.texPath);
        }

        shieldMat.color = Stuff.stuffProps.color;
        var matrix = default(Matrix4x4);
        matrix.SetTRS(vector, Quaternion.AngleAxis(num, Vector3.up), s);
        Graphics.DrawMesh(MeshPool.plane10, matrix, shieldMat, 0);
    }
}