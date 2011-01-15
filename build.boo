solution_file = "PageTypeBuilder.sln"
configuration = "release"

target default, (compile, nuget):
  pass

desc "Compiles the solution"
target compile:
  msbuild(file: solution_file, configuration: configuration)

//desc "Run specifications"
//target specs:
//  exec("Lib/Mspec/mspec.exe","Progressive.EPiServer.Templates.Installation.Specs/bin/${configuration}/Progressive.EPiServer.Templates.Installation.Specs.dll --html Specs/Specs.html")

desc "Build NuGet packages"
target nuget:

  with FileList("PageTypeBuilder/bin/${configuration}"):
    .Include("PageTypeBuilder.dll")
    .ForEach def(file):
      file.CopyToDirectory("nugetpackages/PageTypeBuilder.1.3.0.0/lib/net20")

  with FileList("PageTypeBuilder.Activation.StructureMap/bin/${configuration}"):
    .Include("PageTypeBuilder.Activation.StructureMap.dll")
    .ForEach def(file):
      file.CopyToDirectory("nugetpackages/PageTypeBuilder.Activation.StructureMap.1.3.0.0/lib/net20")

  exec("Libraries/NuGet/NuGet.exe","pack nugetpackages\\PageTypeBuilder.1.3.0.0\\PageTypeBuilder.nuspec /o nugetpackages")
  exec("Libraries/NuGet/NuGet.exe","pack nugetpackages\\PageTypeBuilder.Activation.StructureMap.1.3.0.0\\PageTypeBuilder.Activation.StructureMap.nuspec /o nugetpackages")