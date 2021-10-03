using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RlktElsLuaToSql
{
    class RlktSqlUtils
    {
        public static string TypeToSql(Type inType)
        {
            if (inType == typeof(Int64) || inType == typeof(UInt64))
                return "BIGINT";
            else if (inType == typeof(Int32) || inType == typeof(UInt32))
                return "INT";
            else if (inType == typeof(Int16) || inType == typeof(UInt16))
                return "SMALLINT";
            else if (inType == typeof(byte))
                return "TINYINT";
            else if (inType == typeof(bool))
                return "BIT";
            else if (inType == typeof(float))
                return "FLOAT";
            else if (inType == typeof(double))
                return "DOUBLE";
            else if (inType == typeof(string))
                return "NVARCHAR(4000)";

            return "";
        }
        public static string EnumToSql(string tableName, Type inType)
        {
            MemberInfo[] memberInfo = inType.GetMembers();

            StringBuilder query = new StringBuilder();
            query.Append("CREATE TABLE ");
            query.Append(tableName);
            query.Append(" ( ");

            foreach(MemberInfo info in memberInfo)
            {
                if (info.MemberType != MemberTypes.Property)
                    continue;

                query.Append(info.Name);
                query.Append(" ");
                query.Append( RlktSqlUtils.TypeToSql( ((PropertyInfo)info).PropertyType) );
                query.Append(" ");
                //if (field.isIdentitySpecification)
                //{
                //    query.Append("IDENTITY(1,1) ");
                //}
                //if (field.isPrimaryKey)
                //{
                //    query.Append("PRIMARY KEY ");
                //}
                query.Append(", ");
                query.Append(Environment.NewLine);
            }

            query.Append(")");


            return query.ToString();
        }
    }
}
