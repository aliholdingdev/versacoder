---
title: "Versa Coder — L7 UI Layer Guide (Comprehensive)"
type: architecture
category: layer
layer: L7
date: 2026-08-26
updated: 2026-08-26
status: active
version: 2.0.0
---

# Versa Coder — Kapsamlı UI Geliştirme Rehberi

**Zorunlu Bağlantılar:** [[architecture/l6-host/host-guide]] · [[brain.md]] · [[CLAUDE.md]] §14

---

## 1. Giriş ve Kapsam

Bu rehber, Versa Coder projesinin tüm UI katmanlarını kapsar. DevExpress WinForms ana UI framework olarak kullanılırken, WPF entegrasyonu, MAUI cross-platform desteği ve Blazor Web UI yetenekleri de dahil edilmiştir.

### 1.1 UI Teknoloji Yığını

```
┌─────────────────────────────────────────────────────────┐
│                    UI KATMANLARI                        │
├─────────────────────────────────────────────────────────┤
│  L7.1  DevExpress WinForms (Primary)                   │
│  L7.2  WPF Integration (ElementHost)                   │
│  L7.3  .NET MAUI Cross-Platform                        │
│  L7.4  Blazor Web UI                                   │
│  L7.5  MVVM Architecture (CommunityToolkit)            │
│  L7.6  Responsive Design                               │
│  L7.7  Accessibility (a11y)                            │
│  L7.8  Theming System                                  │
└─────────────────────────────────────────────────────────┘
```

### 1.2 Temel İlkeler

| # | İlke | Açıklama |
|---|------|----------|
| 1 | **MVVM Zorunlu** | Code-behind yasak, CommunityToolkit.Mvvm |
| 2 | **DevExpress Mandatory** | Tüm UI kontrolleri DevExpress |
| 3 | **Accessibility First** | WCAG 2.1 AA uyumlu |
| 4 | **Responsive Design** | DPI-aware, multi-monitor |
| 5 | **Theme Support** | Dark/Light/Custom tema desteği |

---

## 2. DevExpress WinForms (Primary UI)

### 2.1 DXRibbonControl — Ana Menü Sistemi

DXRibbonControl, uygulamanın ana menü ve araç çubuğunu yönetir. Office tarzı ribbon yapısıyla kullanıcı deneyimini optimize eder.

```csharp
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

public class RibbonManager
{
    private readonly RibbonControl _ribbon;
    private readonly BarManager _barManager;
    
    public RibbonManager(RibbonControl ribbon)
    {
        _ribbon = ribbon;
        _barManager = new BarManager();
        InitializeRibbon();
    }
    
    private void InitializeRibbon()
    {
        // Ana sayfalar
        var filePage = new RibbonPage("Dosya");
        var editPage = new RibbonPage("Düzenle");
        var viewPage = new RibbonPage("Görünüm");
        var aiPage = new RibbonPage("Yapay Zeka");
        var toolsPage = new RibbonPage("Araçlar");
        var sessionPage = new RibbonPage("Oturum");
        var helpPage = new RibbonPage("Yardım");
        
        _ribbon.Pages.AddRange(new[] 
        { 
            filePage, editPage, viewPage, aiPage, 
            toolsPage, sessionPage, helpPage 
        });
        
        // File sayfası grupları
        InitializeFilePage(filePage);
        InitializeEditPage(editPage);
        InitializeAIPage(aiPage);
        
        // Quick Access Toolbar
        InitializeQuickAccessToolbar();
        
        // Ribbon ayarları
        _ribbon.ShowPageHeadersMode = ShowPageHeadersMode.Show;
        _ribbon.ToolbarLocation = RibbonToolbarLocation.Top;
        _ribbon.ApplicationButtonDropDownEnabled = true;
    }
    
    private void InitializeFilePage(RibbonPage page)
    {
        // Yeni grubu
        var newGroup = new RibbonPageGroup("Yeni");
        newGroup.ItemLinks.Add(CreateBarButtonItem(
            "Yeni Oturum", "newsession", OnNewSession));
        newGroup.ItemLinks.Add(CreateBarButtonItem(
            "Yeni Proje", "newproject", OnNewProject));
        
        // Aç/Kaydet grubu
        var openSaveGroup = new RibbonPageGroup("Aç/Kaydet");
        openSaveGroup.ItemLinks.Add(CreateBarButtonItem(
            "Aç", "open", OnOpen));
        openSaveGroup.ItemLinks.Add(CreateBarButtonItem(
            "Kaydet", "save", OnSave));
        openSaveGroup.ItemLinks.Add(CreateBarButtonItem(
            "Farklı Kaydet", "saveas", OnSaveAs));
        
        // Çıkış grubu
        var exitGroup = new RibbonPageGroup("Çıkış");
        exitGroup.ItemLinks.Add(CreateBarButtonItem(
            "Çıkış", "exit", OnExit));
        
        page.Groups.AddRange(new[] { newGroup, openSaveGroup, exitGroup });
    }
    
    private void InitializeAIPage(RibbonPage page)
    {
        // AI kontrolleri
        var chatGroup = new RibbonPageGroup("Sohbet");
        chatGroup.ItemLinks.Add(CreateBarButtonItem(
            "Yeni Sohbet", "newchat", OnNewChat));
        chatGroup.ItemLinks.Add(CreateBarButtonItem(
            "Prompt Gönder", "sendprompt", OnSendPrompt));
        
        // Ajan seçimi
        var agentGroup = new RibbonPageGroup("Ajan Seçimi");
        var agentCombo = new BarEditItem();
        agentCombo.Edit = new RepositoryItemComboBox();
        ((RepositoryItemComboBox)agentCombo.Edit).Items.AddRange(
            new[] { "Build", "Plan", "Explore", "General", "Summary", "Title" });
        agentGroup.ItemLinks.Add(agentCombo);
        
        // Model seçimi
        var modelGroup = new RibbonPageGroup("Model Seçimi");
        var modelCombo = new BarEditItem();
        modelCombo.Edit = new RepositoryItemComboBox();
        ((RepositoryItemComboBox)modelCombo.Edit).Items.AddRange(
            new[] { "GPT-4o", "Claude-3.5", "Gemini Pro" });
        modelGroup.ItemLinks.Add(modelCombo);
        
        page.Groups.AddRange(new[] { chatGroup, agentGroup, modelGroup });
    }
    
    private void InitializeQuickAccessToolbar()
    {
        var qat = _ribbon.QuickAccessToolbar;
        qat.ItemLinks.Add(CreateBarButtonItem("Geri", "undo", OnUndo));
        qat.ItemLinks.Add(CreateBarButtonItem("İleri", "redo", OnRedo));
        qat.ItemLinks.Add(CreateBarButtonItem("Kaydet", "save", OnSave));
        qat.ItemLinks.Add(CreateBarButtonItem("Yeni Oturum", "newsession", OnNewSession));
    }
    
    private BarButtonItem CreateBarButtonItem(
        string caption, string glyphName, EventHandler clickHandler)
    {
        var button = new BarButtonItem(caption);
        button.ItemClick += (s, e) => clickHandler?.Invoke(s, e);
        return button;
    }
    
    // Event handler'lar
    private void OnNewSession(object sender, EventArgs e)
    {
        // Yeni oturum mantığı
    }
    
    private void OnNewProject(object sender, EventArgs e)
    {
        // Yeni proje mantığı
    }
    
    private void OnOpen(object sender, EventArgs e)
    {
        // Dosya açma mantığı
    }
    
    private void OnSave(object sender, EventArgs e)
    {
        // Kaydetme mantığı
    }
    
    private void OnSaveAs(object sender, EventArgs e)
    {
        // Farklı kaydetme mantığı
    }
    
    private void OnExit(object sender, EventArgs e)
    {
        // Çıkış mantığı
    }
    
    private void OnNewChat(object sender, EventArgs e)
    {
        // Yeni sohbet mantığı
    }
    
    private void OnSendPrompt(object sender, EventArgs e)
    {
        // Prompt gönderme mantığı
    }
    
    private void OnUndo(object sender, EventArgs e)
    {
        // Geri alma mantığı
    }
    
    private void OnRedo(object sender, EventArgs e)
    {
        // İleri alma mantığı
    }
}
```

### 2.2 DXMdiContainer — MDI Oturum Sekmeleri

DXMdiContainer, Multiple Document Interface yapısıyla oturum sekmelerini yönetir. Her dosya ayrı bir sekmede açılır.

```csharp
using DevExpress.XtraTab;
using DevExpress.XtraMdi;

public class MdiManager
{
    private readonly XtraTabbedMdiManager _mdiManager;
    private readonly XtraTabControl _tabControl;
    
    public MdiManager(XtraTabbedMdiManager mdiManager, XtraTabControl tabControl)
    {
        _mdiManager = mdiManager;
        _tabControl = tabControl;
        InitializeMdiManager();
    }
    
    private void InitializeMdiManager()
    {
        _mdiManager.MdiParent = null; // Tabbed MDI modu
        _mdiManager.AllowDragTabs = true;
        _mdiManager.ShowTabCloseButtons = true;
        _mdiManager.ShowCloseButton = true;
        
        // Olaylar
        _mdiManager.PageAdded += OnPageAdded;
        _mdiManager.PageRemoved += OnPageRemoved;
        _mdiManager.SelectedPageChanged += OnSelectedPageChanged;
    }
    
    public XtraTabPage OpenFile(string filePath, string content)
    {
        // Açık sekmeleri kontrol et
        var existingTab = FindTabByFilePath(filePath);
        if (existingTab != null)
        {
            _tabControl.SelectedTabPage = existingTab;
            return existingTab;
        }
        
        // Yeni sekme oluştur
        var tabPage = new XtraTabPage
        {
            Text = Path.GetFileName(filePath),
            Tag = filePath,
            ToolTip = filePath
        };
        
        // Kod editörü ekle
        var editor = CreateCodeEditor(content);
        tabPage.Controls.Add(editor);
        
        _tabControl.TabPages.Add(tabPage);
        _tabControl.SelectedTabPage = tabPage;
        
        return tabPage;
    }
    
    private MemoEdit CreateCodeEditor(string content)
    {
        var editor = new MemoEdit
        {
            Dock = DockStyle.Fill,
            Text = content,
            WordWrap = false,
            Font = new Font("Cascadia Code", 12f),
            Properties = 
            {
                ScrollBars = ScrollBars.Both,
                AcceptsReturn = true,
                AcceptsTab = true
            }
        };
        
        // Syntax highlighting yapılandırması
        ConfigureSyntaxHighlighting(editor);
        
        return editor;
    }
    
    private void ConfigureSyntaxHighlighting(MemoEdit editor)
    {
        // C# syntax highlighting
        var syntaxColors = new Dictionary<string, Color>
        {
            ["Default"] = Color.FromArgb(212, 212, 212),
            ["Keyword"] = Color.FromArgb(86, 156, 214),
            ["String"] = Color.FromArgb(209, 154, 102),
            ["Comment"] = Color.FromArgb(87, 166, 74),
            ["Number"] = Color.FromArgb(181, 206, 168),
            ["Type"] = Color.FromArgb(78, 201, 176),
            ["Method"] = Color.FromArgb(220, 220, 170)
        };
        
        // Editor syntax ayarları
        editor.Properties.Appearance.TextOptions.WordSpacing = 1;
        editor.Properties.Appearance.TextOptions.LineSpacing = 1.2f;
    }
    
    public void CloseTab(string filePath)
    {
        var tab = FindTabByFilePath(filePath);
        if (tab != null)
        {
            _tabControl.TabPages.Remove(tab);
        }
    }
    
    public void CloseAllTabs()
    {
        _tabControl.TabPages.Clear();
    }
    
    public void CloseOtherTabs(string keepFilePath)
    {
        var tabsToClose = _tabControl.TabPages
            .Where(t => t.Tag?.ToString() != keepFilePath)
            .ToList();
        
        foreach (var tab in tabsToClose)
        {
            _tabControl.TabPages.Remove(tab);
        }
    }
    
    private XtraTabPage? FindTabByFilePath(string filePath)
    {
        return _tabControl.TabPages
            .FirstOrDefault(t => t.Tag?.ToString() == filePath);
    }
    
    private void OnPageAdded(object sender, MDITabPageEventArgs e)
    {
        // Sayfa eklendiğinde yapılacak işlemler
    }
    
    private void OnPageRemoved(object sender, MDITabPageEventArgs e)
    {
        // Sayfa kaldırıldığında yapılacak işlemler
    }
    
    private void OnSelectedPageChanged(object sender, EventArgs e)
    {
        // Seçili sayfa değiştiğinde yapılacak işlemler
    }
}
```

### 2.3 DXGrid — Veri Görüntüleme

DXGrid, veri tablolarını görüntülemek için kullanılır. Sorting, filtering, grouping ve master-detail desteği sağlar.

```csharp
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;

public class GridManager
{
    private readonly GridControl _gridControl;
    private readonly GridView _gridView;
    
    public GridManager(GridControl gridControl)
    {
        _gridControl = gridControl;
        _gridView = (GridView)_gridControl.MainView;
        InitializeGrid();
    }
    
    private void InitializeGrid()
    {
        // Grid ayarları
        _gridView.OptionsBehavior.Editable = false;
        _gridView.OptionsBehavior.ReadOnly = true;
        _gridView.OptionsSelection.MultiSelect = true;
        _gridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
        
        // Sorting
        _gridView.OptionsCustomization.AllowSort = true;
        _gridView.OptionsMenu.EnableColumnMenu = true;
        _gridView.OptionsMenu.EnableFooterMenu = true;
        
        // Filtering
        _gridView.OptionsView.ShowAutoFilterRow = true;
        _gridView.OptionsView.ShowFilterPanel = DefaultBoolean.True;
        
        // Grouping
        _gridView.OptionsBehavior.AllowGroupExpandAnimation = DefaultBoolean.True;
        _gridView.OptionsView.ShowGroupedColumns = true;
        
        // Footer
        _gridView.OptionsView.ShowFooter = true;
        _gridView.Columns["Id"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Count;
        _gridView.Columns["Id"].SummaryItem.DisplayFormat = "Toplam: {0}";
        
        // Master-Detail
        _gridView.MasterRowExpanded += OnMasterRowExpanded;
        _gridView.MasterRowGetChildList += OnMasterRowGetChildList;
    }
    
    public void BindData<T>(IList<T> data)
    {
        _gridControl.DataSource = data;
        
        // Kolon ayarları
        ConfigureColumns();
    }
    
    private void ConfigureColumns()
    {
        foreach (GridColumn column in _gridView.Columns)
        {
            // Varsayılan ayarlar
            column.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            
            // Kolon genişliği
            column.BestFit();
        }
        
        // Özel kolon ayarları
        if (_gridView.Columns["CreatedAt"] != null)
        {
            _gridView.Columns["CreatedAt"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            _gridView.Columns["CreatedAt"].DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
        }
        
        if (_gridView.Columns["Status"] != null)
        {
            _gridView.Columns["Status"].Width = 100;
            _gridView.Columns["Status"].AppearanceCell.BackColor = Color.LightGreen;
        }
    }
    
    public void AddGrouping(string columnName)
    {
        _gridView.GroupedColumnSortInfoCore = 
            new DevExpress.XtraGrid.GridColumnSortInfo[]
            {
                new DevExpress.XtraGrid.GridColumnSortInfo(
                    _gridView.Columns[columnName], 
                    DevExpress.Data.ColumnSortOrder.Ascending)
            };
    }
    
    public void ClearGrouping()
    {
        _gridView.ClearGrouping();
    }
    
    public void ApplyFilter(string filterExpression)
    {
        _gridView.ActiveFilterString = filterExpression;
    }
    
    public void ClearFilter()
    {
        _gridView.ClearColumnsFilter();
    }
    
    public void ExportToExcel(string filePath)
    {
        _gridView.ExportToXlsx(filePath);
    }
    
    public void ExportToPdf(string filePath)
    {
        _gridView.ExportToPdf(filePath);
    }
    
    private void OnMasterRowExpanded(object sender, RowMasterRowExpandedEventArgs e)
    {
        // Master row genişletildiğinde
    }
    
    private void OnMasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
    {
        // Detail row'ları getir
        var masterRow = _gridView.GetRow(e.RowHandle);
        if (masterRow != null)
        {
            // Çocuk verileri yükle
            e.ChildList = GetChildData(masterRow);
        }
    }
    
    private IList GetChildData(object masterRow)
    {
        // Çocuk veri mantığı
        return new List<object>();
    }
}
```

