// RA 1: 24129 - Guilherme Germano de Moraes Araujo
// RA 2: 24143 - Miguel Oliveira de Queiroz

using System;
using System.Collections.Generic;
using System.IO;

public enum Direcao { paraFrente, paraTras };

public class ListaDupla<Dado> where Dado : IComparable<Dado>, IRegistro
{
    /// <summary>
    /// É usado para percorrer a lista e mostrar
    /// o nó que está sendo visitado a cada momento
    /// </summary>
    NoDuplo<Dado> primeiro, ultimo, atual;

    int quantosNos;

    /// <summary>
    /// "Índice" do nó apontado por atual
    /// </summary>
    int numeroDoNoAtual;

    public bool EstaVazia { get => primeiro == null; }

    public NoDuplo<Dado> Primeiro { get => primeiro; }
    public NoDuplo<Dado> Ultimo { get => ultimo; }
    public NoDuplo<Dado> Atual { get => atual; }

    public int NumeroDoNoAtual
    {
        get => numeroDoNoAtual;
        set => numeroDoNoAtual = value;
    }

    public int QuantosNos { get => quantosNos; }

    public ListaDupla()
    {
        primeiro = ultimo = atual = null;
        quantosNos = numeroDoNoAtual = 0;
    }

    public void PosicionarNoInicio()
    {
        atual = primeiro;
        NumeroDoNoAtual = 1;
    }

    public void PosicionarNoFinal()
    {
        atual = ultimo;
        NumeroDoNoAtual = QuantosNos;
    }

    public void Avancar()
    {
        if (atual.Prox != null)
        {
            atual = atual.Prox;
            NumeroDoNoAtual += 1;
        }
    }

    public void Retroceder()
    {
        if (atual.Ant != null)
        {
            atual = atual.Ant;
            NumeroDoNoAtual -= 1;
        }
    }

    public void PosicionarEm(int indice)
    {
        // fica para vocês fazerem
        if (indice < 0 || indice >= quantosNos)
            return;

        atual = primeiro;
        int contador = 0;

        while (contador < indice)
        {
            atual = atual.Prox;
            contador++;
        }
        numeroDoNoAtual = contador + 1;
        
        // verificar se indice é válido ( >= 0 && < quantosNos)
        // se for valido:
        //    atual aponta o primeiro nó;
        //    percorre "indice" nós com o ponteiro atual sequencial
        //      atualiza a variável numeroDoNoAtual
    }

    public Dado this[int indice]
    {
        get
        {
            PosicionarEm(indice);
            return atual.Info;
        }
        set
        {
            PosicionarEm(indice);
            atual.Info = value;
        }
    }

    public List<Dado> Listagem(Direcao qualDirecao)
    {
        var dados = new List<Dado>();
        var referencia = atual;
        if (qualDirecao == Direcao.paraFrente)
        {
            atual = primeiro;     // posiciona ponteiro de percurso no 1o nó
            while (atual != null) // enquanto houver nós a visitar
            {
                dados.Add(atual.Info);  // inclui no listbox os dados do nó visitado agora
                atual = atual.Prox;     // avança o ponteiro de percurso para o nó seguinte
            }
        }
        else
        {
            atual = ultimo;       // posiciona ponteiro de percurso no último nó
            while (atual != null) // enquanto houver nós a visitar
            {
                dados.Add(atual.Info);  // inclui no listbox os dados do nó visitado agora
                atual = atual.Ant;      // retrocede o ponteiro de percurso para o nó anterior
            }
        }
        atual = referencia;
        return dados;
    }

    public void InserirAntesDoInicio(Dado novoDado)
    {
        var novoNo = new NoDuplo<Dado>(novoDado);

        if (EstaVazia)
            ultimo = novoNo;
        else
            primeiro.Ant = novoNo;

        novoNo.Prox = primeiro;
        primeiro = novoNo;
        quantosNos++;
    }

