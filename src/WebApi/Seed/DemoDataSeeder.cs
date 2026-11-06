using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using WebApi.Service;

namespace WebApi.Seed
{
    /// <summary>
    /// Catálogo de demonstração de uma loja de cartas Pokémon: jogo, coleções (Escarlate e Violeta),
    /// categorias, fornecedores, cartas avulsas e selados, compras com itens, estoque e movimentações.
    /// O estoque é derivado das compras confirmadas e das vendas, usando a mesma regra da API
    /// (<see cref="MovimentacaoEstoqueService.AplicarMovimentacao"/>), então os saldos ficam consistentes.
    /// </summary>
    public static class DemoDataSeeder
    {
        private const string ImagemBaseUrl = "https://images.pokemontcg.io";

        private sealed record ColecaoSeed(string Sigla, string Nome, DateTime Lancamento, string? ImagemSetId, StatusColecao Status = StatusColecao.Ativo);

        private sealed record CartaSeed(string Sigla, string Nome, string Numero, string Raridade, decimal Custo, decimal Venda);

        private sealed record SeladoSeed(string Chave, string Categoria, string? Sigla, string Nome, string Descricao, decimal Custo, decimal Venda);

        private static readonly ColecaoSeed[] Colecoes =
        [
            new("SVI", "Escarlate e Violeta", new DateTime(2023, 3, 31), "sv1"),
            new("PAL", "Evoluções em Paldea", new DateTime(2023, 6, 9), "sv2"),
            new("OBF", "Obsidiana em Chamas", new DateTime(2023, 8, 11), "sv3"),
            new("MEW", "Escarlate e Violeta 151", new DateTime(2023, 9, 22), "sv3pt5"),
            new("PAR", "Fenda Paradoxal", new DateTime(2023, 11, 3), "sv4"),
            new("PAF", "Destinos de Paldea", new DateTime(2024, 1, 26), "sv4pt5"),
            new("TEF", "Forças Temporais", new DateTime(2024, 3, 22), "sv5"),
            new("TWM", "Máscaras do Crepúsculo", new DateTime(2024, 5, 24), "sv6"),
            new("SFA", "Fábula Sombria", new DateTime(2024, 8, 2), "sv6pt5"),
            new("SCR", "Coroa Estelar", new DateTime(2024, 9, 13), "sv7"),
            new("SSP", "Fagulhas Impetuosas", new DateTime(2024, 11, 8), "sv8"),
            new("PRE", "Evoluções Prismáticas", new DateTime(2025, 1, 17), "sv8pt5"),
            new("DRI", "Rivais Predestinados", new DateTime(2025, 5, 30), "sv10"),
            new("LOR", "Origem Perdida", new DateTime(2022, 9, 9), "swsh11", StatusColecao.Inativo),
        ];

        private static readonly CartaSeed[] Cartas =
        [
            new("MEW", "Charizard ex", "199/165", "Ilustração Rara Especial", 1400m, 1890m),
            new("MEW", "Mew ex", "193/165", "Ilustração Rara Especial", 450m, 620m),
            new("MEW", "Venusaur ex", "198/165", "Ilustração Rara Especial", 380m, 520m),
            new("MEW", "Blastoise ex", "200/165", "Ilustração Rara Especial", 420m, 580m),
            new("MEW", "Alakazam ex", "201/165", "Ilustração Rara Especial", 150m, 220m),
            new("MEW", "Convite de Erika", "203/165", "Ilustração Rara Especial", 380m, 520m),
            new("MEW", "Pikachu", "173/165", "Ilustração Rara", 90m, 140m),
            new("MEW", "Bulbasaur", "166/165", "Ilustração Rara", 120m, 180m),
            new("MEW", "Charmander", "168/165", "Ilustração Rara", 140m, 210m),
            new("MEW", "Squirtle", "170/165", "Ilustração Rara", 130m, 190m),
            new("OBF", "Charizard ex", "223/197", "Ilustração Rara Especial", 350m, 490m),
            new("OBF", "Charizard ex", "125/197", "Rara Dupla", 25m, 45m),
            new("PAF", "Charizard ex", "234/091", "Ilustração Rara Especial", 900m, 1250m),
            new("PAF", "Mew ex", "232/091", "Ilustração Rara Especial", 220m, 320m),
            new("PRE", "Umbreon ex", "161/131", "Ilustração Rara Especial", 3500m, 4600m),
            new("PRE", "Sylveon ex", "156/131", "Ilustração Rara Especial", 700m, 950m),
            new("PRE", "Espeon ex", "155/131", "Ilustração Rara Especial", 900m, 1200m),
            new("SSP", "Pikachu ex", "238/191", "Ilustração Rara Especial", 1600m, 2200m),
            new("SSP", "Pikachu ex", "057/191", "Rara Dupla", 15m, 30m),
            new("SSP", "Latias ex", "239/191", "Ilustração Rara Especial", 250m, 360m),
            new("TWM", "Greninja ex", "214/167", "Ilustração Rara Especial", 600m, 820m),
            new("TWM", "Ursaluna Lua Sangrenta ex", "216/167", "Ilustração Rara Especial", 90m, 140m),
            new("SVI", "Gardevoir ex", "245/198", "Ilustração Rara Especial", 130m, 190m),
            new("SVI", "Miriam", "251/198", "Ilustração Rara Especial", 110m, 170m),
            new("PAL", "Iono", "269/193", "Ilustração Rara Especial", 450m, 620m),
            new("PAL", "Magikarp", "203/193", "Ilustração Rara", 380m, 520m),
            new("PAR", "Lua Estrondosa ex", "251/182", "Ilustração Rara Especial", 120m, 180m),
            new("TEF", "Onda Andante ex", "205/162", "Ilustração Rara Especial", 100m, 150m),
            new("SCR", "Terapagos ex", "170/142", "Ilustração Rara Especial", 150m, 220m),
        ];

