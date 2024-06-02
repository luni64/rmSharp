using Geolocation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite.Query.Internal;
using Newtonsoft.Json;
using rmSharp;

using Task = rmSharp.Task;

namespace rmtester
{
    internal class Program
    {
        static void Main(string[] args)
        {   
            DB.sqLiteFile = "empty.rmtree";

            using (var db = new DB())
            {
               
            }
        }
    }
}





