solution_file = "PageTypeBuilder.sln"
configuration = "release"
mspec_path = "packages/Machine.Specifications-Signed.0.4.13.0/tools/mspec.exe"
short_release_name = env("releasename")
release_name = "PageTypeBuilder-" + short_release_name
release_dir = "build"
release_versiondir = release_dir + "/" + release_name

target default, (compile):
  pass

target release, (compile, generate_specs, deploy, package):
  pass

desc "Compiles the solution"
target compile:
  msbuild(file: solution_file, configuration: configuration)

desc "Build NuGet packages"
target nuget2:

  exec("Libraries/NuGet/NuGet.exe","pack PageTypeBuilder\\PageTypeBuilder.csproj /o nugetpackages")
  exec("Libraries/NuGet/NuGet.exe","pack PageTypeBuilder.Activation.StructureMap\\PageTypeBuilder.Activation.StructureMap.csproj /o nugetpackages")


desc "Build NuGet packages"
target nuget:

  with FileList("PageTypeBuilder/bin/${configuration}"):
    .Include("PageTypeBuilder.dll")
    .ForEach def(file):
      file.CopyToDirectory("nugetpackages/" + release_name + "/lib/net20")

  with FileList("PageTypeBuilder.Activation.StructureMap/bin/${configuration}"):
    .Include("PageTypeBuilder.Activation.StructureMap.dll")
    .ForEach def(file):
      file.CopyToDirectory("nugetpackages/PageTypeBuilder.Activation.StructureMap" + env("releasename") + "/lib/net20")

  exec("Libraries/NuGet/NuGet.exe","pack nugetpackages\\PageTypeBuilder.1.3.0.0\\PageTypeBuilder.nuspec /o nugetpackages")
  exec("Libraries/NuGet/NuGet.exe","pack nugetpackages\\PageTypeBuilder.Activation.StructureMap.1.3.0.0\\PageTypeBuilder.Activation.StructureMap.nuspec /o nugetpackages")

desc "Copies the binaries to the 'build' directory"
target deploy:
  print "Copying to build dir"

  with FileList("PageTypeBuilder/bin/${configuration}"):
    .Include("*.{dll}")
    .ForEach def(file):
      file.CopyToDirectory(release_versiondir)

  with FileList("PageTypeBuilder.Activation.StructureMap/bin/${configuration}"):
    .Include("*.{dll}")
    .ForEach def(file):
      file.CopyToDirectory(release_versiondir)

desc "Generating specifications"
target generate_specs:
  mkdir(release_versiondir)
  exec(mspec_path, "--html ${release_versiondir}/specifications.html --silent PageTypeBuilder.Specs/bin/${configuration}/PageTypeBuilder.Specs.dll")
  rmdir("${release_versiondir}/resources")

desc "Creates zip package"
target package:
  zip("build", release_dir + "/" + release_name  +'.zip')
  exec("git archive master --format=zip > ${release_dir}/${release_name}-src.zip")

