﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using UniversalistDergiRC.Core;
using UniversalistDergiRC.DataAccess;
using UniversalistDergiRC.Model;
using UniversalistDergiRC.Repositories;
using Xamarin.Forms;

namespace UniversalistDergiRC.ViewModels
{
    public class MagazineListViewModel : BaseModel
    {
        public MagazineListViewModel(NavigationController controller)
        {
            _navigationController = controller;
            refreshMagazineIssueList(true);
        }

        private ObservableCollection<MagazineSummaryModel> _magazineIssueList;

        public ObservableCollection<MagazineSummaryModel> MagazineIssueList
        {
            get
            {
                _magazineIssueList = _magazineIssueList ?? new ObservableCollection<MagazineSummaryModel>();
                return _magazineIssueList;
            }
            set
            {
                if (_magazineIssueList != value)
                {
                    _magazineIssueList = value;
                    OnPropertyChanged(() => MagazineIssueList);
                }
            }
        }

        private ICommand _openReadingPageCommand;
        public ICommand OpenReadingPageCommand
        {
            get
            {
                _openReadingPageCommand = _openReadingPageCommand ?? new Command(openSelectedMagazine);
                return _openReadingPageCommand;
            }
            set
            {
                if (_openReadingPageCommand != value)
                {
                    _openReadingPageCommand = value;
                    OnPropertyChanged(() => OpenReadingPageCommand);
                }
            }
        }
        private MagazineSummaryModel selectedMagazine;
        public MagazineSummaryModel SelectedMagazine
        {
            get
            {
                return selectedMagazine;
            }
            set
            {
                if (selectedMagazine != value)
                {
                    selectedMagazine = value;
                    openSelectedMagazine(selectedMagazine);
                    OnPropertyChanged(() => SelectedMagazine);
                }
            }
        }
        private void openSelectedMagazine(object obj)
        {
            MagazineSummaryModel selectedMagazine = obj as MagazineSummaryModel;
            if (selectedMagazine == null)
                return;

            _navigationController.OpenReadingPage(selectedMagazine.Issue, Constants.FIRST_PAGE_NUMBER);
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                _refreshCommand = _refreshCommand ?? new Command(refreshMagazineIssueListCommand);
                return _refreshCommand;
            }
            set
            {
                if (_refreshCommand != value)
                {
                    _refreshCommand = value;
                    OnPropertyChanged(() => RefreshCommand);
                }
            }
        }



        private bool _isRefreshing;
        private NavigationController _navigationController;

        public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }

            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }
        private void refreshMagazineIssueListCommand(object obj)
        {
            refreshMagazineIssueList(false);
        }
        private void refreshMagazineIssueList(bool tryLocal)
        {
            IsRefreshing = true;
            MagazineIssueList = DataAccessManager.GetMagazineIssues(tryLocal);
            IsRefreshing = false;
        }
    }
}