using CSCM.Model;
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

namespace CSCM.userControl.xamlTemplate
{
    /// <summary>
    /// card.xaml 的交互逻辑
    /// </summary>
    public partial class card : UserControl
    {
        public string name;
        public string description;
        public JArray args;
        public card()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        public void clearData()
        {
            cscmName.Header = "";
            cscmDescription.Text = "";
            proertyGrid.Items.Clear();
        }
        public void LoadData()
        {
            //clearData();
            //加载数据
            cscmName.Header = name;
            cscmDescription.Text = description;
            //加载变量 获取到的数据是一个Array
            List<cscmProperty> properties = new List<cscmProperty>();
            foreach (var item in args)
            {
                //解析
                JObject o = item as JObject;
                if (o != null)
                {
                    cscmProperty cp = new cscmProperty();
                    cp.Name = o["name"].ToString();
                    cp.Label = o["label"].ToString();
                    cp.type = o["type"].ToString();
                    cp.DefaultValue = o["defaultValue"].ToString();
                    cp.propertityDirection = o["propertityDirection"].ToString();
                    properties.Add(cp);
                }
            }
            proertyGrid.ItemsSource = properties;
            /*proertyGrid.Columns[0].Header = "参数变量";
            proertyGrid.Columns[1].Header = "参数名称";
            proertyGrid.Columns[2].Header = "参数类型";
            proertyGrid.Columns[3].Header = "默认值";
            proertyGrid.Columns[4].Header = "参数方向";*/
        }
    }
}
