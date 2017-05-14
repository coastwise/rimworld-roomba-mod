using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

/*
 * Pretty much the same as JobGiver_Work, except for removed deps
 * on mindState and workSettings (and story)
 */

namespace RoombaCode {
	public class JobGiver_RobotWork : JobGiver_Work {

		protected override Job TryGiveJob (Verse.Pawn pawn) {

			if (this.emergency && pawn.mindState != null && pawn.mindState.priorityWork.IsPrioritized)
			{
				Log.Warning("JobGiver_RobotWork doesn't support emergency work atm");
			}

			TargetInfo targetInfo = TargetInfo.Invalid;
			WorkGiver_Scanner workGiver_Scanner = null;
			int num = -999;

			CompRobotWork robotWorkSettings = pawn.TryGetComp<CompRobotWork>();
			if (robotWorkSettings == null)
			{
				Log.Error("JobGiver_RobotWork needs a CompRobotWork on the pawn to function");
				return null;
			}

			List<WorkGiver> list = robotWorkSettings.GetWorkGiversInOrder();

			foreach (WorkGiver workGiver in list)
			{
				if (workGiver.def.priorityInType != num && targetInfo.IsValid)
				{
					break;
				}

				try {
					Job job = workGiver.NonScanJob(pawn);
					if (job != null)
					{
						return job;
					}

					WorkGiver_Scanner scanner = workGiver as WorkGiver_Scanner;
					if (scanner != null)
					{
						if (workGiver.def.scanThings)
						{
							Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t);
							IEnumerable<Thing> enumerable = scanner.PotentialWorkThingsGlobal(pawn);
							Thing thing;
							if (scanner.Prioritized)
							{
								IEnumerable<Thing> enumerable2 = enumerable;
								if (enumerable2 == null)
								{
									enumerable2 = pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
								}
								Predicate<Thing> validator = predicate;
								thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, enumerable2, scanner.PathEndMode, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, (Thing x) => scanner.GetPriority(pawn, x));
							}
							else
							{
								Predicate<Thing> validator = predicate;
								thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, scanner.PotentialWorkThingRequest, scanner.PathEndMode, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, enumerable, scanner.LocalRegionsToScanFirst, enumerable != null);
							}
							if (thing != null)
							{
								targetInfo = thing;
								workGiver_Scanner = scanner;
							}
						}
						if (workGiver.def.scanCells)
						{
							IntVec3 position = pawn.Position;
							float num2 = 99999f;
							float num3 = -3.40282347E+38f;
							bool prioritized = scanner.Prioritized;
							foreach (IntVec3 current in scanner.PotentialWorkCellsGlobal(pawn))
							{
								bool flag = false;
								float lengthHorizontalSquared = (current - position).LengthHorizontalSquared;
								if (prioritized)
								{
									if (!current.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, current))
									{
										float priority = scanner.GetPriority(pawn, current);
										if (priority > num3 || (priority == num3 && lengthHorizontalSquared < num2))
										{
											flag = true;
											num3 = priority;
										}
									}
								}
								else if (lengthHorizontalSquared < num2 && !current.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, current))
								{
									flag = true;
								}
								if (flag)
								{
									targetInfo = new TargetInfo(current, pawn.Map, false);
									workGiver_Scanner = scanner;
									num2 = lengthHorizontalSquared;
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					Log.Error(string.Concat(new object[]
						{
							pawn,
							" threw exception in RobotWorkGiver ",
							workGiver.def.defName,
							": ",
							ex.ToString()
						}));
				}

				if (targetInfo.IsValid)
				{
					pawn.mindState.lastGivenWorkType = workGiver.def.workType;
					Job job3;
					if (targetInfo.HasThing)
					{
						job3 = workGiver_Scanner.JobOnThing(pawn, targetInfo.Thing);
					}
					else
					{
						job3 = workGiver_Scanner.JobOnCell(pawn, targetInfo.Cell);
					}
					if (job3 != null)
					{
						return job3;
					}
					Log.ErrorOnce(string.Concat(new object[]
						{
							workGiver_Scanner,
							" provided target ",
							targetInfo,
							" but yielded no actual job for pawn ",
							pawn,
							". The CanGiveJob and JobOnX methods may not be synchronized."
						}), 6112651);
				}

				num = workGiver.def.priorityInType;

			} // foreach workgiver

			return null;

		} // TryGiveJob

	}
}

