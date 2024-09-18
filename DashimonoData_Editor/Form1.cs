using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace DashimonoData_Editor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static class globalVar
        {
            public static string filepath;
            public static List<DashimonoDataFormat> dashimono;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            globalVar.dashimono = new List<DashimonoDataFormat>();
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            データを保存するToolStripMenuItem.Enabled = false;
        }

        private void データを開くToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "DashimonoDataFile(*.json)|*.json";
            ofd.Title = "読み込む出し物データを選んでください。";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                globalVar.filepath = ofd.FileName;
                try
                {
                    using (var stream = new FileStream(ofd.FileName, FileMode.Open))
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            var js_l = JsonConvert.DeserializeObject<List<DashimonoDataFormat>>(sr.ReadToEnd());
                            globalVar.dashimono = js_l;
                        }
                    }
                    ReDrawList();
                    this.Text = $"出し物データファイル編集ソフト - {Path.GetFileName(globalVar.filepath)}";
                    データを保存するToolStripMenuItem.Enabled = true;
                }
                catch
                {
                    MessageBox.Show($"エラー: \n\r指定されたファイルは見つかりませんでした。\r\nファイルの場所を確認して、もう一度お試しください。(E01)", "データ読み込み失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);return;
                }
            }
            else
            {
                MessageBox.Show($"エラー: \n\rファイルが指定されませんでした。\r\nもう一度やり直してください。(E01)", "データ未指定", MessageBoxButtons.OK, MessageBoxIcon.Error);return;
            }

        }

        private void データを保存するToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var sw = new StreamWriter(globalVar.filepath,false, Encoding.UTF8))
                {
                    var js_savedata = JsonConvert.SerializeObject(globalVar.dashimono, Formatting.Indented);
                    sw.Write(js_savedata);
                    sw.Close();
                }
                this.Text = $"出し物データファイル編集ソフト - {Path.GetFileName(globalVar.filepath)}";
            }
            catch
            {
                MessageBox.Show($"エラー: \n\rファイルの保存に失敗しました。\r\nファイルの場所を確認して、もう一度お試しください。(E02)", "データ保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Error); return;
            }
        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InsertDashimono(textBox1.Text, textBox2.Text ,(int)numericUpDown1.Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                int selectindex = 0;
                selectindex = listView1.SelectedItems[0].Index;
                DeleteDashimono(selectindex);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                int selectindex = 0;
                selectindex = listView1.SelectedItems[0].Index;
                UpdateDashimono(selectindex);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                int selectindex = 0;
                selectindex = listView1.SelectedItems[0].Index;
                textBox1.Text = globalVar.dashimono[selectindex].Class;
                textBox2.Text = globalVar.dashimono[selectindex].Name;
                numericUpDown1.Value = globalVar.dashimono[selectindex].OKNinzu;
            }
        }

        private void ファイルを閉じるToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Text = "出し物データファイル編集ソフト - 無題";
            globalVar.dashimono = new List<DashimonoDataFormat>();
            globalVar.filepath = "";
            mozireset();
            ReDrawList();
        }

        /// <summary>
        /// ListViwewの再描画を行います。
        /// </summary>
        private void ReDrawList()
        {
            listView1.Items.Clear();
            foreach (DashimonoDataFormat data in globalVar.dashimono)
            {
                string[] values = { data.Class, data.Name, data.OKNinzu.ToString() };
                listView1.Items.Add(new ListViewItem(values));
            }
        }

        /// <summary>
        /// 出し物データの挿入
        /// </summary>
        /// <param name="Class">年・組</param>
        /// <param name="Name">出し物の名前</param>
        /// <param name="OKNinzu">受け入れ可能人数</param>
        private void InsertDashimono(string Class,string Name,int OKNinzu)
        {
            if(Class != "" && Name !="") 
            {
                DashimonoDataFormat data = new DashimonoDataFormat()
                {
                    Class = Class,
                    Name = Name,
                    OKNinzu = OKNinzu
                };
                globalVar.dashimono.Add(data);
                ReDrawList();
                mozireset();
            }
            else
            {
                MessageBox.Show($"エラー: \n\rデータが入力されていません。\r\nフィールドを全て埋めてからボタンを押してください。(E02)", "フィールドエラー", MessageBoxButtons.OK, MessageBoxIcon.Error); return;

            }
        }
        
        /// <summary>
        /// すでに追加した出し物を削除します。
        /// </summary>
        /// <param name="dashimono">削除したい出し物</param>
        private void DeleteDashimono(int index)
        {
            globalVar.dashimono.RemoveAt(index);
            ReDrawList();
            mozireset();
        }

        /// <summary>
        /// すでに追加した出し物を編集します。
        /// </summary>
        /// <param name="dashimono">更新したい出し物</param>
        private void UpdateDashimono(int index)
        {
            DashimonoDataFormat updashimono = globalVar.dashimono[index];
            foreach(DashimonoDataFormat value in globalVar.dashimono)
            {
                if(updashimono == value)
                {
                    value.Class = textBox1.Text;
                    value.Name = textBox2.Text;
                    value.OKNinzu = (int)numericUpDown1.Value;
                    break;
                }
            }
            ReDrawList();
            mozireset();
        }

        private void mozireset()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            numericUpDown1.Value = 0;
        }

        private void 名前を付けて保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "*.json";
            sfd.Filter = "DashimonoDataFile(*.json)|*.json";
            sfd.Title = "出し物データの保存先を指定してください。";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Stream stream = sfd.OpenFile();
                    using (var sw = new StreamWriter(stream, Encoding.UTF8))
                    {
                        var js_savedata = JsonConvert.SerializeObject(globalVar.dashimono, Formatting.Indented);
                        sw.Write(js_savedata);
                        sw.Close();
                    }
                    stream.Close();
                    this.Text = $"出し物データファイル編集ソフト - {Path.GetFileName(sfd.FileName)}";
                }
                catch
                {
                    MessageBox.Show($"エラー: \n\rファイルの保存に失敗しました。\r\nファイルの場所を確認して、もう一度お試しください。(E02)", "データ保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Error); return;
                }
            }
            else
            {
                MessageBox.Show($"エラー: \n\rファイルが指定されませんでした。\r\nもう一度やり直してください。(E01)", "データ未指定", MessageBoxButtons.OK, MessageBoxIcon.Error); return;
            }
        }
    }
}
