using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;

namespace MedSuply
{

    public partial class MedSuply : Form
    {

        /// 
        /// VARIAVEIS PARA CONECTAR AO BANCO DE DADOS
        ///
        MySqlConnection Conn;
        string conexao = "server = localhost; username = root; password = 1234; database = medsuply";
        public MedSuply()
        {
            InitializeComponent();
        }
        
        /// 
        /// CARREGAMENTO DO APLICATIVO
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MedSuply_Load(object sender, EventArgs e)
        {

            try
            {

                Conn = new MySqlConnection(conexao);

                Conn.Open();

                TelaInicial(false);

            }
            catch (Exception ex)
            {

                MessageBox.Show("Erro ao conectar ao banco de dados!!!" + ex.Message);

            }
            finally
            {

                Conn.Close();

            }
        }

        /// 
        /// IMPRIME A QUANTIDADE DE CADA PRODUTO DIRETAMENTO DO BANCO DE DADOS E CASO O BOOLEAN ESTEJA "TRUE" VALIDA
        /// SE O PRODUTO ESTÁ COM ESTOQUE ABAIXO DE 10%
        /// 
        /// <param name="id"></param> ID DO PRODUTO
        /// <param name="lblProduto"></param> NOME DO PRODUTO 
        /// <param name="validador"></param> VALIDADOR TRUE E FALSO, PARA VERIFICAR SE A AÇÃO VEIO DO BOTÃO REMOVER
        private void QuantidadeProdutos(int id, Label lblProduto, Boolean validador)
        {

            int qntMinECG = 300;
            int qntMinEEG = 200;
            int qntMinGEL = 200;
            int qntMinJOELHO = 50;

            try
            {

                using (MySqlConnection Conn = new MySqlConnection(conexao))
                {
                    Conn.Open();

                    // BUSCA NO BANCO DE DADOS A QUANTIDADE DO PRODUTO
                    string sql = "SELECT quantidade FROM medsuply WHERE id = @id";
                    MySqlCommand comando = new MySqlCommand(sql, Conn);
                    comando.Parameters.AddWithValue("@id", id);

                    object resultado = comando.ExecuteScalar();

                    int quantidade = Convert.ToInt32(resultado);

                    if (resultado != null)
                    {

                        lblProduto.Text = resultado.ToString();

                    }


                    if (validador == true)
                    {

                        // VALIDAR SE O ESTOQUE ESTÁ ABAIXO DE 10%
                        switch (id) 
                        {
                            case 1: 

                                if(quantidade < qntMinECG)
                                {

                                    MessageBox.Show("URGENTE\nELETRODOS PARA \"ECG\" ESTÁ COM BAIXO NIVEL");

                                }

                                break;

                            case 2:

                                if (quantidade < qntMinEEG)
                                {

                                    MessageBox.Show("URGENTE\nELETRODOS PARA \"EEG\" ESTÁ COM BAIXO NIVEL");

                                }

                                break;

                            case 3:

                                if (quantidade < qntMinGEL)
                                {

                                    MessageBox.Show("URGENTE\nGEL CONDUTOR - FRASCOS 0,5L ESTÁ COM BAIXO NIVEL");
                                    
                                }

                                break;

                            case 4:

                                if (quantidade < qntMinJOELHO)
                                {

                                    MessageBox.Show("URGENTE\nIMOBILIZADOR ARTICULADO PARA JOELHO ESTÁ COM BAIXO NIVEL");
                                   
                                }

                                break;

                            default:

                                MessageBox.Show("ENCONTRAMOS UM ERRO!!!");

                                break;  

                        }

                    }

                }
                

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar quantidade do produto: " + ex.Message);
            }


        }

        /// 
        /// ENVIAR PARA O MÉTODO "QUANTIDADEPRODUTOS" TODOS OS ID'S QUE TEM NO BANCO DE DADOS, 
        /// PARA QUE SEJÁ IMPRESSO
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="validador"></param>
        public void TelaInicial (Boolean validador)
        {
            
            QuantidadeProdutos(1, lblQuantidadeECG, validador);
            QuantidadeProdutos(2, lblQuantidadeEEG, validador);
            QuantidadeProdutos(3, lblQuantidadeGEL, validador);
            QuantidadeProdutos(4, lblQuantidadeIJoelho, validador);
            Grafico(); 

        }

