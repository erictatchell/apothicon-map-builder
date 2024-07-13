using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MapConverter
{
    public partial class Form1 : Form
    {
        public Bitmap mapPNG;
        private ListViewItem selectedItem;
        public string destinationPath;
        public string txtContent;
        public Dictionary<Color, Point> pixels = new Dictionary<Color, Point>();
        public Color[] cols = new Color[256];

        public Form1()
        {
            InitializeComponent();

            // Initialize the TextBox
            textBox1.Visible = false;
            listView1.SmallImageList = imageList1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Colour", 100);
            listView1.Columns.Add("Colour Code", 100);
            listView1.Columns.Add("Tag (Tile ID)", 100);
        }

        private void ListView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                selectedItem = listView1.SelectedItems[0];
                if (selectedItem.SubItems.Count < 2)
                {
                    selectedItem.SubItems.Add("");
                }
                textBox1.Text = selectedItem.SubItems[2].Text;
                textBox1.Visible = true;
                textBox1.Focus();
            }
        }

        private void ListView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                selectedItem = listView1.SelectedItems[0];
                if (selectedItem.SubItems.Count < 2)
                {
                    selectedItem.SubItems.Add("");
                }
                textBox1.Text = selectedItem.SubItems[2].Text;
                textBox1.Visible = true;
                textBox1.Focus();
            }
        }

        private void EditTagTextBox_Leave(object sender, EventArgs e)
        {
            UpdateTag();
        }

        private void EditTagTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateTag();
            }
        }

        private void UpdateTag()
        {
            if (selectedItem != null)
            {
                selectedItem.SubItems[2].Text = textBox1.Text;
                selectedItem.Tag = int.TryParse(textBox1.Text, out int tag) ? tag : (int?)null;
                textBox1.Visible = false;
                selectedItem = null;
            }
        }

        // MAP PNG FILE
        private void button1_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    mapPNG = new Bitmap(filePath);
                    pictureBox1.Image = mapPNG;
                }
            }

            for (int i = 0; i < mapPNG.Height; i++)
            {
                for (int j = 0; j < mapPNG.Width; j++)
                {
                    Color px = mapPNG.GetPixel(i, j);
                    Point xy = new Point(i, j);
                    if (pixels.ContainsKey(px))
                    {
                        continue;
                    }
                    pixels.Add(px, xy);

                    Bitmap colorBitmap = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(colorBitmap))
                    {
                        g.Clear(px);
                    }
                    imageList1.Images.Add(colorBitmap);
                    ListViewItem item = new ListViewItem();
                    item.ImageIndex = imageList1.Images.Count - 1;
                    item.SubItems.Add(px.ToString());
                    item.SubItems.Add("");
                    listView1.Items.Add(item);
                }
            }
        }

        // DESTINATION MAP TXT FILE
        private void button2_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }

                    destinationPath = filePath;
                    txtContent = fileContent;
                    richTextBox1.Text = txtContent;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Dictionary<string, int> colorTagMap = new Dictionary<string, int>();
            foreach (ListViewItem item in listView1.Items)
            {
                string colorString = item.SubItems[1].Text;
                int tag = item.Tag != null ? (int)item.Tag : 0;
                colorTagMap[colorString] = tag;
            }

            StringBuilder output = new StringBuilder();
            for (int y = 0; y < mapPNG.Height; y++)
            {
                for (int x = 0; x < mapPNG.Width; x++)
                {
                    Color px = mapPNG.GetPixel(x, y); 
                    string pxString = px.ToString();
                    int tag = colorTagMap.ContainsKey(pxString) ? colorTagMap[pxString] : 0; // Default tag to 0 if not found
                    output.Append(tag + " ");
                }
                output.AppendLine();
            }

            richTextBox1.Text = output.ToString();
        }
    }
}
