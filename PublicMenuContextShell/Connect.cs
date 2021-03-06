using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.Linq;
using System.Configuration;
using Mysoft.Map.Extensions.Xml;
using System.Windows.Forms;
namespace PublicMenuContextShell
{

    /// <summary>用于实现外接程序的对象。</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {

        private List<Command> myCommandList=new List<Command>();
        private List<CommandBar> toolBarList=new List<CommandBar>();
 
        /// <summary>
        /// ID是来定位选择文件的
        /// </summary>
        readonly static List<string> s_IdList=new List<string>();
        /// <summary>
        /// 右键菜单的选择名称
        /// </summary>
        readonly static List<string> s_Namelist = new List<string>();

        readonly static string s_addinPath = Path.GetDirectoryName(typeof(Connect).Assembly.Location);

        readonly static List<ToolMode> s_list = null;
        readonly static List<MenuItemDefinition> mdfList = new List<MenuItemDefinition>();

        readonly static string s_updateUrl = "http://10.5.10.48:53126//";
        ///// <summary>
        ///// 项目
        ///// </summary>
        //readonly static MenuItemDefinition s_ProjectContextMenu = null;
        ///// <summary>
        ///// 文件夹
        ///// </summary>
        //readonly static MenuItemDefinition s_FolderContextMenu = null;
        ///// <summary>
        ///// 文件
        ///// </summary>
        //readonly static MenuItemDefinition s_ItemContextMenu = null;
        ///// <summary>
        ///// 站点文件夹
        ///// </summary>
        //readonly static MenuItemDefinition s_WebFolderContextMenu = null;
        ///// <summary>
        ///// website
        ///// </summary>
        //readonly static MenuItemDefinition s_WebProjectFolderContextMenu = null;
        ///// <summary>
        ///// website 文件
        ///// </summary>
        //readonly static MenuItemDefinition s_WebItemFolderContextMenu = null;

        static Connect()
        {



            s_list = XmlHelper.XmlDeserializeFromFile<List<ToolMode>>(Path.Combine(s_addinPath, "config.xml"), Encoding.UTF8);

            foreach (var item in s_list)
            {
                s_IdList.Add(item.Id);
                s_Namelist.Add(item.Name);
                List<string> menuContextTypeList = item.MenuContextType.Split(',').ToList();

                foreach (var type in menuContextTypeList)
                {
                    if (type == "1072")
                    {
                        //s_ItemContextMenu = new MenuItemDefinition("Item", "D309F791-903F-11D0-9EFC-00A0C911004F", Convert.ToInt32(type), item.Id);
                        mdfList.Add(new MenuItemDefinition("Item", "D309F791-903F-11D0-9EFC-00A0C911004F", Convert.ToInt32(type), item.Id));
                    }
                    else if (type == "1138")
                    {
                        //s_WebItemFolderContextMenu = new MenuItemDefinition("Web Item", "D309F791-903F-11D0-9EFC-00A0C911004F", Convert.ToInt32(type), item.Id);
                        mdfList.Add(new MenuItemDefinition("Web Item", "D309F791-903F-11D0-9EFC-00A0C911004F", Convert.ToInt32(type), item.Id));
                    }
                    else if (type == "1026")
                    {
                        //s_ProjectContextMenu = new MenuItemDefinition("Project", "D309F791-903F-11D0-9EFC-00A0C911004F", Convert.ToInt32(type), item.Id);
                        mdfList.Add(new MenuItemDefinition("Project", "D309F791-903F-11D0-9EFC-00A0C911004F", Convert.ToInt32(type), item.Id));
                    }
                    else if (type == "1073")
                    {
                        //s_FolderContextMenu = new MenuItemDefinition("Folder", "D309F791-903F-11D0-9EFC-00A0C911004F", Convert.ToInt32(type), item.Id);
                        mdfList.Add(new MenuItemDefinition("Folder", "D309F791-903F-11D0-9EFC-00A0C911004F", Convert.ToInt32(type), item.Id));
                    }
                    else if (type == "1137")
                    {
                        //s_WebFolderContextMenu = new MenuItemDefinition("Web Folder", "D309F791-903F-11D0-9EFC-00A0C911004F", Convert.ToInt32(type), item.Id);
                        mdfList.Add(new MenuItemDefinition("Web Folder", "D309F791-903F-11D0-9EFC-00A0C911004F", Convert.ToInt32(type), item.Id));
                    }
                    else if (type == "1136")
                    {
                        //s_WebProjectFolderContextMenu = new MenuItemDefinition("Web Project Folder", "D309F791-903F-11D0-9EFC-00A0C911004F", Convert.ToInt32(type), item.Id);
                        mdfList.Add(new MenuItemDefinition("Web Project Folder", "D309F791-903F-11D0-9EFC-00A0C911004F", Convert.ToInt32(type), item.Id));
                    }

                    else if (type == "1043")
                    {
                        //s_WebSolutionFolderContextMenu = new MenuItemDefinition("Web Project Folder", "D309F791-903F-11D0-9EFC-00A0C911004F", Convert.ToInt32(type), item.Id);
                        mdfList.Add(new MenuItemDefinition("Solution", "D309F791-903F-11D0-9EFC-00A0C911004F", Convert.ToInt32(type), item.Id));
                    }
               
               

                }
                
            }
            
        }

