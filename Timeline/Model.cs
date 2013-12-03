using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Timeline.UC;

namespace Timeline
{
    public class ProjectItem : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// 元素是否选中
        /// </summary>
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                }
            }
        }
        public string FileName { get; set; }

        /// <summary>
        /// 元素是否已经被打开
        /// </summary>
        private bool _isEnabled;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsEnabled"));
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }

    public class Project
    {
        public string Title { get; set; }
        public string ResourcePath { get; set; }
    }

    public class ScatterViewerTag
    {
        public ProjectSurfaceListBoxUserControl ProjectSurfaceListBoxUserControl { get; set; }
        public int Index { get; set; }
    }
}
