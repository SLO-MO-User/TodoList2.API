using System;

namespace TodoList2.API.Dto
{
    public class TodoForReturnDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TaskName { get; set; }
        public bool IsComplete { get; set; }
        public bool IsImportant { get; set; }
        public string Note { get; set; }
        public bool IsInTodayView { get; set; }
        public DateTime RemindMeDateTime { get; set; }
        public DateTime TaskLastDateTime { get; set; }
        public DateTime CreatedAtDateTime { get; set; }
        public DateTime LastUpdatedAtDateTime { get; set; }
        public string ListName { get; set; }
    }
}