        //readonly static MenuItemDefinition s_ProjectContextMenu = new MenuItemDefinition("Project", "D309F791-903F-11D0-9EFC-00A0C911004F", 1026);
        //readonly static MenuItemDefinition s_FolderContextMenu = new MenuItemDefinition("Folder", "D309F791-903F-11D0-9EFC-00A0C911004F", 1073);
        //readonly static MenuItemDefinition s_ItemContextMenu = new MenuItemDefinition("Item", "D309F791-903F-11D0-9EFC-00A0C911004F", 1072);
        //readonly static MenuItemDefinition s_WebFolderContextMenu = new MenuItemDefinition("Web Folder", "D309F791-903F-11D0-9EFC-00A0C911004F", 1137);
        //readonly static MenuItemDefinition s_WebProjectFolderContextMenu = new MenuItemDefinition("Web Project Folder", "D309F791-903F-11D0-9EFC-00A0C911004F", 1136);
        //readonly static MenuItemDefinition s_WebItemFolderContextMenu = new MenuItemDefinition("Web Item", "D309F791-903F-11D0-9EFC-00A0C911004F", 1138);


        /// <summary>实现外接程序对象的构造函数。请将您的初始化代码置于此方法内。</summary>
        public Connect()
        {
        }

        /// <summary>实现 IDTExtensibility2 接口的 OnConnection 方法。接收正在加载外接程序的通知。</summary>
        /// <param term='application'>宿主应用程序的根对象。</param>
        /// <param term='connectMode'>描述外接程序的加载方式。</param>
        /// <param term='addInInst'>表示此外接程序的对象。</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {


            _applicationObject = (DTE2)application;
            _addInInstance = (AddIn)addInInst;
            if (connectMode == ext_ConnectMode.ext_cm_Startup)
            {

                foreach (Command item in myCommandList)
                {
                    if (item != null)
                    {
                        item.Delete();
                    }

                }


                object[] contextGUIDS = new object[] { };
                Commands2 commands = (Commands2)_applicationObject.Commands;
                foreach (var item in s_list)
                {
                    try
                    {
                        Command myCommand = commands.AddNamedCommand2(_addInInstance,
                                                     item.Id, item.Name, item.Name, true, item.IconId, ref contextGUIDS,
                                                     (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled,
                                                     (int)vsCommandStyle.vsCommandStylePictAndText,
                                                      vsCommandControlType.vsCommandControlTypeButton);


                        CommandBars cmdBars = (CommandBars)(_applicationObject.CommandBars);
                        List<MenuItemDefinition> tempList = mdfList.Where(input => input.Identification == item.Id).ToList();
                        foreach (var mid in tempList)
                        {
                            if (mid != null)
                            {
                                CommandLoading(cmdBars, mid, myCommand, item.IsNewest);
                            }

                        }
                        myCommandList.Add(myCommand);

                    }
                    catch (Exception ex)
                    {

                        SafeWriteLog(ex.ToString());
                        continue;
                    }
                }

            }

        }

