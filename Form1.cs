// RA 1: 24129 - Guilherme Germano de Moraes Araujo
// RA 2: 24143 - Miguel Oliveira de Queiroz

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace apListaLigada
{
    public enum Acao { Nenhum, Incluir, Editar, Excluir, Jogando };

    public partial class FrmAlunos : Form
    {
        Random random;
        ListaDupla<Dicionario> lista1;
        bool salvar = true;

        Acao acao = Acao.Nenhum;
        Dicionario noSalvo = null;

        int erros, pontos, timer;

        int pos;

        public FrmAlunos()
        {
            InitializeComponent();
        }

        private void FazerLeitura(ref ListaDupla<Dicionario> qualLista)
        {
            // instanciar a lista de palavras e dicas
            // pedir ao usuário o nome do arquivo de entrada
            // abrir esse arquivo e lê-lo linha a linha
            // para cada linha, criar um objeto da classe de Palavra e Dica
            // e inseri-lo no final da lista duplamente ligada
            qualLista = new ListaDupla<Dicionario>();

            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var reader = new StreamReader(ofd.FileName);
                while (!reader.EndOfStream) // Lê o arquivo entregue até o fim
                {
                    string line = reader.ReadLine();
                    qualLista.InserirAposFim(new Dicionario(line)); // Carrega no construtor e divide o texto
                }
                reader.Close();

                qualLista.Ordenar(); // Ordena os dados
            }
            else
            {
                salvar = false;
                MessageBox.Show("Você deve selecionar um arquivo para iniciar! Fechando agora...");

                Close();
            }
        }

        private void btnIncluir_Click(object sender, EventArgs e)
        {
            // se o usuário digitou palavra e dica:
            // criar objeto da classe Palavra e Dica para busca
            // tentar incluir em ordem esse objeto na lista1
            // se não incluiu (já existe) avisar o usuário
            string texto = txtPalavra.Text.Trim();
            string dica = txtDica.Text.Trim();

            if (string.IsNullOrWhiteSpace(texto) || string.IsNullOrWhiteSpace(dica))
            {
                MessageBox.Show("Digite a palavra e a dica para incluir:");
                return;
            }

            Dicionario nova = new Dicionario(texto, dica);
            if (!lista1.InserirEmOrdem(nova)) // Impede adicionar palavras já existentes
            {
                MessageBox.Show("Esta palavra já esta sendo utilizada.");
                return;
            }

            MessageBox.Show("Palavra incluida com sucesso!");

            lista1.Existe(nova); // Posiciona o ponteiro e o numero do nó 

            acao = Acao.Incluir; // Guarda os dados de cancelamento
            noSalvo = lista1[lista1.NumeroDoNoAtual - 1];
            
            ExibirRegistroAtual();
        }


        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // se a palavra digitada não é vazia:
            // criar um objeto da classe de Palavra e Dica para busca
            // se a palavra existe na lista1, posicionar o ponteiro atual nesse nó e exibir o registro atual
            // senão, avisar usuário que a palavra não existe
            // exibir o nó atual
            string texto = txtPalavra.Text.Trim();

            if (string.IsNullOrWhiteSpace(texto))
            {
                MessageBox.Show("Digite a palavra que deseja encontrar:");
                    return;
            }
            Dicionario encontrado = new Dicionario(texto, "sla");
           
            if (lista1.Existe(encontrado)) // Confirma se existe um termo com a palavra de "texto"
            {
                MessageBox.Show("Palavra encontrada!!");
            }
            else
            {
                MessageBox.Show("Palavra não encontrada!:(");
            }
            ExibirRegistroAtual();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            // para o nó atualmente visitado e exibido na tela:
            // perguntar ao usuário se realmente deseja excluir essa palavra e dica
            // se sim, remover o nó atual da lista duplamente ligada e exibir o próximo nó
            // se não, manter como está
            if(lista1.EstaVazia)
            {
                MessageBox.Show("A lista está vazia, não há o que excluir");
                return;
            }

            Dicionario atual = lista1[lista1.NumeroDoNoAtual - 1]; // Acessa o indice correto (n - 1)

            DialogResult opcao = MessageBox.Show(
                $"Deseja mesmo excluir a palavra: {atual.Palavra}?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (opcao == DialogResult.Yes) // Confirma que deve ser apagado
            {
                acao = Acao.Excluir; // Salva os dados para cancelamento
                noSalvo = atual;

                lista1.Remover(atual); // Remove

                MessageBox.Show("Palavra excluida com sucesso!");
                if (!lista1.EstaVazia)
                {
                    if (lista1.NumeroDoNoAtual - 1 < lista1.QuantosNos)
                        lista1.PosicionarEm(lista1.NumeroDoNoAtual - 1);
                    else
                        lista1.PosicionarEm(lista1.NumeroDoNoAtual - 2);
                    ExibirRegistroAtual();
                }
                else
                {
                    txtPalavra.Clear();
                    txtDica.Clear();
                    MessageBox.Show("A lista agora está vazia.");
                }
            }
            else
            {
                MessageBox.Show("Exclusão cancelada.");
            }
        }

        private void FrmAlunos_FormClosing(object sender, FormClosingEventArgs e)
        {
            // solicitar ao usuário que escolha o arquivo de saída
            // percorrer a lista ligada e gravar seus dados no arquivo de saída

            if (salvar)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    lista1.GravarDados(ofd.FileName);
                }
            }
        }

        private void ExibirDados(ListaDupla<Dicionario> aLista, ListBox lsb, Direcao qualDirecao)
        {
            lsb.Items.Clear();
            var dadosDaLista = aLista.Listagem(qualDirecao);

            foreach (Dicionario termo in dadosDaLista)
                lsb.Items.Add(termo.FormatoDeArquivo());
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            rbFrente.PerformClick();
        }

        private void rbFrente_Click(object sender, EventArgs e)
        {
            ExibirDados(lista1, lsbDados, Direcao.paraFrente);
        }

        private void rbTras_Click(object sender, EventArgs e)
        {
            ExibirDados(lista1, lsbDados, Direcao.paraTras);
        }

        private void FrmAlunos_Load(object sender, EventArgs e)
        {
            // fazer a leitura do arquivo escolhido pelo usuário e armazená-lo na lista1
            // posicionar o ponteiro atual no início da lista duplamente ligada
            // Exibir o Registro Atual;

            FazerLeitura(ref lista1);

            random = new Random();

            lista1.PosicionarNoInicio();
            ExibirRegistroAtual();
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            // posicionar o ponteiro atual no início da lista duplamente ligada
            // Exibir o Registro Atual;

            lista1.PosicionarNoInicio();
            ExibirRegistroAtual();
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            // Retroceder o ponteiro atual para o nó imediatamente anterior 
            // Exibir o Registro Atual;

            lista1.Retroceder();
            ExibirRegistroAtual();
        }

        private void btnProximo_Click(object sender, EventArgs e)
        {
            // Retroceder o ponteiro atual para o nó seguinte 
            // Exibir o Registro Atual;

            lista1.Avancar();
            ExibirRegistroAtual();
        }

        private void btnFim_Click(object sender, EventArgs e)
        {
            // posicionar o ponteiro atual no último nó da lista 
            // Exibir o Registro Atual;

            lista1.PosicionarNoFinal();
            ExibirRegistroAtual();
        }

        private void ExibirRegistroAtual()
        {
            // se a lista não está vazia:
            // acessar o nó atual e exibir seus campos em txtDica e txtPalavra
            // atualizar no status bar o número do registro atual / quantos nós na lista

            if (!lista1.EstaVazia)
            {
                Dicionario atual = lista1.Atual.Info;
                txtPalavra.Text = atual.Palavra;
                txtDica.Text = atual.Dica;

                slRegistro.Text = $"Registro: {lista1.NumeroDoNoAtual}/{lista1.QuantosNos}";
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            // alterar a dica e guardar seu novo valor no nó exibido
            string texto = txtPalavra.Text.Trim();
            string dica = txtDica.Text.Trim();

            if (string.IsNullOrWhiteSpace(texto) || string.IsNullOrWhiteSpace(dica))
            {
                MessageBox.Show("Digite a palavra que deseja editar e a dica respectiva!");
            }
            else
            {
                Dicionario atualizar = new Dicionario(texto, dica);

                // Posicionamos o ponteiro corretamente e verificamos a
                if (!lista1.Existe(atualizar)) // existencia da palavra
                {
                    MessageBox.Show("Esta palavra não existe! Verifique sua ortografia.");
                }
                else
                {
                    acao = Acao.Editar; // Guarda os dados para cancelamento
                    noSalvo = lista1.Atual.Info;

                    lista1.Atual.Info = atualizar;
                    MessageBox.Show("Dica editada com sucesso!");

                    ExibirRegistroAtual();
                }
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (acao != Acao.Nenhum && noSalvo != null)
            {
                DialogResult opcao = MessageBox.Show(
                    $"Deseja mesmo cancelar?",
                    "Confirmar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (opcao == DialogResult.Yes) // Confirma o pedido de cancelamento
                {
                    // Restauram os dados atualizados na ultima vez
                    if (acao == Acao.Incluir)
                    {
                        lista1.Remover(noSalvo);
                        noSalvo = null;

                        lista1.PosicionarEm(0);
                    }
                    else if (acao == Acao.Editar)
                    {
                        lista1.Existe(noSalvo);
                        lista1.Atual.Info = noSalvo;
                        noSalvo = null;
                    }
                    else if (acao == Acao.Excluir)
                    {
                        lista1.InserirEmOrdem(noSalvo);
                        noSalvo = null;

                        lista1.PosicionarEm(0);
                    }
                    ExibirDados(lista1, lsbDados, Direcao.paraFrente);
                    ExibirRegistroAtual();
                }
            }
            else
            {
                MessageBox.Show("Faça alguma alteração para cancelar!");
            }
        }

        // Jogo

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // Bloqueia trocar de tab enquanto o jogo rola
            if (acao == Acao.Jogando)
                e.Cancel = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            acao = Acao.Jogando;
            ResetTudo();

            toolStrip1.Enabled = false;

            int posicao = random.Next(lista1.QuantosNos - 1);
            lista1.PosicionarEm(posicao);

            var dicio = lista1.Atual.Info;
            string palavra = dicio.Palavra.ToUpper();

            SetDataGridViewWord(palavra.Length);

            if (chkDica.Checked)
            {
                UpdateLabel(lblDica, $"Dica: {dicio.Dica}");

                timer = 30;
                UpdateLabel(lblTempoRestante, $"Tempo Restante: {timer}s");

                tmrTempo.Start();
            }
        }

        private void tmrTempo_Tick(object sender, EventArgs e)
        {
            if (acao == Acao.Jogando)
            {
                timer--;
                UpdateLabel(lblTempoRestante, $"Tempo Restante: {timer}s");

                if (timer <= 0)
                {
                    tmrTempo.Stop();

                    UpdateBonecoMorreu();
                    Perdeu();
                }
            }
        }

        private void ProcessClick(object sender, EventArgs e)
        {
            if (acao == Acao.Jogando)
            {
                ((Control)sender).Enabled = false;

                var dicio = lista1.Atual.Info;

                if(!dicio.AtualizarAcertou(((Control)sender).Text))
                {
                    erros++;

                    UpdateLabel(lblErros, $"Erros: {erros}/8");
                }

                UpdateDataGridViewWord(dicio);

                UpdateBoneco();

                if(erros >= 8)
                {
                    Perdeu();
                }

                bool acertouTodas = true;
                int acertados = 0;
                foreach (var acerto in dicio.Acertou)
                {
                    if (!acerto)
                    {
                        acertouTodas = false;
                    }
                    else
                    {
                        acertados++;
                    }
                }

                pontos = acertados - erros;
                UpdateLabel(lblPontos, $"Pontos: {pontos}");

                if (acertouTodas)
                {
                    // ganhou
                    UpdateBonecoBandeira();
                    Ganhou();
                }
            }
        }

        private void SetDataGridViewWord(int Length)
        {
            dgvPalavra.Columns.Clear();
            dgvPalavra.ColumnCount = Length;
            dgvPalavra.ColumnHeadersVisible = false;
            foreach (DataGridViewColumn column in dgvPalavra.Columns)
                column.Width = 20;
        }

        private void UpdateDataGridViewWord(Dicionario dicio)
        {
            string palavra = dicio.Palavra;
            for (int i = 0; i < palavra.Length; i++)
            {
                if (dicio.Acertou[i])
                {
                    dgvPalavra[i, 0].Value = palavra[i].ToString().ToUpper();
                    dgvPalavra[i, 0].Style.BackColor = Color.Green;
                }
            }
        }

        private void UpdateLabel(Label lbl, string valor)
        {
            lbl.Text = valor;
        }

        private void UpdateBoneco()
        {
            switch (erros)
            {
                case 0: ResetBoneco(); break;
                case 1: UpdateBonecoCabeca(); break;
                case 2: UpdateBonecoCorpo(); break;
                case 3: UpdateBonecoFemur(); break;
                case 4: UpdateBonecoPernaEsquerda(); break;
                case 5: UpdateBonecoPernaDireita(); break;
                case 6: UpdateBonecoBracoEsquerda();  break;
                case 7: UpdateBonecoBracoDireita(); break;
                case 8: UpdateBonecoMorreu();  break;
            }
        }

        private void ResetTudo()
        {
            erros = 0;
            pontos = 0;
            timer = 0;

            UpdateLabel(lblDica, "Dica: ...");
            UpdateLabel(lblErros, $"Erros: {erros}/8");
            UpdateLabel(lblPontos, $"Pontos: {pontos}");
            UpdateLabel(lblTempoRestante, $"Tempo Restante: _s");

            UpdateBoneco();
            ResetButtons();
        }

        private void ResetButtons()
        {
            foreach (Control c in tpForca.Controls)
                if (c is Button)
                    c.Enabled = true;
        }

        private void ResetBoneco()
        {
            pbCabeca.Visible = false;
            pbCabecaMorreu.Visible = false;
            pbQueixo.Visible = false;
            pbCorpo.Visible = false;
            pbFemur.Visible = false;
            pbPernaEsquerda.Visible = false;
            pbBracoDireito.Visible = false;
            pbPernaDireita.Visible = false;
            pbBracoEsquerdo.Visible = false;
            pbBracoDireitoBandeira.Visible = false;

            pbBandeira1.Visible = false;
            pbBandeira2.Visible = false;
        }

        private void UpdateBonecoCabeca()
        {
            pbCabeca.Visible = true;
            pbQueixo.Visible = true;
        }

        private void UpdateBonecoCorpo()
        {
            pbCorpo.Visible = true;
        }

        private void UpdateBonecoFemur()
        {
            pbFemur.Visible = true;
        }

        private void UpdateBonecoPernaEsquerda()
        {
            pbPernaEsquerda.Visible = true;
        }

        private void UpdateBonecoPernaDireita()
        {
            pbPernaDireita.Visible = true;
        }

        private void UpdateBonecoBracoEsquerda()
        {
            pbBracoEsquerdo.Visible = true;
        }

        private void UpdateBonecoBracoDireita()
        {
            pbBracoDireito.Visible = true;
        }

        private void UpdateBonecoMorreu()
        {
            SetAllBoneco();

            pbCabeca.Visible = false;
            pbCabecaMorreu.Visible = true;
        }

        private void UpdateBonecoBandeira()
        {
            SetAllBoneco();

            pbBracoDireito.Visible = false;

            pbBracoDireitoBandeira.Visible = true;
            pbBandeira1.Visible = true;
            pbBandeira2.Visible = true;
        }

        private void SetAllBoneco()
        {
            UpdateBonecoCabeca();
            UpdateBonecoCorpo();
            UpdateBonecoFemur();
            UpdateBonecoPernaEsquerda();
            UpdateBonecoPernaDireita();
            UpdateBonecoBracoEsquerda();
            UpdateBonecoBracoDireita();
        }

        private void Perdeu()
        {
            RestartAll();

            MessageBox.Show("Você Perdeu!");

            AnjoSobeProCeu();
        }

        private void Ganhou()
        {
            RestartAll();
            MessageBox.Show("Você Ganhou!");
        }

        private void RestartAll()
        {
            lista1.Atual.Info.SetAcertou();
            acao = Acao.Nenhum;
            toolStrip1.Enabled = true;
        }

        private void AnjoSobeProCeu()
        {
            pos = 0;

            pbAnjo.Visible = true;

            tmrTempo.Interval = 10;

            tmrTempo.Tick -= tmrTempo_Tick;
            tmrTempo.Tick += MoverAnjo;
            tmrTempo.Start();
        }

        private void MoverAnjo(object sender, EventArgs e)
        {
            pos++;

            pbAnjo.Location = new Point(69, 62 - pos * 2);

            if (pos >= 140)
            {
                tmrTempo.Stop();
                tmrTempo.Tick -= MoverAnjo;
                tmrTempo.Tick += tmrTempo_Tick;

                pbAnjo.Visible = false;
                pbAnjo.Location = new Point(69, 62);

                tmrTempo.Interval = 1000;
            }
        }
    }
}
