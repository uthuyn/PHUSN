using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace uspan
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }
        AlgoUSpan algo = new AlgoUSpan();
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            openFileDialog.Title = "Browse Text Files";

            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Multiselect = true;

            openFileDialog.DefaultExt = "txt";
            openFileDialog.Filter = "Data (.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtInputFile.Text = openFileDialog.FileName;
            }
            else
                return;
        }

        private void btnMining_Click(object sender, EventArgs e)
        {
            listTreeHUSP.Nodes.Clear();
            string external = txtInputFile.Text.Substring(0, txtInputFile.Text.LastIndexOf("_"));
            external += "_ExternalUtility_neg.txt";
            //if (txtInputFile.Text.Substring(txtInputFile.Text.LastIndexOf(@"\") + 1, txtInputFile.TextLength - txtInputFile.Text.LastIndexOf(@"\") - 1) != "sanitized_ouput")
            //{

            //    string externalModifyData = txtInputFile.Text.Substring(0, txtInputFile.Text.LastIndexOf(@"\"));
            //    externalModifyData += "\\sanitized_ouput_ExternalUtility.txt";
            //    File.Delete(externalModifyData);
            //    File.Copy(external, externalModifyData, true);
            //}

            // the path for saving the patterns found
            string output = ".//husp_output.txt";
            // run the algorithm
            if (txtminUntil.Text == "")
                MessageBox.Show("Input minimum utility");
            else
            {
                if (txtMaxLength.Text != "")
                {
                    algo.setMaxPatternLength(int.Parse(txtMaxLength.Text));
                }
                algo.runAlgorithm(external, txtInputFile.Text, float.Parse(txtminUntil.Text),output);
                // print statistics
                //algo.miningPrintStats();
                MessageBox.Show("Finish!");
                addDataToTreeView();
            }
        }
        public void addDataToTreeView()
        {
            TreeNode node = new TreeNode();
            string file = txtInputFile.Text.Substring(txtInputFile.Text.LastIndexOf("\\") + 1, txtInputFile.Text.LastIndexOf(".") - txtInputFile.Text.LastIndexOf("\\") - 1);
            node = listTreeHUSP.Nodes.Add("HUSP-NIV Algorithm in database " + file + " with minUntil = " + txtminUntil.Text);
            switch (file)
            {
                case "kosarak10k":
                    {
                        node.Nodes.Add("Size: 0.98 MB, #Sequences: 10000, #Distinct items: 10094, Avg seq length: 8.14");
                        break;
                    }
                case "sign":
                    {
                        node.Nodes.Add("Size: 375 KB, #Sequences: 800, #Distinct items: 267, Avg seq length: 51.99");
                        break;
                    }
                case "fifa":
                    {
                        node.Nodes.Add("Size: 7.59 MB, #Sequences: 20450, #Distinct items: 2990, Avg seq length: 34.74");
                        break;
                    }
                case "bible":
                    {
                        node.Nodes.Add("Size: 8.56 MB, #Sequences: 36369, #Distinct items: 13905, Avg seq length: 21.64");
                        break;
                    }
                case "bmswebview1":
                    {
                        node.Nodes.Add("Size: 2.80 MB, #Sequences: 59601, #Distinct items: 497, Avg seq length: 2.51");
                        break;
                    }
                case "bmswebview2":
                    {
                        node.Nodes.Add("Size: 3.45 MB, #Sequences: 77512, #Distinct items: 3340, Avg seq length: 4.62");
                        break;
                    }
                case "kosarak990k":
                    {
                        node.Nodes.Add("Size: 57.2 MB, #Sequences: 990002, #Distinct items: 41270, Avg seq length: 8.14");
                        break;
                    }
            }
            node.Nodes.Add("Total time: ~" + (algo.endTimestamp - algo.startTimestamp) + " ms");
            node.Nodes.Add("Memory Usage: ~" + (algo.currentProc.PrivateMemorySize64 / 1024) / 1024 + "Mb");
            if (txtMaxLength.Text == "")
                node.Nodes.Add("Max length: All");
            else
                node.Nodes.Add("Max length: " + txtMaxLength.Text);
            TreeNode childNode = new TreeNode("High-utility sequential patterns count: " + algo.patternCount);
            node.Nodes.Add(childNode);
            for (int i = 0; i < algo.highUtilitySet.Count(); i++)
            {
                Dictionary<int[], IList<QMatrixProjection>> UtilityPattern = algo.highUtilitySet.ElementAt(i).Key;
                float utility = algo.highUtilitySet.ElementAt(i).Value;
                int[] items = UtilityPattern.First().Key;
                StringBuilder buffer = new StringBuilder();
                buffer.Append('<');
                buffer.Append('(');
                for (int j = 0; j < items.Length; j++)
                {
                    if (items[j] == -1)
                    {
                        buffer.Append(")(");
                    }
                    else
                    {
                        buffer.Append(items[j]);
                    }
                }
                buffer.Append(")>:");
                buffer.Append(utility);
                childNode.Nodes.Add(buffer.ToString());
            }
            listTreeHUSP.ExpandAll();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnShowData_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(txtInputFile.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(txtOutputFile.Text);
        }
    }
}
