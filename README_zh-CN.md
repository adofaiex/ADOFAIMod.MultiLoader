# ADOFAI 多加载器 Mod 模板

一个用于创建《A Dance of Fire and Ice》（ADOFAI）Mod 的项目模板，支持多种加载器：
Unity Mod Manager、MelonLoader、BepInEx 和 Doorstop 独立模式。

## 项目结构

```
ProjectRoot/
├── core/
│   ├── AdofaiMod.MultiLoader.Core.csproj   -- 共享核心逻辑
│   ├── IHandler.cs                         -- 加载器抽象接口
│   ├── Main.cs                             -- 入口点 (Initialize)
│   ├── Settings.cs                         -- 可序列化的设置
│   ├── Patches.cs                          -- Harmony 补丁
│   └── ResourceLoader.cs                   -- 文件加载工具
├── loaders/
│   ├── umm/                                -- Unity Mod Manager 适配
│   ├── melon/                              -- MelonLoader 适配
│   ├── bepinex/                            -- BepInEx 适配
│   └── doorstop/                           -- Doorstop 独立模式适配
├── scripts/
│   ├── pack.csx                            -- 分发 zip 打包脚本
│   ├── pack.cmd                            -- Windows 打包命令
│   ├── pack.ps1                            -- PowerShell 打包命令
│   └── pack.sh                             -- Linux/macOS 打包命令
├── Resources/                              -- Mod 资源文件（文本、图片等）
├── ADOFAIMod.targets                       -- MSBuild 构建目标
└── Info.json                               -- UMM 清单（仅选择 UMM 时生成）
```

## 架构说明

每个加载器对应一个独立的适配项目，均引用 `core/` 共享项目。
`IHandler` 接口封装了日志、设置和生命周期事件，核心 Mod 代码无需感知具体加载器。

```
加载器项目 (如 loaders/umm/)
  └── 实现 IHandler
      └── 调用 Main.Initialize(handler)
          └── core/ 代码与加载器无关
```

## 环境要求

- .NET SDK 6.0 或更高版本
- A Dance of Fire and Ice (Steam)
- 游戏目录中安装了一个或多个支持的加载器

## 安装模板

```bash
# 从本地 NuGet 包安装
dotnet new install path/to/AdofaiMod.MultiLoader.1.0.0.nupkg

# 或从本地仓库目录安装
dotnet new install path/to/AdofaiMod.MultiLoader
```

## 创建项目

### 命令行

```bash
# 全部四个加载器（默认）
dotnet new adofaiml -n MyMod -g "C:\Games\ADOFAI\A Dance of Fire and Ice.exe"

# 选择部分加载器（关闭不需要的）
dotnet new adofaiml -n MyMod --bepinex false --doorstop false -g "C:\Games\ADOFAI\A Dance of Fire and Ice.exe"

# 指定作者和描述
dotnet new adofaiml -n MyMod -a "YourName" -d "我的第一个 ADOFAI Mod" -g "path/to/game.exe"
```

### Visual Studio / JetBrains Rider

安装模板后，新建项目并搜索 "ADOFAI" 或 "adofaiml"。
新建项目向导中会为每个加载器显示复选框。

### 参数说明

| 短参数 | 长参数 | 说明 |
|---|---|---|
| `-n` | `--name` | 项目名称（同时也是 Mod 名称） |
| `-g` | `--game-path` | 游戏可执行文件路径（构建必需） |
| `-a` | `--author` | 作者名称 |
| `-d` | `--description` | Mod 描述 |
| `-v` | `--version` | 初始版本号（默认：1.0.0） |
| `-um` | `--umm` | 包含 UMM 加载器（默认：true） |
| `-ml` | `--melon` | 包含 MelonLoader（默认：true） |
| `-bx` | `--bepinex` | 包含 BepInEx（默认：true） |
| `-ds` | `--doorstop` | 包含 Doorstop（默认：true） |

## 构建与部署

在 `.csproj` 文件中设置 `GameExePath`（或通过 `-p` 参数传入），然后：

```bash
# Debug：构建 + 部署到游戏目录 + 启动游戏
dotnet build -p:Loader=UMM
dotnet build -p:Loader=ML
dotnet build -p:Loader=BepInEx
dotnet build -p:Loader=Doorstop

# Release：仅构建，输出到 out/
dotnet build -c Release
```

各加载器部署路径：

| 加载器 | 目标目录 |
|---|---|
| `UMM` | `游戏目录/Mods/{Mod名称}/` |
| `ML` | `游戏目录/Mods/` |
| `BepInEx` | `游戏目录/BepInEx/plugins/{Mod名称}/` |
| `Doorstop` | `游戏目录/`（根目录，与 doorstop_config.ini 同级） |

解决方案中的每个项目均可独立构建。无论构建哪个加载器，
`out/` 目录都会以扁平结构收集所有输出文件：

```
out/
├── {Mod名称}.Core.dll
├── {Mod名称}.Loader.UMM.dll        （如果构建了 UMM）
├── {Mod名称}.Loader.Melon.dll      （如果构建了 Melon）
├── {Mod名称}.Loader.BepInEx.dll    （如果构建了 BepInEx）
├── {Mod名称}.Loader.Doorstop.dll   （如果构建了 Doorstop）
├── Info.json                        （仅构建 UMM 时存在）
├── doorstop_config.ini              （仅构建 Doorstop 时存在）
└── Resources/
```

## 制作分发包

需要 `dotnet script`（通过 `dotnet tool install -g dotnet-script` 安装）。

```bash
# 先 Release 构建，再打包
scripts/pack.cmd      # Windows
scripts/pack.sh       # Linux/macOS
scripts/pack.ps1      # PowerShell
```

脚本会在 `dist/` 目录下生成按加载器分类的 ZIP 压缩包：

| 文件 | 内部结构（相对于游戏根目录） |
|---|---|
| `{Mod名称}_umm.zip` | `Mods/{Mod名称}/` 扁平 |
| `{Mod名称}_melon.zip` | `Mods/` 扁平 |
| `{Mod名称}_bepinex.zip` | `BepInEx/plugins/{Mod名称}/` 扁平 |
| `{Mod名称}_doorstop.zip` | 根目录扁平（含 doorstop_config.ini） |

每个压缩包解压到游戏根目录即可直接使用。

## 开发指南

1. 在 `core/` 中编写 Mod 逻辑，代码不与任何特定加载器耦合。
2. 通过 `Main.Handler` 进行日志输出，`Main.Settings` 读写配置。
3. 在 `core/` 中添加 Harmony 补丁类，启用 Mod 时自动应用。
4. 资源文件放入 `Resources/`，使用 `ResourceLoader` 加载。
5. 使用对应加载器构建，在游戏中测试。

## GitHub 模板

本仓库也可以作为 GitHub 模板使用。在仓库页面点击 "Use this template"
创建新仓库，然后克隆并运行初始化脚本，项目会自动按仓库名称重命名：

```bash
# Linux / macOS
chmod +x init.sh
./init.sh

# Windows (PowerShell)
.\init.ps1
```

初始化脚本会自动读取仓库目录名，替换所有文件中的旧名称、重命名文件
和目录，并重新初始化 Git 历史。

## 卸载模板

```bash
dotnet new uninstall AdofaiMod.MultiLoader
```

## 许可证

GPL-3.0-or-later
