using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LibraryClient
{
    // Klasse som alle ViewModel og også andre objekter arver fra for at kunne bruge databinding.

    public abstract class ObservableObject : INotifyPropertyChanged
    {           
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }     
    }

}
