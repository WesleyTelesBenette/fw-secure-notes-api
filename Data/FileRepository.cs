namespace fw_secure_notes_api.Data;

public class FileRepository
{
    private readonly DatabaseContext _dbContext;

    public FileRepository(DatabaseContext dbContext)
        { _dbContext = dbContext; }


}
