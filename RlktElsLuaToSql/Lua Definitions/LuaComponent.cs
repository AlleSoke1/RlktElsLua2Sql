using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace RlktElsLuaToSql
{
    public class LuaComponent
    {
        public string ComponentName { get; set; } = "Unnamed Component.";
        public string OutputTableName { get; set; } = "Unnamed Table.";

        public bool bHasData { get; set; } = false;

        public virtual string GetDataCount() { return ""; }

        public virtual void ExportToSql() { }
    }
}
