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

        stage('Promote to Test') {
            when {
                // Branch isminde 'feature/' geciyorsa calis
                expression {
                    return env.GIT_BRANCH =~ /.*feature\/.*/
                }
            }
            steps {
                script {
                    // Kullanicidan onay bekle (GitLab'deki Play butonu gibi)
                    input message: 'Test ortamına (test branch) merge edilsin mi?', ok: 'Evet, Merge Et'
                    
                    withCredentials([usernamePassword(credentialsId: '31', usernameVariable: 'GIT_USER', passwordVariable: 'GIT_PASS')]) {
                        sh """
                            git config user.email "jenkins@testautomation.com"
                            git config user.name "Jenkins CI"
                            git remote set-url origin https://${GIT_USER}:${GIT_PASS}@github.com/fsoymaz/TestAutomation.git
                            
                            git fetch origin
                            
                            # Test branch'ine gec
                            git checkout test || git checkout -b test origin/test
                            git pull origin test
                            
                            # Mevcut feature branch'ini test'e merge et
                            # GIT_BRANCH 'origin/feature/...' formatinda olabilir, sadece ismini alalim
                            # Basitce merge origin/feature/... diyebiliriz
                            git merge ${env.GIT_BRANCH}
                            
                            # Degisiklikleri test branch'ine gonder
                            git push origin test
                        """
                    }
                }
            }
        }

        stage('Deploy to Main') {
            when {
                // Branch isminde 'test' geciyorsa calis
                expression {
                    return env.GIT_BRANCH =~ /.*test/
                }
            }
            steps {
                script {
                    // Kullanicidan onay bekle
                    input message: 'Canlı ortama (main branch) deploy edilsin mi?', ok: 'Evet, Deploy Et'
                    
                    withCredentials([usernamePassword(credentialsId: '31', usernameVariable: 'GIT_USER', passwordVariable: 'GIT_PASS')]) {
                        sh """
                            git config user.email "jenkins@testautomation.com"
                            git config user.name "Jenkins CI"
                            git remote set-url origin https://${GIT_USER}:${GIT_PASS}@github.com/fsoymaz/TestAutomation.git
                            
                            git fetch origin
                            git checkout main || git checkout -b main origin/main
                            git pull origin main
                            
                            git merge origin/test --no-commit --no-ff || true
                            
                            git rm -rf Calculator.Tests || true
                            git rm -f Jenkinsfile || true
                            git rm -f README_JENKINS.md || true
                            git rm -f .gitignore || true
                            
                            dotnet sln TestAutomation.sln remove Calculator.Tests/Calculator.Tests.csproj || true

                            git commit -m "Deploy to main: Removed test files" || echo "No changes to commit"
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
// Trigger Jenkins
