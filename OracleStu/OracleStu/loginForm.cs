using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OracleClient;

namespace OracleStu
{
    public partial class loginForm : Form
    {
        public loginForm()
        {
            InitializeComponent();
        }
        private void loginButton_Click(object sender, EventArgs e)
        {
            String username = textBox1.Text;   //定义变量存放数据库用户名
            String password = textBox2.Text;
            String connstring = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)));"+
                "User Id=" + username + ";Password=" + password + ";";
            OracleConnection conn = new OracleConnection(connstring);
            try
            {
                conn.Open();
                System.IO.File.WriteAllText(@"c:\a.txt", connstring);  //把数据库基本配置写入本地文本a.txt 方便后面使用
                ClientForm mainform = new ClientForm();
                conn.Close();
                MessageBox.Show("连接数据库成功!");
                this.Hide();  //隐藏当前窗体
                mainform.ShowDialog();  //显示客户端窗口
            }catch
            {
                //MessageBox.Show(ex.ToString());
                MessageBox.Show("无法登录，请核对用户名和密码，以及数据库是否可用!");
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
