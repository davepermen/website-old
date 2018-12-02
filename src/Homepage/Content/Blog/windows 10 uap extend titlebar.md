# Windows 10 UAP Extend Titlebar

In my case, I want an App to be completely black, with no Titlebar (but the minimize, maximize, close Buttons available and visible).

In Mainpage.xaml.cs, I put this piece of code:

```csharp
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
        var titlebar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
        titlebar.ButtonBackgroundColor = Colors.Transparent;
        titlebar.ButtonForegroundColor = Colors.White;
    }
}
```

And in Mainpage.xaml, I made everything black, and some test-’Hello, World’ that should show up at the top of the window, like this:

```xml
<Page x:Class="App1.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:local="using:App1"
      xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
      xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
      mc:Ignorable="d">
<Grid Background="Black">
<TextBlock FontSize="50" Foreground="White">Hello, World!</TextBlock>
    </Grid>
</Page>
```

The result:
![screenshot of styled window](/blog/windows-10-uap-extend-titlebar/result.png)