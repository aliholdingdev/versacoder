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

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
