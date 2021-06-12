using FetchData;
using GenshinWish.Apis;
using GenshinWish.Enums;
using GenshinWish.Models;
using GenshinWish.Utils;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;

namespace GenshinWish.GameBar.Modules.History
{
    public class HistoryViewModel : ObservableObject
    {
        private const string _outputLog = "output_log.txt";

        private string _wishParam;
        private MessageDialog _requestPermission;

        private readonly IApiService<IWish> _wishService;
        private readonly string _gameDataPath;

        public HistoryViewModel()
        {
            _wishService = Ioc.Default.GetRequiredService<IApiService<IWish>>();
            var dirInfo = new DirectoryInfo(ApplicationData.Current.LocalFolder.Path).Parent.Parent.Parent;
            _gameDataPath = Path.Combine(dirInfo.FullName + "Low", "miHoYo", "Genshin Impact");

            FetchHistoryCommand = new AsyncRelayCommand(FetchHistory);
        }

        private async Task FetchHistory()
        {
            try
            {
                var log = await StorageFile.GetFileFromPathAsync(Path.Combine(_gameDataPath, _outputLog));
                _wishParam = GenshinLog.GetWishParam(await log.OpenStreamForReadAsync());

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
            catch (UnauthorizedAccessException)
            {
                _requestPermission = _requestPermission ??
                    new MessageDialog(
                        $"The app needs to access the {_gameDataPath}. {Environment.NewLine}" +
                        "Press OK to open system settings and give this app File System permission.");
                var okCommand = new UICommand("Go to Settings");
                _requestPermission.Commands.Add(okCommand);
                var cancelCommand = new UICommand("Cancel");
                _requestPermission.Commands.Add(cancelCommand);

                var requestPermissionResult = await _requestPermission.ShowAsync();
                if (requestPermissionResult == okCommand)
                {
                    //open app settings to allow users to give us permission
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
                }
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
