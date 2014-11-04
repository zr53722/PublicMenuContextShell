using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Net.Security;

namespace PublicMenuContextShell
{
    public partial class FrmDownLoad : Form
    {
        public FrmDownLoad()
        {
            InitializeComponent();
        }
        private string Url = "";
        private string ProcessName = "";
        private const int BufferSize = 4018;
        private SynchronizationContext _context;
        private string _addinPath = Path.GetDirectoryName(typeof(FrmDownLoad).Assembly.Location);


        public FrmDownLoad(string url)
        {
            InitializeComponent();
            Url = url;
            _context = SynchronizationContext.Current;
           
        }


        protected override void OnLoad(EventArgs e)
        {
            try
            {
                Download(Url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Url);
                this.Close();
            }

            base.OnLoad(e);
        }

        private void Download(string url)
        {


            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Timeout = 10000;    //10s超时
            //request.Credentials = CredentialCache.DefaultCredentials;
            if (request == null) return;

            DownloadParams dp = new DownloadParams();
            dp.Request = request;
            dp.DestFilePath = Path.Combine(_addinPath, "temp.zip");

            request.BeginGetResponse(AsyncCallBack, dp);
        }

        private void AsyncCallBack(IAsyncResult ar)
        {
            try
            {
                if (ar.IsCompleted)
                {
                    DownloadParams dp = ar.AsyncState as DownloadParams;
                    HttpWebResponse response = dp.Request.EndGetResponse(ar) as HttpWebResponse;
                    Stream stream = response.GetResponseStream();

                    int length = (int)response.ContentLength;

                    byte[] buffer = new byte[BufferSize];
                    int count = 0;
                    using (FileStream filestream = new FileStream(dp.DestFilePath ?? "temp.zip", FileMode.Create))
                    {
                        int downloaded = 0;
                        while (true)
                        {
                            count = stream.Read(buffer, 0, BufferSize);
                            downloaded += count;

                            if (count > 0)
                            {
                                //notice
                                filestream.Write(buffer, 0, count);
                                _context.Post(d => { this.lb_progress.Text = d + "%"; }, downloaded * 100 / length);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    response.Close();

                    //解压到临时目录
                    string tempUpdatePath = Path.Combine(Path.GetTempPath(), "PublicMenuContextShell");
                    ZipHelper.UnZip(dp.DestFilePath, tempUpdatePath);


                    //先删除
                    List<string> tempList = Directory.GetFiles(_addinPath, "*.bak", SearchOption.AllDirectories).ToList();
					foreach( string item in tempList ) {
						if( File.Exists(item) ) {
							try {
								File.Delete(item);
							}
							catch { }
						}

					}
                    //改名
                    List<string> newDllList = Directory.GetFiles(tempUpdatePath,"*.*",SearchOption.AllDirectories).Where(input => Path.GetExtension(input) == ".dll" || Path.GetExtension(input) == ".exe").ToList();
                    List<string> oldDllList = Directory.GetFiles(_addinPath, "*.*", SearchOption.AllDirectories).Where(input => Path.GetExtension(input) == ".dll" || Path.GetExtension(input) == ".exe").ToList();

                    foreach (var newDll in newDllList)
                    {
                        foreach (var item in oldDllList)
                        {
                            string fileNewName = Directory.GetParent(newDll).Name + "\\" + Path.GetFileName(newDll);
                            string itemName = Directory.GetParent(item).Name + "\\" + Path.GetFileName(item);
                            if (fileNewName == itemName&&Path.GetFileName(newDll) != "ICSharpCode.SharpZipLib.dll")
                            {
                                    FileInfo file = new FileInfo(item);
                                    file.MoveTo(item + "." + Guid.NewGuid().ToString().Replace("-", "") + ".bak");
                                
                            }
                        }

                        //if (newDllList.Any(input => - Path.GetFileName(input) == Path.GetFileName(item)) && Path.GetFileName(item) != "ICSharpCode.SharpZipLib.dll")
                        //{
                        //    FileInfo file = new FileInfo(item);
                        //    file.MoveTo(item + "."+Guid.NewGuid().ToString().Replace("-", "") + ".bak");
                        //}
                    }
                    ZipHelper.UnZip(dp.DestFilePath, _addinPath);
                    File.Delete(dp.DestFilePath);
                    Directory.Delete(tempUpdatePath, true);
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Url);
            }
            finally
            {
                _context.Post(d => { this.Close(); }, null);
               
            }
        }

        internal class DownloadParams
        {
            public HttpWebRequest Request { get; set; }

            public string DestFilePath { get; set; }
        }
    }

    public static class ZipHelper
    {
        private const int BufferSize = 4096;

        /// <summary>
        /// ZIP解压缩
        /// </summary>
        /// <param name="filepath">压缩包路径</param>
        /// <param name="directory">解压目录</param>
        public static string UnZip(string filepath, string directory)
        {
            string unzipDirectory = directory;

            if (!File.Exists(filepath))
                return null;

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(filepath)))
            {
                byte[] buffer = new byte[BufferSize];
                int size = 0;


                ZipEntry entry;
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                bool isRoot = true;
                while ((entry = s.GetNextEntry()) != null)
                {
                    string path = Path.Combine(directory, entry.Name);
                    if (entry.IsDirectory)
                    {
                        var info = Directory.CreateDirectory(path);
                        if (isRoot)
                        {
                            unzipDirectory = info.FullName;
                        }
                    }
                    else if (entry.IsFile)
                    {
                        string fullname = Path.Combine(directory, entry.Name);
                        if (File.Exists(fullname))
                        {
                            File.Delete(fullname);
                        }

                        using (FileStream fs = new FileStream(fullname, FileMode.CreateNew))
                        {
                            while (true)
                            {
                                size = s.Read(buffer, 0, BufferSize);
                                if (size > 0)
                                {
                                    fs.Write(buffer, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    isRoot = false;
                }
                return unzipDirectory;
            }
        }
    }
}
