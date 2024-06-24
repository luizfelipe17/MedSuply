using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MedSuply
{
    public partial class MedSuply : Form
    {

        MySqlConnection Conn;
        public MedSuply()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void MedSuply_Load(object sender, EventArgs e)
        {

            try
            {

                string conexao = "server = localhost; username = root; password = 1234; database = medsuply";
                Conn = new MySqlConnection(conexao);

                Conn.Open();

            }
            catch (Exception ex)
            {

                MessageBox.Show("Erro ao conectar ao banco de dados!!!" + ex.Message);

            }

            Conn.Close();

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void splitter4_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void BtnAdicionar_Click(object sender, EventArgs e)
        {

            object numeroSelecionado = downQuantidade.SelectedItem;

            string produto = cbProduto.Text;
            string quantidadeProduto = numeroSelecionado.ToString();

            string sql = "UPGRADE medsuply SET quantidade = (@valor) WHERE produto = (@valor1)";

            
        }
    }
}
