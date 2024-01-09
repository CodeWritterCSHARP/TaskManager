using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskManager
{
    public partial class ListItem : UserControl
    {
        public ListItem()
        {
            InitializeComponent();
        }

        private string _nameoftask;
        private bool _status;
        private Color _color;
        public int number;

        #region GetterMethods
        public string NameGetter
        {
            get { return _nameoftask; }
            set { _nameoftask = value; label1.Text = value; }
        }

        public bool StatusGetter
        {
            get { return _status; }
            set { _status = value; checkBox1.Checked = value; }
        }

        public Color ColorGetter
        {
            get { return _color; }
            set { _color = value; panel2.BackColor = value; }
        }
        #endregion

        Form1 f = (Form1)Application.OpenForms["Form1"];

        #region SeeingBtn
        private void button1_Click(object sender, EventArgs e)
        {
            f = (Form1)Application.OpenForms["Form1"];
            f.currentTask = number; f.label14.Text = number.ToString();
            f.panel3.Visible = true;
            f.flowLayoutPanel2.Controls.Clear(); f.subListItems.Clear();
            f.textBox2.Text = f.dict.ElementAt(number).Value.Item1;
            f.dateTimePicker1.Value = f.dict.ElementAt(number).Value.Item3;
            f.pictureBox1.Image = f.dict.ElementAt(number).Value.Item5;
            for (int i = 0; i < f.dict.ElementAt(number).Value.Item6.Length; i++)
            {
                f.subListItems.Add(new SubListItem());
                f.subListItems[i].label1.Text = (i+1).ToString();
                f.subListItems[i].textBox2.Text = f.dict.ElementAt(number).Value.Item7[i + 1, 0];
                f.subListItems[i].textBox1.Text = f.dict.ElementAt(number).Value.Item7[0, i + 1];
                if (i == f.dict.ElementAt(number).Value.Item6[i]) { f.subListItems[i].checkBox1.Checked = true; }
                f.flowLayoutPanel2.Controls.Add(f.subListItems[i]);
            }
            f.label11.Text = f.dict.ElementAt(number).Value.Item4;
        }
        #endregion

        #region DeletingBtn
        private void button2_Click(object sender, EventArgs e)
        {
            f = (Form1)Application.OpenForms["Form1"];
            //f.label14.Text = number.ToString(); //checking parametr
            f.flowLayoutPanel1.Controls.Remove(f.listItems[number]);
            f.listItems.Remove(f.listItems[number]);

            //Clean all values
            f.flowLayoutPanel2.Controls.Clear(); f.subListItems.Clear(); f.textBox2.Text = null;
            f.dateTimePicker1.Value = DateTime.Now; f.pictureBox1.Image = null;
            if (f.listItems.Count == 0) {f.currentTask = 0;}

            //Rename all tasks names and numbers
            for (int i = 0; i < f.listItems.Count; i++)
            {
                f.listItems[i].Name = i.ToString();
                f.listItems[i].number = i;
            }

            //Rebuild a dictionary
            if (f.dict.Count > 1)
            {
                for (int i = number; i < f.dict.Count - 1; i++)
                {
                    f.dict[i] = f.dict[i + 1];
                }
                f.dict.Remove(f.dict.Keys.Last());
            }
            else f.dict.Clear();

            #region Check All Changes
            //f.label12.Text = ""; f.label13.Text = ""; f.label14.Text = "";
            //Dictionary<int, Tuple<string, Color, DateTime, string, Bitmap, int[], string[,]>>.KeyCollection Keys = f.dict.Keys;
            //foreach (int key in Keys) { f.label13.Text += key.ToString(); }
            //for (int i = 0; i < f.listItems.Count; i++) {f.label12.Text += f.listItems[i].number;}
            #endregion
        }
        #endregion

        #region ColorChangingForBtn
        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = Color.PeachPuff;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(224, 224, 224);
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.BackColor = Color.Red;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackColor = Color.FromArgb(224, 224, 224);
        }
        #endregion
    }
}
