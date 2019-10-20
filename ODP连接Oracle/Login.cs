using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Oracle.ManagedDataAccess;

namespace ODP连接Oracle
{
    public partial class Login : Form
    {
        public const int USER = 0x500;
        public const int MYMESSAGE = USER + 100;
        public string connect = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)" +
            "(HOST=localhost)" +
            "(PORT=1521))" +
            "(CONNECT_DATA=(SID=oracle)));";

        OracleConnection conn = null;

        public struct DBConnect
        {
            public string ID;
            public string password;
        }

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
            IntPtr hWnd,            // 信息发往的窗口的句柄  
            int Msg,                // 消息ID  
            int wParam,             // 参数1  
            ref DBConnect lParam);  // 传送的类

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        DBConnect dBConnect = new DBConnect();


        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            IntPtr ptr = FindWindow(null, "Form1");//获取接收消息的窗体句柄，
            dBConnect.ID = textBox1.Text;
            dBConnect.password = textBox2.Text;

            connect += "User Id=" + dBConnect.ID + ";Password=" + dBConnect.password + ";";
            try //实例化OracleConnection对象 
            {
                conn = new OracleConnection(connect);
                conn.Open();

                if (conn.State.ToString() == "Open")
                {
                    form1.ShowDialog();
                    SendMessage(ptr, MYMESSAGE, 1, ref dBConnect);//发送消息
                    this.Visible = false;

                    MessageBox.Show("登录成功");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("登录失败");
            }
            finally
            {
                conn.Close();//关闭连接
                conn = null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "system";
            this.textBox2.Text = "123456";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