### 2.4 DXTreeList — Dosya Ağacı

DXTreeList, hiyerarşik verileri görüntülemek için kullanılır. Dosya ağacı ve kategori yapıları için idealdir.

```csharp
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

public class TreeListManager
{
    private readonly TreeList _treeList;
    
    public TreeListManager(TreeList treeList)
    {
        _treeList = treeList;
        InitializeTreeList();
    }
    
    private void InitializeTreeList()
    {
        // TreeList ayarları
        _treeList.OptionsBehavior.Editable = false;
        _treeList.OptionsBehavior.ReadOnly = true;
        _treeList.OptionsBehavior.AllowExpandOnDblClick = true;
        
        // Görünüm ayarları
        _treeList.OptionsView.ShowRootLines = true;
        _treeList.OptionsView.ShowLines = true;
        _treeList.OptionsView.ShowHorzLines = false;
        _treeList.OptionsView.ShowIndicator = true;
        
        // Seçim
        _treeList.OptionsSelection.MultiSelect = false;
        _treeList.OptionsSelection.EnableAppearanceFocusedCell = true;
        
        // Olaylar
        _treeList.FocusedNodeChanged += OnFocusedNodeChanged;
        _treeList.NodeChanged += OnNodeChanged;
    }
    
    public void LoadFileTree(string rootPath)
    {
        _treeList.BeginUpdate();
        try
        {
            _treeList.Nodes.Clear();
            LoadDirectoryNodes(rootPath, null);
        }
        finally
        {
            _treeList.EndUpdate();
        }
    }
    
    private void LoadDirectoryNodes(string path, TreeListNode? parentNode)
    {
        var directoryInfo = new DirectoryInfo(path);
        
        // Dizinleri yükle
        foreach (var dir in directoryInfo.GetDirectories())
        {
            var node = _treeList.Nodes.Add(
                dir.Name, 
                dir.FullName, 
                "folder", 
                dir.LastWriteTime.ToString("dd.MM.yyyy HH:mm"));
            
            node.Tag = dir;
            LoadDirectoryNodes(dir.FullName, node);
        }
        
        // Dosyaları yükle
        foreach (var file in directoryInfo.GetFiles())
        {
            var node = _treeList.Nodes.Add(
                file.Name, 
                file.FullName, 
                GetFileIcon(file.Extension), 
                file.LastWriteTime.ToString("dd.MM.yyyy HH:mm"));
            
            node.Tag = file;
        }
    }
    
    private string GetFileIcon(string extension)
    {
        return extension.ToLower() switch
        {
            ".cs" => "csharp",
            ".json" => "json",
            ".md" => "markdown",
            ".xml" => "xml",
            ".config" => "config",
            ".csproj" => "project",
            ".sln" => "solution",
            _ => "file"
        };
    }
    
    public void ExpandAll()
    {
        _treeList.ExpandAll();
    }
    
    public void CollapseAll()
    {
        _treeList.CollapseAll();
    }
    
    public void ExpandToLevel(int level)
    {
        _treeList.ExpandToLevel(level);
    }
    
    public void SearchNodes(string searchText)
    {
        _treeList.BeginUpdate();
        try
        {
            foreach (TreeListNode node in _treeList.Nodes)
            {
                SearchNodeRecursive(node, searchText);
            }
        }
        finally
        {
            _treeList.EndUpdate();
        }
    }
    
    private void SearchNodeRecursive(TreeListNode node, string searchText)
    {
        var nodeName = node.GetDisplayText(0);
        if (nodeName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
        {
            node.Visible = true;
            // Üst düğümleri de görünür yap
            var parent = node.ParentNode;
            while (parent != null)
            {
                parent.Visible = true;
                parent = parent.ParentNode;
            }
        }
        else
        {
            node.Visible = false;
        }
        
        // Çocuk düğümleri ara
        foreach (TreeListNode childNode in node.Nodes)
        {
            SearchNodeRecursive(childNode, searchText);
        }
    }
    
    private void OnFocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
    {
        if (e.Node != null)
        {
            var filePath = e.Node.GetDisplayText(1);
            // Dosya seçildiğinde yapılacak işlemler
        }
    }
    
    private void OnNodeChanged(object sender, NodeChangedEventArgs e)
    {
        // Düğüm değiştiğinde yapılacak işlemler
    }
}
```

### 2.5 DXRichEdit — Kod Editörü

DXRichEdit, gelişmiş kod editörüyetenekleri sağlar. Syntax highlighting, code folding ve IntelliSense desteği sunar.

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

public class CodeEditorManager
{
    private readonly RichEditControl _richEditControl;
    
    public CodeEditorManager(RichEditControl richEditControl)
    {
        _richEditControl = richEditControl;
        InitializeEditor();
    }
    
    private void InitializeEditor()
    {
        // Editör ayarları
        _richEditControl.DocumentAIOptions.Enabled = false;
        _richEditControl.DocumentDiffsOptions.Enabled = false;
        
        // Syntax highlighting
        ConfigureSyntaxHighlighting();
        
        // Code folding
        ConfigureCodeFolding();
        
        // Keyboard shortcuts
        ConfigureKeyboardShortcuts();
    }
    
    private void ConfigureSyntaxHighlighting()
    {
        // C# syntax highlighting
        var csharpHighlighting = new SyntaxHighlightOptions();
        
        // Anahtar kelimeler
        var keywords = new[]
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch",
            "char", "checked", "class", "const", "continue", "decimal", "default",
            "delegate", "do", "double", "else", "enum", "event", "explicit",
            "extern", "false", "finally", "fixed", "float", "for", "foreach",
            "goto", "if", "implicit", "in", "int", "interface", "internal",
            "is", "lock", "long", "namespace", "new", "null", "object",
            "operator", "out", "override", "params", "private", "protected",
            "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
            "sizeof", "stackalloc", "static", "string", "struct", "switch",
            "this", "throw", "true", "try", "typeof", "uint", "ulong",
            "unchecked", "unsafe", "ushort", "using", "virtual", "void",
            "volatile", "while"
        };
        
        // Syntax ayarları
        _richEditControl.Document.DocumentImportSettings.TxtSettings.HighlightSyntax = true;
        _richEditControl.Document.DocumentImportSettings.TxtSettings.AutoDetectEncoding = true;
    }
    
    private void ConfigureCodeFolding()
    {
        // Code folding ayarları
        _richEditControl.Document.DocumentCapabilities.FloatingObjects = DocumentCapability.Enabled;
        _richEditControl.Document.DocumentCapabilities.InlinePictures = DocumentCapability.Enabled;
    }
    
    private void ConfigureKeyboardShortcuts()
    {
        // Keyboard shortcut'lar
        _richEditControl.KeyDown += (sender, e) =>
        {
            // Ctrl+F: Bul
            if (e.Control && e.KeyCode == Keys.F)
            {
                ShowFindDialog();
                e.Handled = true;
            }
            
            // Ctrl+H: Bul ve Değiştir
            if (e.Control && e.KeyCode == Keys.H)
            {
                ShowFindReplaceDialog();
                e.Handled = true;
            }
            
            // Ctrl+G: Satıra Git
            if (e.Control && e.KeyCode == Keys.G)
            {
                ShowGoToLineDialog();
                e.Handled = true;
            }
            
            // Ctrl+Space: IntelliSense
            if (e.Control && e.KeyCode == Keys.Space)
            {
                ShowIntelliSense();
                e.Handled = true;
            }
            
            // Ctrl+/: Yorum Ekle/Kaldır
            if (e.Control && e.KeyCode == Keys.OemQuestion)
            {
                ToggleComment();
                e.Handled = true;
            }
        };
    }
    
    public void LoadCode(string code, string language)
    {
        _richEditControl.Document.Text = code;
        
        // Dil bazlı syntax highlighting
        switch (language.ToLower())
        {
            case "csharp":
                ApplyCSharpHighlighting();
                break;
            case "json":
                ApplyJsonHighlighting();
                break;
            case "xml":
                ApplyXmlHighlighting();
                break;
            case "markdown":
                ApplyMarkdownHighlighting();
                break;
        }
    }
    
    private void ApplyCSharpHighlighting()
    {
        // C# syntax highlighting uygula
        var document = _richEditControl.Document;
        document.BeginUpdate();
        try
        {
            // Anahtar kelimeleri renklendir
            var keywords = new[] { "class", "namespace", "using", "public", "private", "void" };
            foreach (var keyword in keywords)
            {
                var searchRange = document.CreateRange(document.Start, document.End);
                var occurrences = document.FindAllText(searchRange, keyword, true, true);
                foreach (var range in occurrences)
                {
                    document.Colors[range.Start, range.Length] = Color.FromArgb(86, 156, 214);
                }
            }
        }
        finally
        {
            document.EndUpdate();
        }
    }
    
    private void ApplyJsonHighlighting()
    {
        // JSON syntax highlighting uygula
    }
    
    private void ApplyXmlHighlighting()
    {
        // XML syntax highlighting uygula
    }
    
    private void ApplyMarkdownHighlighting()
    {
        // Markdown syntax highlighting uygula
    }
    
    public string GetSelectedText()
    {
        return _richEditControl.Document.GetText(
            _richEditControl.Document.Selection);
    }
    
    public void ReplaceSelectedText(string newText)
    {
        _richEditControl.Document.Replace(
            _richEditControl.Document.Selection, 
            newText);
    }
    
    public void FindAndReplace(string findText, string replaceText, bool matchCase)
    {
        var document = _richEditControl.Document;
        document.BeginUpdate();
        try
        {
            var searchRange = document.CreateRange(document.Start, document.End);
            var occurrences = document.FindAllText(
                searchRange, 
                findText, 
                !matchCase, 
                false);
            
            foreach (var range in occurrences.Reverse())
            {
                document.Replace(range, replaceText);
            }
        }
        finally
        {
            document.EndUpdate();
        }
    }
    
    private void ShowFindDialog()
    {
        // Bul dialog'unu göster
    }
    
    private void ShowFindReplaceDialog()
    {
        // Bul ve Değiştir dialog'unu göster
    }
    
    private void ShowGoToLineDialog()
    {
        // Satıra Git dialog'unu göster
    }
    
    private void ShowIntelliSense()
    {
        // IntelliSense'yi göster
    }
    
    private void ToggleComment()
    {
        // Yorum ekle/kaldır
        var document = _richEditControl.Document;
        document.BeginUpdate();
        try
        {
            var selection = document.Selection;
            var selectedText = document.GetText(selection);
            
            if (selectedText.StartsWith("//"))
            {
                // Yorumu kaldır
                document.Replace(selection, selectedText.Substring(2).TrimStart());
            }
            else
            {
                // Yorum ekle
                document.Replace(selection, "// " + selectedText);
            }
        }
        finally
        {
            document.EndUpdate();
        }
    }
    
    public void Undo()
    {
        _richEditControl.Document.Undo();
    }
    
    public void Redo()
    {
        _richEditControl.Document.Redo();
    }
    
    public void SelectAll()
    {
        _richEditControl.Document.SelectAll();
    }
    
    public void Clear()
    {
        _richEditControl.Document.Text = string.Empty;
    }
}
```

### 2.6 DXChart — Metrik ve Analiz Grafikleri

DXChart, veri görselleştirme için kullanılır. Çizgi grafik, sütun grafik, pasta grafik ve histogram desteği sağlar.

```csharp
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;

public class ChartManager
{
    private readonly ChartControl _chartControl;
    
    public ChartManager(ChartControl chartControl)
    {
        _chartControl = chartControl;
        InitializeChart();
    }
    
    private void InitializeChart()
    {
        // Chart ayarları
        _chartControl.RuntimeSelection = true;
        _chartControl.ToolTipEnabled = true;
        _chartControl.SeriesSelectionMode = SeriesSelectionMode.Point;
    }
    
    public void CreateLineChart(string title, IList<DataPoint> dataPoints)
    {
        _chartControl.Series.Clear();
        
        var series = new Series(title, ViewType.Line);
        series.ArgumentScaleType = ScaleType.DateTime;
        series.ValueScaleType = ScaleType.Numerical;
        
        foreach (var point in dataPoints)
        {
            series.Points.Add(new SeriesPoint(point.Date, point.Value));
        }
        
        // Görünüm ayarları
        var view = (LineSeriesView)series.View;
        view.LineMarkerOptions.Kind = MarkerKind.Circle;
        view.LineMarkerOptions.Size = 8;
        view.LineStyle.DashStyle = DashStyle.Solid;
        
        _chartControl.Series.Add(series);
        
        // Eksen ayarları
        ConfigureDateTimeAxis();
        ConfigureValueAxis();
    }
    
    public void CreateBarChart(string title, IList<DataPoint> dataPoints)
    {
        _chartControl.Series.Clear();
        
        var series = new Series(title, ViewType.Bar);
        series.ArgumentScaleType = ScaleType.Qualitative;
        
        foreach (var point in dataPoints)
        {
            series.Points.Add(new SeriesPoint(point.Label, point.Value));
        }
        
        _chartControl.Series.Add(series);
    }
    
    public void CreatePieChart(string title, IList<DataPoint> dataPoints)
    {
        _chartControl.Series.Clear();
        
        var series = new Series(title, ViewType.Pie);
        series.ArgumentScaleType = ScaleType.Qualitative;
        
        foreach (var point in dataPoints)
        {
            series.Points.Add(new SeriesPoint(point.Label, point.Value));
        }
        
        // Görünüm ayarları
        var view = (PieSeriesView)series.View;
        view.ExplodeMode = PieExplodeMode.UseDefaults;
        
        _chartControl.Series.Add(series);
    }
    
    public void CreateHistogram(string title, IList<double> values, int binCount)
    {
        _chartControl.Series.Clear();
        
        var series = new Series(title, ViewType.Bar);
        series.ArgumentScaleType = ScaleType.Numerical;
        
        // Histogram hesapla
        var min = values.Min();
        var max = values.Max();
        var binWidth = (max - min) / binCount;
        
        for (int i = 0; i < binCount; i++)
        {
            var binStart = min + i * binWidth;
            var binEnd = binStart + binWidth;
            var count = values.Count(v => v >= binStart && v < binEnd);
            
            series.Points.Add(new SeriesPoint(
                $"{binStart:F1}-{binEnd:F1}", 
                count));
        }
        
        _chartControl.Series.Add(series);
    }
    
    public void AddTrendLine(Series series, TrendLineType trendType)
    {
        var trendLine = new TrendLine(trendType);
        trendLine.LineStyle.DashStyle = DashStyle.Dash;
        series.TrendLines.Add(trendLine);
    }
    
