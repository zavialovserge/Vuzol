using System.Configuration;

namespace Vuzol.Model.Db
{
    public class SqlConnectionDb
    {
        public static string ConnectionString =>
           ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    }
}
