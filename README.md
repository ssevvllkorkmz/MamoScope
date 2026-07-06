# MamoScopeTest - Motor Sürücü Test Otomasyon Yazılımı

MamoScopeTest, motor sürücü kartlarının voltaj ve kalite kontrol testlerini gerçekleştirmek, test sonuçlarını yerel bir veri tabanında saklamak ve geriye dönük test geçmişini listelemek amacıyla geliştirilmiş "WPF (MVVM)" tabanlı bir masaüstü yazılımıdır.

Kaynak Kod Yapısı:
Mamascope
 -Models
   --MotorDrivers.cs // tanımlama sınıfı
 -Viewmodel
   --PastRecordsViewModel.cs // geçmiş sayfaları görüntüleyebilmek için yazdığım sınıf
   --TestrecordsViewModel.cs // kayıt,veri test simülasyonunun gerçekleşmesi için yazdığım sınıf
   --ViewModelBase // ViewModellere ortak INotıfyPropertyChanged özelliği kazandırma 
   --MainWindowViewModel // Sayfalar arası geçiş
 -Views
   --MainWindowView.cs // sabit çerçeve
   --TestRecordsView.xmal // kayıt sayfasını görüntüleyen arayüz dosyası
   --PastRecordsView.xmal // geçmiş kayıtların görüntülendiği arayüz dosyası
 -Core
   --RelayCommand.cs // XMAL'deki butonu C# metoduna bağlayan köprü
 -Data
   --AppDbContext.cs // EF core ile veri tabanına bağlanma 
 -App.xmal // Uygulama başlangıcı ve DI servis kayıtları
 -Migrations // değişiklikleri veri tabanına yansıtma 

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

