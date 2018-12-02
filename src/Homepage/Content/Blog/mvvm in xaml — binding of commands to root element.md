# MVVM in XAML — Binding of Commands to Root Element

If you want a delete button in each element, you typically can’t just add the binding as the method is on a) the observable list itself, or b) on some parent object owning that list.

```xml
<Button Command={Binding SomeDeleteMethod} CommandParameter={Binding} />
```

To go to your root object (typically the views viewmodel), bind it to some root xaml tag (\<Window Tag=\{Binding}>…\</Window> or \<Page Tag=\{Binding}>…\</Page>) and access it trough an element binding:

```xml
<Window Tag={Binding}>
    …
    <Button Command="{Binding Tag.RemoveElement, ElementName=window}"
            CommandParameter="{Binding}" />
    …
</Window>