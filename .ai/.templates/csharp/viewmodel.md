---
title: "ViewModel Template"
type: template
category: csharp
version: 1.0.0
---

# ViewModel Template

## Kullanım

Yeni bir ViewModel oluştururken bu template'i kullanın.

## Template

```csharp
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using {AbstractionsNamespace};

namespace {ViewModelNamespace}
{
    /// <summary>
    /// {ViewName} için ViewModel
    /// </summary>
    public partial class {ViewModelName} : ObservableObject
    {
        #region Fields

        private readonly {ServiceName} _service;
        private readonly INavigationService _navigation;
        private readonly IDialogService _dialog;

        #endregion

        #region Properties

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

{AdditionalProperties}

        #endregion

        #region Constructor

        public {ViewModelName}(
            {ServiceName} service,
            INavigationService navigation,
            IDialogService dialog)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));
        }

        #endregion

        #region Commands

        [RelayCommand]
        private async Task LoadAsync()
        {
            try
            {
                IsLoading = true;
                HasError = false;
                ErrorMessage = string.Empty;

                // TODO: Veri yükleme mantığı

            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                IsLoading = true;
                HasError = false;

                // TODO: Kaydetme mantığı

                await _dialog.ShowSuccessAsync("Saved successfully");
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                await _dialog.ShowErrorAsync(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            _navigation.GoBack();
        }

{AdditionalCommands}

        #endregion

        #region Methods

        protected virtual void OnLoaded()
        {
            // Override for custom load logic
        }

        protected virtual void OnUnloaded()
        {
            // Override for cleanup logic
        }

        #endregion
    }
}
```

## Örnek Kullanım

```csharp
// SessionViewModel
public partial class SessionViewModel : ObservableObject
{
    private readonly ISessionService _sessionService;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialog;

    [ObservableProperty]
    private SessionDto? _currentSession;

    [ObservableProperty]
    private ObservableCollection<MessageDto> _messages = new();

    public SessionViewModel(
        ISessionService sessionService,
        INavigationService navigation,
        IDialogService dialog)
    {
        _sessionService = sessionService;
        _navigation = navigation;
        _dialog = dialog;
    }

    [RelayCommand]
    private async Task LoadSessionAsync(Guid sessionId)
    {
        try
        {
            IsLoading = true;
            CurrentSession = await _sessionService.GetByIdAsync(sessionId);
            Messages = new ObservableCollection<MessageDto>(
                CurrentSession.Messages);
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SendMessageAsync(string content)
    {
        try
        {
            IsLoading = true;

            var message = new MessageDto
            {
                Content = content,
                Role = "user",
                Timestamp = DateTime.UtcNow
            };

            Messages.Add(message);

            var response = await _sessionService.SendMessageAsync(
                CurrentSession!.Id, content);

            Messages.Add(response);
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

---

## 4. ViewModel Örnekleri

### 4.1 Main ViewModel

```csharp
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace {ViewModelNamespace}
{
    /// <summary>
    /// Ana ViewModel
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        #region Fields

        private readonly ISessionService _sessionService;
        private readonly INavigationService _navigation;

        #endregion

        #region Properties

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

        #endregion

        #region Constructor

        public MainViewModel(
            ISessionService sessionService,
            INavigationService navigation)
        {
            _sessionService = sessionService;
            _navigation = navigation;
        }

        #endregion

        #region Commands

        [RelayCommand]
        private async Task NewSessionAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Creating new session...";

                var session = await _sessionService.CreateSessionAsync("New Session");
                var sessionVm = new SessionViewModel(session);

                Sessions.Add(sessionVm);
                CurrentSession = sessionVm;

                StatusMessage = "Session created successfully";
            }
            catch (Exception ex)
            {
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

        #endregion
    }
}
```

### 4.2 Chat Panel ViewModel

```csharp
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace {ViewModelNamespace}
{
    /// <summary>
    /// Chat paneli ViewModel
    /// </summary>
    public partial class ChatPanelViewModel : ObservableObject
    {
        #region Fields

        private readonly IAgentRunner _agentRunner;
        private readonly IContextManager _contextManager;

        #endregion

        #region Properties

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

        #endregion

        #region Constructor

        public ChatPanelViewModel(
            IAgentRunner agentRunner,
            IContextManager contextManager)
        {
            _agentRunner = agentRunner;
            _contextManager = contextManager;
        }

        #endregion

        #region Commands

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

        #endregion
    }
}
```

### 4.3 Solution Panel ViewModel

```csharp
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace {ViewModelNamespace}
{
    /// <summary>
    /// Solution paneli ViewModel
    /// </summary>
    public partial class SolutionPanelViewModel : ObservableObject
    {
        #region Fields

        private readonly IProjectRepository _projectRepository;

        #endregion

        #region Properties

        [ObservableProperty]
        private ObservableCollection<FileNodeViewModel> _fileTree = new();

        [ObservableProperty]
        private FileNodeViewModel? _selectedFile;

        [ObservableProperty]
        private string _searchText = string.Empty;

        #endregion

        #region Constructor

        public SolutionPanelViewModel(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        #endregion

        #region Commands

        [RelayCommand]
        private async Task LoadProjectAsync(string projectPath)
        {
            try
            {
                if (!Directory.Exists(projectPath))
                    return;

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
                // Log error
            }
        }

        [RelayCommand]
        private void OpenFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var content = File.ReadAllText(filePath);
                // Open in new tab
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

        #endregion
    }
}
```

---

## 5. ViewModel Base Classes

### 5.1 Base ViewModel

```csharp
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace {ViewModelNamespace}
{
    /// <summary>
    /// Temel ViewModel sınıfı
    /// </summary>
    public abstract class ViewModelBase : ObservableObject
    {
        #region Properties

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        #endregion

        #region Methods

        protected void SetError(string message)
        {
            HasError = true;
            ErrorMessage = message;
        }

        protected void ClearError()
        {
            HasError = false;
            ErrorMessage = string.Empty;
        }

        protected async Task ExecuteAsync(Func<Task> action)
        {
            try
            {
                IsLoading = true;
                ClearError();
                await action();
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion
    }
}
```

---

## 6. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| ViewModel Examples | 3 (Main, Chat, Solution) |
| Base Classes | 1 |
| Commands | 10+ |
| Properties | 20+ |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
