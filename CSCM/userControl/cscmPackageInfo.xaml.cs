using CSCM.DB;
using CSCM.userControl.xamlTemplate;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace CSCM.userControl
{
    /// <summary>
    /// cscmPackageInfo.xaml 的交互逻辑
    /// </summary>
    public partial class cscmPackageInfo : UserControl
    {
        public string packageId;
        public cscmPackageInfo()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("cscmPackageInfo....");
           
           

            
        }
        public void loadData()
        {
            Debug.WriteLine("=======loa infoData=========");
            cscmInfoStack.Children.Clear();
            using (csdpEntities csdp = new csdpEntities())
            {
                List<CSCMDependencies> dependenciesList = csdp.CSCMDependencies.Where(p => p.packageId == packageId).ToList();
                foreach (CSCMDependencies dependency in dependenciesList)
                {
                    card ca = new card();
                    ca.name = dependency.name;
                    ca.description = dependency.message0;
                    cscmVersion version = (cscmVersion)csdp.cscmVersion.Where(v => v.taskId == dependency.id).OrderByDescending(v =>v.taskVersion).First();

                    //JArray array = JArray.FromObject(version.args0.ToString());
                    //Debug.WriteLine(version.args0);
                    //Debug.WriteLine(version.args0.GetType());
                    //Debug.WriteLine(JsonConvert.DeserializeObject(version.args0));
                     ca.args = (JArray)JsonConvert.DeserializeObject(version.args0);
                    ca.LoadData();
                    cscmInfoStack.Children.Add(ca);


                }
            }
        }
    }
}
