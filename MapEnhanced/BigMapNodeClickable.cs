using KBEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MapEnhanced;

/// <summary>宁州大地图节点：右键远程查看该地点的 NPC。</summary>
public class BigMapNodeClickable : MonoBehaviour
{
	public int node_index = -1;

	public void OnMouseOver()
	{
		if (!Input.GetMouseButtonUp(1))
		{
			return;
		}
		// 其他窗口 UI 打开时不响应（Unity UI 不阻挡物理射线，鼠标在 UI 上时仍会触发 OnMouseOver，
		// 导致"跨窗口点击"误刷新左侧 NPC 列表）
		if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		Avatar player = Tools.instance.getPlayer();
		if (player == null)
		{
			return;
		}
		UINPCJiaoHu.Inst.NPCList.needRefresh = node_index == player.NowMapIndex;
		if (UINPCJiaoHu.Inst.NPCList.needRefresh)
		{
			Plugin.list_remote_npc = null;
		}
		else
		{
			UINPCJiaoHu.Inst.RefreshNowMapNPC_Remote(node_index);
		}
	}
}
