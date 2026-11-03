using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Middleware
{
    /// <summary>
    /// Converte exceções de negócio em respostas HTTP corretas (404/400/409) no formato ProblemDetails.
    /// Qualquer outra exceção vira 500 sem vazar detalhes internos.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (!context.Response.HasStarted)
            {
                var (status, title) = ex switch
                {
                    NotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                    BusinessException => (StatusCodes.Status400BadRequest, ex.Message),
                    ConflictException => (StatusCodes.Status409Conflict, ex.Message),
                    DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "O registro foi alterado por outra operação. Tente novamente."),
                    DbUpdateException => (StatusCodes.Status409Conflict, "A operação viola uma restrição de dados (registro em uso, inexistente ou duplicado)."),
                    _ => (StatusCodes.Status500InternalServerError, "Erro interno ao processar a requisição.")
                };

                if (status == StatusCodes.Status500InternalServerError)
                    _logger.LogError(ex, "Erro não tratado em {Method} {Path}", context.Request.Method, context.Request.Path);

                context.Response.Clear();
                context.Response.StatusCode = status;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    Detail = title,
                    Instance = context.Request.Path
                });
            }
        }
    }
}