        Dictionary<int, CommandBarPopup> menuList = new Dictionary<int, CommandBarPopup>();
        /// <summary>
        /// 把命令附加到对应的菜单选项
        /// </summary>
        /// <param name="cmdBars"></param>
        /// <param name="mid"></param>
        /// <param name="cb"></param>
        private void CommandLoading(CommandBars cmdBars, MenuItemDefinition mid, Command comm, bool isNewest)
        {



            
            CommandBar toolBar = FindCommandBar(mid.GuidID, (uint)mid.ID);
            toolBarList.Add(toolBar);



            if (toolBar == null)
            {
                toolBar = cmdBars[mid.Name];
            }

            //cd.AddControl(toolBar, 1);

            CommandBarPopup menu=null;
            if (menuList.Keys.Contains(mid.ID) && isNewest == false)
            {
                CommandBarPopup cbp = menuList[mid.ID] as CommandBarPopup;
                comm.AddControl(cbp.CommandBar, 1);
            }
            else
            {
                if (isNewest == false)
                {
                    menu = toolBar.Controls.Add(MsoControlType.msoControlPopup, Missing.Value, Missing.Value, 1, true) as CommandBarPopup;
                    menu.Caption = "武汉平台工具集合";
                    menu.TooltipText = "武汉平台工具集";
                    menuList.Add(mid.ID, menu);

                    comm.AddControl(menu.CommandBar, 1);
                }
                else
                    comm.AddControl(toolBar, 1);
             
                
            }


   



     

        }



        /// <summary>
        /// 该问题参考
        /// http://stackoverflow.com/questions/8880251/command-bar-control-wont-show-up
        /// http://searchcode.com/codesearch/view/10562318
        /// </summary>
        /// <param name="guidCmdGroup"></param>
        /// <param name="menuID"></param>
        /// <returns></returns>
        private CommandBar FindCommandBar(Guid guidCmdGroup, uint menuID)
        {

            IOleServiceProvider sp = (IOleServiceProvider)_applicationObject;
            Guid guidSvc = typeof(IVsProfferCommands).GUID;
            Object objService;
            sp.QueryService(ref guidSvc, ref guidSvc, out objService);
            IVsProfferCommands vsProfferCmds = (IVsProfferCommands)objService;
            return vsProfferCmds.FindCommandBar(IntPtr.Zero, ref guidCmdGroup, menuID) as CommandBar;
        }

        [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IOleServiceProvider
        {

            [PreserveSig]
            int QueryService([In]ref Guid guidService, [In]ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out System.Object obj);

        }

        /// <summary>实现 IDTExtensibility2 接口的 OnDisconnection 方法。接收正在卸载外接程序的通知。</summary>
        /// <param term='disconnectMode'>描述外接程序的卸载方式。</param>
        /// <param term='custom'>特定于宿主应用程序的参数数组。</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {

            foreach (Command item in myCommandList)
            {
                if (item != null)
                {
                    item.Delete();
                }
               
            }

        }

        /// <summary>实现 IDTExtensibility2 接口的 OnAddInsUpdate 方法。当外接程序集合已发生更改时接收通知。</summary>
        /// <param term='custom'>特定于宿主应用程序的参数数组。</param>
        /// <seealso class='IDTExtensibility2' />		
        public void OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>实现 IDTExtensibility2 接口的 OnStartupComplete 方法。接收宿主应用程序已完成加载的通知。</summary>
        /// <param term='custom'>特定于宿主应用程序的参数数组。</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom)
        {
        }

        /// <summary>实现 IDTExtensibility2 接口的 OnBeginShutdown 方法。接收正在卸载宿主应用程序的通知。</summary>
        /// <param term='custom'>特定于宿主应用程序的参数数组。</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom)
        {
        }

