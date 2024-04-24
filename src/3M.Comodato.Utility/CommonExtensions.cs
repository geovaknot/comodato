using FastMember;
using System;
using System.Collections.Generic;
using System.Data;

namespace _3M.Comodato.Utility
{
    public static class CommonExtensions
    {
        public static DateTime? Data(this object col)
        {
            if (col == DBNull.Value)
                return null;
            else
                return (DateTime)col;
        }
        public static string DataString(this object col)
        {
            if (col == DBNull.Value)
                return string.Empty;
            else
                return ((DateTime)col).ToString("dd/MM/yyyy");
        }
        public static T FieldOrDefault<T>(this DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? default(T) : row.Field<T>(columnName);
        }
        public static int ToInt(this Enum enumValue)
        {
            return Convert.ToInt32(enumValue);
        }

        public static DataTable ToDataTable<T>(this List<T> data) where T: class
        {
            DataTable table = new DataTable();
            using (var reader = ObjectReader.Create(data))
                table.Load(reader);

            return table;
        }
    }
}
