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
                        dotnet test --no-build --configuration Release --logger 'trx;LogFileName=test-results.trx' --collect:'XPlat Code Coverage' &&
                        dotnet tool install --global dotnet-reportgenerator-globaltool || true &&
                        export PATH=\"\$PATH:/root/.dotnet/tools\" &&
                        reportgenerator -reports:**/coverage.cobertura.xml -targetdir:TestReport -reporttypes:Html || echo 'INFO: No coverage report generated'
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
                            
                            # Merge test branch - log if there are conflicts
                            git merge origin/test --no-commit --no-ff || echo "WARNING: Merge had conflicts or no changes to merge"
                            
                            # Remove test-related files from production (ignore if not found)
                            git rm -rf Calculator.Tests 2>/dev/null || echo "INFO: Calculator.Tests not found or already removed"
                            git rm -f Jenkinsfile 2>/dev/null || echo "INFO: Jenkinsfile not found or already removed"
                            git rm -f README_JENKINS.md 2>/dev/null || echo "INFO: README_JENKINS.md not found or already removed"
                            # .gitignore is intentionally kept to prevent build artifacts from being committed
                            
                            # Update solution file (ignore if already updated)
                            dotnet sln TestAutomation.sln remove Calculator.Tests/Calculator.Tests.csproj 2>/dev/null || echo "INFO: Project reference already removed or not found"

                            git commit -m "Deploy to main: Removed test files" || echo "INFO: No changes to commit"
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
