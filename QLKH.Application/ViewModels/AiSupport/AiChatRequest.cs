using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace QLKH.Application.ViewModels.AiSupport
{
    public class AiChatRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập nội dung cần hỏi.")]
        [StringLength(1000, ErrorMessage = "Câu hỏi không được vượt quá 1000 ký tự.")]
        public string Message { get; set; } = string.Empty;
    }
}