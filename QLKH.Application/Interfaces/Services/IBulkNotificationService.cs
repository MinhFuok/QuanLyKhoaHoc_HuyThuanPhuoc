using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Application.DTOs;

namespace QLKH.Application.Interfaces.Services
{
    public interface IBulkNotificationService
    {
        Task<(int sentCount, int failedCount)> SendAsync(
            string subject,
            string message,
            bool sendToStudents,
            bool sendToTeachers,
            List<EmailAttachmentDto>? attachments = null);
    }
}