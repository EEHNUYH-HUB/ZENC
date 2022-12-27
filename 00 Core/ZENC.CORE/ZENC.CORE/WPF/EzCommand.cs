using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ZENC.CORE;


namespace ZENC.CORE.WPF
{
    public class EzCommandResult
    {
        public object Parameter { get; set; }
        public EventArgs Args { get; set; }
    }
    public delegate void EzCommandHandler(object sender, EzCommandResult e);
    public interface IEzCommand : ICommand
    {
        void Execute(object parameter, EzCommandResult e);
    }

    public class EzCommand : IEzCommand
    {

        public Predicate<object> CanExecuteDelegate { get; set; }


        public Action<object> ExecuteDelegate { get; set; }

        public EzCommandHandler ExecuteEventDelegate { get; set; }

        #region ICommand Members


        public bool CanExecute(object parameter)
        {

            if (CanExecuteDelegate != null)
                return CanExecuteDelegate(parameter);
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {

            try
            {
                if (ExecuteDelegate != null)
                {
                    ExecuteDelegate(parameter);
                }

                if (CompletedAction != null)
                {
                    CompletedAction();
                }
            }
            catch { 
            }
        }

        public void Execute(object sender, EzCommandResult e)
        {
            if (ExecuteEventDelegate != null)
                ExecuteEventDelegate(sender, e);
        }

        /// <summary>
        /// constructor
        /// </summary>
        public EzCommand()
        {

        }
        public EzCommand(Action<object> ExecuteDelegate)
            : this(ExecuteDelegate, null)
        {

        }
        public EzCommand(Action<object> ExecuteDelegate, Predicate<object> CanExecuteDelegate)
        {
            this.ExecuteDelegate = ExecuteDelegate;
            this.CanExecuteDelegate = CanExecuteDelegate;
        }

        /// <summary>
        /// 커맨드가 완료되고 후처리가 필요한 경우 사용됩니다. // 한태광 / 2015.3.11
        /// </summary>
        public Action CompletedAction { get; set; }

        #endregion
    }
}
