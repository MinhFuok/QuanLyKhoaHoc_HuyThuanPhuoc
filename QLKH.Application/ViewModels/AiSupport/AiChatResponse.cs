using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Application.ViewModels.AiSupport
{
    public class AiChatResponse
    {
        public bool Success { get; set; }

        public string Answer { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
    }
}