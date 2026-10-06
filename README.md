# Desafio Técnico - Target Sistemas

C# / .NET. Três desafios independentes, orquestrados pelo `Program.cs`.

## Como executar

    dotnet test
    dotnet run --project src/DesafioTarget            # menu
    dotnet run --project src/DesafioTarget -- 1       # 1 = comissões, 2 = estoque, 3 = juros

## Arquitetura

Cada desafio é um módulo com camadas Domain / Application / Infrastructure / Presentation.
`Shared` guarda o que é comum (exceções, parsing, leitura de JSON, formatação).

## Decisões

- Dinheiro em `decimal`; arredondamento `AwayFromZero` com 2 casas.
- Comissão: < 100 = 0; 100..499,99 = 1%; >= 500 = 5%. Arredonda sobre o total do vendedor.
- Juros: simples, 2,5% ao dia sobre o valor original, em dias corridos; sem atraso = sem juros.
- Estoque: ids sequenciais e únicos; saída maior que o saldo é rejeitada; dados em memória.
- Data/hora via `TimeProvider`, para testar o "hoje".

## Fora de escopo

Persistência das movimentações, prorrogação de vencimento em fim de semana/feriado.
