using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CalculoComissao;

/// <summary>
/// Representa um registro individual de venda de um vendedor.
/// </summary>
public class Venda
{
    [JsonPropertyName("vendedor")]
    public string Vendedor { get; set; } = string.Empty;

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }

    /// <summary>
    /// Calcula a comissão para esta venda individual de acordo com as regras:
    /// - Abaixo de R$ 100,00: 0% (sem comissão)
    /// - Abaixo de R$ 500,00: 1% de comissão
    /// - A partir de R$ 500,00: 5% de comissão
    /// </summary>
    public decimal CalcularComissao()
    {
        if (Valor < 100.00m)
            return 0.00m;
        
        if (Valor < 500.00m)
            return Valor * 0.01m; // 1%
        
        return Valor * 0.05m; // 5%
    }

    public decimal ObterAliquota()
    {
        if (Valor < 100.00m) return 0.00m;
        if (Valor < 500.00m) return 0.01m;
        return 0.05m;
    }
}

public class RelatorioVendas
{
    [JsonPropertyName("vendas")]
    public List<Venda> Vendas { get; set; } = new();
}

public class Program
{
    private const string JsonPadrao = """
    {
      "vendas": [
        { "vendedor": "João Silva", "valor": 1200.50 },
        { "vendedor": "João Silva", "valor": 950.75 },
        { "vendedor": "João Silva", "valor": 1800.00 },
        { "vendedor": "João Silva", "valor": 1400.30 },
        { "vendedor": "João Silva", "valor": 1100.90 },
        { "vendedor": "João Silva", "valor": 1550.00 },
        { "vendedor": "João Silva", "valor": 1700.80 },
        { "vendedor": "João Silva", "valor": 250.30 },
        { "vendedor": "João Silva", "valor": 480.75 },
        { "vendedor": "João Silva", "valor": 320.40 },
        
        { "vendedor": "Maria Souza", "valor": 2100.40 },
        { "vendedor": "Maria Souza", "valor": 1350.60 },
        { "vendedor": "Maria Souza", "valor": 950.20 },
        { "vendedor": "Maria Souza", "valor": 1600.75 },
        { "vendedor": "Maria Souza", "valor": 1750.00 },
        { "vendedor": "Maria Souza", "valor": 1450.90 },
        { "vendedor": "Maria Souza", "valor": 400.50 },
        { "vendedor": "Maria Souza", "valor": 180.20 },
        { "vendedor": "Maria Souza", "valor": 90.75 },
        
        { "vendedor": "Carlos Oliveira", "valor": 800.50 },
        { "vendedor": "Carlos Oliveira", "valor": 1200.00 },
        { "vendedor": "Carlos Oliveira", "valor": 1950.30 },
        { "vendedor": "Carlos Oliveira", "valor": 1750.80 },
        { "vendedor": "Carlos Oliveira", "valor": 1300.60 },
        { "vendedor": "Carlos Oliveira", "valor": 300.40 },
        { "vendedor": "Carlos Oliveira", "valor": 500.00 },
        { "vendedor": "Carlos Oliveira", "valor": 125.75 },
        
        { "vendedor": "Ana Lima", "valor": 1000.00 },
        { "vendedor": "Ana Lima", "valor": 1100.50 },
        { "vendedor": "Ana Lima", "valor": 1250.75 },
        { "vendedor": "Ana Lima", "valor": 1400.20 },
        { "vendedor": "Ana Lima", "valor": 1550.90 },
        { "vendedor": "Ana Lima", "valor": 1650.00 },
        { "vendedor": "Ana Lima", "valor": 75.30 },
        { "vendedor": "Ana Lima", "valor": 420.90 },
        { "vendedor": "Ana Lima", "valor": 315.40 }
      ]
    }
    """;

    public static void Main(string[] args)
    {
        var cultura = new CultureInfo("pt-BR");
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        string jsonConteudo = JsonPadrao;
        if (args.Length > 0 && File.Exists(args[0]))
        {
            jsonConteudo = File.ReadAllText(args[0]);
            Console.WriteLine($"[INFO] Lendo dados do arquivo externo: {args[0]}");
        }

        var relatorio = JsonSerializer.Deserialize<RelatorioVendas>(jsonConteudo);
        if (relatorio == null || relatorio.Vendas.Count == 0)
        {
            Console.WriteLine("[AVISO] Nenhum registro de venda encontrado para processar.");
            return;
        }

        Console.WriteLine("==========================================================================");
        Console.WriteLine("               RELATÓRIO CONSOLIDADO DE COMISSÕES DE VENDAS               ");
        Console.WriteLine("==========================================================================");
        Console.WriteLine($"{"Vendedor",-20} | {"Qtd. Vendas",11} | {"Total Vendido",16} | {"Comissão Total",16}");
        Console.WriteLine(new string('-', 71));

        var agrupamento = relatorio.Vendas
            .GroupBy(v => v.Vendedor)
            .Select(g => new
            {
                Vendedor = g.Key,
                QtdVendas = g.Count(),
                TotalVendido = g.Sum(v => v.Valor),
                TotalComissao = g.Sum(v => v.CalcularComissao())
            })
            .OrderBy(r => r.Vendedor)
            .ToList();

        foreach (var vendedor in agrupamento)
        {
            Console.WriteLine($"{vendedor.Vendedor,-20} | {vendedor.QtdVendas,11} | {vendedor.TotalVendido.ToString("C2", cultura),16} | {vendedor.TotalComissao.ToString("C2", cultura),16}");
        }

        Console.WriteLine(new string('-', 71));
        decimal totalGeralVendas = relatorio.Vendas.Sum(v => v.Valor);
        decimal totalGeralComissao = relatorio.Vendas.Sum(v => v.CalcularComissao());
        Console.WriteLine($"{"TOTAL GERAL",-20} | {relatorio.Vendas.Count,11} | {totalGeralVendas.ToString("C2", cultura),16} | {totalGeralComissao.ToString("C2", cultura),16}");
        Console.WriteLine("==========================================================================");
        Console.WriteLine();

        Console.WriteLine("Regras aplicadas por venda:");
        Console.WriteLine(" • Menor que R$ 100,00: 0% de comissão");
        Console.WriteLine(" • De R$ 100,00 até R$ 499,99: 1% de comissão");
        Console.WriteLine(" • A partir de R$ 500,00: 5% de comissão");
    }
}
