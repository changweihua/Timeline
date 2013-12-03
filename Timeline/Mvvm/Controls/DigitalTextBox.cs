using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;

namespace ShiningMeeting.Mvvm.Controls
{
    public class DigitalTextBox : System.Windows.Controls.TextBox
    {
        private bool m_CanInputDecimal = false;
        public bool CanInputDecimal 
        {
            get { return m_CanInputDecimal; }
            set { m_CanInputDecimal = value; }
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Decimal || e.Key.ToString() == "Tab") 
            { 
                if (Text.Contains(".") && e.Key == Key.Decimal) 
                { 
                    e.Handled = true; 
                    return; 
                }
                else if (e.Key == Key.Decimal) 
                {
                    if (!m_CanInputDecimal)
                    { e.Handled = true; return; }
                }
                e.Handled = false; 
            } 
            else if (((e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.OemPeriod) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift) 
            { 
                if (Text.Contains(".") && e.Key == Key.OemPeriod) 
                { 
                    e.Handled = true; 
                    return; 
                }
                else if (e.Key == Key.OemPeriod)
                {
                    if (!m_CanInputDecimal)
                    { e.Handled = true; return; }
                }
                e.Handled = false; 
            } 
            else 
            { 
                e.Handled = true; 
                if (e.Key.ToString() != "RightCtrl") 
                { 
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnTextChanged(System.Windows.Controls.TextChangedEventArgs e)
        {
            TextChange[] change = new TextChange[e.Changes.Count]; 
            e.Changes.CopyTo(change, 0); 
            int offset = change[0].Offset; 
            if (change[0].AddedLength > 0) 
            { 
                double num = 0; 
                if (!Double.TryParse(Text, out num)) 
                { 
                    Text = Text.Remove(offset, change[0].AddedLength); 
                    Select(offset, 0);
                }
            }

            base.OnTextChanged(e);
        }
    }
}
