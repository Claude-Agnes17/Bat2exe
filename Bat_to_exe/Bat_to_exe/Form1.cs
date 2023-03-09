using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Bat_to_exe
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        public Form1()
        {
            InitializeComponent();
            metroButton2.Enabled = false;
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Batch File Select";
            ofd.Filter = "Batch File (*.bat) | *.bat| All Files (*.*) | *.*";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                metroTextBox1.Text = ofd.FileName;
                richTextBox1.Text = File.ReadAllText(ofd.FileName);
                metroButton2.Enabled = true;
            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            var replacement = richTextBox1.Text.Replace("\"", "\"\"");
            replacement = Convert.ToBase64String(Encoding.UTF8.GetBytes(replacement));
            string code = @"
using System;
using System.Diagnostics;
using System.IO;
namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            var batch_code = @" + "\"" + Encoding.UTF8.GetString(Convert.FromBase64String(replacement)) + "\";" + @"
            var temp_file = Path.GetTempFileName() + "".bat"";
            File.WriteAllText(temp_file, batch_code);
            var process = new Process();
            process.StartInfo.FileName = temp_file;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.WaitForExit();
            File.Delete(temp_file);
        }
    }
}
";
            MessageBox.Show(code);

            string savePath = string.Empty;
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "Save Location";
            saveFile.Filter = "exe File(*.exe) |*.exe";
            if(saveFile.ShowDialog() == DialogResult.OK)
            {
                savePath = saveFile.FileName.ToString();
                CodeDomProvider codeDomProvider = new CSharpCodeProvider();
                CompilerParameters compilerParameters = new CompilerParameters();
                compilerParameters.ReferencedAssemblies.Add("System.dll");

                compilerParameters.GenerateExecutable = true;
                compilerParameters.OutputAssembly = savePath;
                CompilerResults compilerResults = codeDomProvider.CompileAssemblyFromSource(compilerParameters, code);
                if (compilerResults.Errors.Count > 0)
                {
                    richTextBox1.Text = compilerResults.Errors[0].ToString();
                }
                else
                {
                    MessageBox.Show("Success");
                    richTextBox1.Text = string.Empty;
                }
            }
            
        }
    }
}
