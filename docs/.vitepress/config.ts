import { defineConfig } from 'vitepress'

// GitHub Pages 项目站点：https://<user>.github.io/<repo>/
const repoName = process.env.GITHUB_REPOSITORY?.split('/')[1] ?? 'programbox'

export default defineConfig({
  title: 'ProgramBox',
  description: 'Windows 开发环境管理工具',
  lang: 'zh-CN',
  base: `/${repoName}/`,
  cleanUrls: true,
  lastUpdated: true,
  themeConfig: {
    logo: '/logo.svg',
    nav: [
      { text: '指南', link: '/guide/getting-started' },
      { text: '功能', link: '/guide/features' },
      { text: '下载', link: '/guide/releases' },
      {
        text: 'GitHub',
        link: 'https://github.com/programbox/programbox',
      },
    ],
    sidebar: [
      {
        text: '介绍',
        items: [
          { text: '什么是 ProgramBox', link: '/' },
          { text: '快速开始', link: '/guide/getting-started' },
        ],
      },
      {
        text: '使用',
        items: [
          { text: '功能概览', link: '/guide/features' },
          { text: '配置说明', link: '/guide/configuration' },
        ],
      },
      {
        text: '开发',
        items: [
          { text: '从源码构建', link: '/guide/build' },
          { text: '版本发布', link: '/guide/releases' },
        ],
      },
    ],
    socialLinks: [
      {
        icon: 'github',
        link: 'https://github.com/programbox/programbox',
      },
    ],
    footer: {
      message: 'Released under the MIT License.',
      copyright: 'Copyright © ProgramBox',
    },
    editLink: {
      pattern:
        'https://github.com/programbox/programbox/edit/main/docs/:path',
      text: '在 GitHub 上编辑此页',
    },
  },
})
