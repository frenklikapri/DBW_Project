using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Common.Extensions
{
    public static class UrlGenerationExtensions
    {
        public static string GenerateDocumentUrl(this string hostName)
        {
            var guid = Guid.NewGuid();
            var url = $"{hostName}{guid}";
            return url;
        }
    }
}
