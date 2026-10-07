using System.Collections.Generic;
using Fungus;
using JSONClass;
using KBEngine;
using UnityEngine;
using GameObject = UnityEngine.GameObject;

namespace MapEnhanced;

/// <summary>
/// 大地图支路（路点）显隐逻辑，替换原版 AllMapManage.RefreshLuDian。
/// 1.4.2 修复：玩家位于 mod 新增地图（灵界的砺剑峰、月影寒渊等）时，
/// player.NowMapIndex 不在 AllMapManage.mapIndex 中，原实现直接按 key 取值抛 KeyNotFoundException。
/// </summary>
public static class AllMapManageExt
{
	public static void Refresh_PathNodes(this AllMapManage __instance)
	{
		__instance.Updates_PathNodes();
	}

	public static void Updates_PathNodes(this AllMapManage __instance, MapComponent path_node_instance = null)
	{
		Avatar player = Tools.instance.getPlayer();
		if (player == null || __instance.LuXianGroup == null)
		{
			return;
		}

		// 玩家当前不在宁州大地图的任何节点上（副本/海域/灵界新增地图）时，无需刷新路点显隐
		MapComponent currentNode = AllMapManage.instance.mapIndex.TryGetValue(player.NowMapIndex, out BaseMapCompont currentObj) ? currentObj as MapComponent : null;
		if (currentNode == null)
		{
			return;
		}

		List<MapComponent> list = new List<MapComponent>();
		foreach (KeyValuePair<int, BaseMapCompont> item in __instance.mapIndex)
		{
			MapComponent val = item.Value as MapComponent;
			if (val == null
				|| !AllMapLuDainType.DataDict.TryGetValue(val.NodeIndex, out AllMapLuDainType luDainType)
				|| luDainType.MapType != 1
				|| val.NodeGroup == 0)
			{
				continue;
			}

			if (val.TaskSpine.initialSkinName != "default" && Tool.Check_Node_Distance(player, val.NodeIndex))
			{
				list.Add(val);
			}

			SetNodeVisible(val, fadeChild: true, val.NodeGroup == currentNode.NodeGroup || list.Contains(val), path_node_instance, includeChildren: true);
		}

		AllMapsLuXian[] componentsInChildren = __instance.LuXianGroup.GetComponentsInChildren<AllMapsLuXian>(true);
		foreach (AllMapsLuXian val2 in componentsInChildren)
		{
			if (val2 == null)
			{
				continue;
			}

			SetNodeVisible(val2, fadeChild: false, val2.NodeGroup == currentNode.NodeGroup, path_node_instance, includeChildren: false);
		}
	}

	/// <summary>切换单个路点/支路的显示状态，animator 非空时带淡入淡出动画，否则直接 SetActive。</summary>
	private static void SetNodeVisible(Component node, bool fadeChild, bool visible, MapComponent animator, bool includeChildren)
	{
		if (visible)
		{
			node.gameObject.SetActive(true);
			if (animator)
			{
				FadeTo(node, fadeChild, 1f, includeChildren);
			}
		}
		else if (node.gameObject.activeSelf)
		{
			if (animator)
			{
				animator.StartCoroutine(animator.setGameobjectActive(node.gameObject, false, 1f));
				FadeTo(node, fadeChild, 0f, includeChildren);
			}
			else
			{
				node.gameObject.SetActive(false);
				iTween.FadeTo(node.gameObject, 0f, 1f);
			}
		}
	}

	/// <summary>淡变目标：fadeChild 为 true 时是路点挂的 "Level1Move" 子物体，否则是节点自身。仅在需要动画时才做 Find。</summary>
	private static void FadeTo(Component node, bool fadeChild, float alpha, bool includeChildren)
	{
		GameObject target = fadeChild ? node.transform.Find("Level1Move").gameObject : node.gameObject;
		iTween.FadeTo(target, iTween.Hash("alpha", alpha, "time", 0.6f, "EaseType", "linear", "includechildren", includeChildren));
	}
}