        private static readonly SeladoSeed[] Selados =
        [
            new("booster-mew", "Booster", "MEW", "Booster Escarlate e Violeta 151", "Pacote avulso com 10 cartas da coleção 151.", 28m, 39.90m),
            new("etb-mew", "Elite Trainer Box", "MEW", "Elite Trainer Box Escarlate e Violeta 151", "9 boosters, 65 protetores, dados, marcadores e guia da coleção.", 380m, 549.90m),
            new("bbox-scr", "Booster Box", "SCR", "Booster Box Coroa Estelar", "Caixa lacrada com 36 boosters de Coroa Estelar.", 720m, 999.90m),
            new("etb-ssp", "Elite Trainer Box", "SSP", "Elite Trainer Box Fagulhas Impetuosas", "9 boosters, 65 protetores, dados e marcadores de condição.", 260m, 379.90m),
            new("bundle-pre", "Booster", "PRE", "Booster Bundle Evoluções Prismáticas", "Kit com 6 boosters de Evoluções Prismáticas.", 230m, 329.90m),
            new("booster-dri", "Booster", "DRI", "Booster Rivais Predestinados", "Pacote avulso com 10 cartas de Rivais Predestinados.", 24m, 34.90m),
            new("blister-twm", "Booster", "TWM", "Blister Triplo Máscaras do Crepúsculo", "3 boosters de Máscaras do Crepúsculo e carta promocional.", 60m, 89.90m),
            new("premium-charizard", "Coleção Especial", null, "Coleção Premium Charizard ex", "Carta promocional de Charizard ex, broche e 6 boosters sortidos.", 260m, 379.90m),
            new("deck-liga", "Deck", null, "Deck de Batalha de Liga Charizard ex", "Deck de 60 cartas pronto para jogar em torneios.", 140m, 219.90m),
            new("sleeves-pikachu", "Acessórios", null, "Protetores de Cartas Pikachu (65 un.)", "Protetores oficiais tamanho padrão.", 45m, 79.90m),
            new("pasta-9", "Acessórios", null, "Pasta Fichário 9 Bolsos Pokémon", "Pasta com 20 folhas de 9 bolsos para até 360 cartas.", 90m, 149.90m),
        ];

