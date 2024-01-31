# How it work:
```git
git clone https://github.com/softbery-org/.softbery <project dir>
```
> For counting bulding version with Softbery Versioner add to your project `.csproj` file this code:
> 
```xml
    <Target Name="PostBuild" AfterTargets="Build">
        <Exec Command=".softbery\bin\Debug\net7.0\softbery.exe" />
    </Target>
```






> When you build your project. Application will be grow up your version.
> 
> Enjoy using `Softbery Versioner by Paweł Tobis`.