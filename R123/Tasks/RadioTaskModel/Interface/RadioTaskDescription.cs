using RadioTask.Model.Generator;
using System.ComponentModel;
using System.Windows;

namespace RadioTask.Interface
{
    public class RadioTaskDescription : INotifyPropertyChanged
    {
        public string Description { get; set; }
        public RadioTaskType Type { get; set; }
        public object[] Frequencys { get; set; }

        private Visibility visibility = Visibility.Collapsed;
        public Visibility Visibility
        {
            get
            {
                return visibility;
            }
            set
            {
                visibility = value;
                OnPropertyChanged("Visibility");
            }
        }

        private object selectedItem;
        public object SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion;
    }
}
