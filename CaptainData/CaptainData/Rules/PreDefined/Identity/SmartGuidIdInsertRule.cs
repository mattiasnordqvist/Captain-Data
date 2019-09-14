using System;

namespace CaptainData.Rules.PreDefined.Identity
{
    public class SmartGuidIdInsertRule : SmartIdInsertRule<Guid>
    {
        protected override Guid GetNextId(Guid lastId)
        {
            return Guid.NewGuid();
        }
    }
}