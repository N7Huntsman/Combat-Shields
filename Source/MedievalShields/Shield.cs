using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;
using RimWorld.BaseGen;
using Harmony;

namespace CombatShields
{
    public class CombatShields
    {

        Toil shieldSwap = new Toil
        {
            initAction = () =>
            {
                Pawn actor = this.pawn;
                Thing thing = actor.CurJob.GetTarget(TargetIndex.A).Thing;
                Toils_Haul.ErrorCheckForCarry(actor, thing);

                int num = Mathf.Min(thing.stackCount, MassUtility.CountToPickUpUntilOverEncumbered(actor, thing));
                if (num <= 0)
                {
                    Job haul = HaulAIUtility.HaulToStorageJob(actor, thing);

                    if (haul?.TryMakePreToilReservations(actor) ?? false)
                    {
                        actor.jobs.jobQueue.EnqueueFirst(haul, new JobTag?(JobTag.Misc));
                    }
                    actor.jobs.curDriver.JumpToToil(wait);
                }
                else
                {
                    actor.inventory.GetDirectlyHeldThings().TryAdd(thing.SplitOff(num), true);
                }
            }
        };    
    }
}
