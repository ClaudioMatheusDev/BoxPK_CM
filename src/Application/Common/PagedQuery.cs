namespace Application.Common
{
    public static class PagedQuery
    {
        public const int DefaultPageSize = 500;
        public const int MaxPageSize = 1000;

        public static (int Skip, int Take) Normalize(int? page, int? pageSize)
        {
            var size = Math.Clamp(pageSize ?? DefaultPageSize, 1, MaxPageSize);
            var skip = (Math.Max(page ?? 1, 1) - 1) * size;
            return (skip, size);
        }
    }
}
