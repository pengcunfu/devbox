# 配置说明

配置文件位于 **与 `ProgramBox.exe` 相同的目录**。

## config.json

应用偏好设置示例：

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

| 字段 | 类型 | 说明 |
|------|------|------|
| `TrayIcon` | bool | 启用托盘；关闭窗口时隐藏到托盘后台运行（无边框窗口右上角 ✕ 与 Alt+F4 均生效） |
| `SelfStarting` | bool | 是否开机自启（写入注册表 Run 项） |
| `BaseEnvPath` | string | 开发环境根目录 |
| `Language` | string | 界面语言，如 `zh-CN` |
| `DarkMode` | bool | 深色模式 |

修改后重启应用生效。`SelfStarting` 开启/关闭会同步系统启动项。

## data.json

由程序维护，存储环境条目、应用列表、Web 收藏、服务配置等。**不建议手动编辑**，除非备份后用于迁移或恢复。

迁移到新机器时：

1. 复制整个程序目录（含 `data.json`、`config.json`）
2. 根据新机器路径调整 `BaseEnvPath` 与各条目的可执行文件路径

## 备份建议

定期备份 `data.json` 与 `config.json`；升级大版本前建议先备份再覆盖安装（解压新版本到同目录时保留这两个文件）。
