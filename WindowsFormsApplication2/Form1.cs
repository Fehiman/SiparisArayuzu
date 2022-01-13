using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using MessagingToolkit.QRCode;
using MessagingToolkit.QRCode.Codec;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        SqlConnection baglanti =new SqlConnection(@"Data Source=ABRA\SQLEXPRESS01;Initial Catalog=SuperMarket;Integrated Security=True;MultipleActiveResultSets=True");
        QRCodeEncoder code = new QRCodeEncoder();
        Image resim;
        void listele()
        {
            
            DataTable dt = new DataTable();
            string sql = @"select s.id, MusAdi,Sehir,Eyalet,Bolge,Urun_adi,Kategori,Alt_kategori,Miktar,Fiyat from 
                    Musteri m inner join Konum k on m.Konum_id=k.id 
                    inner join Siparisler s on s.Mus_id=m.id 
                    inner join Urunler u on s.Urun_id=u.id ";

            if (!string.IsNullOrEmpty(textBox3.Text))
            {
                sql += " WHERE m.MusAdi LIKE '%" + textBox3.Text + "%'";
                sql += " OR k.Sehir LIKE '%" + textBox3.Text + "%'";
                sql += " OR u.Kategori LIKE '%" + textBox3.Text + "%'";
                sql += " OR u.Alt_kategori LIKE '%" + textBox3.Text + "%'";
                sql += " OR u.Urun_adi LIKE '%" + textBox3.Text + "%'";
                sql += " OR s.id LIKE '%" + textBox3.Text + "%'";
                
            }
            baglanti.Open();
            SqlDataAdapter da = new SqlDataAdapter(sql, baglanti);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            baglanti.Close();
            
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listele();
            combobox();
            combobox3();
            
          
            
        }
        void combobox()
        {
            SqlCommand sqlCmd = new SqlCommand("SELECT * FROM Konum", baglanti);
            baglanti.Open();
            SqlDataReader sqlReader = sqlCmd.ExecuteReader();

            while (sqlReader.Read())
            {
                comboBox1.Items.Add(sqlReader["Sehir"].ToString());
            }

            sqlReader.Close();
            
        }
        
        void combobox3()
        {
            SqlCommand sqlCmd = new SqlCommand("SELECT * FROM Urunler", baglanti);

            SqlDataReader sqlReader = sqlCmd.ExecuteReader();

            while (sqlReader.Read())
            {
                comboBox3.Items.Add(sqlReader["Urun_Adi"].ToString());
            }

            sqlReader.Close();
            baglanti.Close();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            string kayit1 = "select id from Konum where Sehir= @seh";
            string kayit2 = "insert into Musteri (MusAdi,Konum_id) values(@adi,@kon) SELECT SCOPE_IDENTITY()";
            string kayit3 = "select id from Urunler where Urun_adi = @urun";
            string kayit4 = "insert into Siparisler(Urun_id,Mus_id,Miktar) values(@uid,@mid,@mik)";

            SqlCommand komut1 = new SqlCommand(kayit1, baglanti);
            SqlCommand komut2 = new SqlCommand(kayit2, baglanti);
            SqlCommand komut3 = new SqlCommand(kayit3, baglanti);
            SqlCommand komut4 = new SqlCommand(kayit4, baglanti);

            komut1.Parameters.AddWithValue("@seh", comboBox1.Text);
            var KonID = komut1.ExecuteScalar();

            komut2.Parameters.AddWithValue("@adi", textBox1.Text);
            komut2.Parameters.AddWithValue("@kon", KonID);
            var MusID = komut2.ExecuteScalar();

            komut3.Parameters.AddWithValue("@urun", comboBox3.Text);
            var UrunID = komut3.ExecuteScalar();

            komut4.Parameters.AddWithValue("@uid", UrunID);
            komut4.Parameters.AddWithValue("@mid", MusID);
            komut4.Parameters.AddWithValue("@mik", textBox2.Text);
            komut4.ExecuteNonQuery();

            baglanti.Close();
            listele();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            var liste = dataGridView1.SelectedRows;
            foreach (DataGridViewRow row in liste)
            {
                SqlCommand komut = new SqlCommand("DELETE FROM Siparisler WHERE id = " + row.Cells["id"].Value, baglanti);
                komut.ExecuteNonQuery();
            }

            baglanti.Close();
            listele();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listele();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            var liste = dataGridView1.SelectedRows;
            int SipID = Convert.ToInt32(dataGridView1.SelectedCells[0].Value);
            baglanti.Open();

            string kayit1 = "select id from Urunler where Urun_adi= @uadi";
            string kayit2 = "update Siparisler set Miktar=@mik,Urun_id=@uid where id=@sid";
            string kayit3 = "select Mus_id from Siparisler where id=@sid";
            string kayit4 = "select id from Konum where Sehir=@seh ";
            string kayit5 ="update Musteri set MusAdi=@adi,Konum_id=@kid where id=@mid";

            SqlCommand komut1 = new SqlCommand(kayit1, baglanti);
            SqlCommand komut2 = new SqlCommand(kayit2, baglanti);
            SqlCommand komut3 = new SqlCommand(kayit3, baglanti);
            SqlCommand komut4 = new SqlCommand(kayit4, baglanti);
            SqlCommand komut5 = new SqlCommand(kayit5, baglanti);

            komut1.Parameters.AddWithValue("@uadi", comboBox3.Text);
            var Urun_id = komut1.ExecuteScalar();

            komut2.Parameters.AddWithValue("@sid",SipID);
            komut2.Parameters.AddWithValue("@mik",textBox2.Text);
            komut2.Parameters.AddWithValue("@uid",Urun_id);
            komut2.ExecuteNonQuery();

            komut3.Parameters.AddWithValue("@sid", SipID);
            var Mus_id = komut3.ExecuteScalar();

            komut4.Parameters.AddWithValue("@seh", comboBox1.Text);
            var Konum_id = komut4.ExecuteScalar();

            komut5.Parameters.AddWithValue("@adi",textBox1.Text);
            komut5.Parameters.AddWithValue("@kid",Konum_id);
            komut5.Parameters.AddWithValue("@mid", Mus_id);
            komut5.ExecuteScalar();

            baglanti.Close();
            listele();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            string ad = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            string sehir = dataGridView1.Rows[secilen].Cells[2].Value.ToString();
            string urun = dataGridView1.Rows[secilen].Cells[5].Value.ToString();
            string Miktar = dataGridView1.Rows[secilen].Cells[8].Value.ToString();

            textBox1.Text = ad;
            comboBox1.Text = sehir;
            comboBox3.Text = urun;
            textBox2.Text = Miktar;
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string sql = @"select distinct Kategori,Alt_kategori from Urunler" ;

            baglanti.Open();
            SqlDataAdapter da = new SqlDataAdapter(sql, baglanti);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            baglanti.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string sql = @"select Sum(Miktar) as 'Toplam Miktar' from Siparisler";

            baglanti.Open();
            SqlDataAdapter da = new SqlDataAdapter(sql, baglanti);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            baglanti.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            string a = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            string s = dataGridView1.Rows[secilen].Cells[2].Value.ToString();
            string u = dataGridView1.Rows[secilen].Cells[5].Value.ToString();
            string m = dataGridView1.Rows[secilen].Cells[8].Value.ToString();
         
            resim = code.Encode(a +
                s + 
                u +
                m);
            pictureBox1.Image = resim;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            listele();
        }
    }
}
