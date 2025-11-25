using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class StudentDb : UserDb
{
    public StudentDb(ZaloRevisionAppDbContext dbContext) : base(dbContext) { }

    public async Task<User> GetStudentByIdAsync(ulong id)
    {
        return await GetUserByIdAsync(id);
    }

    public async Task<bool> AddStudent(User user, Student st)
    {
        if (!(await AddUser(user))) return false;
        _dbContext.Students.Add(new Student(user.Id, st.Grade));
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateStudent(User u, Student st)
    {
        if (!(await UpdateUser(u))) return false;

        var student = await _dbContext.Students.FirstAsync(s => s.Id == st.Id);
        student.Grade = st.Grade;

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteStudent(ulong id)
    {
        if (!(await NullifyUser(id))) return false;

        var student = await _dbContext.Students.FirstAsync(s => s.Id == id);
        _dbContext.Students.Remove(student);

        return await DeleteAllHistoriesByStudentId(id);
    }

    // ATTENDANCE
    public async Task<ICollection<DateTime>> GetAttendance(ulong id)
    {
        return await _dbContext.Students.Where(s => s.Id == id).Select(s => s.Attendance).FirstAsync();
    }

    public async Task<bool> MarkAttendance(ulong id)
    {
        var student = await _dbContext.Students.FirstAsync(s => s.Id == id);
        if (student.Attendance.Any(dt => dt == DateTime.Today)) return true;

        List<DateTime> newDate = [];
        DateTime twoDayAgo = DateTime.Today.AddDays(-2);

        while (twoDayAgo < DateTime.Today)
        {
            if (newDate.Count > 0 || student.Attendance.Any(dt => dt == twoDayAgo))
            {
                newDate.Add(twoDayAgo);
                twoDayAgo.AddDays(1);
            }
            else
            {
                student.Streak = 0;
                break;
            }
        }
        newDate.Add(DateTime.Today);
        student.Streak += 1;
        student.Attendance = newDate;

        return await _dbContext.SaveChangesAsync() > 0;
    }

    // FAVORITE
    public async Task<List<ExamReadDTO>> GetFavorite(ulong studentId)
    {
        var favoriteExams = await _dbContext.Students
            .Where(sp => sp.Id == studentId).Select(sp => sp.Favorites).FirstAsync();

        var result = await (from e in _dbContext.Exams
                            join u in _dbContext.Users
                            on e.TeacherId equals u.Id
                            join s in _dbContext.Subjects
                            on e.SubjectId equals s.Id
                            where favoriteExams.Contains(e.Id)
                            select new ExamReadDTO
                            {
                                Id = e.Id,
                                ExamType = e.ExamType,
                                DisplayType = e.DisplayType,
                                Title = e.Title,
                                Grade = e.Grade,
                                TimeLimit = e.TimeLimit,
                                EarlyTurnIn = e.EarlyTurnIn,
                                AllowShowScore = e.AllowShowScore,
                                AllowPartSwap = e.AllowPartSwap,
                                TeacherId = u.Id,
                                TeacherName = u.Name,
                                SubjectId = s.Id,
                                SubjectName = s.Name,
                                Status = e.Status
                            }).ToListAsync();

        return result;
    }

    public async Task<bool> HandleFavorite(ulong studentId, ulong examId)
    {
        var student = await _dbContext.Students.FirstOrDefaultAsync(sp => sp.Id == studentId);
        if (student == null) return false;

        bool isAvaiable = student.Favorites.Contains(examId);

        if (isAvaiable) student.Favorites.Remove(examId);
        else student.Favorites.Add(examId);

        _dbContext.Students.Update(student);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    // HISTORY
    public async Task<List<ExamReadDTO>> GetHistory(ulong studentId)
    {
        var result = await (from h in _dbContext.StudentHistories
                            join e in _dbContext.Exams
                            on h.ExamId equals e.Id
                            join u in _dbContext.Users
                            on e.TeacherId equals u.Id
                            join s in _dbContext.Subjects
                            on e.SubjectId equals s.Id
                            where h.StudentId == studentId
                            orderby h.Time descending
                            select new ExamReadDTO
                            {
                                Id = e.Id,
                                ExamType = e.ExamType,
                                DisplayType = e.DisplayType,
                                Title = e.Title,
                                Grade = e.Grade,
                                TimeLimit = e.TimeLimit,
                                EarlyTurnIn = e.EarlyTurnIn,
                                AllowShowScore = e.AllowShowScore,
                                AllowPartSwap = e.AllowPartSwap,
                                TeacherId = u.Id,
                                TeacherName = u.Name,
                                SubjectId = s.Id,
                                SubjectName = s.Name,
                                Status = e.Status
                            }).ToListAsync();

        return result;
    }

    public async Task<bool> HandleHistory(ulong studentId, ulong examId)
    {
        var existingHistory = await _dbContext.StudentHistories
            .Where(h => h.StudentId == studentId && h.ExamId == examId && h.Time.Date == DateTime.Today)
            .FirstOrDefaultAsync();

        if (existingHistory == null)
        {
            await _dbContext.StudentHistories.AddAsync(new StudentHistory { StudentId = studentId, ExamId = examId });
        }
        else
        {
            existingHistory.Time = DateTime.Now;
            _dbContext.StudentHistories.Update(existingHistory);
        }
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteHistory(ulong id)
    {
        var deleteHistory = await _dbContext.StudentHistories.Where(h => h.Id == id).FirstAsync();
        _dbContext.StudentHistories.Remove(deleteHistory);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAllHistoriesByStudentId(ulong studentId)
    {
        var deleteHistory = await _dbContext.StudentHistories.Where(h => h.StudentId == studentId).ToListAsync();
        _dbContext.StudentHistories.RemoveRange(deleteHistory);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    // FOLLOWERS
    public async Task<bool> HandleFollowing(ulong studentId, ulong teacherId)
    {
        var favoriteEntity = await _dbContext.Followers
                                .Where(f => f.StudentId == studentId && f.TeacherId == teacherId)
                                .FirstOrDefaultAsync();

        if (favoriteEntity == null)
            await _dbContext.Followers.AddAsync(new Follower { StudentId = studentId, TeacherId = teacherId });
        else
        {
            favoriteEntity.IsVisible = !favoriteEntity.IsVisible;
            _dbContext.Followers.Update(favoriteEntity);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }
}
