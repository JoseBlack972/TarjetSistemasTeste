using System;
using System.Globalization;

namespace CalculoJuros;

/// <summary>
/// DTO contendo o resultado detalhado do cálculo de juros por atraso.
/// </summary>
public class ResultadoCalculoJuros
{
    public decimal ValorOriginal { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime DataCalculo { get; set; }
    public int DiasAtraso { get; set; }
    public decimal TaxaDiaria { get; set; }
    public decimal ValorJuros { get; set; }
    public decimal ValorTotalAtualizado { get; set; }
    public bool PossuiAtraso => DiasAtraso > 0;
}

/// <summary>
/// Serviço de domínio para cálculo de juros diários por mora.
/// </summary>
public static class CalculadoraJuros
{
    public const decimal TaxaDiariaPadrao = 0.025m; // 2,5% ao dia

    /// <summary>
    /// Calcula os juros considerando uma taxa diária (padrão 2,5% ao dia).
    /// </summary>
    /// <param name="valorOriginal">Valor original do título.</param>
    /// <param name="dataVencimento">Data de vencimento estipulada.</param>
    /// <param name="taxaDiaria">Taxa percentual ao dia (ex: 0.025 para 2,5%).</param>
    /// <param name="dataReferencia">Data base do cálculo (padrão: hoje).</param>
    public static ResultadoCalculoJuros Calcular(
        decimal valorOriginal, 
        DateTime dataVencimento, 
        decimal taxaDiaria = TaxaDiariaPadrao, 
        DateTime? dataReferencia = null)
    {
        if (valorOriginal <= 0)
            throw new ArgumentException("O valor original deve ser maior que zero.", nameof(valorOriginal));

        var dataHoje = (dataReferencia ?? DateTime.Today).Date;
        var vencimento = dataVencimento.Date;

        int diasAtraso = 0;
        decimal valorJuros = 0.00m;

        if (dataHoje > vencimento)
        {
            diasAtraso = (dataHoje - vencimento).Days;
            valorJuros = valorOriginal * (taxaDiaria * diasAtraso);
        }

        return new ResultadoCalculoJuros
        {
            ValorOriginal = valorOriginal,
            DataVencimento = vencimento,
            DataCalculo = dataHoje,
            DiasAtraso = diasAtraso,
            TaxaDiaria = taxaDiaria,
            ValorJuros = valorJuros,
            ValorTotalAtualizado = valorOriginal + valorJuros
        };
    }
}

public class Program
{
    public static void Main()
    {
        var cultura = new CultureInfo("pt-BR");
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=================================================================");
        Console.WriteLine("                  CÁLCULO DE JUROS POR ATRASO                    ");
        Console.WriteLine("                     (Multa: 2,5% ao dia)                        ");
        Console.WriteLine("=================================================================");
        Console.WriteLine($"Data de Referência (Hoje): {DateTime.Today:dd/MM/yyyy}");
        Console.WriteLine();

        Console.Write("Informe o valor do título/boleto (ex: 1250,50): R$ ");
        string? entradaValor = Console.ReadLine();
        if (!decimal.TryParse(entradaValor, NumberStyles.Currency, cultura, out decimal valor) || valor <= 0)
        {
            Console.WriteLine("[ERRO] Valor inválido. Digite um valor numérico positivo.");
            return;
        }

        Console.Write("Informe a data de vencimento (formato dd/MM/yyyy): ");
        string? entradaData = Console.ReadLine();
        if (!DateTime.TryParseExact(entradaData, "dd/MM/yyyy", cultura, DateTimeStyles.None, out DateTime vencimento))
        {
            Console.WriteLine("[ERRO] Data em formato inválido. Utilize o formato dd/MM/yyyy (ex: 15/09/2026).");
            return;
        }

        var resultado = CalculadoraJuros.Calcular(valor, vencimento);

        Console.WriteLine();
        Console.WriteLine("--------------------- RESUMO DO CÁLCULO -------------------------");
        Console.WriteLine($"Valor Original:            {resultado.ValorOriginal.ToString("C2", cultura)}");
        Console.WriteLine($"Data de Vencimento:        {resultado.DataVencimento:dd/MM/yyyy}");
        Console.WriteLine($"Data do Cálculo:           {resultado.DataCalculo:dd/MM/yyyy}");
        Console.WriteLine($"Dias em Atraso:            {resultado.DiasAtraso} dia(s)");
        Console.WriteLine($"Taxa de Juros Aplicada:    {(resultado.TaxaDiaria * 100):0.##}% ao dia");

        if (resultado.PossuiAtraso)
        {
            Console.WriteLine($"Valor dos Juros Acumulado: {resultado.ValorJuros.ToString("C2", cultura)}");
            Console.WriteLine($"TOTAL A PAGAR ATUALIZADO:  {resultado.ValorTotalAtualizado.ToString("C2", cultura)}");
        }
        else
        {
            Console.WriteLine("Status:                    Título em dia ou vencendo hoje.");
            Console.WriteLine($"TOTAL A PAGAR:             {resultado.ValorTotalAtualizado.ToString("C2", cultura)} (Sem juros)");
        }
        Console.WriteLine("-----------------------------------------------------------------");
    }
}
