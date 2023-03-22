using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace CombatShields
{
    internal class JobDriver_EquipShield : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            /*yield return Toils_General.Do(delegate
            {
                
            });*/
            yield return ShieldToInventory(pawn);
        }

        public static Toil ShieldToInventory(Pawn pawn)
        {
            var toil = new Toil();
            // do we have a shield equipped
            foreach (var a in pawn.apparel.WornApparel)
            {
                foreach (var tgd in a.def.thingCategories)
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