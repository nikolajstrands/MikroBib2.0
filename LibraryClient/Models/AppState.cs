using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryClient.ViewModels;
using LibraryClient.Models;
using LibraryClient.Services;
using LibraryDTOs;

namespace LibraryClient
{
    public class AppState : ObservableObject
    {
        // Applikationens systembruger
        private UserDTO user;
        public UserDTO User
        {
            get => user;
            set
            {
                user = value;
                OnPropertyChanged();
            }
        }
               
        // Repository for data
        public IRepo Repo { get; set; }

        // Er der uafsluttet aktivitet eller kan der skiftes mellem views?
        private bool canShiftView = true;
        public bool CanShiftView
        {
            get
            {
                return canShiftView;
            }
            set
            {
                if (canShiftView != value)
                {
                    canShiftView = value;
                    OnPropertyChanged();
                }
            }
        }

        // Constructor
        public AppState(IRepo repo)
        {
            Repo = repo;

        }       
    }
}
