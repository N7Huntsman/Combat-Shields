using System;
using CombatShields;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace CombatShields
{
    class JobDriver_EquipShield: JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            /*yield return Toils_General.Do(delegate
            {
                
            });*/
            yield return ShieldToInventory(this.pawn);

        }
        
        public static Toil ShieldToInventory(Pawn pawn)
        {
            Toil toil = new Toil();
            // do we have a shield equipped
            foreach (Apparel a in pawn.apparel.WornApparel)
            {
                foreach (ThingCategoryDef tgd in a.def.thingCategories)
                {
                    if (tgd.defName == "Shield")
                    {
                        pawn.inventory.innerContainer.TryAddOrTransfer(a, false);
                    }
                }
            }
            return toil;
        }
    }
}

/*
 *                    ThingWithComps thingWithComps = (ThingWithComps)this.job.targetA.Thing;
                    ThingWithComps thingWithComps2;
                    if (thingWithComps.def.stackLimit > 1 && thingWithComps.stackCount > 1)
                    {
                        thingWithComps2 = (ThingWithComps)thingWithComps.SplitOff(1);
                    }
                    else
                    {
                        thingWithComps2 = thingWithComps;
                        thingWithComps2.DeSpawn();
                    }
                    //this.pawn.equipment.MakeRoomFor(thingWithComps2);
                    //this.pawn.equipment.AddEquipment(thingWithComps2); 
*/
