using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace RoombaCode {
	public class CompRobotWork : ThingComp
	{
		public CompProperties_RobotWork Props
		{
			get
			{
				return (CompProperties_RobotWork)this.props;
			}
		}

		List<WorkGiver> workGiversByPriority = new List<WorkGiver>();

		public override void Initialize (CompProperties props) {
			base.Initialize(props);
			Log.Message("CompRobotWork Initialized with WorkTypes:");
			foreach (WorkTypeDef workType in this.Props.validWorkTypes)
			{
				Log.Message(workType.defName);

				// TODO: does *not* do priorities yet
				foreach (WorkGiverDef giverDef in workType.workGiversByPriority)
				{
					Log.Message(giverDef.defName);
					workGiversByPriority.Add(giverDef.Worker);
				}

			}
			Log.Message("end of work types for CompRobotWork");
		}

		public override void PostSpawnSetup () {
			Pawn pawn = (Pawn)this.parent;

			if (pawn.Faction != Faction.OfPlayer)
			{
				Log.Warning("pawn is of a different faction than the player! wont get cleaning jobs!");
				Log.Message("forcing pawn faction to " + Faction.OfPlayer.def.defName);
				pawn.SetFactionDirect(Faction.OfPlayer);
			}



			if (pawn.story == null || pawn.workSettings == null)
			{
				Log.Warning("CompRobotWork running on a pawn without a story or workSettings... this is probably fine, old code path");
				return;
			}

			TraitDef traitDef = Robot_TraitDefOf.Vacuum;
			int degree = 0; // not a spectrum
			bool forced = true;
			Trait forcedTrait = new Trait(traitDef, degree, forced);

			pawn.story.traits.GainTrait(forcedTrait);

			if (pawn.workSettings == null)
			{
				Log.Error("Needs Pawn_WorkSettings to function! Requires Humanlike intelligence as of A16");
				return;
			}

			pawn.workSettings.DisableAll();
			int priority = RimWorld.Pawn_WorkSettings.DefaultPriority;

			foreach (WorkTypeDef workType in this.Props.validWorkTypes)
			{
				pawn.workSettings.SetPriority(workType, priority);
			}

			Log.Message("Done!");

		}
	
		public List<WorkGiver> GetWorkGiversInOrder()
		{
			return workGiversByPriority;
		}

	}
}
