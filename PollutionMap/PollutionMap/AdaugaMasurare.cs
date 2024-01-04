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
    public partial class AdaugaMasurare : Form
    {

        SqlConnection con;
        SqlCommand cmd;
        StreamReader reader;
        SqlDataReader r;

        string valoare;
        string cnt;
        int pozx, pozy;
        string date;

        public AdaugaMasurare(string id, int x, int y, string data)
        {
            InitializeComponent();
            AppDomain.CurrentDomain.SetData("DataDirectory", System.Environment.CurrentDirectory.Replace("bin\\Debug", ""));
            con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Poluare.mdf;Integrated Security=True");
            cnt = id;
            pozx = x;
            pozy = y;
            date = data;
        }

        private void AdaugaMasurare_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            valoare = textBox1.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            con.Open();
            string time = System.DateTime.Now.ToString();
            string[] bucati = time.Split(' ');
            date += ' ';
            date += bucati[0];
            cmd = new SqlCommand(String.Format("INSERT INTO Masurare VALUES({0},{1},{2},{3},{4});",cnt,pozx,pozy,valoare,date), con);
            cmd.ExecuteNonQuery();
            con.Close();
            Vizualizare.ActiveForm.Refresh();
            this.Hide();
        }
    }
}
