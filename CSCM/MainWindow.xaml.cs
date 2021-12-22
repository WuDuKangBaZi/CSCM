using CSCM.DB;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CSCM
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private static string auth;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //无法识别的机器码处理方式
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string mac = GetMacByNetworkInterface();
            csdpEntities csdp = new csdpEntities();
            authority au = csdp.authority.Find(mac.Replace("-", ""));
            if(au == null)
            {
                errorAuther.Visibility = Visibility.Visible;
                findAuther.Visibility = Visibility.Collapsed;
            }
            else
            {
                auth = au.useName;
                errorAuther.Visibility = Visibility.Collapsed;
                findAuther.Visibility = Visibility.Visible;
            }
            

        }
        #region 通过NetworkInterface获取MAC地址
        /// <summary>
        /// 通过NetworkInterface获取MAC地址
        /// </summary>
        /// <returns></returns>
        public static string GetMacByNetworkInterface()
        {
            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface ni in interfaces)
                {
                    return BitConverter.ToString(ni.GetPhysicalAddress().GetAddressBytes());
                }
            }
            catch (Exception)
            {
            }
            return "00-00-00-00-00-00";
        }
        #endregion

        private void refreshFile_Click(object sender, RoutedEventArgs e)
            
        {
            RefreshFile();
        }
        private async static void RefreshFile()
        {
            await Task.Run(() =>
            {
                csdpEntities csdp = new csdpEntities();
                csdp.Configuration.ValidateOnSaveEnabled = false;
                //刷新本地文件
                foreach (string folderName in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Z-Factory\\task-component\\components"))
                {
                    string[] template = folderName.Split('\\');
                    string taskId = template[template.Length - 1];
                    cscm task = csdp.cscm.Find(taskId);
                    if (task == null)
                    {
                        task = new cscm();
                        bool creaTask = true;
                        task.ID = taskId;
                        task.author = auth;
                        foreach (string versionPath in Directory.GetDirectories(folderName))
                        {

                            // 版本路径
                            using (System.IO.StreamReader file = System.IO.File.OpenText(versionPath + "\\taskInfo.json"))
                            {
                                using (JsonTextReader jsonReader = new JsonTextReader(file))
                                {
                                    JObject o = (JObject)JToken.ReadFrom(jsonReader);
                                    if (creaTask)
                                    {
                                        task.Name = o["name"].ToString();
                                        task.Version = o["version"].ToString();
                                        task.description = o["component"]["definition"]["message0"].ToString();
                                        creaTask = false;
                                        csdp.cscm.Add(task);
                                        csdp.SaveChanges();
                                    }
                                    cscmVersion taskVersion = new cscmVersion();
                                    taskVersion.id = System.Guid.NewGuid().ToString();
                                    taskVersion.taskId = taskId;
                                    taskVersion.message0 = o["component"]["definition"]["message0"].ToString();
                                    taskVersion.args0 = o["component"]["definition"]["args0"].ToString();
                                    taskVersion.taskVersion = o["taskVersion"].ToString();
                                    csdp.cscmVersion.Add(taskVersion);
                                    csdp.SaveChanges();

                                }
                            }

                        }

                    }
                    else
                    {
                        continue;
                    }


                }
                MessageBox.Show("刷新完成!");
                csdp.Configuration.ValidateOnSaveEnabled = true;
            });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //搜索刷新
            csdpEntities csdp = new csdpEntities();
            cscmList.DataContext = csdp.cscm;
        }
    }

}
