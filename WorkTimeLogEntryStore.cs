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

                WorkTimeLogEntry firstEntry = Entries.FirstOrDefault();

                if (firstEntry == null)
                {
                    return defaultType;
                }
                else
                {
                    return GetNextWorkState(firstEntry.Type);
                }
            }
        }

        private ObservableCollection<WorkTimeLogEntry> _Entries = null;

        public event PropertyChangedEventHandler PropertyChanged;

        private int _LoadCount = 10;

        public int LoadCount
        {
            get
            {
                return this._LoadCount;
            }
            set
            {
                this._LoadCount = value;
                this.Entries = LoadEntries();
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LoadCount"));
                }
            }
        }

        private ObservableCollection<WorkTimeLogEntry> LoadEntries()
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return new ObservableCollection<WorkTimeLogEntry>();
            }
            else
            {
                ObservableCollection<WorkTimeLogEntry> NewEntries;

                using (LiteDatabase db = new LiteDatabase(DB_FILENAME))
                {
                    var workTimeEntries = db.GetCollection<WorkTimeLogEntry>(DB_COLLECTION);
                    NewEntries = new ObservableCollection<WorkTimeLogEntry>(workTimeEntries.Find(Query.All(Query.Descending), limit:LoadCount));
                    NewEntries.CollectionChanged += NewEntries_CollectionChanged;
                }

                return NewEntries;
            }
        }

        public ObservableCollection<WorkTimeLogEntry> Entries
        {
            get
            {
                if (_Entries != null)
                {
                    return _Entries;
                }
                else
                {
                    this.Entries = LoadEntries();

                    return _Entries;
                }
            }
            private set
            {
                _Entries = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Entries"));
                }
            }
        }

        private void NewEntries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("WorkType"));
                }
            }
        }

        public void Insert(WorkTimeLogEntry newEntry)
        {
            Entries.Insert(0, newEntry);
        }

        public void Clear()
        {
            Entries.Clear();
        }

        public void SetLoadCount(int n)
        {

        }
    }
}
