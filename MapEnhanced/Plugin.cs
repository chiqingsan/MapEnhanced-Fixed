using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace MapEnhanced;

[BepInPlugin("bepinex.plugin.mcs.MapEnhanced", "地图增强", "1.7.0")]
public class Plugin : BaseUnityPlugin
{
	public static ConfigEntry<bool> Config_Plugin_Enabled;

	public static ConfigEntry<bool> Config_NumberOfPeople_Enabled;

	public static ConfigEntry<string> Config_NumberOfPeople_Placeholder;

	public static ConfigEntry<bool> Config_RangeLimited_Enbaled;

	public static ConfigEntry<bool> Config_RemoteView_Enabled;

	public static ConfigEntry<bool> Config_RemoteInteraction_Enabled;

	public static ConfigEntry<bool> Config_HideLocationVisibled_Enabled;

	public static ConfigEntry<bool> Config_RemoteView_Endless_Enbaled;

	public static List<BaseMapCompont> list_map_node = new List<BaseMapCompont>();

	public static List<System.Tuple<int, System.Tuple<int, string>>> list_remote_npc;

	public void Start()
	{
		Config_Plugin_Enabled = ((BaseUnityPlugin)this).Config.Bind("Plugin", "Enabled", true, "插件（启用：true/禁用：false）");
		Config_RangeLimited_Enbaled = ((BaseUnityPlugin)this).Config.Bind("Plugin.RangeLimited", "Enabled", true, "神识限制范围（启用：true/禁用：false）");
		Config_RemoteView_Enabled = ((BaseUnityPlugin)this).Config.Bind("Plugin.RemoteView", "Enabled", true, "远程查看（启用：true/禁用：false）");
		Config_RemoteView_Endless_Enbaled = ((BaseUnityPlugin)this).Config.Bind("Plugin.RemoteView", "EndlessSea", true, "支持海域（启用：true/禁用：false）");
		Config_RemoteInteraction_Enabled = ((BaseUnityPlugin)this).Config.Bind("Plugin.RemoteInteraction", "Enabled", false, "远程互动（启用：true/禁用：false）");
		Config_HideLocationVisibled_Enabled = ((BaseUnityPlugin)this).Config.Bind("Plugin.HideLocationVisibled", "Enabled", true, "支路标记（启用：true/禁用：false）");
		Config_NumberOfPeople_Enabled = ((BaseUnityPlugin)this).Config.Bind("Plugin.NumberOfPeople", "Enabled", true, "人数标记（启用：true/禁用：false）");
		Config_NumberOfPeople_Placeholder = ((BaseUnityPlugin)this).Config.Bind("Plugin.NumberOfPeople", "Placeholder", "*", "神识范围内的地点，若没有人时标记显示的内容（可以为空）");

		if (!Config_Plugin_Enabled.Value)
		{
			return;
		}
		Harmony.CreateAndPatchAll(typeof(Patch), null);
		Harmony.CreateAndPatchAll(typeof(PatchSafeHeadShow), null);
		Harmony.CreateAndPatchAll(typeof(PatchListAvatarFix), null);
		if (Config_RemoteView_Enabled.Value)
		{
			Harmony.CreateAndPatchAll(typeof(PatchRemoteView), null);
		}
		if (Config_RemoteView_Endless_Enbaled.Value)
		{
			Harmony.CreateAndPatchAll(typeof(PatchEndlessSea), null);
		}
		if (Config_HideLocationVisibled_Enabled.Value)
		{
			Harmony.CreateAndPatchAll(typeof(PatchHideLocationVisibled), null);
		}
	}
}
