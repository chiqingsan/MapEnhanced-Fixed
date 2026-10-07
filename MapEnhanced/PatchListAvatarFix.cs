using HarmonyLib;

namespace MapEnhanced;

/// <summary>
/// 列表项头像重置修复（1.6.2 引入）。
///
/// 机制：列表项的头像组件是游戏原版的 PlayerSetRandomFace——它是为"玩家自己的头像"
/// 设计的，却被复用到了列表项上。其 OnEnable 会调用 setFace()，而 setFace() 在
/// canInit（默认 true、类内从不置 false）时执行 randomAvatar(1)——把头像重置为"1"（玩家自己）。
/// 首次创建时这一重置紧接着被 NPCData setter → RefreshUI → SetNPCFace(npcid) 纠正，
/// 所以显示正常；但任何"重新激活"（视口裁剪的 SetActive 关-开、滚动划入划出）都会
/// 再次触发 OnEnable → setFace() → 重置为"1"，而这次没有任何纠正点——
/// "1"的立绘在列表项上下文加载失败（安静失败、无异常日志），表现为头像透明空框。
///
/// 修复：setFace() 在宿主属于列表项（UINPCSVItem 下）时跳过——列表项头像由
/// SetNPCFace(npcid) 负责，"重置为玩家自己"的逻辑只应在真正的玩家头像实例上执行。
/// </summary>
public static class PatchListAvatarFix
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(PlayerSetRandomFace), "setFace")]
	public static bool Prefix_SetFace(PlayerSetRandomFace __instance)
	{
		return __instance.GetComponentInParent<UINPCSVItem>() == null;
	}
}
