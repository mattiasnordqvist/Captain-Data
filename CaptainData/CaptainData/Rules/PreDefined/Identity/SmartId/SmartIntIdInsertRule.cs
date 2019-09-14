namespace CaptainData.Rules.PreDefined.Identity
{
    public class SmartIntIdInsertRule : SmartIdInsertRule<int>
    {
        protected override int GetNextId(int lastId)
        {
            return lastId + 1;
        }
    }
}