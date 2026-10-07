using System.Collections.Generic;
using GUIPackage;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Avatar = KBEngine.Avatar;

namespace MapEnhanced;

/// <summary>大地图节点的人数标记与点击组件挂载。</summary>
public class Patch
{
	public static FpBtn FindPeopleBtn;

	[HarmonyPostfix]
	[HarmonyPatch(typeof(AllMapShowLine), "Start")]
	public static void Postfix_AllMapShowLine_Start(AllMapShowLine __instance)
	{
		Plugin.list_map_node.Clear();
		Transform labelTemplate = __instance.transform.Find("1/ImageName");
		if (labelTemplate == null)
		{
			return;
		}
		MapComponent[] componentsInChildren = __instance.GetComponentsInChildren<MapComponent>();
		foreach (MapComponent node in componentsInChildren)
		{
			if (node.GetType() != typeof(MapComponent) || node.transform.Find("ImageName") != null)
			{
				continue;
			}
			GameObject label = Object.Instantiate(labelTemplate.gameObject, node.transform);
			label.name = "NumberOfPeopleLabel";
			label.GetComponent<SpriteRenderer>().sprite = null;
			Text labelText = label.GetComponentInChildren<Text>();
			labelText.text = "";
			labelText.fontSize = 24;
			labelText.alignment = TextAnchor.LowerCenter; // 原版 IL 为 (TextAnchor)7 == LowerCenter
			labelText.horizontalOverflow = HorizontalWrapMode.Overflow; // 原版 IL 为 (HorizontalWrapMode)1 == Overflow
			label.transform.localPosition = new Vector3(0f, 0f, 0f);
			Object.Destroy(label.GetComponentInChildren<TextSpacingEasyTest>());
			if (Plugin.Config_RemoteView_Enabled.Value)
			{
				node.gameObject.AddComponent<BigMapNodeClickable>().node_index = node.NodeIndex;
			}
			Plugin.list_map_node.Add(node);
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(UINPCJiaoHu), "RefreshNowMapNPC")]
	public static void Postfix_UINPCJiaoHu_RefreshNowMapNPC()
	{
		string sceneName = SceneManager.GetActiveScene().name;
		if (sceneName != "AllMaps" && (!sceneName.StartsWith("Sea") || !Plugin.Config_RemoteView_Endless_Enbaled.Value))
		{
			Plugin.list_remote_npc = null;
			return;
		}
		if (sceneName != "AllMaps")
		{
			return;
		}
		Avatar player = Tools.instance.getPlayer();
		if (player == null)
		{
			return;
		}
		foreach (BaseMapCompont item in Plugin.list_map_node)
		{
			string text = "";
			if (Plugin.Config_NumberOfPeople_Enabled.Value)
			{
				if (Tool.Check_Node_Distance(player, item.NodeIndex))
				{
					List<int> npcs = Tool.Get_BigMap_NPC_Dictionary(item.NodeIndex);
					text = ((npcs == null) ? Plugin.Config_NumberOfPeople_Placeholder.Value : npcs.Count.ToString());
				}
			}
			item.transform.Find("NumberOfPeopleLabel").GetComponentInChildren<Text>().text = text;
		}
	}
}
