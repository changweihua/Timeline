using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interactivity;
//using Microsoft.Expression.Interactivity.Core;

namespace ShiningMeeting.Mvvm.Behavior
{
    public class AutoParentSizeBehavior : Behavior<FrameworkElement>
    {
        public AutoParentSizeBehavior()
        {
            // 在此点下面插入创建对象所需的代码。

            //
            // 下面的代码行用于在命令
            // 与要调用的函数之间建立关系。如果您选择
            // 使用 MyFunction 和 MyCommand 的已注释掉的版本，而不是创建自己的实现，
            // 请取消注释以下行并添加对 Microsoft.Expression.Interactions 的引用。
            //
            // 文档将向您提供简单命令实现的示例，
            // 您可以使用该示例，而不是使用 ActionCommand 并引用 Interactions 程序集。
            //
            //this.MyCommand = new ActionCommand(this.MyFunction);
        }

        SizeChangedEventHandler parentElementSizeChangedHandle = null;
        RoutedEventHandler elementLoadedHandle = null;

        FrameworkElement element = null;

        protected override void OnAttached()
        {
            element = this.AssociatedObject as FrameworkElement;
            elementLoadedHandle = new RoutedEventHandler(element_Loaded);
            element.Loaded += elementLoadedHandle;

            base.OnAttached();
            // 插入要在将 Behavior 附加到对象时运行的代码。
        }

        void element_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement parentElement = VisualTreeHelper.GetParent(element) as FrameworkElement;
            parentElementSizeChangedHandle = new SizeChangedEventHandler(parentElement_SizeChanged);
            if (parentElement == null) return;

            parentElement.SizeChanged += parentElementSizeChangedHandle;

            element.MaxHeight = parentElement.ActualHeight;
            element.MinHeight = parentElement.ActualHeight;
            element.Height = parentElement.ActualHeight;
            element.MaxWidth = parentElement.ActualWidth;
            element.MinHeight = parentElement.ActualWidth;
            element.Width = parentElement.ActualWidth;
        }


        void parentElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            element.MaxHeight = e.NewSize.Height;
            element.MinHeight = e.NewSize.Height;
            element.Height = e.NewSize.Height;
            element.MaxWidth = e.NewSize.Width;
            element.MinHeight = e.NewSize.Width;
            element.Width = e.NewSize.Width;
        }

        protected override void OnDetaching()
        {

            FrameworkElement parentElement = VisualTreeHelper.GetParent(element) as FrameworkElement;
            if (elementLoadedHandle != null) 
            {
                element.Loaded -= elementLoadedHandle;
                elementLoadedHandle = null;
            }
            if (parentElementSizeChangedHandle != null) 
            {
                parentElement.SizeChanged -= parentElementSizeChangedHandle;
                parentElementSizeChangedHandle = null;
            }

            base.OnDetaching();
            // 插入要在从对象中删除 Behavior 时运行的代码。
        }

        /*
        public ICommand MyCommand
        {
            get;
            private set;
        }
		 
        private void MyFunction()
        {
            // 插入要在从对象中删除 Behavior 时运行的代码。
        }
        */
    }
}