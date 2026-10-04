# Desafio Target Sistemas — Desenvolvedor de Sistemas Jr.

Aplicação de console em C#/.NET 8 que resolve as três questões propostas:

1. cálculo de comissão por vendedor a partir do JSON de vendas;
2. entradas e saídas de estoque com identificador único e saldo final;
3. cálculo de juros de 2,5% ao dia para valores vencidos.

## Decisões de interpretação

O enunciado contém alguns limites e comportamentos não totalmente explícitos. Foram adotadas as seguintes regras:

- venda menor que R$ 100,00: sem comissão;
- venda de R$ 100,00 até R$ 499,99: 1% de comissão;
- venda a partir de R$ 500,00, inclusive: 5% de comissão;
- a comissão de cada venda é arredondada para duas casas decimais antes da soma por vendedor;
- estoque não pode ficar negativo;
- cada movimentação recebe um `Guid`, descrição obrigatória e data/hora em UTC;
- os juros são simples: `valor × 2,5% × dias em atraso`;
- vencimento na data do cálculo não gera juros;
- datas futuras não geram juros.

## Estrutura

```text
src/TargetChallenge
├── Data             JSONs fornecidos no desafio
├── Infrastructure   leitura e desserialização dos arquivos
├── Models           entidades e resultados
├── Services         regras de negócio
└── Program.cs       menu e interação pelo console

tests/TargetChallenge.Tests
└── Program.cs       testes automatizados sem dependências externas
```

## Pré-requisitos

- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)

## Executar

Na pasta raiz do projeto:

```bash
dotnet run --project src/TargetChallenge
```

A aplicação exibirá um menu para calcular as comissões, registrar movimentações e calcular juros.

## Executar os testes

```bash
dotnet run --project tests/TargetChallenge.Tests
```

O projeto de testes usa um executor pequeno, sem pacotes NuGet, para manter a entrega autocontida. Em um projeto de produção, ele poderia ser substituído por xUnit, NUnit ou MSTest.

## Resultado esperado das comissões

Com os dados fornecidos:

| Vendedor | Total vendido | Comissão |
|---|---:|---:|
| Ana Lima | R$ 8.763,95 | R$ 404,99 |
| Carlos Oliveira | R$ 7.928,35 | R$ 379,38 |
| João Silva | R$ 10.754,70 | R$ 495,69 |
| Maria Souza | R$ 9.874,30 | R$ 465,96 |

## Possíveis evoluções

- persistir movimentações e saldos em SQL Server;
- expor as operações por uma API REST;
- adicionar controle de concorrência para movimentações simultâneas;
- registrar logs estruturados;
- substituir o executor de testes por xUnit e integrar a uma pipeline de CI.
