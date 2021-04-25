using FetchData;
using GenshinWish.Apis;
using GenshinWish.Enums;
using GenshinWish.Models;
using GenshinWish.Utils;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace GenshinWish.GameBar.Modules.History
{
    public class HistoryViewModel : ObservableObject
    {
        private string _wishParam;

        private readonly IApiService<IWish> _wishService;

        public HistoryViewModel()
        {
            _wishService = Ioc.Default.GetRequiredService<IApiService<IWish>>();

            FetchHistoryCommand = new AsyncRelayCommand(FetchHistory);
        }

        private async Task FetchHistory()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".txt");
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            var file = await picker.PickSingleFileAsync();

            using (var str = await file.OpenStreamForReadAsync())
            {
                try
                {
                    _wishParam = GenshinLog.GetWishParam(str);
                }
                catch (Exception _)
                {
                    
                }
            }

            var wishConfigQuery = GenshinLog.GetWishQueryString(WishEndpoint.Config, _wishParam);
            var configResult = await _wishService.Initiated.GetWishConfig(wishConfigQuery);

            foreach (var config in configResult.Data.GachaTypeList)
            {
                Types.Add(config);
            }

            var wishLogParam = GenshinLog.AddWishLogParam(_wishParam, 0, 6, configResult.Data.GachaTypeList.First().Key);
            var wishLogQuery = GenshinLog.GetWishQueryString(WishEndpoint.Log, wishLogParam);
            var logResult = await _wishService.Initiated.GetWishLog(wishLogQuery);

            foreach (var wish in logResult.Data.List)
            {
                Histories.Add(wish);
            }
        }

        private string _selectedUID;
        public string SelectedUID
        {
            get => _selectedUID;
            private set => SetProperty(ref _selectedUID, value);
        }

        private WishType _selectedType;
        public WishType SelectedType
        {
            get => _selectedType;
            private set => SetProperty(ref _selectedType, value);
        }

        private string _search;
        public string Search
        {
            get => _search;
            private set => SetProperty(ref _search, value);
        }

        private string _selectedItemFilter;
        public string SelectedItemFilter
        {
            get => _selectedItemFilter;
            private set => SetProperty(ref _selectedItemFilter, value);
        }

        private string _selectedStarFilter;
        public string SelectedStarFilter
        {
            get => _selectedStarFilter;
            private set => SetProperty(ref _selectedStarFilter, value);
        }

        public ObservableCollection<string> UIDs { get; } = new ObservableCollection<string>();
        public ObservableCollection<WishType> Types { get; } = new ObservableCollection<WishType>();
        public ObservableCollection<Wish> Histories { get; } = new ObservableCollection<Wish>();

        //public IReadOnlyList<string> WishItemsFilter { get; } = new[] { "All", "Character", "Weapon" };
        //public IReadOnlyList<string> WishStarsFilter { get; } = new[] { "All", "3 Star", "4 Star", "5 Star" };

        public ICommand FetchHistoryCommand { get; }
    }
}
