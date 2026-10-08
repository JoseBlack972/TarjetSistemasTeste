# Projeto 1 - Cálculo de Comissão de Vendedores

Este projeto implementa o cálculo de comissões de vendas de um time comercial a partir de um JSON estruturado.

## Regras de Negócio
Para cada venda realizada:
- **Abaixo de R$ 100,00:** Não gera comissão (0%).
- **Abaixo de R$ 500,00 (de R$ 100,00 a R$ 499,99):** Gera 1% de comissão.
- **A partir de R$ 500,00:** Gera 5% de comissão.

## Estrutura do Projeto
- `CalculoComissao.csproj`: Arquivo de projeto .NET 8.
- `Program.cs`: Código-fonte com deserialização JSON, modelos (`Venda`, `RelatorioVendas`) e cálculo agrupado via LINQ.
- `vendas.json`: (Opcional) Permite passar um arquivo JSON externo via argumento.

## Como Executar

### Pré-requisitos
- .NET SDK 8.0 (ou compatível com .NET 6/7/8).

### Execução via Terminal
```bash
# Navegue até a pasta do projeto
cd Projeto1_CalculoComissao

# Execute o projeto com os dados embutidos
dotnet run

# Ou execute passando um arquivo JSON externo:
dotnet run -- caminho/para/seu_arquivo.json
```

## Resultado Esperado
```text
==========================================================================
               RELATÓRIO CONSOLIDADO DE COMISSÕES DE VENDAS               
==========================================================================
Vendedor             | Qtd. Vendas |    Total Vendido |   Comissão Total
-----------------------------------------------------------------------
Ana Lima             |           9 |      R$ 8.763,95 |        R$ 404,98
Carlos Oliveira      |           8 |      R$ 7.928,35 |        R$ 379,37
João Silva           |          10 |     R$ 10.754,70 |        R$ 495,68
Maria Souza          |           9 |      R$ 9.874,30 |        R$ 465,95
-----------------------------------------------------------------------
TOTAL GERAL          |          36 |     R$ 37.321,30 |      R$ 1.745,98
==========================================================================
```
