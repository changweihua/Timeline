using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Windows.Interactivity;
using System.Windows.Controls;
using System.Windows.Data;

namespace Timeline.ToolClasses
{
    public static class WPFUtil
    {
        #region Memory Management
        public static void ClearWeakReference(this DependencyObject dependencyObject)
        {
            ///清除Behavior
            foreach (var behavior in Interaction.GetBehaviors(dependencyObject))
            {
                behavior.Detach();
            }

            ///清除Trigger
            foreach (var trigger in Interaction.GetTriggers(dependencyObject))
            {
                trigger.Detach();
            }

            ///清除DataContext
            FrameworkElement element = dependencyObject as FrameworkElement;
            element.DataContext = null;

            ///清除Binding
            BindingOperations.ClearAllBindings(element);

            ///清除CommandBinding
            element.CommandBindings.Clear();

            ///清除Style
            element.Style = null;
        }
        public static void ClearBitmapMemoryLeak(this UIElement element)
        {

            ///Control
            if (element is Control)
            {
                Control control = element as Control;
                if (control.Background != null && control.Background is ImageBrush)
                {
                    ImageSource image = (control.Background as ImageBrush).ImageSource;
                    if (image.CanFreeze)
                        image.Freeze();
                    image = null;
                }
                int childrenCount = VisualTreeHelper.GetChildrenCount(control);
                if (childrenCount != 0)
                {
                    for (int i = 0; i < childrenCount; i++)
                    {
                        (VisualTreeHelper.GetChild(control, i) as UIElement).ClearBitmapMemoryLeak();
                    }
                }
            }
            else if (element is Panel)
            {
                Panel panel = element as Panel;
                if (panel.Background != null && panel.Background is ImageBrush)
                {
                    ImageSource image = (panel.Background as ImageBrush).ImageSource;
                    if (image.CanFreeze)
                        image.Freeze();
                    image = null;
                }
                if (panel.Children.Count != 0)
                {
                    foreach (UIElement item in panel.Children)
                    {
                        item.ClearBitmapMemoryLeak();
                    }
                }
            }
            else if (element is Border)
            {
                Border border = element as Border;
                if (border.Background != null && border.Background is ImageBrush)
                {
                    ImageSource image = (border.Background as ImageBrush).ImageSource;
                    if (image.CanFreeze)
                        image.Freeze();
                    image = null;
                }
                if (border.Child != null)
                {
                    (border.Child as UIElement).ClearBitmapMemoryLeak();
                }
            }
            else if (element is ContentPresenter)
            {
                ContentPresenter presenter = element as ContentPresenter;
                (presenter.Content as UIElement).ClearBitmapMemoryLeak();
            }

        }
        #endregion

        #region Behabior Action
        public static Behavior GetBehavior(this DependencyObject dependencyObject, Type typeofBehavior)
        {
            foreach (var behavior in Interaction.GetBehaviors(dependencyObject))
            {
                if (behavior.GetType().Equals(typeofBehavior))
                {
                    return behavior;
                }
            }
            return null;
        }
        public static void SetBehavior(this DependencyObject dependencyObject, Behavior behavior)
        {
            behavior.Attach(dependencyObject);
        }
        public static void RemoveBehavior(this DependencyObject dependencyObject, Type typeofBehavior)
        {
            Behavior behavior = dependencyObject.GetBehavior(typeofBehavior);
            if (behavior != null)
            {
                behavior.Detach();
            }
        }
        #endregion

        #region HWAcceleration
        public static void EnableHWAcceleration()
        {
            string subKey = @"SOFTWARE\Microsoft\Avalon.Graphics\";
            RegistryKey registryKey =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Avalon.Graphics\", true);
            if (registryKey == null)
            {
                registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(subKey);
            }
            if (registryKey.GetValue("DisableHWAcceleration") == null)
            {
                registryKey.SetValue("DisableHWAcceleration", 0, RegistryValueKind.DWord);
            }

            if (registryKey != null && registryKey.GetValue("DisableHWAcceleration").ToString() == "1")
            {
                registryKey.SetValue("DisableHWAcceleration", 0);
            }
            registryKey.Dispose();
        }
        public static void DisableHWAcceleration()
        {
            RegistryKey registryKey =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Avalon.Graphics\", true);
            if (registryKey != null && registryKey.GetValue("DisableHWAcceleration", null) != null && registryKey.GetValue("DisableHWAcceleration").ToString() == "0")
            {
                registryKey.SetValue("DisableHWAcceleration", 1);
                registryKey.Dispose();
            }

        }
        #endregion

