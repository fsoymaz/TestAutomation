# Jenkins Kurulum ve Yapılandırma Rehberi

Bu proje, CI/CD süreçleri için Jenkins kullanacak şekilde yapılandırılmıştır. Aşağıdaki adımları takiperek kendi bilgisayarınızda bir Jenkins sunucusu kurabilir ve bu projeyi test edebilirsiniz.

## 1. Ön Hazırlık (Docker)

Jenkins'i en kolay ve temiz şekilde çalıştırmak için **Docker** kullanacağız. Eğer bilgisayarınızda Docker yüklü değilse, [Docker Desktop](https://www.docker.com/products/docker-desktop/) uygulamasını indirip kurun.

## 2. Jenkins'i Çalıştırma

Terminalde şu komutu çalıştırarak Jenkins'i başlatın. Bu komut, Docker içinde çalışan Jenkins'in kendi içinde de Docker komutlarını çalıştırabilmesini sağlar (Docker-in-Docker).

```bash
docker run -u root \
  --rm \
  -d \
  -p 8080:8080 \
  -p 50000:50000 \
  -v jenkins-data:/var/jenkins_home \
  -v /var/run/docker.sock:/var/run/docker.sock \
  jenkinsci/blueocean
```

## 3. İlk Kurulum

1.  Tarayıcınızda `http://localhost:8080` adresine gidin.
2.  Sizden bir **Administrator Password** isteyecektir. Bu şifreyi öğrenmek için terminalde şu komutu çalıştırın:
    ```bash
    docker exec -it <CONTAINER_ID> cat /var/jenkins_home/secrets/initialAdminPassword
    ```
    *(Not: `<CONTAINER_ID>` kısmını `docker ps` komutuyla bulabilirsiniz.)*
3.  **"Install suggested plugins"** seçeneğine tıklayın ve kurulumun bitmesini bekleyin.
4.  Bir yönetici hesabı oluşturun ve kurulumu tamamlayın.

## 4. Gerekli Eklentilerin (Plugins) Kurulumu

Projemizdeki `Jenkinsfile` bazı özel eklentiler kullanıyor. Bunları yüklemek için:

1.  Ana sayfadan **Manage Jenkins** -> **Plugins** menüsüne gidin.
2.  **Available plugins** sekmesine tıklayın.
3.  Şu eklentileri aratıp yükleyin (eğer yüklü değilse):
    *   **Docker Pipeline**
    *   **MSTest** (Test sonuçlarını okumak için)
4.  Yükleme bittikten sonra Jenkins'i yeniden başlatmanız gerekebilir (`http://localhost:8080/restart`).

## 5. Projeyi Jenkins'e Ekleme

1.  Ana sayfada **New Item**'a tıklayın.
2.  Bir isim verin (örneğin: `TestAutomation`) ve **Pipeline** seçeneğini seçip OK deyin.
3.  Açılan ayar sayfasında en alta inin, **Pipeline** başlığı altındaki **Definition** kısmını `Pipeline script from SCM` olarak değiştirin.
4.  **SCM** olarak `Git` seçin.
5.  **Repository URL** kısmına projenizin bulunduğu klasör yolunu yazın (yerel dosya yolu çalışmayabilir, en iyisi projeyi GitHub'a yükleyip oradaki linki vermektir).
    *   *Alternatif:* Eğer yerel çalışacaksanız, proje klasörünüzü Docker'a volume olarak bağlamanız gerekir, bu biraz daha karmaşıktır. **En kolayı projeyi GitHub'a pushlayıp oradan çekmektir.**
6.  **Script Path** kısmında `Jenkinsfile` yazdığından emin olun.
7.  **Save** diyerek kaydedin.

## 6. Çalıştırma

Proje sayfasında sol taraftaki **Build Now** butonuna basın. Jenkins:
1.  Kodu çekecek.
2.  Docker içinde .NET 8 ortamını hazırlayacak.
3.  Testleri çalıştıracak.
4.  Sonuçları raporlayacaktır.
