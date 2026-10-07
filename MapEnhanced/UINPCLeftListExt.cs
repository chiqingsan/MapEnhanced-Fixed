using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MapEnhanced;

public static class UINPCLeftListExt
{
	/// <summary>用远程 NPC 列表重建左侧 NPC 面板（访问 UINPCLeftList 的私有字段）。</summary>
	public static void RefreshNPC_Remote(this UINPCLeftList __instance, bool distance_check_passed)
	{
		if (Plugin.list_remote_npc == null)
		{
			return;
		}
		Traverse traverse = Traverse.Create(__instance);
		traverse.Field("SVy").SetValue(__instance.SVTransform.anchoredPosition.y);
		if (__instance.SVTransform.childCount > 0)
		{
			ToolsEx.DestoryAllChild(__instance.SVTransform);
		}
		List<UINPCSVItem> sNpcList = traverse.Field("sNpcList").GetValue<List<UINPCSVItem>>();
		List<UINPCSVItem> rNpcList = traverse.Field("rNpcList").GetValue<List<UINPCSVItem>>();
		Dictionary<int, UINPCSVItem> npcDict = traverse.Field("npcDict").GetValue<Dictionary<int, UINPCSVItem>>();
		sNpcList.Clear();
		rNpcList.Clear();
		npcDict.Clear();
		__instance.NPCSVItemList.Clear();
		__instance.CanJiaoYiNpcList.Clear();
		__instance.NowShowedNPCCount = Plugin.list_remote_npc.Count;
		__instance.CountText.text = __instance.NowShowedNPCCount.ToString();
		Traverse lastRefreshTime = traverse.Field("lastRefreshTime");
		foreach (Tuple<int, Tuple<int, string>> remoteNpc in Plugin.list_remote_npc)
		{
			if (npcDict.ContainsKey(remoteNpc.Item1))
			{
				continue;
			}
			int npcId = remoteNpc.Item1;
			UINPCData npcData = new UINPCData(npcId, false);
			if (npcData.IsException)
			{
				continue;
			}
			UINPCSVItem item = Object.Instantiate(__instance.SVItemPrefab, __instance.SVTransform).GetComponent<UINPCSVItem>();
			try
			{
				item.NPCData = npcData;
				if (remoteNpc.Item2.Item1 != -1 && remoteNpc.Item2.Item2 != "")
				{
					item.NPCData.UUID = remoteNpc.Item2.Item2;
					item.NPCData.IsSeaNPC = true;
					item.NPCData.SeaEventID = remoteNpc.Item2.Item1;
				}
				if (!Plugin.Config_RemoteInteraction_Enabled.Value)
				{
					item.transform.Find("NPCBtn").gameObject.SetActive(false);
				}
			}
			catch
			{
				Object.Destroy(item.gameObject);
				continue;
			}
			if (npcId < 20000)
			{
				sNpcList.Add(item);
			}
			else
			{
				rNpcList.Add(item);
			}
			npcDict.Add(npcId, item);
			if (UINPCJiaoHu.Inst.NowJiaoHuNPC != null && UINPCJiaoHu.Inst.NowJiaoHuNPC.ID == npcId)
			{
				__instance.SetNowJiaoHu(item);
			}
		}
		if (Plugin.list_remote_npc.Count > 0)
		{
			sNpcList.Sort();
			rNpcList.Sort();
			foreach (UINPCSVItem item in sNpcList)
			{
				item.transform.SetAsLastSibling();
			}
			foreach (UINPCSVItem item in rNpcList)
			{
				item.transform.SetAsLastSibling();
			}
			__instance.ToRight();
			if (lastRefreshTime.GetValue<string>() == PlayerEx.Player.worldTimeMag.nowTime)
			{
				__instance.Invoke("SetSVY", __instance.SVTransform.childCount * 0.01f);
			}
		}
		else
		{
			__instance.ToLeft();
		}
		foreach (UINPCSVItem item in rNpcList)
		{
			if (item.NPCData.FavorLevel >= 3)
			{
				__instance.CanJiaoYiNpcList.Add(item);
			}
		}
		for (int i = 0; i < __instance.SVTransform.childCount; i++)
		{
			__instance.NPCSVItemList.Add(__instance.SVTransform.GetChild(i).GetComponent<UINPCSVItem>());
		}
		if (UINPCJiaoHu.Inst.QingJiao.gameObject.activeInHierarchy)
		{
			UINPCJiaoHu.Inst.QingJiao.RefreshUI();
		}
		__instance.transform.Find("NPCCount").GetComponent<Text>().text = distance_check_passed ? "远处的人" : "无法触及";
		lastRefreshTime.SetValue(PlayerEx.Player.worldTimeMag.nowTime);
	}
}
