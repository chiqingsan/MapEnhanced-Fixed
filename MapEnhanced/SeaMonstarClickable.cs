using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MapEnhanced;

/// <summary>海上怪船：右键查看船上 NPC。</summary>
public class SeaMonstarClickable : MonoBehaviour
{
	public SeaAvatarObjBase sea_object;

	public void OnMouseOver()
	{
		if (!Input.GetMouseButtonUp(1))
		{
			return;
		}
		// 其他窗口 UI 打开时不响应，避免"跨窗口点击"误刷新左侧 NPC 列表
		if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		UINPCJiaoHu.Inst.NPCList.needRefresh = sea_object.NowMapIndex == WASDMove.Inst.GetNowIndex();
		if (UINPCJiaoHu.Inst.NPCList.needRefresh)
		{
			Plugin.list_remote_npc = null;
			return;
		}
		if (Plugin.list_remote_npc == null)
		{
			Plugin.list_remote_npc = new List<Tuple<int, Tuple<int, string>>>();
		}
		else
		{
			Plugin.list_remote_npc.Clear();
		}
		Plugin.list_remote_npc.Add(Tuple.Create(NPCEx.GetSeaNPCIDByEventID(sea_object._EventId), Tuple.Create(sea_object._EventId, sea_object.UUID)));
		UINPCJiaoHu.Inst.NPCList.RefreshNPC_Remote(distance_check_passed: true);
	}
}
