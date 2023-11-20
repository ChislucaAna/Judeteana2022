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
    public partial class Form1 : Form
    {

        SqlConnection con;
        SqlCommand cmd;
        StreamReader reader;
        string line;

        public Form1()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.SetData("DataDirectory", System.Environment.CurrentDirectory.Replace("bin\\Debug", ""));
            con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Poluare.mdf;Integrated Security=True");

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                cmd = new SqlCommand("DELETE FROM Harti;", con);
                cmd.ExecuteNonQuery();
                reader = new StreamReader("harti.txt");
                do
                {
                    line = reader.ReadLine();
                    string[] bucati = line.Split('#');
                    cmd = new SqlCommand(String.Format("INSERT INTO Harti VALUES('{0}','{1}');",bucati[0],bucati[1]), con);
                    cmd.ExecuteNonQuery();
                }
                while (reader.ReadLine() != null);

                cmd = new SqlCommand("DELETE FROM Masurare;", con);
                cmd.ExecuteNonQuery();
                reader = new StreamReader("masurari.txt");
                do
                {
                    line = reader.ReadLine();
                    string[] bucati = line.Split('#');
                    cmd = new SqlCommand(String.Format("INSERT INTO Masurare VALUES({0},{1},{2},{3},'{4}');", bucati[0], bucati[1],bucati[2],bucati[3],bucati[4]), con);
                    cmd.ExecuteNonQuery();
                }
                while (reader.ReadLine() != null);
                con.Close();
                
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
    }
}
