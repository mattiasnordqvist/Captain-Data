namespace CaptainData
{
    public interface ISqlGenerator
    {
        string CreateInsertStatement(RowInstruction rowInstruction);
        string CreateGetScopeIdentityQuery(RowInstruction rowInstruction);
    }
}