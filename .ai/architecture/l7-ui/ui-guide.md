---
title: "Versa Coder — L7 UI Layer Guide (DevExpress)"
type: architecture
category: layer
layer: L7
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — L7 UI Layer Guide (DevExpress)

**Zorunlu Bağlantılar:** [[architecture/l6-host/host-guide]] · [[brain.md]] · [[CLAUDE.md]] §14

---

## 1. Amaç

UI katmanı, Versa Coder'ın **görsel arayüzünü** tanımlar. DevExpress 2026 Universal WinForms, **Ribbon + Docking + Tabbed MDI** hybrid yapısı kullanılır.

---

## 2. UI Mimarisi

```
┌─────────────────────────────────────────────────────────────┐
│                    MAIN FORM (RibbonForm)                    │
│  ┌───────────────────────────────────────────────────────┐  │
│  │                 RIBBON CONTROL                         │  │
│  │  [File] [Edit] [View] [AI] [Tools] [Session] [Help]  │  │
│  └───────────────────────────────────────────────────────┘  │
│  ┌────────────┬────────────────────────────┬──────────────┐ │
│  │            │                            │              │ │
│  │  SOLUTION  │     TABBED MDI AREA        │  AI CHAT     │ │
│  │  EXPLORER  │  ┌──────┬──────┬──────┐   │  PANEL       │ │
│  │  (TreeList)│  │Tab 1 │Tab 2 │Tab 3 │   │  (MemoEdit) │ │
│  │            │  │      │      │      │   │              │ │
│  │  Accordion │  │      │      │      │   │  Agent: Build│ │
│  │  Control   │  └──────┴──────┴──────┘   │  Model: GPT4o│ │
│  │            │                            │              │ │
│  ├────────────┤                            ├──────────────┤ │
│  │            │                            │              │ │
│  │  FILE      │                            │  TERMINAL    │ │
│  │  TREE      │                            │  OUTPUT      │ │
│  │            │                            │              │ │
│  └────────────┴────────────────────────────┴──────────────┘ │
│  ┌───────────────────────────────────────────────────────┐  │
│  │                   STATUS BAR                          │  │
│  │  Agent: Build | Model: GPT-4o | Tokens: 1,234       │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

---

## 3. DevExpress Bileşenleri

### 3.1 Ana Pencere

| Bileşen | Kullanım |
|---------|----------|
| `RibbonForm` | Ana pencere — Office tarzı ribbon |
| `RibbonControl` | Üst ribbon menü |
| `BarManager` | Bar yönetimi (ribbon ile çakışmaz) |
| `RibbonStatusBar` | Alt durum çubuğu |

### 3.2 Sol Panel

| Bileşen | Kullanım |
|---------|----------|
| `DockManager` | Sürükle-bırak panel yönetimi |
| `DockPanel` | Sol panel container |
| `AccordionControl` | Katlanır menü (Solution Explorer) |
| `TreeList` | Dosya ağacı görünümü |
| `ImageCollection` | Dosya tipi ikonları |

### 3.3 Merkezi Alan

| Bileşen | Kullanım |
|---------|----------|
| `XtraTabbedMdiManager` | Sekmeli MDI yönetimi |
| `XtraTabControl` | Kod editörü sekmeleri |
| `XtraTabPage` | Her dosya ayrı sekme |
| `MemoEdit` | Kod editörü (syntax highlight) |
| `BarAndDockingController` | Tüm bar/docking ayarları |

### 3.4 Sağ Panel

| Bileşen | Kullanım |
|---------|----------|
| `DockPanel` | Sağ panel container |
| `MemoEdit` | AI chat alanı |
| `ButtonEdit` | Prompt giriş alanı |
| `ListBoxControl` | Session listesi |
| `GridControl` | Veri tabloları |

### 3.5 Alt Panel

| Bileşen | Kullanım |
|---------|----------|
| `DockPanel` | Alt panel container |
| `MemoEdit` | Terminal/Output alanı |
| `LabelControl` | Status bilgisi |

### 3.6 Ek Bileşenler

| Bileşen | Kullanım |
|---------|----------|
| `SplashScreenManager` | Başlangıç ekranı |
| `DefaultLookAndFeel` | Tema yönetimi |
| `BarAndDockingController` | Global ayarlar |
| `SkinManager` | DevExpress skin yönetimi |

---

## 4. MVVM Pattern

```
┌─────────────────────────────────────────────────┐
│  View (Form)                                    │
│  ├── MainForm.cs                                │
│  ├── SolutionPanelView.cs                       │
│  ├── ChatPanelView.cs                           │
│  └── TerminalPanelView.cs                       │
├─────────────────────────────────────────────────┤
│  ViewModel                                      │
│  ├── MainViewModel.cs (CommunityToolkit.Mvvm)   │
│  ├── SolutionPanelViewModel.cs                  │
│  ├── ChatPanelViewModel.cs                      │
│  └── TerminalPanelViewModel.cs                  │
├─────────────────────────────────────────────────┤
│  Model (Domain + Application)                   │
│  └── L0-L2 katmanları                           │
└─────────────────────────────────────────────────┘
```

---

## 5. Ribbon Menü Yapısı

```
Ribbon
├── [File]
│   ├── New Session
│   ├── Open Project
│   ├── Save
│   ├── Save As
│   └── Exit
├── [Edit]
│   ├── Undo
│   ├── Redo
│   ├── Cut
│   ├── Copy
│   ├── Paste
│   └── Find/Replace
├── [View]
│   ├── Solution Explorer
│   ├── AI Chat Panel
│   ├── Terminal
│   ├── Status Bar
│   └── Theme
├── [AI]
│   ├── New Chat
│   ├── Send Prompt
│   ├── Agent Selection
│   ├── Model Selection
│   ├── Context Settings
│   └── Provider Settings
├── [Tools]
│   ├── Run Tests
│   ├── Git Operations
│   ├── Build Solution
│   └── Settings
├── [Session]
│   ├── Session List
│   ├── Branch Session
│   ├── Fork Session
│   ├── Merge Sessions
│   └── Session History
└── [Help]
    ├── Documentation
    ├── About
    └── Keyboard Shortcuts
```

---

## 6. Kurallar

| # | Kural | Açıklama |
|---|-------|----------|
| 1 | **No Code-Behind** | MVVM + CommunityToolkit.Mvvm zorunlu |
| 2 | **DevExpress Mandatory** | Tüm UI kontrolleri DevExpress |
| 3 | **BindableBase** | Tüm ViewModel'lar BindableBase'den türetilir |
| 4 | **ICommand** | Tıklama işlemleri ICommand ile |
| 5 | **ObservableProperty** | Data binding için [ObservableProperty] attribute |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
