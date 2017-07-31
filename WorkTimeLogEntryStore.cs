using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTimeLog
{
    public class WorkTimeLogEntryStore : INotifyPropertyChanged
    {
        public const string DB_FILENAME = "worktime.litedb";
        public const string DB_COLLECTION = "work_time";

        private WorkState GetNextWorkState(WorkState currentWorkState)
        {
            if (currentWorkState == WorkState.上班)
            {
                return WorkState.下班;
            }
            else
            {
                return WorkState.上班;
            }
        }

        public WorkState WorkType
        {
            get
            {
                WorkState defaultType = WorkState.上班;

                WorkTimeLogEntry lastEntry = Entries.LastOrDefault();

                if (lastEntry == null)
                {
                    return defaultType;
                }
                else
                {
                    return GetNextWorkState(lastEntry.Type);
                }
            }
        }

        private ObservableCollection<WorkTimeLogEntry> _Entries = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<WorkTimeLogEntry> Entries
        {
            get
            {
                if (_Entries != null)
                {
                    return _Entries;
                }
                else if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                {
                    _Entries = new ObservableCollection<WorkTimeLogEntry>();
                }
                else
                {
                    using (LiteDatabase db = new LiteDatabase(DB_FILENAME))
                    {
                        var workTimeEntries = db.GetCollection<WorkTimeLogEntry>(DB_COLLECTION);
                        _Entries = new ObservableCollection<WorkTimeLogEntry>(workTimeEntries.FindAll());
                        _Entries.CollectionChanged += _Entries_CollectionChanged;
                    }
                }

                return _Entries;
            }
        }

        private void _Entries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            using (LiteDatabase db = new LiteDatabase(DB_FILENAME))
            {
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                        db.DropCollection(DB_COLLECTION);
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        var workTimeEntries = db.GetCollection<WorkTimeLogEntry>(DB_COLLECTION);
                        workTimeEntries.Insert(e.NewItems.OfType<WorkTimeLogEntry>());
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("WorkType"));
            }
        }

        public void Insert(WorkTimeLogEntry newEntry)
        {
            Entries.Add(newEntry);
        }

        public void Clear()
        {
            Entries.Clear();
        }
    }
}
