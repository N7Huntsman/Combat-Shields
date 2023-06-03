using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace CombatShields;

internal class JobDriver_EquipShield : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
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