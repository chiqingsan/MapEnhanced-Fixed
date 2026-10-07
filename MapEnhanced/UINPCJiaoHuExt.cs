using System;
using System.Collections.Generic;
using KBEngine;
using UnityEngine.SceneManagement;

namespace MapEnhanced;

public static class UINPCJiaoHuExt
{
	/// <summary>远程刷新指定地点的 NPC 交互列表。</summary>
	public static void RefreshNowMapNPC_Remote(this UINPCJiaoHu __instance, int node_index)
	{
		if (Plugin.list_remote_npc == null)
		{
			Plugin.list_remote_npc = new List<Tuple<int, Tuple<int, string>>>();
		}
		else
		{
			Plugin.list_remote_npc.Clear();
		}
		Avatar player = Tools.instance.getPlayer();
		if (player == null)
		{
			return;
		}
		if (SceneManager.GetActiveScene().name != "AllMaps")
		{
			return;
		}
		bool distance_check_passed = Tool.Check_Node_Distance(player, node_index);
		if (distance_check_passed)
		{
			List<int> npcs = Tool.Get_BigMap_NPC_Dictionary(node_index);
			if (npcs != null)
			{
				foreach (int npcId in npcs)
				{
					Plugin.list_remote_npc.Add(Tuple.Create(npcId, Tuple.Create(-1, "")));
				}
			}
		}
		__instance.NPCList.RefreshNPC_Remote(distance_check_passed);
	}
}
