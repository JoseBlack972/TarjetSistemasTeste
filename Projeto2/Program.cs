using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ControleEstoque;

public enum TipoMovimentacao
{
    Entrada = 1,
    Saida = 2
}

public class Produto
{
    [JsonPropertyName("codigoProduto")]
    public int CodigoProduto { get; set; }

    [JsonPropertyName("descricaoProduto")]
    public string DescricaoProduto { get; set; } = string.Empty;

    [JsonPropertyName("estoque")]
    public int Estoque { get; set; }
}

public class EstoqueContainer
{
    [JsonPropertyName("estoque")]
    public List<Produto> Produtos { get; set; } = new();
}

public class Movimentacao
{
    public int Id { get; set; }
    public int CodigoProduto { get; set; }
    public string DescricaoProduto { get; set; } = string.Empty;
    public TipoMovimentacao Tipo { get; set; }
    public int Quantidade { get; set; }
    public string DescricaoMovimentacao { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
    public int EstoqueAnterior { get; set; }
    public int EstoqueFinal { get; set; }
}

public class EstoqueService
{
    private readonly Dictionary<int, Produto> _produtos;
    private readonly List<Movimentacao> _movimentacoes = new();
    private int _proximoId = 1;

    public EstoqueService(IEnumerable<Produto> produtosIniciais)
    {
        _produtos = produtosIniciais.ToDictionary(p => p.CodigoProduto);
    }

    public IEnumerable<Produto> ObterTodos() => _produtos.Values;

    public Produto? ObterPorCodigo(int codigo) => _produtos.GetValueOrDefault(codigo);

    public Movimentacao RegistrarMovimentacao(int codigoProduto, TipoMovimentacao tipo, int quantidade, string descricaoMovimentacao)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade movimentada deve ser estritamente positiva.");

        if (string.IsNullOrWhiteSpace(descricaoMovimentacao))
            throw new ArgumentException("A descrição da movimentação é obrigatória para identificação.");

        if (!_produtos.TryGetValue(codigoProduto, out var produto))
            throw new KeyNotFoundException($"Produto com código {codigoProduto} não encontrado no catálogo.");

        int estoqueAnterior = produto.Estoque;
        int estoqueFinal;

        if (tipo == TipoMovimentacao.Saida)
        {
            if (produto.Estoque < quantidade)
                throw new InvalidOperationException($"Saldo insuficiente para saída! Estoque atual: {produto.Estoque}, solicitado: {quantidade}.");

            produto.Estoque -= quantidade;
            estoqueFinal = produto.Estoque;
        }
        else // Entrada
        {
            produto.Estoque += quantidade;
            estoqueFinal = produto.Estoque;
        }

        var mov = new Movimentacao
        {
            Id = _proximoId++,
            CodigoProduto = produto.CodigoProduto,
            DescricaoProduto = produto.DescricaoProduto,
            Tipo = tipo,
            Quantidade = quantidade,
            DescricaoMovimentacao = descricaoMovimentacao.Trim(),
            DataHora = DateTime.Now,
            EstoqueAnterior = estoqueAnterior,
            EstoqueFinal = estoqueFinal
        };

        _movimentacoes.Add(mov);
        return mov;
    }

    public IReadOnlyList<Movimentacao> ObterHistorico() => _movimentacoes.AsReadOnly();
}

public class Program
{
    private const string JsonEstoquePadrao = """
    {
        "estoque":
        [
          {
            "codigoProduto": 101,
            "descricaoProduto": "Caneta Azul",
            "estoque": 150
          },
          {
            "codigoProduto": 102,
            "descricaoProduto": "Caderno Universitário",
            "estoque": 75
          },
          {
            "codigoProduto": 103,
            "descricaoProduto": "Borracha Branca",
            "estoque": 200
          },
          {
            "codigoProduto": 104,
            "descricaoProduto": "Lápis Preto HB",
            "estoque": 320
          },
          {
            "codigoProduto": 105,
            "descricaoProduto": "Marcador de Texto Amarelo",
            "estoque": 90
          }
        ]
    }
    """;

    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var dados = JsonSerializer.Deserialize<EstoqueContainer>(JsonEstoquePadrao);
        var service = new EstoqueService(dados?.Produtos ?? new List<Produto>());

