using CSCM.DB;
using CSCM.userControl;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;

namespace CSCM
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private static string auth = "";
        private static bool refarchStatus = true;
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
            if (au == null)
            {
                refreshFile.IsEnabled = false;
                errorAuther.Visibility = Visibility.Visible;
                findAuther.Visibility = Visibility.Collapsed;
            }
            else
            {
                auth = au.useName;
                errorAuther.Visibility = Visibility.Collapsed;
                findAuther.Visibility = Visibility.Visible;
                //refreshFile.IsEnabled = false;
                //macId.Text = $"你的MAC地址是:{mac.Replace("-","")},没有登记备注，无法同步你的自建组件，请将你的MAC地址发给 邦桑迪 添加到数据库中后";
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
            if (auth == "")
            {
                this.ShowMessageAsync("无法更新", $"你的这台电脑mac地址没有在数据库内找到记录，所以无法更新你的组件到共享记录中.\n将{GetMacByNetworkInterface()} 发送给邦桑迪");
            }
            else
            {
                reface(auth);
            }
            //refreshFile.IsEnabled = refarchStatus;

        }
        private async void reface(string auth)
        {
            await Task.Run(() =>
            {
                //refreshFile.IsEnabled = false;
                csdpEntities csdp = new csdpEntities();
                using (StreamReader file = File.OpenText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Z-Factory\\db.json"))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(file))
                    {
                        JObject o = (JObject)JToken.ReadFrom(jsonReader);
                        JArray groups = (JArray)o["group"];
                        foreach (JObject group in groups)
                        {
                            var packageId = group["id"].ToString();
                            CSCMPackage package = csdp.CSCMPackage.Find(packageId);
                            if (package == null)
                            {
                                package = new CSCMPackage
                                {
                                    id = packageId,
                                    name = group["name"].ToString(),
                                    version = group["version"].ToString(),
                                    introduction = group["introduction"].ToString(),
                                    description = group["description"].ToString(),
                                    lastModifyTime = group["lastModifyTime"].ToString(),
                                    registry = group["registry"].ToString(),
                                    auth = auth
                                };
                                csdp.CSCMPackage.Add(package);
                                csdp.SaveChanges();
                                Debug.WriteLine($"添加了组件包:{package.name}");
                            }
                            else
                            {
                                package.id = packageId;
                                package.name = group["name"].ToString();
                                package.version = group["version"].ToString();
                                package.introduction = group["introduction"].ToString();
                                package.description = group["description"].ToString();
                                package.lastModifyTime = group["lastModifyTime"].ToString();
                                package.registry = group["registry"].ToString();
                                package.auth = auth;
                                //csdp.CSCMPackage.Add(package);
                                csdp.SaveChanges();
                                //Debug.WriteLine($"更新了组件包:{package.name}");
                            }
                            JObject dependencies = (JObject)group["dependencies"];
                            foreach (var item in dependencies)
                            {

                                string cscmId = item.Value["id"].ToString();
                                CSCMDependencies cSCMDependencies = csdp.CSCMDependencies.Find(cscmId);
                                if (cSCMDependencies == null)
                                {
                                    cSCMDependencies = new CSCMDependencies
                                    {
                                        id = cscmId,
                                        packageId = packageId,
                                        name = item.Value["name"].ToString(),
                                        version = item.Value["version"].ToString(),
                                        type = item.Value["type"].ToString(),
                                        state = item.Value["state"].ToString(),
                                        descriptioin = item.Value["description"].ToString(),
                                        message0 = item.Value["message0"].ToString(),
                                        componentType = item.Value["componentType"].ToString(),
                                        lastModifyTime = item.Value["lastModifyTime"].ToString()
                                    };
                                    csdp.CSCMDependencies.Add(cSCMDependencies);
                                    csdp.SaveChanges();
                                    //Debug.WriteLine($"添加了组件列表:{cSCMDependencies.name}");
                                }
                                else
                                {
                                    // cSCMDependencies = new CSCMDependencies();
                                    cSCMDependencies.id = cscmId;
                                    cSCMDependencies.packageId = packageId;
                                    cSCMDependencies.name = item.Value["name"].ToString();
                                    cSCMDependencies.version = item.Value["version"].ToString();
                                    cSCMDependencies.type = item.Value["type"].ToString();
                                    cSCMDependencies.state = item.Value["state"].ToString();
                                    cSCMDependencies.descriptioin = item.Value["description"].ToString();
                                    cSCMDependencies.message0 = item.Value["message0"].ToString();
                                    cSCMDependencies.componentType = item.Value["componentType"].ToString();
                                    cSCMDependencies.lastModifyTime = item.Value["lastModifyTime"].ToString();
                                    csdp.SaveChanges();
                                    // Debug.WriteLine($"更新了组件列表:{cSCMDependencies.name}");
                                }
                            }

                        }
                    }
                }
                foreach (string folderName in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Z-Factory\\task-component\\components"))
                {
                    foreach (string versionPath in Directory.GetDirectories(folderName))
                    {
                        using (System.IO.StreamReader file = System.IO.File.OpenText(versionPath + "\\taskInfo.json"))
                        {
                            using (JsonTextReader jsonReader = new JsonTextReader(file))
                            {
                                JObject o = (JObject)JToken.ReadFrom(jsonReader);
                                string taskId = o["id"].ToString();
                                string taskVersion = o["component"]["version"].ToString();
                                List<cscmVersion> versionList = csdp.cscmVersion.Where(p => p.taskId == taskId & p.taskVersion == taskVersion).ToList();
                                Debug.WriteLine(versionList.Count);
                                Debug.WriteLine($"taskId:{taskId},taskVersion:{taskVersion}");
                                if (versionList.Count > 0)
                                {
                                    Debug.WriteLine("============");
                                    continue;
                                }
                                else
                                {
                                    cscmVersion version = new cscmVersion
                                    {
                                        id = System.Guid.NewGuid().ToString(),
                                        taskId = taskId,
                                        message0 = o["component"]["definition"]["message0"].ToString(),
                                        args0 = o["component"]["definition"]["args0"].ToString(),
                                        taskVersion = o["taskVersion"].ToString()
                                    };
                                    csdp.cscmVersion.Add(version);
                                    csdp.SaveChanges();
                                    //Debug.WriteLine($"添加了版本信息{version.args0}");

                                }

                            }
                        }
                    }
                }

            });
            refarchStatus = true;
            await this.ShowMessageAsync("同步", "更新完成");
        }

        private void searchData_Click(object sender, RoutedEventArgs e)
        {
            using (csdpEntities csdp = new csdpEntities())
            {
                var dataList = csdp.CSCMPackage.Where(p => p.name.Contains(searchKey.Text)).ToList();
                cscmList.ItemsSource = dataList;
                cscmList.Columns[0].Visibility = Visibility.Collapsed;
                cscmList.Columns[1].Header = "组件包名称";
                cscmList.Columns[2].Header = "组件包版本号";
                cscmList.Columns[3].Header = "组件包简介";
                cscmList.Columns[4].Header = "组件包描述";
                cscmList.Columns[5].Header = "最后修改时间";
                cscmList.Columns[6].Header = "发布渠道";
                cscmList.Columns[7].Header = "作者";
            }

        }

        private void cscmList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
            if(cscmList.SelectedItem == null)
            {

            }
            else
            {
                CSCMPackage package = (CSCMPackage)cscmList.SelectedItem;
                //Debug.WriteLine((cscmList.SelectedItem as CSCMPackage).id);
                FirstFlyout.Header = package.name;

                /*cscmPackageInfo packageInfo = new cscmPackageInfo();
                packageInfo.packageId = package.id;*/
                cscmInfo.packageId = package.id;
                cscmInfo.loadData();
                FirstFlyout.IsOpen = true;


            }
            
        }
    }

}
