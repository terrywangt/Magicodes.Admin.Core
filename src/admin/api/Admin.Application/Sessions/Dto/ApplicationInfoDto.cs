using System;
using System.Collections.Generic;

namespace Magicodes.Admin.Sessions.Dto
{
    public class ApplicationInfoDto
    {
        public string Version { get; set; }

        public DateTime ReleaseDate { get; set; }

        public Dictionary<string, bool> Features { get; set; }

        public string Name { get; set; }
    }
}