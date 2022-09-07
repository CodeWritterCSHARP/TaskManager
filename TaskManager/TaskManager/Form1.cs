using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TaskManager
{
    public partial class Form1 : Form
    {
        #region Saving paths and SavingList Initialization
        public static string folderpath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\NewFolder";
        public string txtfolderpath = folderpath + "\\newtxt.txt";
        public List<string> txtsaving = new List<string>();
        #endregion

        public Form1()
        {
            InitializeComponent();
            ScrollBarLoader();

            #region Folder exists checker and loader
            if (Directory.Exists(folderpath))
            {
                if (File.Exists(txtfolderpath))
                {
                    LoadindTXTFile();
                }
                else
                {
                    //Create a new file if it is not exists
                    using (StreamWriter sw = File.CreateText(txtfolderpath)) {sw.Flush();}
                }
            }
            else { Directory.CreateDirectory(folderpath); }
            MessageBox.Show("Folder and TXT have been created");
            #endregion
        }

        #region Main public usefull variables

        //Lists of user items
        public List<ListItem> listItems = new List<ListItem>();
        public List<SubListItem> subListItems = new List<SubListItem>();

        //Main Saving System In Current Memory
        //string - name of task, Color - status color, DateTime - deadline, string - compliting status of task, 
        //Bitmap - task image, int[] - checkboxes status, string[,] - name and discription of subtasks
        public Dictionary<int, Tuple<string, Color, DateTime, string, Bitmap, int[], string[,]>> dict = 
            new Dictionary<int, Tuple<string, Color, DateTime, string, Bitmap, int[], string[,]>>();

        //public changing variables
        public int currentTask; public Color curColor; public Bitmap bitmap; private bool btn9 = false;

        #region Languages
        //Arrays for languages
        string[] En = new string[] { "Task List", "Color of priority:", "Name of task:", "Subtask List", "Date (Deadline):", "Image:", "Status:", "Current color is:", "Drag and drop window",
            "Add a new task", "Change", "Add a new subtask", "Close", "R", "Y", "G", "Open manager", "Tutorial",
            "Name of the task", "Status", "See", "Name of subtask", "Status:"};

        string[] Ru = new string[] { "Задачи", "Цвет приоритета:", "Название задачи:", "Список подзадач", "Дата (Крайний срок):", "Изображение:", "Статус:", "Текущий цвет это:", "Перетащите файлы в окно",
            "Добавить задачу", "Изменить", "Добавить подзадачу", "Закрыть", "К", "Ж", "З", "Открыть путь", "Туториал",
            "Имя задачи", "Статус", "Осмотр", "Подзадача", "Статус:"};
        //Current language
        public int Lang = 0;
        #endregion
        #endregion

        #region All panels start parametrs
        //All panels start parametrs
        void ScrollBarLoader()
        {
            flowLayoutPanel1.HorizontalScroll.Maximum = 0; flowLayoutPanel1.HorizontalScroll.Visible = false;
            flowLayoutPanel1.VerticalScroll.Visible = false; flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel2.HorizontalScroll.Maximum = 0; flowLayoutPanel2.HorizontalScroll.Visible = false;
            flowLayoutPanel2.VerticalScroll.Visible = false; flowLayoutPanel2.AutoScroll = true;
            panel3.Visible = false;
            label12.Visible = false; label13.Visible = false; label14.Visible = false; //Labels for checking
            panel12Disable();
        }
        void panel12Disable() {
            panel12.Visible = false; button12.Enabled = false; button11.Enabled = false;
        }
        #endregion

        #region Adding a new task element Btn
        //Add a new task element
        private void button1_Click(object sender, EventArgs e)
        {
            listItems.Add(new ListItem());
            bitmap = null; subListItems.Clear(); flowLayoutPanel2.Controls.Clear(); label11.Text = ".../...";
            dict.Add(listItems.Count - 1, Tuple.Create("...", Color.Red, DateTime.Now, ".../...", bitmap, new int[0], new string[0,0]));

            //Adding tasks in panel with start parametrs
            listItems[listItems.Count - 1].NameGetter = "Name of the task"; listItems[listItems.Count - 1].StatusGetter = false;
            listItems[listItems.Count - 1].ColorGetter = Color.Red; listItems[listItems.Count - 1].Name = (listItems.Count - 1).ToString();
            listItems[listItems.Count - 1].number = listItems.Count - 1;

            //Langauge of added task
            if(Lang == 0)
            {
                listItems[listItems.Count - 1].label1.Text = En[18]; listItems[listItems.Count - 1].label2.Text = En[19];
                listItems[listItems.Count - 1].button1.Text = En[20];
            }
            if (Lang == 1)
            {
                listItems[listItems.Count - 1].label1.Text = Ru[18]; listItems[listItems.Count - 1].label2.Text = Ru[19];
                listItems[listItems.Count - 1].button1.Text = Ru[20];
            }

            flowLayoutPanel1.Controls.Add(listItems[listItems.Count - 1]);    
        }
        #endregion

        #region Sublist deleting and recompling Method
        //Sublist deleting and recompling Method
        public void Sublistdel(List<SubListItem> list)
        {
            for (int i = 0; i < subListItems.Count; i++)
            {
                if (subListItems[i].deletechecker == true)
                {
                    flowLayoutPanel2.Controls.Remove(subListItems[i/*Convert.ToInt32(subListItems[i].label1.Text) - 1*/]);
                    subListItems.Remove(subListItems[i]);
                    Sublistdel(subListItems); break;
                }
                //SubList fixed deleting counter
                for (int j = 0; j < subListItems.Count; j++) {subListItems[j].label1.Text = (j + 1).ToString();}
            }
        }
        #endregion

        #region Changing Btn
        //Changing Btn
        private void button2_Click(object sender, EventArgs e)
        {
            //Sublist deleting and recompling
            Sublistdel(subListItems);

            //Checking of status of checkboxes and of subtasks name and discription
            int counter1 = 0;
            string[,] names = new string[subListItems.Count + 1, subListItems.Count + 1]; names[0, 0] = "1";
            int[] mas = new int[subListItems.Count];
            for (int i = 0; i < subListItems.Count; i++)
            {
                if (subListItems[i].textBox2.Text.Length > 0) names[i + 1, 0] = subListItems[i].textBox2.Text; else names[i + 1, 0] = "empty"; //name
                if (subListItems[i].textBox1.Text.Length > 0) names[0, i + 1] = subListItems[i].textBox1.Text; else names[0, i + 1] = "empty"; //discription
                if (subListItems[i].checkBox1.Checked == true) { counter1++; mas[i] = i; } else mas[i] = -1;
            }

            //Changing task status if checkboxes true checked count = all checkboxed count
            if(subListItems.Count > 0) {
            ListItem L = this.Controls.Find(currentTask.ToString(), true).FirstOrDefault() as ListItem;
            if (counter1 == subListItems.Count) L.checkBox1.Checked = true;
            else L.checkBox1.Checked = false;
            }

            //Saving in dictionary all parametrs
            label11.Text = $"{counter1}/{subListItems.Count}";
            dict[currentTask] = Tuple.Create(textBox2.Text, curColor, dateTimePicker1.Value, $"{counter1}/{subListItems.Count}", bitmap, mas, names);

            ListItem listItem = this.Controls.Find(currentTask.ToString(), true).FirstOrDefault() as ListItem;

            listItem.panel2.BackColor = curColor;
            listItem.label1.Text = dict.ElementAt(currentTask).Value.Item1;
        }
        #endregion

        #region Adding a new subtask Btn
        //Add a new subtask
        private void button3_Click(object sender, EventArgs e)
        {
            subListItems.Add(new SubListItem());
            Sublistdel(subListItems);
            //Language added subtask changing
            if (Lang == 0) { subListItems[subListItems.Count - 1].label2.Text = En[21]; subListItems[subListItems.Count - 1].label3.Text = En[22]; }
            if (Lang == 1) { subListItems[subListItems.Count - 1].label2.Text = Ru[21]; subListItems[subListItems.Count - 1].label3.Text = Ru[22]; }
            flowLayoutPanel2.Controls.Add(subListItems[subListItems.Count-1]);
        }
        #endregion

        #region ColorOfTask
        private void button5_Click(object sender, EventArgs e)
        {
            curColor = Color.Red; if(Lang == 0) label9.Text = curColor.Name; if (Lang == 1) label9.Text = "Красный";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            curColor = Color.Yellow; if (Lang == 0) label9.Text = curColor.Name; if (Lang == 1) label9.Text = "Желтый";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            curColor = Color.Green; if (Lang == 0) label9.Text = curColor.Name; if (Lang == 1) label9.Text = "Зеленый";
        }
        #endregion

        #region OffVisibleOfMainPanel
        private void button4_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
        }
        #endregion

        #region DragAndDrop

        private void panel11_DragDrop(object sender, DragEventArgs e)
        {
            label10.Text = "Drag and drop window";
            string[] line = (string[])e.Data.GetData(DataFormats.FileDrop);

            if(line[0].ToString().EndsWith(".png") || line[0].ToString().EndsWith(".jpeg") || line[0].ToString().EndsWith(".jpg"))
            {
                bitmap = new Bitmap(line[0].ToString());
                pictureBox1.Image = bitmap;
            }
            else MessageBox.Show("Incorrect File Format");
        }

        private void panel11_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                label10.Text = "Drop files";
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void panel11_DragLeave(object sender, EventArgs e)
        {
            label10.Text = "Drag and drop window";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog() {
                InitialDirectory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}",
                Title = "Select an image",
                Filter = "Picture.png|*.png| Pictur.jpeg|*.jpeg| Pictur.jpg|*.jpg"
            })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    bitmap = new Bitmap(openFileDialog.FileName);
                    pictureBox1.Image = bitmap;
                }
            }
        }

        private void panel11_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black, 3);
            pen.DashPattern = new float[] { 6, 5};
            e.Graphics.DrawRectangle(pen, 1, 1, panel11.Width - 3, panel11.Height - 3);
        }
        #endregion

        #region LoadindTXTFile
        public void LoadindTXTFile()
        {
            List<string> textlines = new List<string>();
            textlines = File.ReadAllLines(txtfolderpath).ToList();
            for (int i = 0; i < textlines.Count; i++)
            {
                string i1 = ""; Color i2 = Color.Red; DateTime i3 = DateTime.Now;
                string i4 = ""; Bitmap i5 = null; int[] i6 = new int[0]; string[,] i7 = new string[0, 0];
                string LineS = textlines[i]; string adding = ""; int itemcounter = 0;
                for (int j = 0; j < LineS.Length; j++)
                {
                    if (LineS[j] != '!') adding += LineS[j];
                    else
                    {
                        itemcounter++;
                        switch (itemcounter)
                        {
                            //name of task
                            case 1: i1 = adding; break;

                            //status color
                            case 2: i2 = Color.FromName(adding); break;

                            //deadline
                            case 3: i3 = DateTime.Parse(adding); break;

                            //compliting status of task
                            case 4: i4 = adding; break;

                            //task image
                            case 5:
                                if (adding != "null")
                                {
                                    byte[] bytes = Convert.FromBase64String(adding);
                                    using (var ms = new MemoryStream(bytes)) { i5 = new Bitmap(ms); }
                                } else i5 = null;
                                break;

                            //checkboxes status
                            case 6:
                                if (adding != "null")
                                {
                                    List<int> intlist = new List<int>(); string ch = "";
                                    for (int n = 0; n < adding.Length; n++)
                                    {
                                        if (adding[n] != '=') ch += adding[n];
                                        else
                                        {
                                            intlist.Add(Convert.ToInt32(ch)); ch = "";
                                        }
                                    }
                                    i6 = intlist.ToArray(); i7 = new string[i6.Length + 1, i6.Length + 1]; i7[0, 0] = "1";
                                } else i6 = new int[0];
                                break;

                            //name and discription of subtasks
                            case 7:
                                if (i6.Length > 0) {
                                    string nn = ""; List<string> i71 = new List<string>(); i71.Add("null");
                                    for (int n = 0; n < adding.Length; n++)
                                    {
                                        if(adding[n] != '+') nn += adding[n];
                                        else
                                        {
                                            i71.Add(nn); nn = "";
                                        }
                                    }
                                    for (int n = 1; n < i71.Count; n++) {i7[n, 0] = i71[n];}
                                }else i7 = new string[0,0];
                                break;

                            case 8:
                                if (i6.Length > 0)
                                {
                                    string nn = ""; List<string> i71 = new List<string>(); i71.Add("null");
                                    for (int n = 0; n < adding.Length; n++)
                                    {
                                        if (adding[n] != '-') nn += adding[n];
                                        else
                                        {
                                            i71.Add(nn); nn = "";
                                        }
                                    }
                                    for (int n = 1; n < i71.Count; n++) { i7[0, n] = i71[n]; }
                                }else i7 = new string[0, 0];
                                break;
                            default: break;
                        }
                        adding = "";
                    }
                }
                //Adding elements in dictionary
                dict.Add(i, Tuple.Create(i1, i2, i3, i4, i5, i6, i7));

                //Adding tasks in panel
                listItems.Add(new ListItem());
                listItems[i].NameGetter = dict[i].Item1;
                if(dict[i].Item6.Length > 0) {
                    listItems[i].StatusGetter = true;
                    for (int n = 0; n < dict[i].Item6.Length; n++)
                    {
                        if (dict[i].Item6[n] == -1) { listItems[i].StatusGetter = false; break; }
                    }
                } else listItems[i].StatusGetter = false;
                listItems[i].ColorGetter = dict[i].Item2; curColor = dict[i].Item2;
                listItems[i].Name = i.ToString();
                listItems[i].number = i;
                listItems[i].panel2.BackColor = dict[i].Item2;
                listItems[i].label1.Text = dict[i].Item1;
                flowLayoutPanel1.Controls.Add(listItems[i]); bitmap = null;
            }
        }
        #endregion

        #region SavingInTXTFile

        public void ConvertingDict()
        {
            for (int i = 0; i < dict.Count; i++)
            {
                //Bitmap pixel data converting to string
                string PixelData = "null";
                if(dict[i].Item5 != null) {
                    ImageConverter imageConverter = new ImageConverter();
                    byte[] bytemas = (byte[])imageConverter.ConvertTo(dict[i].Item5, typeof(byte[]));
                    PixelData = Convert.ToBase64String(bytemas);
                }

                //Int[] data converting to string
                string inmas = "null";
                if (dict[i].Item6.Length > 0) {
                    inmas = "";
                    for (int j = 0; j < dict[i].Item6.Length; j++)
                    {
                        inmas += dict[i].Item6[j].ToString() + "=";
                    }
                }

                //String[,] data converting to two strings
                string first = "null"; string second = "null";
                if (inmas != "null") {
                    first = ""; second = "";
                    for (int n = 0; n < dict[i].Item6.Length; n++)
                    {
                        if (string.IsNullOrEmpty(dict[i].Item7[n + 1, 0])) first += "empty" + "+";
                        else first += dict[i].Item7[n + 1, 0] + "+";
                        if (string.IsNullOrEmpty(dict[i].Item7[0, n + 1])) second += "empty" + "-";
                        else second += dict[i].Item7[0, n + 1] + "-";
                    }
                }

                string line = dict[i].Item1 + "!" + dict[i].Item2.Name + "!" + dict[i].Item3.ToString() + "!" + dict[i].Item4 + "!" +
                    PixelData + "!" + inmas + "!" + first + "!" + second + "!";
                txtsaving.Add(line);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Directory.Exists(folderpath))
            {
                if(dict.Count > 0)
                {
                    if (File.Exists(txtfolderpath))
                    {
                        //Clean textfileinformation and write dictionary data in file
                        using (StreamWriter sw = File.CreateText(txtfolderpath))
                        {
                            sw.Flush();
                        }
                        ConvertingDict();
                        File.WriteAllLines(txtfolderpath, txtsaving);
                    }
                    else
                    {
                        //Write dictionary data in file
                        using (StreamWriter sw = File.CreateText(txtfolderpath))
                        {
                            ConvertingDict();
                            File.WriteAllLines(txtfolderpath, txtsaving);
                        }
                    }
                }
            }
            else { Directory.CreateDirectory(folderpath); }
            MessageBox.Show("Have been saved");
        }
        #endregion

        #region Language changing
        //Changing panel activation and disable
        private void button9_Click(object sender, EventArgs e)
        {
            btn9 = !btn9;
            if(btn9 == true) {panel12.Visible = true; button12.Enabled = true; button11.Enabled = true;}
            else { panel12Disable(); }
        }
        //English
        private void button12_Click(object sender, EventArgs e)
        {
            Lang = 0;
            List<Label> labels = new List<Label>() { label1, label2, label3, label4, label5, label6, label7, label8, label10};
            for (int i = 0; i < labels.Count; i++) { labels[i].Text = En[i]; }
            if (label9.Text != "...") label9.Text = curColor.Name;

            List<Button> buttons = new List<Button>() { button1, button2, button3, button4, button5, button6, button7, button8, button10 };
            for (int i = labels.Count; i < buttons.Count + labels.Count; i++) { buttons[i - labels.Count].Text = En[i]; }

            for (int i = 0; i < listItems.Count; i++)
            {
                if(listItems[i].label1.Text == Ru[18]) listItems[i].label1.Text = En[18];
                listItems[i].label2.Text = En[19]; listItems[i].button1.Text = En[20];
            }

            for (int i = 0; i < subListItems.Count; i++)
            {
                subListItems[i].label2.Text = En[21]; subListItems[i].label3.Text = En[22];
            }
        }
        //Russian
        private void button11_Click(object sender, EventArgs e)
        {
            Lang = 1;
            List<Label> labels = new List<Label>() { label1, label2, label3, label4, label5, label6, label7, label8, label10};
            for (int i = 0; i < labels.Count; i++) { labels[i].Text = Ru[i]; }
            if(label9.Text != "...") {
            switch (curColor.Name)
            {
                case "Red": label9.Text = "Красный"; break;
                case "Yellow": label9.Text = "Желтый"; break;
                case "Green": label9.Text = "Зеленый"; break;
                default: break;
            }
            }

            List<Button> buttons = new List<Button>() { button1, button2, button3, button4, button5, button6, button7, button8, button10 };
            for (int i = labels.Count; i < buttons.Count + labels.Count; i++) { buttons[i - labels.Count].Text = Ru[i]; }

            for (int i = 0; i < listItems.Count; i++)
            {
                if (listItems[i].label1.Text == En[18]) listItems[i].label1.Text = Ru[18];
                listItems[i].label2.Text = Ru[19]; listItems[i].button1.Text = Ru[20];
            }

            for (int i = 0; i < subListItems.Count; i++)
            {
                subListItems[i].label2.Text = Ru[21]; subListItems[i].label3.Text = Ru[22];
            }
        }
        #endregion

        #region Tutorial
        private void button10_Click(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
