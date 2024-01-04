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
    
    public partial class Autentificare : Form
    {

        SqlConnection con;
        SqlCommand cmd;
        StreamReader reader;
        SqlDataReader r;
        string line;
        string var; //parametru pt comezi sql
        string email;
        string parola;
        public string numeutilizator;

        public Autentificare()
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
                while ((line=reader.ReadLine())!= null)
                {
                    string[] bucati = line.Split('#');
                    cmd = new SqlCommand(String.Format("INSERT INTO Harti VALUES('{0}','{1}');",bucati[0],bucati[1]), con);
                    cmd.ExecuteNonQuery();
                }
                reader.Close();
                
                cmd = new SqlCommand("DELETE FROM Masurare;", con);
                cmd.ExecuteNonQuery();
                reader = new StreamReader("masurari.txt");
                while((line=reader.ReadLine())!=null)
                {
                    string[] bucati = line.Split('#');

                    //in fucntie de bucati[0]-numele hartii cauti id-ul harti si le interschimbi
                    cmd = new SqlCommand(String.Format("SELECT * FROM Harti WHERE NumeHarta='{0}';", bucati[0]),con);
                    r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        var = r[0].ToString();
                    }
                    r.Close();

                    string[] var1 = bucati[4].Split('/');
                    string text = String.Format("INSERT INTO Masurare VALUES({0},{1},{2},{3},'{4}/{5}/{6}');", var, bucati[1], bucati[2], bucati[3], var1[1],var1[0],var1[2]);
                    cmd = new SqlCommand(text, con);
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            email = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            parola = textBox2.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = new SqlCommand(String.Format("SELECT * FROM Utilizatori WHERE EmailUtilizator='{0}' AND Parola='{1}';",email,parola), con);
            r = cmd.ExecuteReader();
            if (r.Read()==true)
            {
                numeutilizator = r[1].ToString();
                r.Close();
                //gaseste numele utilizatorului logat ca sa-l transmiti formularului vizualizare
                
                //Deschide Formularul VIzualizare
                Vizualizare callable2 = new Vizualizare(numeutilizator);
                callable2.ShowDialog();
                this.Hide();
                //adauga data si ora curenta
                string now = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                cmd=new SqlCommand(String.Format("UPDATE Utilizatori SET UltimaUtilizare='{0}' WHERE EmailUtilizator='{1}';", now,email),con);
                cmd.ExecuteNonQuery();
            }
            else
            {
                MessageBox.Show("Contul nu exista!");
            }
            r.Close();
            con.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Inregistrare callable = new Inregistrare();
            callable.Show();
            this.Hide();
        }
    }
}
