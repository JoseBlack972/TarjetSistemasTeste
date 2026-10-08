# Projeto 3 - Cálculo de Juros por Atraso

Programa em C# que calcula o valor dos juros de mora acumulados na data atual com base no valor original do título e na data de vencimento, aplicando a taxa de **2,5% ao dia**.

## Regra de Cálculo
- **Data Base:** Data atual (`DateTime.Today`).
- **Dias de Atraso:** Diferença em dias corridos entre a data de hoje e o vencimento (`DataAtual - DataVencimento`).
- **Se não houver atraso (vencimento hoje ou futuro):** Juros = R$ 0,00 e Total = Valor Original.
- **Se houver atraso:**
  $$\text{Juros} = \text{Valor Original} \times (0{,}025 \times \text{Dias de Atraso})$$
  $$\text{Total a Pagar} = \text{Valor Original} + \text{Juros}$$

## Estrutura do Projeto
- `CalculoJuros.csproj`: Configuração do projeto .NET 8.
- `Program.cs`:
  - `CalculadoraJuros`: Classe utilitária com lógica de cálculo desacoplada.
  - `ResultadoCalculoJuros`: Objeto com todos os valores calculados.
  - `Program`: Interface de console com formatação brasileira (`pt-BR`).

## Como Executar

### Pré-requisitos
- .NET SDK 8.0 (ou compatível com .NET 6/7/8).

### Execução via Terminal
```bash
cd Projeto3_CalculoJuros
dotnet run
```

## Exemplo Prático
- Data Atual: `08/10/2026`
- Data de Vencimento: `28/09/2026`
- Dias de Atraso: `10 dias`
- Valor Original: `R$ 1.000,00`
- Juros (2,5% * 10 = 25%): `R$ 250,00`
- Total a Pagar: `R$ 1.250,00`
