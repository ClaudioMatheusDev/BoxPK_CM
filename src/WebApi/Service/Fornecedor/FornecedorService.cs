using Application.Dtos;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace WebApi.Service
{
    public class FornecedorService : IFornecedorService
    {
        private readonly AppDbContext _context;

        public FornecedorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CriarFornecedor(FornecedorCriarDto fornecedorCriarDto)
        {
            var fornecedor = new Fornecedor
            {
                Nome = fornecedorCriarDto.Nome,
                CPFCNPJ = fornecedorCriarDto.CPFCNPJ,
                Email = fornecedorCriarDto.Email,
                StatusFornecedor = StatusFornecedor.Ativo,
                Telefone = fornecedorCriarDto.Telefone
            };

            _context.Fornecedores.Add(fornecedor);
            await _context.SaveChangesAsync();

            return fornecedor.IDFornecedor;
        }

        public async Task<List<Fornecedor>> ListarFornecedores()
        {
            var fornecedor = await _context.Fornecedores.ToListAsync();

            if (fornecedor is null)
            {
                throw new Exception("Não há fornecedor cadastrados.");
            }

            return fornecedor;
        }

        public async Task<Fornecedor> ListarFornecedorPorID(int IDFornecedor)
        {
            var fornecedor = await _context.Fornecedores.FirstOrDefaultAsync(c => c.IDFornecedor == IDFornecedor);


            if (fornecedor is null)
            {
                throw new Exception("Não há fornecedores cadastrados.");
            }

            return fornecedor;

        }

        public async Task<int> AtualizarFornecedor(int IDFornecedor, FornecedorAtualizarDto fornecedorAtualizarDto)
        {
            var fornecedor = await _context.Fornecedores.FirstOrDefaultAsync(c => c.IDFornecedor == IDFornecedor);

            if (fornecedor is null)
            {
                throw new Exception("Nenhuma fornecedor encontra nessa ID");
            }

            fornecedor.Nome = fornecedorAtualizarDto.Nome;
            fornecedor.CPFCNPJ = fornecedorAtualizarDto.CPFCNPJ;
            fornecedor.Email = fornecedorAtualizarDto.Email;
            fornecedor.Telefone = fornecedorAtualizarDto.Telefone;
            fornecedor.StatusFornecedor = fornecedorAtualizarDto.StatusFornecedor;
            fornecedor.DataAtualizacao = DateTime.Now;

            await _context.SaveChangesAsync();

            return fornecedor.IDFornecedor;

        }

        public async Task<bool> DeletarFornecedor(int IDFornecedor)
        {
            var fornecedor = await _context.Fornecedores.FirstOrDefaultAsync(c => c.IDFornecedor == IDFornecedor);


            if (fornecedor is null)
            {
                throw new Exception("Nenhuma fornecedor encontra nessa ID");
            }

            _context.Remove(fornecedor);
            await _context.SaveChangesAsync();

            return true;

        }





    }
}
