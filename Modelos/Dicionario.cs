// RA 1: 24129 - Guilherme Germano de Moraes Araujo
// RA 2: 24143 - Miguel Oliveira de Queiroz

using System;

namespace apListaLigada
{
    public class Dicionario : IComparable<Dicionario>, IRegistro
    {
        private const int startPalavra = 0, fimPalavra = 30, inicioDesc = fimPalavra - startPalavra; 
        private string palavra;
        private string dica;
        private bool[] acertou;

        public string Palavra
        {
            get => palavra;
            set
            {
                if (value == null || value == "")
                {
                    throw new Exception();
                }

                palavra = value.Substring(0, value.Length).Trim();
            }
        }

        public string Dica
        {
            get => dica;
            set
            {
                if (value == null || value == "")
                {
                    throw new Exception();
                }

                dica = value;
            }
        }

        public bool[] Acertou
        {
            get => acertou;
            private set { acertou = value; }
        }

        public Dicionario(string linhaDadosEntrada)
        {
            this.Palavra = linhaDadosEntrada.Substring(startPalavra, fimPalavra);
            this.Dica = linhaDadosEntrada.Substring(inicioDesc, linhaDadosEntrada.Length - inicioDesc);

            SetAcertou();
        }

        public Dicionario(string palavraEntrada, string descricaoEntrada)
        {
            this.Palavra = palavraEntrada;
            this.Dica = descricaoEntrada;

            SetAcertou();
        }

        public int CompareTo(Dicionario other)
        {
            return this.Palavra.CompareTo(other.Palavra);
        }

        public string FormatoDeArquivo()
        {
            return this.Palavra.PadRight(30, ' ') + this.Dica;
        }

        public void SetAcertou()
        {
            this.Acertou = new bool[Palavra.Length];
            for (int i = 0; i < Palavra.Length; i++)
            {
                this.Acertou[i] = false;
            }
        }

        public bool AtualizarAcertou(string valor)
        {
            bool achouAoMenosUm = false;
            for (int i = 0; i < Palavra.Length; i++)
            {
                char c = Palavra[i].ToString().ToUpper().ToCharArray()[0];
                if (c.Equals(valor.ToCharArray()[0]))
                {
                    this.Acertou[i] = true;
                    achouAoMenosUm = true;
                }
            }
            return achouAoMenosUm;
        }
    }
}
