using HarmonyLib;
using UnityEngine;

namespace MapEnhanced;

/// <summary>
/// UINPCLeftList.RefreshNPCHeadShow 的兼容修复（1.6.5）。
///
/// 原版行为：每帧隐藏滚动视口外的头像（视口裁剪，原版窗口 = 8 项 × 120px），用于长列表性能优化。
/// 本版的窗口为 10 项（10 × 120px = 1200px）——本机显示环境下列表可视区约 10 项，
/// 原版 8 项窗口会把视野内后几项误隐藏（表现为视野内头像整块消失）。
/// 另保留异常安全防护（原版 catch 中的 RemoveAt 在列表失同步时自身越界、异常每帧逃逸）。
///
/// 1.6.5 关键修复（"对话后/重进后头像永久空缺"的根源）：
/// 列表重建的那一帧，旧项延迟销毁、新旧项共存（children 临时翻倍，如 26 → 52），
/// 而 NPCSVItemList 会在该帧按翻倍后的数量重建——前段全是"即将/已被销毁项"的引用
/// （伪 null）。按其索引访问时新项永远取不到（此前实现直接 continue 跳过），
/// 导致新项被上一帧关闭后再无人打开（滚动、重进均不恢复；短列表因从不被关而无感）。
/// 现改为按"视觉位置"直接取子物体上的组件（GetChild(i).GetComponent），
/// 索引与显示顺序恒一致，不再依赖 NPCSVItemList 的状态。
/// </summary>
[HarmonyPatch(typeof(UINPCLeftList), "RefreshNPCHeadShow")]
public static class PatchSafeHeadShow
{
	private const float ItemHeight = 120f;
	/// <summary>显示窗口的项数：10 项（匹配本机列表可视区，避免视野内头像被误隐藏）。</summary>
	private const int ViewWindow = 10;

	[HarmonyPrefix]
	public static bool Prefix(UINPCLeftList __instance)
	{
		RectTransform sv = __instance.SVTransform;
		int childCount = sv.childCount;
		int first = (int)(Mathf.Max(0f, sv.anchoredPosition.y) / ItemHeight);
		int showFrom = first;
		int showTo = first + ViewWindow - 1;

		for (int i = childCount - 1; i >= 0; i--)
		{
			Transform child = sv.GetChild(i);
			if (child == null)
			{
				continue;
			}
			UINPCSVItem item = child.GetComponent<UINPCSVItem>();
			if (item == null)
			{
				continue;
			}
			GameObject head = item.HeadObj;
			if (head == null)
			{
				continue;
			}
			if (i < showFrom || i > showTo)
			{
				if (head.activeSelf)
				{
					head.SetActive(value: false);
				}
			}
			else if (!head.activeSelf)
			{
				head.SetActive(value: true);
			}
		}
		return false;
	}
}
