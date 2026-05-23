# 版本发布

ProgramBox 通过 [GitHub Releases](https://github.com/pengcunfu/devbox/releases) 分发 Windows 64 位 zip 包，不提供 MSI/安装程序。

## 用户：如何更新

1. 打开 [Releases](https://github.com/pengcunfu/devbox/releases)
2. 下载最新 `ProgramBox-x.y.z-win-x64.zip`
3. 关闭正在运行的 ProgramBox
4. 解压覆盖原目录（保留 `data.json`、`config.json`）或解压到新目录后迁移配置

## 维护者：如何发版

### 1. 更新版本与日志

- 编辑 `VERSION` 与 `ProgramBox.csproj` 中的版本号
- 在 `CHANGELOG.md` 的 `[Unreleased]` 下写好变更，并新增 `## [x.y.z] - 日期` 小节

### 2. 提交并打标签

```bash
git add .
git commit -m "chore: release v1.0.1"
git tag v1.0.1
git push origin master
git push origin v1.0.1
```

推送以 `v` 开头的标签（`v*.*.*`）会触发 [release.yml](https://github.com/pengcunfu/devbox/blob/master/.github/workflows/release.yml)：

- 在 `windows-latest` 上 `dotnet publish`（win-x64，自包含单文件）
- 生成 `ProgramBox-<semver>-win-x64.zip`
- 创建 GitHub Release 并上传附件

### 3. 手动触发（可选）

Actions → **Release** → **Run workflow**，输入标签名如 `v1.0.0`。

::: warning
`workflow_dispatch` 创建的 Release 使用输入的标签名；请与 `CHANGELOG` 版本一致。
:::

## 产物说明

| 文件 | 说明 |
|------|------|
| `ProgramBox-x.y.z-win-x64.zip` | 解压即用，内含 `ProgramBox.exe` 及依赖 |

当前 **不提供** win-x86、MSI、商店包；如有需求请提 [Issue](https://github.com/pengcunfu/devbox/issues)。

## CI

- **CI**：推送/PR 到 `master`/`main`/`develop` 时执行 `dotnet build`
- **Docs**：推送 `docs/**` 时部署 VitePress 到 GitHub Pages
