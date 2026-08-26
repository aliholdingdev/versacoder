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

## 7. ViewModel Detayları

### 7.1 MainViewModel

```csharp
public partial class MainViewModel : ObservableObject
{
    private readonly IAgentRunner _agentRunner;
    private readonly IContextManager _contextManager;
    private readonly ILogger<MainViewModel> _logger;
    
    [ObservableProperty]
    private string _title = "Versa Coder";
    
    [ObservableProperty]
    private string _currentAgent = "Build";
    
    [ObservableProperty]
    private string _currentModel = "GPT-4o";
    
    [ObservableProperty]
    private string _statusMessage = "Ready";
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private ObservableCollection<SessionViewModel> _sessions = new();
    
    [ObservableProperty]
    private SessionViewModel? _currentSession;
    
    public MainViewModel(
        IAgentRunner agentRunner,
        IContextManager contextManager,
        ILogger<MainViewModel> logger)
    {
        _agentRunner = agentRunner;
        _contextManager = contextManager;
        _logger = logger;
    }
    
    [RelayCommand]
    private async Task NewSessionAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Creating new session...";
            
            var session = new SessionViewModel
            {
                Id = Guid.NewGuid(),
                Name = $"Session {Sessions.Count + 1}",
                CreatedAt = DateTime.Now
            };
            
            Sessions.Add(session);
            CurrentSession = session;
            
            StatusMessage = "Session created successfully";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create session");
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task SendPromptAsync(string prompt)
    {
        if (CurrentSession == null || string.IsNullOrWhiteSpace(prompt))
            return;
        
        try
        {
            IsLoading = true;
            StatusMessage = "Sending prompt...";
            
            var userMessage = new MessageViewModel
            {
                Role = "user",
                Content = prompt,
                Timestamp = DateTime.Now
            };
            
            CurrentSession.Messages.Add(userMessage);
            
            var request = new AgentRequest
            {
                Prompt = prompt,
                AgentRole = CurrentAgent,
                ModelName = CurrentModel,
                SessionId = CurrentSession.Id
            };
            
            var response = await _agentRunner.RunAsync(request);
            
            var assistantMessage = new MessageViewModel
            {
                Role = "assistant",
                Content = response.Content,
                Timestamp = DateTime.Now,
                TokenCount = response.TokenCount
            };
            
            CurrentSession.Messages.Add(assistantMessage);
            
            StatusMessage = $"Response received ({response.TokenCount} tokens)";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send prompt");
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private void ChangeAgent(string agentName)
    {
        CurrentAgent = agentName;
        StatusMessage = $"Agent changed to {agentName}";
    }
    
    [RelayCommand]
    private void ChangeModel(string modelName)
    {
        CurrentModel = modelName;
        StatusMessage = $"Model changed to {modelName}";
    }
}
```

### 7.2 SolutionPanelViewModel

```csharp
public partial class SolutionPanelViewModel : ObservableObject
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<SolutionPanelViewModel> _logger;
    
    [ObservableProperty]
    private ObservableCollection<FileNodeViewModel> _fileTree = new();
    
    [ObservableProperty]
    private FileNodeViewModel? _selectedFile;
    
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    public SolutionPanelViewModel(
        IProjectRepository projectRepository,
        ILogger<SolutionPanelViewModel> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }
    
    [RelayCommand]
    private async Task LoadProjectAsync(string projectPath)
    {
        try
        {
            var files = Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories);
            
            FileTree.Clear();
            
            var root = new FileNodeViewModel
            {
                Name = Path.GetFileName(projectPath),
                IsDirectory = true,
                Path = projectPath
            };
            
            foreach (var file in files)
            {
                var node = new FileNodeViewModel
                {
                    Name = Path.GetFileName(file),
                    IsDirectory = false,
                    Path = file,
                    Icon = GetFileIcon(file)
                };
                
                root.Children.Add(node);
            }
            
            FileTree.Add(root);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load project");
        }
    }
    
    [RelayCommand]
    private void OpenFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                var content = File.ReadAllText(filePath);
                // Open in new tab
                StatusMessage = $"Opened: {Path.GetFileName(filePath)}";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open file");
        }
    }
    
    private string GetFileIcon(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();
        return extension switch
        {
            ".cs" => "C#",
            ".xaml" => "XAML",
            ".json" => "JSON",
            ".md" => "Markdown",
            _ => "File"
        };
    }
}
```

