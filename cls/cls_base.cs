    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace SuperSarar.cls
{
    public class cls_base
    {

        public int getFaturaNo(int who)
        {
            int sy = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT  * FROM  AYARLAR WHERE AYR_ID='" + who + "'");
            SqlDataReader reader = new SqlDatabase().ExecuteReader(sql.ToString());
            if (reader.Read())
            {
                sy = GetInt(reader["AYR_DGR"]);
            }

            InsertUpdateUtil util = new InsertUpdateUtil("AYARLAR", "AYR_ID");
            sy = sy + 1;

            util.Columns.Add("AYR_DGR", sy);
            util.PrimaryValue = who;
            util.Update();



            return sy;
        }


        public decimal AlcFirmBky(int firmid)
        {
            decimal sy = 0;
            decimal ss = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT SUM(ODM_TUT) AS TUT FROM ODEMELER WHERE ODM_TIP=0 AND FIRM_ID="+firmid);
            SqlDataReader reader = new SqlDatabase().ExecuteReader(sql.ToString());
            if (reader.Read())
            {
                sy = GetDecimal(reader["TUT"]);
            }
            sql = new StringBuilder();
            sql.Append("SELECT SUM(TPL_TUT) AS TUT FROM FATURALAR WHERE FTR_TUR=1 AND FIRM_ID=" + firmid);
             reader = new SqlDatabase().ExecuteReader(sql.ToString());
            if (reader.Read())
            {
                sy = sy + GetDecimal(reader["TUT"]);
            }
            sql = new StringBuilder();
            sql.Append("SELECT SUM(ODM_TUT) AS TUT FROM ODEMELER WHERE ODM_TIP=1 AND FIRM_ID=" + firmid);
             reader = new SqlDatabase().ExecuteReader(sql.ToString());
            if (reader.Read())
            {
                ss = GetDecimal(reader["TUT"]);
            }
            sy = sy - ss;
            return sy;
        }

        public decimal StcFirmBky(int firmid)
        {
            decimal sy = 0;
            decimal ss = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT SUM(ODM_TUT) AS TUT FROM ODEMELER WHERE ODM_TIP=1 AND FIRM_ID=" + firmid);
            SqlDataReader reader = new SqlDatabase().ExecuteReader(sql.ToString());
            if (reader.Read())
            {
                sy = GetDecimal(reader["TUT"]);
            }
            sql = new StringBuilder();
            sql.Append("SELECT SUM(TPL_TUT) AS TUT FROM FATURALAR WHERE FTR_TUR=2 AND FIRM_ID=" + firmid);
            reader = new SqlDatabase().ExecuteReader(sql.ToString());
            if (reader.Read())
            {
                sy = sy + GetDecimal(reader["TUT"]);
            }
            sql = new StringBuilder();
            sql.Append("SELECT SUM(ODM_TUT) AS TUT FROM ODEMELER WHERE ODM_TIP=0 AND FIRM_ID=" + firmid);
            reader = new SqlDatabase().ExecuteReader(sql.ToString());
            if (reader.Read())
            {
                ss = GetDecimal(reader["TUT"]);
            }

            sy = sy - ss;
            return sy;
        }
        public DateTime GetServerDateTime()
        {
            return (GetServerDate().Add(GetServerTime()));
        }

        public DateTime GetServerDate()
        {

            string sql = "SELECT GETDATE()";
            DataSet ds = SqlUtil.SqlDatabaseDataset(sql);
            DateTime result = Convert.ToDateTime(ds.Tables[0].Rows[0][0]);
            return DateTime.Parse(result.ToShortDateString());

        }


        public TimeSpan GetServerTime()
        {

            string sql = "SELECT GETDATE()";
            DataSet ds = SqlUtil.SqlDatabaseDataset(sql);
            DateTime result = Convert.ToDateTime(ds.Tables[0].Rows[0][0]);
            return TimeSpan.Parse(result.ToShortTimeString());

        }

        public int GetInt(object o)
        {
            return GetInt(o, 0);
        }

        public int GetInt(object o, int defaultValue)
        {
            if (o == System.DBNull.Value)
                return defaultValue;

            if (o.ToString() == "")
                return defaultValue;

            try
            {
                return (int)o;
            }
            catch
            {
                try
                {
                    return int.Parse(o.ToString());
                }
                catch
                {
                    return defaultValue;
                }
            }

        }

        public bool GetBool(object o)
        {
            return GetBool(o, false);
        }

        public bool GetBool(object o, bool defaultValue)
        {
            if (o == System.DBNull.Value)
                return defaultValue;

            if (o.ToString() == "")
                return defaultValue;

            try
            {
                return (bool)o;
            }
            catch
            {
                try
                {
                    return bool.Parse(o.ToString());
                }
                catch
                {
                    return defaultValue;
                }
            }

        }

        public Decimal GetDecimal(object o)
        {
            return GetDecimal(o, 0);
        }

        public Decimal GetDecimal(object o, Decimal defaultValue)
        {
            if (o == System.DBNull.Value)
                return defaultValue;

            if (o.ToString() == "")
                return defaultValue;

            try
            {
                return (Decimal)o;
            }
            catch
            {
                try
                {
                    return Decimal.Parse(o.ToString());
                }
                catch
                {
                    return defaultValue;
                }
            }

        }

        public string GetString(object o)
        {
            return GetString(o, "");
        }

        public string GetString(object o, string defaultValue)
        {
            if (o == System.DBNull.Value)
                return defaultValue;

            return o.ToString();
        }
        public DateTime GetDate(object o)
        {
            return GetDate(o, this.DefaultDate);//new Act().GetServerDate()
        }

        protected DateTime GetDate(object o, DateTime defaultValue)
        {
            if (o == System.DBNull.Value)
                return defaultValue;

            if (o == null)
                return defaultValue;
            if (o.ToString() == "")
                return defaultValue;

            try
            {
                return DateTime.Parse(o.ToString());
            }
            catch
            {
                return defaultValue;
            }

        }

        protected Nullable<DateTime> GetDate(object o, Nullable<DateTime> defaultValue)
        {
            if (o == System.DBNull.Value)
                return defaultValue;

            if (o == null)
                return defaultValue;
            if (o.ToString() == "")
                return defaultValue;

            try
            {
                return DateTime.Parse(o.ToString());
            }
            catch
            {
                return defaultValue;
            }
        }


        public DateTime DefaultDate
        {
            get
            {
                return DateTime.Parse("01/01/1900");
            }
        }

    }
}
