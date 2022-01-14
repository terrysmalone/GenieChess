using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Genie_WPF.Annotations;

namespace Genie_WPF
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected ViewModelBase()
        {
            Close = () => { };
        }

        public Action Close { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                RaisePropertyChanged(propertyName);
            }
        }

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            var changed = !EqualityComparer<T>.Default.Equals(field, value);

            if (changed)
            {
                field = value;
                RaisePropertyChanged(propertyName);
            }
        }

        public void RefreshAllBindings() => RaisePropertyChanged(string.Empty);

        public virtual bool AllowClose() => true;
    }
}
