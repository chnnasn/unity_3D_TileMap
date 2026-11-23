# 3D TileMap

面向 Unity 的 **3D 模块化关卡搭建工具**（仅编辑器）。基于网格吸附，把预制体像瓦片一样铺进场景，支持调色板、材质绘制、随机化摆放与预制体批处理。

> 本项目是 [MAST - Modular Asset Staging Tool](https://www.fertilesoilproductions.com/mast)（Fertile Soil Productions，MIT）的衍生与改造版，改造内容包括：网格偏移/分层、宽网格支持、调色板持久化、材质绘制模块、随机化模块等。

## 安装

**方式一：本地路径**

1. 把本仓库克隆到你的 Unity 项目之外的任意目录；
2. 打开 Unity 的 `Window > Package Manager`，左上角 `+` → `Add package from disk...`；
3. 选择本目录下的 `package.json`，确认安装。

**方式二：Git URL**

`Window > Package Manager` → `+` → `Add package from git URL...`，填入本仓库地址。

安装后菜单栏会出现 `Tools > 3D TileMap > Open Window`。

## 使用

1. 打开 `Tools > 3D TileMap > Open Window`；
2. 工具栏里点 **Grid Toggle** 显示/隐藏网格，用 `Grid Up` / `Grid Down` 切换高度层级；
3. 在 **Palette** 里选择瓦片（prefab），在场景中点击或拖拽绘制；
4. **Paint** 标签可以直接给已放置的模型换材质，支持笔刷/油漆桶/滚筒；
5. **Settings** 面板（工具栏齿轮）里调整网格尺寸、偏移、快捷键与随机化参数。

### 网格参数

| 参数 | 说明 |
|---|---|
| Cell Size X / Z | 单个格子在世界空间的边长 |
| Y Unit Size | 每升/降一层的世界高度 |
| Tiling X / Z | 网格覆盖的格数（总宽度 = 2 × Tiling × Cell Size） |
| Offset X / Y / Z | 网格整体平移，用于对齐既有场景 |

### 随机化

随机化可以对摆放的瓦片施加：旋转范围与步进、缩放范围与轴锁、位置偏移、镜像翻转，以及「同组瓦片按权重随机替换」和「禁止连续重复」。每个预制体也可以在 Inspector 上覆盖全局设置（`MASTPrefabSettings` 组件）。

## 项目级设置存放位置

工具的**设置不写在包里**，而是首次运行时自动创建到项目内：

```
Assets/TileMap3D/Settings/
├── CoreSettings.asset      # 路径配置
├── GUI.asset                # 网格 / 调色板 / 工具栏
├── Placement.asset          # 摆放、旋转、射线、随机化
├── Hotkey.asset             # 快捷键
└── InterfaceState.asset     # 界面状态
```

包内 `Editor/Settings/` 与 `Editor/Etc/` 中的同名文件只是**模板**，用于首次运行时初始化项目设置。想改默认值改模板即可，已存在的项目不会被覆盖。

## 包结构

```
unity_3D_TileMap/
├── package.json                 # UPM 包清单
├── LICENSE / CHANGELOG.md
├── Documentation~/Upstream/     # 上游 MAST 的说明文档（Unity 不会导入该目录）
├── Editor/
│   ├── Scripts/                 # 全部 C# 代码（TileMap3D.Editor.asmdef，仅编辑器）
│   │   ├── Building/            # 网格、摆放、随机化、可视化、涂区、调色板
│   │   ├── Painting/             # 材质绘制（笔刷 / 油漆桶 / 滚筒）
│   │   ├── Component/           # MASTPrefabSettings、缩略图相机
│   │   ├── Settings/            # 设置数据结构与读写
│   │   ├── Tools/               # 预制体生成、合批、网格工具
│   │   └── GUI/                 # 工具窗口、快捷键、状态
│   ├── Etc/                     # GUISkin、网格/涂区材质、橡皮刀、预览相机
│   ├── Images/                  # 工具栏图标
│   └── Settings/                # 设置模板（GUI / Placement / Hotkey）
```

## 环境要求

- Unity **2022.3** 及以上（`package.json` 中的 `unity` 字段声明最低版本）
- 仅编辑器功能，运行时无任何依赖，不进入玩家构建

## 许可

MIT。上游 MAST 的版权声明保留在 [LICENSE](LICENSE) 中，改造部分版权归 chnnasn。
