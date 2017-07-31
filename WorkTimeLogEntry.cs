using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTimeLog
{
    public class WorkTimeLogEntry
    {
        public ObjectId Id { get; set; }
        public DateTime Time { get; set; }
        public WorkState Type { get; set; }

        public DateTime LocalTime
        {
            get
            {
                return Time.ToLocalTime();
            }
        }

        public WorkTimeLogEntry()
        {
            this.Id = ObjectId.NewObjectId();
        }
    }
}
