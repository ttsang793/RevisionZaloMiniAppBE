using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class AchievementDb
{
    private ZaloRevisionAppDbContext _dbContext;

    private readonly List<Achievement> _achievements = new List<Achievement>
    {
        new(1, "Hoàn thành đề đầu tiên", "Hoàn thành đề đầu tiên", "/avatar/mystery.jpg"),
        new(2, "Hoàn thành đề đầu tiên có kết quả từ 8 trở lên", "Hoàn thành đề đầu tiên có kết quả từ 8 trở lên", "/avatar/mystery.jpg"),
        new(3, "Hoàn thành đề đầu tiên đạt điểm 10", "Hoàn thành đề đầu tiên đạt điểm 10", "/avatar/mystery.jpg"),
        new(4, "Sử dụng hết thời gian làm bài", "Sử dụng hết thời gian làm bài", "/avatar/mystery.jpg"),
        new(5, "Nộp bài ngay khi được cho phép", "Nộp bài ngay khi được cho phép", "/avatar/mystery.jpg"),
        new(6, "Nộp bài và làm đầy đủ tất cả các câu", "Nộp bài và làm đầy đủ tất cả các câu", "/avatar/mystery.jpg"),
        new(7, "Bình luận đầu tiên", "Bình luận đầu tiên", "/avatar/mystery.jpg"),
        new(8, "Yêu thích 1 đề thi", "Yêu thích 1 đề thi", "/avatar/mystery.jpg"),
        new(9, "Yêu thích 12 đề thi", "Yêu thích 12 đề thi", "/avatar/mystery.jpg"),
        new(10, "Yêu thích 1 giáo viên", "Yêu thích 1 giáo viên", "/avatar/mystery.jpg"),
        new(11, "Yêu thích 5 giáo viên", "Yêu thích 5 giáo viên", "/avatar/mystery.jpg"),
        new(12, "Đăng nhập liên tục 7 ngày", "Đăng nhập liên tục 7 ngày", "/avatar/mystery.jpg"),
        new(13, "Đăng nhập liên tục 30 ngày", "Đăng nhập liên tục 30 ngày", "/avatar/mystery.jpg"),
        new(14, "Đăng nhập liên tục 60 ngày", "Đăng nhập liên tục 60 ngày", "/avatar/mystery.jpg"),
        new(15, "Đăng nhập liên tục 120 ngày", "Đăng nhập liên tục 120 ngày", "/avatar/mystery.jpg"),
        new(16, "Trở thành thí sinh giải đề nhanh nhất và đạt điểm cao nhất", "Trở thành thí sinh giải đề nhanh nhất và đạt điểm cao nhất", "/avatar/mystery.jpg"),
        new(17, "Giải lại 1 đề 3 lần", "Giải lại 1 đề 3 lần", "/avatar/mystery.jpg"),
        new(18, "Hoàn thành 12 đề đạt được điểm 8 trở lên", "Hoàn thành 12 đề đạt được điểm 8 trở lên", "/avatar/mystery.jpg"),
        new(19, "Hoàn thành 12 đề đạt được điểm 10", "Hoàn thành 12 đề đạt được điểm 10", "/avatar/mystery.jpg"),
        new(20, "Hoàn thành tất cả danh hiệu", "Hoàn thành tất cả danh hiệu", "/avatar/mystery.jpg")
    };

    public AchievementDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Achievement> GetAchievements(ulong studentId)
    {
        try
        {
            var studentAchieve = _dbContext.Students.First(s => s.Id == studentId).Achievements;
            foreach (var a in studentAchieve) _achievements[a - 1].Status = true;

            return _achievements;
        }
        catch
        {
            return _achievements;
        }
    }

    public Student? IsAchieved(ushort achievementId, ulong userId)
    {
        if (_dbContext.Users.First(u => u.Id == userId).Role != "HS") return null;
        var s = _dbContext.Students.First(s => s.Id == userId);
        return s.Achievements.Contains(achievementId) ? null : s;
    }

    private void GainAchivement(bool isAchieve, ushort achievementId, Student student)
    {
        if (isAchieve)
        {
            student.Achievements.Add(achievementId);
            _dbContext.Update(student);
            _dbContext.SaveChanges();

            if (achievementId != 20) DetechAchievement(20, student.Id);
        }
    }

    public void DetechAchievement(ushort achievementId, ulong studentId)
    {
        Student? student = IsAchieved(achievementId, studentId);
        if (student == null) return;
        bool result = false;
        int countNormal, countPdf;

        switch (achievementId)
        {
            case 1:
                result = _dbContext.ExamAttempts.Any(ea => ea.StudentId == studentId) || _dbContext.PdfExamAttempts.Any(ea => ea.StudentId == studentId);
                GainAchivement(result, achievementId, student);
                break;

            case 2:
                result = _dbContext.ExamAttempts.Any(ea => ea.StudentId == studentId && ea.TotalPoint >= 8) || _dbContext.PdfExamAttempts.Any(ea => ea.StudentId == studentId && ea.TotalPoint >= 8);
                GainAchivement(result, achievementId, student);
                break;

            case 3:
                result = _dbContext.ExamAttempts.Any(ea => ea.StudentId == studentId && ea.TotalPoint == 10) || _dbContext.PdfExamAttempts.Any(ea => ea.StudentId == studentId && ea.TotalPoint == 10);
                GainAchivement(result, achievementId, student);
                break;

            case 7:
                result = _dbContext.Comments.Any(c => c.UserId == studentId);
                GainAchivement(result, achievementId, student);
                break;

            case 8:
                result = student.Favorites.Count == 1;
                GainAchivement(result, achievementId, student);
                break;

            case 9:
                result = student.Favorites.Count == 12;
                GainAchivement(result, achievementId, student);
                break;

            case 10:
                result = _dbContext.Followers.Count(s => s.StudentId == studentId) == 1;
                GainAchivement(result, achievementId, student);
                break;

            case 11:
                result = _dbContext.Followers.Count(s => s.StudentId == studentId) == 5;
                GainAchivement(result, achievementId, student);
                break;

            case 17:
                bool resultNormal = _dbContext.ExamAttempts.Where(ea => ea.StudentId == studentId).GroupBy(ea => ea.ExamId).Any(g => g.Count() >= 3);
                bool resultPdf = _dbContext.PdfExamAttempts.Where(ea => ea.StudentId == studentId).GroupBy(ea => ea.ExamId).Any(g => g.Count() >= 3);
                GainAchivement(resultNormal || resultPdf, achievementId, student);
                break;

            case 18:
                countNormal = _dbContext.ExamAttempts.Count(ea => ea.StudentId == studentId && ea.TotalPoint >= 8);
                countPdf = _dbContext.PdfExamAttempts.Count(ea => ea.StudentId == studentId && ea.TotalPoint >= 8);
                GainAchivement(countNormal + countPdf == 12, achievementId, student);
                break;

            case 19:
                countNormal = _dbContext.ExamAttempts.Count(ea => ea.StudentId == studentId && ea.TotalPoint == 10);
                countPdf = _dbContext.PdfExamAttempts.Count(ea => ea.StudentId == studentId && ea.TotalPoint == 10);
                GainAchivement(countNormal + countPdf == 12, achievementId, student);
                break;

            case 20:
                result = student.Achievements.Count == _achievements.Count - 1;
                GainAchivement(result, achievementId, student);
                break;

            default: return;
        }
    }
}
