# MamoScopeTest - Motor Sürücü Test Otomasyon Yazılımı

MamoScopeTest, motor sürücü kartlarının voltaj ve kalite kontrol testlerini gerçekleştirmek, test sonuçlarını yerel bir veri tabanında saklamak ve geriye dönük test geçmişini listelemek amacıyla geliştirilmiş "WPF (MVVM)" tabanlı bir masaüstü yazılımıdır.

MamoScope/
├── Core/
│   └── Interfaces/
│       └── IMotorDriversService.cs      // Servis katmanının sözleşmesi (contract)
│
├── Data/
│   ├── AppDbContext.cs                  // EF Core veritabanı bağlamı
│   ├── Migrations/                      // Veritabanı şema geçmişi
│   └── Repositories/
│       ├── IMotorDriversRepository.cs   // Repository sözleşmesi
│       └── MotorDriverRepository.cs     // Ham CRUD işlemleri (veritabanına git-gel)
│
├── Models/
│   └── MotorDrivers.cs                  // Veri modeli (SerialNumber, Voltage, IsPassed, TestDate)
│
├── Navigations/
│   ├── INavigationService.cs            // Sayfa geçişi sözleşmesi
│   └── NavigationService.cs             // ViewModel-first navigasyon yönetimi
│
├── Resources/
│   ├── Colors.xaml                      // Renk paleti
│   ├── Buttons.xaml                     // Buton stilleri
│   ├── Inputs.xaml                      // Giriş kutusu stilleri
│   └── TextStyles.xaml                  // Yazı stilleri
│
├── Services/
│   ├── MotorDriversService.cs           // İş kuralları (voltaj eşiği, tekrar kayıt engeli)
│   └── MotorDriversStore.cs             // Paylaşılan bellek cache'i (ObservableCollection)
│
├── ViewModels/
│   ├── ViewModelBase.cs                 // Ortak ViewModel altyapısı
│   ├── MainWindowViewModel.cs           // Sayfalar arası geçiş / aktif içerik yönetimi
│   ├── TestRecordsViewModel.cs          // Test simülasyonu ve kayıt işlemleri
│   └── PastRecordsViewModel.cs          // Geçmiş kayıtların listelenmesi
│
├── Views/
│   ├── MainWindowView.xaml              // Uygulamanın gerçek ana penceresi
│   ├── TestRecordsView.xaml             // Kayıt/test sayfası arayüzü
│   └── PastRecordsView.xaml             // Geçmiş kayıtlar sayfası arayüzü
│
└── App.xaml / App.xaml.cs               // Uygulama girişi, DI servis kayıtları, kaynak birleştirme

 MamoScope Veritabanı Oluşturma Scripti:
 Script-Migration komutu ile elde edildi 

 IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [MotorDrivers] (
    [Id] int NOT NULL IDENTITY,
    [SerialNumber] nvarchar(50) NOT NULL,
    [Voltage] float NOT NULL,
    [TestDate] datetime2 NOT NULL,
    [IsPassed] bit NOT NULL,
    CONSTRAINT [PK_MotorDrivers] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260703103037_InitialCreate', N'10.0.9');

COMMIT;
GO

Uygulama Çalıştırılması:
Kullanıcı isterse manuel olarak girdiği seri numarası ve voltaj değerini test edebilir ya da "test verisi simüle et" butonuna basarak random değerlerle test yapabilir.
Geçmiş kayıtları görüntüle butonuyla yaptığı kayıtların listesine göz atabilir.

