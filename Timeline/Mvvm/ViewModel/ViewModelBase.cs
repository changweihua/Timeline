using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using ShiningMeeting.MEF.WPFAttribute;
using System.Windows.Media.Animation;

namespace ShiningMeeting.Mvvm.ViewModel
{
    public class ViewModelBase : Animatable, INotifyPropertyChanged, IViewModel
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
        public virtual void InitialViewModel() { }
        public virtual void ShowDialog() { throw new NotSupportedException(); }
        protected override Freezable CreateInstanceCore() { return this; }
    }

    public static class NotifyChange
    {
        public static void NotifyPropertyChanged<T, TResult>(this T viewModel, Expression<Func<T, TResult>> action) where T : ViewModelBase
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            viewModel.OnPropertyChanged(propertyName);
        }
    }
}
