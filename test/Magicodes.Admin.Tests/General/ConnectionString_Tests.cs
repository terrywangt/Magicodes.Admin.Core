using System.Data.SqlClient;
using Shouldly;
using Xunit;

namespace Magicodes.Admin.Tests.General
{
    public class ConnectionString_Tests
    {
        [Fact]
        public void SqlConnectionStringBuilder_Test()
        {
            var csb = new SqlConnectionStringBuilder("Server=localhost; Database=Admin; Trusted_Connection=True;");
            csb["Database"].ShouldBe("Admin");
        }
    }
}
