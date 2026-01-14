pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            args '-u root' // Run as root to avoid permission issues in some setups
        }
    }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_NOLOGO = 'true'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build --no-restore --configuration Release'
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test --no-build --configuration Release --logger "trx;LogFileName=test-results.trx"'
            }
            post {
                always {
                    // Requires "MSTest" or "JUnit" plugin in Jenkins
                    mstest testResultsFile: '**/test-results.trx', keepLongStdio: true
                }
            }
        }

        stage('Deploy') {
            steps {
                script {
                    // Credential ID '31' user tarafından oluşturuldu
                    withCredentials([usernamePassword(credentialsId: '31', usernameVariable: 'GIT_USER', passwordVariable: 'GIT_PASS')]) {
                        sh """
                            echo "Deploying with user: \${GIT_USER}"
                            // Buraya git tag/push komutları gelebilir
                            // git tag -a "v\${BUILD_NUMBER}" -m "Jenkins Build #\${BUILD_NUMBER}"
                            // git push https://\${GIT_USER}:\${GIT_PASS}@github.com/fsoymaz/TestAutomation.git --tags
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

