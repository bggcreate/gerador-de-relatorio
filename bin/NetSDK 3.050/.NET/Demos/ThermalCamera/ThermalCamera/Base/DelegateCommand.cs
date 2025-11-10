using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IVSDemo.Base
{
    class DelegateCommand:ICommand
    {
        private readonly Action _execute;//执行函数
        private readonly Func<bool> _canExecute;//函数是否能执行
        private ICommand detailCancelCommand;

        public DelegateCommand(Action exectue, Func<bool> canExecute)
        {
            if (null == exectue)
            {
                throw new ArgumentNullException("exectue");
            }
            _execute = exectue;
            _canExecute = canExecute;
        }
        public DelegateCommand(Action exectue) : this(exectue, null)
        {
        }

        public DelegateCommand(ICommand detailCancelCommand)
        {
            this.detailCancelCommand = detailCancelCommand;
        }

        // 当出现影响是否应执行该命令的更改时发生
        // CommandManager检测到可能会影响命令的可执行状态时
        // 触发RequerySuggested 事件
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }
        //检测命令是否可执行，并将bool类型的返回值设置到绑定控件的IsEnabled属性上
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (null == _canExecute)
            {
                return true;
            }
            return _canExecute();
        }
        //命令执行的方法
        public void Execute(object parameter)
        {
            if (null != _execute && CanExecute(parameter))
            {
                _execute();
            }
        }
    }

    class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _execute;

        private readonly Func<T, bool> _canExecute;

        public DelegateCommand(Action<T> exectue, Func<T, bool> canExecute)
        {
            if (null == exectue)
            {
                throw new ArgumentNullException("exectue");
            }
            _execute = exectue;
            _canExecute = canExecute;
        }

        public DelegateCommand(Action<T> exectue)
            : this(exectue, null)
        {

        }

        // 当出现影响是否应执行该命令的更改时发生
        // CommandManager检测到可能会影响命令的可执行状态时
        // 触发RequerySuggested 事件
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        //检测命令是否可执行，并将bool类型的返回值设置到绑定控件的IsEnabled属性上
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (null == _canExecute)
            {
                return true;
            }
            return _canExecute((T)parameter);
        }
        //命令执行的方法
        public void Execute(object parameter)
        {
            if (null != _execute && CanExecute(parameter))
            {
                _execute((T)parameter);
            }
        }
    }
}
