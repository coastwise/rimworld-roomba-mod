using System;
using Verse;

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

		public override void Initialize (CompProperties props) {
			base.Initialize(props);
			Log.Message("CompRobotWork Initialized with WorkTypes:");
			foreach (WorkTypeDef workType in this.Props.validWorkTypes)
			{
				Log.Message(workType.defName);
			}
			Log.Message("end of work types for CompRobotWork");
		}

		public override void PostSpawnSetup () {
			Pawn pawn = (Pawn)this.parent;

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
	
	}
}
