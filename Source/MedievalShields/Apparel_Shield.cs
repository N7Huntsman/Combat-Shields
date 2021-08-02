using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace CombatShields
{
    public class Apparel_Shield : Apparel
    {
        public Material shieldMat;
        public static readonly SoundDef SoundAbsorbDamage = SoundDef.Named("PersonalShieldAbsorbDamage");
        public static readonly SoundDef SoundBreak = SoundDef.Named("PersonalShieldBroken");
        public Vector3 impactAngleVect;

        public bool colorable;

        public bool ShouldDisplay
        {
            get
            {
                if (this.Wearer.Dead || this.Wearer.InBed() || this.Wearer.Downed || Wearer.IsPrisonerOfColony)
                    return false;
                if (this.Wearer.IsColonist && !this.Wearer.Drafted && !this.Wearer.InAggroMentalState)
                    return false;
                return true;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            // Color c = this.DrawColor;
            // Scribe_Values.Look<Color>(ref c, "color", new Color(0.8f, 0.8f, 0.8f), false);
        }
        public override void Tick()
        {
            base.Tick();
        }


        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            if (dinfo.Instigator != null)
            {
                SkillRecord melee = this.Wearer.skills.GetSkill(SkillDefOf.Melee);
                System.Random random = new System.Random();
                int chance = random.Next(0, 21);
                // this.AbsorbedDamage(dinfo);

                // full hit

                // partial hit

                // partial block

                // full block


                if (chance >= melee.levelInt)
                {
                    int i = (int)dinfo.Amount;
                    i = i / 4;

                    this.HitPoints -= i;
                }
                else
                {
                    int i = (int)dinfo.Amount;
                    i = i / 10;

                    this.HitPoints -= i;
                }
            }
            return false;
        }
        public void AbsorbedDamage(DamageInfo dinfo)
        {
            Apparel_Shield.SoundAbsorbDamage.PlayOneShotOnCamera(null);
            this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 loc = this.Wearer.TrueCenter() + this.impactAngleVect.RotatedBy(180f) * 0.5f;
            // this.HitPoints -= (int)dinfo.Amount;
            // MoteThrown.ThrowStatic(loc, ThingDefOf.Mote_ShotHit_Spark, 1f);
            // MoteThrown.SetVelocity(loc.magnitude, 1f);
        }
        public void Break()
        {
            Apparel_Shield.SoundBreak.PlayOneShotOnCamera(null);
            this.Destroy();
        }

        [DebuggerHidden]
        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            yield return new Apparel_Shield.Gizmo_ShieldStatus
            {
                shield = this
            };
            yield break;
        }
        [StaticConstructorOnStartup]
        internal class Gizmo_ShieldStatus : Gizmo
        {
            public Apparel_Shield shield;
            private static readonly Texture2D FullTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));
            private static readonly Texture2D EmptyTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
            public override float GetWidth(float maxWidth)
            {

                return 140f;
            }

            public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
            {
                Rect rect = new Rect(topLeft.x, topLeft.y, this.GetWidth(140f), 75f);
                Widgets.DrawWindowBackground(rect);

                Rect rect2 = GenUI.ContractedBy(rect, 6f);
                Rect rect3 = rect2;

                rect3.height = rect.height / 2f;

                Text.Font = GameFont.Tiny;
                Widgets.Label(rect3, this.shield.LabelCap);

                Rect rect4 = rect2;

                rect4.yMin = rect.y + rect.height / 2f;
                float num = this.shield.MaxHitPoints / this.shield.HitPoints;

                Widgets.FillableBar(rect4, num, Apparel_Shield.Gizmo_ShieldStatus.FullTex, Apparel_Shield.Gizmo_ShieldStatus.EmptyTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;

                Widgets.Label(rect4, (this.shield.HitPoints).ToString() + " / " + (this.shield.MaxHitPoints).ToString());
                Text.Anchor = TextAnchor.UpperLeft;

                return new GizmoResult(0);
            }
        }

        public override void DrawWornExtras()
        {
            // base.DrawWornExtras();

            if (this.ShouldDisplay)
            {
                // base.DrawWornExtras();

                float num = 0f;
                Vector3 vector = this.Wearer.Drawer.DrawPos;
                // vector.y = Altitudes.AltitudeFor(AltitudeLayer.Pawn);
                Vector3 s = new Vector3(1f, 1f, 1f);
                if (this.Wearer.Rotation == Rot4.North)
                {
                    vector.y = Altitudes.AltitudeFor(AltitudeLayer.Item);
                    vector.x -= 0.2f;
                    vector.z -= 0.2f;
                }
                else
                {
                    if (this.Wearer.Rotation == Rot4.South)
                    {
                        vector.y = Altitudes.AltitudeFor(AltitudeLayer.Pawn);
                        vector.y += 0.033f;
                        vector.x += 0.2f;
                        vector.z -= 0.2f;
                    }
                    else
                    {
                        if (this.Wearer.Rotation == Rot4.East)
                        {
                            vector.y = Altitudes.AltitudeFor(AltitudeLayer.Pawn);
                            vector.z = 0.2f;
                            num = 90f;
                        }
                        else
                        {
                            if (this.Wearer.Rotation == Rot4.West)
                            {
                                vector.y = Altitudes.AltitudeFor(AltitudeLayer.MoteOverhead);
                                vector.z -= 0.2f;
                                num = 270f;
                            }
                        }
                    }
                }

                // colorable = def.colorable;
                // if (shield as Shield.ColorableShield != null) // make sure it actually IS a MyCoolNewGun, HAS the code, etc:
                //{
                // use the thingdef:
                // var def = shield.def as Shield.ColorableShield;
                // colorable = def.colorable;

                // or access directly:
                // var myBool = ((myNamespace.MyCustomDef_ThingDef)weapon.def).myNewBool;
                //     this will throw an exception if somehow the weapon isn't a MyCoolNewGun!

                // use the defs, etc
                // this works in beta 0.18.1722
                // }

                if (shieldMat == null)
                {
                    shieldMat = MaterialPool.MatFrom(def.graphicData.texPath);
                }

                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(vector, Quaternion.AngleAxis(num, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, shieldMat, 0);
            }
        }

    }

    public class ColorableShield : Apparel
    {
        public Material shieldMat;
        public static readonly SoundDef SoundAbsorbDamage = SoundDef.Named("PersonalShieldAbsorbDamage");
        public static readonly SoundDef SoundBreak = SoundDef.Named("PersonalShieldBroken");
        public Vector3 impactAngleVect;
        public List<string> stuff = new List<string>();
        public bool colorable;

        public bool ShouldDisplay
        {
            get
            {
                if (this.Wearer.Dead || this.Wearer.InBed() || this.Wearer.Downed || Wearer.IsPrisonerOfColony)
                    return false;
                if (this.Wearer.IsColonist && !this.Wearer.Drafted && !this.Wearer.InAggroMentalState)
                    return false;
                return true;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();

            // Color c = this.DrawColor;
            // Scribe_Values.Look<Color>(ref c, "color", new Color(0.8f, 0.8f, 0.8f), false);
            // ValidWeapon();
        }

        public override void Tick()
        {
            base.Tick();
        }

        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            if (dinfo.Instigator != null)
            {
                SkillRecord melee = this.Wearer.skills.GetSkill(SkillDefOf.Melee);
                System.Random random = new System.Random();
                int chance = random.Next(0, 21);
                // this.AbsorbedDamage(dinfo);

                // full hit

                // partial hit

                // partial block

                // full block


                if (chance >= melee.levelInt)
                {
                    int i = (int)dinfo.Amount;
                    i = i / 4;

                    this.HitPoints -= i;
                }
                else
                {
                    int i = (int)dinfo.Amount;
                    i = i / 10;

                    this.HitPoints -= i;
                }
            }
            return false;
        }

        public void AbsorbedDamage(DamageInfo dinfo)
        {
            Apparel_Shield.SoundAbsorbDamage.PlayOneShotOnCamera(null);
            this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 loc = this.Wearer.TrueCenter() + this.impactAngleVect.RotatedBy(180f) * 0.5f;
            // this.HitPoints -= (int)dinfo.Amount;
            // MoteThrown.ThrowStatic(loc, ThingDefOf.Mote_ShotHit_Spark, 1f);
            // MoteThrown.SetVelocity(loc.magnitude, 1f);
        }
        public void Break()
        {
            Apparel_Shield.SoundBreak.PlayOneShotOnCamera(null);
            this.Destroy();
        }


        [DebuggerHidden]
        public override IEnumerable<Gizmo> GetWornGizmos()
        {

            yield return new ColorableShield.Gizmo_ShieldStatus
            {
                shield = this
            };
            yield break;

        }
        [StaticConstructorOnStartup]
        internal class Gizmo_ShieldStatus : Gizmo
        {
            public ColorableShield shield;
            private static readonly Texture2D FullTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));
            private static readonly Texture2D EmptyTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
            public override float GetWidth(float maxWidth)
            {

                return 140f;
                // throw new NotImplementedException();
            }

            public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
            {
                Rect rect = new Rect(topLeft.x, topLeft.y, this.GetWidth(140f), 75f);
                Widgets.DrawWindowBackground(rect);
                Rect rect2 = GenUI.ContractedBy(rect, 6f);
                Rect rect3 = rect2;
                rect3.height = rect.height / 2f;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rect3, this.shield.LabelCap);
                Rect rect4 = rect2;
                rect4.yMin = rect.y + rect.height / 2f;
                float num = this.shield.MaxHitPoints / this.shield.HitPoints;
                Widgets.FillableBar(rect4, num, ColorableShield.Gizmo_ShieldStatus.FullTex, ColorableShield.Gizmo_ShieldStatus.EmptyTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect4, (this.shield.HitPoints).ToString("F0") + " / " + (this.shield.MaxHitPoints).ToString("F0"));
                Text.Anchor = TextAnchor.UpperLeft;
                return new GizmoResult(0);
            }
        }

        public override void DrawWornExtras()
        {
            // base.DrawWornExtras();

            if (this.ShouldDisplay)
            {
                // base.DrawWornExtras();

                float num = 0f;
                Vector3 vector = this.Wearer.Drawer.DrawPos;
                // vector.y = Altitudes.AltitudeFor(AltitudeLayer.Pawn);
                Vector3 s = new Vector3(1f, 1f, 1f);
                if (this.Wearer.Rotation == Rot4.North)
                {
                    vector.y = Altitudes.AltitudeFor(AltitudeLayer.Item);
                    vector.x -= 0.2f;
                    vector.z -= 0.2f;
                }
                else
                {
                    if (this.Wearer.Rotation == Rot4.South)
                    {
                        vector.y = Altitudes.AltitudeFor(AltitudeLayer.Pawn);
                        vector.y += 0.033f;
                        vector.x += 0.2f;
                        vector.z -= 0.2f;
                    }
                    else
                    {
                        if (this.Wearer.Rotation == Rot4.East)
                        {
                            vector.y = Altitudes.AltitudeFor(AltitudeLayer.Pawn);
                            vector.z = 0.2f;
                            num = 90f;
                        }
                        else
                        {
                            if (this.Wearer.Rotation == Rot4.West)
                            {
                                vector.y = Altitudes.AltitudeFor(AltitudeLayer.MoteOverhead);
                                vector.z -= 0.2f;
                                num = 270f;
                            }
                        }
                    }
                }


                if (shieldMat == null)
                {
                    shieldMat = MaterialPool.MatFrom(def.graphicData.texPath);
                }

                shieldMat.color = Stuff.stuffProps.color;
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(vector, Quaternion.AngleAxis(num, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, shieldMat, 0);
            }
        }
    }
}