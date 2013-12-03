using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Timers;
using System.Windows.Threading;
using System.ComponentModel;

namespace ShiningMeeting.Mvvm.Controls
{
	/// <summary>
	/// ========================================
	/// WinFX Custom Control
	/// ========================================
	///
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:CustomControlLibrary"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:CustomControlLibrary;assembly=CustomControlLibrary"
	///
	/// You will also need to add a project reference from the project where the XAML file lives
	/// to this project and Rebuild to avoid compilation errors:
	///
	///     Right click on the target project in the Solution Explorer and
	///     "Add Reference"->"Projects"->[Browse to and select this project]
	///
	///
	/// Step 2)
	/// Go ahead and use your control in the XAML file. Note that Intellisense in the
	/// XML editor does not currently work on custom controls and its child elements.
	///
	///     <MyNamespace:Clock/>
	///
	/// </summary>

	public class Clock : System.Windows.Controls.Control
	{
		private DispatcherTimer timer;

		static Clock()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Clock), new FrameworkPropertyMetadata(typeof(Clock)));
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			UpdateDateTime();

			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(1000 - DateTime.Now.Millisecond);
			timer.Tick += new EventHandler(Timer_Tick);
			timer.Start();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			UpdateDateTime();

			timer.Interval = TimeSpan.FromMilliseconds(1000 - DateTime.Now.Millisecond);
			timer.Start();
		}

		private void UpdateDateTime()
		{
            this.DateTime = TimeZoneInfo.ConvertTime(System.DateTime.Now, TimeZone);
		}

		#region DateTime property
		public DateTime DateTime
		{
			get
			{
				return (DateTime)GetValue(DateTimeProperty);
			}
			private set
			{
				SetValue(DateTimeProperty, value);
			}
		}

		public static DependencyProperty DateTimeProperty = DependencyProperty.Register(
				"DateTime",
				typeof(DateTime),
				typeof(Clock),
				new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(OnDateTimeInvalidated)));

		public static readonly RoutedEvent DateTimeChangedEvent =
			EventManager.RegisterRoutedEvent("DateTimeChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<DateTime>), typeof(Clock));

		protected virtual void OnDateTimeChanged(DateTime oldValue, DateTime newValue)
		{
			RoutedPropertyChangedEventArgs<DateTime> args = new RoutedPropertyChangedEventArgs<DateTime>(oldValue, newValue);
			args.RoutedEvent = Clock.DateTimeChangedEvent;
			RaiseEvent(args);
		}

		private static void OnDateTimeInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Clock clock = (Clock)d;

			DateTime oldValue = (DateTime)e.OldValue;
			DateTime newValue = (DateTime)e.NewValue;

			clock.OnDateTimeChanged(oldValue, newValue);

		}
		#endregion



        public TimeZoneInfo TimeZone
        {
            get { return (TimeZoneInfo)GetValue(TimeZoneProperty); }
            set { SetValue(TimeZoneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeZone.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeZoneProperty =
            DependencyProperty.Register("TimeZone", typeof(TimeZoneInfo), typeof(Clock), new UIPropertyMetadata(TimeZoneInfo.Local));

        public string DisplayName { get; set; }
	}
}
