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

| Bileşen | Kullanım | Durum |
|---------|----------|-------|
| `RibbonForm` | Ana pencere — Office tarzı ribbon | ❌ Stub |
| `RibbonControl` | Üst ribbon menü | ❌ Stub |
| `BarManager` | Bar yönetimi | ❌ Stub |
| `RibbonStatusBar` | Alt durum çubuğu | ❌ Stub |

### 3.2 Sol Panel

| Bileşen | Kullanım | Durum |
|---------|----------|-------|
| `DockManager` | Sürükle-bırak panel yönetimi | ❌ Stub |
| `DockPanel` | Sol panel container | ❌ Stub |
| `AccordionControl` | Katlanır menü | ❌ Stub |
| `TreeList` | Dosya ağacı görünümü | ❌ Stub |
| `ImageCollection` | Dosya tipi ikonları | ❌ Stub |

### 3.3 Merkezi Alan

| Bileşen | Kullanım | Durum |
|---------|----------|-------|
| `XtraTabbedMdiManager` | Sekmeli MDI yönetimi | ❌ Stub |
| `XtraTabControl` | Kod editörü sekmeleri | ❌ Stub |
| `XtraTabPage` | Her dosya ayrı sekme | ❌ Stub |
| `MemoEdit` | Kod editörü | ❌ Stub |

### 3.4 Sağ Panel

| Bileşen | Kullanım | Durum |
|---------|----------|-------|
| `DockPanel` | Sağ panel container | ❌ Stub |
| `MemoEdit` | AI chat alanı | ❌ Stub |
| `ButtonEdit` | Prompt giriş alanı | ❌ Stub |
| `ListBoxControl` | Session listesi | ❌ Stub |
| `GridControl` | Veri tabloları | ❌ Stub |

### 3.5 Alt Panel

| Bileşen | Kullanım | Durum |
|---------|----------|-------|
| `DockPanel` | Alt panel container | ❌ Stub |
| `MemoEdit` | Terminal/Output alanı | ❌ Stub |

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
│  ViewModel (CommunityToolkit.Mvvm)              │
│  ├── MainViewModel.cs                           │
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

## 6. View-ViewModel İlişkisi

| View | ViewModel | Binding |
|------|-----------|---------|
| MainForm | MainViewModel | SessionList, CurrentSession |
| SolutionPanelView | SolutionPanelViewModel | FileTree, SelectedFile |
| ChatPanelView | ChatPanelViewModel | Messages, Prompt |
| TerminalPanelView | TerminalPanelViewModel | Output, Commands |

---

## 7. Kurallar

| # | Kural | Açıklama |
|---|-------|----------|
| 1 | **No Code-Behind** | MVVM + CommunityToolkit.Mvvm zorunlu |
| 2 | **DevExpress Mandatory** | Tüm UI kontrolleri DevExpress |
| 3 | **BindableBase** | Tüm ViewModel'lar BindableBase'den türetilir |
| 4 | **ICommand** | Tıklama işlemleri ICommand ile |
| 5 | **ObservableProperty** | Data binding için [ObservableProperty] attribute |
| 6 | **Temiz Kod** | View'da iş mantığı yasak |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
