# -*- mode: ruby -*-
# vi: set ft=ruby :

# --- Linux script ---
$linux_script = <<-SHELL
    echo "--- Starting Ubuntu setup (.NET 8) ---"
    
    sudo apt-get update
    
    sudo apt-get install -y wget apt-transport-https gpg
    
    wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    
    sudo dpkg -i packages-microsoft-prod.deb
    
    rm packages-microsoft-prod.deb
    
    sudo apt-get update
    
    echo "--- Installing .NET 8 SDK ---"
    sudo apt-get install -y dotnet-sdk-8.0
    
    echo "--- Adding BaGet private repository ---"
    dotnet nuget add source "http://baget.local/v3/index.json" -n "BaGetRepo"
    
    echo "--- Installing package BusinessAnalyticsSystem... ---"
    mkdir /opt/app_install
    cd /opt/app_install
    dotnet new console -n DummyApp
    cd DummyApp
    dotnet add package BusinessAnalyticsSystem --version 1.0.0 --source "BaGetRepo"
    
    echo "--- Deployment on Ubuntu complete ---"
SHELL

Vagrant.configure("2") do |config|

  # --- Ubuntu Linux ---
  config.vm.define "ubuntu" do |ubuntu|
    ubuntu.vm.box = "ubuntu/focal64"
    ubuntu.vm.hostname = "ubuntu-server"
    ubuntu.vm.provider "virtualbox" do |vb|
      vb.memory = "2048"
      vb.gui = true  
    end

    ubuntu.vm.provision "shell", inline: $linux_script
  end
end