using RimWorld;
using Shield.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Shield
{
    [StaticConstructorOnStartup]
    class SilverFishModule: IExposable
    {
        public static String noShield = "NOSHIELD";

        public Pawn Owner;


        public SilverFishModule() : this(null, false) { }

        public SilverFishModule(Pawn owner) : this(owner, false) { }

        public SilverFishModule(Pawn owner, bool fillExisting)
        {

        }

        public void ExposeData()
        {


            // Scribe_Collections.Look<string>(ref weapons, "weapons", LookMode.Value);
            // Scribe_Values.Look<string>(ref primary, "primary", NoWeaponString, true);
        }
    }
}
