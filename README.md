# Introducing .NET Multi-platform App UI (MAUI)

.NET Multi-platform App UI (.NET MAUI) is a cross-platform framework for creating native mobile and desktop apps with C# and XAML. Using .NET MAUI, you can develop apps that can run on Android, iOS, macOS, and Windows from a single shared code-base. [Read More](https://docs.microsoft.com/en-us/dotnet/maui/what-is-maui)

## Getting Started ##

* [Install .NET MAUI](https://github.com/dotnet/maui/wiki/Getting-Started)
* [MAUI documentation](https://docs.microsoft.com/en-us/dotnet/maui)
* [MAUI samples](https://github.com/dotnet/maui-samples)

## Build Status ##

| Private  | Public |
|--|--|
|  [![Build Status](https://devdiv.visualstudio.com/DevDiv/_apis/build/status/MAUI?repoName=dotnet%2Fmaui&branchName=main)](https://devdiv.visualstudio.com/DevDiv/_build/latest?definitionId=13330&repoName=dotnet%2Fmaui&branchName=main) |   [![Build Status](https://dev.azure.com/xamarin/public/_apis/build/status/MAUI-public?repoName=dotnet%2Fmaui&branchName=main)](https://dev.azure.com/xamarin/public/_build/latest?definitionId=57&repoName=dotnet%2Fmaui&branchName=main) |


## Maui.sln

### SDKS required

- Install the SDKs listed here https://github.com/dotnet/maui-samples

### Running

#### IDE

- If you have Visual Studio 16.9 P4 or newer installed you can open `Maui.sln` and run it from there.

#### .NET 6

You can run a `Cake` target to bootstrap .NET 6 in `bin\dotnet` and launch Visual Studio:

```dotnetcli
dotnet tool restore
dotnet cake --target=VS-NET6
```
_NOTES:_
- _VS Mac is not yet supported._
- _If the IDE doesn't show any android devices try unloading and reloading the `Sample.Droid-net6` project._

You can also run commands individually:
```dotnetcli
# install local tools required to build (cake, pwsh, etc..)
dotnet tool restore
# Provision .NET 6 in bin\dotnet
dotnet build src\DotNet\DotNet.csproj
# Builds Maui MSBuild tasks
.\bin\dotnet\dotnet build Microsoft.Maui.BuildTasks-net6.sln
# Builds the rest of Maui
.\bin\dotnet\dotnet build Microsoft.Maui-net6.sln
# (Windows-only) to launch Visual Studio
dotnet cake --target=VS-DOGFOOD
```

To build & run .NET 6 sample apps, you will also need to use `.\bin\dotnet\dotnet`:
```dotnetcli
.\bin\dotnet\dotnet build src\Controls\samples\Controls.Sample.Droid\Maui.Controls.Sample.Droid-net6.csproj -t:Run
.\bin\dotnet\dotnet build src\Controls\samples\Controls.Sample.iOS\Maui.Controls.Sample.iOS-net6.csproj -t:Run
```

Try out a "single project", you will need the `-f` switch to choose the platform:

```dotnetcli
.\bin\dotnet\dotnet build src\Controls\samples\Controls.Sample.SingleProject\Maui.Controls.Sample.SingleProject.csproj -t:Run -f net6.0-android
.\bin\dotnet\dotnet build src\Controls\samples\Controls.Sample.SingleProject\Maui.Controls.Sample.SingleProject.csproj -t:Run -f net6.0-ios
```

To build and run Blazor Desktop samples, check out the [Blazor Desktop](https://github.com/dotnet/maui/wiki/Blazor-Desktop) wiki topic.

## Current News

[June 17, 2021 - Announcing .NET MAUI Preview 5](https://devblogs.microsoft.com/dotnet/announcing-net-maui-preview-5/)

[May 25, 2021 - Announcing .NET MAUI Preview 4](https://devblogs.microsoft.com/dotnet/announcing-net-maui-preview-4/)

[May 24, 2021 - Azure DevOps Podcast - Interview with David Ortinau](http://azuredevopspodcast.clear-measure.com/david-ortinau-on-multi-platform-app-development-using-net-maui-episode-142)

[April 21, 2021 - WinUI Community Live Stream](https://youtu.be/SyLXctia1B0?t=777)

[April 21, 2021 - InfoQ Interview with David Ortinau](https://www.infoq.com/articles/net-maui/)

[April 9, 2021 - Announcing .NET Multi-platform App UI Preview 3](https://devblogs.microsoft.com/dotnet/announcing-net-multi-platform-app-ui-preview-3/)

[April 9, 2021 - Xamarin Podcast - .NET MAUI Q&A](https://www.xamarinpodcast.com/90)

[April 6, 2021 - On .NET with guest Maddy Leger - A Journey to .NET MAUI](https://www.youtube.com/watch?v=hoC5FIblKz8)

[April 1, 2021 - Xamarin Community Standup with Guest Jonathan Peppers discussing .NET 6 Project System updates](https://www.youtube.com/watch?v=su3ntRjEN1I)

Additional live streams and presentations:

* March 11, 2021 - [Announcing .NET 6 Preview 2](https://devblogs.microsoft.com/dotnet/announcing-net-6-preview-2/)
* March 2021 - [.NET Community Standup report](https://youtu.be/NEbRo0ltniM?t=1242) 
* February 25, 2021 - David Ortinau and Maddy Leger at .NET Conf: Focus on Windows: [The Future of Client App Development in .NET 6](https://www.youtube.com/watch?v=fPEdgXeqhE4)
* January 28, 2021 - David Ortinau at .NET Frontend Day: [A .NET MAUI Progress Report](https://youtu.be/RnyZZKjdUxk)
* September 19, 2020 - Shane Neuville at ReactiveUI Virtual Conference: [Dual Screen, .NET MAUI, and RxUI](https://www.youtube.com/watch?v=Rkz6Dkk1uWU)
* October 3, 2020 - James Clancey at Xamarin Expert Day: [.NET MAUI MVU](https://www.youtube.com/watch?v=9kwrgm_-FCk)
* October 3, 2020 - David Ortinau at Xamarin Expert Day: [Introducing .NET MAUI](https://youtu.be/qbHO8J3bId0)
* December 2, 2020 - Javier Suarez Ruiz at MonkeyConf: [.NET MAUI Handlers](https://youtu.be/TBMauxRGkiI) (Spanish)

## FAQs

Do you have questions? Do not worry, we have prepared a complete [FAQ](https://github.com/dotnet/maui/wiki/FAQs) answering the most common questions.

## How to Engage, Contribute, and Give Feedback

Some of the best ways to [contribute](./.github/CONTRIBUTING.md) are to try things out, file issues, join in design conversations,
and make pull-requests. Proposals for changes specific to MAUI can be found [here for discussion](https://github.com/dotnet/maui/issues).

See [CONTRIBUTING](./.github/CONTRIBUTING.md)

## Code of conduct

See [CODE-OF-CONDUCT](./.github/CODE_OF_CONDUCT.md)
