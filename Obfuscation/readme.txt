
When the PlatformsIdentity plugin project is updated, the following steps need to be carried out to incorporate the new QSR.NVivo.Plugins.PlatformsIdentity.dll into projects using this dll.

- Build PlatformsIdentity.Interface in Release configuration
- Build PlatformsIdentity in Release configuration, using the Any CPU platform, ensuring that it is signed using QSRKey.snk

- Go to PlatformsIdentity\bin\Release and copy QSR.NVivo.Plugins.PlatformsIdentity.dll and QSR.NVivo.Plugins.PlatformsIdentity.Interface.dll into the directory containing this text file


- Delete the contents of the "Dotfuscated" directory

- In PlatformsIdentityObfuscatorConfig.xml file add required paths to <Loadpaths> section if not added. This includes references assemblies like RestSharp.

- Run obfuscate_and_resign.bat

- Pack the new Dotfuscated\QSR.NVivo.Plugins.PlatformsIdentity.dll and Dotfuscated\PlatformsIdentityDotfuscatorMap.xml into MyGet package 
- the package should copy them into the designated project location e.g. $/../Plugins/PlatformsIdentity

-- Update the installer project to install the pacakge.

