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
    public partial class SubListItem : UserControl
    {
        Form1 f = (Form1)Application.OpenForms["Form1"];

        public SubListItem()
        {
            InitializeComponent();
        }

        #region DeletingBtn
        public int number;
        public bool deletechecker = false;
        private void button1_Click(object sender, EventArgs e)
        {
            f = (Form1)Application.OpenForms["Form1"];
            f.currentTask = number;
            deletechecker = true;
            f.Sublistdel(f.subListItems);
        }
        #endregion
    }
}
