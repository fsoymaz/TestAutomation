pipeline {
    agent any

    tools {
        dotnet 'dotnet-sdk-8.0'
    }

    environment {
        DOTNET_VERSION = '8.0'
        PROJECT_PATH = 'Calculator'
        TEST_PATH = 'Calculator.Tests'
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
                script {
                    try {
                        sh 'dotnet test --no-build --configuration Release --verbosity normal --logger "trx;LogFileName=test-results.trx" --logger "console;verbosity=detailed"'
                    } catch (Exception e) {
                        // Test failures will be logged
                        echo "Tests failed: ${e.getMessage()}"
                        currentBuild.result = 'UNSTABLE'
                    }
                }
            }
            post {
                always {
                    // Publish test results
                    publishTestResults(
                        testResultsPattern: '**/test-results.trx',
                        testResultsFormat: 'MSTest',
                        allowEmptyResults: true
                    )
                    
                    // Archive test results
                    archiveArtifacts artifacts: '**/test-results.trx', allowEmptyArchive: true
                    
                    // Publish HTML report if available
                    publishHTML([
                        reportDir: 'TestResults',
                        reportFiles: 'index.html',
                        reportName: 'Test Report',
                        keepAll: true
                    ])
                }
                failure {
                    // Log failure details
                    echo "Test execution failed. Check test results for details."
                    script {
                        def testLog = sh(
                            script: 'cat **/test-results.trx | head -1000',
                            returnStdout: true
                        ).trim()
                        
                        echo """
                        ============================================
                        TEST FAILURE DETAILS
                        ============================================
                        ${testLog}
                        ============================================
                        """
                    }
                }
            }
        }

        stage('Deploy to Main') {
            when {
                expression { 
                    env.BRANCH_NAME == 'test' && currentBuild.result == 'SUCCESS'
                }
            }
            steps {
                script {
                    // Merge test branch to main (requires appropriate permissions)
                    sh '''
                        git config user.name "Jenkins"
                        git config user.email "jenkins@example.com"
                        git checkout main
                        git merge test --no-edit
                        git push origin main
                    '''
                }
            }
        }
    }

    post {
        always {
            // Clean workspace
            cleanWs()
        }
        failure {
            echo "Pipeline failed. Check logs for details."
        }
        unstable {
            echo "Pipeline is unstable due to test failures."
        }
    }
}