        #region Uri
        public static string FileName(this System.Uri uri, bool hasExtension = true)
        {
            if (uri != null)
            {
                int startIndex = uri.LocalPath.LastIndexOf(@"\") + 1;
                if (hasExtension)
                {
                    return uri.LocalPath.Substring(startIndex);
                }
                else
                {
                    int endIndex = uri.LocalPath.LastIndexOf(@".");
                    return uri.LocalPath.Substring(startIndex, endIndex - startIndex);
                }
            }
            return string.Empty;
        }
        public static string FileName(this string uri, bool hasExtension = true)
        {
            if (uri != null)
            {
                if (Path.HasExtension(uri))
                {
                    int startIndex = uri.LastIndexOf(@"\") + 1;
                    if (hasExtension)
                    {
                        return uri.Substring(startIndex);
                    }
                    else
                    {
                        int endIndex = uri.LastIndexOf(@".");
                        return uri.Substring(startIndex, endIndex - startIndex);
                    }
                }
                else
                {
                    int startIndex = uri.LastIndexOf(@"\") + 1;
                    return uri.Substring(startIndex);
                }
            }
            return string.Empty;
        }
        public static string Extension(this string uri)
        {
            if (!uri.Contains('.'))
                throw new ArgumentException("无法解析此文件!");
            if (!string.IsNullOrEmpty(uri))
            {
                int endIndex = uri.LastIndexOf(@".");
                return uri.Substring(endIndex + 1);
            }
            return string.Empty;
        }
        #endregion

        #region Other
        public static Transform CloneTransform(this Transform transform)
        {
            ScaleTransform transform2 = null;
            RotateTransform transform3 = null;
            SkewTransform transform4 = null;
            TranslateTransform transform5 = null;
            MatrixTransform transform6 = null;
            TransformGroup group = null;
            if (transform == null)
            {
                return null;
            }
            transform.GetType();
            transform2 = transform as ScaleTransform;
            if (transform2 != null)
            {
                return new ScaleTransform { CenterX = transform2.CenterX, CenterY = transform2.CenterY, ScaleX = transform2.ScaleX, ScaleY = transform2.ScaleY };
            }
            transform3 = transform as RotateTransform;
            if (transform3 != null)
            {
                return new RotateTransform { Angle = transform3.Angle, CenterX = transform3.CenterX, CenterY = transform3.CenterY };
            }
            transform4 = transform as SkewTransform;
            if (transform4 != null)
            {
                return new SkewTransform { AngleX = transform4.AngleX, AngleY = transform4.AngleY, CenterX = transform4.CenterX, CenterY = transform4.CenterY };
            }
            transform5 = transform as TranslateTransform;
            if (transform5 != null)
            {
                return new TranslateTransform { X = transform5.X, Y = transform5.Y };
            }
            transform6 = transform as MatrixTransform;
            if (transform6 != null)
            {
                return new MatrixTransform { Matrix = transform6.Matrix };
            }
            group = transform as TransformGroup;
            if (group == null)
            {
                return null;
            }
            TransformGroup group2 = new TransformGroup();
            foreach (Transform transform12 in group.Children)
            {
                group2.Children.Add(CloneTransform(transform12));
            }
            return group2;
        }

        public static BitmapSource StringToBitmapImage(this string bitmapPath)
        {
            try
            {
                Stream bitmapStream = null;
                if (bitmapPath.StartsWith("pack://"))
                {
                    string nameSpace = bitmapPath.Substring(bitmapPath.IndexOf(":,,,/") + ":,,,/".Length);
                    string resourceName = nameSpace.Substring(nameSpace.IndexOf("/") + 1);
                    nameSpace = nameSpace.Substring(0, nameSpace.IndexOf(";"));
                    Assembly resourceAssembly = System.Reflection.Assembly.Load(nameSpace);
                    bitmapStream = resourceAssembly.GetManifestResourceStream((nameSpace + "/" + resourceName).Replace('/', '.'));

                    //System.Resources.ResourceManager resources = new System.Resources.ResourceManager(nameSpace, resourceAssembly);
                    //bitmapStream = resources.GetStream(nameSpace + "/" + resourceName);
                }
                using (BinaryReader reader = new BinaryReader(
                    bitmapStream != null ?
                    bitmapStream :
                    File.Open(bitmapPath.ToString(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {

                    byte[] bytes = reader.ReadBytes((int)reader.BaseStream.Length);
                    reader.Close();
                    try
                    {
                        BitmapFrame bf = BitmapFrame.Create(new MemoryStream(bytes));
                        if (bf.CanFreeze)
                            bf.Freeze();
                        return bf;
                    }
                    catch (Exception e)
                    {
                        //LogRecord.Instance.Log(e.ToString());
                    }
                    finally
                    {
                        if (bitmapStream != null)
                            bitmapStream.Close();
                    }
                    //bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                }
            }
            catch (Exception e)
            {
                //LogRecord.Instance.Log(e.ToString());
            }
            return null;
        }

        public static ImageSource CombineBitmaps(ImageSource mainImage, ImageSource otherImage)
        {
            DrawingImage drawingImage = new DrawingImage();
            ImageDrawing mainImageDrawing = new ImageDrawing()
            {
                ImageSource = mainImage,
                Rect = new Rect(0, 0, mainImage.Width, mainImage.Height)
            };
            ImageDrawing otherImageDrawing = new ImageDrawing()
            {
                ImageSource = otherImage,
                Rect = new Rect(0, 0, otherImage.Width, otherImage.Height)
            };
            drawingImage.Drawing = new DrawingGroup()
            {
                Children = new DrawingCollection { mainImageDrawing, otherImageDrawing }
            };
            RenderOptions.SetCachingHint(drawingImage, CachingHint.Cache);
            RenderOptions.SetBitmapScalingMode(drawingImage, BitmapScalingMode.LowQuality);
            if (drawingImage.CanFreeze)
                drawingImage.Freeze();

            return drawingImage;
        }

        public static void BeginNewUIThread(Delegate action)
        {
            new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(delegate
            {
                Application.Current.Dispatcher.BeginInvoke(action, null);
            })).Start();
        }

        public sealed class GlassFrame
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct MARGINS
            {
                public int cxLeftWidth;      // width of left border that retains its size
                public int cxRightWidth;     // width of right border that retains its size
                public int cyTopHeight;      // height of top border that retains its size
                public int cyBottomHeight;   // height of bottom border that retains its size
            };


            [DllImport("DwmApi.dll")]
            public static extern int DwmExtendFrameIntoClientArea(
                IntPtr hwnd,
                ref MARGINS pMarInset);

            public static void UsingGlassFrame(Window window, int cxLeftWidth = 5, int cxRightWidth = 5, int cyTopHeight = 5, int cyBottomHeight = 5)
            {
                try
                {
                    // Obtain the window handle for WPF application
                    IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
                    HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                    mainWindowSrc.CompositionTarget.BackgroundColor = System.Windows.Media.Color.FromArgb(0, 0, 0, 0);

                    // Get System Dpi
                    System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(mainWindowPtr);
                    float DesktopDpiX = desktop.DpiX;
                    float DesktopDpiY = desktop.DpiY;

                    // Set Margins
                    MARGINS margins = new MARGINS();

                    // Extend glass frame into client area
                    // Note that the default desktop Dpi is 96dpi. The  margins are
                    // adjusted for the system Dpi.
                    margins.cxLeftWidth = Convert.ToInt32(cxLeftWidth * (DesktopDpiX / 96));
                    margins.cxRightWidth = Convert.ToInt32(cxRightWidth * (DesktopDpiX / 96));
                    margins.cyTopHeight = Convert.ToInt32(cyTopHeight * (DesktopDpiX / 96));
                    margins.cyBottomHeight = Convert.ToInt32(cyTopHeight * (DesktopDpiX / 96));

                    int hr = DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
                    //
                    if (hr < 0)
                    {
                        //DwmExtendFrameIntoClientArea Failed
                    }
                }
                // If not Vista, paint background white.
                catch (DllNotFoundException)
                {
                    Application.Current.MainWindow.Background = System.Windows.Media.Brushes.White;
                }

            }
        }
        #endregion
    }

    public class TaskInfo
    {
        public bool IsRequestCancel { get; set; }
        public Task Task { get { return m_Task; } }

        private Task m_Task = null;

        public TaskInfo(Task task)
        {
            m_Task = task;
        }

        public void Start()
        {
            m_Task.Start();
        }
    }

    public static class Win32Util
    {
        public static class ShutdownUtil
        {
            private enum ExitWindows : uint
            {
                EWX_LOGOFF = 0x00,
                EWX_SHUTDOWN = 0x01,
                EWX_REBOOT = 0x02,
                EWX_POWEROFF = 0x08,
                EWX_RESTARTAPPS = 0x40,
                EWX_FORCE = 0x04,
                EWX_FORCEIFHUNG = 0x10,
            }

            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            private static extern bool ExitWindowsEx(ExitWindows uFlags,
                int dwReason);


            public static void Shutdown()
            {
                PrivilegeUtil.AdjustToken(PrivilegeUtil.SE_SHUTDOWN_NAME);
                ExitWindowsEx(ExitWindows.EWX_POWEROFF | ExitWindows.EWX_FORCE, 0);
            }
        }

        public static class PrivilegeUtil
        {
            public const string SE_BACKUP_NAME = "SeBackupPrivilege";
            public const string SE_RESTORE_NAME = "SeRestorePrivilege";
            public const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
            public const string SE_DEBUG_NAME = "SeDebugPrivilege";

            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            private static extern IntPtr GetCurrentProcess();

            [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
            private static extern bool OpenProcessToken(IntPtr ProcessHandle,
                uint DesiredAccess,
                out IntPtr TokenHandle);

            [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true,
                CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            private static extern bool LookupPrivilegeValue(string lpSystemName,
                string lpName,
                out long lpLuid);

            [System.Runtime.InteropServices.StructLayout(
               System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
            private struct TOKEN_PRIVILEGES
            {
                public int PrivilegeCount;
                public long Luid;
                public int Attributes;
            }

            [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
            private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle,
                bool DisableAllPrivileges,
                ref TOKEN_PRIVILEGES NewState,
                int BufferLength,
                IntPtr PreviousState,
                IntPtr ReturnLength);

            public static void AdjustToken(string privilege)
            {
                const uint TOKEN_ADJUST_PRIVILEGES = 0x20;
                const uint TOKEN_QUERY = 0x8;
                const int SE_PRIVILEGE_ENABLED = 0x2;
                //const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

                if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                    return;

                IntPtr procHandle = GetCurrentProcess();

                //取得令牌
                IntPtr tokenHandle;
                OpenProcessToken(procHandle,
                    TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out tokenHandle);
                //取得LUID
                TOKEN_PRIVILEGES tp = new TOKEN_PRIVILEGES();
                tp.Attributes = SE_PRIVILEGE_ENABLED;
                tp.PrivilegeCount = 1;
                LookupPrivilegeValue(null, privilege, out tp.Luid);
                //设定权限
                AdjustTokenPrivileges(
                    tokenHandle, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
            }
        }

        public static class USBUtil
        {
            public const int DBT_DEVICEARRIVAL = 0x8000;  //就是用来表示U盘可用的。一个设备或媒体已被插入一块，现在可用。
            public const int DBT_CONFIGCHANGECANCELED = 0x0019;  //要求更改当前的配置（或取消停靠码头）已被取消。
            public const int DBT_CONFIGCHANGED = 0x0018;  //当前的配置发生了变化，由于码头或取消固定。
            public const int DBT_CUSTOMEVENT = 0x8006; //自定义的事件发生。 的Windows NT 4.0和Windows 95：此值不支持。
            public const int DBT_DEVICEQUERYREMOVE = 0x8001;  //审批要求删除一个设备或媒体作品。任何应用程序也不能否认这一要求，并取消删除。
            public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;  //请求删除一个设备或媒体片已被取消。
            public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;  //一个设备或媒体片已被删除。
            public const int DBT_DEVICEREMOVEPENDING = 0x8003;  //一个设备或媒体一块即将被删除。不能否认的。
            public const int DBT_DEVICETYPESPECIFIC = 0x8005;  //一个设备特定事件发生。
            public const int DBT_DEVNODES_CHANGED = 0x0007;  //一种设备已被添加到或从系统中删除。
            public const int DBT_QUERYCHANGECONFIG = 0x0017;  //许可是要求改变目前的配置（码头或取消固定）。
            public const int DBT_USERDEFINED = 0xFFFF;  //此消息的含义是用户定义的

            public static bool IsUsbArrival
            {
                get
                {
                    DriveInfo[] s = DriveInfo.GetDrives();
                    foreach (DriveInfo drive in s)
                    {
                        if (drive.DriveType == DriveType.Removable)
                        {
                            if (drive.RootDirectory.Exists)
                                return true;
                        }
                    }
                    return false;
                }
            }

            public static DirectoryInfo GetUsbDirectory()
            {
                DriveInfo[] s = DriveInfo.GetDrives();
                foreach (DriveInfo drive in s)
                {

                    if (drive.DriveType == DriveType.Removable)
                    {
                        if (drive.RootDirectory.Exists)
                            return drive.RootDirectory;
                    }
                }
                return null;
            }

            public static void EnableAutoRun()
            {
                RegistryKey rkAutoRun = Registry.GetRegistryKey(Registry.SpecialRegistryKey.Explorer);
                if (rkAutoRun != null)
                {
                    rkAutoRun.SetValue("NoDriveTypeAutoRun", 145);
                }
            }
            public static void DisableAutoRun()
            {
                RegistryKey rkAutoRun = Registry.GetRegistryKey(Registry.SpecialRegistryKey.Explorer);
                if (rkAutoRun != null)
                {
                    rkAutoRun.SetValue("NoDriveTypeAutoRun", 149);
                }
            }
        }

        public static class DeleteFileUtil
        {
            public static void DeleteFileToRecycleBin(string path)
            {
                if (File.Exists(path))
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(path,
           Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
           Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin,
           Microsoft.VisualBasic.FileIO.UICancelOption.ThrowException);
                }
            }
            public static void DeleteFolderToRecycleBin(string path)
            {
                if (Directory.Exists(path))
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(path,
           Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
           Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin,
           Microsoft.VisualBasic.FileIO.UICancelOption.ThrowException);
                }
            }
        }

        public static class FileMappingUtil
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

            [DllImport("kernel32", EntryPoint = "OpenFileMapping")]
            static extern IntPtr OpenFileMapping(int dwDesiredAccess, int bInheritHandle, string lpName);

            [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
            static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);

            [DllImport("Kernel32.dll", EntryPoint = "UnmapViewOfFile", SetLastError = true)]
            static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

            [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
            static extern int CloseHandle(IntPtr hObject);

            public static IntPtr CreateAnMapViewOfFileMapping(string buffer, out IntPtr lpSection, string sectionName = null)
            {
                IntPtr map;
                IntPtr section;
                uint pCount = (uint)buffer.Length;
                section = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, pCount, sectionName);
                map = MapViewOfFile(section, 0xF001F, 0, 0, pCount);
                if (map.ToInt32() > 0)
                {
                    CopyMemory(map, Marshal.StringToHGlobalAnsi(buffer), buffer.Length);
                }
                else
                {
                    int x = Marshal.GetLastWin32Error();
                    throw new InvalidOperationException();
                }
                lpSection = section;
                return map;
            }

            public static string OpenAnMapViewFromMappingName(string sectionName, uint pCount)
            {
                IntPtr section = OpenFileMapping(0xF001F, 0, sectionName);
                IntPtr map = MapViewOfFile(section, 0xF001F, 0, 0, pCount);
                string buffer = Marshal.PtrToStringAnsi(map, (int)pCount);
                return buffer;
            }

            public static bool UnMapView(IntPtr lpBaseAddress, IntPtr lpSectionAddress)
            {
                return UnmapViewOfFile(lpBaseAddress) && Convert.ToBoolean(CloseHandle(lpSectionAddress));
            }
        }
    }

    public static class Registry
    {
        public enum SpecialRegistryKey
        {
            Uninstall,
            Explorer
        }
        public static bool IsX86 { get { return m_IsX86; } }

        static bool m_IsX86 = false;
        static Registry()
        {
            m_IsX86 = System.IntPtr.Size * 8 == 32;
        }

        public static RegistryKey GetRegistryKey(SpecialRegistryKey key)
        {
            switch (key)
            {
                case SpecialRegistryKey.Uninstall:
                    if (m_IsX86)
                    {
                        return Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true);
                    }
                    else
                    {
                        return Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall", true);
                    }
                case SpecialRegistryKey.Explorer:
                    if (m_IsX86)
                    {
                        return Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer", true);
                    }
                    else
                    {
                        return Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Policies\Explorer", true);
                    }
            }
            return null;
        }
    }

    public static class USBUtil
    {
        public const int DBT_DEVICEARRIVAL = 0x8000;  //就是用来表示U盘可用的。一个设备或媒体已被插入一块，现在可用。
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;  //要求更改当前的配置（或取消停靠码头）已被取消。
        public const int DBT_CONFIGCHANGED = 0x0018;  //当前的配置发生了变化，由于码头或取消固定。
        public const int DBT_CUSTOMEVENT = 0x8006; //自定义的事件发生。 的Windows NT 4.0和Windows 95：此值不支持。
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;  //审批要求删除一个设备或媒体作品。任何应用程序也不能否认这一要求，并取消删除。
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;  //请求删除一个设备或媒体片已被取消。
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;  //一个设备或媒体片已被删除。
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;  //一个设备或媒体一块即将被删除。不能否认的。
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;  //一个设备特定事件发生。
        public const int DBT_DEVNODES_CHANGED = 0x0007;  //一种设备已被添加到或从系统中删除。
        public const int DBT_QUERYCHANGECONFIG = 0x0017;  //许可是要求改变目前的配置（码头或取消固定）。
        public const int DBT_USERDEFINED = 0xFFFF;  //此消息的含义是用户定义的

        public static bool IsUsbArrival
        {
            get
            {
                DriveInfo[] s = DriveInfo.GetDrives();
                foreach (DriveInfo drive in s)
                {
                    if (drive.DriveType == DriveType.Removable)
                    {
                        if (drive.RootDirectory.Exists)
                            return true;
                    }
                }
                return false;
            }
        }

        public static DirectoryInfo GetUsbDirectory()
        {
            DriveInfo[] s = DriveInfo.GetDrives();
            foreach (DriveInfo drive in s)
            {

                if (drive.DriveType == DriveType.Removable)
                {
                    if (drive.RootDirectory.Exists)
                        return drive.RootDirectory;
                }
            }
            return null;
        }

        public static void EnableAutoRun()
        {
            RegistryKey rkAutoRun = Registry.GetRegistryKey(Registry.SpecialRegistryKey.Explorer);
            if (rkAutoRun != null)
            {
                rkAutoRun.SetValue("NoDriveTypeAutoRun", 145);
            }
        }
        public static void DisableAutoRun()
        {
            RegistryKey rkAutoRun = Registry.GetRegistryKey(Registry.SpecialRegistryKey.Explorer);
            if (rkAutoRun != null)
            {
                rkAutoRun.SetValue("NoDriveTypeAutoRun", 149);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CopyDataStruct
    {

        public IntPtr dwData;

        public int cbData;//字符串长度

        [MarshalAs(UnmanagedType.LPStr)]

        public string lpData;//字符串

    }

    public static class WindowsMessage
    {
        public const int WM_COPYDATA = 0x004A;
        public const int WM_HOTKEY = 0x0312;
        public const int WM_DEVICECHANGE = 0x219;

        public static void SendMessage(string lpWindowClassName, string lpWindowName, string strMsg)
        {
            if (strMsg == null) return;

            IntPtr hwnd = FindWindow(lpWindowClassName, lpWindowName);
            if (hwnd != IntPtr.Zero)
            {
                CopyDataStruct cds;
                cds.dwData = IntPtr.Zero;
                cds.lpData = strMsg;
                //注意：长度为字节数
                cds.cbData = System.Text.Encoding.Default.GetBytes(strMsg).Length + 1;
                // 消息来源窗体
                int fromWindowHandler = 0;
                SendMessage(hwnd, WM_COPYDATA, fromWindowHandler, ref  cds);
            }

        }



        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage
        (
        IntPtr hWnd,                   //目标窗体句柄
        int Msg,                       //WM_COPYDATA
        int wParam,                                             //自定义数值
        ref  CopyDataStruct lParam             //结构体
        );


    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        /// <summary>         
        /// Specifies the rectangle's width. The units depend on which function uses this.      
        /// /// </summary>     
        public int cx;
        /// <summary>      
        ///  Specifies the rectangle's height. The units depend on which function uses this.         /// 
        ///  </summary>         
        public int cy;
        /// <summary>         
        /// Simple constructor for SIZE structs.         
        /// </summary>         
        /// <param name="cx">The initial width of the SIZE structure.</param>           
        /// <param name="cy">The initial height of the SIZE structure.</param>         
        public SIZE(int cx, int cy) { this.cx = cx; this.cy = cy; }
    }

    [ComImportAttribute(), GuidAttribute("BB2E617C-0920-11d1-9A0B-00C04FC2D6C1"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IExtractImage
    {
        void GetLocation([Out(), MarshalAs(UnmanagedType.LPWStr)]     
        StringBuilder pszPathBuffer, int cch, ref int pdwPriority, ref SIZE prgSize, int dwRecClrDepth, ref int pdwFlags); void Extract(ref IntPtr phBmpThumbnail);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214E6-0000-0000-C000-000000000046")]
    public interface IShellFolder
    {
        void ParseDisplayName(
            IntPtr hwnd,
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName,
            out uint pchEaten,
            out IntPtr ppidl,
            ref uint pdwAttributes);

        [PreserveSig]
        int EnumObjects(IntPtr hWnd, SHCONTF flags, out IntPtr enumIDList);

        void BindToObject(
            IntPtr pidl,
            IntPtr pbc,
            [In()] ref Guid riid,
            out IShellFolder ppv);

        void BindToStorage(
            IntPtr pidl,
            IntPtr pbc,
            [In()] ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppv);

        [PreserveSig()]
        uint CompareIDs(
            int lParam,
            IntPtr pidl1,
            IntPtr pidl2);

        void CreateViewObject(
            IntPtr hwndOwner,
            [In()] ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppv);

        void GetAttributesOf(
            uint cidl,
            [In(), MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl,
           ref SFGAO rgfInOut);

        //[PreserveSig]
        //Int32 GetUIObjectOf(
        //    IntPtr hwndOwner,
        //    uint cidl,
        //    [MarshalAs(UnmanagedType.LPArray)]
        //    IntPtr[] apidl,
        //    Guid riid,
        //    IntPtr rgfReserved,
        //    out IntPtr ppv);
        void GetUIObjectOf(IntPtr hwndOwner, int cidl, ref IntPtr apidl, ref Guid riid, ref int prgfInOut, ref IExtractImage ppvOut);

        void GetDisplayNameOf(
            IntPtr pidl,
            SHGNO uFlags,
            IntPtr lpName);

        IntPtr SetNameOf(
            IntPtr hwnd,
            IntPtr pidl,
            [MarshalAs(UnmanagedType.LPWStr)] string pszName,
           SHGNO uFlags);

    }

    [Flags()]
    public enum SHCONTF
    {
        FOLDERS = 0x20,
        NONFOLDERS = 0x40,
        INCLUDEHIDDEN = 0x80,
        INIT_ON_FIRST_NEXT = 0x100,
        NETPRINTERSRCH = 0x200,
        SHAREABLE = 0x400,
        STORAGE = 0x800
    }

    [Flags()]
    public enum SFGAO
    {
        CANCOPY = 0x1,
        CANMOVE = 0x2,
        CANLINK = 0x4,
        STORAGE = 0x8,
        CANRENAME = 0x10,
        CANDELETE = 0x20,
        HASPROPSHEET = 0x40,
        DROPTARGET = 0x100,
        CAPABILITYMASK = 0x177,
        ENCRYPTED = 0x2000,
        ISSLOW = 0x4000,
        GHOSTED = 0x8000,
        LINK = 0x10000,
        SHARE = 0x20000,
        READONLY = 0x40000,
        HIDDEN = 0x80000,
        DISPLAYATTRMASK = 0xFC000,
        FILESYSANCESTOR = 0x10000000,
        FOLDER = 0x20000000,
        FILESYSTEM = 0x40000000,
        HASSUBFOLDER = unchecked((int)0x80000000),
        CONTENTSMASK = unchecked((int)0x80000000),
        VALIDATE = 0x1000000,
        REMOVABLE = 0x2000000,
        COMPRESSED = 0x4000000,
        BROWSABLE = 0x8000000,
        NONENUMERATED = 0x100000,
        NEWCONTENT = 0x200000,
        CANMONIKER = 0x400000,
        HASSTORAGE = 0x400000,
        STREAM = 0x400000,
        STORAGEANCESTOR = 0x800000,
        STORAGECAPMASK = 0x70C50008
    }

    [Flags()]
    public enum SHGNO
    {
        NORMAL = 0x0,
        INFOLDER = 0x1,
        FOREDITING = 0x1000,
        FORADDRESSBAR = 0x4000,
        FORPARSING = 0x8000,
    }

    [Flags()]
    public enum CSIDL
    {
        ADMINTOOLS = 0x30,
        ALTSTARTUP = 0x1d,
        APPDATA = 0x1a,
        BITBUCKET = 10,
        CDBURN_AREA = 0x3b,
        COMMON_ADMINTOOLS = 0x2f,
        COMMON_ALTSTARTUP = 30,
        COMMON_APPDATA = 0x23,
        COMMON_DESKTOPDIRECTORY = 0x19,
        COMMON_DOCUMENTS = 0x2e,
        COMMON_FAVORITES = 0x1f,
        COMMON_MUSIC = 0x35,
        COMMON_PICTURES = 0x36,
        COMMON_PROGRAMS = 0x17,
        COMMON_STARTMENU = 0x16,
        COMMON_STARTUP = 0x18,
        COMMON_TEMPLATES = 0x2d,
        COMMON_VIDEO = 0x37,
        CONTROLS = 3,
        COOKIES = 0x21,
        DESKTOP = 0,
        DESKTOPDIRECTORY = 0x10,
        DRIVES = 0x11,
        FAVORITES = 6,
        FLAG_CREATE = 0x8000,
        FONTS = 20,
        HISTORY = 0x22,
        INTERNET = 1,
        INTERNET_CACHE = 0x20,
        LOCAL_APPDATA = 0x1c,
        MYDOCUMENTS = 12,
        MYMUSIC = 13,
        MYPICTURES = 0x27,
        MYVIDEO = 14,
        NETHOOD = 0x13,
        NETWORK = 0x12,
        PERSONAL = 5,
        PRINTERS = 4,
        PRINTHOOD = 0x1b,
        PROFILE = 40,
        PROFILES = 0x3e,
        PROGRAM_FILES = 0x26,
        PROGRAM_FILES_COMMON = 0x2b,
        PROGRAMS = 2,
        RECENT = 8,
        SENDTO = 9,
        STARTMENU = 11,
        STARTUP = 7,
        SYSTEM = 0x25,
        TEMPLATES = 0x15,
        WINDOWS = 0x24
    }

    [Flags()]
    public enum SHGFI : uint
    {
        ADDOVERLAYS = 0x20,
        ATTR_SPECIFIED = 0x20000,
        ATTRIBUTES = 0x800,
        DISPLAYNAME = 0x200,
        EXETYPE = 0x2000,
        ICON = 0x100,
        ICONLOCATION = 0x1000,
        LARGEICON = 0,
        LINKOVERLAY = 0x8000,
        OPENICON = 2,
        OVERLAYINDEX = 0x40,
        PIDL = 8,
        SELECTED = 0x10000,
        SHELLICONSIZE = 4,
        SMALLICON = 1,
        SYSICONINDEX = 0x4000,
        TYPENAME = 0x400,
        USEFILEATTRIBUTES = 0x10
    }

    [Flags]
    public enum FILE_ATTRIBUTE
    {
        READONLY = 0x00000001,
        HIDDEN = 0x00000002,
        SYSTEM = 0x00000004,
        DIRECTORY = 0x00000010,
        ARCHIVE = 0x00000020,
        DEVICE = 0x00000040,
        NORMAL = 0x00000080,
        TEMPORARY = 0x00000100,
        SPARSE_FILE = 0x00000200,
        REPARSE_POINT = 0x00000400,
        COMPRESSED = 0x00000800,
        OFFLINE = 0x00001000,
        NOT_CONTENT_INDEXED = 0x00002000,
        ENCRYPTED = 0x00004000
    }

    public enum GetCommandStringInformations
    {
        VERB = 0x00000004,
        HELPTEXT = 0x00000005,
        VALIDATE = 0x00000006,
    }

    [Flags]
    public enum CMF : uint
    {
        NORMAL = 0x00000000,
        DEFAULTONLY = 0x00000001,
        VERBSONLY = 0x00000002,
        EXPLORE = 0x00000004,
        NOVERBS = 0x00000008,
        CANRENAME = 0x00000010,
        NODEFAULT = 0x00000020,
        INCLUDESTATIC = 0x00000040,
        EXTENDEDVERBS = 0x00000100,
        RESERVED = 0xffff0000
    }

    [Flags]
    public enum TPM : uint
    {
        LEFTBUTTON = 0x0000,
        RIGHTBUTTON = 0x0002,
        LEFTALIGN = 0x0000,
        CENTERALIGN = 0x0004,
        RIGHTALIGN = 0x0008,
        TOPALIGN = 0x0000,
        VCENTERALIGN = 0x0010,
        BOTTOMALIGN = 0x0020,
        HORIZONTAL = 0x0000,
        VERTICAL = 0x0040,
        NONOTIFY = 0x0080,
        RETURNCMD = 0x0100,
        RECURSE = 0x0001,
        HORPOSANIMATION = 0x0400,
        HORNEGANIMATION = 0x0800,
        VERPOSANIMATION = 0x1000,
        VERNEGANIMATION = 0x2000,
        NOANIMATION = 0x4000,
        LAYOUTRTL = 0x8000
    }

    public enum EIEIFLAG
    {
        ASYNC = 0x0001, // ask the extractor if it supports ASYNC extract (free threaded)         
        CACHE = 0x0002, // returned from the extractor if it does NOT cache the thumbnail         
        ASPECT = 0x0004, // passed to the extractor to beg it to render to the aspect ratio of the supplied rect         
        OFFLINE = 0x0008, // if the extractor shouldn't hit the net to get any content neede for the rendering         
        GLEAM = 0x0010, // does the image have a gleam ? this will be returned if it does         
        SCREEN = 0x0020, // render as if for the screen (this is exlusive with IEIFLAG_ASPECT )         
        ORIGSIZE = 0x0040, // render to the approx size passed, but crop if neccessary         
        NOSTAMP = 0x0080, // returned from the extractor if it does NOT want an icon stamp on the thumbnail         
        NOBORDER = 0x0100, // returned from the extractor if it does NOT want an a border around the thumbnail         
        QUALITY = 0x0200 // passed to the Extract method to indicate that a slower, higher quality image is desired, re-compute the thumbnail   
    }

    public class API
    {
        #region API 导入

        public const int MAX_PATH = 260;
        public const int S_OK = 0;
        public const int S_FALSE = 1;
        public const uint CMD_FIRST = 1;
        public const uint CMD_LAST = 30000;

        [DllImport("shell32.dll")]
        public static extern Int32 SHGetDesktopFolder(out IntPtr ppshf);

        [DllImport("Shlwapi.Dll", CharSet = CharSet.Auto)]
        public static extern Int32 StrRetToBuf(IntPtr pstr, IntPtr pidl, StringBuilder pszBuf, int cchBuf);

        [DllImport("shell32.dll")]
        public static extern int SHGetSpecialFolderLocation(IntPtr handle, CSIDL nFolder, out IntPtr ppidl);

        [DllImport("shell32",
            EntryPoint = "SHGetFileInfo",
            ExactSpelling = false,
            CharSet = CharSet.Auto,
            SetLastError = true)]
        public static extern IntPtr SHGetFileInfo(
            IntPtr ppidl,
            FILE_ATTRIBUTE dwFileAttributes,
            ref SHFILEINFO sfi,
            int cbFileInfo,
            SHGFI uFlags);

        [DllImport("user32",
            SetLastError = true,
            CharSet = CharSet.Auto)]
        public static extern IntPtr CreatePopupMenu();

        [DllImport("user32.dll",
            ExactSpelling = true,
            CharSet = CharSet.Auto)]
        public static extern uint TrackPopupMenuEx(
            IntPtr hmenu,
            TPM flags,
            int x,
            int y,
            IntPtr hwnd,
            IntPtr lptpm);

        #endregion

        /// <summary>
        /// 获得桌面 Shell
        /// </summary>
        public static IShellFolder GetDesktopFolder(out IntPtr ppshf)
        {
            SHGetDesktopFolder(out ppshf);
            Object obj = Marshal.GetObjectForIUnknown(ppshf);
            return (IShellFolder)obj;
        }

        /// <summary>
        /// 获取显示名称
        /// </summary>
        public static string GetNameByIShell(IShellFolder Root, IntPtr pidlSub)
        {
            IntPtr strr = Marshal.AllocCoTaskMem(MAX_PATH * 2 + 4);
            Marshal.WriteInt32(strr, 0, 0);
            StringBuilder buf = new StringBuilder(MAX_PATH);
            Root.GetDisplayNameOf(pidlSub, SHGNO.INFOLDER, strr);
            API.StrRetToBuf(strr, pidlSub, buf, MAX_PATH);
            return buf.ToString();
        }

        /// <summary>
        /// 根据 PIDL 获取显示名称
        /// </summary>
        public static string GetNameByPIDL(IntPtr pidl)
        {
            SHFILEINFO info = new SHFILEINFO();
            API.SHGetFileInfo(pidl, 0, ref info, Marshal.SizeOf(typeof(SHFILEINFO)),
                SHGFI.PIDL | SHGFI.DISPLAYNAME | SHGFI.TYPENAME);
            return info.szDisplayName;
        }

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public SFGAO dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = API.MAX_PATH)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    public class Guids
    {
        public static Guid IID_DesktopGUID = new Guid("{00021400-0000-0000-C000-000000000046}");

        public static Guid IID_IShellFolder = new Guid("{000214E6-0000-0000-C000-000000000046}");
        public static Guid IID_IContextMenu = new Guid("{000214e4-0000-0000-c000-000000000046}");
        public static Guid IID_IContextMenu2 = new Guid("{000214f4-0000-0000-c000-000000000046}");
        public static Guid IID_IContextMenu3 = new Guid("{bcfce0a0-ec17-11d0-8d10-00a0c90f2719}");

        public static Guid IID_IDropTarget = new Guid("{00000122-0000-0000-C000-000000000046}");
        public static Guid IID_IDataObject = new Guid("{0000010e-0000-0000-C000-000000000046}");

        public static Guid IID_IQueryInfo = new Guid("{00021500-0000-0000-C000-000000000046}");
        public static Guid IID_IPersistFile = new Guid("{0000010b-0000-0000-C000-000000000046}");

        public static Guid CLSID_DragDropHelper = new Guid("{4657278A-411B-11d2-839A-00C04FD918D0}");
        public static Guid CLSID_NewMenu = new Guid("{D969A300-E7FF-11d0-A93B-00A0C90F2719}");
        public static Guid IID_IDragSourceHelper = new Guid("{DE5BF786-477A-11d2-839D-00C04FD918D0}");
        public static Guid IID_IDropTargetHelper = new Guid("{4657278B-411B-11d2-839A-00C04FD918D0}");

        public static Guid IID_IShellExtInit = new Guid("{000214e8-0000-0000-c000-000000000046}");
        public static Guid IID_IExtractImage = new Guid("{BB2E617C-0920-11d1-9A0B-00C04FC2D6C1}");
        public static Guid IID_IStream = new Guid("{0000000c-0000-0000-c000-000000000046}");
        public static Guid IID_IStorage = new Guid("{0000000B-0000-0000-C000-000000000046}");
    }


}
