using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLKH.Application.Interfaces.Services;
using QLKH.Application.ViewModels;
using QLKH.Domain.Entities;

namespace QLKH.Web.Areas.HocVu.Controllers
{
    [Area("HocVu")]
    [Authorize(Roles = "Admin,HocVu")]
    public class ClassScheduleController : Controller
    {
        private readonly IClassScheduleService _classScheduleService;
        private readonly IClassRoomService _classRoomService;

        private static readonly TimeSpan MorningStart = new(7, 0, 0);
        private static readonly TimeSpan MorningEnd = new(11, 30, 0);
        private static readonly TimeSpan AfternoonStart = new(13, 0, 0);
        private static readonly TimeSpan AfternoonEnd = new(17, 0, 0);

        public ClassScheduleController(
            IClassScheduleService classScheduleService,
            IClassRoomService classRoomService)
        {
            _classScheduleService = classScheduleService;
            _classRoomService = classRoomService;
        }

        public async Task<IActionResult> Index(int? classRoomId)
        {
            await LoadClassRoomDropdownAsync(classRoomId);

            IEnumerable<ClassSchedule> schedules = classRoomId.HasValue
                ? await _classScheduleService.GetByClassRoomIdAsync(classRoomId.Value)
                : await _classScheduleService.GetAllAsync();

            ViewBag.SelectedClassRoomId = classRoomId;
            return View(schedules);
        }

