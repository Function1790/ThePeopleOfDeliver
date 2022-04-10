using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace 배탈의민족_Client
{
    //ICON COLOR : #FF384A57

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        //Constant
        const int Type_Hamburger = 0;
        const int Type_Pizza = 1;
        const int PORT = 1089;

        //Variable
        List<FoodSelector[]> Markets = new List<FoodSelector[]>();
        string uid = "Guest";
        int last_open_type = 0;

        //Function
        void Main()
        {
            LoadGrid.Visibility = Visibility.Visible;
            Connect_To_Deliver_Server();
            SetMarkets();
            LoadGrid.Visibility = Visibility.Hidden;
        }

        void Connect_To_Deliver_Server()
        {
            //Set Socketand Connect to Server
            setSocket();
            //Finish Loading
            Opacity_Animation(LoadGrid, 0, 0.5);
        }

        bool Execute_Command(string text)
        {
            if (text == "")
            {
                return false;
            }
            JObject json = JObject.Parse(text);
            string _command = json["Command"].ToString();
            string _arg = json["Arg"].ToString();
            switch (_command)
            {
                case "login":
                    if (_arg == "true")
                    {
                        uid_Text.Text = Input_ID.Text;
                        uid = Input_ID.Text;
                        LoginGrid.Visibility = Visibility.Hidden;
                    }
                    break;
            }
            return true;
        }


        //Opacity Animaion
        static void Opacity_Animation(FrameworkElement ctr, double opacity_to, double duration_from_sec)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = ctr.Opacity;
            animation.To = opacity_to;
            animation.Duration = new Duration(TimeSpan.FromSeconds(duration_from_sec));
            ctr.BeginAnimation(OpacityProperty, animation);
        }

        //Set Menu
        void SetMarkets()
        {
            Markets.Add(new FoodSelector[5]);
            Markets[Type_Hamburger][0] = new FoodSelector("불고기 버거", 3500);
            Markets[Type_Hamburger][1] = new FoodSelector("세슘 버거", 3700);
            Markets[Type_Hamburger][2] = new FoodSelector("황산수소 버거", 4000);
            Markets[Type_Hamburger][3] = new FoodSelector("우라늄 버거", 9700);
            Markets[Type_Hamburger][4] = new FoodSelector("우라늄 디럭스 버거", 10400);

            Markets.Add(new FoodSelector[4]);
            Markets[Type_Pizza][0] = new FoodSelector("불고기 피자", 12000);
            Markets[Type_Pizza][1] = new FoodSelector("스테이크 피자", 13200);
            Markets[Type_Pizza][2] = new FoodSelector("고르곤 졸라 피자", 9000);
            Markets[Type_Pizza][3] = new FoodSelector("고구마 피자", 12500);

            DeliverRequest.payBtn.MouseDown += new MouseButtonEventHandler(Pay_MouseDown);
        }

        //Open Deliver Menu
        void OpenDeliver(int type)
        {
            last_open_type = type;
            DeliverRequest.Visibility = Visibility.Visible;
            DeliverRequest.MenuGrid.Children.Clear();
            DeliverRequest.foodSelectors.Clear();
            for (int i = 0; i < Markets[type].Length; i++) 
            {
                DeliverRequest.addMenu(Markets[type][i].MenuName, Markets[type][i].PricePerOne);
            }
        }

        string DeliverDump()
        {
            string menus = "";
            int _count = 0;
            for (int i = 0; i < DeliverRequest.foodSelectors.Count; i++)
            {
                _count = DeliverRequest.foodSelectors[i].foodCount;
                if (_count > 0)
                {
                    menus += $"\"{DeliverRequest.foodSelectors[i].FoodName.Text}\":{_count},";
                }
            }
            if (menus == "")
            {
                return null;
            }
            string result = "";
            for (int i = 0; i < menus.Length - 1; i++)
            {
                result += menus[i];
            }
            return "{" + result + "}";
        }

        bool Deliver_Requst()
        {
            string menus = DeliverDump();
            if (menus == null)
            {
                return false;
            }
            string market_name = "";
            if (last_open_type == 0)
            {
                market_name = "Hamburger";
            }
            else if (last_open_type == 1)
            {
                market_name = "Pizza";
            }

            string _arg = $"\"market_name\": \"{market_name}\", \"menu\": {menus}";
            Send(DumpCommand("deliver_insert", DumpPY(_arg)));
            return true;
        }

        //SET Socket
        static Socket Client_Socket;
        Thread Receive_Thread;

        static void Log(string title, object content)
        {
            Console.WriteLine($"[{title}] >> {content}");
        }

        static void Send(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            Client_Socket.Send(data);
            Log("Send", text);
        }

        void Recv_thread()
        {
            while (true)
            {
                byte[] Buffer = new byte[Client_Socket.SendBufferSize];
                int bytesRead = Client_Socket.Receive(Buffer);
                byte[] formatted = new byte[bytesRead];
                for (int i = 0; i < bytesRead; ++i)
                {
                    formatted[i] = Buffer[i];
                }
                string text = Encoding.UTF8.GetString(formatted);
                Log("Get", text);
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { Execute_Command(text); })); 
            }
        }

        void setSocket()
        {
            Client_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), PORT);

            try
            {
                Client_Socket.Connect(localEndPoint);
                Log("PORT", PORT);
            }
            catch
            {
                Log("Connect Failed", "재시도..");
                setSocket();
            }

            Receive_Thread = new Thread(Recv_thread);
            Receive_Thread.Start();
        }

        //
        string DumpCommand(string cmd, string arg)
        {
            return "{"+$"\"Command\":\"{cmd}\",\"Arg\":{arg}" + "}";
        }

        string DumpPY(string text)
        {
            return "{" + text + "}";
        }

        //Event Function
        public MainWindow()
        {
            InitializeComponent();
        }

        //Entry
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Main();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //TopGrid Mouse Down
            DragMove();
        }

        //Opacity Animation
        private void Opactiy_MouseEnter(object sender, MouseEventArgs e)
        {
            Opacity_Animation(sender as FrameworkElement, 1, 0.1);
        }

        private void Opactiy_MouseLeave(object sender, MouseEventArgs e)
        {
            Opacity_Animation(sender as FrameworkElement, 0.5, 0.1);
            
        }


        //Select Hamburger
        private void IconBtn_Hamburger_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenDeliver(Type_Hamburger);
        }

        //Select Pizza
        private void IconBtn_Pizza_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenDeliver(Type_Pizza);
        }
        
        private void Pay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Deliver_Requst();
            DeliverRequest.Visibility = Visibility.Hidden;
        }

        //Open Login Page
        private void LoginBtnGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginGrid.Visibility = Visibility.Visible;
        }

        //Login
        private void LoginBtnGrid_Copy_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string arg = DumpPY($"\"uid\":\"{Input_ID.Text}\",\"upw\":\"{Input_PW.Text}\"");
            Send(DumpCommand("login",arg));
        }

        //Close Login Page
        private void Close_LoginGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginGrid.Visibility = Visibility.Hidden;
        }

        //Close Program
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Client_Socket.Close();
            Close();
        }
    }
}