    public void AddConstantLine(Axis axis, double value, string title)
    {
        var constantLine = new ConstantLine(title, value);
        constantLine.LineStyle.DashStyle = DashStyle.Dash;
        constantLine.LineStyle.Thickness = 2;
        axis.ConstantLines.Add(constantLine);
    }
    
    public void ExportToImage(string filePath, ImageFormat format)
    {
        _chartControl.ExportToImage(filePath, format);
    }
    
    public void ExportToPdf(string filePath)
    {
        _chartControl.ExportToPdf(filePath);
    }
    
    private void ConfigureDateTimeAxis()
    {
        var diagram = (XYDiagram)_chartControl.Diagram;
        if (diagram != null)
        {
            var xAxis = diagram.AxisX;
            xAxis.DateTimeScaleOptions.MeasureUnit = DateTimeMeasureUnit.Day;
            xAxis.DateTimeScaleOptions.GridAlignment = DateTimeGridAlignment.Day;
            xAxis.Label.TextPattern = "{V:dd.MM}";
        }
    }
    
    private void ConfigureValueAxis()
    {
        var diagram = (XYDiagram)_chartControl.Diagram;
        if (diagram != null)
        {
            var yAxis = diagram.AxisY;
            yAxis.NumericOptions.Format = NumericFormat.General;
            yAxis.NumericOptions.Precision = 0;
        }
    }
}

public class DataPoint
{
    public DateTime Date { get; set; }
    public double Value { get; set; }
    public string Label { get; set; } = string.Empty;
}
```

### 2.7 DXDocking — Panel Yönetimi

DXDocking, sürükle-bırak panel yönetimi sağlar. Kullanıcılar panelleri istedikleri yere taşıyabilir.

```csharp
using DevExpress.XtraBars.Docking;

public class DockingManager
{
    private readonly DockManager _dockManager;
    
    public DockingManager(DockManager dockManager)
    {
        _dockManager = dockManager;
        InitializeDocking();
    }
    
    private void InitializeDocking()
    {
        // DockManager ayarları
        _dockManager.AllowDockFill = true;
        _dockManager.AllowDockLeft = true;
        _dockManager.AllowDockRight = true;
        _dockManager.AllowDockTop = true;
        _dockManager.AllowDockBottom = true;
        _dockManager.AllowDocking = true;
        
        // Olaylar
        _dockManager.DockPanelAdded += OnDockPanelAdded;
        _dockManager.DockPanelRemoved += OnDockPanelRemoved;
        _dockManager.DockPanelActivated += OnDockPanelActivated;
    }
    
    public DockPanel CreatePanel(string text, DockingStyle dockStyle, Control content)
    {
        var panel = new DockPanel
        {
            Text = text,
            Dock = dockStyle,
            Width = 300,
            Height = 400
        };
        
        var controlContainer = new ControlContainer();
        controlContainer.Controls.Add(content);
        panel.Controls.Add(controlContainer);
        
        _dockManager.AddPanel(panel);
        
        return panel;
    }
    
    public DockPanel CreateAutoHidePanel(string text, DockingStyle dockStyle, Control content)
    {
        var panel = CreatePanel(text, dockStyle, content);
        panel.Options.ShowCloseButton = false;
        panel.Options.ShowMaximizeButton = false;
        panel.Options.ShowMinimizeButton = false;
        
        // Auto-hide modu
        panel.Visibility = DockVisibility.AutoHide;
        
        return panel;
    }
    
    public void ShowPanel(DockPanel panel)
    {
        panel.Visibility = DockVisibility.Visible;
        panel.Activate();
    }
    
    public void HidePanel(DockPanel panel)
    {
        panel.Visibility = DockVisibility.Hidden;
    }
    
    public void AutoHidePanel(DockPanel panel)
    {
        panel.Visibility = DockVisibility.AutoHide;
    }
    
    public void FloatPanel(DockPanel panel)
    {
        panel.FloatLocation = new Point(100, 100);
        panel.FloatSize = new Size(300, 400);
        panel.Dock = DockingStyle.Float;
    }
    
    public void DockPanel(DockPanel panel, DockPanel target, DockingStyle dockStyle)
    {
        panel.DockTo(target, dockStyle);
    }
    
    public void ClosePanel(DockPanel panel)
    {
        panel.Close();
    }
    
    public void SaveLayout(string filePath)
    {
        _dockManager.SaveLayoutToXml(filePath);
    }
    
    public void LoadLayout(string filePath)
    {
        if (File.Exists(filePath))
        {
            _dockManager.RestoreLayoutFromXml(filePath);
        }
    }
    
    public void ResetLayout()
    {
        _dockManager.BeginUpdate();
        try
        {
            foreach (DockPanel panel in _dockManager.Panels)
            {
                panel.Visibility = DockVisibility.Visible;
            }
        }
        finally
        {
            _dockManager.EndUpdate();
        }
    }
    
    private void OnDockPanelAdded(object sender, DockPanelEventArgs e)
    {
        // Panel eklendiğinde
    }
    
    private void OnDockPanelRemoved(object sender, DockPanelEventArgs e)
    {
        // Panel kaldırıldığında
    }
    
    private void OnDockPanelActivated(object sender, DockPanelEventArgs e)
    {
        // Panel aktifleştirildiğinde
    }
}
```

### 2.8 DXAlertWindow — Bildirim Sistemi

DXAlertWindow, kullanıcılara bildirimler gösterir. Uyarı, hata, başarı ve bilgi mesajları için kullanılır.

```csharp
using DevExpress.XtraBars.Alerter;

public class AlertManager
{
    private readonly AlertControl _alertControl;
    
    public AlertManager()
    {
        _alertControl = new AlertControl
        {
            AutoFormDelay = 3000,
            AllowHotTrack = true,
            AllowHtmlText = true,
            ShowPinButton = true,
            ShowCloseButton = true,
            ShowExpandButton = true
        };
        
        _alertControl.AlertClick += OnAlertClick;
        _alertControl.ButtonClick += OnButtonClick;
    }
    
    public void ShowSuccess(string title, string message)
    {
        var alertInfo = new AlertInfo(
            title,
            message,
            null, // Icon
            new AlertButton("Tamam", true));
        
        _alertControl.Show(null, alertInfo);
    }
    
    public void ShowWarning(string title, string message)
    {
        var alertInfo = new AlertInfo(
            title,
            message,
            null,
            new AlertButton("Tamam", true));
        
        _alertControl.Show(null, alertInfo);
    }
    
    public void ShowError(string title, string message)
    {
        var alertInfo = new AlertInfo(
            title,
            message,
            null,
            new AlertButton("Tamam", true));
        
        _alertControl.Show(null, alertInfo);
    }
    
    public void ShowInfo(string title, string message)
    {
        var alertInfo = new AlertInfo(
            title,
            message,
            null,
            new AlertButton("Tamam", true));
        
        _alertControl.Show(null, alertInfo);
    }
    
    public void ShowWithActions(string title, string message, 
        Action onConfirm, Action? onCancel = null)
    {
        var confirmButton = new AlertButton("Onayla");
        confirmButton.Style = PredefinedButtonStyle.OK;
        
        var cancelButton = new AlertButton("İptal");
        cancelButton.Style = PredefinedButtonStyle.Cancel;
        
        var alertInfo = new AlertInfo(
            title,
            message,
            null,
            confirmButton,
            cancelButton);
        
        _alertControl.Show(null, alertInfo);
    }
    
    private void OnAlertClick(object sender, AlertClickEventArgs e)
    {
        // Bildirime tıklandığında
    }
    
    private void OnButtonClick(object sender, AlertButtonEventArgs e)
    {
        // Butona tıklandığında
        if (e.Button.Caption == "Onayla")
        {
            // Onaylama mantığı
        }
        else if (e.Button.Caption == "İptal")
        {
            // İptal mantığı
        }
    }
}
```

### 2.9 DXBadge — Durum Göstergeleri

DXBadge, kontrollerin üzerine durum göstergeleri ekler. Sayı, sembol ve metin göstergeleri destekler.

```csharp
using DevExpress.XtraEditors;

public class BadgeManager
{
    private readonly Dictionary<Control, BadgeControl> _badges = new();
    
    public void AddBadge(Control control, string text, Color? color = null)
    {
        if (_badges.ContainsKey(control))
        {
            RemoveBadge(control);
        }
        
        var badge = new BadgeControl
        {
            Text = text,
            Location = new Point(
                control.Width - 10,
                -5),
            BadgeTextColor = color ?? Color.White,
            Badges = { new Badge(text, color ?? Color.Red) }
        };
        
        control.Controls.Add(badge);
        _badges[control] = badge;
    }
    
    public void AddNumberBadge(Control control, int count, Color? color = null)
    {
        var text = count > 99 ? "99+" : count.ToString();
        AddBadge(control, text, color ?? Color.Red);
    }
    
    public void AddIconBadge(Control control, string iconText, Color? color = null)
    {
        AddBadge(control, iconText, color ?? Color.Blue);
    }
    
    public void UpdateBadgeText(Control control, string text)
    {
        if (_badges.TryGetValue(control, out var badge))
        {
            badge.Text = text;
        }
    }
    
    public void UpdateBadgeColor(Control control, Color color)
    {
        if (_badges.TryGetValue(control, out var badge))
        {
            badge.BadgeTextColor = color;
        }
    }
    
    public void RemoveBadge(Control control)
    {
        if (_badges.TryGetValue(control, out var badge))
        {
            control.Controls.Remove(badge);
            _badges.Remove(control);
        }
    }
    
    public void RemoveAllBadges()
    {
        foreach (var kvp in _badges)
        {
            kvp.Key.Controls.Remove(kvp.Value);
        }
        _badges.Clear();
    }
}
```

### 2.10 DXToastNotification — Toast Mesajları

DXToastNotification, kısa süreli bildirimler gösterir. Kullanıcı etkileşimi için idealdir.

```csharp
using DevExpress.XtraBars.ToastNotifications;

public class ToastManager
{
    private readonly ToastNotificationsManager _toastManager;
    
    public ToastManager()
    {
        _toastManager = new ToastNotificationsManager();
        _toastManager.ApplicationId = "VersaCoder";
        _toastManager.RegisterApplication();
    }
    
    public void ShowToast(string title, string message, 
        ToastNotificationsFormLocation formLocation = ToastNotificationsFormLocation.TopRight)
    {
        var toast = _toastManager.CreateToastNotification(
            title,
            message,
            null,
            null);
        
        toast.FormLocation = formLocation;
        toast.Duration = ToastDuration.Short;
        
        _toastManager.Show(toast);
    }
    
    public void ShowSuccessToast(string message)
    {
        ShowToast("Başarılı", message, ToastNotificationsFormLocation.TopRight);
    }
    
    public void ShowErrorToast(string message)
    {
        ShowToast("Hata", message, ToastNotificationsFormLocation.TopRight);
    }
    
    public void ShowWarningToast(string message)
    {
        ShowToast("Uyarı", message, ToastNotificationsFormLocation.TopRight);
    }
    
    public void ShowInfoToast(string message)
    {
        ShowToast("Bilgi", message, ToastNotificationsFormLocation.TopRight);
    }
    
    public void ShowToastWithAction(string title, string message, 
        Action onClick, ToastNotificationsFormLocation formLocation = ToastNotificationsFormLocation.TopRight)
    {
        var toast = _toastManager.CreateToastNotification(
            title,
            message,
            null,
            null);
        
        toast.FormLocation = formLocation;
        toast.Duration = ToastDuration.Short;
        
        toast.Click += (s, e) => onClick?.Invoke();
        
        _toastManager.Show(toast);
    }
    
    public void Dispose()
    {
        _toastManager?.Dispose();
    }
}
```

### 2.11 Tema Yönetimi (Dark/Light/Custom Themes)

DevExpress tema yönetimi ile uygulamanın görünümünü özelleştiririz.

```csharp
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;

public class ThemeManager
{
    private readonly DefaultLookAndFeel _defaultLookAndFeel;
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
    
    public ThemeManager(DefaultLookAndFeel defaultLookAndFeel)
    {
        _defaultLookAndFeel = defaultLookAndFeel;
        LoadSavedTheme();
    }
    
    public string[] AvailableThemes => _availableThemes;
    
    public void SetTheme(string themeName)
    {
        if (_availableThemes.Contains(themeName))
        {
            _defaultLookAndFeel.SetSkinStyle(themeName);
            SaveTheme(themeName);
        }
    }
    
    public void SetDarkMode()
    {
        SetTheme("Office 2019 Dark");
    }
    
    public void SetLightMode()
    {
        SetTheme("Office 2019 White");
    }
    
    public void ToggleDarkLight()
    {
        var currentTheme = GetCurrentTheme();
        if (currentTheme.Contains("Dark"))
        {
            SetLightMode();
        }
        else
        {
            SetDarkMode();
        }
    }
    
    public string GetCurrentTheme()
    {
        return _defaultLookAndFeel.LookAndFeel.SkinName;
    }
    
    public void ApplyCustomTheme(Color primaryColor, Color secondaryColor, Color accentColor)
    {
        // Özel tema uygula
        var skin = SkinManager.Default.GetSkin(DefaultSkinName);
        
        // Renkleri ayarla
        skin.Colors["Window"] = primaryColor;
        skin.Colors["Control"] = secondaryColor;
        skin.Colors["Highlight"] = accentColor;
        
        _defaultLookAndFeel.LookAndFeel.SkinName = DefaultSkinName;
    }
    
    private void SaveTheme(string themeName)
    {
        Properties.Settings.Default.Theme = themeName;
        Properties.Settings.Default.Save();
    }
    
    private void LoadSavedTheme()
    {
        var savedTheme = Properties.Settings.Default.Theme;
        if (!string.IsNullOrEmpty(savedTheme) && _availableThemes.Contains(savedTheme))
        {
            SetTheme(savedTheme);
        }
        else
        {
            SetDarkMode(); // Varsayılan tema
        }
    }
}
```

### 2.12 Skin Özelleştirme

DevExpress skin'lerini özelleştirerek benzersiz görünüm oluşturabiliriz.

```csharp
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Design;

public class SkinCustomizer
{
    public void CreateCustomSkin(string skinName, string baseSkinName)
    {
        // Temel skin'i kopyala
        var baseSkin = SkinManager.Default.GetSkin(baseSkinName);
        var customSkin = new Skin(baseSkin);
        
        // Özel renkler
        customSkin.Name = skinName;
        customSkin.Colors["Window"] = Color.FromArgb(30, 30, 30);
        customSkin.Colors["Control"] = Color.FromArgb(43, 43, 43);
        customSkin.Colors["Text"] = Color.FromArgb(212, 212, 212);
        customSkin.Colors["Highlight"] = Color.FromArgb(0, 122, 204);
        
        // Font ayarları
        customSkin.Properties["FontName"] = "Segoe UI";
        customSkin.Properties["FontSize"] = 10f;
        
        // Özel stiller
        customSkin.Properties["ButtonRoundCorner"] = 4;
        customSkin.Properties["ButtonGradientMode"] = true;
        
        // Skin'i kaydet
        SaveCustomSkin(customSkin);
    }
    
    public void ApplyCustomSkin(string skinName)
    {
        var skin = LoadCustomSkin(skinName);
        if (skin != null)
        {
            SkinManager.Default.SetSkin(skin);
        }
    }
    
    private void SaveCustomSkin(Skin skin)
    {
        var skinPath = GetSkinPath(skin.Name);
        skin.Save(skinPath);
    }
    
