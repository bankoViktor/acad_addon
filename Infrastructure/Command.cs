using System;
using System.Windows.Input;

namespace Addon.Infrastructure
{
    /// <summary>
    /// Простая комманда, реализующая интерфейс <see cref="ICommand"/>.
    /// </summary>
    public class Command : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        /// <summary>
        /// Конструктор без контроля возможности выполнения команды.
        /// </summary>
        /// <param name="execute">Делегат, выполняемый при активации команды.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Command(Action<object> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        /// <summary>
        /// Конструкто.
        /// </summary>
        /// <param name="execute">Делегат, выполняемый при активации команды.</param>
        /// <param name="canExecute">Делегат, определяющий возможность выполнения команды.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Command(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        #region ICommand

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => _execute(parameter);

        #endregion
    }
}
