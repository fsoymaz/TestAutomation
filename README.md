# Test Automation Project

Bu proje, .NET 8 ile geliştirilmiş bir test otomasyon projesidir. GitHub Actions ve Jenkins ile CI/CD pipeline'ı yapılandırılmıştır.

## Proje Yapısı

```
TestAutomation/
├── Calculator/              # Ana proje (Class Library)
├── Calculator.Tests/        # Unit test projesi
├── .github/
│   └── workflows/
│       └── ci-cd.yml       # GitHub Actions workflow
├── Jenkinsfile             # Jenkins pipeline yapılandırması
└── README.md
```

## Özellikler

- ✅ .NET 8.0
- ✅ xUnit test framework
- ✅ GitHub Actions CI/CD
- ✅ Jenkins pipeline
- ✅ Otomatik merge (test geçerse main branch'e)
- ✅ Hata loglama ve issue oluşturma

## CI/CD Pipeline

### GitHub Actions

1. **Test Branch'ine Push**: Test branch'ine push yapıldığında workflow tetiklenir
2. **Test Çalıştırma**: Unit testler otomatik olarak çalıştırılır
3. **Başarılı Durumda**: Testler geçerse otomatik olarak main branch'e merge edilir
4. **Başarısız Durumda**: Testler fail olursa:
   - Test sonuçları loglanır
   - GitHub'da otomatik issue oluşturulur
   - Detaylı hata logları issue'da görüntülenir

### Jenkins

Jenkins pipeline aşağıdaki adımları içerir:
- Checkout
- Restore dependencies
- Build
- Test (sonuçlar loglanır)
- Deploy to Main (test geçerse)

## Kullanım

### Projeyi Çalıştırma

```bash
# Dependencies restore
dotnet restore

# Build
dotnet build

# Test çalıştırma
dotnet test
```

### Test Branch'ine Push

```bash
git checkout test
git add .
git commit -m "Your commit message"
git push origin test
```

GitHub Actions otomatik olarak:
- Testleri çalıştıracak
- Başarılıysa main'e merge edecek
- Başarısızsa issue oluşturacak

## Notlar

- İlk commit'te Calculator sınıfı yanlış implementasyonla oluşturulmuştur (toplama yerine çıkarma yapıyor)
- Bu kasıtlı olarak testlerin fail etmesi için yapılmıştır
- Düzeltme yapıldıktan sonra testler geçecek ve main branch'e merge edilecektir