        private DTE2 _applicationObject;
        private AddIn _addInInstance;
        #region IDTCommandTarget 成员
        public void Exec(string CmdName, vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            Handled = false;

            if (ExecuteOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
            {
                
                if (s_IdList.Any(input => CmdName == _addInInstance.ProgID+"."+input))
                {
                    try
                    {
                        List<ToolMode> tmList = s_list.Where(input => input.Id == CmdName.Split('.')[2]).ToList();
                        ProjectPath project = SelectedPath();

                        //if (tmList[0].Suffix != null && File.Exists(project.SelectPath))
                        //{
                        //    if (tmList[0].Suffix.Split(',').Any(input => input != Path.GetExtension(project.SelectPath)))
                        //    {
                        //        MessageBox.Show("文件类型不被该工具支持");
                        //        Handled = false;
                        //        return;
                        //    }
                        //}
                        string assemblyName = tmList[0].AddinFile;
                        string filePath = Path.Combine(s_addinPath, assemblyName);

                        if (File.Exists(filePath))
                        {
                            CheckUpdate();
                            Type t = Assembly.LoadFrom(filePath).GetType(tmList[0].Type);
                            if (t != null)
                            {
                                Func<string, object>myFunc= _applicationObject.GetObject;
                                Dictionary<string, object> paramters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                                paramters.Add("slnPath", project.SolutionFile);
                                paramters.Add("selectPath", project.SelectPath);
                                paramters.Add("projectPath", project.SelectProjectPath);
                                paramters.Add("dte", _applicationObject);
                                paramters.Add("getObject", myFunc);
                                paramters.Add("refresh", new Action<List<string>>(Refresh));
                                t.GetMethod(tmList[0].AssemblyMethod).Invoke(null, new object[] { paramters});
                            }

                        }
                        else
                        {
                            MessageBox.Show(filePath+"   文件不存在!");
                        }


                    }
                    catch (Exception ex)
                    {
                        SafeWriteLog(ex.ToString());
                    }
                }

            }

            Handled = true;
        }

        public void QueryStatus(string CmdName, vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            if (NeededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {

                if (s_IdList.Any(input => CmdName == _addInInstance.ProgID + "." + input))
                {
                    StatusOption = vsCommandStatus.vsCommandStatusInvisible;

                    List<ToolMode> tmList = s_list.Where(input => input.Id == CmdName.Split('.')[2]).ToList();
                    ProjectPath project = SelectedPath();

                    if (tmList[0].Suffix != null && File.Exists(project.SelectPath))
                    {
                        if (tmList[0].Suffix.Split(',').Any(input => input != Path.GetExtension(project.SelectPath)))
                        {
                            StatusOption = vsCommandStatus.vsCommandStatusInvisible;

                        }
                        else
                        {
                            StatusOption = (vsCommandStatus)(vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported);
                        }
                    }
                    else
                    {
                        StatusOption = (vsCommandStatus)(vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported);
                    }

                }
                else
                {
                    StatusOption = vsCommandStatus.vsCommandStatusInvisible;
                }

                //if (s_IdList.Any(input => CmdName == _addInInstance.ProgID + "." + input))
                //{
                //    StatusOption = vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported;
                //}
                //else
                //{
                //    StatusOption = vsCommandStatus.vsCommandStatusUnsupported;
                //}
             
            }
        }
        /// <summary>
        /// 选中的路径
        /// http://social.msdn.microsoft.com/Forums/vstudio/en-US/2b0cf6d8-d0f4-498b-97ec-d6599800af2c/refresh-solution-explorer?forum=vsx
        /// </summary>
        /// <returns></returns>
        private ProjectPath SelectedPath()
        {
            if (_applicationObject.Solution == null || _applicationObject.Solution.Projects == null || _applicationObject.Solution.Projects.Count < 1)
                return null;

            SelectedItems items = _applicationObject.SelectedItems;
            if (_applicationObject.SelectedItems.Count > 0)
            {
                ProjectPath projectPath = new ProjectPath();
                projectPath.SolutionFile = _applicationObject.Solution.FullName;
                SelectedItem si = items.Item(1);

                if (si.Project != null || (si.ProjectItem!=null&&si.ProjectItem.ProjectItems != null))
                {
                    Project chooseProject = si.Project != null ? si.Project : si.ProjectItem.ProjectItems.ContainingProject;
                    projectPath.SelectProjectPath = chooseProject.FullName;
                }

                if (si.Project != null)
                {
                    projectPath.SelectPath = Path.GetDirectoryName(si.Project.FullName);
                    return projectPath;
                }
                if (si.ProjectItem != null)
                {
                    ProjectItem item = si.ProjectItem;
                    string path = item.get_FileNames(1);
                    projectPath.SelectPath = path;
                }
                return projectPath;
            }

            return null;
        }

        /// <summary>
        /// http://social.msdn.microsoft.com/Forums/vstudio/en-US/25fc9eab-32dc-488f-9770-e457ec0a796d/vs2005-addin-include-an-existing-file-in-a-project?forum=vsx
        /// </summary>
        /// <param name="path"></param>
        private void Refresh(List<string> paths)
        {
            List<string> tempPaths = paths.ToList();

            var aspxFileList = tempPaths.Where(input => string.IsNullOrEmpty(input) == false && input.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase)).ToList();

            List<string> removeList = new List<string>();
            foreach (var item in aspxFileList)
            {

                //VB
                var disignerPage = item + ".designer.vb";
                //var index = tempPaths.FindIndex(input => StringComparer.CurrentCultureIgnoreCase.Equals(input, disignerPage));
                //if( index > -1 ) {
                //    tempPaths.RemoveAt(index);
                //}
                var vbPage = item + ".vb";
                removeList.Add(disignerPage);
                removeList.Add(vbPage);


                //C#
                disignerPage = item + ".designer.cs";

                vbPage = item + ".cs";
                removeList.Add(disignerPage);
                removeList.Add(vbPage);

            }
            tempPaths = tempPaths.Except(removeList, StringComparer.CurrentCultureIgnoreCase).ToList();
            //for( int i = 0; i < paths.Count; i++ ) {
            //    if( Path.GetExtension(paths[i]) == "aspx" ) {
            //        string className = Path.GetFileName(paths[i]) + ".vb";
            //        string designerName = Path.GetFileName(paths[i]) + ".designer.vb";
            //        paths.Remove(Path.Combine(Path.GetDirectoryName(paths[i]), className));
            //        paths.Remove(Path.Combine(Path.GetDirectoryName(paths[i]), designerName));
            //    }
            //}
            foreach (var p in tempPaths)
            {
                if (File.Exists(p))
                {
                    //Projects projectList = _applicationObject.Solution.Projects;
                    SelectedItem item = _applicationObject.SelectedItems.Item(1);
                    Project chooseProject = item.Project != null ? item.Project : item.ProjectItem.ProjectItems.ContainingProject;
                    ProjectItems items = item.Project != null ? chooseProject.ProjectItems : item.ProjectItem.ProjectItems;

                    items.AddFromFileCopy(p);
                }
            }

        }
        #endregion




        /// <summary>
        /// 获得工具壳所在路径
        /// </summary>
        public static string ShellPath
        {
            get
            {
                return Path.GetDirectoryName(typeof(Connect).Assembly.Location);
            }
        }

        /// <summary>
        /// 安全的写入日志信息
        /// </summary>
        /// <param name="message">日志信息</param>
        public static void SafeWriteLog(string message)
        {
            try
            {
                string filePath = string.Format("{0}.log", DateTime.Now.ToString("yyyy-MM-dd"));

                filePath = Path.Combine(ShellPath, filePath);

                message = string.Format("\r\n-------------------{0}-------------------\r\n{1}", DateTime.Now.ToString("HH:mm:ss"), message);

                File.AppendAllText(filePath, message);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 检查更新情况
        /// </summary>
        /// <param name="tm"></param>
        public void CheckUpdate()
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(s_updateUrl + "config.xml");
                List<ToolMode> tempList = XmlHelper.XmlDeserialize<List<ToolMode>>(doc.OuterXml, Encoding.UTF8);
                List<ToolMode> tmList = XmlHelper.XmlDeserializeFromFile<List<ToolMode>>(Path.Combine(s_addinPath, "config.xml"), Encoding.UTF8);
                bool isUpdate = false;

                //新增节点需要更新
                if (tempList.Count != tmList.Count)
                    isUpdate = true;
                foreach (var item in tmList)
                {
                    ToolMode Newtm = tempList.Where(input => input.Id == item.Id).ToList()[0];
                    if (new Version(Newtm.Version) > new Version(item.Version))
                    {
                        isUpdate = true;
                    }
                }
                if (isUpdate)
                {
                    FrmDownLoad fdl = new FrmDownLoad(s_updateUrl + "update.zip");
                    fdl.ShowDialog();
                    MessageBox.Show("下次启动VS会运用最新版本");

                }
            }
            catch(Exception ex)
            {
                SafeWriteLog(ex.ToString());
            }
        }


    }

