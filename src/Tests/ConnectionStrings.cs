namespace Tests
{
    class ConnectionStrings : CoreTechs.Common.ConnectionStrings
    {
        public static string Northwind
        {
            get { return GetConnectionString(); }
        }
    }
}