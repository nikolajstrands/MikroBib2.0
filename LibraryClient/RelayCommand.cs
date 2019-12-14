using System;
using System.Diagnostics;
using System.Windows.Input;

namespace LibraryClient
{

    public class RelayCommand : ICommand
    { 
        // Delegat der peger på metode, kommandoen skal udføre
        readonly Action<object> execute;
        // Delegat der peger på den metode, der styrer om kommandoen kan udføres
        readonly Predicate<object> canExecute;        


        // Constructor der skabes en ny kommando
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            this.execute = execute;
            this.canExecute = canExecute;           
        }

        // Når kommandoen skal udføres invokeres første delegat
        public void Execute(object parameters)
        {
            execute(parameters);
        }

        // For at undersøge om kommandoen kan udføres, invokeres anden delegat
        public bool CanExecute(object parameters)
        {
            return canExecute == null ? true : canExecute(parameters);
        }
          
        // Kommandoen tilføjes til CommandManager
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;

        }

    }
}