using Addon.Commands;
using Addon.Extensions;
using Addon.Resources;
using Autodesk.Windows;
using System.Windows.Controls;

namespace Addon.Ribbon.Panels
{
    class GeneralRibbonPanel : RibbonPanel
    {
        public GeneralRibbonPanel()
        {
            Source = new RibbonPanelSource()
            {
                Title = Strings.GeneralRibbonPanel_Text,
                Description = Strings.GeneralRibbonPanel_Description,
                Items =
                {
                    MainCircuitBreakerButton(),
                    CircuitBreakerButton(),
                    JunctionBoxButton(),
                },
            };
        }

        private RibbonItem MainCircuitBreakerButton() => new RibbonButton()
        {
            ShowText = true,
            Text = "Main Circuit\nBreaker",
            Description = "Place a main circuit breaker on drawing.",
            ShowImage = true,
            Image = Icons.MCBSmall.ToBitmapSource(),
            LargeImage = Icons.MCBLarge.ToBitmapSource(),
            Size = RibbonItemSize.Large,
            Orientation = Orientation.Vertical,
            CommandHandler = ControlCommands.CommonCommand,
            CommandParameter = CommandKeywords.MainCircuitBreaker,
        };

        private RibbonItem CircuitBreakerButton() => new RibbonButton()
        {
            ShowText = true,
            Text = "Circuit\nBreaker",
            Description = "Place a circuit breaker on drawing",
            ShowImage = true,
            Image = Icons.CBSmall.ToBitmapSource(),
            LargeImage = Icons.CBLarge.ToBitmapSource(),
            Size = RibbonItemSize.Large,
            Orientation = Orientation.Vertical,
            CommandHandler = ControlCommands.CommonCommand,
            CommandParameter = CommandKeywords.CircuitBreaker,
        };

        private RibbonItem JunctionBoxButton() => new RibbonButton()
        {
            ShowText = true,
            Text = "Junction\nBox",
            Description = "Place a junction blox on drawing",
            ShowImage = true,
            Image = Icons.LineSmall.ToBitmapSource(),
            LargeImage = Icons.LineLarge.ToBitmapSource(),
            Size = RibbonItemSize.Large,
            Orientation = Orientation.Vertical,
            CommandHandler = ControlCommands.CommonCommand,
            CommandParameter = CommandKeywords.JunctionBox,
        };
    }
}
