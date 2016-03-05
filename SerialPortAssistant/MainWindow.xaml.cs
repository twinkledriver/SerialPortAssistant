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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace SerialPortAssistant
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private SerialPort comm = new SerialPort();

       

        private void btn_Open_Click(object sender, RoutedEventArgs e)
        {
            if (combo_Choose_serial.Text.Length == 0)
            { 
                System.Windows.Forms.MessageBox.Show("请输入串口号！");
                    return;
            }
            if (comm.IsOpen)
            {
                comm.Close();
            }
            else
            {
                comm.PortName = combo_Choose_serial.Text;
                try
                {
                    comm.BaudRate = int.Parse(combo_BaudRate.Text);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(" 波特率不正确，将使用默认的波特率：" + comm.BaudRate + "!");
                    combo_BaudRate.Text = comm.BaudRate.ToString();
                }
                try
                {
                    comm.Open();
                }
                catch (Exception ex)
                {
                    comm = new SerialPort();
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }
            }
            this.btn_Open.Content = comm.IsOpen ? "关闭端口" : "打开端口";
            this.btn_Send.IsEnabled = comm.IsOpen;
        }
        private void Receive_ContentBoxChanged(object sender, EventArgs e)
        {

            if (checkBox.IsEnabled)

                this.Receive_ContentBox.TextWrapping=TextWrapping.Wrap;
        }

        private void btn_Send_Click_1(object sender, RoutedEventArgs e)
        {
            int n = 0;

            if (checkBoxSHex.IsEnabled)
            {
                MatchCollection mc = Regex.Matches(Send_ContentBox.Text, @"(?i)[/da-f]{2}");
            }
        }

    

        

       
    }
}
