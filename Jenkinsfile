pipeline {
    agent any

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_NOLOGO = 'true'
        // Dotnet'i yerel klasore kuracagiz
        DOTNET_ROOT = "${env.WORKSPACE}/.dotnet"
        PATH = "${env.WORKSPACE}/.dotnet:${env.PATH}"
    }

    stages {
        stage('Setup .NET') {
            steps {
                sh '''
                    # .NET yukleme scriptini indir
                    curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh
                    chmod +x dotnet-install.sh
                    
                    # .NET 8 SDK'yi kur
                    ./dotnet-install.sh --channel 8.0 --install-dir .dotnet
                '''
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet --version'
                sh 'dotnet restore'
                sh 'dotnet build --no-restore --configuration Release'
                sh 'dotnet test --no-build --configuration Release --logger "trx;LogFileName=test-results.trx"'
            }
            post {
                always {
                    // Requires "MSTest" plugin in Jenkins
                    mstest testResultsFile: '**/test-results.trx', keepLongStdio: true
                    
                    // Ayrica dosyayi da saklayalim
                    archiveArtifacts artifacts: '**/test-results.trx', allowEmptyArchive: true
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

