using UnityEngine;
using UnityEngine.EventSystems;

namespace MapEnhanced;

/// <summary>海图节点：右键恢复默认的附近 NPC 列表。</summary>
public class SeaMapNodeClickable : MonoBehaviour
{
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
		Plugin.list_remote_npc = null;
		UINPCJiaoHu.Inst.NPCList.needRefresh = true;
	}
}
