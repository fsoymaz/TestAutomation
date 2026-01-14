pipeline {
    agent any

    environment {
        // Docker ve diger araclarin bulunabilmesi icin PATH'i guncelliyoruz
        PATH = "/usr/local/bin:/opt/homebrew/bin:/usr/bin:/bin:/usr/sbin:/sbin:${env.PATH}"
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_NOLOGO = 'true'
    }

    stages {
        stage('Test') {
            steps {
                // Docker konteyneri icinde manuel olarak komutlari calistiriyoruz
                sh '''
                    docker run --rm -v "${WORKSPACE}:/app" -w /app -u root:root mcr.microsoft.com/dotnet/sdk:8.0 /bin/sh -c "
                        dotnet --version &&
                        dotnet restore &&
                        dotnet build --no-restore --configuration Release &&
                        dotnet test --no-build --configuration Release --logger 'trx;LogFileName=test-results.trx' &&
                        dotnet tool install --global dotnet-reportgenerator-globaltool || true &&
                        export PATH=\"\$PATH:/root/.dotnet/tools\" &&
                        reportgenerator -reports:**/test-results.trx -targetdir:TestReport -reporttypes:Html
                    "
                '''
            }
            post {
                always {
                    // Requires "MSTest" plugin in Jenkins
                    mstest testResultsFile: '**/test-results.trx', keepLongStdio: true
                    
                    // TRX dosyasini ve yeni olusturdugumuz HTML rapor klasorunu arsivle
                    archiveArtifacts artifacts: '**/test-results.trx, TestReport/**/*', allowEmptyArchive: true
                }
            }
        }

        stage('Deploy to Main') {
            when {
                expression {
                    // Sadece test branch'inde ve testler basariliysa calisir
                    env.BRANCH_NAME == 'test' || env.GIT_BRANCH == 'origin/test'
                }
            }
            steps {
                script {
                    withCredentials([usernamePassword(credentialsId: '31', usernameVariable: 'GIT_USER', passwordVariable: 'GIT_PASS')]) {
                        sh """
                            # Git kimlik bilgilerini ayarla
                            git config user.email "jenkins@testautomation.com"
                            git config user.name "Jenkins CI"
                            
                            # Remote URL'yi guncelle (sifreli erisim icin)
                            git remote set-url origin https://${GIT_USER}:${GIT_PASS}@github.com/fsoymaz/TestAutomation.git
                            
                            # Guncel durumu cek
                            git fetch origin
                            
                            # Main branch'e gec ve guncelle
                            git checkout main || git checkout -b main origin/main
                            git pull origin main
                            
                            # Test branch'ini merge et (commit yapmadan, duzenleme icin)
                            git merge origin/test --no-commit --no-ff
                            
                            # Test projesini ve CI dosyalarini main branch'ten sil
                            git rm -r Calculator.Tests || true
                            git rm Jenkinsfile || true
                            git rm README_JENKINS.md || true
                            git rm .gitignore || true
                            
                            # Solution dosyasindan Test projesini cikarmak gerekebilir ama simdilik dosyayi silmek yeterli
                            # (Solution dosyasi bozuk kalabilir, idealde `dotnet sln remove` yapmak lazim)
                            dotnet sln TestAutomation.sln remove Calculator.Tests/Calculator.Tests.csproj || true

                            # Degisiklikleri commit et
                            git commit -m "Deploy to main: Removed test files"
                            
                            # Degisiklikleri gonder
                            git push origin main
                        """
                    }
                }
            }
        }
    }

    post {
        always {
            cleanWs()
        }
    }

