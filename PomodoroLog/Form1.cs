using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PomodoroLog
{
    public partial class Form1 : Form
    {
        private const string logFileName = "tasklog.csv";
        HashSet<string> taskList = new HashSet<string>();
        private const int pomodoroUnit = 25;
        HashSet<string> projectList = new HashSet<string>();
        string lastTask;
        DateTime start;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnStartClick();
        }

        private void btnStartClick()
        {
            this.WindowState = FormWindowState.Minimized;
            start = DateTime.Now;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime t = DateTime.Now;
            TimeSpan ts = TimeSpan.FromSeconds((t - start).TotalSeconds);
            string sMin = (ts.Minutes < 10)?"0"+ts.Minutes.ToString():ts.Minutes.ToString();
            string sSec= (ts.Seconds < 10)?"0"+ts.Seconds.ToString():ts.Seconds.ToString();
            this.Text = sMin + ":" + sSec;
            if (ts.Minutes == pomodoroUnit)
            {
                this.WindowState = FormWindowState.Normal;
                timer1.Enabled = false;
                string fileName = AppDomain.CurrentDomain.BaseDirectory + "/"+logFileName;
                using (StreamWriter sw = new StreamWriter(fileName,true))
                {
                    string task = textBox1.Text;
                    string project = string.IsNullOrWhiteSpace(textBox2.Text)? "Default" : textBox2.Text;
                    sw.WriteLine(task + "," + start.ToShortDateString() + "," + start.ToShortTimeString() + "," + t.ToShortDateString() + "," + t.ToShortTimeString()+","+ts.TotalMinutes.ToString()+","+project);
                    textBox1.Text = string.Empty;
                }
                UpdateTaskAndProjectList();
            }
        }

        private void UpdateTaskAndProjectList()
        {
            GetPreviousTaskList();
            LoadPreviousTaskList();
            GetPreviousProjectList();
            LoadPreviousProjectList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            DateTime t = DateTime.Now;
            TimeSpan ts = TimeSpan.FromSeconds((t - start).TotalSeconds);
            //write to csv file
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "/" + logFileName;
            using (StreamWriter sw = new StreamWriter(fileName,true))
            {
                string task = textBox1.Text;
                string project = string.IsNullOrWhiteSpace(textBox2.Text) ? "Default" : textBox2.Text;
                sw.WriteLine(task + "," + start.ToShortDateString() + "," + start.ToShortTimeString() + "," + t.ToShortDateString() + "," + t.ToShortTimeString() + "," + ts.TotalMinutes.ToString() + "," + project);
                textBox1.Text = string.Empty;
            }
            UpdateTaskAndProjectList();
        }

        private void tbxTask_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnStartClick();
            }
        }

        private void tbxTask_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Oemcomma)
            {
                textBox1.Text = textBox1.Text.Replace(",", string.Empty);
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.SelectionLength = 0;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "/" + logFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                UpdateTaskAndProjectList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void GetPreviousProjectList()
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "/" + logFileName;
            using (StreamReader sr = new StreamReader(fileName, true))
            {
                while (sr.Peek() >= 0)
                {
                    string[] items = sr.ReadLine().Split(',');
                    if (items.Length >= 6)
                    {
                        projectList.Add(items[6]);
                    }
                }
            }
        }

        private void LoadPreviousProjectList()
        {
            comboBox2.Items.Clear();
            string[] itemList = projectList.ToArray();
            comboBox2.Items.AddRange(itemList);
            comboBox2.SelectedIndex = 0;
        }

        private void GetPreviousTaskList()
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "/" + logFileName;
            using (StreamReader sr = new StreamReader(fileName, true))
            {
                while (sr.Peek() >= 0)
                {
                    taskList.Add(sr.ReadLine().Split(',')[0]);
                }
            }
        }

        private void LoadPreviousTaskList()
        {
            comboBox1.Items.Clear();
            string[] itemList = taskList.ToArray();
            comboBox1.Items.AddRange(itemList);
            comboBox1.SelectedIndex = 0;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            comboBox1.Enabled = true;
            textBox1.Text = comboBox1.Items[comboBox1.SelectedIndex].ToString();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = true;
            comboBox1.Enabled = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = comboBox1.Items[comboBox1.SelectedIndex].ToString();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = true;
            comboBox2.Enabled = false;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = true;
            textBox2.Enabled = false;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Text = comboBox2.Items[comboBox2.SelectedIndex].ToString();
        }

        private void btnAddOldTask_Click(object sender, EventArgs e)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "/" + logFileName;
            using (StreamWriter sw = new StreamWriter(fileName, true))
            {
                string task = textBox1.Text;
                string project = string.IsNullOrWhiteSpace(textBox2.Text) ? "Default" : textBox2.Text;
                TimeSpan ts = TimeSpan.FromSeconds((dateTimePicker2.Value.TimeOfDay - dateTimePicker1.Value.TimeOfDay).TotalSeconds);
                sw.WriteLine(task + "," + dateTimePicker1.Value.ToShortDateString()+ "," + dateTimePicker1.Value.ToShortTimeString() + "," + dateTimePicker2.Value.ToShortDateString() + "," + dateTimePicker2.Value.ToShortTimeString() + "," + ts.TotalMinutes.ToString() + "," + project);
                textBox1.Text = string.Empty;
            }
            UpdateTaskAndProjectList();
        }

        
    }
}
