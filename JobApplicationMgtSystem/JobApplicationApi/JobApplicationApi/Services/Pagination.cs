namespace JobApplicationApi.Services;

public static class Pagination
{
    public static void Validate(int pageNumber, int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100 || (long)(pageNumber - 1) * pageSize > int.MaxValue)
            throw new BadHttpRequestException("Page number must be positive, page size must be between 1 and 100, and the offset must fit within the supported range.");
    }
}