        /// <summary>
        /// Popula o tenant do contexto informado. Retorna false (sem alterar nada) se o tenant já possui dados.
        /// O contexto precisa ter um ITenantProvider com TenantId definido.
        /// </summary>
        public static async Task<bool> SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
        {
            if (await context.Jogos.AnyAsync(cancellationToken))
                return false;

            var agora = DateTime.Now;
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            // 1. Cadastros base
            var pokemon = new Jogo
            {
                Nome = "Pokémon Estampas Ilustradas",
                Fabricante = "The Pokémon Company",
                StatusJogo = StatusJogo.Ativo,
                DataAtualizacao = agora
            };

            var categorias = new[]
            {
                NovaCategoria("Carta Avulsa", "Cartas individuais (singles), em sua maioria raras e ilustradas.", agora),
                NovaCategoria("Booster", "Pacotes, blisters e bundles de boosters lacrados.", agora),
                NovaCategoria("Elite Trainer Box", "Caixas de Treinador Avançado com boosters e acessórios.", agora),
                NovaCategoria("Booster Box", "Caixas lacradas com 36 boosters.", agora),
                NovaCategoria("Coleção Especial", "Coleções premium, latas e boxes com cartas promocionais.", agora),
                NovaCategoria("Deck", "Decks prontos para jogar.", agora),
                NovaCategoria("Acessórios", "Protetores, pastas, deck boxes e playmats.", agora),
                NovaCategoria("Carta Graduada", "Cartas avaliadas por PSA/CGC (em breve).", agora, StatusCategoria.Desabilitado),
            }.ToDictionary(c => c.Nome);

            var colecoes = Colecoes.ToDictionary(c => c.Sigla, c => new Colecao
            {
                Nome = c.Nome,
                Sigla = c.Sigla,
                JogoTCG = pokemon,
                DataLancamento = c.Lancamento,
                StatusColecao = c.Status,
                DataAlteracao = agora
            });

            var fornecedores = new[]
            {
                NovoFornecedor("Distribuidora Kanto Cards Ltda.", "12.345.678/0001-95", "comercial@kantocards.example.com", "(11) 3456-7890", agora),
                NovoFornecedor("Johto Importadora de Colecionáveis Ltda.", "23.456.781/0001-29", "pedidos@johtoimport.example.com", "(21) 3987-6543", agora),
                NovoFornecedor("Hoenn TCG Atacado Ltda.", "34.567.892/0001-29", "vendas@hoenntcg.example.com", "(31) 3222-1100", agora),
                NovoFornecedor("Sinnoh Acessórios Geek ME", "45.678.903/0001-64", "contato@sinnohgeek.example.com", "(41) 3030-4040", agora, StatusFornecedor.Inativo),
                NovoFornecedor("Ash Ketchum (colecionador pessoa física)", "123.456.789-09", "ash.colecionador@example.com", "(11) 98765-4321", agora),
            };

            context.Jogos.Add(pokemon);
            context.Categorias.AddRange(categorias.Values);
            context.Colecoes.AddRange(colecoes.Values);
            context.Fornecedores.AddRange(fornecedores);
            await context.SaveChangesAsync(cancellationToken);

            // 2. Produtos: cartas avulsas (com imagem) e selados
            var cartas = Cartas.ToDictionary(Chave, c => new Produto
            {
                Nome = $"{c.Nome} {c.Numero} ({c.Sigla})",
                Descricao = $"{c.Raridade} da coleção {colecoes[c.Sigla].Nome}. Carta avulsa em estado Near Mint, idioma português.",
                PrecoCusto = c.Custo,
                PrecoVenda = c.Venda,
                Categoria = categorias["Carta Avulsa"],
                JogoTCG = pokemon,
                Colecao = colecoes[c.Sigla],
                ImagemUrl = ImagemDaCarta(c),
                StatusProduto = StatusProduto.Ativo,
                DataAlteracao = agora
            });

            var selados = Selados.ToDictionary(s => s.Chave, s => new Produto
            {
                Nome = s.Nome,
                Descricao = s.Descricao,
                PrecoCusto = s.Custo,
                PrecoVenda = s.Venda,
                Categoria = categorias[s.Categoria],
                JogoTCG = pokemon,
                Colecao = s.Sigla is null ? null : colecoes[s.Sigla],
                StatusProduto = StatusProduto.Ativo,
                DataAlteracao = agora
            });

            var produtos = cartas.Values.Concat(selados.Values).ToList();
            context.Produtos.AddRange(produtos);
            await context.SaveChangesAsync(cancellationToken);

            // 3. Estoque zerado para todos os produtos; os saldos vêm das movimentações abaixo
            var estoques = produtos.ToDictionary(p => p.IDProduto, p =>
            {
                var avulsa = p.Categoria.Nome == "Carta Avulsa";
                return new Estoque
                {
                    IDProduto = p.IDProduto,
                    QuantidadeAtual = 0,
                    QuantidadeMinima = avulsa ? 1 : 5,
                    QuantidadeMaxima = avulsa ? 10 : 120,
                    DataAtualizacao = agora
                };
            });
            context.Estoques.AddRange(estoques.Values);
            await context.SaveChangesAsync(cancellationToken);

            // 4. Compras
            Produto C(string sigla, string numero) => cartas[$"{sigla}-{numero}"];
            Produto S(string chave) => selados[chave];

            var compras = new List<Compra>
            {
                NovaCompra(fornecedores[0], new DateTime(2026, 8, 5), StatusCompra.Confirmada,
                    "Reposição de selados para o lançamento do mês.",
                    (S("booster-mew"), 72), (S("etb-mew"), 10), (S("bbox-scr"), 4), (S("etb-ssp"), 8)),

                NovaCompra(fornecedores[4], new DateTime(2026, 8, 20), StatusCompra.Confirmada,
                    "Compra de coleção particular (singles 151 e Paldea).",
                    (C("MEW", "199/165"), 2), (C("MEW", "193/165"), 2), (C("MEW", "198/165"), 1), (C("MEW", "200/165"), 1),
                    (C("MEW", "201/165"), 3), (C("MEW", "203/165"), 1), (C("MEW", "173/165"), 4), (C("MEW", "166/165"), 2),
                    (C("MEW", "168/165"), 2), (C("MEW", "170/165"), 2), (C("SVI", "245/198"), 3), (C("SVI", "251/198"), 2),
                    (C("PAL", "269/193"), 1), (C("PAL", "203/193"), 1), (C("PAR", "251/182"), 3), (C("TEF", "205/162"), 3)),

                NovaCompra(fornecedores[1], new DateTime(2026, 9, 15), StatusCompra.Confirmada,
                    "Importação: Evoluções Prismáticas, Fagulhas Impetuosas e acessórios.",
                    (C("PRE", "161/131"), 1), (C("PRE", "156/131"), 2), (C("PRE", "155/131"), 1),
                    (C("SSP", "238/191"), 1), (C("SSP", "057/191"), 12), (C("SSP", "239/191"), 2),
                    (C("OBF", "223/197"), 2), (C("OBF", "125/197"), 10), (C("PAF", "234/091"), 1), (C("PAF", "232/091"), 2),
                    (C("TWM", "214/167"), 1), (C("TWM", "216/167"), 3), (C("SCR", "170/142"), 2),
                    (S("bundle-pre"), 12), (S("premium-charizard"), 6), (S("deck-liga"), 6),
                    (S("sleeves-pikachu"), 30), (S("pasta-9"), 10)),

                NovaCompra(fornecedores[2], new DateTime(2026, 10, 2), StatusCompra.Cancelada,
                    "Cancelada: fornecedor sem prazo de entrega.",
                    (S("bbox-scr"), 6)),

                NovaCompra(fornecedores[0], new DateTime(2026, 10, 28), StatusCompra.Pendente,
                    "Pré-venda Rivais Predestinados e reposição de blisters.",
                    (S("booster-dri"), 144), (S("blister-twm"), 20), (S("etb-mew"), 6)),
            };
            context.Compras.AddRange(compras);
            await context.SaveChangesAsync(cancellationToken);

            // 5. Entradas no estoque pelas compras confirmadas (mesma regra da confirmação via API)
            foreach (var compra in compras.Where(c => c.StatusCompra == StatusCompra.Confirmada))
            {
                foreach (var item in compra.Itens)
                {
                    Movimentar(context, estoques[item.IDProduto], TipoMovimentacao.Entrada, item.Quantidade,
                        "Recebimento de compra", $"Compra #{compra.IDCompra}", compra.DataCompra);
                }
            }

            // 6. Saídas: vendas e ajustes posteriores
            var saidas = new (Produto Produto, int Quantidade, string Motivo, string? Observacao, DateTime Data)[]
            {
                (S("booster-mew"), 30, "Venda", "Vendas no balcão", new DateTime(2026, 8, 12)),
                (S("booster-mew"), 4, "Avaria", "Pacotes danificados no transporte", new DateTime(2026, 8, 13)),
                (S("etb-mew"), 7, "Venda", "Loja online", new DateTime(2026, 8, 18)),
                (S("bbox-scr"), 1, "Uso interno", "Aberta para evento de pré-lançamento", new DateTime(2026, 9, 20)),
                (S("etb-ssp"), 5, "Venda", "Loja online", new DateTime(2026, 9, 25)),
                (C("MEW", "199/165"), 1, "Venda", "Pedido #1042", new DateTime(2026, 9, 1)),
                (C("MEW", "203/165"), 1, "Venda", "Pedido #1050", new DateTime(2026, 9, 3)),
                (C("MEW", "173/165"), 2, "Venda", "Vendas no balcão", new DateTime(2026, 9, 6)),
                (C("PAL", "269/193"), 1, "Venda", "Pedido #1061", new DateTime(2026, 9, 9)),
                (C("PRE", "161/131"), 1, "Venda", "Pedido #1103", new DateTime(2026, 9, 22)),
                (C("SSP", "057/191"), 5, "Venda", "Vendas no balcão", new DateTime(2026, 10, 4)),
                (C("OBF", "125/197"), 4, "Venda", "Torneio de sábado", new DateTime(2026, 10, 10)),
                (S("bundle-pre"), 9, "Venda", "Loja online", new DateTime(2026, 10, 15)),
                (S("sleeves-pikachu"), 12, "Venda", "Vendas no balcão", new DateTime(2026, 10, 18)),
            };

            foreach (var (produto, quantidade, motivo, observacao, data) in saidas)
            {
                Movimentar(context, estoques[produto.IDProduto], TipoMovimentacao.Saida, quantidade, motivo, observacao, data);
            }

            // 7. Status do produto coerente com o saldo e com compras pendentes
            var aguardandoEntrega = compras
                .Where(c => c.StatusCompra == StatusCompra.Pendente)
                .SelectMany(c => c.Itens)
                .Select(i => i.IDProduto)
                .ToHashSet();

            foreach (var produto in produtos)
            {
                if (estoques[produto.IDProduto].QuantidadeAtual > 0)
                    produto.StatusProduto = StatusProduto.Ativo;
                else if (aguardandoEntrega.Contains(produto.IDProduto))
                    produto.StatusProduto = StatusProduto.AguardandoEntrega;
                else
                    produto.StatusProduto = StatusProduto.ForaEstoque;
            }

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }

