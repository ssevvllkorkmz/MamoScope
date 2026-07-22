# MamoScope — Motor Sürücü Test Otomasyon Yazılımı

MamoScope, motor sürücü kartlarının voltaj ve kalite kontrol testlerini gerçekleştirmek, test sonuçlarını yerel bir veri tabanında saklamak, geriye dönük test geçmişini listelemek ve mevcut kayıtları düzenleyip yönetmek amacıyla geliştirilmiş **WPF (MVVM)** tabanlı bir masaüstü yazılımıdır.

## Kullanılan Teknolojiler

- **WPF (.NET)** — kullanıcı arayüzü
- **CommunityToolkit.Mvvm** — MVVM altyapısı (`ObservableObject`, `RelayCommand`, `[ObservableProperty]` gibi source generator tabanlı yapılar)
- **Entity Framework Core** — veritabanı erişimi (Code-First, Migrations)
- **Microsoft.Extensions.DependencyInjection** — bağımlılık yönetimi (Dependency Injection)

## Mimari Yaklaşım

Uygulama, sorumlulukları net biçimde ayıran katmanlı bir yapı izler:

```
ViewModel → Store → Service → Repository → DbContext → Veritabanı
```

- **Repository** — veritabanına yapılan ham CRUD işlemlerini yürütür, hiçbir iş kuralı içermez
- **Service** — iş kurallarını uygular (voltaj eşik kontrolü, seri numarası format kontrolü, aynı seri numarasıyla tekrar kayıt engeli)
- **Store** — uygulama genelinde paylaşılan veriyi (test kayıtları listesi) bellekte tek bir kopya olarak tutar; bir ekranda yapılan değişiklik diğer ekranlara otomatik yansır
- **ViewModel** — View'in ihtiyaç duyduğu veriyi ve komutları sağlar

Bağımlılıklar Dependency Injection container'ı üzerinden yönetilir. Durum (state) tutmayan sınıflar (`Repository`, `Service`) **Transient**, paylaşılan durumu bellekte tutan sınıflar (`Store`, `NavigationService`) ise gerekçeli bir istisna olarak **Singleton** kaydedilir.

### Runtime Parametreli ViewModel'ler İçin: ViewModel Factory

Bazı ViewModel'ler (örneğin kayıt detay ekranı), Dependency Injection container'ının önceden bilemeyeceği, yalnızca çalışma zamanında (kullanıcının seçtiği kayıt gibi) belirlenen parametrelere ihtiyaç duyar. Bu tür ViewModel'ler, `IViewModelFactory` aracılığıyla üretilir — container'ın sağladığı sabit servisler ile çağıran taraftan gelen dinamik veriyi birleştirerek kullanıma hazır bir ViewModel inşa eder.

## Kaynak Kod Yapısı

```
MamoScope/
├── Core/
│   └── Interfaces/
│       └── IMotorDriversService.cs      // Servis katmanının sözleşmesi (contract)
│
├── Converters/
│   └── BoolToResultConverter.cs         // bool (IsPassed) → "Başarılı"/"Başarısız" görüntü dönüşümü
│
├── Data/
│   ├── AppDbContext.cs                  // EF Core veritabanı bağlamı
│   ├── Migrations/                      // Veritabanı şema geçmişi
│   └── Repositories/
│       ├── IMotorDriversRepository.cs   // Repository sözleşmesi
│       └── MotorDriverRepository.cs     // Ham CRUD işlemleri (Create/Read/Update/Delete)
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
│   ├── MotorDriversService.cs           // İş kuralları (voltaj eşiği, format kontrolü, tekrar/çakışma engeli)
│   └── MotorDriversStore.cs             // Paylaşılan bellek cache'i (ObservableCollection)
│
├── ViewModels/
│   ├── Factories/
│   │   └── ViewModelFactory.cs          // Runtime parametreli ViewModel üretimi
│   ├── ViewModelBase.cs                 // Ortak ViewModel altyapısı
│   ├── MainWindowViewModel.cs           // Sayfalar arası geçiş / aktif içerik yönetimi
│   ├── TestRecordsViewModel.cs          // Test simülasyonu ve yeni kayıt işlemleri
│   ├── PastRecordsViewModel.cs          // Geçmiş kayıtların listelenmesi, seçme ve silme
│   └── RecordDetailsViewModel.cs        // Tekil kayıt görüntüleme ve düzenleme
│
├── Views/
│   ├── MainWindowView.xaml              // Uygulamanın gerçek ana penceresi
│   ├── TestRecordsView.xaml             // Kayıt/test sayfası arayüzü
│   ├── PastRecordsView.xaml             // Geçmiş kayıtlar sayfası arayüzü
│   └── RecordDetailsView.xaml           // Kayıt detay/düzenleme sayfası arayüzü
│
└── App.xaml / App.xaml.cs               // Uygulama girişi, DI servis kayıtları, kaynak birleştirme
```

