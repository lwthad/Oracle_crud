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
    public partial class ClientForm : Form
    {

        public ClientForm()
        {
            InitializeComponent();
        }
        //定义两个全局变量
        public String connstring; 
        public int vcount;

        //刷新学生表
        public void student_datarefresh()
        {
            String sqltxt = "select sid 学号,sname 姓名,zhuanye 专业,ssex 性别,sbirthday 出生日期,snote 备注 from stu_inf";
            connstring = System.IO.File.ReadAllText(@"c:\a.txt");
            OracleConnection conn = new OracleConnection(connstring);
            OracleDataAdapter data = new OracleDataAdapter(sqltxt, conn);
            data.SelectCommand.CommandType = CommandType.Text;
            conn.Open();
            DataTable table = new DataTable();
            data.Fill(table);
            dataGridView1.DataSource = table;
            conn.Close();
        }
        //刷新课程表
        public void course_datarefresh()
        {
            String sqltxt = "select cid 课程号,cname 课程名,begintime 开学学期,xueshi 学时,credit 学分 from course_inf";
            connstring = System.IO.File.ReadAllText(@"c:\a.txt");
            OracleConnection conn = new OracleConnection(connstring);
            OracleDataAdapter data = new OracleDataAdapter(sqltxt, conn);
            data.SelectCommand.CommandType = CommandType.Text;
            conn.Open();
            DataTable table = new DataTable();
            data.Fill(table);
            dataGridView2.DataSource = table;
            conn.Close();
        }
        //刷新成绩表
        public void score_datarefresh()
        {
            String sqltxt = "select scid 选课编号,sid 学号,cid 课程号,score 成绩 from stu_course";
            connstring = System.IO.File.ReadAllText(@"c:\a.txt");
            OracleConnection conn = new OracleConnection(connstring);
            OracleDataAdapter data = new OracleDataAdapter(sqltxt, conn);
            data.SelectCommand.CommandType = CommandType.Text;
            conn.Open();
            DataTable table = new DataTable();
            data.Fill(table);
            dataGridView3.DataSource = table;
            conn.Close();
        }
        //窗体关闭事件
        private void ClientForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();  //结束项目
        }


        //***111111111111***    //学生管理模块
        private void stuButton_Click_1(object sender, EventArgs e)  //控制功能板块的显示
        {
            panel1.Visible = true;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            student_datarefresh();        //刷新学生信息表格
            S_btn_Click_1(sender, e);
        }
        //控制双击表格控件使得所选行对应显示到文本框中
        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            snametextB.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            zhuanyetextB.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            ssex_comboBox.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            dateTimePicker1.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();  //？？？
            snotetextB.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
        }
        //插入学生
        private void insert_stu_button_Click_1(object sender, EventArgs e)
        {
            string sbirth = "1990-01-01";
            DateTime vbirth = Convert.ToDateTime(sbirth);       
            OracleConnection conn = new OracleConnection(connstring);
            conn.Open();
            OracleCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "s_insert";
            cmd.Parameters.Add("v_sname", OracleType.VarChar).Direction = ParameterDirection.Input;
            cmd.Parameters["v_sname"].Value = snametextB.Text;
            cmd.Parameters.Add("v_zhuanye", OracleType.VarChar).Direction = ParameterDirection.Input;
            cmd.Parameters["v_zhuanye"].Value = zhuanyetextB.Text;
            cmd.Parameters.Add("v_ssex", OracleType.VarChar).Direction = ParameterDirection.Input;
            cmd.Parameters["v_ssex"].Value = ssex_comboBox.Text;
            cmd.Parameters.Add("v_sbirthday", OracleType.DateTime).Direction = ParameterDirection.Input;
            if (this.dateTimePicker1.Checked == false)  //判断时间控件是否被选中
                cmd.Parameters["v_sbirthday"].Value = vbirth;
            else
                cmd.Parameters["v_sbirthday"].Value = dateTimePicker1.Value.Date;
            cmd.Parameters.Add("v_snote", OracleType.VarChar).Direction = ParameterDirection.Input;
            cmd.Parameters["v_snote"].Value = snotetextB.Text;
            //接收返回值
            cmd.Parameters.Add("vresult", OracleType.Number).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery(); //执行sql语句
            vcount = Int32.Parse(cmd.Parameters["vresult"].Value.ToString());
            cmd.ExecuteNonQuery();
            conn.Close();
            if (vcount == 0){ MessageBox.Show("插入失败！该学生已存在！"); }
            else if (vcount == 2) { MessageBox.Show("插入数据违反数据约束，请核对！"); }
            else { MessageBox.Show("添加学生成功！");  student_datarefresh(); }
        }
        //删除学生
        private void delete_stu_button_Click_1(object sender, EventArgs e)
        {
            int sid = Int32.Parse(dataGridView1.CurrentRow.Cells[0].Value.ToString().Trim());
            OracleConnection conn = new OracleConnection(connstring);
            conn.Open();
            OracleCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "s_delete";
            cmd.Parameters.Add("v_sid", OracleType.Number).Direction = ParameterDirection.Input;
            cmd.Parameters["v_sid"].Value = sid;
            cmd.Parameters.Add("vresult", OracleType.Number).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            vcount = Int32.Parse(cmd.Parameters["vresult"].Value.ToString());
            conn.Close();
            if (vcount == 0){ MessageBox.Show("删除失败！该学生不存在或已删除！"); }
            else if (vcount == 2) { MessageBox.Show("删除数据违反数据约束，请核对！"); }
            else { MessageBox.Show("删除学生成功！");  student_datarefresh(); }
        }
        //修改学生信息
        private void update_stu_button_Click_1(object sender, EventArgs e)  
        {
            string sbirth = "1990-01-01";
            DateTime vbirth = Convert.ToDateTime(sbirth);   
            //在控件dataGridView1中双击某行，该行首列数据将传给sid
            int sid = Int32.Parse(dataGridView1.CurrentRow.Cells[0].Value.ToString().Trim());
            OracleConnection conn = new OracleConnection(connstring);
            conn.Open();
            OracleCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "s_update";
            cmd.Parameters.Add("v_sid", OracleType.Number).Direction = ParameterDirection.Input;
            cmd.Parameters["v_sid"].Value = sid;
            cmd.Parameters.Add("v_sname", OracleType.VarChar).Direction = ParameterDirection.Input;
            cmd.Parameters["v_sname"].Value = snametextB.Text;
            cmd.Parameters.Add("v_zhuanye", OracleType.VarChar).Direction = ParameterDirection.Input;
            cmd.Parameters["v_zhuanye"].Value = zhuanyetextB.Text;
            cmd.Parameters.Add("v_ssex", OracleType.VarChar).Direction = ParameterDirection.Input;
            cmd.Parameters["v_ssex"].Value = ssex_comboBox.Text;
            cmd.Parameters.Add("v_sbirthday", OracleType.DateTime).Direction = ParameterDirection.Input;
            if (this.dateTimePicker1.Checked == false)  //判断时间控件是否被选中
                 cmd.Parameters["v_sbirthday"].Value = sbirth;   //赋空值
            else
                 cmd.Parameters["v_sbirthday"].Value = dateTimePicker1.Value.Date;
            cmd.Parameters.Add("v_snote", OracleType.VarChar).Direction = ParameterDirection.Input;
            cmd.Parameters["v_snote"].Value = snotetextB.Text;
            cmd.Parameters.Add("vresult", OracleType.Number).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery(); //执行oracle更新语句
            vcount = Int32.Parse(cmd.Parameters["vresult"].Value.ToString());
            conn.Close();
            if (vcount == 0){ MessageBox.Show("更新失败！该学生不存在！"); }
            else if (vcount == 2) { MessageBox.Show("更新的数据违反数据约束，请核对！"); }
            else { MessageBox.Show("更新完毕！");  student_datarefresh(); }
        }
        private void S_btn_Click_1(object sender, EventArgs e)
        {
            snametextB.Text = "";
            zhuanyetextB.Text = "";
            ssex_comboBox.Text = "";
            snotetextB.Text = "";
            dateTimePicker1.Text = "1990-01-01";
            dateTimePicker1.Checked = false;
        }
        //模糊查询学生基本信息
        private void select_stu_button_Click_1(object sender, EventArgs e)
        {
            String sqltxt;
            if(this.dateTimePicker1.Checked == false)  //判断时间控件是否被选中
                sqltxt = "select sid 学号,sname 姓名, zhuanye 专业, ssex 性别, sbirthday 出生日期,snote 备注 from "+
                    "stu_inf where sname like '%" + snametextB.Text.Trim() + "%' and zhuanye "+
                    "like '%" + zhuanyetextB.Text.Trim() + "%' and ssex like '%" + ssex_comboBox.Text.Trim() + "%' and "+
                    "snote like '%" + snotetextB.Text.Trim() + "%'";
            else
                sqltxt = "select sid 学号,sname 姓名, zhuanye 专业, ssex 性别, sbirthday 出生日期,snote 备注 from "+
                    "stu_inf where sname like '%" + snametextB.Text.Trim() + "%' and zhuanye "+
                    "like '%" + zhuanyetextB.Text.Trim() + "%' and ssex like '%" + ssex_comboBox.Text.Trim() + "%' and "+
                    " sbirthday like '%" + dateTimePicker1.Value.Date + "%' and snote like '%" + snotetextB.Text.Trim() + "%'";
            connstring = System.IO.File.ReadAllText(@"c:\a.txt");    //从本地读取之前存储的数据库配置文件
            OracleConnection conn = new OracleConnection(connstring);
            OracleDataAdapter data = new OracleDataAdapter(sqltxt, conn);
            data.SelectCommand.CommandType = CommandType.Text;
            conn.Open();
            DataTable table = new DataTable();
            data.Fill(table);
            dataGridView1.DataSource = table;
            conn.Close();
        }

        //***2222222222***  //课程管理模块
        private void courseButton_Click_1(object sender, EventArgs e) 
        {
            panel1.Visible = false;
            panel2.Visible = true;
            panel3.Visible = false;
            panel4.Visible = false;
            course_datarefresh();
            C_btn_Click_1(sender,e);
        }
        //控制单击表格控件使得所选行对应显示到文本框中
        private void dataGridView2_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            cnametextB.Text = dataGridView2.CurrentRow.Cells[1].Value.ToString();
            beginTimetextB.Text = dataGridView2.CurrentRow.Cells[2].Value.ToString();
            xueshitextB.Text = dataGridView2.CurrentRow.Cells[3].Value.ToString();
            credittextB.Text = dataGridView2.CurrentRow.Cells[4].Value.ToString();
        }
        //插入课程
        private void insert_c_button_Click_1(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(connstring);
            conn.Open();
            OracleCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "c_insert";
            try
            {
                cmd.Parameters.Add("v_cname", OracleType.VarChar).Direction = ParameterDirection.Input;
                cmd.Parameters["v_cname"].Value = cnametextB.Text;
                cmd.Parameters.Add("v_beginTime", OracleType.Number).Direction = ParameterDirection.Input;
                cmd.Parameters["v_beginTime"].Value = Int32.Parse(beginTimetextB.Text); //强制转换文本->int
                cmd.Parameters.Add("v_xueshi", OracleType.Number).Direction = ParameterDirection.Input;
                cmd.Parameters["v_xueshi"].Value = Int32.Parse(xueshitextB.Text);
                cmd.Parameters.Add("v_credit", OracleType.Number).Direction = ParameterDirection.Input;
                cmd.Parameters["v_credit"].Value = Int32.Parse(credittextB.Text);

                cmd.Parameters.Add("vresult", OracleType.Number).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();   //执行oracle插入语句
                vcount = Int32.Parse(cmd.Parameters["vresult"].Value.ToString());//oracle返回值
                conn.Close();
                if (vcount == 0) { MessageBox.Show("插入失败！本课程已存在！"); }
                else if (vcount == 2) { MessageBox.Show("插入数据违反数据约束，请核对！"); }
                else { MessageBox.Show("插入课程成功"); course_datarefresh(); }
            }catch (Exception ec)
            { MessageBox.Show("ERROR：存在类型转换错误 或 文本框内容为空，请检查！"); }
        }
        //删除课程
        private void delete_c_button_Click_1(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(connstring);
            conn.Open();
            OracleCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "c_delete";
            cmd.Parameters.Add("v_cid", OracleType.Number).Direction = ParameterDirection.Input;
            cmd.Parameters["v_cid"].Value = Int32.Parse(dataGridView2.CurrentRow.Cells[0].Value.ToString().Trim());
            cmd.Parameters.Add("vresult", OracleType.Number).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            vcount = Int32.Parse(cmd.Parameters["vresult"].Value.ToString()); 
            conn.Close();
            if (vcount == 0) { MessageBox.Show("删除失败！该课程不存在或已删除！"); }
            else if (vcount == 2) { MessageBox.Show("删除数据违反数据约束，请核对！"); }
            else { MessageBox.Show("删除课程成功！"); course_datarefresh(); }
        }
        //更新课程信息
        private void update_c_button_Click_1(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(connstring);
            conn.Open();
            OracleCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "c_update";
            try
            {
                cmd.Parameters.Add("v_cid", OracleType.Number).Direction = ParameterDirection.Input;
                cmd.Parameters["v_cid"].Value = Int32.Parse(dataGridView2.CurrentRow.Cells[0].Value.ToString().Trim());
                cmd.Parameters.Add("v_cname", OracleType.VarChar).Direction = ParameterDirection.Input;
                cmd.Parameters["v_cname"].Value = cnametextB.Text;
                cmd.Parameters.Add("v_begintime", OracleType.Number).Direction = ParameterDirection.Input;
                cmd.Parameters["v_begintime"].Value = Int32.Parse(beginTimetextB.Text.Trim());
                cmd.Parameters.Add("v_xueshi", OracleType.Number).Direction = ParameterDirection.Input;
                cmd.Parameters["v_xueshi"].Value = Int32.Parse(xueshitextB.Text.Trim());
                cmd.Parameters.Add("v_credit", OracleType.Float).Direction = ParameterDirection.Input;
                cmd.Parameters["v_credit"].Value = float.Parse(credittextB.Text.Trim());
                cmd.Parameters.Add("vresult", OracleType.Number).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery(); //执行Oracle更新语句
                vcount = Int32.Parse(cmd.Parameters["vresult"].Value.ToString());
                cmd.ExecuteNonQuery();
                conn.Close();
                if (vcount == 0) { MessageBox.Show("更新失败！该课程不存在！"); }
                else if (vcount == 2) { MessageBox.Show("更新的数据违反数据约束，请核对！"); }
                else { MessageBox.Show("更新课程完毕！"); course_datarefresh(); }
            }catch(Exception ec)
            { MessageBox.Show("ERROR：存在类型转换错误 或 文本框内容为空，请检查！"); }

        }
        //清空文本框中的数据
        private void C_btn_Click_1(object sender, EventArgs e)
        {
            cnametextB.Text = "";
            beginTimetextB.Text = "";
            xueshitextB.Text = "";
            credittextB.Text = "";
        }
        //模糊查询课程基本信息
        private void select_cid_button_Click_1(object sender, EventArgs e)
        {
            String sqltxt = "select cid 课程号, cname 课程名, begintime 开学学期, xueshi 学时, credit 学分 from "+
                "course_inf where cname like '%" + cnametextB.Text + "%' and begintime "+
                "like '%" +beginTimetextB.Text + "%' and xueshi like '%" + xueshitextB.Text + "%' and "+
                "credit like '%" + credittextB.Text + "%'";
            connstring = System.IO.File.ReadAllText(@"c:\a.txt");   
            OracleConnection conn = new OracleConnection(connstring);
            OracleDataAdapter data = new OracleDataAdapter(sqltxt, conn);
            data.SelectCommand.CommandType = CommandType.Text;
            conn.Open();
            DataTable table = new DataTable();
            data.Fill(table);
            dataGridView2.DataSource = table;
            conn.Close();
        }

        //***3333333333***      //选课管理模块
        private void stuCourseButton_Click_1(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = true;
            panel4.Visible = false;
            //填充datagridview_sid网格
            String sqltext = "select sid as 学号,sname as 姓名 from stu_inf order by sid";
            connstring = System.IO.File.ReadAllText(@"c:\a.txt");
            OracleConnection conn = new OracleConnection(connstring);
            OracleDataAdapter data = new OracleDataAdapter(sqltext, conn);
            data.SelectCommand.CommandType = CommandType.Text;
            conn.Open();
            DataTable table = new DataTable();
            data.Fill(table);
            dataGridView_sid.DataSource = table;
            conn.Close();
            //填充dataGridView_cid 网格
            sqltext = "select cid as 课程号,cname as 课程名 from course_inf order by cid";
            connstring = System.IO.File.ReadAllText(@"c:\a.txt");
            OracleConnection conn1 = new OracleConnection(connstring);
            OracleDataAdapter data1 = new OracleDataAdapter(sqltext, conn);
            data1.SelectCommand.CommandType = CommandType.Text;
            conn1.Open();
            DataTable table1 = new DataTable();
            data1.Fill(table1);
            dataGridView_cid.DataSource = table1;
            conn1.Close();
            score_datarefresh(); //刷新成绩表 dataGridView3
            SC_btn_Click_1( sender,  e);
        }
        private void dataGridView3_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            sidtextBox.Text = dataGridView3.CurrentRow.Cells[1].Value.ToString();
            cidtextBox.Text = dataGridView3.CurrentRow.Cells[2].Value.ToString();
            score_textB.Text = dataGridView3.CurrentRow.Cells[3].Value.ToString();
        }
        //单击dataGridView_sid控件某行，可将该行首列内容放入sidtextBox文本框
        private void dataGridView_sid_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            sidtextBox.Text = dataGridView_sid.CurrentRow.Cells[0].Value.ToString();
        }
        //单击dataGridView_cid控件某行，可将该行首列内容放入cidtextBox文本框
        private void dataGridView_cid_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            cidtextBox.Text = dataGridView_cid.CurrentRow.Cells[0].Value.ToString();
        }
        //插入学生选课信息
        private void insert_sc_button_Click(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(connstring);
            conn.Open();
            OracleCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sc_insert";
            try
            {
                cmd.Parameters.Add("vs_id", OracleType.Number).Direction = ParameterDirection.Input;
                cmd.Parameters["vs_id"].Value = Int32.Parse(sidtextBox.Text.Trim()); //强制将文本类型去头尾空转换为int32
                cmd.Parameters.Add("vc_id", OracleType.Number).Direction = ParameterDirection.Input;
                cmd.Parameters["vc_id"].Value = Int32.Parse(cidtextBox.Text.Trim());
                cmd.Parameters.Add("vscore", OracleType.Number).Direction = ParameterDirection.Input;
                cmd.Parameters["vscore"].Value = Int32.Parse(score_textB.Text.Trim());
                //接收返回数据
                cmd.Parameters.Add("vresult", OracleType.Number).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();  //执行插入操作
                vcount = Int32.Parse(cmd.Parameters["vresult"].Value.ToString());
                conn.Close();
                if (vcount == 0) { MessageBox.Show("选课失败！该学号和课程代码所对应的成绩已经存在！"); }
                else if (vcount == 2) { MessageBox.Show("插入的数据违反数据约束，请核对！"); }
                else { MessageBox.Show("选课成功！"); score_datarefresh(); }
            }catch
            {{ MessageBox.Show("ERROR：存在类型转换错误 或 文本框内容为空，请检查！"); }}
        }
        //删除学生选课信息
        private void detele_sc_button_Click(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(connstring); 
            conn.Open();
            OracleCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sc_delete";
            cmd.Parameters.Add("vs_id", OracleType.Number).Direction = ParameterDirection.Input;
            cmd.Parameters["vs_id"].Value = Int32.Parse(sidtextBox.Text.Trim());
            cmd.Parameters.Add("vc_id", OracleType.Number).Direction = ParameterDirection.Input;
            cmd.Parameters["vc_id"].Value = Int32.Parse(cidtextBox.Text.Trim());
            cmd.Parameters.Add("vresult", OracleType.Number).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            vcount = Int32.Parse(cmd.Parameters["vresult"].Value.ToString());
            conn.Close();
            if (vcount == 0){ MessageBox.Show("删除失败！该学号和课程代码所对应的成绩不存在！"); }
            else if (vcount == 2){ MessageBox.Show("删除数据违反数据约束，请核对！"); }
            else { MessageBox.Show("删除选课成功！"); score_datarefresh(); }
        }
        //更新选课信息
        private void update_sc_button_Click(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(connstring); 
            conn.Open();
            OracleCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sc_update";
            try
            {
                cmd.Parameters.Add("vs_id", OracleType.Number).Direction = ParameterDirection.Input;
                cmd.Parameters["vs_id"].Value = Int32.Parse(sidtextBox.Text.Trim());
                cmd.Parameters.Add("vc_id", OracleType.Number).Direction = ParameterDirection.Input;
                cmd.Parameters["vc_id"].Value = Int32.Parse(cidtextBox.Text.Trim());
                cmd.Parameters.Add("vscore", OracleType.Number).Direction = ParameterDirection.Input;
                cmd.Parameters["vscore"].Value = Int32.Parse(score_textB.Text.Trim());

                cmd.Parameters.Add("vresult", OracleType.Number).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                vcount = Int32.Parse(cmd.Parameters["vresult"].Value.ToString());
                conn.Close();
                if (vcount == 0) { MessageBox.Show("更新成绩失败！该学号和课程代码所对应的成绩不存在！"); }
                else if (vcount == 2) { MessageBox.Show("修改的数据违反数据约束，请核对！"); }
                else { MessageBox.Show("更新成绩成功！"); score_datarefresh(); }
            }catch
            { MessageBox.Show("ERROR：存在类型转换错误 或 文本框内容为空，请检查！"); }
        }
        private void SC_btn_Click_1(object sender, EventArgs e)
        {
            sidtextBox.Text = "";
            cidtextBox.Text = "";
            score_textB.Text = "";
        }
        //查询选课信息
        private void select_sc_button_Click_1(object sender, EventArgs e)
        {
            String sqltxt = "select scid 选课编号, sid 学号, cid 课程号, score 成绩 from stu_course where "+
                "sid like '%" + sidtextBox.Text + "%' and cid like '%" + cidtextBox.Text + "%' and "+
                "score like '%" + score_textB.Text + "%'";
            connstring = System.IO.File.ReadAllText(@"c:\a.txt");    //从本地读取之前存储的数据库配置文件
            OracleConnection conn = new OracleConnection(connstring);
            OracleDataAdapter data = new OracleDataAdapter(sqltxt, conn);
            data.SelectCommand.CommandType = CommandType.Text;
            conn.Open();
            DataTable table = new DataTable();
            data.Fill(table);
            dataGridView3.DataSource = table;
            conn.Close();
        }

        //***4444444444***  //查询学生成绩和总学分模块
        private void selectButton_Click_1(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = true;
            connstring = System.IO.File.ReadAllText(@"c:\a.txt");    //从本地读取之前存储的数据库配置文件
           // dataGridView_query. Rows.Clear();  //清空表格
        }
        //查询所有学生课程成绩
        private void select_score_button_Click_1(object sender, EventArgs e)
        {
            String sqltxt = "select * from view_score";
            OracleConnection conn = new OracleConnection(connstring);
            OracleDataAdapter data = new OracleDataAdapter(sqltxt, conn);
            data.SelectCommand.CommandType = CommandType.Text;
            conn.Open();
            DataTable table = new DataTable();
            data.Fill(table);
            dataGridView_query.DataSource = table;
            conn.Close();
        }
        //查询总学分
        private void select_credit_button_Click_1(object sender, EventArgs e)
        {
            String sqltxt = "select * from credit_view";
            OracleConnection conn = new OracleConnection(connstring);
            OracleDataAdapter data = new OracleDataAdapter(sqltxt, conn);
            data.SelectCommand.CommandType = CommandType.Text;
            conn.Open();
            DataTable table = new DataTable();
            data.Fill(table);
            dataGridView_query.DataSource = table;
            conn.Close();
        }

    }
}
