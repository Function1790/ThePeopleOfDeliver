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
    /// FoodSelector.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FoodSelector : UserControl
    {
        public int PricePerOne = 1000;

        public FoodSelector()
        {
            InitializeComponent();
        }
        
        public FoodSelector(string foodname, int price)
        {
            InitializeComponent();
            SetMenu(foodname, price);
        }

        public void SetMenu(string foodname, int price)
        {
            MenuName = foodname;
            PricePerOne = price;
        }

        public int foodCount
        {
            get
            {
                return FoodCounter.Count;
            }
        }

        public string MenuName
        {
            get
            {
                return FoodName.Text;
            }
            set
            {
                FoodName.Text = value;
            }
        }

        public int PayPrice
        {
            get
            {
                return foodCount * PricePerOne;
            }
        }

        private void FoodCounter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PriceTextBox.Text = PayPrice + "\\";
        }
    }
}