    private Skin? LoadCustomSkin(string skinName)
    {
        var skinPath = GetSkinPath(skinName);
        if (File.Exists(skinPath))
        {
            return Skin.Load(skinPath);
        }
        return null;
    }
    
    private string GetSkinPath(string skinName)
    {
        var appDataPath = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appDataPath, "VersaCoder", "Skins", $"{skinName}.skin");
    }
}
```

---

## 3. WPF Entegrasyonu

### 3.1 WPF Host in WinForms (ElementHost)

<<<<<<< HEAD
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
=======
WinForms uygulamasında WPF kontrollerini kullanmak için ElementHost kullanılır.

```csharp
using System.Windows.Forms.Integration;
>>>>>>> c3e202adbf05605c413ce8e18757b121c201aecb

public class WpfHostManager
{
    private readonly ElementHost _elementHost;
    
    public WpfHostManager(ElementHost elementHost)
    {
        _elementHost = elementHost;
        InitializeElementHost();
    }
    
    private void InitializeElementHost()
    {
        _elementHost.Dock = DockStyle.Fill;
        _elementHost.BackColor = Color.Transparent;
        _elementHost.BackColorTransparent = true;
    }
    
    public void LoadWpfControl(System.Windows.Controls.UserControl wpfControl)
    {
        _elementHost.Child = wpfControl;
    }
    
    public void LoadWpfControl(System.Windows.FrameworkElement element)
    {
        _elementHost.Child = element;
    }
    
    public void UnloadWpfControl()
    {
        _elementHost.Child = null;
    }
    
    public T? GetWpfControl<T>() where T : System.Windows.Controls.UserControl
    {
        return _elementHost.Child as T;
    }
}
```
<<<<<<< HEAD
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
=======

### 3.2 MVVM with CommunityToolkit.Mvvm

CommunityToolkit.Mvvm ile WPF MVVM deseni uygulanır.

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class WpfMainViewModel : ObservableObject
{
    private readonly IAgentRunner _agentRunner;
    private readonly ILogger<WpfMainViewModel> _logger;
    
    [ObservableProperty]
    private string _title = "Versa Coder WPF";
    
    [ObservableProperty]
    private string _inputText = string.Empty;
    
    [ObservableProperty]
    private ObservableCollection<MessageViewModel> _messages = new();
    
    [ObservableProperty]
    private bool _isProcessing;
    
    public WpfMainViewModel(
        IAgentRunner agentRunner,
        ILogger<WpfMainViewModel> logger)
    {
        _agentRunner = agentRunner;
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
            
            var response = await _agentRunner.RunAsync(new AgentRequest
            {
                Prompt = userMessage.Content
            });
            
            var assistantMessage = new MessageViewModel
            {
                Role = "assistant",
                Content = response.Content,
                Timestamp = DateTime.Now
            };
            
            Messages.Add(assistantMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mesaj gönderme hatası");
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
    private void ClearMessages()
    {
        Messages.Clear();
    }
}
```

### 3.3 Data Binding Patterns

WPF'de veri bağlama desenleri.

```xml
<!-- MainWindow.xaml -->
<Window x:Class="VersaCoder.Wpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="{Binding Title}" Height="600" Width="800">
    
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        
        <!-- Üst Panel -->
        <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="10">
            <TextBox Text="{Binding InputText, UpdateSourceTrigger=PropertyChanged}"
                     Width="400" Margin="5"/>
            <Button Content="Gönder" 
                    Command="{Binding SendMessageCommand}"
                    Margin="5"/>
            <Button Content="Temizle" 
                    Command="{Binding ClearMessagesCommand}"
                    Margin="5"/>
        </StackPanel>
        
        <!-- Mesaj Listesi -->
        <ListBox Grid.Row="1" 
                 ItemsSource="{Binding Messages}"
                 Margin="10">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <Border Background="{Binding Role, Converter={StaticResource RoleToColorConverter}}"
                            CornerRadius="5" Padding="10" Margin="5">
                        <StackPanel>
                            <TextBlock Text="{Binding Role}" FontWeight="Bold"/>
                            <TextBlock Text="{Binding Content}" TextWrapping="Wrap"/>
                            <TextBlock Text="{Binding Timestamp}" FontSize="10" Foreground="Gray"/>
                        </StackPanel>
                    </Border>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
        
        <!-- Alt Panel -->
        <StatusBar Grid.Row="2">
            <StatusBarItem>
                <TextBlock Text="{Binding Messages.Count, StringFormat='Mesaj Sayısı: {0}'}"/>
            </StatusBarItem>
        </StatusBar>
    </Grid>
</Window>
```

### 3.4 RelayCommand / ObservableObject

CommunityToolkit.Mvvm ile komut ve gözlemlenebilir nesne desenleri.

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

// ObservableObject deseni
public partial class ProductViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;
    
    [ObservableProperty]
    private decimal _price;
    
    [ObservableProperty]
    private int _stock;
    
    [ObservableProperty]
    private bool _isAvailable;
    
    // Özellik değişikliği callback
    partial void OnPriceChanged(decimal value)
    {
        IsAvailable = value > 0 && Stock > 0;
    }
    
    partial void OnStockChanged(int value)
    {
        IsAvailable = Price > 0 && value > 0;
    }
}

// RelayCommand deseni
public partial class RelayCommandExampleViewModel : ObservableObject
{
    [RelayCommand]
    private void SimpleCommand()
    {
        // Basit komut
    }
    
    [RelayCommand(CanExecute = nameof(CanExecuteAsync))]
    private async Task AsyncCommandAsync()
    {
        // Asenkron komut
        await Task.Delay(1000);
    }
    
    private bool CanExecuteAsync()
    {
        return !IsProcessing;
    }
    
    [RelayCommand]
    private void ParameterCommand(string parameter)
    {
        // Parametreli komut
    }
}
```

### 3.5 UserControl Creation

WPF UserControl oluşturma.

```csharp
// ChatControl.xaml.cs
using System.Windows.Controls;

namespace VersaCoder.Wpf.Controls;

public partial class ChatControl : UserControl
{
    public ChatControl()
    {
        InitializeComponent();
    }
}

// ChatControl.xaml
<UserControl x:Class="VersaCoder.Wpf.Controls.ChatControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        
        <ListBox Grid.Row="0"
                 ItemsSource="{Binding Messages, RelativeSource={RelativeSource AncestorType=UserControl}}"
                 ScrollViewer.VerticalScrollBarVisibility="Auto">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <Border Margin="5" Padding="10" CornerRadius="5"
                            Background="{Binding Role, Converter={StaticResource RoleToColorConverter}}">
                        <TextBlock Text="{Binding Content}" TextWrapping="Wrap"/>
                    </Border>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
        
        <StackPanel Grid.Row="1" Orientation="Horizontal" Margin="10">
            <TextBox Width="300" Margin="5"
                     Text="{Binding InputText, RelativeSource={RelativeSource AncestorType=UserControl}, 
                            UpdateSourceTrigger=PropertyChanged}"/>
            <Button Content="Gönder" Margin="5"
                    Command="{Binding SendMessageCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"/>
        </StackPanel>
    </Grid>
</UserControl>
```

### 3.6 Resource Dictionaries

WPF kaynak sözlükleri ile stil ve tema yönetimi.

```xml
<!-- Themes/DarkTheme.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <!-- Renkler -->
    <Color x:Key="PrimaryColor">#1E1E1E</Color>
    <Color x:Key="SecondaryColor">#2D2D2D</Color>
    <Color x:Key="AccentColor">#007ACC</Color>
    <Color x:Key="TextColor">#D4D4D4</Color>
    
    <!-- Fırçalar -->
    <SolidColorBrush x:Key="PrimaryBrush" Color="{StaticResource PrimaryColor}"/>
    <SolidColorBrush x:Key="SecondaryBrush" Color="{StaticResource SecondaryColor}"/>
    <SolidColorBrush x:Key="AccentBrush" Color="{StaticResource AccentColor}"/>
    <SolidColorBrush x:Key="TextBrush" Color="{StaticResource TextColor}"/>
    
    <!-- Stiller -->
    <Style x:Key="DarkWindowStyle" TargetType="Window">
        <Setter Property="Background" Value="{StaticResource PrimaryBrush}"/>
        <Setter Property="Foreground" Value="{StaticResource TextBrush}"/>
    </Style>
    
    <Style x:Key="DarkButtonStyle" TargetType="Button">
        <Setter Property="Background" Value="{StaticResource SecondaryBrush}"/>
        <Setter Property="Foreground" Value="{StaticResource TextBrush}"/>
        <Setter Property="BorderBrush" Value="{StaticResource AccentBrush}"/>
        <Setter Property="Padding" Value="10,5"/>
        <Setter Property="Margin" Value="5"/>
    </Style>
    
    <Style x:Key="DarkTextBoxStyle" TargetType="TextBox">
        <Setter Property="Background" Value="{StaticResource SecondaryBrush}"/>
        <Setter Property="Foreground" Value="{StaticResource TextBrush}"/>
        <Setter Property="BorderBrush" Value="{StaticResource AccentBrush}"/>
        <Setter Property="Padding" Value="5"/>
    </Style>
</ResourceDictionary>
```

### 3.7 Styles and Templates

WPF stiller ve şablonlar.

```xml
<!-- Styles/CustomStyles.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <!-- Card Stili -->
    <Style x:Key="CardStyle" TargetType="Border">
        <Setter Property="Background" Value="White"/>
        <Setter Property="BorderBrush" Value="#E0E0E0"/>
        <Setter Property="BorderThickness" Value="1"/>
        <Setter Property="CornerRadius" Value="8"/>
        <Setter Property="Padding" Value="15"/>
        <Setter Property="Margin" Value="10"/>
        <Setter Property="Effect">
            <Setter.Value>
                <DropShadowEffect BlurRadius="10" ShadowDepth="2" Opacity="0.3"/>
            </Setter.Value>
        </Setter>
    </Style>
    
    <!-- Animated Button Template -->
    <ControlTemplate x:Key="AnimatedButtonTemplate" TargetType="Button">
        <Border x:Name="border" 
                Background="{TemplateBinding Background}"
                BorderBrush="{TemplateBinding BorderBrush}"
                BorderThickness="{TemplateBinding BorderThickness}"
                CornerRadius="4">
            <ContentPresenter HorizontalAlignment="Center" 
                              VerticalAlignment="Center"/>
        </Border>
        <ControlTemplate.Triggers>
            <Trigger Property="IsMouseOver" Value="True">
                <Setter TargetName="border" Property="Background" Value="#E0E0E0"/>
            </Trigger>
            <Trigger Property="IsPressed" Value="True">
                <Setter TargetName="border" Property="Background" Value="#C0C0C0"/>
            </Trigger>
        </ControlTemplate.Triggers>
    </ControlTemplate>
    
    <!-- DataTemplate for Message -->
    <DataTemplate x:Key="MessageTemplate" DataType="{x:Type local:MessageViewModel}">
        <Border Style="{StaticResource CardStyle}">
            <StackPanel>
                <TextBlock Text="{Binding Role}" FontWeight="Bold" FontSize="12"/>
                <TextBlock Text="{Binding Content}" TextWrapping="Wrap" Margin="0,5,0,0"/>
                <TextBlock Text="{Binding Timestamp}" FontSize="10" Foreground="Gray" 
                           Margin="0,5,0,0"/>
            </StackPanel>
        </Border>
    </DataTemplate>
</ResourceDictionary>
>>>>>>> c3e202adbf05605c413ce8e18757b121c201aecb
```

---

## 4. MAUI Cross-Platform

### 4.1 .NET MAUI Overview

.NET MAUI ile cross-platform uygulama geliştirme.

```csharp
// MauiProgram.cs
using Microsoft.Extensions.Logging;

namespace VersaCoder.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        
        // Servisleri kaydet
        builder.Services.AddSingleton<IAgentRunner, AgentRunner>();
        builder.Services.AddSingleton<IContextManager, ContextManager>();
        
        // ViewModels
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<ChatViewModel>();
        
        // Views
        builder.Services.AddTransient MainPage>();
        builder.Services.AddTransient<ChatPage>();
        
#if DEBUG
        builder.Logging.AddDebug();
#endif
        
        return builder.Build();
    }
}
```

### 4.2 Blazor Hybrid Approach

MAUI'da Blazor Hybrid yaklaşımı.

```csharp
// MauiProgram.cs - Blazor Hybrid
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
        
        // Blazor Hybrid
        builder.Services.AddMauiBlazorWebView();
        
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        
        return builder.Build();
    }
}

// Components/Pages/Chat.razor
@page "/chat"
@using VersaCoder.Shared.Services
@inject IAgentRunner AgentRunner

<h3>AI Chat</h3>

<div class="chat-container">
    @foreach (var message in Messages)
    {
        <div class="message @message.Role">
            <strong>@message.Role:</strong>
            <p>@message.Content</p>
            <small>@message.Timestamp.ToString("HH:mm")</small>
        </div>
    }
</div>

<div class="input-area">
    <input @bind="InputText" @bind:event="oninput" 
           placeholder="Mesajınızı yazın..." />
    <button @onclick="SendMessage" disabled="@IsProcessing">
        @if (IsProcessing)
        {
            <span>Gönderiliyor...</span>
        }
        else
        {
            <span>Gönder</span>
        }
    </button>
</div>

@code {
    private List<MessageViewModel> Messages { get; set; } = new();
    private string InputText { get; set; } = string.Empty;
    private bool IsProcessing { get; set; }
    
    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(InputText) || IsProcessing)
            return;
        
        IsProcessing = true;
        StateHasChanged();
        
        try
        {
            var userMessage = new MessageViewModel
            {
                Role = "user",
                Content = InputText,
                Timestamp = DateTime.Now
            };
            
            Messages.Add(userMessage);
            InputText = string.Empty;
            
            var response = await AgentRunner.RunAsync(new AgentRequest
            {
                Prompt = userMessage.Content
            });
            
            var assistantMessage = new MessageViewModel
            {
                Role = "assistant",
                Content = response.Content,
                Timestamp = DateTime.Now
            };
            
            Messages.Add(assistantMessage);
        }
        finally
        {
            IsProcessing = false;
            StateHasChanged();
        }
    }
}
```

### 4.3 Shared Code Strategy

Platformlar arası paylaşımlı kod stratejisi.

```csharp
// Shared/Interfaces/IAgentRunner.cs
namespace VersaCoder.Shared.Interfaces;

public interface IAgentRunner
{
    Task<AgentResponse> RunAsync(AgentRequest request, CancellationToken cancellationToken = default);
}

// Shared/Models/AgentRequest.cs
namespace VersaCoder.Shared.Models;

public class AgentRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string AgentRole { get; set; } = "General";
    public string ModelName { get; set; } = "GPT-4o";
    public Guid SessionId { get; set; }
}

// Shared/Models/AgentResponse.cs
namespace VersaCoder.Shared.Models;

public class AgentResponse
{
    public string Content { get; set; } = string.Empty;
    public int TokenCount { get; set; }
    public string AgentRole { get; set; } = string.Empty;
}

// Shared/Models/MessageViewModel.cs
namespace VersaCoder.Shared.Models;

public class MessageViewModel
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int TokenCount { get; set; }
}
```

### 4.4 Platform-Specific Adaptations

Platforma özgü uyarlamalar.

```csharp
// Platforms/Android/MainActivity.cs
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace VersaCoder.Maui;

[Activity(Theme = "@style/Maui.SplashTheme", 
          MainLauncher = true, 
          ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        // Android-specific ayarlar
        Window?.SetStatusBarColor(Android.Graphics.Color.ParseColor("#1E1E1E"));
        Window?.SetNavigationBarColor(Android.Graphics.Color.ParseColor("#1E1E1E"));
    }
}

