namespace CaptainData.CustomRules.PreDefined
{
    public class SmartIntIdInsertRule : SmartIdInsertRule<int>
    {
        protected override int GetNextId(int lastId)
        {
            return lastId + 1;
        }
    }
}