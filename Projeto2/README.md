# Projeto 2 - Controle de Movimentação de Estoque

Sistema em C# para controle de entradas e saídas de mercadorias em depósito a partir do catálogo inicial fornecido em JSON.

## Requisitos Atendidos
- **Identificador Único:** Cada movimentação recebe um número de identificação único (`Id`).
- **Descrição da Movimentação:** Campo textual obrigatório para categorizar o motivo da movimentação (ex: *Recebimento de Mercadoria*, *Venda ao Cliente*, *Devolução*).
- **Retorno da Quantidade Final:** O método de registro e a interface retornam imediatamente a quantidade em estoque atualizada do produto movimentado.
- **Validações de Regra de Negócio:**
  - Impede saídas caso o saldo seja insuficiente.
  - Valida a existência do produto.
  - Garante quantidades estritamente positivas.

## Estrutura do Projeto
- `ControleEstoque.csproj`: Configuração do projeto .NET 8.
- `Program.cs`: 
  - `Produto`: Entidade com código, descrição e saldo.
  - `Movimentacao`: Registro imutável de cada operação realizada.
  - `EstoqueService`: Encapsulamento das regras de negócio de estoque.
  - `Program`: Interface interativa via Console.

## Como Executar

### Pré-requisitos
- .NET SDK 8.0 (ou compatível com .NET 6/7/8).

### Execução via Terminal
```bash
cd Projeto2_ControleEstoque
dotnet run
```

## Menu do Sistema
1. **Consultar Catálogo e Estoque Atual**: Exibe tabela com código, produto e quantidade disponível.
2. **Lançar Movimentação**: Solicita código, tipo (Entrada/Saída), quantidade e descrição, exibindo o comprovante com o saldo final.
3. **Consultar Histórico**: Lista todas as movimentações realizadas com timestamp e saldo resultante.
0. **Sair**: Encerra o programa.
