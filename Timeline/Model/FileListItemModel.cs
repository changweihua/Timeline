using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Timeline.Model
{
    public class FileListItemModel : INotifyPropertyChanged
    {
        private string m_FileIcon = string.Empty;

        public string FileIcon
        {
            get { return m_FileIcon; }
            set { m_FileIcon = value; OnPropertyChanged("FileIcon"); }
        }
        public string FileType { get; set; }
        public string FileSize { get; set; }
        public string FileName { get; set; }
        public string FileFullPath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
