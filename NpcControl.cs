using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony12;
using UnityEngine;
using UnityModManagerNet;

namespace ControlNpc
{
	// Token: 0x02000003 RID: 3
	public static class Main
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002068 File Offset: 0x00000268
		public static bool make_new_event_key_list()
		{
			string text = "";
			Main.new_event_key_list = new List<int>();
			int num = DateFile.instance.eventDate.Keys.Max();
			num++;
			text = text + num.ToString() + "|";
			DateFile.instance.eventDate.Add(num, new Dictionary<int, string>(DateFile.instance.eventDate[900500001]));
			DateFile.instance.eventDate[num][3] = "控制该角色";
			DateFile.instance.eventDate[num][6] = "";
			DateFile.instance.eventDate[num][8] = "END&90051&893";
			Main.new_event_key_list.Add(num);
			num++;
			text = text + num.ToString() + "|";
			DateFile.instance.eventDate.Add(num, new Dictionary<int, string>(DateFile.instance.eventDate[900500001]));
			DateFile.instance.eventDate[num][3] = "传剑给该角色";
			DateFile.instance.eventDate[num][6] = "";
			DateFile.instance.eventDate[num][8] = "END&90051&1893";
			Main.new_event_key_list.Add(num);
			num++;
			text = text + num.ToString() + "|";
			DateFile.instance.eventDate.Add(num, new Dictionary<int, string>(DateFile.instance.eventDate[900500001]));
			DateFile.instance.eventDate[num][3] = "转化为太吾村民";
			DateFile.instance.eventDate[num][6] = "";
			DateFile.instance.eventDate[num][8] = "END&90051&2893";
			Main.new_event_key_list.Add(num);
			num++;
			text = text + num.ToString() + "|";
			DateFile.instance.eventDate.Add(num, new Dictionary<int, string>(DateFile.instance.eventDate[900500001]));
			DateFile.instance.eventDate[num][3] = "以原有身份加入/移出同道";
			DateFile.instance.eventDate[num][6] = "";
			DateFile.instance.eventDate[num][8] = "END&90051&3893";
			Main.new_event_key_list.Add(num);
			num++;
			text += num.ToString();
			DateFile.instance.eventDate.Add(num, new Dictionary<int, string>(DateFile.instance.eventDate[900500001]));
			DateFile.instance.eventDate[num][3] = "转化为前世";
			DateFile.instance.eventDate[num][6] = "";
			DateFile.instance.eventDate[num][8] = "END&90051&4893";
			Main.new_event_key_list.Add(num);
			num++;
			DateFile.instance.eventDate.Add(num, new Dictionary<int, string>(DateFile.instance.eventDate[9001]));
			DateFile.instance.eventDate[num][0] = "操纵类互动";
			DateFile.instance.eventDate[num][5] = text;
			Main.new_event_key_list.Add(num);
			num++;
			DateFile.instance.eventDate.Add(num, new Dictionary<int, string>(DateFile.instance.eventDate[1000000001]));
			DateFile.instance.eventDate[num][3] = "(操纵类选项...)";
			DateFile.instance.eventDate[num][7] = (1 - num).ToString();
			Main.new_event_key_list.Add(num);
			foreach (int key in DateFile.instance.eventDate.Keys)
			{
				if (DateFile.instance.eventDate[key][5].Split(new char[]
				{
					'|'
				}).Contains("1000000006"))
				{
					DateFile.instance.eventDate[key][5] = DateFile.instance.eventDate[key][5].Replace("1000000006", num.ToString() + "|1000000006");
				}
			}
			return false;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000254C File Offset: 0x0000074C
		public static void get_gangValueId(int actorId)
		{
			int id = int.Parse(DateFile.instance.GetActorDate(actorId, 19, false));
			int num = int.Parse(DateFile.instance.GetActorDate(actorId, 20, false));
			Main.gangValueId = DateFile.instance.GetGangValueId(id, num);
			Main.need_threat = ((int.Parse(DateFile.instance.presetGangGroupDateValue[Main.gangValueId][201]) == 0) ? 1000 : 100);
			int num2 = int.Parse(DateFile.instance.GetGangDate(id, 2));
			Main.isable = (num2 != 1 && !DateFile.instance.GetActorSocial(actorId, 305, false, false).Contains(DateFile.instance.mianActorId));
			Main.isgang = (num2 != 1);
			Main.threat = Main.GetActorSocial(actorId, 401, false).Count * 5 + Main.GetActorSocial(actorId, 402, false).Count * 10 + int.Parse(DateFile.instance.presetGangGroupDateValue[Main.gangValueId][811]);
			int num3 = 0;
			int num4 = int.Parse(DateFile.instance.GetActorDate(actorId, 9, false));
			Main.togangid = num4;
			Main.togangname = DateFile.instance.presetGangDate[num4][0];
			Main.isgang = (num4 > 0);
			Main.spaceused = DateFile.instance.GetGangActor(num4, 9, true).Count;
			Main.spacemax = int.Parse(DateFile.instance.presetGangGroupDateValue[DateFile.instance.GetGangValueId(num4, 9)][1]);
			Main.enoughspace = (DateFile.instance.GetGangActor(num4, 9, true).Count < int.Parse(DateFile.instance.presetGangGroupDateValue[DateFile.instance.GetGangValueId(num4, 9)][1]));
			if (num4 > 0 && DateFile.instance.GetGangActor(num4, 9, true).Count < int.Parse(DateFile.instance.presetGangGroupDateValue[DateFile.instance.GetGangValueId(num4, 9)][1]))
			{
				List<int> list = new List<int>(DateFile.instance.GetGangActor(num4, 1, true));
				for (int i = 0; i < list.Count; i++)
				{
					int num5 = list[i];
					if (int.Parse(DateFile.instance.GetActorDate(num5, 20, false)) >= 0)
					{
						num3 = num5;
						break;
					}
				}
			}
			Main.toactorId = num3;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000027C4 File Offset: 0x000009C4
		public static List<int> GetActorSocial(int actorId, int socialTyp, bool getDieActor)
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>(DateFile.instance.GetLifeDateList(actorId, socialTyp, false));
			for (int i = 0; i < list2.Count; i++)
			{
				int key = list2[i];
				if (DateFile.instance.actorSocialDate.ContainsKey(key))
				{
					for (int j = 0; j < DateFile.instance.actorSocialDate[key].Count; j++)
					{
						int num = DateFile.instance.actorSocialDate[key][j];
						if (num != actorId && !list.Contains(num) && (getDieActor || int.Parse(DateFile.instance.GetActorDate(num, 26, false)) == 0))
						{
							list.Add(num);
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000288C File Offset: 0x00000A8C
		public static bool Load(UnityModManager.ModEntry modEntry)
		{
			HarmonyInstance harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);
			Main.settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
			harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
			Main.logger = modEntry.Logger;
			modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
			modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);
			modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(Main.OnSaveGUI);
			return true;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002900 File Offset: 0x00000B00
		public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
		{
			if (!value)
			{
				return false;
			}
			Main.enabled = value;
			return true;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002910 File Offset: 0x00000B10
		public static void OnGUI(UnityModManager.ModEntry modEntry)
		{
			GUILayout.Label("控制和你对话的NPC", new GUILayoutOption[0]);
			if (Main.settings.family)
			{
				if (GUILayout.Button("当前模式：原太吾留在同道中", Array.Empty<GUILayoutOption>()))
				{
					Main.settings.family = false;
					return;
				}
			}
			else if (GUILayout.Button("当前模式：原太吾出现在当前地块上", Array.Empty<GUILayoutOption>()))
			{
				Main.settings.family = true;
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002972 File Offset: 0x00000B72
		private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
		{
			Main.settings.Save(modEntry);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002980 File Offset: 0x00000B80
		public static void SetNewMianActor(int chooseNewActor)
		{
			bool flag = false;
			int num = DateFile.instance.MianActorID();
			int num2 = (!flag) ? chooseNewActor : DateFile.instance.MakeNewActor(int.Parse(DateFile.instance.placeWorldDate[DateFile.instance.mianWorldId + 1][87]) + UnityEngine.Random.Range(0, 2), true, 0, UnityEngine.Random.Range(12, 15), -1, null, null, null, null, 20);
			for (int i = 0; i < 5; i++)
			{
				if (DateFile.instance.acotrTeamDate[i] == num2)
				{
					DateFile.instance.acotrTeamDate[i] = -1;
				}
			}
			DateFile.instance.buyStudyValue = 0;
			DateFile.instance.AddActorScore(0, 100);
			if (flag)
			{
				DateFile.instance.actorsDate[num2][4] = "50";
				DateFile.instance.actorsDate[num2].Remove(18);
				List<int> list = new List<int>(DateFile.instance.actorsDate.Keys);
				for (int j = 0; j < list.Count; j++)
				{
					int num3 = list[j];
					if (int.Parse(DateFile.instance.GetActorDate(num3, 8, false)) == 1)
					{
						DateFile.instance.actorsDate[num3][3] = "-1";
						DateFile.instance.actorLife[num3].Remove(1001);
					}
				}
			}
			else
			{
				int actorFame = DateFile.instance.GetActorFame(num);
				if (actorFame >= 50)
				{
					DateFile.instance.SetActorFameList(num2, 601, 1, 0);
				}
				else if (actorFame <= -50)
				{
					DateFile.instance.SetActorFameList(num2, 602, 1, 0);
				}
				List<int> list2 = new List<int>(DateFile.instance.actorsDate.Keys);
				for (int k = 0; k < list2.Count; k++)
				{
					int num4 = list2[k];
					if (int.Parse(DateFile.instance.GetActorDate(num4, 8, false)) == 1)
					{
						int num5 = int.Parse(DateFile.instance.GetActorDate(num4, 3, false));
						DateFile.instance.actorsDate[num4][3] = "-1";
						DateFile.instance.actorLife[num4].Remove(1001);
						if (num5 > 0)
						{
							DateFile.instance.ChangeFavor(num4, num5 * 50 / 100, false, false);
						}
					}
				}
			}
			for (int l = 0; l < 4; l++)
			{
				DateFile.instance.actorsDate[num2].Remove(902 + l);
			}
			DateFile.instance.actorsDate[num2][207] = "1";
			DateFile.instance.actorsDate[num2][208] = "1";
			DateFile.instance.actorsDate[num2].Remove(3);
			DateFile.instance.actorsDate[num2].Remove(27);
			DateFile.instance.actorsDate[num2][706] = DateFile.instance.GetActorDate(num, 706, false);
			DateFile.instance.actorsDate[num2][901] = DateFile.instance.GetActorDate(num, 901, false);
			DateFile.instance.AddActorToFamily(num2, true);
			DateFile.instance.ChangeGangDage(16, 9, 1, num2);
			for (int m = 0; m < 15; m++)
			{
				DateFile.instance.gangDate[m + 1].Remove(10);
			}
			List<int> list3 = new List<int>(DateFile.instance.actorItemsDate[num].Keys);
			for (int n = 0; n < list3.Count; n++)
			{
				int itemId = list3[n];
				DateFile.instance.ChangeTwoActorItem(num, num2, itemId, DateFile.instance.GetItemNumber(num, itemId), -1, 0, 0);
			}
			DateFile.instance.actorEquipGongFas.Remove(num2);
			Dictionary<int, int[]> dictionary = new Dictionary<int, int[]>
			{
				{
					0,
					new int[4]
				}
			};
			for (int num6 = 1; num6 < 5; num6++)
			{
				dictionary.Add(num6, new int[9]);
			}
			DateFile.instance.actorEquipGongFas.Add(num2, dictionary);
			DateFile.instance.actorGongFas.Remove(num2);
			DateFile.instance.actorGongFas.Add(num2, new SortedDictionary<int, int[]>());
			List<int> list4 = new List<int>(DateFile.instance.actorGongFas[num].Keys);
			for (int num7 = 0; num7 < list4.Count; num7++)
			{
				int key = list4[num7];
				int[] array = DateFile.instance.actorGongFas[num][key];
				DateFile.instance.actorGongFas[num2].Add(key, (int[])DateFile.instance.actorGongFas[num][key].Clone());
			}
			if (int.Parse(DateFile.instance.GetActorDate(num, 26, false)) > 0)
			{
				DateFile.instance.actorGongFas.Remove(num);
			}
			DateFile.instance.acotrTeamDate[0] = num2;
			DateFile.instance.samsara++;
			DateFile.instance.oldMianActorId = num;
			DateFile.instance.mianActorId = num2;
			UIDate.instance.ChangeResource(num2, 0, int.Parse(DateFile.instance.GetActorDate(num, 401, false)), false);
			UIDate.instance.ChangeResource(num2, 1, int.Parse(DateFile.instance.GetActorDate(num, 402, false)), false);
			UIDate.instance.ChangeResource(num2, 2, int.Parse(DateFile.instance.GetActorDate(num, 403, false)), false);
			UIDate.instance.ChangeResource(num2, 3, int.Parse(DateFile.instance.GetActorDate(num, 404, false)), false);
			UIDate.instance.ChangeResource(num2, 4, int.Parse(DateFile.instance.GetActorDate(num, 405, false)), false);
			UIDate.instance.ChangeResource(num2, 5, int.Parse(DateFile.instance.GetActorDate(num, 406, false)), false);
			UIDate.instance.ChangeResource(num2, 6, int.Parse(DateFile.instance.GetActorDate(num, 407, false)), false);
			for (int num8 = 0; num8 < 7; num8++)
			{
				DateFile.instance.actorsDate[num].Remove(401 + num8);
			}
			int maxDayTime = DateFile.instance.GetMaxDayTime();
			if (DateFile.instance.dayTime > maxDayTime)
			{
				DateFile.instance.dayTime = maxDayTime;
				UIDate.instance.UpdateMaxDayTime();
			}
			MissionSystem.instance.NewActorRemoveMission();
			WorldMapSystem.instance.UpdatePlaceActor(WorldMapSystem.instance.choosePartId, WorldMapSystem.instance.choosePlaceId, false);
			DateFile.instance.needUpdateFace = true;
			DateFile.instance.setNewMianActor = false;
		}

		// Token: 0x04000002 RID: 2
		public static bool enabled;

		// Token: 0x04000003 RID: 3
		public static UnityModManager.ModEntry.ModLogger logger;

		// Token: 0x04000004 RID: 4
		public static Settings settings;

		// Token: 0x04000005 RID: 5
		public static Dictionary<int, List<int>> mianActorItem;

		// Token: 0x04000006 RID: 6
		public static bool notfinish;

		// Token: 0x04000007 RID: 7
		public static int true_mainactor;

		// Token: 0x04000008 RID: 8
		public static int gangValueId;

		// Token: 0x04000009 RID: 9
		public static int threat;

		// Token: 0x0400000A RID: 10
		public static int need_threat;

		// Token: 0x0400000B RID: 11
		public static int toactorId;

		// Token: 0x0400000C RID: 12
		public static string toactorname;

		// Token: 0x0400000D RID: 13
		public static string output;

		// Token: 0x0400000E RID: 14
		public static bool isable;

		// Token: 0x0400000F RID: 15
		public static int togangid;

		// Token: 0x04000010 RID: 16
		public static string togangname;

		// Token: 0x04000011 RID: 17
		public static bool isgang;

		// Token: 0x04000012 RID: 18
		public static bool enoughspace;

		// Token: 0x04000013 RID: 19
		public static int spaceused;

		// Token: 0x04000014 RID: 20
		public static int spacemax;

		// Token: 0x04000015 RID: 21
		public static int tempid = int.MaxValue;

		// Token: 0x04000016 RID: 22
		public static int gangId;

		// Token: 0x04000017 RID: 23
		public static int gangLevel;

		// Token: 0x04000018 RID: 24
		public static bool is_true_taiwu = true;

		// Token: 0x04000019 RID: 25
		public static List<int> new_event_key_list = new List<int>();

		// Token: 0x02000004 RID: 4
		[HarmonyPatch(typeof(Loading), "LoadBaseDate")]
		public static class Loading_LoadBaseDate_Patch
		{
			// Token: 0x0600000C RID: 12 RVA: 0x000030A0 File Offset: 0x000012A0
			public static void Postfix()
			{
				if (!Main.enabled)
				{
					return;
				}
				Main.make_new_event_key_list();
			}
		}

		// Token: 0x02000005 RID: 5
		[HarmonyPatch(typeof(MassageWindow), "EndEvent9005_1")]
		public static class EndEvent9005_1_patch
		{
			// Token: 0x0600000D RID: 13 RVA: 0x000030B0 File Offset: 0x000012B0
			public static bool Prefix()
			{
				if (!Main.enabled)
				{
					return true;
				}
				int num = MassageWindow.instance.eventValue[1];
				if (num <= 2893)
				{
					if (num == 893)
					{
						int num2 = MassageWindow.instance.mianEventDate[1];
						int num3 = DateFile.instance.MianActorID();
						DateFile.instance.mianActorId = num2;
						DateFile.instance.acotrTeamDate[0] = num2;
						DateFile.instance.actorFamilyDate.Remove(num2);
						DateFile.instance.actorFamilyDate.Add(num2);
						for (int i = 1; i < DateFile.instance.acotrTeamDate.Count; i++)
						{
							if (DateFile.instance.acotrTeamDate[i] == num2)
							{
								DateFile.instance.acotrTeamDate[i] = -1;
							}
						}
						if (!Main.settings.family)
						{
							DateFile.instance.actorFamilyDate.Remove(num3);
							DateFile.instance.MoveToPlace(DateFile.instance.mianPartId, DateFile.instance.mianPlaceId, num3, false);
						}
						UIDate.instance.UpdateManpower();
						WorldMapSystem.instance.UpdatePlaceActor(DateFile.instance.mianPartId, DateFile.instance.mianPlaceId, false);
						return false;
					}
					if (num == 1893)
					{
						Main.SetNewMianActor(MassageWindow.instance.mianEventDate[1]);
						DateFile.instance.SetEvent(new int[]
						{
							0,
							-1,
							17
						}, true, true);
						return false;
					}
					if (num == 2893)
					{
						DateFile.instance.RemoveGangActor(int.Parse(DateFile.instance.GetActorDate(MassageWindow.instance.mianEventDate[1], 19, false)), int.Parse(DateFile.instance.GetActorDate(MassageWindow.instance.mianEventDate[1], 20, false)), MassageWindow.instance.mianEventDate[1]);
						DateFile.instance.AddGangDate(16, 9, MassageWindow.instance.mianEventDate[1], true);
						UIDate.instance.UpdateManpower();
						WorldMapSystem.instance.UpdatePlaceActor(DateFile.instance.mianPartId, DateFile.instance.mianPlaceId, false);
						return false;
					}
				}
				else if (num <= 4893)
				{
					if (num == 3893)
					{
						int num4 = MassageWindow.instance.mianEventDate[1];
						if (DateFile.instance.GetFamily(false, true).Contains(num4))
						{
							for (int j = 0; j < 5; j++)
							{
								if (DateFile.instance.acotrTeamDate[j] == num4)
								{
									DateFile.instance.acotrTeamDate[j] = -1;
								}
							}
							DateFile.instance.actorFamilyDate.Remove(num4);
							DateFile.instance.actorsDate[num4].Remove(27);
							DateFile.instance.MoveToPlace(DateFile.instance.mianPartId, DateFile.instance.mianPlaceId, num4, false);
							UIDate.instance.UpdateManpower();
							WorldMapSystem.instance.UpdatePlaceActor(DateFile.instance.mianPartId, DateFile.instance.mianPlaceId, false);
						}
						else
						{
							DateFile.instance.MoveToPlace(DateFile.instance.mianPartId, DateFile.instance.mianPlaceId, num4, false);
							DateFile.instance.MoveOutPlace(num4);
							DateFile.instance.RemoveWorker(num4);
							if (!DateFile.instance.actorFamilyDate.Contains(num4))
							{
								DateFile.instance.actorFamilyDate.Add(num4);
							}
						}
						return false;
					}
					if (num == 4893)
					{
						DateFile.instance.RemoveActor(new List<int>
						{
							MassageWindow.instance.mianEventDate[1]
						}, true, true);
						WorldMapSystem.instance.UpdatePlaceActor(DateFile.instance.mianPartId, DateFile.instance.mianPlaceId, false);
						DateFile.instance.deadActors.Remove(MassageWindow.instance.mianEventDate[1]);
						List<int> list = new List<int>(DateFile.instance.GetLifeDateList(MassageWindow.instance.mianEventDate[1], 801, false))
						{
							MassageWindow.instance.mianEventDate[1]
						};
						if (!DateFile.instance.actorLife[DateFile.instance.mianActorId].ContainsKey(801))
						{
							DateFile.instance.actorLife[DateFile.instance.mianActorId].Add(801, list);
						}
						else
						{
							DateFile.instance.actorLife[DateFile.instance.mianActorId][801].AddRange(list);
						}
						DateFile.instance.actorLife[DateFile.instance.mianActorId][801] = DateFile.instance.actorLife[DateFile.instance.mianActorId][801].Distinct<int>().ToList<int>();
						return false;
					}
				}
				else
				{
					if (num == 5893)
					{
						return false;
					}
					if (num == 6893)
					{
						return false;
					}
				}
				return true;
			}
		}
	}
}