        /// 
        /// AO CLICAR NO BOTÃO "ADICIONAR" SERÁ FEITO O INCREMENTO DA QUANTIDADE INFORMADA NO BANCO DE DADOS.
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAdicionar_Click(object sender, EventArgs e)
        {
            try
            {

                Conn = new MySqlConnection(conexao);

                string produto = cbProduto.Text;
                int quantidadeProduto = (int)downQuantidade.Value;

                // ATUALIZAR A QUANTIDADE NO BANCO DE DADOS, SOMANDO COM A QUANTIDADE JÁ PRESENTE NO BANCO
                string sql = "UPDATE medsuply SET quantidade = quantidade + " + quantidadeProduto + " WHERE produto = '" + cbProduto.Text + "'";

                if (quantidadeProduto > 0 
                    && produto == "ELETRODOS PARA \"ECG\""
                    || produto == "ELETRODOS PARA \"EEG\""
                    || produto == "GEL CONDUTOR - FRASCOS 0,5L"
                    || produto == "IMOBILIZADOR ARTICULADO PARA JOELHO")
                {

                    MySqlCommand comando = new MySqlCommand(sql, Conn);

                    Conn.Open();
                    comando.ExecuteReader();

                    MessageBox.Show("ITENS ADICIONADOS COM SUCESSO!!!");

                    TelaInicial(false);


                }
                else
                {

                    MessageBox.Show("INFORME UM VALOR VÁLIDO!");

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Erro ao adicionar item no Banco de Dados!" + ex);

            }
            finally
            {

                Conn.Close();

            }
        }

        /// <summary>
        /// AO CLICAR NO BOTÃO O MESMO FERÁ A EXCLUSÃO DA QUANTIDADE INFORMADA PELO USUARIO NO BANCO DE DADOS.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRemover_Click(object sender, EventArgs e)
        {

            try
            {

                Conn = new MySqlConnection(conexao);

                string produto = cbProduto.Text;
                int quantidadeProduto = (int)downQuantidade.Value;

                //VALIDAR SE A QUANTIDADE INFORMADA TEM EM ESTOQUE
                string sql = "SELECT quantidade FROM medsuply WHERE produto = '" + produto + "'";

                MySqlCommand comando = new MySqlCommand(sql, Conn);

                Conn.Open();

                object resultado = comando.ExecuteScalar();

                int quantidade = (int)resultado;

                if(quantidadeProduto <= quantidade)
                {
                    sql = "UPDATE medsuply SET quantidade = quantidade - " + quantidadeProduto + ", qntVendas = qntVendas + " + quantidadeProduto + " WHERE produto = '" + produto + "'";

                    if (quantidadeProduto > 0
                        && produto == "ELETRODOS PARA \"ECG\""
                        || produto == "ELETRODOS PARA \"EEG\""
                        || produto == "GEL CONDUTOR - FRASCOS 0,5L"
                        || produto == "IMOBILIZADOR ARTICULADO PARA JOELHO")
                    {

                        comando = new MySqlCommand(sql, Conn);

                        comando.ExecuteReader();

                        MessageBox.Show("ITENS REMOVIDOS COM SUCESSO!!");

                        TelaInicial(true);


                    }
                    else
                    {

                        MessageBox.Show("INFORME UM VALOR VÁLIDO!");

                    }
                }
                else
                {

                    MessageBox.Show("A QUANTIDADE INFORMADA É MAIOR DO QUE A DISPONÍVEL EM ESTOQUE!");

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("ERRO AO REMOVER ITEM DO BANCO DE DADOS!" + ex);

            }
            finally
            {

                Conn.Close();

            }

        }

        /// <summary>
        /// SERÁ INFORMADO QUE O PEDIDO SERÁ SOLICITADO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSolicitar_Click(object sender, EventArgs e)
        {

            string produto = cbProduto.Text;
            int quantidadeProduto = (int)downQuantidade.Value;

            if (quantidadeProduto > 0
            && produto == "ELETRODOS PARA \"ECG\""
            || produto == "ELETRODOS PARA \"EEG\""
            || produto == "GEL CONDUTOR - FRASCOS 0,5L"
            || produto == "IMOBILIZADOR ARTICULADO PARA JOELHO")
            {

                MessageBox.Show("PEDIDO FOI SOLICITADO SOLICITADO!!!");

            }
            else
            {

                MessageBox.Show("INFORME UM VALOR VÁLIDO!");

            }
        }

        /// <summary>
        /// ADICIONA O GRAFICO NO APLICATIVO
        /// </summary>
        public void Grafico()
        {

            try
            {

                using (MySqlConnection Conn = new MySqlConnection(conexao))
                {
                    int qntECG, qntEEG, qntGel, qntJoelho;

                    LimparGrafico();

                    // titulo grafico
                    Title title = new Title
                    {
                        Font = new Font("Arial", 14, FontStyle.Bold),
                        ForeColor = Color.Black,
                        Text = "Consumo Mensal"
                    };
                    graficoConsumo.Titles.Add(title);

                    //sub titulo
                    Title title2 = new Title
                    {
                        Font = new Font("Arial", 10),
                        ForeColor = Color.Black,
                        Text = "Mês Junho"
                    };

                    graficoConsumo.Titles.Add(title2);

                    graficoConsumo.Series.Add("produtos");
                    graficoConsumo.Series["produtos"].LegendText = "Qnt Produtos";

                    graficoConsumo.Series["produtos"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                    graficoConsumo.Series["produtos"].BorderWidth = 4;

                    // inserir legenda no grafico
                    Legend legend = new Legend();
                    graficoConsumo.Legends.Add(legend);
                    graficoConsumo.Legends[0].Title = "Legenda";

                    // retirar linhas verticais
                    graficoConsumo.ChartAreas["ChartArea1"].AxisX.MajorGrid.LineWidth = 0;

                    graficoConsumo.ChartAreas[0].AxisY.Maximum = 3000;

                    string sql = "SELECT qntVendas FROM medsuply WHERE id = 1";
                    MySqlCommand comando = new MySqlCommand(sql, Conn);

                    Conn.Open();

                    object resultado = comando.ExecuteScalar();

                    int quantidade = (int)resultado;

                    qntECG = quantidade;

                    sql = "SELECT qntVendas FROM medsuply WHERE id = 2";
                    comando = new MySqlCommand(sql, Conn);

                    resultado = comando.ExecuteScalar();

                    quantidade = (int)resultado;

                    qntEEG = quantidade;

                    sql = "SELECT qntVendas FROM medsuply WHERE id = 3";
                    comando = new MySqlCommand(sql, Conn);

                    resultado = comando.ExecuteScalar();

                    quantidade = (int)resultado;

                    qntGel = quantidade;

                    sql = "SELECT qntVendas FROM medsuply WHERE id = 4";
                    comando = new MySqlCommand(sql, Conn);

                    resultado = comando.ExecuteScalar();

                    quantidade = (int)resultado;

                    qntJoelho = quantidade;


                    graficoConsumo.Series["produtos"].Points.AddXY("ELETRODOS PARA \"ECG\"", qntECG);
                    graficoConsumo.Series["produtos"].Points.AddXY("ELETRODOS PARA \"EEG\"", qntEEG);
                    graficoConsumo.Series["produtos"].Points.AddXY("GEL CONDUTOR - FRASCOS 0,5L", qntGel);
                    graficoConsumo.Series["produtos"].Points.AddXY("IMOBILIZADOR ARTICULADO PARA JOELHO", qntJoelho);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERRO NO BANCO DE DADOS fe: " + ex.Message);
            }
            finally
            {

                Conn.Close();

            }
        }

        /// <summary>
        /// REALIZADO PARA APAGAR O GRAFICO.
        /// </summary>
        private void LimparGrafico()
        {

            graficoConsumo.Series.Clear();

            graficoConsumo.Series.Clear();

            graficoConsumo.Titles.Clear();

            graficoConsumo.Legends.Clear();

        }
    }
}
