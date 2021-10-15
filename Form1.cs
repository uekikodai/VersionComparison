using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp10
{
    public partial class Form1 : Form
    {
        private string _directory = "C:";
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectFolder(this.textBox1);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SelectFolder(this.textBox2);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            SelectFolder(this.textBox3);
        }

        private void SelectFolder(TextBox textBox)
        {
            var dialog = new FolderSelectDialog
            {
                InitialDirectory = _directory,
                Title = "フォルダを選択してください。"
            };
            if (dialog.Show(Handle))
            {
                textBox.Text = dialog.FileName;
                _directory = dialog.FileName;
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            var fileName1 = Directory.GetFiles(this.textBox1.Text, "*").Select(x => Path.GetFileName(x)).ToList();
            var fileName2 = Directory.GetFiles(this.textBox2.Text, "*").Select(x => Path.GetFileName(x)).ToList();

            // LoggingConfigurationを生成 
            var config = new LoggingConfiguration();

            // FileTargetを生成し LoggingConfigurationに設定 
            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // fileTargetのプロパティを設定
            fileTarget.Name = "f";
            fileTarget.FileName = $"{this.textBox3.Text}/execution.log";
            fileTarget.Layout = "${message}";

            // LoggingRuleを定義
            var rule1 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule1);

            // 設定を有効化
            LogManager.Configuration = config;

            var count = 0;
            foreach (var name in fileName2)
            {
                if (!fileName1.Contains(name))
                {
                    continue;
                }
                var ComparisonFile1 = System.IO.Path.Combine(this.textBox1.Text, name);
                var ComparisonFile2 = System.IO.Path.Combine(this.textBox2.Text, name);
                var ComparisonFile3 = System.IO.Path.Combine(this.textBox3.Text, name);

                if(!Directory.Exists(this.textBox1.Text))
                {
                    MessageBox.Show($"{this.textBox1.Text}フォルダは存在しません");
                }
                if (!Directory.Exists(this.textBox2.Text))
                {
                    MessageBox.Show($"{this.textBox2.Text}フォルダは存在しません");
                }
                if (!Directory.Exists(this.textBox3.Text))
                {
                    Directory.CreateDirectory(this.textBox3.Text);
                }

                var vi1 = System.Diagnostics.FileVersionInfo.GetVersionInfo(
                        ComparisonFile1);
                var vi2 = System.Diagnostics.FileVersionInfo.GetVersionInfo(
                        ComparisonFile2);

                if(vi1.FileVersion.Equals(vi2.FileVersion))
                {
                    continue;
                }
                if (vi1.ProductVersion.Equals(vi2.ProductVersion))
                {
                    continue;
                }

                File.Copy(Path.Combine(ComparisonFile1), ComparisonFile3, true);
                count++;
                logger.Info($"{name}：{vi1.FileVersion}");
            }
            MessageBox.Show($"実行を終了しました。\n\r出力件数：{count}/{fileName1.Count}",
                "通知",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
