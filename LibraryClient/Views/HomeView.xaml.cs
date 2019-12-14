using LibraryClient.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LibraryClient.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();

        }

        // En-vejs databinding fra View til ViewModel
        // Herfra https://stackoverflow.com/questions/1483892/how-to-bind-to-a-passwordbox-in-mvvm
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
               
                ((HomeViewModel)this.DataContext).SecurePassword = ((PasswordBox)sender).SecurePassword;
            }
        }

        // Når HomeView-komponenten loades, tilføjes event-handleren til PropertyChanged-eventet i HomeViewModel
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ((HomeViewModel)this.DataContext).PropertyChanged += new PropertyChangedEventHandler(ViewModel_PropertyChanged);
        }

        // Event-handler, der lytter efter om passwordet i HomeViewModel bliver tomt
        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(((HomeViewModel)sender).SecurePassword.Length == 0){
                PasswordBox.Clear();
            }
          
        }

       
    }
}
