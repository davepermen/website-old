# UWP App for Kiosk Mode

I want to use my older Surface Pro 2 as a picture frame, while it’s running my webserver in the background ([davepermen.net](https://davepermen.net)).
To do so, I considered setting up Windows 10 Kiosk Mode (a.k.a. Assigned Access), creating a simple UWP App and deploy it.

What I learned:

To be able to chose the application in ‘Assigned Access’, you need to enable it in the Package.appxmanifest. Add this to it:

```xml
<Package>
  <Applications>
    <Application>
    <!-- insert this extension -->
      <Extensions>
        <uap:Extension Category="windows.aboveLockScreen" />
      </Extensions>
    </Application>
  </Applications>
</Package>
```

Once you’ve created your package and installed it, it should be choseable from the ‘Assigned Access’ app list.

Given I want this to be an always running app, I need it to force the tablet to never go to sleep. To do this, I have to request the display to stay active. I do so in the MainPage constructor

```csharp
public sealed partial class MainPage : Page
{
    DisplayRequest request;
    public MainPage()
    {
        this.InitializeComponent();
        request = new DisplayRequest();
        request.RequestActive();
    }
}
```

And that is all that’s needed to allow a UWP app to run continuously in Kiosk Mode on Windows 10.

To make the Kiosk user auto-logon on boot, I used ‘control userpasswords2’, launched from cmd.exe, to configure a user.