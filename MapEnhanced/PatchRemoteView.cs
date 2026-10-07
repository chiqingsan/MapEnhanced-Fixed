using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MapEnhanced;

/// <summary>远程查看：在宁州大地图/海域上右键节点查看其中的 NPC 列表。</summary>
public class PatchRemoteView
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(UINPCJiaoHu), "AutoShowNPCList")]
	public static void Postfix_UINPCJiaoHu_AutoShowNPCList(UINPCJiaoHu __instance)
	{
		if (Plugin.list_remote_npc != null)
		{
			__instance.NPCList.needRefresh = false;
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(BaseMapCompont), "movaAvatar")]
	public static void Postfix_BaseMapCompont_movaAvatar()
	{
		Plugin.list_remote_npc = null;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(UINPCLeftList), "RefreshNPC")]
	public static void Prefix_UINPCLeftList_RefreshNPC(UINPCLeftList __instance)
	{
		Plugin.list_remote_npc = null;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(UINPCLeftList), "CanShow")]
	public static bool Prefix_UINPCLeftList_CanShow(ref bool __result)
	{
		// 条件与原版 UINPCLeftList.CanShow 等价，仅在大地图/海域放宽 HasNPC 要求（支持远程查看空地点）
		__result = !UINPCLeftList.ShoudHide
			&& !UINPCJiaoHu.AllShouldHide
			&& (PanelMamager.inst == null || PanelMamager.inst.UISceneGameObject != null)
			&& (PanelMamager.inst == null || (int)PanelMamager.inst.nowPanel == 5 /* 5 == PanelMamager.PanelType.空 */)
			&& NpcJieSuanManager.inst.isCanJieSuan;
		string sceneName = SceneManager.GetActiveScene().name;
		if (sceneName != "AllMaps" && (!sceneName.StartsWith("Sea") || !Plugin.Config_RemoteView_Endless_Enbaled.Value))
		{
			__result = __result && UINPCLeftList.HasNPC;
		}
		return false;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(UINPCLeftList), "RefreshNPC")]
	public static void Postfix_UINPCLeftList_RefreshNPC(UINPCLeftList __instance)
	{
		__instance.transform.Find("NPCCount").GetComponent<Text>().text = "附近的人";
		string sceneName = SceneManager.GetActiveScene().name;
		if ((sceneName == "AllMaps" || (sceneName.StartsWith("Sea") && Plugin.Config_RemoteView_Endless_Enbaled.Value))
			&& (Plugin.list_remote_npc == null || Plugin.list_remote_npc.Count == 0)
			&& !UINPCLeftList.HasNPC
			&& !__instance.nowLeft)
		{
			__instance.ToLeft();
		}
	}
}
