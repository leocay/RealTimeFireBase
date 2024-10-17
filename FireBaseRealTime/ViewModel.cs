using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Database;
using Firebase.Database.Query;
using System.Diagnostics;

namespace FireBaseRealTime;

public partial class ViewModel : ObservableObject
{
    private readonly FirebaseClient firebaseClient;

    public ViewModel()
    {
        var firebaseUrl = "https://pjrealtimedatabase-default-rtdb.asia-southeast1.firebasedatabase.app/";

        // Tạo FirebaseClient với URL database
        firebaseClient = new FirebaseClient(firebaseUrl);

        ListenForUserChanges();
    }

    [RelayCommand]
    public async Task AddData()
    {
        var data = new { Name = "John Doe", Age = 30 };

        // Ghi dữ liệu vào Firebase
        var result = await firebaseClient
            .Child("Users")     // Tạo hoặc truy cập node "Users"
            .PostAsync(data);
    }

    public void ListenForUserChanges()
    {
        firebaseClient
            .Child("Users")
            .AsObservable<dynamic>()
            .Subscribe(user =>
            {
                Debug.Print($"User {user.Key} changed: {user.Object}");
            });
    }

}
