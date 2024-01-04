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
using System.Data.SqlClient;
using System.Threading;

namespace PollutionMap
{
    public partial class Vizualizare : Form
    {

        SqlConnection con;
        SqlCommand cmd;
        StreamReader reader;
        SqlDataReader r;
        string harta;
        string data;
        string id;
        string[] bucati;
        string[] bucati1;
        int[] x = new int[100];
        int[] y = new int[100];
        int cnt;
        int[] valoare = new int[100];
        int filtru;
        int deseneaza = 0;
        int startx,  starty;
        int mx1 = 0, mx2 = 0, x1, y1, x2, y2;

        public Vizualizare(string numeutilizator)
        {
            InitializeComponent();
            label4.Text += numeutilizator;
            AppDomain.CurrentDomain.SetData("DataDirectory", System.Environment.CurrentDirectory.Replace("bin\\Debug", ""));
            con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Poluare.mdf;Integrated Security=True");
        }

        private void Vizualizare_Load(object sender, EventArgs e)
        {
            //populezi comboboxul din tabela harti
            con.Open();
            cmd = new SqlCommand("SELECT * FROM Harti;", con);
            r = cmd.ExecuteReader();
            while(r.Read())
            {
                comboBox1.Items.Add(r[1]);
            }
            con.Close();
            r.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            harta = comboBox1.SelectedItem.ToString();
            //adaugi in imagine harta aia
            con.Open();
            cmd = new SqlCommand(String.Format("SELECT * FROM Harti WHERE NumeHarta='{0}';",harta), con);
            r = cmd.ExecuteReader();
            while (r.Read())
            {
                id = r[0].ToString();
                pictureBox1.Image = Image.FromFile(r[2].ToString());
                pictureBox2.Image = Image.FromFile(r[2].ToString());
            }
            r.Close();
            con.Close();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            data = dateTimePicker1.Value.ToString();
            bucati = data.Split(' '); //pastrezi numa data fara ora


            con.Open();
            cmd = new SqlCommand(String.Format("SELECT * FROM Masurare WHERE IdHarta={0};", id,bucati[0]), con);
            r = cmd.ExecuteReader();
            while (r.Read())
            {
                //pentru fiecare masuratoare in care data e corecta, desenezi punctul
                string val = r[5].ToString();
                bucati1 = val.Split(' ');
                //MessageBox.Show(bucati[0]);
                //MessageBox.Show(bucati1[0]);
                if (String.Compare(bucati[0], bucati1[0]) == 0)
                {
                    int aux1 = Convert.ToInt32(r[2]);
                    int aux2 = Convert.ToInt32(r[3]);
                    int aux3 = Convert.ToInt32(r[4]);
                    x[cnt] = aux1;
                    y[cnt] = aux2;
                    valoare[cnt] = aux3;
                    cnt++;
                }
            }
            this.Refresh();
            r.Close();
            con.Close();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
                int i;
                for (i = 0; i < cnt; i++)
                {
                    Rectangle rect = new Rectangle(x[i], y[i], 20, 20);
                    Pen black = new Pen(Color.Black);
                    SolidBrush red = new SolidBrush(Color.Red);
                    SolidBrush green = new SolidBrush(Color.Green);
                    SolidBrush galben = new SolidBrush(Color.Yellow);
                    SolidBrush aux1 = new SolidBrush(Color.DarkRed);
                    SolidBrush aux2 = new SolidBrush(Color.DarkGreen);
                    SolidBrush aux3 = new SolidBrush(Color.Orange);
                    Font font = new Font("Arial", 12, FontStyle.Bold);
                    
                    if (valoare[i] > 40 && (filtru==0 || filtru ==3))
                    {
                        e.Graphics.DrawEllipse(black, rect);
                        e.Graphics.FillEllipse(red, rect);
                        e.Graphics.DrawString(valoare[i].ToString(), font , aux1, x[i], y[i]);
                    }
                    if (valoare[i] < 20 && (filtru == 0 || filtru == 1))
                    {
                        e.Graphics.DrawEllipse(black, rect);
                        e.Graphics.FillEllipse(green, rect);
                        e.Graphics.DrawString(valoare[i].ToString(), font, aux2, x[i], y[i]);
                    }
                    if (valoare[i] <= 40 && valoare[i] >= 20 && (filtru == 0 || filtru == 2))
                    {
                        e.Graphics.DrawEllipse(black, rect);
                        e.Graphics.FillEllipse(galben, rect);
                        e.Graphics.DrawString(valoare[i].ToString(), font, aux3, x[i], y[i]);
                    }                     
                }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            filtru = comboBox2.SelectedIndex;
            pictureBox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            filtru = 0;
            pictureBox1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //nui eventul bun
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int i;
            int ok = 1;
            for (i = 0; i < cnt; i++)
            {
                Rectangle rect = new Rectangle(e.X, e.Y, 10, 10);
                Rectangle rect2 = new Rectangle(x[i], y[i], 20, 20);
                if (rect.IntersectsWith(rect2))
                {
                    ok = 0;
                    break;
                }
            }
            if(ok==1)
            {
                AdaugaMasurare callable5 = new AdaugaMasurare(id,e.X, e.Y, data);
                callable5.Show();
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            //nimic
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            int i;
            for (i = 0; i < cnt; i++)
            {
                Rectangle rect = new Rectangle(x[i], y[i], 20, 20);
                Pen black = new Pen(Color.Black);
                SolidBrush red = new SolidBrush(Color.Red);
                SolidBrush green = new SolidBrush(Color.Green);
                SolidBrush galben = new SolidBrush(Color.Yellow);
                SolidBrush aux1 = new SolidBrush(Color.DarkRed);
                SolidBrush aux2 = new SolidBrush(Color.DarkGreen);
                SolidBrush aux3 = new SolidBrush(Color.Orange);
                Font font = new Font("Arial", 12, FontStyle.Bold);

                if (valoare[i] > 40 && (filtru == 0 || filtru == 3))
                {
                    e.Graphics.DrawEllipse(black, rect);
                    e.Graphics.FillEllipse(red, rect);
                    e.Graphics.DrawString(valoare[i].ToString(), font, aux1, x[i], y[i]);
                }
                if (valoare[i] < 20 && (filtru == 0 || filtru == 1))
                {
                    e.Graphics.DrawEllipse(black, rect);
                    e.Graphics.FillEllipse(green, rect);
                    e.Graphics.DrawString(valoare[i].ToString(), font, aux2, x[i], y[i]);
                }
                if (valoare[i] <= 40 && valoare[i] >= 20 && (filtru == 0 || filtru == 2))
                {
                    e.Graphics.DrawEllipse(black, rect);
                    e.Graphics.FillEllipse(galben, rect);
                    e.Graphics.DrawString(valoare[i].ToString(), font, aux3, x[i], y[i]);
                }
            }
            if(deseneaza==1)
            {
                mx1 = 0;
                mx2 = 0;
                for (i=0; i<cnt; i++)
                {
                    if(mx1<valoare[i])
                    {
                        mx1 = valoare[i];
                        x1 = x[i];
                        y1 = y[i];
                    }
                }
                for (i = 0; i < cnt; i++)
                {
                    if (mx2 < valoare[i] && valoare[i]!=mx1)
                    {
                        mx2 = valoare[i];
                        x2 = x[i];
                        y2 = y[i];
                    }
                }
                if ((x1 == startx && y1 == starty) || (x2 == startx && y2 == starty))
                {
                    //desenezi doar o linie
                    //MessageBox.Show("entered2");
                    Pen red = new Pen(Color.Red);
                    e.Graphics.DrawLine(red, x1, y1, x2, y2);
                }
                else
                {
                    //desenezi 2 linii
                    //MessageBox.Show("entered");
                    Pen red = new Pen(Color.Red);
                    e.Graphics.DrawLine(red, x1, y1, x2, y2);
                    int patrat1 = (x1 - startx) * (x1 - startx) + (y1 - starty) * (y1 - starty);
                    int patrat2 = (x2 - startx) * (x2 - startx) + (y2 - starty) * (y2 - starty);
                    if (patrat1 < patrat2)
                        e.Graphics.DrawLine(red, startx, starty, x1, y1);
                    else
                        e.Graphics.DrawLine(red, startx, starty, x2, y2);
                }
            }
        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            deseneaza = 1;
            startx = e.X;
            starty = e.Y;
            pictureBox2.Refresh();
        }
    }
}
