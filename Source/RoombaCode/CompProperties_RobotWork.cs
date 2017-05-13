using System;
using System.Collections.Generic;
using Verse;

namespace RoombaCode {
	public class CompProperties_RobotWork : CompProperties
	{
		public List<WorkTypeDef> validWorkTypes;

		public CompProperties_RobotWork()
		{
			this.compClass = typeof(CompRobotWork);
		}
	}
}
