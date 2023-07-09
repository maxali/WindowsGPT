using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Drawing;

using System.Windows.Media;
using Markdig;
using Newtonsoft.Json;
using NHotkey;
using NHotkey.Wpf;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Brushes = System.Windows.Media.Brushes;
using System.Security.Authentication;

namespace WindowsGPT
{
  public partial class MainWindow : Window
  {
    private readonly ChatGptApi _chatGptApi;
    private readonly ErrorLogger _errorLogger;
    private readonly MarkdownPipeline _pipeline;
    private string _contextToggle = "";
    public MainWindowViewModel ViewModel { get; set; }

    public MainWindow()
    {
      InitializeComponent();
      ViewModel = new MainWindowViewModel();
      this.DataContext = ViewModel; // Setting data context

      try
      {
        HotkeyManager.Current.AddOrReplace("ShowChatGptApp", System.Windows.Input.Key.I, System.Windows.Input.ModifierKeys.Alt, OnHotKeyHandler);
      }
      catch (Exception ex)
      {
        ViewModel.ErrorLogger.Log(ex);
      }
    }

    private void OnHotKeyHandler(object sender, HotkeyEventArgs e)
    {
      this.Show();
      e.Handled = true;
    }

    private async void GoButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        progressBar.Visibility = Visibility.Visible;
        inputBox.IsEnabled = false;

        // Assuming that ProcessPrompt is an async function that returns Task<string>
        await ViewModel.ProcessPrompt(_contextToggle, inputBox.Text);
        inputBox.Text = String.Empty;
        inputBox.IsEnabled = true;
      }
      catch (AuthenticationException ex)
      {
        MessageBox.Show(ex.Message);
      }
      catch (Exception ex)
      {
        _errorLogger.Log(ex);
        MessageBox.Show("An error occurred while processing your request. Please try again later.");
      }
      finally
      {
        progressBar.Visibility = Visibility.Hidden;
      }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
      this.Hide();
      GC.Collect();
    }

    private void InputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == Key.Return)
      {
        GoButton_Click(sender, e);
      }
      else if (e.Key == Key.Escape)
      {
        CloseButton_Click(sender, e);
      }
    }

    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
        this.DragMove();
    }

    private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 2)
      {
        if (this.WindowState == WindowState.Maximized)
          this.WindowState = WindowState.Normal;
        else
          this.WindowState = WindowState.Maximized;
      }
    }

    private void ShowHidePanel_Click(object sender, RoutedEventArgs e)
    {
      if (SidePanel.Visibility == Visibility.Visible)
      {
        SidePanel.Visibility = Visibility.Collapsed;
        ShowHidePanel.ToolTip = "Open settings sidebar";
        ShowHidePanel.Content = "⮞";
        ShowHidePanel.Height = 30;
        ShowHidePanel.Background = Brushes.Transparent;
      }
      else
      {
        SidePanel.Visibility = Visibility.Visible;
        ShowHidePanel.ToolTip = "Close settings sidebar";
        ShowHidePanel.Content = "⮜";
        ShowHidePanel.Height = 30;
        ShowHidePanel.Background = Brushes.Black;
      }
    }

    private void ToggleButton_Checked(object sender, RoutedEventArgs e)
    {
      ToggleButton button = sender as ToggleButton;
      if (button?.IsChecked == true)
      {
        var content = button.Content.ToString();
        _contextToggle = content;

        foreach (object child in ContextToggleButtons.Children)
        {
          if (child is not ToggleButton) return;

          ToggleButton childButton = (ToggleButton)child;
          if (childButton.Content.ToString() != content)
          {
            childButton.IsChecked = false;
          }
        }
      }
      else
      {
        _contextToggle = "";
      }


    }
    

    private void ClearResponse_OnClick(object sender, RoutedEventArgs e)
    {
      ViewModel.ClearResponseBox();
    }

    private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
    {
      var settingsWindow = new SettingsWindow();
      settingsWindow.Owner = this;
      settingsWindow.ShowDialog();
    }

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        CloseButton_Click(sender, e);
    }
  }
}
