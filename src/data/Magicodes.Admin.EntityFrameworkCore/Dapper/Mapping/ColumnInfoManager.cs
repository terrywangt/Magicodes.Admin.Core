using DapperExtensions.Mapper;
using Magicodes.Admin.Contents;

namespace Magicodes.Admin.Dapper.Mapping
{
    public class ColumnInfoManager : ClassMapper<ColumnInfo>
    {
        public ColumnInfoManager()
        {
            Table("ColumnInfos");
            AutoMap();
        }
    }
}
