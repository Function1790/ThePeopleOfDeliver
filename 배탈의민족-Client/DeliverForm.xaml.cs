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
    /// DeliverForm.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DeliverForm : UserControl
    {
        public List<FoodSelector> foodSelectors = new List<FoodSelector>();

        public DeliverForm()
        {
            InitializeComponent();
        }

        public void addMenu(string name, int price)
        {
            //Add Children
            int n = foodSelectors.Count;
            foodSelectors.Add(new FoodSelector());
            MenuGrid.Children.Add(foodSelectors[n]);

            //UI
            foodSelectors[n].SetMenu(name, price);

            //LayOut Setting
            foodSelectors[n].HorizontalAlignment = HorizontalAlignment.Left;
            foodSelectors[n].VerticalAlignment = VerticalAlignment.Top;
            foodSelectors[n].Margin = new Thickness(0, n * (100 + 10) + 10, 0, 0);
            foodSelectors[n].Width = 600;
            foodSelectors[n].Height = 100;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void Rectangle_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void MenuGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int Sum = 0;
            for(int i=0; i<foodSelectors.Count; i++)
            {
                Sum += foodSelectors[i].PayPrice;
            }
            SummaryText.Text=$"합계 : {Sum}\\";
        }
    }
}