    /// <summary>
    /// 项目路径类
    /// </summary>
    public class ProjectPath
    {
        /// <summary>
        /// 表示选择的解决方案中的文件夹
        /// </summary>
        public string SolutionFile { get; set; }
        /// <summary>
        /// 表示sln文件路径
        /// </summary>
        public string SelectPath { get; set; }

        /// <summary>
        /// 选中的文件所在的project或者是你选择的project
        /// </summary>
        public string SelectProjectPath { get; set; }
    }

    public class MenuItemDefinition
    {
        public string Name = string.Empty;
        public Guid GuidID = Guid.Empty;
        public int ID = 0;

        public string Identification = string.Empty;

        public MenuItemDefinition(string name, string guidid, int id,string  identiId)
        {
            this.Name = name;
            this.GuidID = new Guid(guidid);
            this.ID = id;
            this.Identification = identiId;
        }
    }



    public class ToolMode
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Type { get; set; }
        [XmlAttribute]
        public string MenuContextType { get; set; }
        [XmlAttribute]
        public string AssemblyMethod { get; set; }

        [XmlAttribute]
        public string AddinFile { get; set; }

        [XmlAttribute]
        public bool IsNewest { get; set; }

        [XmlAttribute]
        public string Suffix { get; set; }
        [XmlAttribute]
        public string Version { get; set; }

        [XmlAttribute]
        public int IconId { get; set; }
    }
}
