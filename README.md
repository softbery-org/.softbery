# How it work

```git
git clone https://github.com/softbery-org/.softbery <project dir>
```

> For counting bulding version with Softbery Verbery add to your project `.csproj` file this code:
>
```xml
    <Target Name="PostBuild" AfterTargets="Compile">
    <Message Importance="high" Text="PostBuild: $(TargetPath)" />
    <Message Importance="high" Text="PostBuild: $(TargetDir)" />
    <Message Importance="high" Text="PostBuild: $(TargetName)" />
    <Message Importance="high" Text="PostBuild: $(TargetFrameworkVersion)" />
    <Message Importance="high" Text="PostBuild: $(TargetFramework)" />
    <Exec Command="..\.softbery\bin\Debug\net8.0-windows7.0\softbery.exe" />
  </Target>
```

> This code will run `softbery.exe` after build your project. You can change path to `softbery.exe` if you want.
> You can also add this code to your `.csproj` file:
```cmd
cd <project dir>
./bin/Debug/net8.0-windows7.0/softbery.exe --help
```
> When you build your project. Application will be grow up your version.
>
> Enjoy using `Verbery by Paweł Tobis`.