### 7.3 ChatPanelViewModel

```csharp
public partial class ChatPanelViewModel : ObservableObject
{
    private readonly IAgentRunner _agentRunner;
    private readonly IContextManager _contextManager;
    private readonly ILogger<ChatPanelViewModel> _logger;
    
    [ObservableProperty]
    private ObservableCollection<MessageViewModel> _messages = new();
    
    [ObservableProperty]
    private string _inputText = string.Empty;
    
    [ObservableProperty]
    private bool _isProcessing;
    
    [ObservableProperty]
    private string _selectedAgent = "General";
    
    [ObservableProperty]
    private string _selectedModel = "GPT-4o";
    
    [ObservableProperty]
    private int _tokenCount;
    
    public ChatPanelViewModel(
        IAgentRunner agentRunner,
        IContextManager contextManager,
        ILogger<ChatPanelViewModel> logger)
    {
        _agentRunner = agentRunner;
        _contextManager = contextManager;
        _logger = logger;
    }
    
    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(InputText))
            return;
        
        try
        {
            IsProcessing = true;
            
            var userMessage = new MessageViewModel
            {
                Role = "user",
                Content = InputText,
                Timestamp = DateTime.Now
            };
            
            Messages.Add(userMessage);
            InputText = string.Empty;
            
            var request = new AgentRequest
            {
                Prompt = userMessage.Content,
                AgentRole = SelectedAgent,
                ModelName = SelectedModel
            };
            
            var response = await _agentRunner.RunAsync(request);
            
            var assistantMessage = new MessageViewModel
            {
                Role = "assistant",
                Content = response.Content,
                Timestamp = DateTime.Now,
                TokenCount = response.TokenCount
            };
            
            Messages.Add(assistantMessage);
            TokenCount += response.TokenCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message");
            
            var errorMessage = new MessageViewModel
            {
                Role = "error",
                Content = $"Error: {ex.Message}",
                Timestamp = DateTime.Now
            };
            
            Messages.Add(errorMessage);
        }
        finally
        {
            IsProcessing = false;
        }
    }
    
    private bool CanSendMessage()
    {
        return !IsProcessing && !string.IsNullOrWhiteSpace(InputText);
    }
    
    [RelayCommand]
    private void ClearChat()
    {
        Messages.Clear();
        TokenCount = 0;
    }
    
    [RelayCommand]
    private void CopyMessage(string content)
    {
        Clipboard.SetText(content);
    }
}
```

---

## 8. View Detayları

### 8.1 MainForm

