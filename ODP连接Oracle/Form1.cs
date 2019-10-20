using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Oracle.ManagedDataAccess;
using System.IO;
using System.Xml;
using System.Xml.Linq;  

namespace ODP连接Oracle
{

    public partial class Form1 : Form
    {

        public const int USER = 0x500;
        public const int MYMESSAGE = USER + 1;
        private static string table; //表名字  都是用 xml文件获取的
        private int pageW = 1, pageU = 10; //分页上下线
        private int page; //每一页多少条

        private string myXMLFilePath = "config.xml";    //xml文件存储路径
        private static string sql;
        string fileName;
        private static bool isConnect;

        private List<Monster> list = new List<Monster>();
        private Monster monsterTemp = new Monster();
        private OracleConnection conn = null;
        private OracleCommand comm = null;
        private OracleDataReader rdr = null;

        //Search searchForm = null;

        //各种连接参数 位置：E:\Oracle\product\11.2.0\dbhome_2\network\admin\tnsnames.ora
        static string conString =
            "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)" +
            "(HOST=localhost)" +
            "(PORT=1521))" +
            "(CONNECT_DATA=(SID=oracle)));";// +
                                            //"User Id=system;" +
                                            //"Password=123456;";

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                //接收自定义消息MYMESSAGE，并显示其参数  
                case MYMESSAGE:
                    Search.My_lParam ml = new Search.My_lParam();
                    Type t = ml.GetType();
                    ml = (Search.My_lParam)m.GetLParam(t);
                    sql = "select * from "
                + table
                + " where "
                + ml.valueName
                + "='"
                + ml.value
                + "'";
                    MessageBox.Show(sql);
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }

        }




        public Form1()
        {
            this.MaximizeBox = false;   //取消最大化
            this.FormBorderStyle = FormBorderStyle.FixedDialog;//设置边框为不可调节
            InitializeComponent();      //显示from窗口
            InitListView(listView);     //调用显示表单
            isConnect = false;
            button6.Enabled = false;

        }



        //数据库事件
        public void SendConnectEvent(string sql)
        {
            if (!isConnect)
            {
                MessageBox.Show("数据库未连接");
                return;
            }
            try //实例化OracleConnection对象 
            {
                Monster.count = 0;
                list.Clear();
                conn.Open(); //开启数据库连接
                comm = new OracleCommand(sql, conn);//事物处理
                rdr = comm.ExecuteReader(); //返回的结果
                while (rdr.Read())
                {
                    monsterTemp.Mno = rdr.GetString(0);
                    monsterTemp.Mname = rdr.GetString(1);
                    monsterTemp.Mrace = rdr.GetString(2);
                    monsterTemp.Mlevel = rdr.GetInt32(3);
                    monsterTemp.Mblood1 = rdr.GetInt32(4);
                    monsterTemp.Mattack = rdr.GetInt32(5);
                    monsterTemp.Mdefense = rdr.GetInt32(6);
                    monsterTemp.Mgrade = rdr.GetInt32(7);
                    list.Add(monsterTemp);
                    Monster.count++;
                    monsterTemp = null;
                    monsterTemp = new Monster(); //这里每次都销毁并重新实例化
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();//关闭连接
                comm = null;
                rdr = null;
            }

        }


        //初始化列表
        public void InitListView(ListView lv)
        {
            //设置属性
            lv.GridLines = true;     //显示网格线
            lv.FullRowSelect = true; //显示全行
            lv.MultiSelect = true;   //设置多选
            lv.LabelEdit = true;
            lv.AllowColumnReorder = true;
            lv.View = View.Details;  //设置显示模式为详细
            //添加列名
            lv.Columns.Add("编号", 60, HorizontalAlignment.Center);//一步添加 
            lv.Columns.Add("名称", 80, HorizontalAlignment.Center);//一步添加 
            lv.Columns.Add("种族", 80, HorizontalAlignment.Center);//一步添加 
            lv.Columns.Add("等级", 80, HorizontalAlignment.Center);//一步添加 
            lv.Columns.Add("血条", 80, HorizontalAlignment.Center);//一步添加 
            lv.Columns.Add("攻击", 80, HorizontalAlignment.Center);//一步添加 
            lv.Columns.Add("防御", 80, HorizontalAlignment.Center);//一步添加 
            lv.Columns.Add("级别", 80, HorizontalAlignment.Center);//一步添加 
        }

        //将数据库数据导入表格
        public void ImportListView(ListView lv)
        {
            if (!isConnect)
            {
                MessageBox.Show("数据库未连接");
                return;
            }
            lv.Items.Clear(); //先清空一下数据
            //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度 
            for (int i = 0; i < list.Count; i++)  //添加10行数据 
            {
                ListViewItem lvi = new ListViewItem();
                lvi.ImageIndex = i;
                lvi.Text = list[i].Mno;
                lvi.SubItems.Add(list[i].Mname);
                lvi.SubItems.Add(list[i].Mrace);
                lvi.SubItems.Add(list[i].Mlevel.ToString());
                lvi.SubItems.Add(list[i].Mblood1.ToString());
                lvi.SubItems.Add(list[i].Mattack.ToString());
                lvi.SubItems.Add(list[i].Mdefense.ToString());
                lvi.SubItems.Add(list[i].Mgrade.ToString());
                lv.Items.Add(lvi);
            }
            lv.Update();
        }

        /// 当listview选中状态改变时调用的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {

            //当有选择行的数据时
            if (this.listView.SelectedItems.Count > 0)
            {
                cmMenu.Top = Control.MousePosition.Y;
                cmMenu.Left = Control.MousePosition.X;
                this.cmMenu.Visible = true;
                //MessageBox.Show(Control.MousePosition.X.ToString());
                //把选择的信息显示在相应的文本框中
                this.textBox1.Text = this.listView.SelectedItems[0].SubItems[0].Text;
                this.textBox2.Text = this.listView.SelectedItems[0].SubItems[1].Text;
                this.textBox3.Text = this.listView.SelectedItems[0].SubItems[2].Text;
                this.textBox4.Text = this.listView.SelectedItems[0].SubItems[3].Text;
                this.textBox5.Text = this.listView.SelectedItems[0].SubItems[4].Text;
                this.textBox6.Text = this.listView.SelectedItems[0].SubItems[5].Text;
                this.textBox7.Text = this.listView.SelectedItems[0].SubItems[6].Text;
                this.textBox8.Text = this.listView.SelectedItems[0].SubItems[7].Text;
            }
        }

        private void CmMenu_LocationChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void 保存所有ToolStripMenuItem_Click(object sender, EventArgs e)
        {


            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                DirectoryInfo theFolder = new DirectoryInfo(foldPath);
                fileName = theFolder.FullName
                    + "\\"
                    + DateTime.Now.Hour
                    + "-"
                    + DateTime.Now.Minute
                    + "-"
                    + DateTime.Now.Second
                    + "-data.csv";
            }

            if (!System.IO.File.Exists(fileName))
            {
                FileStream stream = System.IO.File.Create(fileName);
                stream.Close();
                stream.Dispose();
            }

            //写入文件
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                writer.Write(ODP连接Oracle.Monster.name1 + ",");
                writer.Write(ODP连接Oracle.Monster.name2 + ",");
                writer.Write(ODP连接Oracle.Monster.name3 + ",");
                writer.Write(ODP连接Oracle.Monster.name4 + ",");
                writer.Write(ODP连接Oracle.Monster.name5 + ",");
                writer.Write(ODP连接Oracle.Monster.name6 + ",");
                writer.Write(ODP连接Oracle.Monster.name7 + ",");
                writer.Write(ODP连接Oracle.Monster.name8);
                writer.WriteLine();

                for (int i = 0; i < list.Count; i++)
                {
                    writer.Write(list[i].Mno + ",");
                    writer.Write(list[i].Mname + ",");
                    writer.Write(list[i].Mrace + ",");
                    writer.Write(list[i].Mlevel + ",");
                    writer.Write(list[i].Mblood1 + ",");
                    writer.Write(list[i].Mattack + ",");
                    writer.Write(list[i].Mdefense + ",");
                    writer.Write(list[i].Mgrade);
                    writer.WriteLine();
                }
            }

            System.Diagnostics.Process.Start(fileName);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }


        private void 导入数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sql = "select * from " + table;
            SendConnectEvent(sql);
            ImportListView(listView);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
        }

        private void 程序信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox1 = new AboutBox1();
            aboutBox1.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void 条件查找ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Search searchForm = new Search();
            //searchForm.Show();
            //listView.Items.Clear();
            //SendConnectEvent(searchSQL);
            //ImportListView(listView);
            //   SELECT* FROM
            //(
            //SELECT A.*, ROWNUM RN
            //FROM(SELECT * FROM monster) A
            //WHERE ROWNUM <= 400
            //)  
            //WHERE RN >= 21

            //还原参数
            pageW = 1;
            pageU = 10;

            sql = "SELECT * FROM(SELECT A.*，ROWNUM RN FROM (SELECT * FROM "
                + table
                + ") A WHERE ROWNUM <= "
                + pageU + ") WHERE RN >="
                + pageW;
            SendConnectEvent(sql);
            ImportListView(listView);

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
            this.textBox2.Text = "";
            this.textBox3.Text = "";
            this.textBox4.Text = "";
            this.textBox5.Text = "";
            this.textBox6.Text = "";
            this.textBox7.Text = "";
            this.textBox8.Text = "";


        }



        private void button5_Click(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                sql = "select * from "
                    + table
                    + " where "
                    + ODP连接Oracle.Monster.name1
                    + "='"
                    + this.textBox1.Text
                    + "'";
            }
            else if (this.checkBox2.Checked)
            {
                sql = "select * from "
                    + table
                    + " where "
                    + ODP连接Oracle.Monster.name2
                    + "='"
                    + this.textBox2.Text
                    + "'";
            }

            else if (this.checkBox3.Checked)
            {
                sql = "select * from "
                    + table
                    + " where "
                    + ODP连接Oracle.Monster.name3
                    + "='"
                    + this.textBox3.Text
                    + "'";
            }
            else if (this.checkBox4.Checked)
            {
                sql = "select * from "
                    + table
                    + " where "
                    + ODP连接Oracle.Monster.name4
                    + "='"
                    + this.textBox4.Text
                    + "'";
            }
            else
            {
                MessageBox.Show("请勾选指定查询的选项", "查询提示");
            }

            SendConnectEvent(sql);
            ImportListView(listView);
        }

        private void 清空表格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView.Items.Clear();
        }

        private void 连接数据库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isConnect)
            {
                MessageBox.Show("数据库已经连接");
                return;
            }
            else
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(myXMLFilePath);
                    //读取Activity节点下的数据。SelectSingleNode匹配第一个Activity节点  
                    XmlNode root = xmlDoc.SelectSingleNode("//Oracle");//当节点Workflow带有属性是，使用SelectSingleNode无法读取          
                    if (root != null)
                    {
                        string OracleUser = (root.SelectSingleNode("OracleUser")).InnerText;
                        string OraclePassword = (root.SelectSingleNode("OraclePassword ")).InnerText;
                        table = (root.SelectSingleNode("table")).InnerText;
                        page = int.Parse((root.SelectSingleNode("page")).InnerText);
                        conString += "User Id=" + OracleUser + ";Password=" + OraclePassword + ";";
                    }
                    conn = new OracleConnection(conString);//初始化连接
                    MessageBox.Show("连接成功", "登录提示");
                    isConnect = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "登录失败");
                }
            }
        }

        private void 数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked)
            {
                sql = "update " + table + " set "
                    + ODP连接Oracle.Monster.name2 + "='"
                    + this.textBox2.Text
                    + "' where "
                    + ODP连接Oracle.Monster.name1
                    + "='" + this.textBox1.Text + "'";

                SendConnectEvent(sql);
                sql = "select * from " + table;
                SendConnectEvent(sql);
                ImportListView(listView);
            }
            if (this.checkBox3.Checked)
            {
                sql = "update " + table + " set "
                    + ODP连接Oracle.Monster.name3 + "='"
                    + this.textBox3.Text
                    + "' where "
                    + ODP连接Oracle.Monster.name1
                    + "='" + this.textBox1.Text + "'";
                SendConnectEvent(sql);
                sql = "select * from " + table;
                SendConnectEvent(sql);
                ImportListView(listView);
            }
            if (this.checkBox4.Checked)
            {
                sql = "update " + table + " set "
                    + ODP连接Oracle.Monster.name4 + "='"
                    + this.textBox4.Text
                    + "' where "
                    + ODP连接Oracle.Monster.name1
                    + "='" + this.textBox1.Text + "'";
                SendConnectEvent(sql);
                sql = "select * from " + table;
                SendConnectEvent(sql);
                ImportListView(listView);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text != ""
                && this.textBox2.Text != ""
                && this.textBox3.Text != ""
                && this.textBox4.Text != ""
                && this.textBox5.Text != ""
                && this.textBox6.Text != ""
                && this.textBox7.Text != ""
                && this.textBox8.Text != "")
            {
                //必须保证前三个条件被输入

                sql = "insert into " + table + " values("
                    + "'" + this.textBox1.Text + "',"
                    + "'" + this.textBox2.Text + "',"
                    + "'" + this.textBox3.Text + "',"
                    + "'" + this.textBox4.Text + "',"
                    + "'" + this.textBox5.Text + "',"
                    + "'" + this.textBox6.Text + "',"
                    + "'" + this.textBox7.Text + "',"
                    + "'" + this.textBox8.Text + "')";
                SendConnectEvent(sql);
                sql = "select * from " + table;
                SendConnectEvent(sql);
                ImportListView(listView);
            }
            else
            {
                MessageBox.Show("尚未输入完整信息", "插入失败");
                return;
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text != "")
            {
                sql = "delete from " + table + " where "                   
                    + ODP连接Oracle.Monster.name1 + " ='"
                    + this.textBox1.Text + "'"; 

                SendConnectEvent(sql);
                sql = "select * from " + table;
                SendConnectEvent(sql);
                ImportListView(listView);
            }
            else
            {
                MessageBox.Show("编号为空", "删除失败");
                return;
            }

        }

        private void cmMenu_Opening(object sender, CancelEventArgs e)
        {
            this.cmMenu.SetBounds(Control.MousePosition.X,Control.MousePosition.Y,200, 100);

        }

        private void button7_Click(object sender, EventArgs e)
        {
            pageU += page;
            pageW += page;
            if (pageW > 10)
            {
                button6.Enabled = true;
            }
            if (pageU > list.Count + page * 2)
            {
                button7.Enabled = false;
            }
            sql = "SELECT * FROM(SELECT A.*，ROWNUM RN FROM (SELECT * FROM "
                + table
                + ") A WHERE ROWNUM <= "
                + pageU + ") WHERE RN >="
                + pageW;
            SendConnectEvent(sql);
            ImportListView(listView);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pageU -= page;
            pageW -= page;
            if (pageW < 10)
            {
                button6.Enabled = false;
            }
            if (pageU < list.Count+page*3)
            {
                button7.Enabled = true;
            }

            sql = "SELECT * FROM(SELECT A.*，ROWNUM RN FROM (SELECT * FROM "
                + table
                + ") A WHERE ROWNUM <= "
                + pageU + ") WHERE RN >="
                + pageW;
            SendConnectEvent(sql);
            ImportListView(listView);
        }

        private void 啊是ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void 啊是ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text != "")
            {
                sql = "delete from " + table + " where "
                    + ODP连接Oracle.Monster.name1 + " ='"
                    + this.textBox1.Text + "'";

                SendConnectEvent(sql);
                sql = "select * from " + table;
                SendConnectEvent(sql);
                ImportListView(listView);
            }
            else
            {
                MessageBox.Show("编号为空", "删除失败");
                return;
            }
        }
    }
}
