using System;
using System.Collections.Generic;
using KBEngine;
using UnityEngine;

namespace MapEnhanced;

public static class Tool
{
	public static List<int> Get_BigMap_NPC_Dictionary(int node_index)
	{
		NPCMap npcMap = NpcJieSuanManager.inst.npcMap;
		if (npcMap != null && npcMap.bigMapNPCDictionary != null && npcMap.bigMapNPCDictionary.ContainsKey(node_index))
		{
			List<int> npcs = npcMap.bigMapNPCDictionary[node_index];
			if (npcs.Count > 0)
			{
				return npcs;
			}
		}
		return null;
	}

	public static bool Check_Node_Distance(Avatar player, int node_index)
	{
		if (Plugin.Config_RangeLimited_Enbaled.Value
			&& AllMapManage.instance.mapIndex.TryGetValue(player.NowMapIndex, out BaseMapCompont playerNode)
			&& AllMapManage.instance.mapIndex.TryGetValue(node_index, out BaseMapCompont targetNode))
		{
			float distance = Vector3.Distance(playerNode.transform.position, targetNode.transform.position);
			return player.shengShi >= Math.Log(distance, 2.0) * distance;
		}
		return true;
	}
}
