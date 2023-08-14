## About Timeliner

*Interactive control for visualizing the schedule*

The GUI element is designed for WPF .NET applications. Allows the developer to show tasks grouped in a timeline. An element can include multiple Items within which multiple Jobs can be included. Jobs must not overlap. The control is under development.

Some features:
* Open source
* When the work is small - a sign is shown
* Scroll up and down elements
* Scroll left and right in time
* Mouse and bind controls
* Does not use third party libraries

### Installation

```dotnet add package TimelinerNet```

## Usage

It has the data structure shown below. Arrange your data in these structures:
```c#
TimelinerData
    List<TimelinerItem> Items
        string Name
        string Description
        List<TimelinerJob> Jobs
            string Name
            string TextUp
            string TextDown
            string TextRight
            Brush Color
            bool IsStripedColor
            DateTime Begin
            DateTime End
```

To use it, you need to insert a XAML element.
* `Data` - **TimelinerData** | Restriction for setting by the user. Less than this value, the user will not be able to set.
* `Now` - **DateTime** | At this time, a red marker will be shown.
* `LeftEdge` - **DateTime** | Left border of screen time.
* `RightEdge` - **DateTime** | Right border of screen time.
* `TrackNow` - **bool** | When changed the `Now` parameter, the time bar will follow it.
```xaml
<tl:Timeliner Data="{Binding Data}"/>
```
Don't forget to add the library to the list:
```xaml
<Window x:Class="Example.MainWindow"
        ...
        xmlns:tl="clr-namespace:TimelinerNet;assembly=TimelinerNet"
        ...
```

In addition. Clicking on a Job shows a Popup with additional information. To change the view of this window, use the DataTemplatePopup property.
```xaml
<tl:Timeliner Data="{Binding Data}">
    <tl:Timeliner.DataTemplatePopup>
        <DataTemplate>
            <StackPanel>
                <TextBlock Text="{Binding Name}"/>
                <TextBlock Text="{Binding CustomObject.CustomString}" TextWrapping="Wrap"/>
                <TextBlock Text="{Binding Begin, StringFormat={}{0:dd.MM.yyyy HH:mm}}"/>
            </StackPanel>
        </DataTemplate>
    </tl:Timeliner.DataTemplatePopup>
</tl:Timeliner>
```
## License

Distributed under the MIT License. See `LICENSE` for more information.