> **Not:** Komut (Command) tanımları ve `INotifyPropertyChanged` altyapısı elle yazılmıyor; `CommunityToolkit.Mvvm` kütüphanesinin `[RelayCommand]` ve `[ObservableProperty]` attribute'ları (source generator) ile otomatik üretiliyor.

## Öne Çıkan Davranışlar

- **Kültürden bağımsız sayı ayrıştırma:** Voltaj değeri girilirken hem nokta (`24.5`) hem virgül (`24,5`) kabul edilir; sistem dilinden (Türkçe/İngilizce) bağımsız, tutarlı şekilde işlenir.
- **Seri numarası format kontrolü:** Seri numarası her zaman `OPT-DRV-XXXX` (4 haneli sayı) formatında olmak zorundadır; hem yeni kayıt oluştururken hem düzenlerken bu kural uygulanır.
- **Tekrar/çakışma engeli:** Aynı seri numarasıyla ikinci kez kayıt oluşturulması veya başka bir kayda ait seri numarasının kullanılması engellenir.
- **Otomatik senkronizasyon:** Bir ekranda yapılan ekleme, düzenleme veya silme işlemi, herhangi bir manuel yenileme gerekmeden diğer ekranlara (örn. Geçmiş Kayıtlar listesi) otomatik yansır.
- **Kayıt düzenleme:** Geçmiş Kayıtlar listesinden bir kayda tıklanarak detay sayfası açılır; burada yalnızca seri numarası ve voltaj değeri düzenlenebilir, test tarihi değişmez, başarı/başarısızlık durumu güncellenen voltaja göre otomatik yeniden hesaplanır.
- **Kayıt silme:** Geçmiş Kayıtlar listesinde bir satırın üzerine gelindiğinde beliren silme butonu ile, onay penceresi sonrası kayıt kalıcı olarak silinir.
- **Uygulama içi stilli bildirimler:** Sistemin varsayılan mesaj kutuları yerine, uygulamanın görsel kimliğine uygun özel tasarlanmış bilgilendirme ve onay pencereleri kullanılır.

## Veritabanı

Veritabanı şeması, Entity Framework Core Migrations ile yönetilir. Aşağıdaki script, `Script-Migration` komutu ile üretilmiştir:

```sql
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
```

## Uygulamanın Çalıştırılması

1. Uygulama açıldığında **Test Kaydı** ekranı ile karşılanır.
2. Kullanıcı isterse:
   - Seri numarası ve voltaj değerini **manuel olarak** girip test edebilir, veya
   - **"Test Verisi Simüle Et"** butonuna basarak rastgele üretilmiş değerlerle test yapabilir.
3. **"Voltaj Test ve Kaydet"** butonuna basıldığında:
   - Seri numarasının doğru formatta ve tekrarsız olduğu doğrulanır.
   - Girilen/simüle edilen voltaj değeri 23.5V–24.5V aralığında mı diye kontrol edilir, sonuca göre "Başarılı" / "Başarısız" olarak işaretlenir.
   - Kayıt veritabanına yazılır ve ekran, yeni bir test girilebilmesi için otomatik olarak temizlenir.
4. **"Geçmiş Kayıtları Görüntüle"** butonuyla, o ana kadar yapılmış tüm testlerin listesine göz atılabilir; bu liste, yeni bir test kaydedildiğinde veya bir kayıt düzenlenip silindiğinde otomatik olarak güncel kalır.
5. Listede bir kayda **tıklanarak** detay sayfası açılır:
   - Seri numarası ve voltaj değeri düzenlenebilir; test tarihi ve sonuç bilgisi salt okunurdur.
   - Değişiklikler kaydedildiğinde, aynı doğrulama kuralları (format, tekrar/çakışma, voltaj aralığı) tekrar uygulanır ve başarı durumu yeniden hesaplanır.
   - Kaydetme işlemi başarılı olduğunda kullanıcı otomatik olarak Geçmiş Kayıtlar sayfasına yönlendirilir.
6. Listede bir satırın üzerine gelindiğinde beliren **silme butonuna** basıldığında, onay penceresi sonrası kayıt kalıcı olarak silinir.
