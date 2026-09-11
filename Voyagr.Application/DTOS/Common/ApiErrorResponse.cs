using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyagr.Application.DTOS.Common
{
    public class ApiErrorResponse
    {
        public string Message { get; set; } = string.Empty;

        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
