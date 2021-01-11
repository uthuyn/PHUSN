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

public partial class FrmMain : Form
{
    public FrmMain()
    {
        InitializeComponent();
    }
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
            if (!txtInputFile.Text.Contains("Scalability Test"))
            {
                string file = txtInputFile.Text.Substring(txtInputFile.Text.LastIndexOf("\\") + 1, txtInputFile.Text.LastIndexOf(".") - txtInputFile.Text.LastIndexOf("\\") - 1);
            }
            else
            {
                string file2 = txtInputFile.Text.Substring(txtInputFile.Text.LastIndexOf("\\") + 1, txtInputFile.Text.LastIndexOf("_") - txtInputFile.Text.LastIndexOf("\\") - 1);
            }
        }
        else
            return;
    }
    public void addDataToTreeViewForPUSOM()
    {
        TreeNode node = new TreeNode();
        string file = txtInputFile.Text.Substring(txtInputFile.Text.LastIndexOf("\\") + 1, txtInputFile.Text.LastIndexOf(".") - txtInputFile.Text.LastIndexOf("\\") - 1);
        node = listTreeHUSP.Nodes.Add("HUS-Span-NIV algorithm in database " + file + " with minUntil = " + txtminUntil.Text);
        switch (file)
        {
            case ".kosarak10k":
                {
                    node.Nodes.Add("Size: 0.98 MB, #Sequences: 10000, #Distinct items: 10094, Avg seq length: 8.14");
                    break;
                }
            case ".sign":
                {
                    node.Nodes.Add("Size: 375 KB, #Sequences: 800, #Distinct items: 267, Avg seq length: 51.99");
                    break;
                }
            case ".fifa":
                {
                    node.Nodes.Add("Size: 7.59 MB, #Sequences: 20450, #Distinct items: 2990, Avg seq length: 34.74");
                    break;
                }
            case ".bible":
                {
                    node.Nodes.Add("Size: 8.56 MB, #Sequences: 36369, #Distinct items: 13905, Avg seq length: 21.64");
                    break;
                }
            case ".bms1":
                {
                    node.Nodes.Add("Size: 2.80 MB, #Sequences: 59601, #Distinct items: 497, Avg seq length: 2.51");
                    break;
                }
            case ".bms2":
                {
                    node.Nodes.Add("Size: 3.45 MB, #Sequences: 77512, #Distinct items: 3340, Avg seq length: 4.62");
                    break;
                }
            case ".syn":
                {
                    node.Nodes.Add("Size: 156 MB, #Sequences: 920467, #Distinct items: 10000, Avg seq length: 5");
                    break;
                }
        }
        node.Nodes.Add("Total time: ~" + (pusom.endTimestamp - pusom.startTimestamp) + " ms");
        node.Nodes.Add("Memory Usage: ~" + (pusom.currentProc.PrivateMemorySize64 / 1024) / 1024 + "Mb");
        if (txtMaxLength.Text == "")
            node.Nodes.Add("Max length: All");
        else
            node.Nodes.Add("Max length: " + txtMaxLength.Text);
        TreeNode childNode = new TreeNode("High utility sequential patterns: " + pusom.patternCount);
        node.Nodes.Add(childNode);
        for (int i = 0; i < pusom.highUtilitySet.Count(); i++)
        {
            int[] items = pusom.highUtilitySet.ElementAt(i).Key;
            float utility = pusom.highUtilitySet.ElementAt(i).Value;
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
    PUSOM pusom;

    private void btnOutput_Click(object sender, EventArgs e)
    {
        System.Diagnostics.Process.Start(txtOutputFile.Text);
    }

    private void btn_HUS_Span_Niv_Click(object sender, EventArgs e)
    {
        pusom = new PUSOM();
        listTreeHUSP.Nodes.Clear();
        string external = txtInputFile.Text.Substring(0, txtInputFile.Text.LastIndexOf("_"));
        external += "_ExternalUtility_neg.txt";
        Random r = new Random();
        int name = int.Parse(r.Next().ToString());
        // the path for saving the patterns found
        string output = ".//output" + name.ToString() + ".txt";
        // run the algorithm
        if (txtminUntil.Text == "")
            MessageBox.Show("Input minimum utility");
        else
        {
            if (txtMaxLength.Text != "")
            {
                pusom.setMaxPatternLength(int.Parse(txtMaxLength.Text));
            }
            pusom.runAlgorithmforPUSOM(external, txtInputFile.Text, float.Parse(txtminUntil.Text), output);
            MessageBox.Show("Finish!!!");
            addDataToTreeViewForPUSOM();
        }
    }
}
