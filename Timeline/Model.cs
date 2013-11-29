using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Timeline
{
    public class ProjectItem
    {
        public bool IsOpened { get; set; }
        public string FileName { get; set; }
    }

    public class Project
    {
        public string Title { get; set; }
        public string ResourcePath { get; set; }
    }
}
