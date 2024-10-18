using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using Firebase.Database;
using FireBaseRealTime.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FireBaseRealTime;

public partial class ViewModel : ObservableObject
{
    private readonly FirebaseClient firebaseClient;

    private IMapper mapper;    

    [ObservableProperty]
    private ObservableCollection<AccountInforModel> _listAccountInforModels = [];

    public ViewModel()
    {
        var config = new MapperConfiguration(cfg => cfg.CreateMap<object, AccountInforModel>());
        mapper = config.CreateMapper();

        var firebaseUrl = "https://pjrealtimedatabase-default-rtdb.asia-southeast1.firebasedatabase.app/";
        // Tạo FirebaseClient với URL database
        firebaseClient = new FirebaseClient(firebaseUrl);
        ListenForUserChanges();

        ListAccountInforModels.CollectionChanged += ListAccountInforModels_CollectionChanged; 
    }

    private void ListAccountInforModels_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action) 
        { 
            case NotifyCollectionChangedAction.Add:

                break;
            case NotifyCollectionChangedAction.Remove:

                break;
            default:
                break;
        }
    }

    public void ListenForUserChanges()
    {
        ObservableCollection<AccountInforModel> TG = new();
        var observable = firebaseClient
        .Child("Accounts")
        .AsObservable<dynamic>() // Lắng nghe thay đổi của node Accounts, kiểu dữ liệu là Account
        .Subscribe(async firebaseEvent =>
        {
            // Mỗi khi có sự thay đổi trong node Accounts, ta lấy toàn bộ danh sách
            if (firebaseEvent.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate ||
                firebaseEvent.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
            {
                // Lấy toàn bộ dữ liệu từ node Accounts
                var accounts = await firebaseClient
                    .Child("Accounts")
                    .OnceAsync<AccountInforModel>();

                var accountList = accounts.Select(a => a.Object).ToList();
                TG = new ObservableCollection<AccountInforModel>(accountList);

            }
            ListAccountInforModels = TG;
        });
    }

}