        bool ativo = true;
        while (ativo)
        {
            Console.WriteLine();
            Console.WriteLine("=================================================================");
            Console.WriteLine("              SISTEMA DE MOVIMENTAÇÃO DE ESTOQUE                 ");
            Console.WriteLine("=================================================================");
            Console.WriteLine("1. Consultar Catálogo e Estoque Atual");
            Console.WriteLine("2. Lançar Movimentação (Entrada ou Saída)");
            Console.WriteLine("3. Consultar Histórico de Movimentações");
            Console.WriteLine("0. Sair");
            Console.Write("Selecione uma opção: ");

            string? opcao = Console.ReadLine();
            Console.WriteLine();

            switch (opcao)
            {
                case "1":
                    ExibirEstoque(service);
                    break;
                case "2":
                    RealizarLancamento(service);
                    break;
                case "3":
                    ExibirHistorico(service);
                    break;
                case "0":
                    ativo = false;
                    Console.WriteLine("Encerrando o sistema de estoque. Até logo!");
                    break;
                default:
                    Console.WriteLine("[ERRO] Opção inválida. Tente novamente.");
                    break;
            }
        }
    }

    private static void ExibirEstoque(EstoqueService service)
    {
        Console.WriteLine($"{"Código",-8} | {"Descrição do Produto",-30} | {"Estoque Atual",13}");
        Console.WriteLine(new string('-', 58));
        foreach (var p in service.ObterTodos().OrderBy(p => p.CodigoProduto))
        {
            Console.WriteLine($"{p.CodigoProduto,-8} | {p.DescricaoProduto,-30} | {p.Estoque,13}");
        }
    }

    private static void RealizarLancamento(EstoqueService service)
    {
        ExibirEstoque(service);
        Console.WriteLine();

        Console.Write("Código do Produto: ");
        if (!int.TryParse(Console.ReadLine(), out int codigo))
        {
            Console.WriteLine("[ERRO] Código inválido.");
            return;
        }

        Console.Write("Tipo da Operação (1 = Entrada | 2 = Saída): ");
        if (!Enum.TryParse(Console.ReadLine(), out TipoMovimentacao tipo) || 
            (tipo != TipoMovimentacao.Entrada && tipo != TipoMovimentacao.Saida))
        {
            Console.WriteLine("[ERRO] Operação inválida. Informe 1 para Entrada ou 2 para Saída.");
            return;
        }

        Console.Write("Quantidade: ");
        if (!int.TryParse(Console.ReadLine(), out int quantidade) || quantidade <= 0)
        {
            Console.WriteLine("[ERRO] Quantidade deve ser um número inteiro positivo.");
            return;
        }

        Console.Write("Descrição/Motivo da Movimentação (ex: 'Compra NF 451', 'Venda Balcão'): ");
        string? descricao = Console.ReadLine();

        try
        {
            var mov = service.RegistrarMovimentacao(codigo, tipo, quantidade, descricao ?? string.Empty);
            
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("             MOVIMENTAÇÃO REGISTRADA COM SUCESSO!                ");
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine($"ID da Movimentação:   {mov.Id}");
            Console.WriteLine($"Data/Hora:            {mov.DataHora:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"Produto:              {mov.CodigoProduto} - {mov.DescricaoProduto}");
            Console.WriteLine($"Tipo da Operação:     {mov.Tipo}");
            Console.WriteLine($"Quantidade:           {mov.Quantidade}");
            Console.WriteLine($"Descrição/Motivo:     {mov.DescricaoMovimentacao}");
            Console.WriteLine($"Estoque Anterior:     {mov.EstoqueAnterior}");
            Console.WriteLine($"ESTOQUE FINAL:        {mov.EstoqueFinal}");
            Console.WriteLine("-----------------------------------------------------------------");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FALHA NA MOVIMENTAÇÃO]: {ex.Message}");
        }
    }

    private static void ExibirHistorico(EstoqueService service)
    {
        var historico = service.ObterHistorico();
        if (historico.Count == 0)
        {
            Console.WriteLine("Nenhuma movimentação registrada nesta sessão.");
            return;
        }

        Console.WriteLine($"{"ID",-4} | {"Data/Hora",-19} | {"Produto",-24} | {"Tipo",-7} | {"Qtd",5} | {"Estoque Final",13} | {"Descrição"}");
        Console.WriteLine(new string('-', 100));
        foreach (var m in historico)
        {
            Console.WriteLine($"{m.Id,-4} | {m.DataHora:dd/MM/yyyy HH:mm:ss} | {m.DescricaoProduto,-24} | {m.Tipo,-7} | {m.Quantidade,5} | {m.EstoqueFinal,13} | {m.DescricaoMovimentacao}");
        }
    }
}
