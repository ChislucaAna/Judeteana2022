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
    public partial class Inregistrare : Form
    {

        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader r;

        public Inregistrare()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.SetData("DataDirectory", System.Environment.CurrentDirectory.Replace("bin\\Debug", ""));
            con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Poluare.mdf;Integrated Security=True");
        }
        string nume, email, parola,confirmare;

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            email = textBox3.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        { 
            Autentificare callable1 = new Autentificare();
            callable1.Show();
            this.Close();
        }

        private void Inregistrare_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            con.Open();
            if(nume.Length>4)
            {
                cmd = new SqlCommand(String.Format("SELECT * FROM Utilizatori WHERE NumeUtilizator='{0}'", nume),con);
                r = cmd.ExecuteReader();
                if(r.Read()==true)
                {
                    MessageBox.Show("numele exista deja");
                    r.Close();
                }
                else
                {
                    r.Close();
                    if (parola.Length<6 && String.Compare(parola,confirmare)!=0)
                    {
                        MessageBox.Show("parola prea scurta sau nu coincicde!");
                    }
                    else
                    {
                        string[] bucati = email.Split('@');
                        string[] bucati1 = email.Split('.');
                        
                        if (String.Compare(bucati[0], email)==0 || String.Compare(bucati1[0], email) == 0)
                        {
                            MessageBox.Show("emailul nu exista");
                        }
                        else
                        {
                            MessageBox.Show("contul nou a fost creat");
                            //introduci inregistrare in tabela
                            string ultima = DateTime.Now.ToString();
                            cmd = new SqlCommand(String.Format("INSERT INTO Utilizatori VALUES('{0}','{1}','{2}','{3}');",nume,parola,email,ultima),con);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("nume prea scrut");
            }
            con.Close();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            confirmare = textBox3.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            parola = textBox2.Text;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            nume = textBox1.Text;
        }
    }
}
