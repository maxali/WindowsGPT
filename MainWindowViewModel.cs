using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Markdig;

namespace ChatGptApp
{

  public class MainWindowViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    public ICommand ShutdownCommand => new RelayCommand(() =>
    {
      Application.Current.Shutdown();
    });
    public ICommand ShowCommand => new RelayCommand(() =>
    {
      if (Application.Current.MainWindow != null)
      {
        Application.Current.MainWindow.Show();
      }
    });
    public ChatGptApi ChatGptApi { get; }
    public ErrorLogger ErrorLogger { get; }

    private readonly MarkdownPipeline _pipeline;

    private string _markdownText;

    public string MarkdownText
    {
      get { return _markdownText; }
      set
      {
        if (value != _markdownText)
        {
          _markdownText = value;
          OnPropertyChanged(nameof(MarkdownText));
        }
      }
    }

    public MainWindowViewModel()
    {
      ChatGptApi = new ChatGptApi();
      ErrorLogger = new ErrorLogger();

      _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    }

    protected virtual void OnPropertyChanged(string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void ClearResponseBox()
    {
      MarkdownText = string.Empty;
    }


    public async Task ProcessPrompt(string contextToggle, string prompt)
    {
      if (string.IsNullOrEmpty(prompt)) return;

      MarkdownText += @"

### " + (contextToggle == "" ? "" : contextToggle + ": ") + prompt + @"

";
      MarkdownText += await ChatGptApi.SendPrompt(contextToggle, prompt);

    }
  }
}