// Platforms/iOS/AppDelegate.cs
using Foundation;
using UIKit;

namespace VersaCoder.Maui;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    
    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        // iOS-specific ayarlar
        UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(30, 30, 30);
        UINavigationBar.Appearance.TintColor = UIColor.White;
        UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes
        {
            ForegroundColor = UIColor.White
        };
        
        return base.FinishedLaunching(app, options);
    }
}
```

---

<<<<<<< HEAD
## 6. View-ViewModel İlişkisi

| View | ViewModel | Binding |
|------|-----------|---------|
| MainForm | MainViewModel | SessionList, CurrentSession |
| SolutionPanelView | SolutionPanelViewModel | FileTree, SelectedFile |
| ChatPanelView | ChatPanelViewModel | Messages, Prompt |
| TerminalPanelView | TerminalPanelViewModel | Output, Commands |

---

## 7. Kurallar
=======
## 5. Blazor Web UI

### 5.1 Blazor Server vs WASM

Blazor Server ve WebAssembly karşılaştırması.

```csharp
// Program.cs - Blazor Server
var builder = WebApplication.CreateBuilder(args);

// Blazor Server servisleri
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// SignalRHub
builder.Services.AddSignalR();

// Servisler
builder.Services.AddScoped<IAgentRunner, AgentRunner>();
builder.Services.AddScoped<IContextManager, ContextManager>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

// Program.cs - Blazor WASM
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Servisler
builder.Services.AddScoped(sp => 
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IAgentRunner, AgentRunner>();
builder.Services.AddScoped<IContextManager, ContextManager>();

await builder.Build().RunAsync();
```

### 5.2 Component Architecture

Blazor bileşen mimarisi.

```csharp
// Components/Layout/MainLayout.razor
@inherits LayoutComponentBase

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>
    
    <main>
        <div class="top-row px-4">
            <a href="https://docs.microsoft.com/aspnet/" target="_blank">About</a>
        </div>
        
        <article class="content px-4">
            @Body
        </article>
    </main>
</div>

// Components/Pages/Chat.razor
@page "/chat"
@using VersaCoder.Blazor.Services
@inject IAgentRunner AgentRunner
@inject IJSRuntime JS

<h3>AI Chat</h3>

<div class="chat-container" @ref="chatContainer">
    @foreach (var message in Messages)
    {
        <ChatMessage Message="message" />
    }
</div>

<ChatInput OnSendMessage="HandleSendMessage" 
           IsProcessing="IsProcessing" />

@code {
    private List<MessageViewModel> Messages { get; set; } = new();
    private bool IsProcessing { get; set; }
    private ElementReference chatContainer;
    
    private async Task HandleSendMessage(string prompt)
    {
        if (IsProcessing) return;
        
        IsProcessing = true;
        StateHasChanged();
        
        try
        {
            var userMessage = new MessageViewModel
            {
                Role = "user",
                Content = prompt,
                Timestamp = DateTime.Now
            };
            
            Messages.Add(userMessage);
            await ScrollToBottom();
            
            var response = await AgentRunner.RunAsync(new AgentRequest
            {
                Prompt = prompt
            });
            
            var assistantMessage = new MessageViewModel
            {
                Role = "assistant",
                Content = response.Content,
                Timestamp = DateTime.Now
            };
            
            Messages.Add(assistantMessage);
            await ScrollToBottom();
        }
        finally
        {
            IsProcessing = false;
            StateHasChanged();
        }
    }
    
    private async Task ScrollToBottom()
    {
        await JS.InvokeVoidAsync("scrollToBottom", chatContainer);
    }
}

// Components/ChatMessage.razor
<div class="message @Message.Role">
    <div class="message-header">
        <strong>@Message.Role</strong>
        <span class="timestamp">@Message.Timestamp.ToString("HH:mm")</span>
    </div>
    <div class="message-content">
        @Message.Content
    </div>
</div>

@code {
    [Parameter]
    public MessageViewModel Message { get; set; } = default!;
}

// Components/ChatInput.razor
<div class="input-area">
    <input @bind="InputText" @bind:event="oninput"
           @onkeydown="HandleKeyDown"
           placeholder="Mesajınızı yazın..."
           disabled="@IsProcessing" />
    <button @onclick="Send" disabled="@IsProcessing || string.IsNullOrWhiteSpace(InputText)">
        @if (IsProcessing)
        {
            <span class="spinner"></span>
        }
        else
        {
            <span>Gönder</span>
        }
    </button>
</div>

@code {
    [Parameter]
    public EventCallback<string> OnSendMessage { get; set; }
    
    [Parameter]
    public bool IsProcessing { get; set; }
    
    private string InputText { get; set; } = string.Empty;
    
    private async Task Send()
    {
        if (string.IsNullOrWhiteSpace(InputText)) return;
        
        await OnSendMessage.InvokeAsync(InputText);
        InputText = string.Empty;
    }
    
    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !e.ShiftKey)
        {
            await Send();
        }
    }
}
```

### 5.3 State Management

Blazor'da durum yönetimi.

```csharp
// Services/ChatState.cs
namespace VersaCoder.Blazor.Services;

public class ChatState
{
    private readonly List<MessageViewModel> _messages = new();
    
    public event Action? OnChange;
    
    public IReadOnlyList<MessageViewModel> Messages => _messages.AsReadOnly();
    
    public void AddMessage(MessageViewModel message)
    {
        _messages.Add(message);
        NotifyStateChanged();
    }
    
    public void ClearMessages()
    {
        _messages.Clear();
        NotifyStateChanged();
    }
    
    private void NotifyStateChanged() => OnChange?.Invoke();
}

// Components/Pages/ChatWithState.razor
@page "/chat-state"
@implements IDisposable
@using VersaCoder.Blazor.Services
@inject ChatState ChatState

<h3>AI Chat (State Management)</h3>

<div class="chat-container">
    @foreach (var message in ChatState.Messages)
    {
        <ChatMessage Message="message" />
    }
</div>

<ChatInput OnSendMessage="HandleSendMessage" />

@code {
    protected override void OnInitialized()
    {
        ChatState.OnChange += StateHasChanged;
    }
    
    public void Dispose()
    {
        ChatState.OnChange -= StateHasChanged;
    }
    
    private async Task HandleSendMessage(string prompt)
    {
        ChatState.AddMessage(new MessageViewModel
        {
            Role = "user",
            Content = prompt,
            Timestamp = DateTime.Now
        });
        
        // AI yanıtını bekle
        var response = await AgentRunner.RunAsync(new AgentRequest
        {
            Prompt = prompt
        });
        
        ChatState.AddMessage(new MessageViewModel
        {
            Role = "assistant",
            Content = response.Content,
            Timestamp = DateTime.Now
        });
    }
}
```

### 5.4 SignalR Integration for Real-Time

SignalR ile gerçek zamanlı iletişim.

```csharp
// Hubs/ChatHub.cs
using Microsoft.AspNetCore.SignalR;

namespace VersaCoder.Blazor.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
    
    public async Task JoinSession(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        await Clients.Group(sessionId).SendAsync("UserJoined", Context.User?.Identity?.Name);
    }
    
    public async Task LeaveSession(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        await Clients.Group(sessionId).SendAsync("UserLeft", Context.User?.Identity?.Name);
    }
    
    public async Task SendAgentResponse(string sessionId, string response)
    {
        await Clients.Group(sessionId).SendAsync("ReceiveAgentResponse", response);
    }
}

// Services/SignalRChatService.cs
using Microsoft.AspNetCore.SignalR.Client;

namespace VersaCoder.Blazor.Services;

public class SignalRChatService : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    
    public event Action<string, string>? OnMessageReceived;
    public event Action<string>? OnUserJoined;
    public event Action<string>? OnUserLeft;
    
    public async Task StartConnection(string hubUrl)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();
        
        _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            OnMessageReceived?.Invoke(user, message);
        });
        
        _hubConnection.On<string>("UserJoined", (user) =>
        {
            OnUserJoined?.Invoke(user);
        });
        
        _hubConnection.On<string>("UserLeft", (user) =>
        {
            OnUserLeft?.Invoke(user);
        });
        
        await _hubConnection.StartAsync();
    }
    
    public async Task SendMessage(string user, string message)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("SendMessage", user, message);
        }
    }
    
    public async Task JoinSession(string sessionId)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("JoinSession", sessionId);
        }
    }
    
    public async Task LeaveSession(string sessionId)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("LeaveSession", sessionId);
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
```

### 5.5 Authentication & Authorization

Blazor'da kimlik doğrulama ve yetkilendirme.

```csharp
// Program.cs - Auth Configuration
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => 
        policy.RequireRole("User", "Admin"));
});

// Components/Pages/Chat.razor
@page "/chat"
@attribute [Authorize]

<h3>AI Chat</h3>

<AuthorizeView>
    <Authorized>
        <div class="user-info">
            Hoş geldiniz, @context.User.Identity?.Name!
        </div>
        
        <ChatContainer />
    </Authorized>
    <NotAuthorized>
        <div class="login-prompt">
            <p>Bu sayfaya erişmek için giriş yapmanız gerekmektedir.</p>
            <a href="/login">Giriş Yap</a>
        </div>
    </NotAuthorized>
</AuthorizeView>

// Components/Pages/Admin.razor
@page "/admin"
@attribute [Authorize(Policy = "AdminOnly")]

<h3>Admin Panel</h3>

<AuthorizeView Policy="AdminOnly">
    <Authorized>
        <div class="admin-panel">
            <h4>Yönetim Araçları</h4>
            <button @onclick="ShowUsers">Kullanıcıları Yönet</button>
            <button @onclick="ShowSettings">Sistem Ayarları</button>
        </div>
    </Authorized>
    <NotAuthorized>
        <div class="access-denied">
            <h4>Erişim Reddedildi</h4>
            <p>Bu sayfaya erişim yetkiniz bulunmamaktadır.</p>
        </div>
    </NotAuthorized>
</AuthorizeView>

@code {
    private void ShowUsers()
    {
        // Kullanıcı yönetimi
    }
    
    private void ShowSettings()
    {
        // Sistem ayarları
    }
}
```

---

## 6. MVVM Architecture

### 6.1 CommunityToolkit.Mvvm Integration

CommunityToolkit.Mvvm entegrasyonu ve kullanımı.

```csharp
// Installation
// Install-Package CommunityToolkit.Mvvm

// Base Classes
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

// ViewModelBase.cs
namespace VersaCoder.Mvvm;

public partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _statusMessage = string.Empty;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    [ObservableProperty]
    private bool _hasError;
    
    protected void SetLoading(bool loading)
    {
        IsLoading = loading;
    }
    
    protected void SetError(string message)
    {
        ErrorMessage = message;
        HasError = !string.IsNullOrEmpty(message);
    }
    
    protected void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }
    
    protected void SetStatus(string message)
    {
        StatusMessage = message;
    }
}
```

### 6.2 ViewModelBase Class

Gelişmiş ViewModelBase sınıfı.

```csharp
// ViewModelBase.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace VersaCoder.Mvvm;

public abstract partial class ViewModelBase : ObservableObject
{
    protected readonly ILogger Logger;
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _title = string.Empty;
    
    [ObservableProperty]
    private string _statusMessage = string.Empty;
    
    [ObservableProperty]
    private bool _isBusy;
    
    protected ViewModelBase(ILogger logger)
    {
        Logger = logger;
    }
    
    [RelayCommand]
    protected virtual async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            await OnLoadDataAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Veri yükleme hatası");
            StatusMessage = $"Hata: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    protected virtual Task OnLoadDataAsync()
    {
        return Task.CompletedTask;
    }
    
    protected async Task ExecuteAsync(Func<Task> operation, string? successMessage = null)
    {
        if (IsBusy) return;
        
        try
        {
            IsBusy = true;
            await operation();
            
            if (successMessage != null)
            {
                StatusMessage = successMessage;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "İşlem hatası");
            StatusMessage = $"Hata: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
```

### 6.3 INotifyPropertyChanged Pattern

INotifyPropertyChanged deseni.

```csharp
// Manual INotifyPropertyChanged implementation
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VersaCoder.Mvvm;

public class BaseModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Usage
public class ProductModel : BaseModel
{
    private string _name = string.Empty;
    private decimal _price;
    private int _stock;
    
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
    
    public decimal Price
    {
        get => _price;
        set => SetProperty(ref _price, value);
    }
    
    public int Stock
    {
        get => _stock;
        set => SetProperty(ref _stock, value);
    }
}

// With CommunityToolkit.Mvvm
public partial class ProductViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;
    
    [ObservableProperty]
    private decimal _price;
    
    [ObservableProperty]
    private int _stock;
    
    // Property changed callback
    partial void OnPriceChanged(decimal value)
    {
        // Price değiştiğinde yapılacak işlemler
    }
}
```

### 6.4 Command Pattern

Komut deseni.

```csharp
// AsyncRelayCommand with CanExecute
public partial class RelayCommandViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _canExecute;
    
    [RelayCommand(CanExecute = nameof(CanExecute))]
    private async Task ExecuteAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Asenkron işlem
            await Task.Delay(1000);
            StatusMessage = "İşlem tamamlandı";
        });
    }
    
    // Parametreli komut
    [RelayCommand]
    private async Task ExecuteWithParameterAsync(string parameter)
    {
        await ExecuteAsync(async () =>
        {
            // Parametreli işlem
            StatusMessage = $"Parametre: {parameter}";
        });
    }
    
    // Komut koleksiyonu
    public ObservableCollection<IRelayCommand> Commands { get; } = new()
    {
        new AsyncRelayCommand(async () => await Task.CompletedTask),
        new RelayCommand(() => { })
    };
}

// Composite Command
public class CompositeCommand : IRelayCommand
{
    private readonly List<IRelayCommand> _commands = new();
    
    public void AddCommand(IRelayCommand command)
    {
        _commands.Add(command);
        command.CanExecuteChanged += (s, e) => RaiseCanExecuteChanged();
    }
    
    public bool CanExecute(object? parameter)
    {
        return _commands.All(c => c.CanExecute(parameter));
    }
    