```csharp
public partial class MainForm : RibbonForm
{
    private readonly MainViewModel _viewModel;
    
    public MainForm(MainViewModel viewModel)
    {
        InitializeComponent();
        
        _viewModel = viewModel;
        DataContext = _viewModel;
        
        InitializeRibbon();
        InitializeDocking();
        InitializeEvents();
    }
    
    private void InitializeRibbon()
    {
        // Ribbon sayfaları
        var filePage = new RibbonPage("File");
        var editPage = new RibbonPage("Edit");
        var viewPage = new RibbonPage("View");
        var aiPage = new RibbonPage("AI");
        var toolsPage = new RibbonPage("Tools");
        var sessionPage = new RibbonPage("Session");
        var helpPage = new RibbonPage("Help");
        
        ribbonControl.Pages.AddRange(new[] 
        { 
            filePage, editPage, viewPage, aiPage, 
            toolsPage, sessionPage, helpPage 
        });
        
        // File sayfası
        var newSessionGroup = new RibbonPageGroup("New");
        var openGroup = new RibbonPageGroup("Open");
        var saveGroup = new RibbonPageGroup("Save");
        
        var newSessionButton = new BarButtonItem("New Session", "newsession");
        newSessionButton.ItemClick += async (s, e) => 
            await _viewModel.NewSessionCommand.ExecuteAsync(null);
        
        newSessionGroup.ItemLinks.Add(newSessionButton);
        filePage.Groups.AddRange(new[] { newSessionGroup, openGroup, saveGroup });
    }
    
    private void InitializeDocking()
    {
        // Sol panel - Solution Explorer
        var solutionPanel = dockManager.AddPanel(DockingStyle.Left);
        solutionPanel.Text = "Solution Explorer";
        solutionPanel.Controls.Add(new SolutionPanelView());
        
        // Sağ panel - AI Chat
        var chatPanel = dockManager.AddPanel(DockingStyle.Right);
        chatPanel.Text = "AI Chat";
        chatPanel.Controls.Add(new ChatPanelView());
        
        // Alt panel - Terminal
        var terminalPanel = dockManager.AddPanel(DockingStyle.Bottom);
        terminalPanel.Text = "Terminal";
        terminalPanel.Controls.Add(new TerminalPanelView());
    }
    
    private void InitializeEvents()
    {
        // Tab değişikliği
        tabbedMdiManager.PageAdded += (s, e) =>
        {
            _viewModel.StatusMessage = $"Opened: {e.Page.Text}";
        };
        
        // Dosya seçimi
        solutionPanelView.FileSelected += (s, e) =>
        {
            OpenFileInTab(e.FilePath);
        };
    }
    
    private void OpenFileInTab(string filePath)
    {
        var existingTab = tabbedMdiManager.Pages
            .FirstOrDefault(p => p.Tag?.ToString() == filePath);
        
        if (existingTab != null)
        {
            tabbedMdiManager.SelectedPage = existingTab;
            return;
        }
        
        var tabPage = new XtraTabPage
        {
            Text = Path.GetFileName(filePath),
            Tag = filePath
        };
        
        var editor = new MemoEdit
        {
            Dock = DockStyle.Fill,
            Text = File.ReadAllText(filePath),
            WordWrap = false,
            Font = new Font("Consolas", 12f)
        };
        
        tabPage.Controls.Add(editor);
        tabbedMdiManager.Pages.Add(tabPage);
        tabbedMdiManager.SelectedPage = tabPage;
    }
}
```

---

## 9. Data Binding

### 9.1 Binding Expressions

```xml
<!-- SolutionPanelView.xaml -->
<dxf:LayoutControl>
    <dxf:LayoutControlItem Text="Search">
        <dxf:LayoutItem>
            <dxe:ButtonEdit>
                <dxe:ButtonEdit.Properties>
                    <dxe:EditSettings 
                        NullText="Search files..."
                        Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}" />
                </dxe:ButtonEdit.Properties>
            </dxe:ButtonEdit>
        </dxf:LayoutItem>
    </dxf:LayoutControlItem>
    
    <dxf:LayoutControlItem Text="Files">
        <dx:TreeList
            ItemsSource="{Binding FileTree}"
            SelectedItem="{Binding SelectedFile}"
            DisplayMember="Name"
            KeyMember="Id"
            ParentMember="ParentId" />
    </dxf:LayoutControlItem>
</dxf:LayoutControl>
```

### 9.2 Command Binding

```xml
<!-- ChatPanelView.xaml -->
<dxf:LayoutControl>
    <dxf:LayoutControlItem Text="Agent">
        <dxe:ComboBoxEdit
            SelectedItem="{Binding SelectedAgent}"
            ItemsSource="{Binding AvailableAgents}" />
    </dxf:LayoutControlItem>
    
    <dxf:LayoutControlItem Text="Model">
        <dxe:ComboBoxEdit
            SelectedItem="{Binding SelectedModel}"
            ItemsSource="{Binding AvailableModels}" />
    </dxf:LayoutControlItem>
    
    <dxf:LayoutControlItem Text="Input">
        <dxe:ButtonEdit
            Text="{Binding InputText, UpdateSourceTrigger=PropertyChanged}"
            IsEnabled="{Binding IsProcessing, Converter={StaticResource InverseBoolConverter}}">
            <dxe:ButtonEdit.Buttons>
                <dxe:ButtonEditButton
                    Content="Send"
                    Command="{Binding SendMessageCommand}"
                    Kind="Glyph" />
            </dxe:ButtonEdit.Buttons>
        </dxe:ButtonEdit>
    </dxf:LayoutControlItem>
    
    <dxf:LayoutControlItem Text="Messages">
        <ListBox
            ItemsSource="{Binding Messages}"
            ItemTemplate="{StaticResource MessageTemplate}" />
    </dxf:LayoutControlItem>
</dxf:LayoutControl>
```

