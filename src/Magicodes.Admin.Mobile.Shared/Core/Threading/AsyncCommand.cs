using System;
using System.Windows.Input;
using Abp.Extensions;
using Xamarin.Forms;

namespace Magicodes.Admin.Core.Threading
{
    public class AsyncCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly ICommand _command;

        protected AsyncCommand(ICommand command)
        {
            _command = command;
            command.CanExecuteChanged += (sender, args) => CanExecuteChanged.InvokeSafely(sender, args);
        }

        public bool CanExecute(object parameter)
        {
            return _command.CanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _command.Execute(parameter);
        }

        public static AsyncCommand Create(Func<System.Threading.Tasks.Task> func)
        {
            return new AsyncCommand(new Command(async () => await func()));
        }
    }
}