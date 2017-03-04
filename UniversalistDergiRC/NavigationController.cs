﻿using UniversalistDergiRC.DataAccess;
using UniversalistDergiRC.Model;
using UniversalistDergiRC.Repositories;
using UniversalistDergiRC.ViewModels;
using UniversalistDergiRC.Views;
using Xamarin.Forms;

namespace UniversalistDergiRC
{
    public class NavigationController
    {
        private int currentIssue;
        private int currentPage;
        private TabbedPage detailTabPage;
        private MasterDetailPage mainPage;
        private CarouselPage menuCarouselPage;

        public void InitializeController(MasterDetailPage mainMasterDetail, TabbedPage detailCarousel, CarouselPage menuCarousel)
        {
            menuCarouselPage = menuCarousel;
            detailTabPage = detailCarousel;
            mainPage = mainMasterDetail;
        }

        public void OpenReadingPage(int issueNumber, int pageNumber)
        {
            if (detailTabPage == null || issueNumber == 0)
                return;

            if (detailTabPage.Children.Count == 1)
                detailTabPage.Children.Add(new ReadingPageView(this));

            ReadingPageView readingPage = detailTabPage.Children[1] as ReadingPageView;

            if (readingPage == null) return;

            mainPage.IsPresented = false;

            ReadingPageViewModel vmReadingPage = readingPage.BindingContext as ReadingPageViewModel;
            if (vmReadingPage == null) return;
            detailTabPage.IsBusy = true;
            vmReadingPage.OpenMagazine(issueNumber, pageNumber);
            detailTabPage.IsBusy = false;

            detailTabPage.CurrentPage = detailTabPage.Children[1];

            SetCurrentPageForResume(0, 0);
        }

        internal void CloseBookmarkListPage()
        {
            if (menuCarouselPage == null || menuCarouselPage.Children.Count == 0)
                return;
            menuCarouselPage.CurrentPage = menuCarouselPage.Children[0];
        }

        internal void OpenBookmarkListPage()
        {
            if (menuCarouselPage == null)
                return;

            if (menuCarouselPage.Children.Count == 1)
                menuCarouselPage.Children.Add(new BookmarkListView(this));

            BookmarkListView bookmarkListPage = menuCarouselPage.Children[1] as BookmarkListView;

            if (bookmarkListPage == null) return;

            BookmarkListViewModel vmBookmarkList = bookmarkListPage.BindingContext as BookmarkListViewModel;
            if (vmBookmarkList == null) return;

            menuCarouselPage.IsBusy = true;
            vmBookmarkList.ListAllBookMarks();
            menuCarouselPage.IsBusy = false;

            menuCarouselPage.CurrentPage = menuCarouselPage.Children[1];
        }

        internal void OpenMagazineListPage()
        {
            if (detailTabPage == null || detailTabPage.Children.Count == 0)
                return;
            mainPage.IsPresented = false;
            detailTabPage.CurrentPage = detailTabPage.Children[0];
            SetCurrentPageForResume(0, 0);

            if (detailTabPage.Children.Count > 1)
                detailTabPage.Children.RemoveAt(1);
        }

        internal async void ResumeAsync()
        {
            BookmarkModel savedState = ClientDataManager.GetState();

            if (savedState != null)
            {
                var resumeSelected = await mainPage.DisplayAlert("UNIVERSALIST DERGI", "Kaldığınız yerden devam etmek ister misiniz?", "Evet", "Hayır");

                if (resumeSelected)
                    OpenReadingPage(savedState.IssueNumber, savedState.PageNumber);
                else
                    OpenMagazineListPage();
            }
        }

        internal void SaveState()
        {
            string state = string.Empty;

            if (currentIssue > 0 && currentPage > 0)
                state = string.Format(Constants.GENERIC_STATE_FORMAT, currentIssue, currentIssue);
            ClientDataManager.SaveState(state);
        }

        internal void SetCurrentPageForResume(int issue, int activePageNumber)
        {
            currentIssue = issue;
            currentPage = activePageNumber;
        }
    }
}
