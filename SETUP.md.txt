On macOS:

- Clone GitHub Repository

- Install IDE (preferably Rider)

- Install Android Support Plugin for Rider

- Install Homebrew (/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)")
	- Check if working (brew --version)
	- Else: Set PATH (echo 'eval "$(/opt/homebrew/bin/brew shellenv)"' >> ~/.zshrc
	  source ~/.zshrc)

- Install .NET 9.0 
	- Check if working (botnet --version)
	- Else: Set PATH (echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.zshrc
	  echo 'export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools' >> ~/.zshrc
	  source ~/.zshrc)

- Install MAUI Workloads (dotnet workload install maui)
	- Check if working (dotnet workload list)
	- Install following packages if missing (dotnet workload install maui-android maui-ios maui-maccatalyst)

- Install JDK17 (brew install openjdk@17)
	- Check if working (java --version)
	- Set PATH (echo 'export JAVA_HOME=/opt/homebrew/opt/openjdk@17' >> ~/.zshrc
echo 'export PATH=$JAVA_HOME/bin:$PATH' >> ~/.zshrc
source ~/.zshrc)
	- Set JDK Path in Rider (“Preferences” → “Languages & Frameworks” → “SDKs”) = /opt/homebrew/opt/openjdk@17

- Install Android Studio (https://developer.android.com/studio?hl=de)
	- check if working (echo $ANDROID_HOME
sdkmanager --list)
	- Else: Set PATH (echo 'export ANDROID_HOME=$HOME/Library/Android/sdk' >> ~/.zshrc
echo 'export PATH=$PATH:$ANDROID_HOME/emulator:$ANDROID_HOME/tools:$ANDROID_HOME/tools/bin:$ANDROID_HOME/platform-tools' >> ~/.zshrc
source ~/.zshrc)
	- Set Path of echo $ANDROID_HOME in Rider (Android SDK)
	- Start virtual device in Android Studio

- Install MySQL Connector (brew install mysql)

- Install AWS CLI (brew install awscli)
	- Check if working (aws --version)
	




