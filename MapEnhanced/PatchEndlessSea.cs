using HarmonyLib;
using UnityEngine;

namespace MapEnhanced;

/// <summary>无尽海域：给海图节点和海上怪船挂点击组件。</summary>
public static class PatchEndlessSea
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(SeaGrid), "Awake")]
	public static void Postfix_SeaGrid_Awake(SeaGrid __instance)
	{
		Plugin.list_map_node.Clear();
		MapSeaCompent[] componentsInChildren = __instance.GetComponentsInChildren<MapSeaCompent>();
		foreach (MapSeaCompent node in componentsInChildren)
		{
			if (node.GetType() == typeof(MapSeaCompent))
			{
				node.gameObject.AddComponent<SeaMapNodeClickable>();
			}
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(EndlessSeaMag), "CreateMonstar")]
	public static void Postfix_EndlessSeaMag_CreateMonstar(EndlessSeaMag __instance)
	{
		// 列表为空或该对象已挂过组件时跳过：避免越界异常打断游戏自己的船只创建流程，
		// 也避免组件被重复叠加到同一艘船上
		if (__instance.MonstarList == null || __instance.MonstarList.Count == 0)
		{
			return;
		}
		SeaAvatarObjBase monstar = __instance.MonstarList[__instance.MonstarList.Count - 1];
		if (monstar.ThinkType != 12 || monstar.gameObject.GetComponent<SeaMonstarClickable>() != null)
		{
			return;
		}
		BoxCollider collider = monstar.gameObject.AddComponent<BoxCollider>();
		collider.center = new Vector3(0f, 0.8f, 0f);
		collider.size = new Vector3(2f, 2f, 2f);
		monstar.gameObject.AddComponent<SeaMonstarClickable>().sea_object = monstar;
	}
}
