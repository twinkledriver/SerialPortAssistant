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
       

        private SerialPort comm = new SerialPort();
        private long send_count = 0;
        private long receive_count = 0;
        private StringBuilder builder = new StringBuilder();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void load_Loaded(object sender, RoutedEventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            if (ports.Length != 0)
                combo_Choose_serial.Text = ports[0];

                combo_BaudRate.Text= "9600";
                
                combo_Choose_serial.Items.Add(ports[0]);
                combo_Choose_serial.Items.Add(ports[1]);
                combo_BaudRate.Items.Add("9600");
                combo_BaudRate.Items.Add("4800");


               

                combo_Choose_serial.SelectedIndex = combo_Choose_serial.Items.Count > 0 ? 0 : -1;
                combo_BaudRate.SelectedIndex = combo_BaudRate.Items.IndexOf("9600");
            

            comm.NewLine = "/r/n";
            comm.RtsEnable = true;
            comm.DataReceived += comm_DataReceived;
            this.btn_Open.Content = comm.IsOpen ? "关闭端口" : "打开端口";
            this.btn_Send.IsEnabled = comm.IsOpen;
        }


        void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = comm.BytesToRead;
            byte[] buf = new byte[n];
            receive_count += n;
            comm.Read(buf, 0, n);
            builder.Remove(0, builder.Length);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            
            {

                //处理16进制显示
                if ((bool)checkBoxRHex.IsChecked)
                {
                    foreach (byte b in buf)
                    {
                        builder.Append(b.ToString("X2") + " ");
                    }
                }
                //处理ASCII显示
                else
                {
                    builder.Append(Encoding.ASCII.GetString(buf));
                }
                //将接收到的数据追加到文本框末端，并将文本框滚动到末端
                this.Receive_ContentBox.AppendText(builder.ToString());
                //修改接收计数
                this.label_Rec_Count.Content = "接收计数" + receive_count.ToString();

            }));
          //  comm.Write(new byte[] { 0xFF, 0xFF, 0x04, 0x01, 0x01, 0x00, 0x00, 0xF9 }, 0, 8);
  
        }
       

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

        private void Receive_ContentBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((bool)checkBoxAN.IsChecked)
            {
                this.Receive_ContentBox.TextWrapping = TextWrapping.Wrap;
            }
            else
            {
                this.Receive_ContentBox.TextWrapping = TextWrapping.NoWrap;
            }
        }

        private void btn_Send_Click_1(object sender, RoutedEventArgs e)
        {
            int n = 0;

            if ((bool)checkBoxSHex.IsChecked)
            {

                MatchCollection mc = Regex.Matches(Send_ContentBox.Text, @"(?i)[\da-f]{2}");
                List<byte> buf = new List<byte>();
                foreach (Match m in mc)
                {
                    int kk = Convert.ToByte(m.Value, 16);
                    buf.Add(byte.Parse(kk.ToString()));
                }

                ////转换为数组之后发送
                try
                {
                    comm.Write(buf.ToArray(), 0, buf.Count);
                }
                catch
                {
                    return;
                }


                n = buf.Count;  
            }
            else
            {
                if ((bool)checkBox.IsChecked)
                {
                    comm.WriteLine(Send_ContentBox.Text);
                    n = Send_ContentBox.Text.Length + 2;
                }
                else
                {
                    comm.Write(Send_ContentBox.Text);
                    n = Send_ContentBox.Text.Length;  
                    
                }
            }
            send_count+=n;
            label_Send_Count.Content = "发送计数：" + send_count.ToString();
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
          
            send_count = receive_count = 0;
            label_Rec_Count.Content = "接收计数：0";
            label_Send_Count.Content = "发送计数：0";
        }

      

      

      


       
  
    }
}
