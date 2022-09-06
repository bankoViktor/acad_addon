using Addon.Infrastructure;
using System;
using System.Windows.Input;

namespace Addon.Commands
{
    class UpdateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        void ICommand.Execute(object parameter)
        {
            var updater = new Updater();
            updater.Update();
        }
    }
}