    public void Execute(object? parameter)
    {
        foreach (var command in _commands)
        {
            if (command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }
    }
    
    public event EventHandler? CanExecuteChanged;
    
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
```

### 6.5 Navigation Service

Navigasyon servisi.

```csharp
// Services/INavigationService.cs
namespace VersaCoder.Mvvm.Services;

public interface INavigationService
{
    Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase;
    Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase;
    Task GoBackAsync();
    Task GoForwardAsync();
    bool CanGoBack { get; }
    bool CanGoForward { get; }
}

// Services/NavigationService.cs
using Microsoft.Extensions.DependencyInjection;

namespace VersaCoder.Mvvm.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<NavigationEntry> _backStack = new();
    private readonly Stack<NavigationEntry> _forwardStack = new();
    
    private ViewModelBase? _currentViewModel;
    
    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public bool CanGoBack => _backStack.Count > 0;
    public bool CanGoForward => _forwardStack.Count > 0;
    
    public event Action<ViewModelBase>? Navigated;
    
    public async Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase
    {
        await NavigateToAsync<TViewModel>(null);
    }
    
    public async Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase
    {
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        
        if (_currentViewModel != null)
        {
            _backStack.Push(new NavigationEntry
            {
                ViewModel = _currentViewModel,
                Parameter = parameter
            });
            _forwardStack.Clear();
        }
        
        _currentViewModel = viewModel;
        
        if (viewModel is IParameterizedViewModel parameterized && parameter != null)
        {
            parameterized.ApplyParameter(parameter);
        }
        
        await viewModel.LoadDataCommand.ExecuteAsync(null);
        
        Navigated?.Invoke(viewModel);
    }
    
    public async Task GoBackAsync()
    {
        if (!CanGoBack) return;
        
        var entry = _backStack.Pop();
        
        if (_currentViewModel != null)
        {
            _forwardStack.Push(new NavigationEntry
            {
                ViewModel = _currentViewModel
            });
        }
        
        _currentViewModel = entry.ViewModel;
        
        if (_currentViewModel is IParameterizedViewModel parameterized && entry.Parameter != null)
        {
            parameterized.ApplyParameter(entry.Parameter);
        }
        
        await _currentViewModel.LoadDataCommand.ExecuteAsync(null);
        
        Navigated?.Invoke(_currentViewModel);
    }
    
    public async Task GoForwardAsync()
    {
        if (!CanGoForward) return;
        
        var entry = _forwardStack.Pop();
        
        if (_currentViewModel != null)
        {
            _backStack.Push(new NavigationEntry
            {
                ViewModel = _currentViewModel
            });
        }
        
        _currentViewModel = entry.ViewModel;
        
        if (_currentViewModel is IParameterizedViewModel parameterized && entry.Parameter != null)
        {
            parameterized.ApplyParameter(entry.Parameter);
        }
        
        await _currentViewModel.LoadDataCommand.ExecuteAsync(null);
        
        Navigated?.Invoke(_currentViewModel);
    }
}

// Models/NavigationEntry.cs
namespace VersaCoder.Mvvm.Models;

public class NavigationEntry
{
    public ViewModelBase ViewModel { get; set; } = default!;
    public object? Parameter { get; set; }
}

// Interfaces/IParameterizedViewModel.cs
namespace VersaCoder.Mvvm.Interfaces;

public interface IParameterizedViewModel
{
    void ApplyParameter(object parameter);
}
```

### 6.6 Dialog Service

Dialog servisi.

```csharp
// Services/IDialogService.cs
namespace VersaCoder.Mvvm.Services;

public interface IDialogService
{
    Task<bool> ShowConfirmationAsync(string title, string message);
    Task<string?> ShowInputAsync(string title, string message, string defaultValue = "");
    Task ShowMessageAsync(string title, string message);
    Task ShowErrorAsync(string title, string message);
    Task ShowWarningAsync(string title, string message);
    Task ShowInfoAsync(string title, string message);
}

// Services/DialogService.cs
namespace VersaCoder.Mvvm.Services;

public class DialogService : IDialogService
{
    private readonly IWin32Window? _owner;
    
    public DialogService(IWin32Window? owner = null)
    {
        _owner = owner;
    }
    
    public Task<bool> ShowConfirmationAsync(string title, string message)
    {
        var result = MessageBox.Show(
            _owner,
            message,
            title,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        
        return Task.FromResult(result == DialogResult.Yes);
    }
    
    public Task<string?> ShowInputAsync(string title, string message, string defaultValue = "")
    {
        // DevExpress InputBox kullanarak
        var result = DevExpress.XtraEditors.XtraInputBox.Show(
            _owner,
            message,
            title,
            defaultValue);
        
        return Task.FromResult(result);
    }
    
    public Task ShowMessageAsync(string title, string message)
    {
        MessageBox.Show(
            _owner,
            message,
            title,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        
        return Task.CompletedTask;
    }
    
    public Task ShowErrorAsync(string title, string message)
    {
        MessageBox.Show(
            _owner,
            message,
            title,
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
        
        return Task.CompletedTask;
    }
    
    public Task ShowWarningAsync(string title, string message)
    {
        MessageBox.Show(
            _owner,
            message,
            title,
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
        
        return Task.CompletedTask;
    }
    
    public Task ShowInfoAsync(string title, string message)
    {
        MessageBox.Show(
            _owner,
            message,
            title,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        
        return Task.CompletedTask;
    }
}
```

### 6.7 Dependency Injection for ViewModels

ViewModel'lar için bağımlılık enjeksiyonu.

```csharp
// Program.cs - DI Configuration
using Microsoft.Extensions.DependencyInjection;
using VersaCoder.Mvvm.Services;
using VersaCoder.ViewModels;

var services = new ServiceCollection();

// Servisleri kaydet
services.AddSingleton<INavigationService, NavigationService>();
services.AddSingleton<IDialogService, DialogService>();
services.AddSingleton<IAgentRunner, AgentRunner>();
services.AddSingleton<IContextManager, ContextManager>();

// ViewModels
services.AddTransient<MainViewModel>();
services.AddTransient<ChatViewModel>();
services.AddTransient<SolutionViewModel>();
services.AddTransient<TerminalViewModel>();

// Factory pattern
services.AddSingleton<Func<Type, ViewModelBase>>(serviceProvider =>
    type => (ViewModelBase)serviceProvider.GetRequiredService(type));

var serviceProvider = services.BuildServiceProvider();

// ViewModel'de kullanım
public partial class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private readonly IAgentRunner _agentRunner;
    
    public MainViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IAgentRunner agentRunner,
        ILogger<MainViewModel> logger) : base(logger)
    {
        _navigationService = navigationService;
        _dialogService = dialogService;
        _agentRunner = agentRunner;
    }
    
    [RelayCommand]
    private async Task OpenChatAsync()
    {
        await _navigationService.NavigateToAsync<ChatViewModel>();
    }
    
    [RelayCommand]
    private async Task ShowSettingsAsync()
    {
        var result = await _dialogService.ShowConfirmationAsync(
            "Ayarlar",
            "Ayarlar sayfasını açmak istediğinize emin misiniz?");
        
        if (result)
        {
            await _navigationService.NavigateToAsync<SettingsViewModel>();
        }
    }
}

// View'de kullanım (WinForms)
public partial class MainForm : RibbonForm
{
    private readonly MainViewModel _viewModel;
    
    public MainForm(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        
        // Event'leri bağla
        _viewModel.NavigationRequested += OnNavigationRequested;
    }
    
    private void OnNavigationRequested(Type viewModelType)
    {
        // ViewModel türüne göre view aç
        if (viewModelType == typeof(ChatViewModel))
        {
            OpenChatPanel();
        }
    }
}
```

---

## 7. Responsive Design

### 7.1 Layout Managers

Yöneticiler ile responsive tasarım.

```csharp
// ResponsiveLayoutHelper.cs
namespace VersaCoder.UI.Helpers;

public class ResponsiveLayoutHelper
{
    private readonly Panel _container;
    private readonly Dictionary<Control, LayoutRule> _rules = new();
    
    public ResponsiveLayoutHelper(Panel container)
    {
        _container = container;
        _container.Resize += OnContainerResize;
    }
    
    public void AddRule(Control control, LayoutRule rule)
    {
        _rules[control] = rule;
    }
    
    private void OnContainerResize(object? sender, EventArgs e)
    {
        ApplyLayout();
    }
    
    private void ApplyLayout()
    {
        var containerWidth = _container.Width;
        var containerHeight = _container.Height;
        
        foreach (var kvp in _rules)
        {
            var control = kvp.Key;
            var rule = kvp.Value;
            
            // Yatay konum
            control.Left = CalculateX(rule.XAnchor, containerWidth, control.Width);
            
            // Dikey konum
            control.Top = CalculateY(rule.YAnchor, containerHeight, control.Height);
            
            // Boyut
            if (rule.AutoSize)
            {
                control.Width = CalculateWidth(rule.WidthAnchor, containerWidth);
                control.Height = CalculateHeight(rule.HeightAnchor, containerHeight);
            }
        }
    }
    
    private int CalculateX(HorizontalAnchor anchor, int containerWidth, int controlWidth)
    {
        return anchor switch
        {
            HorizontalAnchor.Left => 0,
            HorizontalAnchor.Center => (containerWidth - controlWidth) / 2,
            HorizontalAnchor.Right => containerWidth - controlWidth,
            _ => 0
        };
    }
    
    private int CalculateY(VerticalAnchor anchor, int containerHeight, int controlHeight)
    {
        return anchor switch
        {
            VerticalAnchor.Top => 0,
            VerticalAnchor.Middle => (containerHeight - controlHeight) / 2,
            VerticalAnchor.Bottom => containerHeight - controlHeight,
            _ => 0
        };
    }
    
    private int CalculateWidth(WidthAnchor anchor, int containerWidth)
    {
        return anchor switch
        {
            WidthAnchor.Fixed => 200,
            WidthAnchor.Percentage => containerWidth / 2,
            WidthAnchor.Stretch => containerWidth,
            _ => 200
        };
    }
    
    private int CalculateHeight(HeightAnchor anchor, int containerHeight)
    {
        return anchor switch
        {
            HeightAnchor.Fixed => 100,
            HeightAnchor.Percentage => containerHeight / 2,
            HeightAnchor.Stretch => containerHeight,
            _ => 100
        };
    }
}

public enum HorizontalAnchor { Left, Center, Right }
public enum VerticalAnchor { Top, Middle, Bottom }
public enum WidthAnchor { Fixed, Percentage, Stretch }
public enum HeightAnchor { Fixed, Percentage, Stretch }

public class LayoutRule
{
    public HorizontalAnchor XAnchor { get; set; } = HorizontalAnchor.Left;
    public VerticalAnchor YAnchor { get; set; } = VerticalAnchor.Top;
    public WidthAnchor WidthAnchor { get; set; } = WidthAnchor.Fixed;
    public HeightAnchor HeightAnchor { get; set; } = HeightAnchor.Fixed;
    public bool AutoSize { get; set; } = true;
}
```

### 7.2 DPI Awareness

DPI farkındalık.

```csharp
// Program.cs - DPI Awareness
using System.Windows.Forms;

static class Program
{
    [STAThread]
    static void Main()
    {
        // DPI awareness
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        
        Application.Run(new MainForm());
    }
}

// DpiHelper.cs
namespace VersaCoder.UI.Helpers;

public static class DpiHelper
{
    private static float _dpiScale = 1.0f;
    
    public static float DpiScale
    {
        get
        {
            if (_dpiScale == 1.0f)
            {
                using var graphics = Graphics.FromHwnd(IntPtr.Zero);
                _dpiScale = graphics.DpiX / 96.0f;
            }
            return _dpiScale;
        }
    }
    
    public static int Scale(int value)
    {
        return (int)(value * DpiScale);
    }
    
    public static float Scale(float value)
    {
        return value * DpiScale;
    }
    
    public static Size Scale(Size size)
    {
        return new Size(
            (int)(size.Width * DpiScale),
            (int)(size.Height * DpiScale));
    }
    
    public static Point Scale(Point point)
    {
        return new Point(
            (int)(point.X * DpiScale),
            (int)(point.Y * DpiScale));
    }
    
    public static Rectangle Scale(Rectangle rect)
    {
        return new Rectangle(
            (int)(rect.X * DpiScale),
            (int)(rect.Y * DpiScale),
            (int)(rect.Width * DpiScale),
            (int)(rect.Height * DpiScale));
    }
    
    public static Font ScaleFont(Font font)
    {
        return new Font(font.FontFamily, font.Size * DpiScale, font.Style);
    }
}

// Usage in Form
public partial class MainForm : RibbonForm
{
    public MainForm()
    {
        InitializeComponent();
        ApplyDpiAwareLayout();
    }
    
    private void ApplyDpiAwareLayout()
    {
        // DPI-aware boyutlandırma
        this.MinimumSize = DpiHelper.Scale(new Size(800, 600));
        this.Size = DpiHelper.Scale(new Size(1200, 800));
        
        // Font ölçekleme
        this.Font = DpiHelper.ScaleFont(this.Font);
        
        // Kontroller için DPI-aware ayarlar
        foreach (Control control in this.Controls)
        {
            ApplyDpiScaling(control);
        }
    }
    
    private void ApplyDpiScaling(Control control)
    {
        control.Font = DpiHelper.ScaleFont(control.Font);
        
        if (control is Panel panel)
        {
            panel.Padding = DpiHelper.Scale(panel.Padding);
        }
        
        foreach (Control child in control.Controls)
        {
            ApplyDpiScaling(child);
        }
    }
}
```

### 7.3 Multi-Monitor Support

Çoklu monitör desteği.

```csharp
// MultiMonitorHelper.cs
using System.Windows.Forms;

namespace VersaCoder.UI.Helpers;

public static class MultiMonitorHelper
{
    public static Screen GetPrimaryScreen()
    {
        return Screen.PrimaryScreen ?? Screen.AllScreens[0];
    }
    
    public static Screen[] GetAllScreens()
    {
        return Screen.AllScreens;
    }
    
    public static Screen GetScreenFromPoint(Point point)
    {
        return Screen.FromPoint(point);
    }
    
    public static Screen GetScreenFromControl(Control control)
    {
        return Screen.FromControl(control);
    }
    
    public static Rectangle GetWorkingArea(Screen screen)
    {
        return screen.WorkingArea;
    }
    
    public static Rectangle GetWorkingArea(Control control)
    {
        var screen = Screen.FromControl(control);
        return screen.WorkingArea;
    }
    
    public static bool IsOnPrimaryScreen(Control control)
    {
        var screen = Screen.FromControl(control);
        return screen.Primary;
    }
    
    public static void MoveToScreen(Control control, Screen targetScreen)
    {
        var currentScreen = Screen.FromControl(control);
        
        if (currentScreen != targetScreen)
        {
            // Yeni monitöre taşı
            control.Left = targetScreen.WorkingArea.Left + 
                (control.Left - currentScreen.WorkingArea.Left);
            control.Top = targetScreen.WorkingArea.Top + 
                (control.Top - currentScreen.WorkingArea.Top);
        }
    }
    
    public static void CenterOnScreen(Control control, Screen? screen = null)
    {
        screen ??= Screen.FromControl(control);
        
        control.Left = screen.WorkingArea.Left + 
            (screen.WorkingArea.Width - control.Width) / 2;
        control.Top = screen.WorkingArea.Top + 
            (screen.WorkingArea.Height - control.Height) / 2;
    }
    
    public static void MaximizeOnScreen(Control control, Screen? screen = null)
    {
        screen ??= Screen.FromControl(control);
        
        control.Bounds = screen.WorkingArea;
    }
}

// MultiMonitorForm.cs
public partial class MultiMonitorForm : Form
{
    private Screen _currentScreen;
    
    public MultiMonitorForm()
    {
        InitializeComponent();
        _currentScreen = Screen.FromControl(this);
        
        // Ekran değişikliğini izle
        this.LocationChanged += OnLocationChanged;
    }
    
    private void OnLocationChanged(object? sender, EventArgs e)
    {
        var newScreen = Screen.FromControl(this);
        
        if (newScreen != _currentScreen)
        {
            _currentScreen = newScreen;
            OnScreenChanged(newScreen);
        }
    }
    
    protected virtual void OnScreenChanged(Screen newScreen)
    {
        // Ekran değiştiğinde yapılacak işlemler
        // Örneğin: DPI ayarlarını güncelle
        ApplyDpiForScreen(newScreen);
    }
    
    private void ApplyDpiForScreen(Screen screen)
    {
        // Ekran bazlı DPI ayarları
        var dpi = screen.Bounds.Width / screen.WorkingArea.Width;
        // DPI ayarlarını uygula
    }
}
```

### 7.4 Panel Docking Strategies

Panel sabitleme stratejileri.

```csharp
// PanelDockingStrategy.cs
using DevExpress.XtraBars.Docking;

namespace VersaCoder.UI.Strategies;

public class PanelDockingStrategy
{
    private readonly DockManager _dockManager;
    
    public PanelDockingStrategy(DockManager dockManager)
    {
        _dockManager = dockManager;
    }
    
    public void ApplyDefaultLayout()
    {
        // Varsayılan düzeni uygula
        ResetLayout();
        
        // Sol panel - Solution Explorer
        var solutionPanel = CreatePanel("Solution Explorer", DockingStyle.Left);
        solutionPanel.Width = 300;
        
        // Sağ panel - AI Chat
        var chatPanel = CreatePanel("AI Chat", DockingStyle.Right);
        chatPanel.Width = 350;
        
        // Alt panel - Terminal
        var terminalPanel = CreatePanel("Terminal", DockingStyle.Bottom);
        terminalPanel.Height = 200;
    }
    
    public void ApplyCompactLayout()
    {
        // Kompakt düzen
        ResetLayout();
        
        // Tüm panelleri auto-hide yap
        foreach (DockPanel panel in _dockManager.Panels)
        {
            panel.Visibility = DockVisibility.AutoHide;
        }
    }
    
    public void ApplyWideLayout()
    {
        // Geniş düzen
        ResetLayout();
        
        // Sol panel
        var solutionPanel = CreatePanel("Solution Explorer", DockingStyle.Left);
        solutionPanel.Width = 400;
        
        // Sağ panel
        var chatPanel = CreatePanel("AI Chat", DockingStyle.Right);
        chatPanel.Width = 500;
    }
    
    public void ApplyCodingLayout()
    {
        // Kodlama düzeni
        ResetLayout();
        
        // Sol panel - Dosya ağacı
        var fileTreePanel = CreatePanel("Dosya Ağacı", DockingStyle.Left);
        fileTreePanel.Width = 250;
        
        // Alt panel - Terminal
        var terminalPanel = CreatePanel("Terminal", DockingStyle.Bottom);
        terminalPanel.Height = 250;
        
        // Sağ panel - gizli
        var chatPanel = CreatePanel("AI Chat", DockingStyle.Right);
        chatPanel.Visibility = DockVisibility.Hidden;
    }
    
    public void ApplyChatLayout()
    {
        // Sohbet düzeni
        ResetLayout();
        
        // Sol panel - gizli
        var solutionPanel = CreatePanel("Solution Explorer", DockingStyle.Left);
        solutionPanel.Visibility = DockVisibility.Hidden;
        
        // Sağ panel - Sohbet
        var chatPanel = CreatePanel("AI Chat", DockingStyle.Right);
        chatPanel.Width = 600;
    }
    
    public void SaveLayout(string layoutName)
    {
        var filePath = GetLayoutPath(layoutName);
        _dockManager.SaveLayoutToXml(filePath);
    }
    
    public void LoadLayout(string layoutName)
    {
        var filePath = GetLayoutPath(layoutName);
        if (File.Exists(filePath))
        {
            _dockManager.RestoreLayoutFromXml(filePath);
        }
    }
    
    public void ResetLayout()
    {
        _dockManager.BeginUpdate();
        try
        {
            foreach (DockPanel panel in _dockManager.Panels)
            {
                panel.Visibility = DockVisibility.Visible;
                panel.Dock = DockingStyle.Left;
            }
        }
        finally
        {
            _dockManager.EndUpdate();
        }
    }
    
    private DockPanel CreatePanel(string text, DockingStyle dockStyle)
    {
        var panel = new DockPanel
        {
            Text = text,
            Dock = dockStyle
        };
        
        _dockManager.AddPanel(panel);
        return panel;
    }
    
    private string GetLayoutPath(string layoutName)
    {
        var appDataPath = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appDataPath, "VersaCoder", "Layouts", $"{layoutName}.xml");
    }
}
```

---

## 8. Accessibility (a11y)

### 8.1 Screen Reader Support

Ekran okuyucu desteği.

```csharp
// AccessibilityHelper.cs
using System.Windows.Forms;

namespace VersaCoder.UI.Accessibility;

public static class AccessibilityHelper
{
    public static void SetAccessibleName(Control control, string name)
    {
        control.AccessibleName = name;
        control.AccessibleRole = AccessibleRole.Default;
    }
    
    public static void SetAccessibleDescription(Control control, string description)
    {
        control.AccessibleDescription = description;
    }
    
    public static void SetAccessibleRole(Control control, AccessibleRole role)
    {
        control.AccessibleRole = role;
    }
    
    public static void Announce(Control control, string message)
    {
        AccessibilityHelper.RaiseLiveRegionAnnouncement(control, message);
    }
    
    public static void RaiseLiveRegionAnnouncement(Control control, string message)
    {
        // Ekran okuyucuya duyuru yap
        if (control.IsHandleCreated)
        {
            var handle = control.Handle;
            // UI Automation kullanarak duyuru
        }
    }
    
    public static void SetKeyboardShortcut(Control control, Keys shortcut)
    {
        control.ShortcutKeys = shortcut;
    }
}

// Usage
public partial class MainForm : RibbonForm
{
    public MainForm()
    {
        InitializeComponent();
        SetupAccessibility();
    }
    
    private void SetupAccessibility()
    {
        // Ana pencere
        AccessibilityHelper.SetAccessibleName(this, "Versa Coder Ana Pencere");
        AccessibilityHelper.SetAccessibleDescription(this, 
            "Versa Coder uygulamasının ana penceresi");
        
        // Ribbon
        AccessibilityHelper.SetAccessibleName(ribbonControl, "Ana Menü");
        AccessibilityHelper.SetAccessibleRole(ribbonControl, AccessibleRole.MenuBar);
        
        // TreeList
        AccessibilityHelper.SetAccessibleName(treeList, "Dosya Ağacı");
        AccessibilityHelper.SetAccessibleRole(treeList, AccessibleRole.TreeView);
        
        // Grid
        AccessibilityHelper.SetAccessibleName(gridControl, "Veri Tablosu");
        AccessibilityHelper.SetAccessibleRole(gridControl, AccessibleRole.Table);
        
        // Chat paneli
        AccessibilityHelper.SetAccessibleName(chatPanel, "AI Sohbet Paneli");
        AccessibilityHelper.SetAccessibleRole(chatPanel, AccessibleRole.Group);
        
        // Butonlar
        AccessibilityHelper.SetAccessibleName(sendButton, "Gönder Butonu");
        AccessibilityHelper.SetAccessibleDescription(sendButton, 
            "Mesajı göndermek için tıklayın");
        AccessibilityHelper.SetKeyboardShortcut(sendButton, Keys.Enter);
    }
}
```

### 8.2 Keyboard Navigation

Klavye navigasyonu.

```csharp
// KeyboardNavigationHelper.cs
using System.Windows.Forms;

namespace VersaCoder.UI.Accessibility;

public class KeyboardNavigationHelper
{
    private readonly Control _root;
    private readonly Dictionary<Keys, Action> _shortcuts = new();
    
    public KeyboardNavigationHelper(Control root)
    {
        _root = root;
        _root.KeyDown += OnKeyDown;
        _root.KeyPreview = true;
    }
    
    public void RegisterShortcut(Keys keys, Action action)
    {
        _shortcuts[keys] = action;
    }
    
    public void RegisterGlobalShortcut(Keys keys, Action action)
    {
        // Global kısayol
        RegisterShortcut(keys, action);
    }
    
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        var key = e.KeyData;
        
        if (_shortcuts.TryGetValue(key, out var action))
        {
            action.Invoke();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }
    
    public void SetupTabOrder(params Control[] controls)
    {
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i].TabIndexes = i;
            controls[i].TabStop = true;
        }
    }
    