        public async Task<IActionResult> Create()
        {
            await LoadClassRoomDropdownAsync();

            var model = new ClassScheduleRecurringCreateViewModel
            {
                Session = "Morning"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassScheduleRecurringCreateViewModel model)
        {
            if (model.ClassRoomId <= 0)
            {
                ModelState.AddModelError(nameof(model.ClassRoomId), "Vui lòng chọn lớp học.");
            }

            if (model.SelectedDays == null || !model.SelectedDays.Any())
            {
                ModelState.AddModelError(nameof(model.SelectedDays), "Vui lòng chọn ít nhất 1 ngày trong tuần.");
            }

            if (model.Session != "Morning" && model.Session != "Afternoon")
            {
                ModelState.AddModelError(nameof(model.Session), "Buổi học không hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                await LoadClassRoomDropdownAsync(model.ClassRoomId);
                return View(model);
            }

            var result = await _classScheduleService.CreateRecurringAsync(
                model.ClassRoomId,
                model.SelectedDays ?? new List<int>(),
                model.Session,
                model.TeamCode,
                model.Note);

            if (!result.Success)
            {
                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                }

                if (result.ConflictDates.Any())
                {
                    var text = string.Join(", ", result.ConflictDates.Select(x => x.ToString("dd/MM/yyyy")));
                    ModelState.AddModelError(string.Empty, $"Các ngày bị trùng lịch: {text}");
                }

                await LoadClassRoomDropdownAsync(model.ClassRoomId);
                return View(model);
            }

            TempData["SuccessMessage"] = $"Tạo thành công {result.CreatedCount} lịch học.";
            return RedirectToAction(nameof(Index), new { classRoomId = model.ClassRoomId });
        }

        public async Task<IActionResult> EditSelect(int id)
        {
            var model = await BuildPickSessionViewModelAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSelect(ClassSchedulePickSessionViewModel model)
        {
            if (model.SelectedScheduleId <= 0)
            {
                ModelState.AddModelError(nameof(model.SelectedScheduleId), "Vui lòng chọn buổi cần sửa.");
            }

            if (!ModelState.IsValid)
            {
                var rebuilt = await BuildPickSessionViewModelAsync(model.AnchorId, model.SelectedScheduleId);
                if (rebuilt == null)
                {
                    return NotFound();
                }

                return View(rebuilt);
            }

            return RedirectToAction(nameof(Edit), new { id = model.SelectedScheduleId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _classScheduleService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            await LoadClassRoomDropdownAsync(entity.ClassRoomId);

            var model = new ClassScheduleFormViewModel
            {
                Id = entity.Id,
                ClassRoomId = entity.ClassRoomId,
                StudyDate = entity.StudyDate.Date,
                Session = DetectSession(entity.StartTime, entity.EndTime),
                TeamCode = entity.RoomName,
                Note = entity.Note
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClassScheduleFormViewModel model)
        {
            if (model.ClassRoomId <= 0)
            {
                ModelState.AddModelError(nameof(model.ClassRoomId), "Vui lòng chọn lớp học.");
            }

            if (model.Session != "Morning" && model.Session != "Afternoon")
            {
                ModelState.AddModelError(nameof(model.Session), "Buổi học không hợp lệ.");
            }

            var classRoom = model.ClassRoomId > 0
                ? await _classRoomService.GetByIdAsync(model.ClassRoomId)
                : null;

            if (classRoom == null)
            {
                ModelState.AddModelError(nameof(model.ClassRoomId), "Lớp học không tồn tại.");
            }
            else
            {
                if (model.StudyDate.Date < classRoom.StartDate.Date || model.StudyDate.Date > classRoom.EndDate.Date)
                {
                    ModelState.AddModelError(
                        nameof(model.StudyDate),
                        $"Ngày học phải nằm trong khoảng từ {classRoom.StartDate:dd/MM/yyyy} đến {classRoom.EndDate:dd/MM/yyyy}.");
                }

                var (startTimeToCheck, endTimeToCheck, _) = GetSessionTime(model.Session);
                var conflictExists = await HasConflictForEditAsync(
                    model.Id,
                    model.StudyDate.Date,
                    startTimeToCheck,
                    endTimeToCheck);

                if (conflictExists)
                {
                    ModelState.AddModelError(
                        nameof(model.StudyDate),
                        "Đã có lịch học khác trùng ngày và buổi này. Vui lòng chọn ngày hoặc buổi khác.");
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadClassRoomDropdownAsync(model.ClassRoomId);
                return View(model);
            }

            var existing = await _classScheduleService.GetByIdAsync(model.Id);
            if (existing == null)
            {
                return NotFound();
            }

            var (startTime, endTime, sessionText) = GetSessionTime(model.Session);

            existing.ClassRoomId = model.ClassRoomId;
            existing.StudyDate = model.StudyDate.Date;
            existing.StartTime = startTime;
            existing.EndTime = endTime;
            existing.LessonTitle = $"{classRoom!.ClassCode} - {model.StudyDate:dd/MM/yyyy} - Buổi {sessionText}";
            existing.RoomName = model.TeamCode;
            existing.Note = model.Note;

            await _classScheduleService.UpdateAsync(existing);

            TempData["SuccessMessage"] = "Cập nhật lịch học thành công.";
            return RedirectToAction(nameof(Index), new { classRoomId = model.ClassRoomId });
        }

        public async Task<IActionResult> Details(int id)
        {
            var classSchedule = await _classScheduleService.GetByIdAsync(id);
            if (classSchedule == null)
            {
                return NotFound();
            }

            return View(classSchedule);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var model = await BuildPickSessionViewModelAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ClassSchedulePickSessionViewModel model)
        {
            if (!model.DeleteAllSessions && model.SelectedScheduleId <= 0)
            {
                ModelState.AddModelError(nameof(model.SelectedScheduleId), "Vui lòng chọn buổi cần xóa.");
            }

            if (!ModelState.IsValid)
            {
                var rebuilt = await BuildPickSessionViewModelAsync(model.AnchorId, model.SelectedScheduleId);
                if (rebuilt == null)
                {
                    return NotFound();
                }

                rebuilt.DeleteAllSessions = model.DeleteAllSessions;
                return View("Delete", rebuilt);
            }

            if (model.DeleteAllSessions)
            {
                var deletedCount = await _classScheduleService.DeleteGroupedSessionsAsync(model.AnchorId);
                if (deletedCount <= 0)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy nhóm lịch học để xóa.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = $"Đã xóa toàn bộ {deletedCount} buổi trong nhóm lịch.";
                return RedirectToAction(nameof(Index));
            }

            var deleted = await _classScheduleService.DeleteAsync(model.SelectedScheduleId);

            if (!deleted)
            {
                TempData["ErrorMessage"] = "Không tìm thấy buổi học để xóa.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Xóa buổi học thành công.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadClassRoomDropdownAsync(int? selectedClassRoomId = null)
        {
            var classRooms = (await _classRoomService.GetAllAsync()).ToList();

            ViewBag.ClassRoomId = new SelectList(classRooms, "Id", "ClassName", selectedClassRoomId);

            var classRoomMeta = classRooms.Select(x => new
            {
                id = x.Id,
                classCode = x.ClassCode,
                className = x.ClassName,
                startDate = x.StartDate.ToString("yyyy-MM-dd"),
                endDate = x.EndDate.ToString("yyyy-MM-dd")
            });

            ViewBag.ClassRoomMetaJson = JsonSerializer.Serialize(classRoomMeta);
        }

        private async Task<ClassSchedulePickSessionViewModel?> BuildPickSessionViewModelAsync(int anchorId, int? selectedScheduleId = null)
        {
            var anchor = await _classScheduleService.GetByIdAsync(anchorId);
            if (anchor == null)
            {
                return null;
            }

            var schedules = await _classScheduleService.GetByClassRoomIdAsync(anchor.ClassRoomId);

            var groupSchedules = schedules
                .Where(x =>
                    x.ClassRoomId == anchor.ClassRoomId &&
                    x.StartTime == anchor.StartTime &&
                    x.EndTime == anchor.EndTime &&
                    string.Equals(x.RoomName ?? "", anchor.RoomName ?? "", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(x.Note ?? "", anchor.Note ?? "", StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.StudyDate)
                .ToList();

            var sessionOptions = groupSchedules
                .Select((x, index) => new ClassScheduleSessionOptionViewModel
                {
                    Id = x.Id,
                    Text = $"Buổi {index + 1} - {x.StudyDate:dd/MM/yyyy} - {GetWeekDayText(x.StudyDate.DayOfWeek)} - {GetSessionText(x.StartTime, x.EndTime)}"
                })
                .ToList();

            return new ClassSchedulePickSessionViewModel
            {
                AnchorId = anchorId,
                SelectedScheduleId = selectedScheduleId ?? anchorId,
                ClassCode = anchor.ClassRoom?.ClassCode ?? "",
                ClassName = anchor.ClassRoom?.ClassName ?? "",
                CourseName = anchor.ClassRoom?.Course?.CourseName,
                SessionText = GetSessionText(anchor.StartTime, anchor.EndTime),
                TimeRangeText = $"{anchor.StartTime:hh\\:mm} - {anchor.EndTime:hh\\:mm}",
                TeamCode = anchor.RoomName,
                Note = anchor.Note,
                TotalSessions = groupSchedules.Count,
                SessionOptions = sessionOptions
            };
        }

        private async Task<bool> HasConflictForEditAsync(
            int editingScheduleId,
            DateTime studyDate,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            var allSchedules = await _classScheduleService.GetAllAsync();

            return allSchedules.Any(x =>
                x.Id != editingScheduleId &&
                x.StudyDate.Date == studyDate.Date &&
                x.StartTime < endTime &&
                startTime < x.EndTime);
        }

        private static (TimeSpan StartTime, TimeSpan EndTime, string SessionText) GetSessionTime(string session)
        {
            return session switch
            {
                "Morning" => (MorningStart, MorningEnd, "Sáng"),
                "Afternoon" => (AfternoonStart, AfternoonEnd, "Chiều"),
                _ => throw new InvalidOperationException("Buổi học không hợp lệ.")
            };
        }

        private static string DetectSession(TimeSpan startTime, TimeSpan endTime)
        {
            if (startTime == MorningStart && endTime == MorningEnd)
            {
                return "Morning";
            }

            if (startTime == AfternoonStart && endTime == AfternoonEnd)
            {
                return "Afternoon";
            }

            return "Morning";
        }

        private static string GetSessionText(TimeSpan startTime, TimeSpan endTime)
        {
            if (startTime == MorningStart && endTime == MorningEnd)
            {
                return "Sáng";
            }

            if (startTime == AfternoonStart && endTime == AfternoonEnd)
            {
                return "Chiều";
            }

            return $"{startTime:hh\\:mm} - {endTime:hh\\:mm}";
        }

        private static string GetWeekDayText(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => "Thứ hai",
                DayOfWeek.Tuesday => "Thứ ba",
                DayOfWeek.Wednesday => "Thứ tư",
                DayOfWeek.Thursday => "Thứ năm",
                DayOfWeek.Friday => "Thứ sáu",
                DayOfWeek.Saturday => "Thứ bảy",
                _ => "Chủ nhật"
            };
        }
    }
}