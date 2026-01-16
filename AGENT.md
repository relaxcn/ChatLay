# ChatLay – AGENT.md (Revised for .NET 10 & Optimized Architecture)

## 1. 项目愿景与第一性原理

**第一性原理 (First Principles):** ChatLay 的本质是 “像素到文本的非侵入式实时转换器”。其存在的核心逻辑只有三步：

* **捕获 (Capture):** 获取屏幕特定区域的原始像素。
* **理解 (Identify):** 将像素转化为结构化文本 (OCR) 并按需翻译。
* **呈现 (Present):** 在不干扰原有渲染链的前提下，将结果投影回用户视野。

**当前状态:** WIP (Work In Progress)。优先打通核心链路，不为极端边界情况进行过度设计。

## 2. 技术栈

* **Runtime:** .NET 10 (Current)
* **Language:** C#
* **UI Framework:** WPF (Classic Desktop)
* **Architecture:** 模块化服务架构 (Service-Based Architecture)

## 3. 核心设计准则 (不可逾越)

### 3.1 绝对安全性 (Anti-Cheat Safety)

* **零注入:** 严禁 DLL 注入、内存读写或 Hook。
* **合法获取:** 仅使用系统级捕获 API (Windows.Graphics.Capture)。
* **外部进程:** 必须保持为独立的 User-mode 进程。

### 3.2 性能平衡

* **按需更新:** 仅在识别区域发生显著变化或定时器触发时执行 OCR。
* **异步管道:** 捕获 -> OCR -> 翻译 -> 渲染 必须是异步流，不得阻塞 UI 或捕获频率。

## 4. 优化后的项目结构

按照最佳工程实践，我们将代码分为 Core (核心逻辑)、Infrastructure (外部实现) 和 App (入口与UI)。

```plaintext
ChatLay/
├── src/
│   ├── ChatLay.App/                # 启动入口与 WPF 资源
│   │   ├── ViewModels/             # 视图模型 (MVVM)
│   │   ├── Views/                  # 窗口定义 (Overlay, Settings)
│   │   └── App.xaml
│   │
│   ├── ChatLay.Core/               # 业务领域逻辑 (纯 C#)
│   │   ├── Interfaces/             # 定义 ICapture, IOcr, ITranslator
│   │   ├── Models/                 # ROI 坐标, 文本条目, 配置模型
│   │   └── Services/               # 核心协调逻辑 (如 CaptureManager)
│   │
│   ├── ChatLay.Infrastructure/     # 具体技术实现
│   │   ├── Capture/                # WinRT Graphics Capture 实现
│   │   ├── Ocr/                    # Windows.Media.Ocr 封装
│   │   └── Translation/            # API 或 本地模型实现
│   │
│   └── ChatLay.Shared/             # 通用工具类、常量
│
├── docs/                           # 协议与设计文档
├── tests/                          # 单元测试 (WIP 阶段可精简)
├── AGENT.md                        # 本文档
└── ChatLay.sln
```

依赖方向（单向，不能乱）

```
ChatLay.App
    ↓
ChatLay.Infrastructure
    ↓
ChatLay.Core
    ↓
ChatLay.Shared
```

永远不要反向引用，否则架构会很快腐化。

能力开放方向（真正的答案）

```
ChatLay.Shared
    ↑
ChatLay.Core   ←【定义规则 / 抽象 / 契约】
    ↑
ChatLay.Infrastructure  ←【提供实现】
    ↑
ChatLay.App   ←【使用 & 组合】
```

能力 / 抽象 / 接口 = 向上开放

## 5. 关键模块实现策略

### 5.1 捕获层 (Capture)

* **策略:** 使用 GraphicsCaptureItem。
* **WIP 优化:** 初期仅实现全屏截图后的局部裁切 (ROI)，后续再优化为直接捕获特定窗口。

### 5.2 识别层 (OCR)

* **策略:** 默认集成 Windows.Media.Ocr。
* **去重:** 每一帧识别结果需与前一帧进行字符串相似度比对，若无明显变化则跳过翻译环节。

### 5.3 翻译层 (Translation)

* **策略:** 采用插件式接口 ITranslator。
* **缓存:** 相同的原文在同一会话内必须缓存结果，减少 API 调用。

### 5.4 叠加层 (Overlay)

* **策略:** WindowStyle="None", AllowsTransparency="True", Topmost="True"。
* **交互:** 使用 SetWindowLong 设置 WS_EX_TRANSPARENT 实现鼠标穿透。

## 6. 开发原则 (给 AI 代理)

* **保持简单 (KISS):** 在 WIP 阶段，如果一个复杂的异步锁可以用一个简单的 bool 标志位解决，优先选择简单方案。
* **显式依赖:** 使用依赖注入 (DI) 在 App.xaml.cs 中组装对象，方便后续替换 IOcr 实现。
* **错误处理:** 优先记录日志 (Log)，而不是弹出复杂的错误对话框，避免中断游戏体验。
* **配置驱动:** ROI 区域和 API Key 应当持久化在本地 JSON 中。

## 7. 非目标 (Non-Goals)

* 不提供任何形式的游戏内交互。
* 不追求 60 FPS 的 OCR 频率（5-10 FPS 足矣）。
* 不尝试适配非 Windows 平台。
