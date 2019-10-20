using Oracle.ManagedDataAccess.Client;
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

namespace ODP连接Oracle
{
    public partial class Search : Form
    {
        static string conString =
            "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)" +
            "(HOST=localhost)" +
            "(PORT=1521))" +
            "(CONNECT_DATA=(SID=oracle)));"+
            "User Id=system;" +
            "Password=123456;";
        public string sql;
        public string table = "monsters";
        public string selectName;
        public const int USER = 0x500;
        public const int MYMESSAGE = USER + 1;
        public struct My_lParam
        {
            public string value;
            public string valueName;
        }
        //消息发送API  

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
            IntPtr hWnd,        // 信息发往的窗口的句柄  
            int Msg,            // 消息ID  
            int wParam,         // 参数1  
            ref My_lParam lParam);


        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        My_lParam m;
        public Search()
        {
            InitializeComponent();
            m = new My_lParam();
            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Search_Load(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        public void SendConnectEvent(string sql)
        {
            OracleConnection conn = null;
            OracleCommand comm = null;
            try //实例化OracleConnection对象 
            {
                
                conn = new OracleConnection(conString);
                conn.Open(); //开启数据库连接
                comm = new OracleCommand(sql, conn);//事物处理
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();//关闭连接
                conn = null;
                comm = null;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            IntPtr ptr = FindWindow(null, "Form1");//获取接收消息的窗体句柄，这个地方有一点我们要注意Form1必须唯一，否则windows无法将消息正确发送  

            //消息构建  
 
            m.value = this.textBox1.Text;
            SendMessage(ptr, MYMESSAGE, 1, ref m);//发送消息
            sql = "select * from "
                 + table
                 + " where "
                 + ODP连接Oracle.Monster.name1
                 + "='"
                 + this.textBox1.Text
                 + "'";
            //SendConnectEvent(sql);
            //MessageBox.Show(dataMessage.value + " " + dataMessage.valueName);
  
            //Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedItem.ToString())
            {
                case "编号":
                    m.valueName = ODP连接Oracle.Monster.name1;
                    break;
                case "名称":
                    m.valueName = ODP连接Oracle.Monster.name2;
                    break;
                case "种族":
                    m.valueName = ODP连接Oracle.Monster.name3;
                    break;
                case "等级":
                    m.valueName = ODP连接Oracle.Monster.name4;
                    break;
                case "血条":
                    m.valueName = ODP连接Oracle.Monster.name5;
                    break;
                case "攻击":
                    m.valueName = ODP连接Oracle.Monster.name6;
                    break;
                case "防御":
                    m.valueName = ODP连接Oracle.Monster.name7;
                    break;
                case "级别":
                    m.valueName = ODP连接Oracle.Monster.name8;
                    break;
            }
        }
    }
}