        private static string Chave(CartaSeed carta) => $"{carta.Sigla}-{carta.Numero}";

        private static string? ImagemDaCarta(CartaSeed carta)
        {
            var setId = Colecoes.First(c => c.Sigla == carta.Sigla).ImagemSetId;
            if (setId is null) return null;

            var numero = int.Parse(carta.Numero.Split('/')[0]);
            return $"{ImagemBaseUrl}/{setId}/{numero}_hires.png";
        }

        private static Categoria NovaCategoria(string nome, string descricao, DateTime agora, StatusCategoria status = StatusCategoria.Ativo) => new()
        {
            Nome = nome,
            Descricao = descricao,
            StatusCategoria = status,
            DataAlteracao = agora
        };

        private static Fornecedor NovoFornecedor(string nome, string documento, string email, string telefone, DateTime agora,
            StatusFornecedor status = StatusFornecedor.Ativo) => new()
        {
            Nome = nome,
            CPFCNPJ = documento,
            Email = email,
            Telefone = telefone,
            StatusFornecedor = status,
            DataAtualizacao = agora
        };

        private static Compra NovaCompra(Fornecedor fornecedor, DateTime data, StatusCompra status, string observacao,
            params (Produto Produto, int Quantidade)[] itens)
        {
            var compra = new Compra
            {
                Fornecedor = fornecedor,
                DataCompra = data,
                StatusCompra = status,
                Observacao = observacao
            };

            foreach (var (produto, quantidade) in itens)
            {
                compra.Itens.Add(new ItemCompra
                {
                    Produto = produto,
                    Quantidade = quantidade,
                    ValorUnitario = produto.PrecoCusto,
                    ValorTotal = produto.PrecoCusto * quantidade
                });
            }

            compra.ValorTotal = compra.Itens.Sum(i => i.ValorTotal);
            return compra;
        }

        private static void Movimentar(AppDbContext context, Estoque estoque, TipoMovimentacao tipo, int quantidade,
            string motivo, string? observacao, DateTime data)
        {
            var movimentacao = MovimentacaoEstoqueService.AplicarMovimentacao(estoque, tipo, quantidade, motivo, observacao);
            movimentacao.DataMovimentacao = data;
            estoque.DataAtualizacao = data;
            context.MovimentacaoEstoques.Add(movimentacao);
        }
    }
}
