namespace Application.Exceptions
{
    /// <summary>Recurso não encontrado (HTTP 404).</summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    /// <summary>Violação de regra de negócio ou dado inválido (HTTP 400).</summary>
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }

    /// <summary>Conflito de estado ou concorrência (HTTP 409).</summary>
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
