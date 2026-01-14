pipeline {
    agent any

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_NOLOGO = 'true'
    }

    stages {
        stage('Test') {
            steps {
                // Docker Pipeline eklentisi olmadigi icin manuel docker run kullaniyoruz
                sh '''
                    docker run --rm -v $(pwd):/app -w /app mcr.microsoft.com/dotnet/sdk:8.0 /bin/sh -c "
                        dotnet restore &&
                        dotnet build --no-restore --configuration Release &&
                        dotnet test --no-build --configuration Release --logger 'trx;LogFileName=test-results.trx'
                    "
                '''
            }
            post {
                always {
                    // Requires "MSTest" or "JUnit" plugin in Jenkins
                    mstest testResultsFile: '**/test-results.trx', keepLongStdio: true
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
                            
                            # Test branch'ini merge et
                            git merge origin/test --no-edit
                            
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

