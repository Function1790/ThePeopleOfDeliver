using System;
using System.Collections.Generic;
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

namespace 배탈의민족_Client
{
    /// <summary>
    /// CountControler.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CountControler : UserControl
    {
        int count = 0;

        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                int result =  value;
                if (result < 0)
                {
                    result = 0;
                }
                else if(result > 10)
                {
                    result = 10;
                }
                count = result;
                CountView.Text = count + "";
            }
        }

        public CountControler()
        {
            InitializeComponent();
        }

        private void Decrease_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Count = Count - 1;
        }

        private void Increase_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Count = Count + 1;
        }
    }
}
