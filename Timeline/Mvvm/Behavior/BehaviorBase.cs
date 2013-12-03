using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interactivity;
using System.Windows;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ShiningMeeting.Mvvm.Behavior
{
    public abstract class BehaviorBase : Behavior<FrameworkElement>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
    }

    public static class NotifyChange
    {
        public static void NotifyPropertyChanged<T, TResult>(this T viewModel, Expression<Func<T, TResult>> action) where T : BehaviorBase
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            viewModel.OnPropertyChanged(propertyName);
        }
    }
}
