# -*- mode: ruby -*-
# vi: set ft=ruby :

# Script for Ubuntu/Debian 
$install_script = <<-SHELL
    echo "--- Starting Setup (.NET 8) ---"
    
    sudo apt-get update
    sudo apt-get install -y wget apt-transport-https gpg
    
    wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
    
    sudo apt-get update
    
    echo "--- Installing .NET 8 SDK ---"
    sudo apt-get install -y dotnet-sdk-8.0
    
    echo "--- Installing App from local package ---"
    
    dotnet tool uninstall -g BusinessAnalyticsSystem 2>/dev/null || true
    
    dotnet tool install --global BusinessAnalyticsSystem --add-source "/vagrant/packages" --version 1.0.9
    
    export PATH="$PATH:$HOME/.dotnet/tools"
    
    echo "--- Starting App ---"
    nohup bas-app --urls "http://0.0.0.0:8080" > /home/vagrant/app.log 2>&1 &
SHELL

Vagrant.configure("2") do |config|

  # Ubuntu 
  config.vm.define "ubuntu" do |ubuntu|
    ubuntu.vm.box = "ubuntu/focal64"
    ubuntu.vm.hostname = "ubuntu-server"
    
    ubuntu.vm.network "forwarded_port", guest: 8080, host: 8080
    ubuntu.vm.synced_folder "./packages", "/vagrant/packages"

    ubuntu.vm.provider "virtualbox" do |vb|
      vb.memory = "2048"
      vb.cpus = 2
      vb.gui = true
    end

    ubuntu.vm.provision "shell", inline: $install_script, run: "always"
  end

  # Debian
  config.vm.define "debian" do |debian|
    debian.vm.box = "debian/bullseye64" # Debian 11
    debian.vm.hostname = "debian-server"
    
    debian.vm.network "forwarded_port", guest: 8080, host: 8081
    debian.vm.synced_folder "./packages", "/vagrant/packages"

    debian.vm.provider "virtualbox" do |vb|
      vb.memory = "2048"
      vb.cpus = 2
      vb.gui = true
    end

    debian.vm.provision "shell", inline: $install_script, run: "always"
  end

end