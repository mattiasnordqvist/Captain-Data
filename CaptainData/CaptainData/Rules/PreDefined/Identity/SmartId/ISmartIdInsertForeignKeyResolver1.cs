﻿using System;
using CaptainData.Schema;

namespace CaptainData.Rules.PreDefined.Identity
{
    public interface ISmartIdInsertForeignKeyResolver1
    {
        Func<ColumnSchema, string> Get { get; }
        Func<ColumnSchema, string[], bool> Is { get; }
    }
}