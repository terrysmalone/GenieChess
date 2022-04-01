using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Genie_WPF;

public class RelayCommand : ICommand
{
    readonly Action<object> _execute;
    readonly Predicate<object> _canExecute;

    public RelayCommand(Action<object> execute) : this(execute, null) { }

    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    {
        if (execute is null)
        {
            throw new ArgumentNullException("execute");
        }

        _execute = execute; _canExecute = canExecute;
    }

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
        return _canExecute?.Invoke(parameter) ?? true;
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public void Execute(object parameter)
    {
        _execute(parameter);
    }

}

