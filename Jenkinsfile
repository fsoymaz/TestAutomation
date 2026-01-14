pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            // Docker konteynerinin root kullanicisi olarak calismasini saglar (izin sorunlarini onler)
            args '-u root:root'
        }
    }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_NOLOGO = 'true'
        // Docker icinde tool path'i
        PATH = "$PATH:/root/.dotnet/tools"
    }

    stages {
        stage('Test') {
            steps {
                sh 'dotnet --version'
                sh 'dotnet restore'
                sh 'dotnet build --no-restore --configuration Release'
                sh 'dotnet test --no-build --configuration Release --logger "trx;LogFileName=test-results.trx"'
                
                // HTML Rapor Olusturucu (ReportGenerator) Kurulumu ve Calistirilmasi
                sh 'dotnet tool install --global dotnet-reportgenerator-globaltool || true'
                sh 'reportgenerator -reports:**/test-results.trx -targetdir:TestReport -reporttypes:Html'
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
}

