<p align="center">
  <a href="https://red4sec.com" target="_blank"><img src="https://avatars0.githubusercontent.com/u/33096324?s=200&v=4" width="200px"></a>
</p>

<h1 align="center">
CSharpLab
</h1>

<p align="center">
 CSharpLab is a C# web application that has vulnerabilities, so security researchers can practice their skills.
</p>

# CSharpApiLab

CSharpApiLab includes:
  - SQL Injection
  - XSS
  - RCE
  - LFI
  - File Upload
  - User enumeration
  - Open Redirect
  - and more

# ObfuscationSample

ObfuscationSample it's a project for practice with your favorite decompiler. We recommend [dnSpy](https://github.com/0xd4d/dnSpy)

# Usage

You can create a docker container like this:

```
docker build . -t red4sec/csharpapilab
docker run --rm -it -p 8081:80 red4sec/csharpapilab
```

After that, just go to http://localhost:8081

# Warning
Do not expose CSharpApiLAb to the Internet, your server will be compromised immediately after exposure.

# Disclaimer
We have made the purposes of the application as clear as possible and it should not be used maliciously. We have given the users warnings and taken measures to prevent them from installing CSharpApiLab on to live web servers, avoiding exposure, malicious use of the application and other uses that are not specified by us.

If your web server is compromised via an installation of CSharpApiLAb, it is not our responsibility, it is the responsibility of the person/s who uploaded and installed it.

Therefore the user must follow and understand the information provided and the right use of it. If the user chooses to use the information and application, maliciosly and in unauthorized fashion it is solely the users responsabilty. Nor the instructor, or anyone else associated with this program shall be liable or responsible for any unethical or criminal choices that the user may make and execute using the methodologies and tools described.

# License
The contents of this repository are licensed under the GNU General Public License v3.0.
