📋 Proje Hakkında
Bu sistem, tam olarak 6 kişilik ekipler için tasarlanmış yenilikçi bir görev yönetim platformudur. Sudoku mantığından ilham alan benzersiz bir algoritma kullanarak, görevlerin adil ve dengeli bir şekilde dağıtılmasını sağlar.
✨ Temel Özellikler

🎲 Sudoku Temelli Algoritma: Her ekip üyesi haftalık döngüde her zorluk seviyesinden tam bir görev alır
⚖️ Adil Dağıtım: Matematiksel garanti ile dengeli iş yükü
📅 Esnek Planlama: Herhangi bir gün başlayabilen görev üretimi
👥 Ekip Yönetimi: 6 kişilik ekipler için optimize edilmiş
📊 Dashboard: Gerçek zamanlı görev takibi ve yönetimi
🔒 Güvenlik: JWT tabanlı kimlik doğrulama

🏗️ Teknoloji Stack
Backend

.NET 8 Web API - Güçlü sunucu tarafı framework
Entity Framework Core - ORM
SQLite - Veritabanı
JWT Authentication - Güvenli kimlik doğrulama

Frontend

React 18 - Modern UI framework
React Router - Client-side routing
Tailwind CSS - Responsive tasarım
Axios - HTTP client

🚀 Kurulum
Gereksinimler

.NET 8 SDK
Node.js (v16 veya üzeri)
npm veya yarn

Backend Kurulumu
bash# Proje dizinine gidin
cd TaskManagerAPI

# Bağımlılıkları yükleyin ve çalıştırın
dotnet restore
dotnet run
Backend varsayılan olarak http://localhost:5000 adresinde çalışacaktır.
Frontend Kurulumu
bash# Frontend dizinine gidin
cd TaskManagerFrontend

# Bağımlılıkları yükleyin
npm install

# Uygulamayı başlatın
npm start
Frontend http://localhost:3000 adresinde açılacaktır.
📊 Veritabanı Yapısı
Sistem 4 ana varlık üzerine kurulmuştur:

Company - Şirket bilgileri
User - Kullanıcı ve ekip bilgileri
TaskType - Görev türleri ve zorluk seviyeleri (1-6)
TaskItem - Görev detayları ve takibi

🎯 Sudoku Algoritması
Görev dağıtım algoritması şu formülü kullanır:
kullaniciIndex = (gorevIndex + gunOffset) % 6
Burada:

gorevIndex: Zorluk seviyesi (0-5, seviye 1-6'ya eşlenir)
gunOffset: Haftalık döngüdeki gün (0-5)
kullaniciIndex: Atanan ekip üyesi

Bu algoritma, her ekip üyesinin:

✅ Hafta başına her zorluk seviyesinden tam bir görev almasını
✅ Aynı hafta içinde aynı zorluk seviyesini tekrar almamasını
✅ 6 çalışma gününe dengeli dağılım sağlar

🔐 API Endpoints
Authentication
POST /api/Auth/register    # Kullanıcı kaydı
POST /api/Auth/login       # Giriş ve JWT token
GET  /api/Auth/companies   # Şirket listesi
Task Management
GET  /api/Tasks/weekly           # Haftalık görevler
POST /api/Tasks/generate/{id}    # Görev üretimi
PUT  /api/Tasks/{id}/status      # Görev durumu güncelle
👤 Varsayılan Giriş Bilgileri
Email: admin@example.com
Password: 123456
📱 Kullanıcı Arayüzü Özellikleri
Dashboard

📊 Şirkete özel görev görselleştirme
🎨 Dinamik görev kartları
🔍 Çok boyutlu filtreleme (Gün, Kullanıcı, Ekip)
✅ Gerçek zamanlı durum güncellemeleri

Kayıt Sistemi

🏢 Dinamik şirket seçimi
👥 Otomatik ekip kapasitesi kontrolü
⚠️ Kullanıcı dostu hata mesajları
✨ Anında geri bildirim

🛡️ Güvenlik Özellikleri

JWT tabanlı kimlik doğrulama
Güvenli şifre hashleme
SQL injection koruması
Input validation ve sanitization
CORS konfigürasyonu