    public void EnableArrowKeyNavigation(Panel panel, Control[] controls)
    {
        panel.KeyDown += (s, e) =>
        {
            var currentIndex = Array.IndexOf(controls, panel.FocusedControl);
            
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Left:
                    if (currentIndex > 0)
                    {
                        controls[currentIndex - 1].Focus();
                        e.Handled = true;
                    }
                    break;
                    
                case Keys.Down:
                case Keys.Right:
                    if (currentIndex < controls.Length - 1)
                    {
                        controls[currentIndex + 1].Focus();
                        e.Handled = true;
                    }
                    break;
                    
                case Keys.Home:
                    controls[0].Focus();
                    e.Handled = true;
                    break;
                    
                case Keys.End:
                    controls[^1].Focus();
                    e.Handled = true;
                    break;
            }
        };
    }
}

// Usage
public partial class MainForm : RibbonForm
{
    private KeyboardNavigationHelper? _keyboardHelper;
    
    public MainForm()
    {
        InitializeComponent();
        SetupKeyboardNavigation();
    }
    
    private void SetupKeyboardNavigation()
    {
        _keyboardHelper = new KeyboardNavigationHelper(this);
        
        // Kısayolları kaydet
        _keyboardHelper.RegisterShortcut(
            Keys.Control | Keys.N, 
            () => NewSessionCommand?.Execute(null));
        
        _keyboardHelper.RegisterShortcut(
            Keys.Control | Keys.S, 
            () => SaveCommand?.Execute(null));
        
        _keyboardHelper.RegisterShortcut(
            Keys.Control | Keys.F, 
            () => FindCommand?.Execute(null));
        
        _keyboardHelper.RegisterShortcut(
            Keys.Control | Keys.G, 
            () => GoToLineCommand?.Execute(null));
        
        _keyboardHelper.RegisterShortcut(
            Keys.F5, 
            () => RefreshCommand?.Execute(null));
        
        _keyboardHelper.RegisterShortcut(
            Keys.F1, 
            () => ShowHelpCommand?.Execute(null));
        
        // Tab sırası
        _keyboardHelper.SetupTabOrder(
            solutionPanelView,
            chatPanelView,
            terminalPanelView);
    }
}
```

### 8.3 High Contrast Themes

Yüksek kontrast temaları.

```csharp
// HighContrastTheme.cs
using DevExpress.LookAndFeel;

namespace VersaCoder.UI.Themes;

public class HighContrastTheme
{
    public static void ApplyHighContrastTheme()
    {
        // Yüksek kontrast teması uygula
        DefaultLookAndFeel.Default.SetSkinStyle("High Contrast");
        
        // Özel renkler
        var colors = new HighContrastColors
        {
            Background = Color.Black,
            Foreground = Color.White,
            Accent = Color.Yellow,
            Error = Color.Red,
            Warning = Color.Orange,
            Success = Color.Lime,
            Border = Color.White
        };
        
        ApplyColors(colors);
    }
    
    public static void ApplyHighContrastLight()
    {
        // Yüksek kontrast - açık tema
        DefaultLookAndFeel.Default.SetSkinStyle("High Contrast");
        
        var colors = new HighContrastColors
        {
            Background = Color.White,
            Foreground = Color.Black,
            Accent = Color.Blue,
            Error = Color.DarkRed,
            Warning = Color.DarkOrange,
            Success = Color.DarkGreen,
            Border = Color.Black
        };
        
        ApplyColors(colors);
    }
    
    private static void ApplyColors(HighContrastColors colors)
    {
        // Form renkleri
        AppearanceObject.DefaultBackColor = colors.Background;
        AppearanceObject.DefaultForeColor = colors.Foreground;
        
        // Font
        AppearanceObject.DefaultFont = new Font("Segoe UI", 12f, FontStyle.Bold);
        
        // Butonlar
        DefaultLookAndFeel.Default.LookAndFeel.SetSkinStyle("High Contrast");
    }
    
    public static bool IsHighContrastEnabled()
    {
        return SystemParameters.HighContrast;
    }
}

public class HighContrastColors
{
    public Color Background { get; set; }
    public Color Foreground { get; set; }
    public Color Accent { get; set; }
    public Color Error { get; set; }
    public Color Warning { get; set; }
    public Color Success { get; set; }
    public Color Border { get; set; }
}

// Usage
public partial class MainForm : RibbonForm
{
    public MainForm()
    {
        InitializeComponent();
        
        // Yüksek kontrast kontrolü
        if (HighContrastTheme.IsHighContrastEnabled())
        {
            HighContrastTheme.ApplyHighContrastTheme();
        }
    }
}
```

### 8.4 WCAG Compliance

WCAG uyumluluğu.

```csharp
// WcagHelper.cs
namespace VersaCoder.UI.Accessibility;

public static class WcagHelper
{
    // Renk kontrast oranı hesaplama
    public static double CalculateContrastRatio(Color foreground, Color background)
    {
        var l1 = GetRelativeLuminance(background);
        var l2 = GetRelativeLuminance(foreground);
        
        var lighter = Math.Max(l1, l2);
        var darker = Math.Min(l1, l2);
        
        return (lighter + 0.05) / (darker + 0.05);
    }
    
    private static double GetRelativeLuminance(Color color)
{
        var r = color.R / 255.0;
        var g = color.G / 255.0;
        var b = color.B / 255.0;
        
        r = r <= 0.03928 ? r / 12.92 : Math.Pow((r + 0.055) / 1.055, 2.4);
        g = g <= 0.03928 ? g / 12.92 : Math.Pow((g + 0.055) / 1.055, 2.4);
        b = b <= 0.03928 ? b / 12.92 : Math.Pow((b + 0.055) / 1.055, 2.4);
        
        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }
    
    // WCAG AA standardı (4.5:1)
    public static bool MeetsWcagAA(Color foreground, Color background)
    {
        return CalculateContrastRatio(foreground, background) >= 4.5;
    }
    
    // WCAG AAA standardı (7:1)
    public static bool MeetsWcagAAA(Color foreground, Color background)
    {
        return CalculateContrastRatio(foreground, background) >= 7.0;
    }
    
    // Renk paleti doğrulama
    public static List<string> ValidateColorPalette(Color[] colors)
    {
        var issues = new List<string>();
        
        for (int i = 0; i < colors.Length; i++)
        {
            for (int j = i + 1; j < colors.Length;j++)
            {
                var ratio = CalculateContrastRatio(colors[i], colors[j]);
                if (ratio < 4.5)
                {
                    issues.Add(
                        $"Yetersiz kontrast: Renk {i} ve Renk {j} - Oran: {ratio:F2}:1");
                }
            }
        }
        
        return issues;
    }
    
    // Font boyutu kontrolü
    public static bool IsValidFontSize(float fontSize, bool isLargeText = false)
    {
        // WCAG: Normal metin için 14pt, büyük metin için 18pt
        var minSize = isLargeText ? 18f : 14f;
        return fontSize >= minSize;
    }
    
    // Alternatif metin kontrolü
    public static bool HasAlternativeText(Control control)
    {
        return !string.IsNullOrEmpty(control.AccessibleName);
    }
    
    // Klavye erişilebilirlik kontrolü
    public static bool IsKeyboardAccessible(Control control)
    {
        return control.TabStop && control.Enabled;
    }
}

// Usage in Form
public partial class MainForm : RibbonForm
{
    public MainForm()
    {
        InitializeComponent();
        ValidateWcagCompliance();
    }
    
