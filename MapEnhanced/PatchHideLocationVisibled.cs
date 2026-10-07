using HarmonyLib;

namespace MapEnhanced;

/// <summary>
/// 替换原版的 RefreshLuDian / showLuDian，改用 AllMapManageExt 的路点显隐实现。
/// </summary>
public static class PatchHideLocationVisibled
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(AllMapManage), "RefreshLuDian")]
	public static bool Prefix_AllMapManage_RefreshLuDian()
	{
		return false;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(AllMapManage), "RefreshLuDian")]
	public static void Postfix_AllMapManage_RefreshLuDian()
	{
		AllMapManage.instance.Refresh_PathNodes();
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(MapComponent), "showLuDian")]
	public static bool Prefix_MapComponent_ShowLuDian()
	{
		return false;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(MapComponent), "showLuDian")]
	public static void Postfix_MapComponent_ShowLuDian(MapComponent __instance)
	{
		AllMapManage.instance.Updates_PathNodes(__instance);
	}
}
