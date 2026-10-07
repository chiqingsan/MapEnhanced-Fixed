# 地图增强 MapEnhanced（修复版 v1.7.0）

基于 Shiro 的 Steam 创意工坊 mod《地图增强》（工坊 ID `2825162391`，v1.4.1，2022-11 后停更）的修复版。在保持原有架构与行为的前提下做了针对性修复，以解决其与当前游戏版本及常见 mod 组合的兼容性问题（进入灵界等新增地图时闪退、多 mod 环境下 NPC 列表过长时出现头像空白等）。**如有侵权立即删除。**

## 与原版的差异（本版修复内容）

| 问题（原版 1.4.1） | 本版修复 |
|---|---|
| 进入 mod 新增地图（灵界 mod·砺剑峰等）时报 `KeyNotFoundException`，进图报错/闪退 | 玩家当前地图不在宁州大地图节点表时，安全跳过路点刷新 |
| 左侧 NPC 列表在特定环境下的两类缺陷：①每帧抛出异常（游戏原版 `RefreshNPCHeadShow` 的 catch 中 `RemoveAt` 在列表失同步时自身越界、异常逃逸），面板闪烁、errorlog 刷屏；②头像视口裁剪窗口写死为 8 项（960px），高分辨率下列表可视区约 10 项（1200px），屏幕内头像被误判为“视口外”而隐藏，出现视野内头像空白 | 以索引安全的方式重新实现该裁剪逻辑（不删除列表项、条目缺失时安全跳过），消除每帧异常；**窗口修正为 10 项（10 × 120px），匹配实际可视区**——屏幕内头像不再空白，隐藏只发生在屏幕外（滚动时划入即显示） |
| 列表重建后头像永久空缺（重建帧新旧项共存，导致内部索引列表错位）；滚动划入划出时头像被重置为玩家 | 裁剪按视觉位置直接取子物体组件（不再依赖内部索引列表）；列表项上的 `PlayerSetRandomFace.setFace` 跳过 |
| 打开其他窗口时点击地图会穿透误刷新左侧 NPC 列表（Unity UI 不阻挡物理射线，右键穿透触发“远程查看/刷新”） | 三个点击组件（大地图节点 / 海图节点 / 灵舟）在鼠标位于 UI 上时不响应 |
| 海域船只补丁在 `MonstarList` 为空时抛出越界异常（会打断游戏自身的创建流程）、或对同一船只重复叠加组件 | 增加空列表保护与重复组件检查 |
| 大地图人数标记文字的对齐与原版不一致 | 修正为与原版一致的 `LowerCenter`（下居中） |

## 功能（与原版一致）

- 宁州大地图野外地点的 **NPC 人数标记**（受神识范围限制，可在配置中关闭）
- 大地图/海域节点 **右键远程查看**该地点的 NPC 列表
- 支路（路点）标记显隐（替换原版 `RefreshLuDian` 实现）
- 海域支持：海图节点、NPC 灵舟右键查看

## 性能（与原版 1.4.1 对比）

- **每帧路径**
  - 头像显隐：与原版同为每帧遍历列表；本版以安全方式实现（列表异常时不再抛异常、不再每帧栈展开），净开销低于原版
  - 左侧面板可见性判断（`CanShow`）：与原版一致
  - 点击组件：与原版一致（UI 遮挡检查仅在鼠标右键抬起的那一帧执行，悬停帧的开销与原来相同）
- **低频路径**（地图刷新、场景加载、右键交互）：与原版等价或更优——字典访问由“`ContainsKey` + 索引器”双查找优化为单次 `TryGetValue`；路点淡变仅在需要动画时才查找子对象；跳过原版异常方法后省去其每次调用的循环
- 无每帧内存分配、无 LINQ、无反射热路径（`Traverse` 反射仅在远程查看时使用，为原版设计）

## 与原版的兼容性

- `BepInPlugin` GUID、**全部配置项 key、Harmony patch 目标与原版完全一致**——已有的 BepInEx 配置文件直接沿用
- 类名、方法名保持原样，便于与原版对照
- 除上表修复点外，所有逻辑与原版行为等价（逐项反编译比对）

## 构建

前置：.NET SDK（6.0 实测通过，`LangVersion 10`）；本机装有游戏与 BepInEx。

1. 编辑 `MapEnhanced.csproj` 里的路径：
   ```xml
   <GameDir>D:\app\steam\steamapps\common\觅长生</GameDir>
   <WorkshopDir>D:\app\steam\steamapps\workshop\content\1189490</WorkshopDir>
   ```
   （`WorkshopDir\2824349934\BepInEx\core` 下是 BepInEx 与 0Harmony 引用）
2. 编译：
   ```
   dotnet build -c Release
   ```
3. 产物：`bin\Release\MapEnhanced.dll`

## 部署

将编译产物（或 `release\MapEnhanced-1.7.0.dll`）替换工坊 mod 的插件文件：

```
steamapps\workshop\content\1189490\2825162391\plugins\MapEnhanced.dll
```

建议保留原版备份为 `MapEnhanced.dll.1.4.1.bak`（改为其他后缀后 BepInEx 不会加载它）。回滚：换回原名即可。

## 代码结构

```
MapEnhanced.csproj            工程文件（引用路径按本机配置）
Properties/AssemblyInfo.cs    程序集信息（版本号在此同步）
MapEnhanced/
  Plugin.cs                   BepInEx 入口 + 配置项定义 + patch 注册
  Patch.cs                    大地图节点人数标记 / 点击组件挂载
  PatchHideLocationVisibled.cs  替换原版 RefreshLuDian / showLuDian 跳板
  AllMapManageExt.cs          路点显隐主逻辑（含新增地图 KeyNotFound 修复）
  PatchRemoteView.cs          远程查看（CanShow 等补丁）
  PatchSafeHeadShow.cs        左侧列表头像视口裁剪（安全实现，按视觉位置取项）
  PatchListAvatarFix.cs       列表项头像重置修复（setFace 跳过）
  PatchEndlessSea.cs          海域支持（含船只补丁防护）
  Tool.cs                     NPC 字典查询 / 神识距离检查
  BigMapNodeClickable.cs      大地图节点右键组件（UI 遮挡防护）
  SeaMapNodeClickable.cs      海图节点右键组件（UI 遮挡防护）
  SeaMonstarClickable.cs      灵舟右键组件（UI 遮挡防护）
  UINPCJiaoHuExt.cs           远程查看入口
  UINPCLeftListExt.cs         远程 NPC 列表 UI 重建
```

## 注意事项

- **Steam 更新该 mod 时会覆盖修复版 DLL**，需要重新替换（可用 `release\MapEnhanced-1.7.0.dll`）
- 反编译/比对工具：`dotnet tool install --global ilspycmd --version 8.2.0.7535`（新版需 .NET 8）
- 排查加载问题：查看 `workshop\content\1189490\2824349934\BepInEx\LogOutput.log`，搜索 `Loading [地图增强`；游戏内报错在 `MCSSave\errorlog.log`