    public void InserirAposFim(Dado novoDado)
    {
        var novoNo = new NoDuplo<Dado>(novoDado);

        if (EstaVazia)
        {
            primeiro = novoNo;
        }
        else
        {
            ultimo.Prox = novoNo;
            novoNo.Ant = ultimo;
        }

        ultimo = novoNo;
        quantosNos++;
    }

    public void InserirAposFim(NoDuplo<Dado> noExistente)
    {
        if (noExistente != null)
        {
            if (EstaVazia)
            {
                primeiro = noExistente;
                primeiro.Ant = null;
            }
            else
            {
                ultimo.Prox = noExistente;
                noExistente.Ant = ultimo;
            }

            noExistente.Prox = null;
            ultimo = noExistente;
            quantosNos++;
        }
    }

    public bool Existe(Dado outroProcurado)
    {
        NoDuplo<Dado> referenciaAtual = atual;
        atual = primeiro;

        int referencia = NumeroDoNoAtual;
        NumeroDoNoAtual = 1;

        //	Em seguida, é verificado se a lista está vazia. Caso esteja, é
        //	retornado false ao local de chamada, indicando que a chave não foi
        //	encontrada, e atual e anterior ficam valendo null
        if (EstaVazia)
        {
            atual = referenciaAtual;
            numeroDoNoAtual = referencia;
            return false;
        }

        // a lista não está vazia, possui nós

        // dado procurado é menor que o primeiro dado da lista:
        // portanto, dado procurado não existe
        if (outroProcurado.CompareTo(primeiro.Info) < 0)
        {
            atual = referenciaAtual;
            numeroDoNoAtual = referencia;
            return false;
        }

        // dado procurado é maior que o último dado da lista:
        // portanto, dado procurado não existe

        if (outroProcurado.CompareTo(ultimo.Info) > 0)
        {
            atual.Ant = ultimo;
            atual = null;

            atual = referenciaAtual;
            numeroDoNoAtual = referencia;
            return false;
        }

        //	caso não tenha sido definido que a chave está fora dos limites de 
        //	chaves da lista, vamos procurar no seu interior
        //	o apontador atual indica o primeiro nó da lista e consideraremos que
        //	ainda não achou a chave procurada nem chegamos ao final da lista
        bool achou = false;
        bool fim = false;

        //	repete os comandos abaixo enquanto não achou o RA nem chegou ao
        //	final da pesquisa
        while (!achou && !fim)
        {
            // se o apontador atual vale null, indica final físico da lista
            if (atual == null)
            {
                fim = true;
            }

            // se não chegou ao final da lista, verifica o valor da chave atual
            // verifica igualdade entre chave procurada e chave do nó atual
            else if (outroProcurado.CompareTo(atual.Info) == 0)
            {
                achou = true;
            }

            // se chave atual é maior que a procurada, significa que
            // a chave procurada não existe na lista ordenada e, assim,
            // termina a pesquisa indicando que não achou. Anterior
            // aponta o nó anterior ao atual, que foi acessado na
            // última repetição
            else if (atual.Info.CompareTo(outroProcurado) > 0)
            {
                fim = true;
            }

            // se não achou a chave procurada nem uma chave > que ela,
            // então a pesquisa continua, de maneira que o apontador
            // anterior deve apontar o nó atual e o apontador atual
            // deve seguir para o nó seguinte
            //    anterior = atual;
            else
            {
                atual = atual.Prox;
                numeroDoNoAtual++;
            }
        }

        // por fim, caso a pesquisa tenha terminado, o apontador atual
        // aponta o nó onde está a chave procurada, caso ela tenha sido
        // encontrada, ou aponta o nó onde ela deveria estar para manter a
        // ordenação da lista. O apontador anterior aponta o nó anterior
        // ao atual

        if (!achou)
        {
            atual = referenciaAtual;
            numeroDoNoAtual = referencia;
        }

        return achou;   // devolve o valor da variável achou, que indica
    }