    private void ValidateWcagCompliance()
    {
        var issues = new List<string>();
        
        // Renk kontrast kontrolü
        var colors = new[]
        {
            Color.FromArgb(30, 30, 30),   // Background
            Color.FromArgb(212, 212, 212), // Text
            Color.FromArgb(0, 122, 204)    // Accent
        };
        
        var colorIssues = WcagHelper.ValidateColorPalette(colors);
        issues.AddRange(colorIssues);
        
        // Kontrol erişilebilirliği
        foreach (Control control in this.Controls)
        {
            if (!WcagHelper.HasAlternativeText(control))
            {
                issues.Add($"Eksik erişilebilirlik adı: {control.Name}");
            }
            
            if (!WcagHelper.IsKeyboardAccessible(control))
            {
                issues.Add($"Klavye erişilemez: {control.Name}");
            }
        }
        
        // Sorunları raporla
        if (issues.Any())
        {
            Console.WriteLine("WCAG Uyumluluk Sorunları:");
            foreach (var issue in issues)
            {
                Console.WriteLine($"  - {issue}");
            }
        }
    }
}
```

---

## 9. Theming System

### 9.1 DevExpress Skin Manager

DevExpress Skin Manager kullanımı.

```csharp
using DevExpress.LookAndFeel;

public class SkinManagerService
{
    private readonly DefaultLookAndFeel _defaultLookAndFeel;
    
    public SkinManagerService(DefaultLookAndFeel defaultLookAndFeel)
    {
        _defaultLookAndFeel = defaultLookAndFeel;
    }
    
    public string[] GetAvailableSkins()
    {
        return SkinManager.Default.Skins
            .Select(s => s.Name)
            .ToArray();
    }
    
    public string GetCurrentSkin()
    {
        return _defaultLookAndFeel.LookAndFeel.SkinName;
    }
    
    public void SetSkin(string skinName)
    {
        if (GetAvailableSkins().Contains(skinName))
        {
            _defaultLookAndFeel.SetSkinStyle(skinName);
            SaveSkinPreference(skinName);
        }
    }
    
    public void SetDefaultSkin()
    {
        SetSkin("Office 2019 Dark");
    }
    
    private void SaveSkinPreference(string skinName)
    {
        Properties.Settings.Default.SkinName = skinName;
        Properties.Settings.Default.Save();
    }
    
    public string LoadSkinPreference()
    {
        return Properties.Settings.Default.SkinName ?? "Office 2019 Dark";
    }
}
```

### 9.2 Custom Theme Creation

Özel tema oluşturma.

```csharp
// CustomThemeManager.cs
using DevExpress.LookAndFeel;

namespace VersaCoder.UI.Themes;

public class CustomThemeManager
{
    public void CreateCustomTheme(string themeName, ThemeColors colors)
    {
        // Özel tema oluştur
        var skin = new Skin(SkinManager.Default.GetSkin("Office 2019 Dark"));
        skin.Name = themeName;
        
        // Renkleri ayarla
        skin.Colors["Window"] = colors.BackgroundColor;
        skin.Colors["Control"] = colors.ControlColor;
        skin.Colors["Text"] = colors.TextColor;
        skin.Colors["Highlight"] = colors.AccentColor;
        skin.Colors["Button"] = colors.ButtonColor;
        skin.Colors["ButtonText"] = colors.ButtonTextColor;
        
        // Özel stiller
        skin.Properties["ButtonRoundCorner"] = 4;
        skin.Properties["FontName"] = "Segoe UI";
        skin.Properties["FontSize"] = 10f;
        
        // Temayı kaydet
        SaveTheme(skin);
    }
    
    public void ApplyCustomTheme(string themeName)
    {
        var skin = LoadTheme(themeName);
        if (skin != null)
        {
            SkinManager.Default.SetSkin(skin);
        }
    }
    
    private void SaveTheme(Skin skin)
    {
        var themePath = GetThemePath(skin.Name);
        skin.Save(themePath);
    }
    
    private Skin? LoadTheme(string themeName)
    {
        var themePath = GetThemePath(themeName);
        if (File.Exists(themePath))
        {
            return Skin.Load(themePath);
        }
        return null;
    }
    
    private string GetThemePath(string themeName)
    {
        var appDataPath = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appDataPath, "VersaCoder", "Themes", $"{themeName}.skin");
    }
}

public class ThemeColors
{
    public Color BackgroundColor { get; set; }
    public Color ControlColor { get; set; }
    public Color TextColor { get; set; }
    public Color AccentColor { get; set; }
    public Color ButtonColor { get; set; }
    public Color ButtonTextColor { get; set; }
}
```

### 9.3 Dark/Light Mode Toggle

Dark/Light mod geçişi.

```csharp
// DarkLightModeManager.cs
namespace VersaCoder.UI.Themes;

public class DarkLightModeManager
{
    private readonly ThemeManager _themeManager;
    
    public DarkLightModeManager(ThemeManager themeManager)
    {
        _themeManager = themeManager;
    }
    
    public bool IsDarkMode { get; private set; }
    
    public event Action<bool>? ModeChanged;
    
    public void ToggleMode()
    {
        if (IsDarkMode)
        {
            SetLightMode();
        }
        else
        {
            SetDarkMode();
        }
    }
    
    public void SetDarkMode()
    {
        IsDarkMode = true;
        _themeManager.SetTheme("Office 2019 Dark");
        OnModeChanged();
    }
    
    public void SetLightMode()
    {
        IsDarkMode = false;
        _themeManager.SetTheme("Office 2019 White");
        OnModeChanged();
    }
    
    public void SetSystemMode()
    {
        // Sistem temasını kontrol et
        var isDarkSystem = IsSystemDarkMode();
        
        if (isDarkSystem)
        {
            SetDarkMode();
        }
        else
        {
            SetLightMode();
        }
    }
    
    private bool IsSystemDarkMode()
    {
        // Windows registry'den sistem temasını oku
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            
            if (key?.GetValue("AppsUseLightTheme") is int value)
            {
                return value == 0;
            }
        }
        catch
        {
            // Varsayılan olarak light mode
        }
        
        return false;
    }
    
    private void OnModeChanged()
    {
        ModeChanged?.Invoke(IsDarkMode);
        SaveModePreference();
    }
    
    private void SaveModePreference()
    {
        Properties.Settings.Default.IsDarkMode = IsDarkMode;
        Properties.Settings.Default.Save();
    }
    
    public void LoadModePreference()
    {
        var isDarkMode = Properties.Settings.Default.IsDarkMode;
        
        if (isDarkMode)
        {
            SetDarkMode();
        }
        else
        {
            SetLightMode();
        }
    }
}
```

### 9.4 User Preference Persistence

Kullanıcı tercihleri kalıcılığı.

```csharp
// UserPreferencesManager.cs
namespace VersaCoder.UI.Preferences;

public class UserPreferencesManager
{
    private readonly string _preferencesPath;
    
    public UserPreferencesManager()
    {
        var appDataPath = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);
        _preferencesPath = Path.Combine(appDataPath, "VersaCoder", "preferences.json");
    }
    
    public UserPreferences LoadPreferences()
    {
        if (File.Exists(_preferencesPath))
        {
            var json = File.ReadAllText(_preferencesPath);
            return JsonSerializer.Deserialize<UserPreferences>(json) ?? new UserPreferences();
        }
        
        return new UserPreferences();
    }
    
    public void SavePreferences(UserPreferences preferences)
    {
        var directory = Path.GetDirectoryName(_preferencesPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }
        
        var json = JsonSerializer.Serialize(preferences, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        File.WriteAllText(_preferencesPath, json);
    }
    
    public void UpdateTheme(string themeName)
    {
        var preferences = LoadPreferences();
        preferences.ThemeName = themeName;
        SavePreferences(preferences);
    }
    
    public void UpdateLayout(string layoutName)
    {
        var preferences = LoadPreferences();
        preferences.LayoutName = layoutName;
        SavePreferences(preferences);
    }
    
    public void UpdateFontSize(float fontSize)
    {
        var preferences = LoadPreferences();
        preferences.FontSize = fontSize;
        SavePreferences(preferences);
    }
    
    public void UpdateLanguage(string language)
    {
        var preferences = LoadPreferences();
        preferences.Language = language;
        SavePreferences(preferences);
    }
}

public class UserPreferences
{
    public string ThemeName { get; set; } = "Office 2019 Dark";
    public string LayoutName { get; set; } = "Default";
    public float FontSize { get; set; } = 10f;
    public string Language { get; set; } = "tr-TR";
    public bool HighContrast { get; set; }
    public bool KeyboardNavigation { get; set; }
    public bool ScreenReaderOptimized { get; set; }
    public Dictionary<string, string> CustomSettings { get; set; } = new();
}
```

---

## 10. UI Testleri

### 10.1 ViewModel Testleri

ViewModel testleri.

```csharp
// Tests/MainViewModelTests.cs
using Moq;
using Xunit;

namespace VersaCoder.Tests.UI;

public class MainViewModelTests
{
    private readonly Mock<IAgentRunner> _agentRunnerMock;
    private readonly Mock<IDialogService> _dialogServiceMock;
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly MainViewModel _viewModel;
    
    public MainViewModelTests()
    {
        _agentRunnerMock = new Mock<IAgentRunner>();
        _dialogServiceMock = new Mock<IDialogService>();
        _navigationServiceMock = new Mock<INavigationService>();
        
        _viewModel = new MainViewModel(
            _navigationServiceMock.Object,
            _dialogServiceMock.Object,
            _agentRunnerMock.Object,
            Mock.Of<ILogger<MainViewModel>>());
    }
    
    [Fact]
    public async Task NewSession_ShouldCreateSession()
    {
        // Arrange
        _dialogServiceMock
            .Setup(d => d.ShowConfirmationAsync(
                It.IsAny<string>(), 
                It.IsAny<string>()))
            .ReturnsAsync(true);
        
        // Act
        await _viewModel.NewSessionCommand.ExecuteAsync(null);
        
        // Assert
        Assert.NotNull(_viewModel.CurrentSession);
        Assert.Single(_viewModel.Sessions);
    }
    
    [Fact]
    public async Task SendMessage_ShouldAddMessages()
    {
        // Arrange
        await _viewModel.NewSessionCommand.ExecuteAsync(null);
        
        _agentRunnerMock
            .Setup(r => r.RunAsync(
                It.IsAny<AgentRequest>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AgentResponse
            {
                Content = "Test yanıtı",
                TokenCount = 100
            });
        
        // Act
        await _viewModel.SendPromptCommand.ExecuteAsync("Test mesajı");
        
        // Assert
        Assert.Equal(2, _viewModel.CurrentSession!.Messages.Count);
        Assert.Equal("Test mesajı", _viewModel.CurrentSession.Messages[0].Content);
        Assert.Equal("Test yanıtı", _viewModel.CurrentSession.Messages[1].Content);
    }
    
    [Fact]
    public async Task ChangeTheme_ShouldUpdateTheme()
    {
        // Arrange
        var themeName = "Office 2019 White";
        
        // Act
        await _viewModel.ChangeThemeCommand.ExecuteAsync(themeName);
        
        // Assert
        Assert.Equal(themeName, _viewModel.CurrentTheme);
    }
    
    [Fact]
    public void CanSendMessage_ShouldReturnFalse_WhenProcessing()
    {
        // Arrange
        _viewModel.IsProcessing = true;
        
        // Act & Assert
        Assert.False(_viewModel.SendPromptCommand.CanExecute(null));
    }
    
    [Fact]
    public void CanSendMessage_ShouldReturnTrue_WhenNotProcessingAndHasInput()
    {
        // Arrange
        _viewModel.IsProcessing = false;
        _viewModel.InputText = "Test";
        
        // Act & Assert
        Assert.True(_viewModel.SendPromptCommand.CanExecute(null));
    }
}
```

### 10.2 View Testleri

View testleri.

```csharp
// Tests/MainFormTests.cs
using Xunit;

namespace VersaCoder.Tests.UI;

public class MainFormTests
{
    [Fact]
    public void MainForm_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var form = new MainForm(Mock.Of<MainViewModel>());
        
        // Assert
        Assert.NotNull(form);
        Assert.Equal("Versa Coder", form.Text);
    }
    
    [Fact]
    public void MainForm_ShouldHaveRibbonControl()
    {
        // Arrange & Act
        var form = new MainForm(Mock.Of<MainViewModel>());
        
        // Assert
        Assert.NotNull(form.Controls["ribbonControl"]);
    }
    
    [Fact]
    public void MainForm_ShouldHaveDockManager()
    {
        // Arrange & Act
        var form = new MainForm(Mock.Of<MainViewModel>());
        
        // Assert
        Assert.NotNull(form.Controls["dockManager"]);
    }
    
    [Fact]
    public void MainForm_ShouldHaveTabbedMdiManager()
    {
        // Arrange & Act
        var form = new MainForm(Mock.Of<MainViewModel>());
        
        // Assert
        Assert.NotNull(form.Controls["tabbedMdiManager"]);
    }
    
    [Fact]
    public void MainForm_ShouldOpenFileInTab()
    {
        // Arrange
        var form = new MainForm(Mock.Of<MainViewModel>());
        var filePath = "test.cs";
        var content = "class Test { }";
        
        // Act
        form.OpenFileInTab(filePath, content);
        
        // Assert
        Assert.Single(form.TabControl.TabPages);
        Assert.Equal("test.cs", form.TabControl.TabPages[0].Text);
    }
    
    [Fact]
    public void MainForm_ShouldCloseTab()
    {
        // Arrange
        var form = new MainForm(Mock.Of<MainViewModel>());
        var filePath = "test.cs";
        var content = "class Test { }";
        
        form.OpenFileInTab(filePath, content);
        
        // Act
        form.CloseTab(filePath);
        
        // Assert
        Assert.Empty(form.TabControl.TabPages);
    }
    
    [Fact]
    public void MainForm_ShouldCloseAllTabs()
    {
        // Arrange
        var form = new MainForm(Mock.Of<MainViewModel>());
        
        form.OpenFileInTab("test1.cs", "class Test1 { }");
        form.OpenFileInTab("test2.cs", "class Test2 { }");
        
        // Act
        form.CloseAllTabs();
        
        // Assert
        Assert.Empty(form.TabControl.TabPages);
    }
}
```

---

## 11. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 2.0.0 |
| Status | Active |
| Total Lines | 1500+ |
| DevExpress Components | 12+ |
| MVVM Patterns | 7 |
| Accessibility Features | 4 |
| Theme Support | 3 modes |
| Platform Support | WinForms, WPF, MAUI, Blazor |

---

## 12. Kurallar
>>>>>>> c3e202adbf05605c413ce8e18757b121c201aecb

| # | Kural | Açıklama |
|---|-------|----------|
| 1 | **MVVM Zorunlu** | Code-behind yasak, CommunityToolkit.Mvvm |
| 2 | **DevExpress Mandatory** | Tüm UI kontrolleri DevExpress |
<<<<<<< HEAD
| 3 | **BindableBase** | Tüm ViewModel'lar BindableBase'den türetilir |
| 4 | **ICommand** | Tıklama işlemleri ICommand ile |
| 5 | **ObservableProperty** | Data binding için [ObservableProperty] attribute |
| 6 | **Temiz Kod** | View'da iş mantığı yasak |
=======
| 3 | **Accessibility First** | WCAG 2.1 AA uyumlu |
| 4 | **Responsive Design** | DPI-aware, multi-monitor |
| 5 | **Theme Support** | Dark/Light/Custom tema desteği |
| 6 | **Test Coverage** | ViewModel ve View testleri zorunlu |
| 7 | **Documentation** | Tüm bileşenler dokümante edilmeli |
| 8 | **Performance** | UI thread'i bloke edilmemeli |
| 9 | **Error Handling** | Tüm hatalar yakalanmalı ve loglanmalı |
| 10 | **Localization** | Çoklu dil desteği |
>>>>>>> c3e202adbf05605c413ce8e18757b121c201aecb

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
<<<<<<< HEAD
=======
**Mode:** Red Team · Human Mode · Truth Mode
>>>>>>> c3e202adbf05605c413ce8e18757b121c201aecb