---

## 10. Styling ve Tema

### 10.1 DevExpress Theme Yapılandırması

```csharp
// MainForm'de tema ayarı
public void SetTheme(string themeName)
{
    DefaultLookAndFeel.Default.SetSkinStyle(themeName);
    
    // Tema tercihini kaydet
    Properties.Settings.Default.Theme = themeName;
    Properties.Settings.Default.Save();
}

// Kullanılabilir temalar
private readonly string[] _availableThemes = new[]
{
    "Office 2019 Colorful",
    "Office 2019 Dark",
    "Office 2019 White",
    "The Bezier",
    "Puzzle",
    "Soho Loft",
    "High Contrast"
};
```

### 10.2 Custom Style

```csharp
// Özel stiller
public static class VersaCoderStyles
{
    public static void ApplyDarkTheme()
    {
        DefaultLookAndFeel.Default.SetSkinStyle("Office 2019 Dark");
        
        // Özel renk ayarları
        AppearanceObject.DefaultFont = new Font("Segoe UI", 10f);
        
        // Kod editörü renkleri
        var editorColors = new Dictionary<string, Color>
        {
            ["Default"] = Color.FromArgb(30, 30, 30),
            ["Keyword"] = Color.FromArgb(86, 156, 214),
            ["String"] = Color.FromArgb(209, 154, 102),
            ["Comment"] = Color.FromArgb(87, 166, 74),
            ["Number"] = Color.FromArgb(181, 206, 168),
            ["Type"] = Color.FromArgb(78, 201, 176)
        };
    }
}
```

---

## 11. UI Testleri

### 11.1 ViewModel Testleri

```csharp
public class MainViewModelTests
{
    private readonly Mock<IAgentRunner> _agentRunnerMock;
    private readonly Mock<IContextManager> _contextManagerMock;
    private readonly MainViewModel _viewModel;
    
    public MainViewModelTests()
    {
        _agentRunnerMock = new Mock<IAgentRunner>();
        _contextManagerMock = new Mock<IContextManager>();
        _viewModel = new MainViewModel(
            _agentRunnerMock.Object,
            _contextManagerMock.Object,
            Mock.Of<ILogger<MainViewModel>>());
    }
    
    [Fact]
    public async Task NewSession_ShouldAddSession()
    {
        // Act
        await _viewModel.NewSessionCommand.ExecuteAsync(null);
        
        // Assert
        Assert.Single(_viewModel.Sessions);
        Assert.NotNull(_viewModel.CurrentSession);
    }
    
    [Fact]
    public async Task SendPrompt_ShouldAddMessages()
    {
        // Arrange
        await _viewModel.NewSessionCommand.ExecuteAsync(null);
        
        _agentRunnerMock
            .Setup(r => r.RunAsync(It.IsAny<AgentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AgentResponse
            {
                Content = "Test response",
                TokenCount = 100
            });
        
        // Act
        await _viewModel.SendPromptCommand.ExecuteAsync("Test prompt");
        
        // Assert
        Assert.Equal(2, _viewModel.CurrentSession.Messages.Count);
        Assert.Equal("Test prompt", _viewModel.CurrentSession.Messages[0].Content);
        Assert.Equal("Test response", _viewModel.CurrentSession.Messages[1].Content);
    }
}
```

---

## 12. UI Gelecek Planı

### 12.1 Kısa Vadeli (1-2 hafta)

| Görev | Öncelik |
|-------|---------|
| Temel UI oluştur | Yüksek |
| MVVM binding | Yüksek |
| Ribbon menü | Yüksek |

### 12.2 Orta Vadeli (1-2 ay)

| Görev | Öncelik |
|-------|---------|
| Syntax highlighting | Orta |
| Theme support | Orta |
| Performance optimization | Düşük |

### 12.3 Uzun Vadeli (3-6 ay)

| Görev | Öncelik |
|-------|---------|
| Plugin UI | Düşük |
| Custom controls | Düşük |
| Accessibility | Orta |

---

## 13. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Views | 4 (Main, Solution, Chat, Terminal) |
| ViewModels | 4 |
| DevExpress Components | 15+ |
| MVVM Commands | 10+ |
| Themes | 7 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