    public bool InserirEmOrdem(Dado dados)
    {
        // Existe() configura anterior e atual
        if (Existe(dados))
        {
            return false;
        }

        // aqui temos certeza de que a chave não existe
        // guardaremos os dados no novo nó
        if (EstaVazia) // se a lista está vazia, então o 	
        {
            InserirAntesDoInicio(dados);  // dado ficará como primeiro da lista
        }
        else if (atual == primeiro) // testa se nova chave < primeira chave
        {
            InserirAntesDoInicio(dados); // liga novo nó antes do primeiro
        }
        else if (atual == null) // testa se nova chave > última chave
        {
            InserirAposFim(dados);
        }
        else
        {
            InserirNoMeio(dados);  // insere entre os nós anterior e atual
        }

        return true;  // conseguiu incluir pois não é repetido
    }

    private void InserirNoMeio(Dado dados)
    {
        // Existe() encontrou intervalo de inclusão do novo nó (entre anterior e atual)

        var novo = new NoDuplo<Dado>(dados);

        atual.Ant.Prox = novo;    // Liga anterior ao novo
        novo.Ant = atual.Ant;     // Liga novo ao anterior,

        novo.Prox = atual;        // Por fim, liga novo no atual
        atual.Ant = novo;         // e atual no novo

        if (atual.Ant == ultimo)  // Se incluiu ao final da lista
        {
            ultimo = novo;        // atualiza o apontador ultimo
        }

        quantosNos++;             // Incrementa número de nós da lista
    }

    public bool Remover(Dado dadoARemover)
    {
        if (EstaVazia)
        {
            return false;
        }

        if (!Existe(dadoARemover))
        {
            return false;
        }

        // Aqui sabemos que o nó foi encontrado e o método
        // Existe() configurou os ponteiros atual e anterior
        // para delimitar onde está o nó a ser removido

        if (atual == primeiro)
        {
            primeiro = primeiro.Prox;

            // Removemos o único nó da lista
            if (primeiro == null)
            {
                ultimo = null;
            }
            else
            {
                primeiro.Ant = null;
            }
        }
        else if (atual == ultimo)
        {
            // Desliga o último nó
            ultimo = atual.Ant;
            atual.Ant.Prox = null;
        }
        else
        {
            // Nó interno a ser excluido
            atual.Ant.Prox = atual.Prox;
            atual.Prox.Ant = atual.Ant;
        }

        quantosNos--;
        return true;
    }

    public void GravarDados(string nomeArq)
    {
        var arquivo = new StreamWriter(nomeArq);

        atual = primeiro;

        while (atual != null)
        {
            arquivo.WriteLine(atual.Info.FormatoDeArquivo());
            atual = atual.Prox;
        }

        arquivo.Close();
    }

    public void Ordenar()
    {
        ListaDupla<Dado> listaDupla = new ListaDupla<Dado>(); // Lista reserva
        NoDuplo<Dado> menor = null;

        // Até esvaziar a lista original
        while (!EstaVazia)
        {
            menor = atual = primeiro;

            // Lê cada elemento da lista e procura pelo menor
            while (atual != null)
            {
                if (atual.Info.CompareTo(menor.Info) < 0)
                {
                    menor = atual;
                }

                atual = atual.Prox;
            }

            // Remover o menor nó
            if (menor == primeiro)
            {
                primeiro = primeiro.Prox;

                // Removemos o único nó da lista
                if (primeiro == null)
                {
                    ultimo = null;
                }
                else
                {
                    primeiro.Ant = null;
                }
            }
            else if (menor == ultimo)
            {
                // Desliga o último nó
                menor.Ant.Prox = null;
                ultimo = menor.Ant;
            }
            else
            {
                // Nó interno a ser excluido
                menor.Ant.Prox = menor.Prox;
                menor.Prox.Ant = menor.Ant;
            }

            // Adicionar a uma lista reserva
            listaDupla.InserirAposFim(menor);
        }

        // Associar a lista original (this) à lista reserva
        this.primeiro = listaDupla.primeiro;
        this.ultimo = listaDupla.ultimo;

        this.PosicionarNoInicio();
    }
}
