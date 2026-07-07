<#
.SYNOPSIS
  SQL Server hizmet hesabına belirtilen klasöre NTFS tam erişim verir.

.DESCRIPTION
  Yönetici olarak çalıştırın. Betik: hedef klasörü oluşturur, sistemdeki MSSQL* servislerini bulur
  ve her hizmetin çalıştığı hesaba klasöre Full Control izni verir.

.PARAMETER FolderPath
  Oluşturulacak / izin verilecek klasör (varsayılan: C:\MamoScopeDbData)

.PARAMETER ServiceName
  (İsteğe bağlı) Sadece belirli bir MSSQL servisine izin vermek için servis adı girin (ör. MSSQL$SQLEXPRESS).

.EXAMPLE
  .\Set-MSSQLFolderPermissions.ps1 -FolderPath "C:\MamoScopeDbData"
#>

param(
  [string]$FolderPath = "C:\MamoScopeDbData",
  [string]$ServiceName = ""
)

# Yönetici kontrolü
$principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
  Write-Error "Bu betiği Yönetici olarak çalıştırmalısınız."
  exit 1
}

# Klasör oluştur
if (-not (Test-Path -Path $FolderPath)) {
  New-Item -Path $FolderPath -ItemType Directory -Force | Out-Null
  Write-Output "Klasör oluşturuldu: $FolderPath"
} else {
  Write-Output "Klasör zaten var: $FolderPath"
}

# MSSQL servislerini al
$filter = "Name LIKE 'MSSQL%'"
if ($ServiceName -ne "") {
  $filter = "Name = '$ServiceName'"
}
$services = Get-CimInstance -ClassName Win32_Service -Filter $filter | Select-Object Name,StartName

if (-not $services -or $services.Count -eq 0) {
  Write-Warning "Hiçbir MSSQL servisi bulunamadı. Eğer LocalDB kullanıyorsanız bu betik gerekli değildir."
  Write-Output "Bulunan hizmetler:"
  Get-CimInstance -ClassName Win32_Service -Filter "Name LIKE 'MSSQL%'" | Select Name,StartName | Format-Table -AutoSize
  exit 0
}

foreach ($svc in $services) {
  $acct = $svc.StartName

  # StartName gösterimlerini normalize et
  switch ($acct) {
	"LocalSystem" { $aclAccount = "NT AUTHORITY\SYSTEM" ; break }
	"NT AUTHORITY\LocalService" { $aclAccount = "NT AUTHORITY\LOCAL SERVICE"; break }
	"LocalService" { $aclAccount = "NT AUTHORITY\LOCAL SERVICE"; break }
	"NT AUTHORITY\NetworkService" { $aclAccount = "NT AUTHORITY\NETWORK SERVICE"; break }
	"NetworkService" { $aclAccount = "NT AUTHORITY\NETWORK SERVICE"; break }
	default { $aclAccount = $acct }
  }

  Write-Output "Servis: $($svc.Name) - Hesap: $acct -> İzin veriliyor: $aclAccount"

  # icacls ile izin ver (OI)(CI)F = Full Control
  $icaclsCmd = "icacls \"$FolderPath\" /grant \"$aclAccount\":(OI)(CI)F /T"
  Write-Output "Çalıştırılıyor: $icaclsCmd"
  $res = cmd.exe /c $icaclsCmd
  Write-Output $res
}

Write-Output "İşlem tamamlandı. Gerekirse SQL servislerini yeniden başlatın."