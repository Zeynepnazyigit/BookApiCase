# Kitap Yönetim REST API

Bu proje ASP.NET Core Web API ile geliştirilmiş basit bir REST API uygulamasıdır.
Kitap verileri veritabanı yerine yerel bir JSON dosyasında (books.json) tutulur.

## Teknolojiler

- ASP.NET Core Web API
- .NET 8
- Swagger / OpenAPI
- System.Text.Json

## Proje Yapısı

- Controllers
- Services
- Models
- DTOs
- Middleware
- Data

## Özellikler

### Kitap Endpoint’leri

- GET /api/books
  - Tüm kitapları döner
  - Opsiyonel yazar filtresi: GET /api/books?author=Orhan

- GET /api/books/{id}
  - Belirtilen id’ye sahip kitabı döner
  - Bulunamazsa 404 Not Found

- POST /api/books
  - Yeni kitap ekler
  - id sistem tarafından otomatik atanır
  - Başarılıysa 201 Created

- PUT /api/books/{id}
  - Mevcut kitabı günceller
  - Bulunamazsa 404 Not Found

- DELETE /api/books/{id}
  - Kitabı siler
  - Başarılıysa 204 No Content

### Comments Endpoint’i

- GET /api/comments
  - Dış servisten yorumları çeker: https://jsonplaceholder.typicode.com/posts/1/comments
  - Dönüşümler:
    - name → büyük harfe çevrilir
    - email → maskelenir
    - body → ilk 50 karakter alınır ve sonuna ... eklenir
  - Dış servise erişilemezse 502 Bad Gateway döner

## Validasyon Kuralları

POST ve PUT işlemleri için:

- title zorunlu (boş olamaz)
- author zorunlu (boş olamaz)
- publishedYear 1000 ile mevcut yıl arasında olmalı
- pageCount 1’den büyük olmalı

## Hata Yönetimi

- Service katmanında try/catch kullanılır
- Global exception handler middleware eklenmiştir
- Hata durumunda kullanıcıya anlamlı mesaj döndürülür

## Loglama

ILogger ile loglama yapılır:

- Kitap ekleme / güncelleme / silme işlemleri
- Kitap bulunamadığında LogWarning
- Beklenmeyen hatalarda LogError

## Çalıştırma (CLI)

-dotnet restore

-dotnet run

Çalıştırdıktan sonra Swagger UI:
- /swagger

## Örnek Request Body (POST / PUT)

{
  "title": "Tutunamayanlar",
  "author": "Oğuz Atay",
  "publishedYear": 1971,
  "pageCount": 724
}
