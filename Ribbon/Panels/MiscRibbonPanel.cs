using Addon.Commands;
using Addon.Extensions;
using Addon.Forms;
using Addon.Infrastructure;
using Addon.Resources;
using Autodesk.Windows;
using System.Windows.Controls;

namespace Addon.Ribbon.Panels
{
    class MiscRibbonPanel : RibbonPanel
    {
        public MiscRibbonPanel()
        {
            Source = new RibbonPanelSource()
            {
                Title = Strings.MiscRibbonPanel_Text,
                Description = Strings.MiscRibbonPanel_Description,
                Items =
                {
                    new RibbonRowPanel()
                    {
                        Text = "Row Panel",
                        Description = "Description",
                        ShowText = true,
                        Items =
                        {
                            UpdateButton(),
                            new RibbonRowBreak(),
                            SettingsButton(),
                            new RibbonRowBreak(),
                            AboutButton(),
                        },
                    },
                },
            };
        }

        private RibbonItem UpdateButton() => new RibbonButton()
        {
            ShowText = true,
            Text = Strings.MiscRibbonPanel_UpdateButton_Text,
            Description = Strings.MiscRibbonPanel_UpdateButton_Description,
            ShowImage = true,
            Image = Icons.UpdateSmall.ToBitmapSource(),
            Orientation = Orientation.Horizontal,
            CommandHandler = new UpdateCommand(),
        };

        private RibbonItem SettingsButton() => new RibbonButton()
        {
            ShowText = true,
            Text = Strings.MiscRibbonPanel_SettingsButton_Text,
            Description = Strings.MiscRibbonPanel_SettingsButton_Description,
            ShowImage = true,
            Image = Icons.LineSmall.ToBitmapSource(),
            Orientation = Orientation.Horizontal,
            CommandHandler = new Command(parameter =>
            {
                var dlg = new SettingsDialog();
                dlg.ShowDialog();
            }),
        };

        private RibbonItem AboutButton() => new RibbonButton()
        {
            ShowText = true,
            Text = Strings.MiscRibbonPanel_AboutButton_Text,
            Description = Strings.MiscRibbonPanel_AboutButton_Description,
            ShowImage = true,
            Image = Icons.AboutSmall.ToBitmapSource(),
            Orientation = System.Windows.Controls.Orientation.Horizontal,
            CommandHandler = new Command(parameter =>
            {
                var dlg = new AboutDialog();
                dlg.ShowDialog();
            }),
        };
    }
}
