# Changelog

本项目遵循 [语义化版本](https://semver.org/lang/zh-CN/)。

## [1.0.2] - 2026-11-23

- 修复从文件夹加载预制体或材质时生成缩略图的空引用，并在独立预览场景中渲染缩略图。

## [1.0.1] - 2026-11-23

- 修复 Git 包安装时找不到网格材质和窗口皮肤，导致选择预制体时出现空引用和 GUILayout 报错的问题。
- 重新打开窗口时，先加载调色板，再恢复选中的预制体和绘制工具。

## [1.0.0] - 2026-11-23

### 变更

- 从 MAST（Modular Asset Staging Tool）派生，整理为独立的 UPM 包 `com.chnnasn.unity-3d-tilemap`
- 包根目录定位改为基于 `TileMap3D.Editor.asmdef` 查找，同时兼容「以普通文件夹方式放入 Assets」的安装方式
- 设置数据从包内迁出，首次运行时在 `Assets/TileMap3D/Settings` 自动创建，包内仅保留模板
- 程序集重命名为 `TileMap3D.Editor`，限定为编辑器平台并开启 `autoReferenced`
- 全部资源 GUID 重新生成，包与项目内既有副本互不冲突
- 菜单入口改为 `Tools > 3D TileMap > Open Window`
- 修复 `GUISkin.guiskin` 中指向已失效资源的贴图引用（网格上下移动图标、工具栏状态底图、窗口底图）

### 新增

- `package.json`、`LICENSE`、`CHANGELOG.md` 与完整的使用文档
