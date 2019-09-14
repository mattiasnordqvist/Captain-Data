using System;

namespace CaptainData.CustomRules.PreDefined
{
    public class SmartGuidIdInsertRule : SmartIdInsertRule<Guid>
    {
        protected override Guid GetNextId(Guid lastId)
        {
            return Guid.NewGuid();
        }
    }
}