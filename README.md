# DavidLievrouw.MSBuildTasks

### General
| Spec | Value | 
| --------- | --------- | 
| Package | DavidLievrouw.MSBuildTasks | 
| Initial release date | TBA | 

### Tasks
* GetVersionParts
  * This task accepts a Major.Minor.Build.Revision version number string, and splits it up into four integer properties. 
    * Input properties
      * [VersionNumber] - A valid version number string, that consists of (major).(minor).(build).(revision). Internally System.Version.Parse(string) is used to validate the input.
    * Output properties
      * [MajorVersion] - An integer that contains the Major version number.
      * [MinorVersion] - An integer that contains the Minor version number.
      * [BuildVersion] - An integer that contains the Build version number.
      * [RevisionVersion] - An integer that contains the Revision version number.
* EncryptForLocalMachineScope
  * This task encrypts the specified input string, so that it can only be decrypted on the local machine.
    * Input properties
      * [StringToEncrypt] - The string to encrypt.
      * [Purposes] - An optional array of strings. These represent purposes for the data. The same purposes have to be passed to decrypt the data, later on. Empty, whitespace or null purposes are not used.
    * Output properties
      * [EncryptedString] - The BASE64 string that represents the encrypted data.

### Installation instructions
* Get the latest version at [Nuget.org](https://www.nuget.org/packages/DavidLievrouw.MSBuildTasks/).
* Install by executing:
```sh
PM> Install-Package DavidLievrouw.MSBuildTasks
```
* Include the .targets file in your MSBuild scripts:
```xml
<Import Project="$(DLMSBuildTasksPath)\DLMSBuildTasks.targets"/>
```
* Use any of the included tasks in your MSBuild scripts, e.g.:
```xml
<Target Name="MyTarget">
  <PropertyGroup>
    <VersionNumber>12.2.1.31255</VersionNumber>
  </PropertyGroup>
  <GetVersionParts VersionNumber="$(VersionNumber)">
    <Output TaskParameter="MajorVersion" PropertyName="Major" />
    <Output TaskParameter="MinorVersion" PropertyName="Minor" />
    <Output TaskParameter="BuildVersion" PropertyName="Build" />
    <Output TaskParameter="RevisionVersion" PropertyName="Revision" />
  </GetVersionParts>
  <Message Text="$(VersionNumber) consists of $(Major)-$(Minor)-$(Build)-$(Revision)." />
</Target>
```

### License
MIT
