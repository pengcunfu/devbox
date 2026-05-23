# ProgramBox - 编程环境管理工具

ProgramBox 是一款专为程序员设计的多语言开发环境管理工具，采用 Avalonia UI 跨平台框架构建。

## 🚀 项目架构

本项目从 WPF 迁移到 Avalonia UI，保留了所有原有功能：

### 技术栈
- **.NET 10.0**: 现代化的 .NET 平台
- **Avalonia UI 11.3.6**: 跨平台 UI 框架
- **Newtonsoft.Json**: JSON 数据序列化

### 项目结构
```
ProgramBox/
├── Models/              # 数据模型
│   ├── AppConfig.cs    # 应用配置
│   └── Atom.cs         # 原子类（WebAtom, NativeAtom, InsAtom, WinToolAtom）
├── Utils/              # 工具类
│   ├── AppHelper.cs    # 应用帮助工具
│   ├── JsonDataManager.cs  # JSON数据管理
│   └── WindowsToolsHelper.cs  # Windows工具助手
├── Views/              # 视图
│   ├── DevelopmentToolsView.axaml  # 开发环境管理视图
│   ├── SystemToolsView.axaml      # 系统工具视图
│   ├── ApplicationsView.axaml     # 应用程序管理视图
│   └── SettingsWindow.axaml       # 设置窗口
├── App.axaml           # 应用程序入口和全局样式
├── MainWindow.axaml    # 主窗口
└── Program.cs          # 程序入口点
```

## 🎯 主要功能

### 1. 开发环境管理
- **多语言支持**: 管理 Java、Node.js、Python、PHP、C++ 等开发环境
- **版本管理**: 支持多版本共存和快速切换
- **环境隔离**: 独立的环境配置，避免版本冲突
- **路径管理**: 自动配置环境变量和系统路径

### 2. 系统工具集成
- **快速访问**: 一键打开常用Windows系统管理工具
- **分类管理**: 按功能分类整理系统工具
  - 系统管理：计算机管理、服务、系统配置、注册表、环境变量
  - 硬件与设备：设备管理器、磁盘管理、声音设置、显示设置、电源选项
  - 网络与安全：防火墙、本地安全策略、网络连接、用户账户控制
  - 性能与监控：任务管理器、资源监视器、事件查看器、系统信息、计划任务

### 3. 应用程序管理
- **本地应用**: 管理和快速启动常用开发工具
- **Web应用**: 收藏和管理常用在线工具
- **快捷操作**: 支持快速启动、在资源管理器中显示等功能

### 4. 服务管理
- **数据库服务**: MySQL、Redis、MongoDB 等数据库服务管理
- **Web服务**: Nginx、Apache 等Web服务器管理
- **消息队列**: RabbitMQ、Kafka 等消息服务管理

## 📋 系统要求

- **操作系统**: Windows 10 或更高版本
- **.NET 版本**: .NET 10.0 SDK
- **IDE**: Visual Studio 2026（推荐）
- **内存**: 最少 512MB RAM
- **磁盘空间**: 最少 100MB 可用空间

## 🛠️ 构建和运行

### 从源码构建
```bash
# 克隆仓库
git clone https://github.com/programbox/programbox.git

# 进入项目目录
cd ProgramBox

# 还原依赖
dotnet restore

# 构建项目
dotnet build

# 运行项目
dotnet run
```

### 发布应用
```bash
# 发布为单文件可执行程序
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## ⚙️ 配置

### 配置文件位置
- **数据文件**: `data.json` (与可执行文件同目录)
- **配置文件**: `config.json` (与可执行文件同目录)

### 配置选项
```json
{
  "Config": {
    "TrayIcon": true,
    "SelfStarting": false,
    "BaseEnvPath": "D:\\ProgramBox\\env",
    "Language": "zh-CN",
    "DarkMode": false
  }
}
```

## 🎨 界面特性

- **现代化设计**: 采用 Material Design 风格
- **响应式布局**: 适配不同分辨率屏幕
- **流畅动画**: 提供流畅的用户体验
- **主题支持**: 支持浅色和深色主题

## 🔧 开发说明

### 从 WPF 迁移到 Avalonia

本项目已从 WPF 成功迁移到 Avalonia UI，主要变更包括：

1. **XAML 语法**: 
   - WPF: `xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"`
   - Avalonia: `xmlns="https://github.com/avaloniaui"`

2. **样式系统**:
   - 使用 `Classes` 代替 `Style` 属性
   - 使用 Selector 语法定义样式

3. **事件处理**:
   - `MouseLeftButtonUp` → `PointerPressed`
   - `Click` 事件保持不变

4. **控件差异**:
   - `Page` → `UserControl`
   - `Frame` → `ContentControl`
   - `MessageBox` → 自定义对话框

### 添加新功能

1. 在 `Models` 中定义数据模型
2. 在 `Views` 中创建 AXAML 视图
3. 在 `Utils` 中添加工具类
4. 更新 `JsonDataManager` 以支持数据持久化

## 📄 许可证

本项目采用 [MIT 许可证](LICENSE) 开源。

## 🙏 致谢

感谢以下开源项目的支持：
- [Avalonia UI](https://github.com/AvaloniaUI/Avalonia)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)

## 📞 联系方式

- **GitHub**: https://github.com/programbox/programbox
- **邮箱**: support@programbox.dev

---

**ProgramBox** - 让开发环境管理更简单 ❤️
