using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using MauiApiStress.Model;

namespace MauiApiStress.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public Command InsertTask { get; set; }
        public Command InsertOrReplaceTask { get; set; }
        public Command BulkInsertTask { get; set; }
        public Command DeleteAll { get; set; }
        public MainViewModel() 
        {
            MainModel = new MainModel();
            
            InsertTask = new Command(async () => await MainModel.RunInsertTask());
            InsertOrReplaceTask = new Command(async () => await MainModel.RunInsertOrReplaceTask());
            BulkInsertTask = new Command(async () => await MainModel.RunBulkInsertTask());
            DeleteAll = new Command(async () => await MainModel.DeleteAllData());
        }

        private MainModel model;
        public MainModel MainModel 
        { 
            get { return model; }
            set 
            { 
                model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MainModel))); 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
