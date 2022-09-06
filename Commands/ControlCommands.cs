using Addon.Infrastructure;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Addon.Commands
{
    public static class ControlCommands
    {
        #region Команда CommonCommand
        public static ICommand CommonCommand
        {
            get
            {
                if (_commonCommand == null)
                {
                    _commonCommand = new Command(
                        execute: sender =>
                        {
                            var ribbonCmd = sender as RibbonCommandItem;
                            var cmd = ribbonCmd.CommandParameter;
                            if (cmd is string stringCmd && !string.IsNullOrWhiteSpace(stringCmd))
                            {
                                var doc = AcadApp.DocumentManager.MdiActiveDocument;
                                doc.SendStringToExecute(stringCmd + " ", true, false, true);
                            }
                        }
                    );
                }

                return _commonCommand;
            }
        }
        private static ICommand _commonCommand;
        #endregion
    }
}
