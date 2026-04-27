using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Application.ViewModels.AiSupport;

namespace QLKH.Application.Interfaces.Services
{
    public interface IAiSupportService
    {
        Task<AiChatResponse> AskAsync(AiChatRequest request);
    }
}